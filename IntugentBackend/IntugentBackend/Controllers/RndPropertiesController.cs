using Microsoft.AspNetCore.Mvc;
using IntugentBackend.Models;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Rnd;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RndPropertiesController : ControllerBase
    {
        private readonly RNDHome _rndHome;
        private readonly RNDProperties _rndProperties;
        private readonly RndPropertiesService _svc;
        private readonly ILogger<RndPropertiesController> _logger;

        public RndPropertiesController(RNDHome rndHome, RNDProperties rndProperties, RndPropertiesService svc, ILogger<RndPropertiesController> logger)
        {
            _rndHome = rndHome;
            _rndProperties = rndProperties;
            _svc = svc;
            _logger = logger;
        }

        /// <summary>
        /// Load the Properties &amp; Data Files page for the currently selected R&amp;D dataset (mirrors the old page's OnGet).
        /// </summary>
        [HttpGet("load")]
        public IActionResult Load()
        {
            try
            {
                bool ok = _svc.Initialize();
                if (!ok)
                {
                    return Ok(new ApiResponse<RndPropertiesDto>
                    {
                        Success = false,
                        Error = "No R&D dataset is selected. Select a dataset on the R&D Home page first."
                    });
                }

                return Ok(new ApiResponse<RndPropertiesDto> { Success = true, Data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading R&D properties data");
                return StatusCode(500, new ApiResponse<RndPropertiesDto> { Success = false, Error = ex.Message });
            }
        }

        [HttpPost("update-reaction")]
        public IActionResult UpdateReaction([FromBody] GridCellUpdateRequest request)
            => HandleUpdate(() => _svc.UpdateReactionData(request.Row, request.Col, request.Text));

        [HttpPost("update-photo")]
        public IActionResult UpdatePhoto([FromBody] GridCellUpdateRequest request)
            => HandleUpdate(() => _svc.UpdatePhotoData(request.Row, request.Col, request.Text));

        [HttpPost("update-datafile")]
        public IActionResult UpdateDataFile([FromBody] GridCellUpdateRequest request)
            => HandleUpdate(() => _svc.UpdateDataFile(request.Row, request.Col, request.Text));

        [HttpPost("update-product")]
        public IActionResult UpdateProduct([FromBody] GridCellUpdateRequest request)
            => HandleUpdate(() => _svc.UpdateProductCode(request.Row, request.Text));

        [HttpPost("update-notes")]
        public IActionResult UpdateNotes([FromBody] GridCellUpdateRequest request)
            => HandleUpdate(() => _svc.UpdateNote(request.Row, request.Text));

        [HttpPost("update-prop-testing-complete")]
        public IActionResult UpdatePropTestingComplete([FromBody] BoolValueRequest request)
            => HandleUpdate(() => _svc.UpdatePropTestingComplete(request.Value));

        private IActionResult HandleUpdate(Action apply)
        {
            try
            {
                apply();
                return Ok(new ApiResponse<RndPropertiesDto> { Success = true, Data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating R&D properties data");
                return StatusCode(500, new ApiResponse<RndPropertiesDto> { Success = false, Error = ex.Message });
            }
        }

        private RndPropertiesDto BuildDto() => new()
        {
            ReacE = ToGrid(_rndProperties.dtReacE),
            ReacC = ToGrid(_rndProperties.dtReacC),
            PhotoE = ToGrid(_rndProperties.dtPhotoE),
            PhotoC = ToGrid(_rndProperties.dtPhotoC),
            PropsE = ToGrid(_rndProperties.dtPropsE),
            PropsC = ToGrid(_rndProperties.dtPropsC),
            DataFiles = ToGrid(_rndProperties.dtDataFiles),
            Prod = ToGrid(_rndProperties.dtComProd),
            Notes = ToGrid(_rndProperties.dtNotes),
            ProdList = _svc.GetProductList(),
            PropTestingComplete = _svc.GetPropTestingComplete()
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

    public class RndPropertiesDto
    {
        public GridDto ReacE { get; set; } = new();
        public GridDto ReacC { get; set; } = new();
        public GridDto PhotoE { get; set; } = new();
        public GridDto PhotoC { get; set; } = new();
        public GridDto PropsE { get; set; } = new();
        public GridDto PropsC { get; set; } = new();
        public GridDto DataFiles { get; set; } = new();
        public GridDto Prod { get; set; } = new();
        public GridDto Notes { get; set; } = new();
        public List<string> ProdList { get; set; } = new();
        public bool PropTestingComplete { get; set; }
    }

    public class BoolValueRequest
    {
        public bool Value { get; set; }
    }
}
