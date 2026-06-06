using Microsoft.AspNetCore.Mvc;
using IntugentBackend.Services.Core;
using System.Data;

namespace IntugentBackend.Controllers.Rnd
{
    [ApiController]
    [Route("api/[controller]")]
    public class RndTdrvController : ControllerBase
    {
        private readonly ObjectsService _os;

        public RndTdrvController(ObjectsService os) => _os = os;

        // Inside Controllers/Rnd/RndTdrvController.cs

        

        [HttpPost("update")]
        public IActionResult UpdateProps([FromBody] TdrvUpdateModel model)
        {
            // Direct processing here
            var dtF = _os.RNDHome.dtF;
            // Your logic for GetTDRVValues moved directly into this scope

            _os.RNDHome.UpdateFormulatiions();
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