using IntugentBackend.Models;
using IntugentBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MfgHomeController : ControllerBase
    {
        private readonly ObjectsService _objectsService;
        private readonly ILogger<MfgHomeController> _logger;

        public MfgHomeController(ObjectsService objectsService, ILogger<MfgHomeController> logger)
        {
            _objectsService = objectsService;
            _logger = logger;
        }

        /// <summary>
        /// Get all filter dropdown data and default values for Mfg Home page
        /// </summary>
        [HttpGet("filters")]
        public IActionResult GetFilters()
        {
            try
            {
                if (_objectsService.CDefualts == null || _objectsService.CLists == null)
                {
                    return BadRequest(new ApiResponse<MfgFiltersDto>
                    {
                        Success = false,
                        Error = "Session not initialized. Call /api/Session/begin first."
                    });
                }

                // Location
                string location = _objectsService.CDefualts.IDLocation != 3
                    ? _objectsService.CDefualts.sLocation
                    : string.Empty;

                // Products (from dvComProdAll)
                var products = new List<FilterOptionDto>();
                if (_objectsService.CLists.dvComProdAll != null)
                {
                    foreach (DataRowView row in _objectsService.CLists.dvComProdAll)
                    {
                        products.Add(new FilterOptionDto
                        {
                            Code = row["Product Code"]?.ToString(),
                            Name = row["Product"]?.ToString() ?? string.Empty
                        });
                    }
                }

                // Helper to extract lists
                List<FilterOptionDto> ExtractList(string listName)
                {
                    var result = new List<FilterOptionDto>();
                    if (_objectsService.CLists.dvLists == null) return result;

                    var dv = _objectsService.CLists.dtLists.DefaultView;
                    dv.RowFilter = $"sList = '{listName}'";
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

                var filters = new MfgFiltersDto
                {
                    Products = products,
                    TestingStatus = ExtractList("Testing Status Mfg"),
                    AgedRValue = ExtractList("Aged R Value Mfg"),
                    DimStability = ExtractList("Dim Stability Mfg"),
                    RunTypes = ExtractList("Run Type Mfg"),
                    Location = location,
                    // Default values from drEmployee
                    DefaultProductCode = _objectsService.CLists.drEmployee?["Mfg Product Code"] == DBNull.Value
                        ? _objectsService.CDefualts.sProdMfgAll
                        : (_objectsService.CLists.drEmployee?["Mfg Product Code"]?.ToString() ?? string.Empty),
                    DefaultTestingStatusId = _objectsService.CLists.drEmployee?["MfgIDTestingStatus"] == DBNull.Value
                        ? _objectsService.CDefualts.iMfgTestingStat
                        : Convert.ToInt32(_objectsService.CLists.drEmployee?["MfgIDTestingStatus"]),
                    DefaultAgedRValueId = _objectsService.CLists.drEmployee?["MfgIDAgedTesting"] == DBNull.Value
                        ? _objectsService.CDefualts.iMfgAgedRValue
                        : Convert.ToInt32(_objectsService.CLists.drEmployee?["MfgIDAgedTesting"]),
                    DefaultDimStabilityId = _objectsService.CLists.drEmployee?["MfgIDDimStability"] == DBNull.Value
                        ? _objectsService.CDefualts.iMfgDimStability
                        : Convert.ToInt32(_objectsService.CLists.drEmployee?["MfgIDDimStability"]),
                    DefaultRunTypeId = _objectsService.CLists.drEmployee?["MfgIDRunType"] == DBNull.Value
                        ? _objectsService.CDefualts.iMfgRunType
                        : Convert.ToInt32(_objectsService.CLists.drEmployee?["MfgIDRunType"]),
                    DefaultDateFrom = _objectsService.CLists.drEmployee?["MfgDate1"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(_objectsService.CLists.drEmployee?["MfgDate1"]),
                    DefaultDateTo = _objectsService.CLists.drEmployee?["MfgDate2"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(_objectsService.CLists.drEmployee?["MfgDate2"])
                };

                return Ok(new ApiResponse<MfgFiltersDto>
                {
                    Success = true,
                    Data = filters
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Mfg filters");
                return StatusCode(500, new ApiResponse<MfgFiltersDto>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Search Mfg database with filters
        /// </summary>
        [HttpPost("search")]
        public IActionResult Search([FromBody] SearchMfgRequest request)
        {
            try
            {
                if (_objectsService.MfgHome == null)
                {
                    return BadRequest(new ApiResponse<MfgSearchResultDto>
                    {
                        Success = false,
                        Error = "MfgHome not initialized."
                    });
                }

                // Apply filters to drEmployee (same as old OnPostSearchDB_Click)
                _objectsService.CLists.drEmployee["Mfg Product Code"] =
                    string.IsNullOrEmpty(request.ProductCode) ? DBNull.Value : request.ProductCode;

                _objectsService.CLists.drEmployee["MfgDate1"] =
                    request.DateFrom == null ? DBNull.Value : request.DateFrom;

                _objectsService.CLists.drEmployee["MfgDate2"] =
                    request.DateTo == null ? DBNull.Value : request.DateTo;

                _objectsService.CLists.drEmployee["MfgIDTestingStatus"] =
                    request.TestingStatusId <= 0 ? DBNull.Value : request.TestingStatusId;

                _objectsService.CLists.drEmployee["MfgIDAgedTesting"] =
                    request.AgedRValueId <= 0 ? DBNull.Value : request.AgedRValueId;

                _objectsService.CLists.drEmployee["MfgIDDimStability"] =
                    request.DimStabilityId <= 0 ? DBNull.Value : request.DimStabilityId;

                _objectsService.CLists.drEmployee["MfgIDRunType"] =
                    request.RunTypeId <= 0 ? DBNull.Value : request.RunTypeId;

                // Perform search
                bool found = _objectsService.MfgHome.SearchMfgDB();

                // Update Cbfile
                if (found && _objectsService.MfgHome.dt.Rows.Count > 0)
                {
                    _objectsService.Cbfile.iIDMfgIndex = 0;
                    _objectsService.Cbfile.iIDMfg = Convert.ToInt32(_objectsService.MfgHome.dt.Rows[0]["ID4ALL"]);
                }

                // Save employee preferences
                //CLists_UpdateEmployee.UpdateEmployee(_objectsService.CLists);

                // Build response
                var result = BuildSearchResult();

                return Ok(new ApiResponse<MfgSearchResultDto>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching Mfg DB");
                return StatusCode(500, new ApiResponse<MfgSearchResultDto>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get current search results (for page reload/navigation)
        /// </summary>
        [HttpGet("results")]
        public IActionResult GetResults()
        {
            try
            {
                if (_objectsService.MfgHome?.dt == null)
                {
                    return Ok(new ApiResponse<MfgSearchResultDto>
                    {
                        Success = true,
                        Data = new MfgSearchResultDto()
                    });
                }

                var result = BuildSearchResult();
                return Ok(new ApiResponse<MfgSearchResultDto>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Mfg results");
                return StatusCode(500, new ApiResponse<MfgSearchResultDto>
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
        public IActionResult SelectDataset([FromBody] SelectDatasetRequest request)
        {
            try
            {
                if (!_objectsService.Cbfile.bCanSwitchRecord)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = false,
                        Error = "Cannot switch record: " + _objectsService.Cbfile.sNoRecSwitchMsg
                    });
                }

                if (request.SelectedIndex < 0 || request.SelectedIndex >= request.RowCount)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Error = "Invalid selection index."
                    });
                }

                if (_objectsService.MfgHome.dt.Rows[request.SelectedIndex]["ID4ALL"] == DBNull.Value)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Error = "Selected dataset does not have a valid ID."
                    });
                }

                // Update state
                _objectsService.Cbfile.iIDMfgIndex = request.SelectedIndex;
                _objectsService.Cbfile.iIDMfg = request.DatasetId;

                // Load all Mfg data
                (_objectsService.MfgInProcess,
                 _objectsService.MfgFinishedGoods,
                 _objectsService.MfgDimStability,
                 _objectsService.MfgPlantData,
                 _objectsService.MfgJetMixing) = _objectsService.MfgHome.GetAllMfgData(
                     _objectsService.MfgInProcess,
                     _objectsService.MfgFinishedGoods,
                     _objectsService.MfgDimStability,
                     _objectsService.MfgPlantData,
                     _objectsService.MfgJetMixing);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = new { message = $"Dataset {request.DatasetId} selected." }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error selecting dataset");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        // ========== HELPERS ==========

        private MfgSearchResultDto BuildSearchResult()
        {
            var result = new MfgSearchResultDto
            {
                SelectedIndex = _objectsService.Cbfile.iIDMfgIndex,
                CurrentDatasetId = _objectsService.Cbfile.iIDMfg
            };

            if (_objectsService.MfgHome.dt == null) return result;

            // Get columns
            foreach (DataColumn col in _objectsService.MfgHome.dt.Columns)
            {
                result.Columns.Add(col.ColumnName);
            }

            // Get rows
            foreach (DataRow row in _objectsService.MfgHome.dt.Rows)
            {
                var dict = new Dictionary<string, object?>();
                foreach (DataColumn col in _objectsService.MfgHome.dt.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }
                result.Rows.Add(dict);
            }

            return result;
        }
    }
}