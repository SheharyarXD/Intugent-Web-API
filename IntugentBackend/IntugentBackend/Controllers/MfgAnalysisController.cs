using IntugentBackend.Services.Core;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Rnd;
using Microsoft.AspNetCore.Mvc;

namespace IntugentBackend.Controllers.Mfg
{
    [ApiController]
    [Route("api/[controller]")]
    public class MfgAnalysisController : ControllerBase
    {
        private readonly RNDHome _rndHome;
        private readonly MfgAnalysisService _svc;

        public MfgAnalysisController(RNDHome rndHome, MfgAnalysisService svc)
        {
            _rndHome = rndHome;
            _svc = svc;
        }

        [HttpGet("load")]
        public IActionResult Load()
        {
            // Always ensure data is loaded
            _rndHome.GetDataSet(1);

            // Call service logic to perform analysis
            var results = _svc.PerformAnalysis();

            return Ok(results);
        }
    }
}