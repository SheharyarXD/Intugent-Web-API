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
        private readonly Cbfile _cbfile;
        private readonly CDefualts _cDefualts;
        private readonly CLists _cLists;
        private readonly MainWindow _mainWindow;
        private readonly MfgJetMixing _mfgJetMixing;
        private readonly MfgInProcess _mfgInProcess;
        private readonly SessionState _sessionState;
        private readonly ILogger<SessionController> _logger;

        public SessionController(
            Cbfile cbfile,
            CDefualts cDefualts,
            CLists cLists,
            MainWindow mainWindow,
            MfgJetMixing mfgJetMixing,
            MfgInProcess mfgInProcess,
            SessionState sessionState,
            ILogger<SessionController> logger)
        {
            _cbfile = cbfile;
            _cDefualts = cDefualts;
            _cLists = cLists;
            _mainWindow = mainWindow;
            _mfgJetMixing = mfgJetMixing;
            _mfgInProcess = mfgInProcess;
            _sessionState = sessionState;
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

                bool ok = _mainWindow.MainWindowConstructor(request.UserId);

                if (!ok)
                {
                    _logger.LogError("MainWindowConstructor failed for UserId: {UserId}", request.UserId);
                    return BadRequest(new { message = "Failed to initialize session." });
                }

                SetOptionBoxes(_cDefualts, _cLists);

                // These two snapshot data at construction time, so they must be re-run
                // now that CLists/Cbfile have been refreshed for this session.
                _mfgJetMixing.Startup();
                _mfgInProcess.GetDataSet();

                _sessionState.UserIndex = request.UserId;

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

            // Employee (Chemist/Operator) dropdowns — Roster rows filtered by location.
            // This mirrors legacy MainWindow.SetOptionBoxes(), which was never actually wired up
            // (it only existed inside a commented-out WPF block), leaving these lists empty.
            if (lists.dtLoc != null && lists.dtLoc.Columns.Contains("IDLocation") && lists.dtLoc.Columns.Contains("Employees"))
            {
                lists.dvEmployees = lists.dtLoc.DefaultView;

                lists.dvEmployees.RowFilter = "IDLocation = 3";
                lists.dvEmployees.Sort = "Employees Asc";
                lists.dvEmployeesRND = lists.dvEmployees.ToTable().DefaultView;

                lists.dvEmployees.RowFilter = "IDLocation = " + defualts.IDLocation;
                lists.dvEmployees.Sort = "Employees Asc";
                lists.dvEmployeesMfg = lists.dvEmployees.ToTable().DefaultView;

                lists.dvEmployees.RowFilter = string.Empty;
            }
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