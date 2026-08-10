using IntugentBackend.Models;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using Microsoft.AspNetCore.Mvc;

namespace IntugentBackend.Controllers.Mfg
{
    [ApiController]
    [Route("api/[controller]")]
    public class MfgAnalysisController : ControllerBase
    {
        private readonly MfgAnalysisService _svc;
        private readonly CAnalysisData _data;
        private readonly ILogger<MfgAnalysisController> _logger;

        public MfgAnalysisController(MfgAnalysisService svc, CAnalysisData data, ILogger<MfgAnalysisController> logger)
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
                _svc.Search();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Mfg Analysis view");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody] MfgAnalysisSearchRequest request)
        {
            try
            {
                _data.Prop1SelectedValue = string.IsNullOrEmpty(request.Prop1SelectedValue) ? "FG-Compressive Str Avg6" : request.Prop1SelectedValue;
                _data.Prod1SelectedValue = request.Prod1SelectedValue;
                _data.MfgSiteSelectedValue = request.MfgSiteSelectedValue;
                _data.MfgDate1 = request.MfgDate1;
                _data.MfgDate2 = request.MfgDate2;

                _svc.Search();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running Mfg Analysis search");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("prop-change")]
        public IActionResult PropChange([FromBody] MfgAnalysisPropRequest request)
        {
            try
            {
                _svc.ChangeProperty(request.Value);
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing Mfg Analysis property");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        private MfgAnalysisDto BuildDto()
        {
            return new MfgAnalysisDto
            {
                ProductOptions = _svc.GetProductOptions().Select(o => new AnalysisFilterOptionDto { Value = o.Value, Name = o.Name }).ToList(),
                LocationOptions = _svc.GetLocationOptions().Select(o => new AnalysisFilterOptionDto { Value = o.Value, Name = o.Name }).ToList(),
                PropertyOptions = _svc.GetPropertyOptions().Select(o => new AnalysisFilterOptionDto { Value = o.Value, Name = o.Name }).ToList(),

                Prod1SelectedValue = _data.Prod1SelectedValue,
                MfgSiteSelectedValue = _data.MfgSiteSelectedValue,
                Prop1SelectedValue = _data.Prop1SelectedValue,
                MfgDate1 = _data.MfgDate1,
                MfgDate2 = _data.MfgDate2,

                XA = _data.XA,
                YA = _data.YA,
                XAvg1 = _data.XAvg1,
                YAvg1 = _data.YAvg1,
                YUCL1 = _data.YUCL1,
                YLCL1 = _data.YLCL1,

                Correlations = _svc.GetCorrelationRows().Select(c => new CorrRowDto { PropName = c.PropName, CorrValue = c.CorrValue }).ToList()
            };
        }
    }
}
