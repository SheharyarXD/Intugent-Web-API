//using IntugentClassLibrary.Classes;
//using IntugentClassLibrary.Pages.Admins;
//using IntugentClassLibrary.Pages.Mfg;
//using IntugentClassLibrary.Pages.Rnd;
//using IntugentClassLibrary.Utilities;
//using IntugentClassLbrary.Classes;
//using IntugentClassLbrary.Pages;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Admin;
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

        // POST api/session/begin
        // replaces: OnPostGBeginSession_Click
        [HttpPost("begin")]
        public IActionResult BeginSession([FromBody] int userId)
        {
            MainWindow mainWindow = new MainWindow();
            (_objectsService.CDefualts, _objectsService.CLists, _objectsService.Cbfile)
                = mainWindow.MainWindowConstructor(userId);

            if (_objectsService.CDefualts == null ||
                _objectsService.CLists == null ||
                _objectsService.Cbfile == null)
            {
                return BadRequest("Failed to initialize session — MainWindowConstructor returned null.");
            }

            SetOptionBoxes(_objectsService.CDefualts, _objectsService.CLists);

            _objectsService.CProdTargets = new CProdTargets(_objectsService.Cbfile, _objectsService.CDefualts);
            _objectsService.cMatrix = new cMatrix();
            _objectsService.CDBase = new CDBase(_objectsService.Cbfile);
            _objectsService.CNNData = new CNNData(_objectsService.Cbfile);
            _objectsService.CNNModel = new CNNModel(_objectsService.CNNData);
            _objectsService.CAnalysisData = new CAnalysisData(_objectsService.Cbfile, _objectsService.CDefualts);

            _objectsService.MfgHome = new MfgHome(_objectsService.CDefualts, _objectsService.CLists, _objectsService.Cbfile);
            _objectsService.MfgInProcess = new MfgInProcess(_objectsService.Cbfile);
            _objectsService.CIPProdTargets = new CIPProdTargets(_objectsService.Cbfile, _objectsService.MfgInProcess);
            _objectsService.MfgFinishedGoods = new MfgFinishedGoods(_objectsService.Cbfile);
            _objectsService.MfgDimStability = new MfgDimStability(_objectsService.Cbfile);
            _objectsService.MfgPlantData = new MfgPlantData(_objectsService.Cbfile, _objectsService.CLists, _objectsService.CDefualts);
            _objectsService.MfgJetMixing = new MfgJetMixing(_objectsService.CLists);
            _objectsService.MfgProcessCheck = new MfgProcessCheck(_objectsService.Cbfile, _objectsService.CDefualts);
            _objectsService.MfgReports = new MfgReports(_objectsService.Cbfile, _objectsService.CDefualts);

            _objectsService.RNDHome = new RNDHome(_objectsService.CDefualts, _objectsService.CLists, _objectsService.Cbfile);
            _objectsService.RNDFormulations = new RNDFormulations(_objectsService.CDefualts, _objectsService.Cbfile, _objectsService.RNDHome);
            _objectsService.RNDRValues = new RNDRValues(_objectsService.CLists);
            _objectsService.RNDRawProps = new RNDRawProps();
            _objectsService.RNDProperties = new RNDProperties();
            _objectsService.RNDTDRV = new RNDTDRV();

            _objectsService.MfgAdmin = new MfgAdmin(_objectsService.Cbfile);
            _objectsService.AIModel = new AIModel();

            _objectsService.UserIndex = userId;

            return Ok(new { userId, message = "Session initialized successfully." });
        }

        // GET api/session/groups
        // replaces: OnGet (the gGroup list)
        [HttpGet("groups")]
        public IActionResult GetGroups()
        {
            var groups = new List<string>
            {
                "OKTA_GAF_Intugent_RnD_Users",
                "OKTA_GAF_Intugent_Mfg_Users_Gainesville",
                "OKTA_GAF_Intugent_Mfg_Users_Statesboro",
                "OKTA_GAF_Intugent_Mfg_Users_CedarCity",
                "OKTA_GAF_Intugent_Mfg_Users_NewColumbia",
                "OKTA_GAF_Intugent_Admins"
            };

            return Ok(groups);
        }

        // Private helper — same logic as SetOptionBoxes
        private void SetOptionBoxes(CDefualts defualts, CLists lists)
        {
            lists.dvComProd = lists.dtComProd.DefaultView.ToTable().DefaultView;

            var dr = lists.dtComProd.NewRow();
            dr["Product Code"] = defualts.sProdMfgAll;
            dr["Product"] = defualts.sProdMfgAll;
            lists.dtComProd.Rows.InsertAt(dr, 0);

            lists.dvComProdAll = lists.dtComProd.DefaultView;
        }
    }
}