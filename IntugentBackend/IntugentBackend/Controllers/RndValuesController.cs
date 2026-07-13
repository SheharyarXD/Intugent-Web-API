using Microsoft.AspNetCore.Mvc;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Rnd;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RndValuesController : ControllerBase
    {
        private readonly RNDRValuesService _service;

        public RndValuesController(RNDRValuesService service)
        {
            _service = service;
        }
       
        [HttpPost("update")]
        public IActionResult Update()
        {
            _service.UpdateDataset();
            return Ok(new { success = true });
        }

        [HttpGet("collect")]
        public IActionResult Collect()
        {
            _service.CollectBlowGases();
            return Ok(new { success = true });
        }
    }
}