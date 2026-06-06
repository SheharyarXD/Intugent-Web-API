using System.Data;
using IntugentBackend.Services.Admin;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Rnd;
using Microsoft.AspNetCore.Mvc;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ObjectsService _objectsService;

        public SessionController(ObjectsService objectsService)
        {
            _objectsService = objectsService;
        }

        [HttpPost("begin")]
        public IActionResult BeginSession([FromBody] int userId)
        {
            try
            {
                MainWindow mainWindow = new MainWindow();
                (_objectsService.CDefualts, _objectsService.CLists, _objectsService.Cbfile)
                    = mainWindow.MainWindowConstructor(userId);

                if (_objectsService.CDefualts == null || _objectsService.CLists == null || _objectsService.Cbfile == null)
                {
                    return BadRequest("Failed to initialize session — MainWindowConstructor returned null.");
                }

                SetOptionBoxes(_objectsService.CDefualts, _objectsService.CLists);

                // Initialize services
                InitializeServices();

                _objectsService.UserIndex = userId;

                return Ok(new { userId, message = "Session initialized successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception to help debug "Failed to fetch"
                System.Diagnostics.Debug.WriteLine($"CRITICAL ERROR in BeginSession: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private void InitializeServices()
        {
            _objectsService.CProdTargets = new CProdTargets(_objectsService.Cbfile, _objectsService.CDefualts);
            // ... (rest of your initializations) ...
            _objectsService.RNDHome = new RNDHome(_objectsService.CDefualts, _objectsService.CLists, _objectsService.Cbfile);
            // ...
        }

        private void SetOptionBoxes(CDefualts defualts, CLists lists)
        {
            if (lists.dtComProd == null) return;

            // Ensure columns exist before trying to add a row
            if (!lists.dtComProd.Columns.Contains("Product Code"))
                lists.dtComProd.Columns.Add("Product Code", typeof(string));

            if (!lists.dtComProd.Columns.Contains("Product"))
                lists.dtComProd.Columns.Add("Product", typeof(string));

            var dr = lists.dtComProd.NewRow();
            dr["Product Code"] = defualts.sProdMfgAll ?? "Default";
            dr["Product"] = defualts.sProdMfgAll ?? "Default";
            lists.dtComProd.Rows.InsertAt(dr, 0);

            lists.dvComProd = lists.dtComProd.DefaultView;
            lists.dvComProdAll = lists.dtComProd.DefaultView;
        }

        [HttpGet("groups")]
        public IActionResult GetGroups()
        {
            var groups = new List<string> { "OKTA_GAF_Intugent_RnD_Users", "OKTA_GAF_Intugent_Admins" };
            return Ok(groups);
        }
    }
}