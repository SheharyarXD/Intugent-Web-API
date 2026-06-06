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
        private readonly ObjectsService _os;
        private readonly MfgAnalysisService _svc;

        public MfgAnalysisController(ObjectsService os, MfgAnalysisService svc)
        {
            _os = os;
            _svc = svc;
        }

        [HttpGet("load")]
        public IActionResult Load()
        {
            // Always ensure data is loaded
            _os.RNDHome.GetDataSet(1);

            // Call service logic to perform analysis
            var results = _svc.PerformAnalysis();

            return Ok(results);
        }
    }
}