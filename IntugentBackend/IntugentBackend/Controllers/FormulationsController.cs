using Microsoft.AspNetCore.Mvc;
using IntugentBackend.Models;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Rnd;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormulationsController : ControllerBase
    {
        private readonly RNDHome _rndHome;
        private readonly RNDFormulations _rndFormulations;
        private readonly CLists _cLists;
        private readonly FormulationsPageService _svc;
        private readonly ILogger<FormulationsController> _logger;

        public FormulationsController(RNDHome rndHome, RNDFormulations rndFormulations, CLists cLists, FormulationsPageService svc, ILogger<FormulationsController> logger)
        {
            _rndHome = rndHome;
            _rndFormulations = rndFormulations;
            _cLists = cLists;
            _svc = svc;
            _logger = logger;
        }

        /// <summary>
        /// Load the Formulations page for the currently selected R&amp;D dataset (mirrors the old page's OnGet).
        /// </summary>
        [HttpGet("load")]
        public IActionResult Load()
        {
            try
            {
                bool ok = _svc.Initialize();
                if (!ok)
                {
                    return Ok(new ApiResponse<FormulationsDto>
                    {
                        Success = false,
                        Error = "No R&D dataset is selected. Select a dataset on the R&D Home page first."
                    });
                }

                return Ok(new ApiResponse<FormulationsDto> { Success = true, Data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Formulations data");
                return StatusCode(500, new ApiResponse<FormulationsDto> { Success = false, Error = ex.Message });
            }
        }

        [HttpPost("gen-info")]
        public IActionResult UpdateGenInfo([FromBody] NameValueRequest request)
            => HandleUpdate(() => _svc.UpdateGenInfo(request.Name, request.Value));

        [HttpPost("update-nco")]
        public IActionResult UpdateNco([FromBody] GridCellUpdateRequest request)
            => HandleUpdate(() => _svc.UpdateNcoCell(request.Row, request.Col, request.Text));

        [HttpPost("update-po")]
        public IActionResult UpdatePO([FromBody] GridCellUpdateRequest request)
            => HandleUpdate(() => _svc.UpdatePOCell(request.Row, request.Col, request.Text));

        [HttpPost("update-iso")]
        public IActionResult UpdateIso([FromBody] GridCellUpdateRequest request)
            => HandleUpdate(() => _svc.UpdateIsoCell(request.Row, request.Col, request.Text));

        [HttpPost("add-po-row")]
        public IActionResult AddPORow()
            => HandleUpdate(() => _svc.AddPORow());

        [HttpPost("paste-po")]
        public IActionResult PastePO([FromBody] List<string[]> rows)
            => HandleUpdate(() => _svc.PastePOData(rows));

        [HttpPost("foamat-gm")]
        public IActionResult UpdateFoamatGm([FromBody] StringValueRequest request)
            => HandleUpdate(() => _svc.UpdateFoamatGm(request.Value));

        private IActionResult HandleUpdate(Action apply)
        {
            try
            {
                apply();
                return Ok(new ApiResponse<FormulationsDto> { Success = true, Data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Formulations data");
                return StatusCode(500, new ApiResponse<FormulationsDto> { Success = false, Error = ex.Message });
            }
        }

        private FormulationsDto BuildDto()
        {
            var forms = _rndFormulations.Forms;

            var dto = new FormulationsDto
            {
                Id = _svc.GetId(),
                StudyName = _svc.GetStudyName(),
                ChemistId = _svc.GetChemist(),
                OperatorId = _svc.GetOperator(),
                ProductId = _svc.GetProductId(),
                StudyTypeId = _svc.GetStudyType(),
                DateCreated = _svc.GetDateCreated(),
                Abandoned = _svc.GetAbandoned(),
                FoamatGm = _svc.GetFoamatGm(),
                FormProps = ToGrid(_rndFormulations.dtFormProp),
                PolyolList = _rndFormulations.sMatNameListPO.ToList(),
                IsoList = _rndFormulations.sMatNameListIso.ToList()
            };

            foreach (DataRowView row in _cLists.dvRunTypeRND2)
                dto.StudyTypes.Add(new FilterOptionDto { Id = Convert.ToInt32(row["ID"]), Name = row["sName"]?.ToString() ?? string.Empty });

            foreach (DataRowView row in _cLists.dvComProd)
                dto.Products.Add(new FilterOptionDto { Code = row["Product Code"]?.ToString(), Name = row["Product"]?.ToString() ?? string.Empty });

            foreach (DataRowView row in _cLists.dvEmployeesRND)
                dto.Employees.Add(new FilterOptionDto { Id = Convert.ToInt32(row["ID"]), Name = row["Employees"]?.ToString() ?? string.Empty });

            dto.Nco.Add(new MaterialRowDto
            {
                MatName = forms.NCOIndexMats[0].MatName,
                Pbws = new List<string?> { forms.NCOIndexMats[0].Pbw1, forms.NCOIndexMats[0].Pbw2, forms.NCOIndexMats[0].Pbw3, forms.NCOIndexMats[0].Pbw4, forms.NCOIndexMats[0].Pbw5, forms.NCOIndexMats[0].Pbw6, forms.NCOIndexMats[0].Pbw7, forms.NCOIndexMats[0].Pbw8 }
            });

            foreach (var m in forms.IsoMats)
            {
                dto.Iso.Add(new MaterialRowDto
                {
                    MatName = m.MatName,
                    MatType = m.MatType,
                    MatNco = m.MatNco,
                    MatFunc = m.MatFunc,
                    Pbws = new List<string?> { m.Pbw1, m.Pbw2, m.Pbw3, m.Pbw4, m.Pbw5, m.Pbw6, m.Pbw7, m.Pbw8 }
                });
            }

            foreach (var m in forms.POMats)
            {
                dto.Po.Add(new MaterialRowDto
                {
                    MatName = m.MatName,
                    MatType = m.MatType,
                    MatOHNum = m.MatOHNum,
                    MatFunc = m.MatFunc,
                    Pbws = new List<string?> { m.Pbw1, m.Pbw2, m.Pbw3, m.Pbw4, m.Pbw5, m.Pbw6, m.Pbw7, m.Pbw8 }
                });
            }

            return dto;
        }

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

    public class FormulationsDto
    {
        public string Id { get; set; } = string.Empty;
        public string StudyName { get; set; } = string.Empty;
        public int ChemistId { get; set; }
        public int OperatorId { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public int StudyTypeId { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool Abandoned { get; set; }
        public string FoamatGm { get; set; } = string.Empty;

        public List<FilterOptionDto> StudyTypes { get; set; } = new();
        public List<FilterOptionDto> Products { get; set; } = new();
        public List<FilterOptionDto> Employees { get; set; } = new();

        public List<MaterialRowDto> Nco { get; set; } = new();
        public List<MaterialRowDto> Iso { get; set; } = new();
        public List<MaterialRowDto> Po { get; set; } = new();

        public List<string> PolyolList { get; set; } = new();
        public List<string> IsoList { get; set; } = new();

        public GridDto FormProps { get; set; } = new();
    }

    public class MaterialRowDto
    {
        public string? MatName { get; set; }
        public string? MatType { get; set; }
        public double MatOHNum { get; set; }
        public double MatNco { get; set; }
        public double MatFunc { get; set; }
        public List<string?> Pbws { get; set; } = new();
    }

    public class NameValueRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class StringValueRequest
    {
        public string Value { get; set; } = string.Empty;
    }
}
