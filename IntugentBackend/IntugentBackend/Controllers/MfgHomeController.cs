using IntugentBackend.Models;
using IntugentBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.Json;

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

                string location = _objectsService.CDefualts.IDLocation != 3
                    ? _objectsService.CDefualts.sLocation : string.Empty;

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
                        ? null : Convert.ToDateTime(_objectsService.CLists.drEmployee?["MfgDate1"]),
                    DefaultDateTo = _objectsService.CLists.drEmployee?["MfgDate2"] == DBNull.Value
                        ? null : Convert.ToDateTime(_objectsService.CLists.drEmployee?["MfgDate2"])
                };

                return Ok(new ApiResponse<MfgFiltersDto> { Success = true, Data = filters });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Mfg filters");
                return StatusCode(500, new ApiResponse<MfgFiltersDto>
                { Success = false, Error = ex.Message });
            }
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody] SearchMfgRequest request)
        {
            try
            {
                if (_objectsService.MfgHome == null || _objectsService.CLists?.drEmployee == null)
                {
                    return BadRequest(new ApiResponse<MfgSearchResultDto>
                    { Success = false, Error = "MfgHome not initialized." });
                }

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

                bool found = _objectsService.MfgHome.SearchMfgDB();

                if (found && _objectsService.MfgHome.dt.Rows.Count > 0)
                {
                    _objectsService.Cbfile.iIDMfgIndex = 0;
                    _objectsService.Cbfile.iIDMfg = Convert.ToInt32(_objectsService.MfgHome.dt.Rows[0]["ID4ALL"]);
                }

                var result = BuildSearchResult();
                return Ok(new ApiResponse<MfgSearchResultDto> { Success = true, Data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching Mfg DB");
                return StatusCode(500, new ApiResponse<MfgSearchResultDto>
                { Success = false, Error = ex.Message });
            }
        }

        [HttpGet("results")]
        public IActionResult GetResults()
        {
            try
            {
                if (_objectsService.MfgHome?.dt == null)
                {
                    return Ok(new ApiResponse<MfgSearchResultDto>
                    { Success = true, Data = new MfgSearchResultDto() });
                }

                var result = BuildSearchResult();
                return Ok(new ApiResponse<MfgSearchResultDto> { Success = true, Data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Mfg results");
                return StatusCode(500, new ApiResponse<MfgSearchResultDto>
                { Success = false, Error = ex.Message });
            }
        }

        [HttpPost("select-dataset")]
        public IActionResult SelectDataset([FromBody] SelectDatasetRequest request)
        {
            try
            {
                if (!_objectsService.Cbfile.bCanSwitchRecord)
                {
                    return Ok(new ApiResponse<object>
                    { Success = false, Error = "Cannot switch record: " + _objectsService.Cbfile.sNoRecSwitchMsg });
                }

                if (request.SelectedIndex < 0 || request.SelectedIndex >= request.RowCount)
                {
                    return BadRequest(new ApiResponse<object>
                    { Success = false, Error = "Invalid selection index." });
                }

                if (_objectsService.MfgHome.dt.Rows[request.SelectedIndex]["ID4ALL"] == DBNull.Value)
                {
                    return BadRequest(new ApiResponse<object>
                    { Success = false, Error = "Selected dataset does not have a valid ID." });
                }

                _objectsService.Cbfile.iIDMfgIndex = request.SelectedIndex;
                _objectsService.Cbfile.iIDMfg = request.DatasetId;

                (_objectsService.MfgInProcess, _objectsService.MfgFinishedGoods,
                 _objectsService.MfgDimStability, _objectsService.MfgPlantData,
                 _objectsService.MfgJetMixing) = _objectsService.MfgHome.GetAllMfgData(
                     _objectsService.MfgInProcess, _objectsService.MfgFinishedGoods,
                     _objectsService.MfgDimStability, _objectsService.MfgPlantData,
                     _objectsService.MfgJetMixing);

                return Ok(new ApiResponse<object>
                { Success = true, Data = new { message = $"Dataset {request.DatasetId} selected." } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error selecting dataset");
                return StatusCode(500, new ApiResponse<object>
                { Success = false, Error = ex.Message });
            }
        }

        private MfgSearchResultDto BuildSearchResult()
        {
            var result = new MfgSearchResultDto
            {
                SelectedIndex = _objectsService.Cbfile.iIDMfgIndex,
                CurrentDatasetId = _objectsService.Cbfile.iIDMfg
            };

            if (_objectsService.MfgHome.dt == null) return result;

            foreach (DataColumn col in _objectsService.MfgHome.dt.Columns)
                result.Columns.Add(col.ColumnName);

            foreach (DataRow row in _objectsService.MfgHome.dt.Rows)
            {
                var dict = new Dictionary<string, JsonValue>();
                foreach (DataColumn col in _objectsService.MfgHome.dt.Columns)
                {
                    dict[col.ColumnName] = new JsonValue(row[col] == DBNull.Value ? null : row[col]);
                }
                result.Rows.Add(dict);
            }

            return result;
        }
    }

    // ========== DTOs ==========

    public class MfgFiltersDto
    {
        public List<FilterOptionDto> Products { get; set; } = new();
        public List<FilterOptionDto> TestingStatus { get; set; } = new();
        public List<FilterOptionDto> AgedRValue { get; set; } = new();
        public List<FilterOptionDto> DimStability { get; set; } = new();
        public List<FilterOptionDto> RunTypes { get; set; } = new();
        public string Location { get; set; } = string.Empty;
        public string DefaultProductCode { get; set; } = string.Empty;
        public int DefaultTestingStatusId { get; set; }
        public int DefaultAgedRValueId { get; set; }
        public int DefaultDimStabilityId { get; set; }
        public int DefaultRunTypeId { get; set; }
        public DateTime? DefaultDateFrom { get; set; }
        public DateTime? DefaultDateTo { get; set; }
    }


    public class JsonValue
    {
        private readonly object? _value;
        public JsonValue() { }
        public JsonValue(object? value) => _value = value;
        public object? RawValue => _value;

        public override string ToString()
        {
            if (_value == null) return "";
            if (_value is JsonElement je)
            {
                return je.ValueKind switch
                {
                    JsonValueKind.Null => "",
                    JsonValueKind.String => je.GetString() ?? "",
                    JsonValueKind.Number => je.TryGetInt32(out var i) ? i.ToString() : je.GetDouble().ToString(),
                    JsonValueKind.True => "True",
                    JsonValueKind.False => "False",
                    _ => je.ToString()
                };
            }
            return _value.ToString() ?? "";
        }

        public int GetInt32()
        {
            if (_value == null) return 0;
            if (_value is JsonElement je)
            {
                if (je.ValueKind == JsonValueKind.Number && je.TryGetInt32(out var i)) return i;
                if (je.ValueKind == JsonValueKind.String && int.TryParse(je.GetString(), out var p)) return p;
                return 0;
            }
            if (_value is int i2) return i2;
            if (_value is long l) return (int)l;
            if (_value is double d) return (int)d;
            if (int.TryParse(_value.ToString(), out var parsed)) return parsed;
            return 0;
        }

        public double GetDouble()
        {
            if (_value == null) return 0;
            if (_value is JsonElement je && je.ValueKind == JsonValueKind.Number) return je.GetDouble();
            if (_value is double d) return d;
            if (_value is int i) return i;
            if (double.TryParse(_value.ToString(), out var p)) return p;
            return 0;
        }

        public DateTime? GetDateTime()
        {
            if (_value == null) return null;
            if (_value is JsonElement je && je.ValueKind == JsonValueKind.String)
                if (DateTime.TryParse(je.GetString(), out var dt)) return dt;
            if (_value is DateTime dt2) return dt2;
            return null;
        }
    }

    public class MfgSearchResultDto
    {
        public List<Dictionary<string, JsonValue>> Rows { get; set; } = new();
        public List<string> Columns { get; set; } = new();
        public int SelectedIndex { get; set; }
        public int CurrentDatasetId { get; set; }
    }

    public class SearchMfgRequest
    {
        public string? ProductCode { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int TestingStatusId { get; set; }
        public int AgedRValueId { get; set; }
        public int DimStabilityId { get; set; }
        public int RunTypeId { get; set; }
    }

}