using Microsoft.AspNetCore.Mvc;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Rnd;
using System.Data;

namespace IntugentBackend.Controllers.Rnd
{
    [ApiController]
    [Route("api/[controller]")]
    public class RndRawPropsController : ControllerBase
    {
        private readonly ObjectsService _os;
        private readonly RndRawPropsService _svc;

        public RndRawPropsController(ObjectsService os, RndRawPropsService svc)
        {
            _os = os;
            _svc = svc;
        }

        // GET: api/RndRawProps/load-density
        [HttpGet("load-density")]
        public IActionResult LoadDensity()
        {
            _os.RNDHome.GetDataSet(1);

            var data = _os.RNDRawProps.dtDensityE.AsEnumerable()
                .Select(row => {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in row.Table.Columns)
                    {
                        dict[col.ColumnName] = row[col] == DBNull.Value ? "" : row[col].ToString();
                    }
                    return dict;
                })
                .ToList();

            return Ok(data);
        }

        // POST: api/RndRawProps/update-density
        [HttpPost("update-density")]
        public IActionResult UpdateDensity([FromBody] TdrvUpdateModel model)
        {
            System.Diagnostics.Debug.WriteLine("Loading Dataset ID: " + _os.UserIndex);
            _os.RNDHome.GetDataSet(1);
            // 1. Data initialization

            // 2. Input validation
            if (!int.TryParse(model.RowId, out int irow) || !int.TryParse(model.ColId, out int icol))
            {
                return BadRequest("Invalid format for RowId or ColId. Please send numeric values.");
            }

            int icol1 = icol - 1;

            // 3. Safety check to prevent IndexOutOfRangeException
            if (icol1 < 0 || icol1 >= _os.RNDHome.dtF.Rows.Count)
            {
                return BadRequest($"Index {icol1} out of range. Rows available: {_os.RNDHome.dtF.Rows.Count}");
            }

            string[] sFields = { "DensT1", "DensT2", "DensT3", "DensT4", "DensT5", "DensL1", "DensL2", "DensW1", "DensW2", "DensMass" };

            // 4. Processing
            _svc.GetDoubleFromGrid(sFields, irow, icol1, model.Text);
            _svc.CalculateDensity(icol, icol1);

            _os.RNDHome.UpdateFormulatiions();

            return Ok(new { success = true });
        }
    }

}