using Microsoft.AspNetCore.Mvc;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Rnd;
using System.Data;

namespace IntugentBackend.Controllers.Rnd
{
    [ApiController]
    [Route("api/[controller]")]
    public class RndTdrvController : ControllerBase
    {
        private readonly RNDHome _rndHome;

        public RndTdrvController(RNDHome rndHome) => _rndHome = rndHome;

        // Inside Controllers/Rnd/RndTdrvController.cs

        

        [HttpPost("update")]
        public IActionResult UpdateProps([FromBody] TdrvUpdateModel model)
        {
            // Direct processing here
            var dtF = _rndHome.dtF;
            // Your logic for GetTDRVValues moved directly into this scope

            _rndHome.UpdateFormulatiions();
            return Ok(new { success = true });
        }
    }

    public class TdrvUpdateModel
    {
        public string RowId { get; set; } = "";
        public string ColId { get; set; } = "";
        public string Text { get; set; } = "";
    }
}