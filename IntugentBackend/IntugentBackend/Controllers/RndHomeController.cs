using IntugentBackend.Models;
using IntugentBackend.Services;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Rnd;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RndHomeController : ControllerBase
    {
        private readonly CDefualts _cDefualts;
        private readonly CLists _cLists;
        private readonly RNDHome _rndHome;
        private readonly Cbfile _cbfile;
        private readonly ILogger<RndHomeController> _logger;

        public RndHomeController(
            CDefualts cDefualts,
            CLists cLists,
            RNDHome rndHome,
            Cbfile cbfile,
            ILogger<RndHomeController> logger)
        {
            _cDefualts = cDefualts;
            _cLists = cLists;
            _rndHome = rndHome;
            _cbfile = cbfile;
            _logger = logger;
        }

        /// <summary>
        /// Get all filter dropdown data and default values for R&D Home page
        /// </summary>
        [HttpGet("filters")]
        public IActionResult GetFilters()
        {
            try
            {
                if (_cDefualts == null || _cLists == null)
                {
                    return BadRequest(new ApiResponse<RndFiltersDto>
                    {
                        Success = false,
                        Error = "Session not initialized. Call /api/Session/begin first."
                    });
                }

                // Initialize RNDHome if not already done
                _rndHome.bInit = false;

                // Setup lists (same as old OnGet/Startup)
                _cLists.dvLists = _cLists.dtLists.DefaultView;

                // Testing Status
                _cLists.dvLists.RowFilter = "sList = 'Testing Status RND'";
                _cLists.dvTestingStatRND = _cLists.dvLists.ToTable().DefaultView;

                // Study Type
                _cLists.dvLists.RowFilter = "sList = 'RND Study Type'";
                _cLists.dvRunTypeRND2 = _cLists.dvLists.ToTable().DefaultView;

                // Products
                var products = new List<FilterOptionDto>();
                if (_cLists.dvComProdAll != null)
                {
                    foreach (DataRowView row in _cLists.dvComProdAll)
                    {
                        products.Add(new FilterOptionDto
                        {
                            Code = row["Product Code"]?.ToString(),
                            Name = row["Product"]?.ToString() ?? string.Empty
                        });
                    }
                }

                // Helper to extract lists
                List<FilterOptionDto> ExtractList(DataView dv)
                {
                    var result = new List<FilterOptionDto>();
                    if (dv == null) return result;
                    foreach (DataRowView row in dv)
                    {
                        result.Add(new FilterOptionDto
                        {
                            Id = Convert.ToInt32(row["ID"]),
                            Name = row["sName"]?.ToString() ?? string.Empty
                        });
                    }
                    return result;
                }

                // Run Startup logic to get defaults from drEmployee
                Startup();

                var filters = new RndFiltersDto
                {
                    Products = products,
                    TestingStatus = ExtractList(_cLists.dvTestingStatRND),
                    StudyTypes = ExtractList(_cLists.dvRunTypeRND2),
                    Location = _cDefualts.sLocation,
                    DefaultProductCode = _cLists.drEmployee?["Rnd Product Code"] == DBNull.Value
                        ? _cDefualts.sProdRNDAll
                        : (_cLists.drEmployee?["Rnd Product Code"]?.ToString() ?? string.Empty),
                    DefaultTestingStatusId = _cLists.drEmployee?["RndIDTestingStatus"] == DBNull.Value
                        ? _cDefualts.iRNDTestingStat
                        : Convert.ToInt32(_cLists.drEmployee?["RndIDTestingStatus"]),
                    DefaultStudyTypeId = _cLists.drEmployee?["RndIDStudyType"] == DBNull.Value
                        ? _cDefualts.iMfgRunType
                        : Convert.ToInt32(_cLists.drEmployee?["RndIDStudyType"]),
                    DefaultDateFrom = _cLists.drEmployee?["RndDate1"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(_cLists.drEmployee?["RndDate1"]),
                    DefaultDateTo = _cLists.drEmployee?["RndDate2"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(_cLists.drEmployee?["RndDate2"]),
                    DefaultNameSearch = _cLists.drEmployee?["RNDNameSearch"] == DBNull.Value
                        ? null
                        : _cLists.drEmployee?["RNDNameSearch"]?.ToString()
                };

                return Ok(new ApiResponse<RndFiltersDto>
                {
                    Success = true,
                    Data = filters
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting R&D filters");
                return StatusCode(500, new ApiResponse<RndFiltersDto>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Search R&D database with filters
        /// </summary>
        [HttpPost("search")]
        public IActionResult Search([FromBody] SearchRndRequest request)
        {
            try
            {
                if (_rndHome == null)
                {
                    return BadRequest(new ApiResponse<RndSearchResultDto>
                    {
                        Success = false,
                        Error = "RNDHome not initialized."
                    });
                }

                // Apply filters to drEmployee (same as old OnPostGSearchDataSets_Click)
                _cLists.drEmployee["RndDate1"] =
                    request.DateFrom == null ? DBNull.Value : request.DateFrom;

                _cLists.drEmployee["RndDate2"] =
                    request.DateTo == null ? DBNull.Value : request.DateTo;

                _cLists.drEmployee["Rnd Product Code"] =
                    string.IsNullOrEmpty(request.ProductCode) ? DBNull.Value : request.ProductCode;

                _cLists.drEmployee["RndIDTestingStatus"] =
                    request.TestingStatusId <= 0 ? DBNull.Value : request.TestingStatusId;

                _cLists.drEmployee["RndIDStudyType"] =
                    request.StudyTypeId <= 0 ? DBNull.Value : request.StudyTypeId;

                _cLists.drEmployee["RNDNameSearch"] =
                    string.IsNullOrEmpty(request.NameSearch) ? DBNull.Value : request.NameSearch;

                // Perform search
                bool found = SearchRNDDB();

                if (found && _rndHome.dt.Rows.Count > 0)
                {
                    _rndHome.indSet = 0;
                    _rndHome.IdSet = (int)_rndHome.dt.Rows[0]["ID"];
                    _cLists.UpdateEmployee();
                }
                else
                {
                    _rndHome.EnableRNDPages(false);
                }

                // Build response
                var result = BuildSearchResult();

                return Ok(new ApiResponse<RndSearchResultDto>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching R&D DB");
                return StatusCode(500, new ApiResponse<RndSearchResultDto>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get current search results
        /// </summary>
        [HttpGet("results")]
        public IActionResult GetResults()
        {
            try
            {
                if (_rndHome?.dt == null)
                {
                    return Ok(new ApiResponse<RndSearchResultDto>
                    {
                        Success = true,
                        Data = new RndSearchResultDto()
                    });
                }

                var result = BuildSearchResult();
                return Ok(new ApiResponse<RndSearchResultDto>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting R&D results");
                return StatusCode(500, new ApiResponse<RndSearchResultDto>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Select a dataset from search results
        /// </summary>
        [HttpPost("select-dataset")]
        public IActionResult SelectDataset([FromBody] SelectRndDatasetRequest request)
        {
            try
            {
                int iOldSet = _rndHome.IdSet;
                if (request.SelectedIndex >= 0)
                {
                    _rndHome.IdSet = request.DatasetId;
                    if (_rndHome.GetDataSet(request.DatasetId))
                    {
                        _rndHome.indSet = request.SelectedIndex;
                        _cLists.drEmployee["RndIDSelected"] = _rndHome.IdSet;
                        _cLists.UpdateEmployee();

                        return Ok(new ApiResponse<object>
                        {
                            Success = true,
                            Data = new { message = $"Dataset {request.DatasetId} selected." }
                        });
                    }
                    else
                    {
                        _rndHome.IdSet = iOldSet;
                        return Ok(new ApiResponse<object>
                        {
                            Success = false,
                            Error = "Failed to load dataset."
                        });
                    }
                }
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Error = "Invalid selection index."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error selecting R&D dataset");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Create a new R&D dataset
        /// </summary>
        [HttpPost("new-dataset")]
        public IActionResult CreateNewDataset()
        {
            try
            {
                int idOld = _rndHome.IdSet;

                if (_rndHome.GetNewDataset())
                {
                    _rndHome.dr = _rndHome.dt.NewRow();
                    _rndHome.dr["ID"] = _rndHome.IdSet;
                    _rndHome.dr["DateDSCreated"] = _rndHome.drS["DateDSCreated"];
                    _rndHome.dt.Rows.InsertAt(_rndHome.dr, 0);

                    var data = _rndHome.dt.AsEnumerable()
                        .Select(row => row.ItemArray.Select(item => item.ToString() ?? "").ToArray())
                        .ToList();

                    return Ok(new ApiResponse<NewDatasetResponseDto>
                    {
                        Success = true,
                        Data = new NewDatasetResponseDto
                        {
                            Rows = data,
                            SelectedIndex = 0,
                            DatasetId = _rndHome.IdSet
                        }
                    });
                }

                return Ok(new ApiResponse<NewDatasetResponseDto>
                {
                    Success = false,
                    Error = "Failed to create new dataset."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new R&D dataset");
                return StatusCode(500, new ApiResponse<NewDatasetResponseDto>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Copy all datasets meeting search criteria to clipboard
        /// </summary>
        [HttpPost("copy")]
        public IActionResult CopyToClipboard()
        {
            try
            {
                var sData = new StringBuilder();
                DataTable dt2 = new DataTable();
                string sMsg;

                string sqlBase = @"Select DateDSCreated, RN.[Study Name], RN.[Product ID], R3.[Product Description], R5.sLocation, R1.Employees as 'Operator', R2.Employees, PropTestingComplete, AgedTestingComplete, R4.[ID] ,R4.[IDDataset] ,R4.[FormNo] ,[NCOIndex] ,[IsoPbw] ,[DensT1],[DensT2] ,[DensT3] ,[DensT4] ,[DensT5] ,[DensL1] ,[DensL2],[DensW1] ,[DensW2],[DensAvgT],[DensAvgL],[DensAvgW],[DensMass],[Density],[CompStr1],[CompStr2],[CompStr3],[CompStr4],[CompStr],[CellDiaTop],[CellStDevTop],[CellCountTop],[CellDiaSide],[CellStDevSide],[CellCountSide],[CellDia],[CellDiaIsotropy] ,[CellCount],[ClosedCellPer1] ,[ClosedCellPer2],[ClosedCellPer3] ,[ClosedCellPer] ,[HotPlateInitMass],[HotPlateFinalMass] ,[HotPlateInitH1] ,[HotPlateInitH2],[HotPlateInitH3] ,[HotPlateInitH4],[HotPlateInitH5],[HotPlateInitH] ,[HotPlateFinalH1] ,[HotPlateFinalH2] ,[HotPlateFinalH3],[HotPlateFinalH4],[HotPlateFinalH5],[HotPlateFinalH],[HotPlateRetainMass],[HotPlateRetainThick],[ReactMixingTime] ,[React15PTime],[React50PTime] ,[React80PTime],[ReactCupEdgeTime],[React98PTime],[ReactMaxTempTime],[ReactMaxTemp],[ReactMaxHeight],[ReactSampleMass],[PhotoPirPur],[PhotoIso],[PhotoCarbo],[PhotoTrimer],[sFileFTIR],[sFileTGA],[sFileFoamat],R4.[Product Code],[K10D25FInit],[K10D40FInit] ,[K10D75FInit],[K10D110FInit],[K10D25FFinal],[K10D40FFinal],[K10D75FFinal] ,[K10D110FFinal] ,[K90D25FInit],[K90D40FInit],[K90D75FInit] ,[K90D110FInit] ,[K90D25FFinal] ,[K90D40FFinal] ,[K90D75FFinal] ,[K90D110FFinal] ,[K180D25FInit],[K180D40FInit] ,[K180D75FInit] ,[K180D110FInit] ,[K180D25FFinal],[K180D40FFinal] ,[K180D75FFinal],[K180D110FFinal] ,[R10D25FInit],[R10D40FInit],[R10D75FInit] ,[R10D110FInit],[R10D25FFinal] ,[R10D40FFinal] ,[R10D75FFinal] ,[R10D110FFinal],[R90D25FInit],[R90D40FInit] ,[R90D75FInit] ,[R90D110FInit] ,[R90D25FFinal],[R90D40FFinal] ,[R90D75FFinal],[R90D110FFinal],[R180D25FInit],[R180D40FInit] ,[R180D75FInit],[R180D110FInit],[R180D25FFinal],[R180D40FFinal],[R180D75FFinal] ,[R180D110FFinal] ,[sNote],' ' as 'Empty1', '' as 'Empty2','' as 'Empty3',R4.NCOIndex, RN.IsoMats, RN.sIsoMatsNCO, RN.POMats , RN.sPOMatsOH as 'OH#s',R4.POPbws as PBW from dbo.RNDDatasets as RN  Left JOIN Roster AS R1 ON RN.Operator = R1.ID  Left JOIN Roster AS R2 ON RN.Chemist = R2.ID   Left Join[Product Matrix] as R3 on RN.[Product ID] = [Product Code]    right join[RNDFormulations] as R4 on RN.ID = R4.IDDataset   Left join dbo.tblLocations as R5 on RN.Location = R5.ID ";

                string sql = GetSearchCriteria();

                if (!string.IsNullOrEmpty(sql)) sql = sqlBase + " Where " + sql;
                else sql = sqlBase;

                sql += " order by RN.[DateDSCreated] DESC";

                try
                {
                    _rndHome.da = new SqlDataAdapter(sql, _cbfile.conAZ);
                    dt2.Clear();
                    int itmp = _rndHome.da.Fill(dt2);
                    if (itmp < 1)
                    {
                        return Ok(new ApiResponse<string>
                        {
                            Success = false,
                            Error = "No R&D Dataset was found to meet the search criteria."
                        });
                    }
                }
                catch (SqlException ex)
                {
                    _logger.LogError(ex, "SQL error in copy");
                    return StatusCode(500, new ApiResponse<string>
                    {
                        Success = false,
                        Error = $"SQL Error: {ex.Message}"
                    });
                }

                // Build tab-delimited string
                sData.Append(dt2.Columns[0].ColumnName);
                for (int icol = 1; icol < dt2.Columns.Count; icol++)
                    sData.Append("\t" + dt2.Columns[icol].ColumnName);

                for (int irow = 0; irow < dt2.Rows.Count; irow++)
                {
                    sData.Append("\n" + (dt2.Rows[irow][0] ?? "").ToString());
                    for (int icol = 1; icol < dt2.Columns.Count; icol++)
                        sData.Append("\t" + (dt2.Rows[irow][icol] ?? "").ToString());
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Data = sData.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error copying R&D data");
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        // ========== PRIVATE HELPERS ==========

        private void Startup()
        {
            int idTemp;

            if (_cLists.drEmployee["RndDate1"] == DBNull.Value)
                _cLists.drEmployee["RndDate1"] = DBNull.Value;
            else
                _cLists.drEmployee["RndDate1"] = (DateTime)_cLists.drEmployee["RndDate1"];

            if (_cLists.drEmployee["RndDate2"] == DBNull.Value)
                _cLists.drEmployee["RndDate2"] = DBNull.Value;
            else
                _cLists.drEmployee["RndDate2"] = (DateTime)_cLists.drEmployee["RndDate2"];

            if (_cLists.drEmployee["RNDNameSearch"] == DBNull.Value)
                _cLists.drEmployee["RNDNameSearch"] = DBNull.Value;
            else
                _cLists.drEmployee["RNDNameSearch"] = (string)_cLists.drEmployee["RNDNameSearch"];

            if (_cLists.drEmployee["Rnd Product Code"] == DBNull.Value)
                _cLists.drEmployee["Rnd Product Code"] = _cDefualts.sProdRNDAll;
            else
                _cLists.drEmployee["Rnd Product Code"] = (string)_cLists.drEmployee["Rnd Product Code"];

            if (_cLists.drEmployee["RndIDTestingStatus"] == DBNull.Value)
                _cLists.drEmployee["RndIDTestingStatus"] = _cDefualts.iRNDTestingStat;
            else
                _cLists.drEmployee["RndIDTestingStatus"] = (int)_cLists.drEmployee["RndIDTestingStatus"];

            if (_cLists.drEmployee["RndIDStudyType"] == DBNull.Value)
                _cLists.drEmployee["RndIDStudyType"] = _cDefualts.iMfgRunType;
            else
                _cLists.drEmployee["RndIDStudyType"] = (int)_cLists.drEmployee["RndIDStudyType"];

            if (_cLists.drEmployee["RndSql"] != DBNull.Value)
                _rndHome.sqlSearchDS = (string)_cLists.drEmployee["RndSql"];

            if (SearchRNDDB() && _rndHome.dt.Rows.Count > 0)
            {
                _rndHome.indSet = 0;
                _rndHome.IdSet = (int)_rndHome.dt.Rows[0]["ID"];

                if (_cLists.drEmployee["RndIDSelected"] != DBNull.Value)
                {
                    idTemp = (int)_cLists.drEmployee["RndIDSelected"];
                    for (int i = 0; i < _rndHome.dt.Rows.Count; i++)
                    {
                        if ((int)_rndHome.dt.Rows[i]["ID"] == idTemp)
                        {
                            _rndHome.indSet = i;
                            _rndHome.IdSet = (int)_rndHome.dt.Rows[i]["ID"];
                        }
                    }
                }

                _rndHome.EnableRNDPages(true);
            }
            else
            {
                _rndHome.EnableRNDPages(false);
            }
        }

        private bool SearchRNDDB()
        {
            string sql = GetSearchCriteria();

            if (!string.IsNullOrEmpty(sql))
                sql = _rndHome.sqlSearchDS + " Where " + sql;
            else
                sql = _rndHome.sqlSearchDS;

            sql = sql + " Order by DateDSCreated DESC";

            try
            {
                _rndHome.da = new SqlDataAdapter(sql, _cbfile.conAZ);
                _rndHome.da.SelectCommand.Parameters.AddWithValue("@sParam1", _rndHome.sParamValue1);

                _rndHome.dt.Clear();
                int itmp = _rndHome.da.Fill(_rndHome.dt);
                if (itmp < 1) return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error in SearchRNDDB");
                return false;
            }
            return true;
        }

        private string GetSearchCriteria()
        {
            DateTime dateTime;
            string sql = string.Empty, sql1 = string.Empty;

            _rndHome.sParamValue1 = string.Empty;

            if (_cLists.drEmployee["RndDate1"] != DBNull.Value)
            {
                dateTime = ((DateTime)_cLists.drEmployee["RndDate1"]).AddDays(1);
                sql1 = "DateDSCreated < '" + dateTime.ToString() + "'";
                if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1;
            }

            if (_cLists.drEmployee["RndDate2"] != DBNull.Value)
            {
                dateTime = ((DateTime)_cLists.drEmployee["RndDate2"]);
                sql1 = "DateDSCreated >= '" + dateTime.ToString() + "'";
                if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1;
            }

            sql1 = string.Empty;
            if (_cLists.drEmployee["RndIDStudyType"] != DBNull.Value)
                if ((int)_cLists.drEmployee["RndIDStudyType"] != 59)
                    sql1 = " RN.[Study Type] = " + ((int)_cLists.drEmployee["RndIDStudyType"]).ToString();
            if (sql1 != string.Empty) { if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1; }

            sql1 = string.Empty;
            if (_cLists.drEmployee["RndIDTestingStatus"] != DBNull.Value)
            {
                if ((int)_cLists.drEmployee["RndIDTestingStatus"] == 52)
                    sql1 = " RN.[PropTestingComplete] = 'false' and (RN.[Abandoned] is null or RN.[Abandoned] ='false' ) ";
                else if ((int)_cLists.drEmployee["RndIDTestingStatus"] == 53)
                    sql1 = " RN.[PropTestingComplete] = 'true' and (RN.[Abandoned] is null or RN.[Abandoned] ='false' ) ";
                else if ((int)_cLists.drEmployee["RndIDTestingStatus"] == 54)
                    sql1 = " RN.[AgedTestingComplete] = 'true' and (RN.[Abandoned] is null or RN.[Abandoned] ='false' ) ";
                else if ((int)_cLists.drEmployee["RndIDTestingStatus"] == 56)
                    sql1 = " (RN.[Abandoned] is null or RN.[Abandoned] ='false' ) ";
                else if ((int)_cLists.drEmployee["RndIDTestingStatus"] == 64)
                    sql1 = " RN.[Abandoned]  ='true' ";
            }
            if (sql1 != string.Empty) { if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1; }

            sql1 = string.Empty;
            if (_cLists.drEmployee["Rnd Product Code"] != DBNull.Value)
            {
                if ((string)_cLists.drEmployee["Rnd Product Code"] != "All Products")
                    sql1 = " RN.[Product ID] = '" + (string)_cLists.drEmployee["Rnd Product Code"] + "' ";
            }
            if (sql1 != string.Empty) { if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1; }

            sql1 = _rndHome.sParamValue1 = string.Empty;
            if (_cLists.drEmployee["RNDNameSearch"] != DBNull.Value)
            {
                _rndHome.sParamValue1 = "%" + _cLists.drEmployee["RNDNameSearch"].ToString() + "%";
                sql1 = "RN.[Study Name] Like @sParam1";
            }
            if (sql1 != string.Empty) { if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1; }

            return sql;
        }

        private RndSearchResultDto BuildSearchResult()
        {
            var result = new RndSearchResultDto
            {
                SelectedIndex = _rndHome.indSet,
                CurrentDatasetId = _rndHome.IdSet
            };

            if (_rndHome.dt == null) return result;

            foreach (DataColumn col in _rndHome.dt.Columns)
            {
                result.Columns.Add(col.ColumnName);
            }

            foreach (DataRow row in _rndHome.dt.Rows)
            {
                var dict = new Dictionary<string, object?>();
                foreach (DataColumn col in _rndHome.dt.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }
                result.Rows.Add(dict);
            }

            return result;
        }
    }
}