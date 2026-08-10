using IntugentBackend.Models;
using IntugentBackend.Services.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIHomeController : ControllerBase
    {
        private readonly CDBase _cdBase;
        private readonly CNNData _cnnData;
        private readonly ILogger<AIHomeController> _logger;

        public AIHomeController(CDBase cdBase, CNNData cnnData, ILogger<AIHomeController> logger)
        {
            _cdBase = cdBase;
            _cnnData = cnnData;
            _logger = logger;
        }

        /// <summary>
        /// Lists the top 20 AI models (search filters were never implemented in the legacy page).
        /// </summary>
        [HttpGet("search")]
        public IActionResult Search()
        {
            try
            {
                bool ok = _cdBase.SearchDatabase(string.Empty);
                if (!ok)
                    return Ok(new { success = false, error = "No AI Model was found to meet the search criteria." });

                _cdBase.IndexModel = 0;
                _cdBase.dr = _cdBase.dt.Rows[0];
                _cdBase.IDModel = (int)_cdBase.dr["ID"];

                TryPreloadModelData();

                return Ok(new { success = true, data = BuildListDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching AI models");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("new-model")]
        public IActionResult NewModel()
        {
            try
            {
                bool ok = _cdBase.CreateNewModel();
                if (!ok)
                    return Ok(new { success = false, error = "Could not create a new AI model." });

                return Ok(new { success = true, data = BuildListDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new AI model");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("select-model")]
        public IActionResult SelectModel([FromBody] SelectAiModelRequest request)
        {
            try
            {
                if (_cdBase.dt == null)
                    return Ok(new { success = false, error = "No models loaded." });

                for (int i = 0; i < _cdBase.dt.Rows.Count; i++)
                {
                    if ((int)_cdBase.dt.Rows[i]["ID"] == request.Id)
                    {
                        _cdBase.IndexModel = i;
                        _cdBase.dr = _cdBase.dt.Rows[i];
                        break;
                    }
                }
                _cdBase.IDModel = _cdBase.dr != null && _cdBase.dr["ID"] != DBNull.Value ? (int)_cdBase.dr["ID"] : request.Id;

                TryPreloadModelData();

                return Ok(new { success = true, data = BuildListDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error selecting AI model {Id}", request.Id);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        private void TryPreloadModelData()
        {
            try { _cnnData.ReadData(_cdBase); }
            catch (Exception ex) { _logger.LogWarning(ex, "Could not preload training data for model {Id}", _cdBase.IDModel); }
        }

        private AiModelListDto BuildListDto()
        {
            var dto = new AiModelListDto { SelectedId = _cdBase.IDModel };
            if (_cdBase.dt == null) return dto;

            foreach (DataRow row in _cdBase.dt.Rows)
            {
                dto.Rows.Add(new AiModelRowDto
                {
                    Id = row["ID"] != DBNull.Value ? (int)row["ID"] : 0,
                    DateModel = row["DateModel"] != DBNull.Value ? ((DateTime)row["DateModel"]).ToString("MM/dd/yyyy hh:mm tt") : string.Empty,
                    Note = row.Table.Columns.Contains("sNote") && row["sNote"] != DBNull.Value ? row["sNote"].ToString()! : string.Empty,
                    Property = row.Table.Columns.Contains("sProperty") && row["sProperty"] != DBNull.Value ? row["sProperty"].ToString()! : string.Empty,
                    DataSource = row.Table.Columns.Contains("sDataSource") && row["sDataSource"] != DBNull.Value ? row["sDataSource"].ToString()! : string.Empty
                });
            }
            return dto;
        }
    }
}
