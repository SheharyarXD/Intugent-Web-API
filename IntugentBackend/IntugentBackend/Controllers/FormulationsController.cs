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
        private readonly RNDHome _rndHome;
        private readonly RNDFormulations _rndFormulations;
        public FormulationsController(RNDHome rndHome, RNDFormulations rndFormulations)
        {
            _rndHome = rndHome;
            _rndFormulations = rndFormulations;
        }

        [HttpGet("data/{id}")]
        public IActionResult GetFormulationData(int id)
        {
            if (!_rndHome.GetDataSet(id)) return NotFound();
            _rndFormulations.ReadDataset();
            _rndFormulations.FormDescriptors();

            var table = _rndFormulations.dtFormProp;
            var list = table.AsEnumerable().Select(row =>
                table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col] == DBNull.Value ? 0 : row[col])
            ).ToList();

            return Ok(new { FormProps = list });
        }

        [HttpPost("update-nco/{id}")]
        public IActionResult UpdateNcoIndex(int id, [FromBody] NcoUpdateRequest request)
        {
            if (!_rndHome.GetDataSet(id)) return NotFound();
            _rndFormulations.Forms.FormAr[request.ColIndex].NcoIndex = request.Value;
            _rndHome.dtF.Rows[request.ColIndex]["NCOIndex"] = request.Value;
            _rndHome.UpdateFormulatiions();
            return Ok(new { success = true });
        }
    }
    public class NcoUpdateRequest { public int ColIndex { get; set; } public double Value { get; set; } }
}