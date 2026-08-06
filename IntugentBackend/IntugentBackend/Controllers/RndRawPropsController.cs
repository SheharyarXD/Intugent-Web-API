using Microsoft.AspNetCore.Mvc;
using IntugentBackend.Models;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Rnd;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RndRawPropsController : ControllerBase
    {
        private readonly RNDHome _rndHome;
        private readonly RNDRawProps _rndRawProps;
        private readonly RndRawPropsService _svc;
        private readonly ILogger<RndRawPropsController> _logger;

        public RndRawPropsController(RNDHome rndHome, RNDRawProps rndRawProps, RndRawPropsService svc, ILogger<RndRawPropsController> logger)
        {
            _rndHome = rndHome;
            _rndRawProps = rndRawProps;
            _svc = svc;
            _logger = logger;
        }

        /// <summary>
        /// Load all raw property grids for the currently selected R&amp;D dataset (mirrors the old page's OnGet).
        /// </summary>
        [HttpGet("load")]
        public IActionResult Load()
        {
            try
            {
                if (_rndHome.IdSet <= 0)
                {
                    return Ok(new ApiResponse<RawPropsDto>
                    {
                        Success = false,
                        Error = "No R&D dataset is selected. Select a dataset on the R&D Home page first."
                    });
                }

                _rndHome.GetDataSet(_rndHome.IdSet);
                _svc.Initialize();

                return Ok(new ApiResponse<RawPropsDto> { Success = true, Data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading raw property data");
                return StatusCode(500, new ApiResponse<RawPropsDto> { Success = false, Error = ex.Message });
            }
        }

        [HttpPost("update-density")]
        public IActionResult UpdateDensity([FromBody] GridCellUpdateRequest request)
        {
            return HandleUpdate(request, () =>
            {
                string[] sFields = { "DensT1", "DensT2", "DensT3", "DensT4", "DensT5", "DensL1", "DensL2", "DensW1", "DensW2", "DensMass" };
                if (request.Col == 0 || request.Row > 9) return;
                int icol1 = request.Col - 1;
                _svc.GetDoubleFromGrid(sFields, request.Row, icol1, request.Text);
                _svc.CalculateDensity(request.Col, icol1);
                _rndHome.UpdateFormulatiions();
            });
        }

        [HttpPost("update-compstr")]
        public IActionResult UpdateCompStr([FromBody] GridCellUpdateRequest request)
            => HandleUpdate(request, () => _svc.UpdateCompStr(request.Row, request.Col, request.Text));

        [HttpPost("update-closedcell")]
        public IActionResult UpdateClosedCell([FromBody] GridCellUpdateRequest request)
            => HandleUpdate(request, () => _svc.UpdateClosedCell(request.Row, request.Col, request.Text));

        [HttpPost("update-porescan")]
        public IActionResult UpdatePoreScan([FromBody] GridCellUpdateRequest request)
            => HandleUpdate(request, () => _svc.UpdatePoreScan(request.Row, request.Col, request.Text));

        [HttpPost("update-hotplates")]
        public IActionResult UpdateHotPlates([FromBody] GridCellUpdateRequest request)
            => HandleUpdate(request, () => _svc.UpdateHotPlates(request.Row, request.Col, request.Text));

        private IActionResult HandleUpdate(GridCellUpdateRequest request, Action apply)
        {
            try
            {
                apply();
                return Ok(new ApiResponse<RawPropsDto> { Success = true, Data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating raw property grid cell");
                return StatusCode(500, new ApiResponse<RawPropsDto> { Success = false, Error = ex.Message });
            }
        }

        private RawPropsDto BuildDto() => new()
        {
            DensityE = ToGrid(_rndRawProps.dtDensityE),
            DensityC = ToGrid(_rndRawProps.dtDensityC),
            CompStrE = ToGrid(_rndRawProps.dtCompStrE),
            CompStrC = ToGrid(_rndRawProps.dtCompStrC),
            PoreScanE = ToGrid(_rndRawProps.dtPoreScanE),
            PoreScanC = ToGrid(_rndRawProps.dtPoreScanC),
            ClosedCellE = ToGrid(_rndRawProps.dtClosedCellE),
            ClosedCellC = ToGrid(_rndRawProps.dtClosedCellC),
            HotPlatesE = ToGrid(_rndRawProps.dtHotPlatesE),
            HotPlatesC = ToGrid(_rndRawProps.dtHotPlatesC),
            HotPlatesC1 = ToGrid(_rndRawProps.dtHotPlatesC1)
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

    public class GridDto
    {
        public List<string> Columns { get; set; } = new();
        public List<List<string>> Rows { get; set; } = new();
    }

    public class RawPropsDto
    {
        public GridDto DensityE { get; set; } = new();
        public GridDto DensityC { get; set; } = new();
        public GridDto CompStrE { get; set; } = new();
        public GridDto CompStrC { get; set; } = new();
        public GridDto PoreScanE { get; set; } = new();
        public GridDto PoreScanC { get; set; } = new();
        public GridDto ClosedCellE { get; set; } = new();
        public GridDto ClosedCellC { get; set; } = new();
        public GridDto HotPlatesE { get; set; } = new();
        public GridDto HotPlatesC { get; set; } = new();
        public GridDto HotPlatesC1 { get; set; } = new();
    }

    public class GridCellUpdateRequest
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
