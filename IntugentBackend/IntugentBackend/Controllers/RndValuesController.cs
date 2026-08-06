using Microsoft.AspNetCore.Mvc;
using IntugentBackend.Models;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Rnd;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RndValuesController : ControllerBase
    {
        private readonly RNDRValuesService _service;
        private readonly RNDRValues _rndRValues;
        private readonly ILogger<RndValuesController> _logger;

        public RndValuesController(RNDRValuesService service, RNDRValues rndRValues, ILogger<RndValuesController> logger)
        {
            _service = service;
            _rndRValues = rndRValues;
            _logger = logger;
        }

        /// <summary>
        /// Load the R Values tool for the currently selected R&amp;D dataset (mirrors the old page's OnGet).
        /// </summary>
        [HttpGet("load")]
        public IActionResult Load()
        {
            try
            {
                bool ok = _service.Initialize();
                if (!ok)
                {
                    return Ok(new ApiResponse<RValuesDto>
                    {
                        Success = false,
                        Error = "No R&D dataset is selected. Select a dataset on the R&D Home page first."
                    });
                }

                return Ok(new ApiResponse<RValuesDto> { Success = true, Data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading R Values tool");
                return StatusCode(500, new ApiResponse<RValuesDto> { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// A calculation-parameter field lost focus (mirrors OnPostLostFocus_Fields).
        /// </summary>
        [HttpPost("lost-focus")]
        public IActionResult LostFocus([FromBody] FieldChangeRequest request)
        {
            try
            {
                _service.UpdateLostFocusField(request.Name, request.Value);
                return Ok(new ApiResponse<RValuesDto> { Success = true, Data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating R Values field {Name}", request?.Name);
                return StatusCode(500, new ApiResponse<RValuesDto> { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// The X or Y axis selector changed (mirrors OnPostGAxis_SelectionChanged).
        /// </summary>
        [HttpPost("axis-changed")]
        public IActionResult AxisChanged([FromBody] AxisChangeRequest request)
        {
            try
            {
                _service.UpdateAxisSelection(request.Name, request.Value, request.Item);
                return Ok(new ApiResponse<RValuesDto> { Success = true, Data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating R Values axis selection");
                return StatusCode(500, new ApiResponse<RValuesDto> { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// "Set Default Values" button (mirrors OnPostExportData).
        /// </summary>
        [HttpPost("reset-defaults")]
        public IActionResult ResetDefaults()
        {
            try
            {
                _service.ResetToDefaultValues();
                return Ok(new ApiResponse<RValuesDto> { Success = true, Data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting R Values to defaults");
                return StatusCode(500, new ApiResponse<RValuesDto> { Success = false, Error = ex.Message });
            }
        }

        private RValuesDto BuildDto()
        {
            var dto = new RValuesDto
            {
                GMeasureTemp = _rndRValues.gMeasureTemp,
                GCellSize = _rndRValues.gCellSize,
                GCellPress = _rndRValues.gCellPress,
                GPolDen = _rndRValues.gPolDen,
                GPolCond = _rndRValues.gPolCond,
                GFracStruts = _rndRValues.gFracStruts,
                XAxisSelectedValue = _rndRValues.gXAxisSelectedValue ?? _rndRValues.RData.sXaxisTag ?? "TE",
                YAxisSelectedValue = _rndRValues.gYAxisSelectedValue ?? _rndRValues.RData.sYaxisTag ?? "RV",
                ArX = _rndRValues.dArX,
                Ar0 = _rndRValues.dAr0,
                Ar1 = _rndRValues.dAr1,
                Ar2 = _rndRValues.dAr2,
                Ar3 = _rndRValues.dAr3,
                Ar4 = _rndRValues.dAr4
            };

            if (_rndRValues.gGasComp != null)
            {
                foreach (DataRowView row in _rndRValues.gGasComp)
                {
                    var r = new GasCompRowDto { GasName = row["GasName"]?.ToString() ?? string.Empty };
                    for (int i = 1; i <= _rndRValues.nForms; i++)
                        r.Values.Add(NormalizeValue(row["#" + i]));
                    dto.GasComp.Add(r);
                }
            }

            return dto;
        }

        private static string NormalizeValue(object value)
        {
            if (value is double d)
            {
                if (double.IsNaN(d) || double.IsInfinity(d)) return "N/A";
                return d.ToString("G");
            }
            return value?.ToString() ?? "N/A";
        }
    }

    // ========== DTOs ==========

    public class RValuesDto
    {
        public string GMeasureTemp { get; set; } = string.Empty;
        public string GCellSize { get; set; } = string.Empty;
        public string GCellPress { get; set; } = string.Empty;
        public string GPolDen { get; set; } = string.Empty;
        public string GPolCond { get; set; } = string.Empty;
        public string GFracStruts { get; set; } = string.Empty;
        public string XAxisSelectedValue { get; set; } = "TE";
        public string YAxisSelectedValue { get; set; } = "RV";
        public List<GasCompRowDto> GasComp { get; set; } = new();
        public double[] ArX { get; set; } = Array.Empty<double>();
        public double[] Ar0 { get; set; } = Array.Empty<double>();
        public double[] Ar1 { get; set; } = Array.Empty<double>();
        public double[] Ar2 { get; set; } = Array.Empty<double>();
        public double[] Ar3 { get; set; } = Array.Empty<double>();
        public double[] Ar4 { get; set; } = Array.Empty<double>();
    }

    public class GasCompRowDto
    {
        public string GasName { get; set; } = string.Empty;
        public List<string> Values { get; set; } = new();
    }

    public class FieldChangeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class AxisChangeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Item { get; set; } = string.Empty;
    }
}
