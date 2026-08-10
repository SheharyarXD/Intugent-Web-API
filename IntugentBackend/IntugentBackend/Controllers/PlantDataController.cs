using IntugentBackend.Models;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Mfg;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantDataController : ControllerBase
    {
        private readonly MfgPlantData _mfgPlantData;
        private readonly Cbfile _cbfile;
        private readonly MfgHome _mfgHome;
        private readonly CLists _cLists;
        private readonly MfgInProcess _mfgInProcess;
        private readonly MfgFinishedGoods _mfgFinishedGoods;
        private readonly MfgDimStability _mfgDimStability;
        private readonly MfgJetMixing _mfgJetMixing;
        private readonly CDefualts _cDefualts;
        private readonly ILogger<PlantDataController> _logger;

        public PlantDataController(
            MfgPlantData mfgPlantData,
            Cbfile cbfile,
            MfgHome mfgHome,
            CLists cLists,
            MfgInProcess mfgInProcess,
            MfgFinishedGoods mfgFinishedGoods,
            MfgDimStability mfgDimStability,
            MfgJetMixing mfgJetMixing,
            CDefualts cDefualts,
            ILogger<PlantDataController> logger)
        {
            _mfgPlantData = mfgPlantData;
            _cbfile = cbfile;
            _mfgHome = mfgHome;
            _cLists = cLists;
            _mfgInProcess = mfgInProcess;
            _mfgFinishedGoods = mfgFinishedGoods;
            _mfgDimStability = mfgDimStability;
            _mfgJetMixing = mfgJetMixing;
            _cDefualts = cDefualts;
            _logger = logger;
        }

        [HttpGet("data")]
        public IActionResult GetData()
        {
            try
            {
                _mfgPlantData.GetDataSet();
                LinkCrossReferences();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Plant Data");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("navigate")]
        public IActionResult Navigate([FromBody] PlantDataNavigateRequest request)
        {
            try
            {
                if (!_cbfile.bCanSwitchRecord)
                    return Ok(new { success = false, error = _cbfile.sNoRecSwitchMsg });

                switch (request.Direction)
                {
                    case "prev": _cbfile.iIDMfgIndex += 1; break;
                    case "next": _cbfile.iIDMfgIndex -= 1; break;
                    default: return BadRequest(new { success = false, error = "Invalid direction" });
                }

                if (_cbfile.iIDMfgIndex < 0) _cbfile.iIDMfgIndex = 0;
                if (_cbfile.iIDMfgIndex > _mfgHome.dt.Rows.Count - 1) _cbfile.iIDMfgIndex = _mfgHome.dt.Rows.Count - 1;

                _cbfile.iIDMfg = (int)_mfgHome.dt.Rows[_cbfile.iIDMfgIndex]["ID4ALL"];
                _cLists.drEmployee["MfgIDSelected"] = _cbfile.iIDMfg;
                _cLists.UpdateEmployee();

                _mfgHome.GetAllMfgData(_mfgInProcess, _mfgFinishedGoods, _mfgDimStability, _mfgPlantData, _mfgJetMixing);
                _mfgPlantData.GetDataSet();
                LinkCrossReferences();

                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating Plant Data dataset");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// "Extract Relevant Process Data" button — pulls averaged process values from the data lake
        /// around the Finished Board time stamp and persists them.
        /// </summary>
        [HttpPost("extract")]
        public IActionResult Extract()
        {
            try
            {
                string result = _mfgPlantData.GetPlantData(_mfgPlantData.dtFGTime);
                var dto = BuildDto();
                dto.Message = result;
                return Ok(new { success = true, data = dto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting plant process data");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // ========== HELPERS ==========

        private void LinkCrossReferences()
        {
            _mfgPlantData.drIP = _mfgInProcess.dr;
            _mfgPlantData.drFG = _mfgFinishedGoods.dr;
        }

        private static List<ProcessParamRow> ToRows(DataTable? dt)
        {
            var rows = new List<ProcessParamRow>();
            if (dt == null) return rows;
            foreach (DataRow row in dt.Rows)
            {
                rows.Add(new ProcessParamRow
                {
                    Name = row.Table.Columns.Contains("sName") ? row["sName"]?.ToString() ?? string.Empty : string.Empty,
                    Value = row.Table.Columns.Contains("sValue") ? row["sValue"]?.ToString() ?? string.Empty : string.Empty
                });
            }
            return rows;
        }

        private PlantDataDto BuildDto()
        {
            var mfg = _mfgPlantData;
            var dto = new PlantDataDto();

            dto.GDataSetNextIsEnabled = _cbfile.iIDMfgIndex != 0;
            dto.GDataSetPrevIsEnabled = _cbfile.iIDMfgIndex != _mfgHome.dt.Rows.Count - 1;

            dto.GDelTimeButton = "* The two Board Time Stamps must be within " + _cDefualts.dDelTimeButton +
                " minute(s) (site specific) of each other to extract process data.";

            if (mfg.dr == null) return dto;

            dto.GID = mfg.dr["ID4ALL"]?.ToString() ?? string.Empty;
            dto.GProductCode = mfg.drIP != null && mfg.drIP["Product ID"] != DBNull.Value ? mfg.drIP["Product ID"].ToString()! : string.Empty;

            bool timeStampsWithin5Min = true;
            mfg.dtFGTime = DateTime.Now;
            mfg.dtIPTime = mfg.dtFGTime.AddDays(10);
            mfg.dtQCCheckTime = mfg.dtFGTime.AddDays(-10);

            if (mfg.drIP == null || mfg.drIP["Test Date"] == DBNull.Value)
            {
                dto.GProductionDate = string.Empty;
            }
            else
            {
                dto.GProductionDate = ((DateTime)mfg.drIP["Test Date"]).ToString("yyyy-MM-ddTHH:mm");
            }

            if (mfg.drIP == null || mfg.drIP["Test Date"] == DBNull.Value)
            {
                timeStampsWithin5Min = false;
                dto.GProductionTime = string.Empty;
            }
            else
            {
                mfg.dtIPTime = (DateTime)mfg.drIP["Test Date"];
                dto.GProductionTime = mfg.dtIPTime.ToString("yyyy-MM-ddTHH:mm");
            }

            if (mfg.drIP == null || mfg.drIP["Time of Pour Table QC Check"] == DBNull.Value)
            {
                dto.GQCCheckTime = string.Empty;
            }
            else
            {
                mfg.dtQCCheckTime = (DateTime)mfg.drIP["Time of Pour Table QC Check"];
                dto.GQCCheckTime = mfg.dtQCCheckTime.ToString("HH:mm:ss");
            }

            if (mfg.drFG == null || mfg.drFG["Finished Board Time Stamp FG"] == DBNull.Value)
            {
                timeStampsWithin5Min = false;
                dto.GFBTime = string.Empty;
            }
            else
            {
                mfg.dtFGTime = (DateTime)mfg.drFG["Finished Board Time Stamp FG"];
                dto.GFBTime = mfg.dtFGTime.ToString("yyyy-MM-ddTHH:mm");
            }

            if (timeStampsWithin5Min && Math.Abs((mfg.dtIPTime - mfg.dtFGTime).TotalMinutes) > _cDefualts.dDelTimeButton)
                timeStampsWithin5Min = false;

            dto.GGetPlantDataIsEnabled = timeStampsWithin5Min;

            dto.GChemDel = ToRows(mfg.dtPPChemDel);
            dto.GChemDel1 = ToRows(mfg.dtPPChemDel1);
            dto.GPTable = ToRows(mfg.dtPPPTable);
            dto.GDBelt = ToRows(mfg.dtPPDBelt);
            dto.GOthers = ToRows(mfg.dtPPOthers);
            dto.GNewInsData = ToRows(mfg.dtNewInsData);

            return dto;
        }
    }
}
