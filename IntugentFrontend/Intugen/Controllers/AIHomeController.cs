using Microsoft.AspNetCore.Mvc;
using IntugentClassLibrary.Classes;
using IntugentWebApp.Utilities;

namespace Intugen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIHomeController : ControllerBase
    {
        private readonly ObjectsService _objectsService;

        public AIHomeController(ObjectsService objectsService)
        {
            _objectsService = objectsService;
            _objectsService.gInputIndex = 0;
        }

        [HttpGet("models")]
        public IActionResult GetModelsList()
        {
            try
            {
                if (_objectsService.CDBase.SearchDatabase(string.Empty) && 
                    _objectsService.CDBase.dt.Rows.Count > 0)
                {
                    _objectsService.CDBase.IndexModel = 0;
                    var models = new List<ModelListItem>();

                    foreach (System.Data.DataRow row in _objectsService.CDBase.dt.Rows)
                    {
                        models.Add(new ModelListItem
                        {
                            ID = Convert.ToInt32(row["ID"]),
                            DateModel = row["DateModel"]?.ToString(),
                            SNote = row["sNote"]?.ToString(),
                            SProperty = row["sProperty"]?.ToString(),
                            SDataSource = row["sDataSource"]?.ToString()
                        });
                    }

                    _objectsService.CDBase.dr = _objectsService.CDBase.dt.Rows[0];
                    _objectsService.CDBase.IDModel = (int)_objectsService.CDBase.dr["ID"];
                    _objectsService.CNNData.ReadData(_objectsService.CDBase);

                    return Ok(models);
                }
                return Ok(new List<ModelListItem>());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("select")]
        public IActionResult SelectModel([FromBody] int modelId)
        {
            try
            {
                var dt = _objectsService.CDBase.dt;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (modelId == Convert.ToInt32(dt.Rows[i]["ID"]))
                    {
                        _objectsService.CDBase.dr = dt.Rows[i];
                        _objectsService.CDBase.IndexModel = modelId;
                        _objectsService.CDBase.IDModel = modelId;
                        _objectsService.CNNData.ReadData(_objectsService.CDBase);
                        break;
                    }
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("create")]
        public IActionResult CreateNewModel()
        {
            try
            {
                if (_objectsService.CDBase.CreateNewModel())
                {
                    return Ok(true);
                }
                return Ok(false);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class ModelListItem
    {
        public int ID { get; set; }
        public string? DateModel { get; set; }
        public string? SNote { get; set; }
        public string? SProperty { get; set; }
        public string? SDataSource { get; set; }
    }
}