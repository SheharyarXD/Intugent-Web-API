using Microsoft.AspNetCore.Mvc;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Rnd;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormulationsController : ControllerBase
    {
        private readonly ObjectsService _objectsService;
        public FormulationsController(ObjectsService objectsService) => _objectsService = objectsService;

        [HttpGet("data/{id}")]
        public IActionResult GetFormulationData(int id)
        {
            if (!_objectsService.RNDHome.GetDataSet(id)) return NotFound();
            _objectsService.RNDFormulations.ReadDataset();
            _objectsService.RNDFormulations.FormDescriptors();

            var table = _objectsService.RNDFormulations.dtFormProp;
            var list = table.AsEnumerable().Select(row =>
                table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col] == DBNull.Value ? 0 : row[col])
            ).ToList();

            return Ok(new { FormProps = list });
        }

        [HttpPost("update-nco/{id}")]
        public IActionResult UpdateNcoIndex(int id, [FromBody] NcoUpdateRequest request)
        {
            if (!_objectsService.RNDHome.GetDataSet(id)) return NotFound();
            _objectsService.RNDFormulations.Forms.FormAr[request.ColIndex].NcoIndex = request.Value;
            _objectsService.RNDHome.dtF.Rows[request.ColIndex]["NCOIndex"] = request.Value;
            _objectsService.RNDHome.UpdateFormulatiions();
            return Ok(new { success = true });
        }
    }
    public class NcoUpdateRequest { public int ColIndex { get; set; } public double Value { get; set; } }
}