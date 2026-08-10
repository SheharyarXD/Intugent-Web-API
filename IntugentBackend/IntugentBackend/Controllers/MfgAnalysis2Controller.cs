using IntugentBackend.Models;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using Microsoft.AspNetCore.Mvc;

namespace IntugentBackend.Controllers.Mfg
{
    [ApiController]
    [Route("api/[controller]")]
    public class MfgAnalysis2Controller : ControllerBase
    {
        private readonly MfgAnalysis2Service _svc;
        private readonly CAnalysisData _data;
        private readonly ILogger<MfgAnalysis2Controller> _logger;

        public MfgAnalysis2Controller(MfgAnalysis2Service svc, CAnalysisData data, ILogger<MfgAnalysis2Controller> logger)
        {
            _svc = svc;
            _data = data;
            _logger = logger;
        }

        [HttpGet("load")]
        public IActionResult Load()
        {
            try
            {
                _svc.EnsureListsLoaded();
                _svc.Load();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Mfg Analysis-2 view");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-charts")]
        public IActionResult UpdateCharts([FromBody] MfgAnalysis2AxesRequest request)
        {
            try
            {
                _svc.UpdateAxes(request.X1, request.X2, request.Y1, request.Y2);
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Mfg Analysis-2 charts");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        private MfgAnalysis2Dto BuildDto()
        {
            return new MfgAnalysis2Dto
            {
                PropertyOptions = _svc.GetPropertyOptions().Select(o => new AnalysisFilterOptionDto { Value = o.Value, Name = o.Name }).ToList(),
                X1SelectedValue = _data.X1SelectedValue,
                X2SelectedValue = _data.X2SelectedValue,
                Y1SelectedValue = _data.Y1SelectedValue,
                Y2SelectedValue = _data.Y2SelectedValue,
                X1Y1_X = _data.X1Y1_X,
                X1Y1_Y = _data.X1Y1_Y,
                X1Y2_X = _data.X1Y2_X,
                X1Y2_Y = _data.X1Y2_Y,
                X2Y1_X = _data.X2Y1_X,
                X2Y1_Y = _data.X2Y1_Y,
                X2Y2_X = _data.X2Y2_X,
                X2Y2_Y = _data.X2Y2_Y
            };
        }
    }
}
