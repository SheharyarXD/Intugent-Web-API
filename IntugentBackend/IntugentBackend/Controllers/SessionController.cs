using IntugentBackend.Services.Admin;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Rnd;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ObjectsService _objectsService;
        private readonly ILogger<SessionController> _logger;

        public SessionController(ObjectsService objectsService, ILogger<SessionController> logger)
        {
            _objectsService = objectsService;
            _logger = logger;
        }

        /// <summary>
        /// Begin a new session by initializing all domain services for the selected user group.
        /// </summary>
        [HttpPost("begin")]
        [ProducesResponseType(typeof(BeginSessionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BeginSession([FromBody] BeginSessionRequest request)
        {
            try
            {
                if (request == null || request.UserId <= 0)
                {
                    return BadRequest(new { message = "Invalid user ID." });
                }

                _logger.LogInformation("Beginning session for UserId: {UserId}", request.UserId);

                MainWindow mainWindow = new MainWindow();
                (_objectsService.CDefualts, _objectsService.CLists, _objectsService.Cbfile)
                    = mainWindow.MainWindowConstructor(request.UserId);

                if (_objectsService.CDefualts == null || _objectsService.CLists == null || _objectsService.Cbfile == null)
                {
                    _logger.LogError("MainWindowConstructor returned null for UserId: {UserId}", request.UserId);
                    return BadRequest(new { message = "Failed to initialize session — MainWindowConstructor returned null." });
                }

                SetOptionBoxes(_objectsService.CDefualts, _objectsService.CLists);
                InitializeServices(request.UserId);

                _objectsService.UserIndex = request.UserId;

                return Ok(new BeginSessionResponse
                {
                    UserId = request.UserId,
                    Message = "Session initialized successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CRITICAL ERROR in BeginSession for UserId: {UserId}", request?.UserId);
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get available OKTA user groups.
        /// </summary>
        [HttpGet("groups")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public IActionResult GetGroups()
        {
            var groups = new List<string>
            {
                "OKTA_GAF_Intugent_RnD_Users",
                "OKTA_GAF_Intugent_Mfg_Users_Gainesville",
                "OKTA_GAF_Intugent_Mfg_Uses_Stratesboro",
                "OKTA_GAF_Intugent_Mfg_Users_CedarCity",
                "OKTA_GAF_Intugent_Mfg_Users_NewColumbia",
                "OKTA_GAF_Intugent_Admins"
            };

            return Ok(groups);
        }

        private void InitializeServices(int userId)
        {
            // Production Targets
            _objectsService.CProdTargets = new CProdTargets(_objectsService.Cbfile, _objectsService.CDefualts);

            // Matrix
            _objectsService.cMatrix = new cMatrix();

            // Database
            _objectsService.CDBase = new CDBase(_objectsService.Cbfile);

            // Neural Network
            _objectsService.CNNData = new CNNData(_objectsService.Cbfile);
            _objectsService.CNNModel = new CNNModel(_objectsService.CNNData);

            // Analysis
            _objectsService.CAnalysisData = new CAnalysisData(_objectsService.Cbfile, _objectsService.CDefualts);

            // MFG Pages
            _objectsService.MfgHome = new MfgHome(_objectsService.CDefualts, _objectsService.CLists, _objectsService.Cbfile);
            _objectsService.MfgInProcess = new MfgInProcess(_objectsService.Cbfile);
            _objectsService.CIPProdTargets = new CIPProdTargets(_objectsService.Cbfile, _objectsService.MfgInProcess);
            _objectsService.MfgFinishedGoods = new MfgFinishedGoods(_objectsService.Cbfile);
            _objectsService.MfgDimStability = new MfgDimStability(_objectsService.Cbfile);
            _objectsService.MfgPlantData = new MfgPlantData(_objectsService.Cbfile, _objectsService.CLists, _objectsService.CDefualts);
            _objectsService.MfgJetMixing = new MfgJetMixing(_objectsService.CLists);
            _objectsService.MfgProcessCheck = new MfgProcessCheck(_objectsService.Cbfile, _objectsService.CDefualts);
            _objectsService.MfgReports = new MfgReports(_objectsService.Cbfile, _objectsService.CDefualts);

            // R&D Pages
            _objectsService.RNDHome = new RNDHome(_objectsService.CDefualts, _objectsService.CLists, _objectsService.Cbfile);
            _objectsService.RNDFormulations = new RNDFormulations(_objectsService.CDefualts, _objectsService.Cbfile, _objectsService.RNDHome);
            _objectsService.RNDRValues = new RNDRValues(_objectsService.CLists);
            _objectsService.RNDRawProps = new RNDRawProps();
            _objectsService.RNDProperties = new RNDProperties();
            _objectsService.RNDTDRV = new RNDTDRV();

            // Admin & AI
            _objectsService.MfgAdmin = new MfgAdmin(_objectsService.Cbfile);
            _objectsService.AIModel = new AIModel();
        }

        private void SetOptionBoxes(CDefualts defualts, CLists lists)
        {
            if (lists.dtComProd == null) return;

            // Ensure columns exist
            if (!lists.dtComProd.Columns.Contains("Product Code"))
                lists.dtComProd.Columns.Add("Product Code", typeof(string));

            if (!lists.dtComProd.Columns.Contains("Product"))
                lists.dtComProd.Columns.Add("Product", typeof(string));

            // Make a copy for dvComProd (preserve original)
            lists.dvComProd = lists.dtComProd.Copy().DefaultView;

            // Add "All" row to the original table
            var dr = lists.dtComProd.NewRow();
            dr["Product Code"] = defualts.sProdMfgAll ?? "All";
            dr["Product"] = defualts.sProdMfgAll ?? "All";
            lists.dtComProd.Rows.InsertAt(dr, 0);

            // dvComProdAll includes the "All" row
            lists.dvComProdAll = lists.dtComProd.DefaultView;
        }
    }

    // DTOs
    public class BeginSessionRequest
    {
        public int UserId { get; set; }
    }

    public class BeginSessionResponse
    {
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}