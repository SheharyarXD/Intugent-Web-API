using Microsoft.AspNetCore.Mvc;
using IntugentBackend.Models;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Rnd;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RndTdrvController : ControllerBase
    {
        private readonly RNDHome _rndHome;
        private readonly RNDTDRV _rndTdrv;
        private readonly RNDTDRVService _svc;
        private readonly ILogger<RndTdrvController> _logger;

        public RndTdrvController(RNDHome rndHome, RNDTDRV rndTdrv, RNDTDRVService svc, ILogger<RndTdrvController> logger)
        {
            _rndHome = rndHome;
            _rndTdrv = rndTdrv;
            _svc = svc;
            _logger = logger;
        }

        /// <summary>
        /// Load the TDRV page for the currently selected R&amp;D dataset (mirrors the old page's OnGet).
        /// </summary>
        [HttpGet("load")]
        public IActionResult Load()
        {
            try
            {
                bool ok = _svc.Initialize();
                if (!ok)
                {
                    return Ok(new ApiResponse<TdrvDto>
                    {
                        Success = false,
                        Error = "No R&D dataset is selected. Select a dataset on the R&D Home page first."
                    });
                }

                return Ok(new ApiResponse<TdrvDto> { Success = true, Data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading TDRV data");
                return StatusCode(500, new ApiResponse<TdrvDto> { Success = false, Error = ex.Message });
            }
        }

        [HttpPost("update")]
        public IActionResult UpdateProps([FromBody] GridCellUpdateRequest request)
        {
            try
            {
                _svc.UpdateExpProp(request.Row, request.Col, request.Text);
                return Ok(new ApiResponse<TdrvDto> { Success = true, Data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating TDRV data");
                return StatusCode(500, new ApiResponse<TdrvDto> { Success = false, Error = ex.Message });
            }
        }

        [HttpPost("update-aged-testing-complete")]
        public IActionResult UpdateAgedTestingComplete([FromBody] BoolValueRequest request)
        {
            try
            {
                _svc.UpdateAgedTestingComplete(request.Value);
                return Ok(new ApiResponse<TdrvDto> { Success = true, Data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Aged Testing Complete flag");
                return StatusCode(500, new ApiResponse<TdrvDto> { Success = false, Error = ex.Message });
            }
        }

        private TdrvDto BuildDto() => new()
        {
            ExpPropsE = ToGrid(_rndTdrv.dtTdrvE),
            ExpPropsC = ToGrid(_rndTdrv.dtTdrvC),
            ExpPropsP = ToGrid(_rndTdrv.dtTdrvP),
            AgedTestingComplete = _svc.GetAgedTestingComplete()
        };

        private static GridDto ToGrid(DataTable dt)
        {
            var grid = new GridDto();
            foreach (DataColumn col in dt.Columns) grid.Columns.Add(col.ColumnName);
            foreach (DataRow row in dt.Rows)
            {
                var cells = new List<string>();
                foreach (DataColumn col in dt.Columns) cells.Add(row[col]?.ToString() ?? string.Empty);
                grid.Rows.Add(cells);
            }
            return grid;
        }
    }

    // ========== DTOs ==========

    public class TdrvDto
    {
        public GridDto ExpPropsE { get; set; } = new();
        public GridDto ExpPropsC { get; set; } = new();
        public GridDto ExpPropsP { get; set; } = new();
        public bool AgedTestingComplete { get; set; }
    }
}
