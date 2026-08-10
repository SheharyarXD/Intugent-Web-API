using IntugentBackend.Models;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using Microsoft.AspNetCore.Mvc;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InProcessBoardPropertiesController : ControllerBase
    {
        private readonly MfgInProcess _mfgInProcess;
        private readonly Cbfile _cbfile;
        private readonly MfgHome _mfgHome;
        private readonly CLists _cLists;
        private readonly MfgFinishedGoods _mfgFinishedGoods;
        private readonly MfgDimStability _mfgDimStability;
        private readonly MfgPlantData _mfgPlantData;
        private readonly MfgJetMixing _mfgJetMixing;
        private readonly CDefualts _cDefualts;
        private readonly CIPProdTargets _cipProdTargets;
        private readonly SessionState _sessionState;
        private readonly ILogger<InProcessBoardPropertiesController> _logger;

        public InProcessBoardPropertiesController(
            MfgInProcess mfgInProcess,
            Cbfile cbfile,
            MfgHome mfgHome,
            CLists cLists,
            MfgFinishedGoods mfgFinishedGoods,
            MfgDimStability mfgDimStability,
            MfgPlantData mfgPlantData,
            MfgJetMixing mfgJetMixing,
            CDefualts cDefualts,
            CIPProdTargets cipProdTargets,
            SessionState sessionState,
            ILogger<InProcessBoardPropertiesController> logger)
        {
            _mfgInProcess = mfgInProcess;
            _cbfile = cbfile;
            _mfgHome = mfgHome;
            _cLists = cLists;
            _mfgFinishedGoods = mfgFinishedGoods;
            _mfgDimStability = mfgDimStability;
            _mfgPlantData = mfgPlantData;
            _mfgJetMixing = mfgJetMixing;
            _cDefualts = cDefualts;
            _cipProdTargets = cipProdTargets;
            _sessionState = sessionState;
            _logger = logger;
        }

        [HttpGet("data")]
        public IActionResult GetData()
        {
            try
            {
                if (_mfgInProcess == null)
                    return BadRequest(new { success = false, error = "MfgInProcess not initialized." });

                _mfgInProcess.GetDataSet();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading In Process Board Properties data");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("navigate")]
        public IActionResult Navigate([FromBody] IPNavigateRequest request)
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

                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating In Process dataset");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-thickness")]
        public IActionResult UpdateThickness([FromBody] IPFieldUpdateRequest request)
        {
            try
            {
                _mfgInProcess.bDataSetChanged = true;
                switch (request.Name)
                {
                    case "gThicknessIP_1": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 1"); break;
                    case "gThicknessIP_2": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 2"); break;
                    case "gThicknessIP_3": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 3"); break;
                    case "gThicknessIP_4": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 4"); break;
                    case "gThicknessIP_5": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 5"); break;
                    case "gThicknessIP_6": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 6"); break;
                    case "gThicknessIP_7": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 7"); break;
                    case "gThicknessIP_8": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 8"); break;
                    case "gThicknessIP_9": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 9"); break;
                    case "gThicknessIP_10": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 10"); break;
                    case "gThicknessIP_11": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 11"); break;
                    case "gThicknessIP_12": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 12"); break;
                    case "gThicknessIP_13": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 13"); break;
                    case "gThicknessIP_14": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 14"); break;
                    case "gThicknessIP_15": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 15"); break;
                    case "gThicknessIP_16": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 16"); break;
                    case "gThicknessIP_17": _mfgInProcess.SetDoubleFieldValue(request.Value, "Thickness IP - 17"); break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                _mfgInProcess.UpdateThicknessCalculations();
                _mfgInProcess.UpdateDataSet();

                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating thickness field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-compressive")]
        public IActionResult UpdateCompressive([FromBody] IPFieldUpdateRequest request)
        {
            try
            {
                _mfgInProcess.bDataSetChanged = true;
                switch (request.Name)
                {
                    case "gCompressiveIP_1": _mfgInProcess.SetDoubleFieldValue(request.Value, "Compressive IP - 1"); break;
                    case "gCompressiveIP_2": _mfgInProcess.SetDoubleFieldValue(request.Value, "Compressive IP - 2"); break;
                    case "gCompressiveIP_3": _mfgInProcess.SetDoubleFieldValue(request.Value, "Compressive IP - 3"); break;
                    case "gCompressiveIP_4": _mfgInProcess.SetDoubleFieldValue(request.Value, "Compressive IP - 4"); break;
                    case "gCompressiveIP_5": _mfgInProcess.SetDoubleFieldValue(request.Value, "Compressive IP - 5"); break;
                    case "gCompressiveIP_6": _mfgInProcess.SetDoubleFieldValue(request.Value, "Compressive IP - 6"); break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                _mfgInProcess.UpdateCompressiveStrengthCalculations();
                _mfgInProcess.UpdateDataSet();

                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating compressive strength field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-board-dims")]
        public IActionResult UpdateBoardDims([FromBody] IPFieldUpdateRequest request)
        {
            try
            {
                _mfgInProcess.bDataSetChanged = true;
                bool isDiagonal = false;

                switch (request.Name)
                {
                    case "gLength": _mfgInProcess.SetDoubleFieldValue(request.Value, "Length"); break;
                    case "gWidth": _mfgInProcess.SetDoubleFieldValue(request.Value, "Width"); break;
                    case "gBundleHeightIP": _mfgInProcess.SetDoubleFieldValue(request.Value, "Bundle Height IP"); break;
                    case "gDiagoanl1": _mfgInProcess.SetDoubleFieldValue(request.Value, "IP Diagonal 1"); isDiagonal = true; break;
                    case "gDiagoanl2": _mfgInProcess.SetDoubleFieldValue(request.Value, "IP Diagonal 2"); isDiagonal = true; break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                if (isDiagonal && _mfgInProcess.dr["IP Diagonal 1"] != DBNull.Value && _mfgInProcess.dr["IP Diagonal 2"] != DBNull.Value)
                {
                    double diff = Math.Abs((double)_mfgInProcess.dr["IP Diagonal 1"] - (double)_mfgInProcess.dr["IP Diagonal 2"]);
                    _mfgInProcess.dr["IP Diagonal Diff"] = diff;
                }

                _mfgInProcess.UpdateDataSet();

                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating board dimension field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-knit-line")]
        public IActionResult UpdateKnitLine([FromBody] IPFieldUpdateRequest request)
        {
            try
            {
                _mfgInProcess.bDataSetChanged = true;
                switch (request.Name)
                {
                    case "gKnitLineLoc_1": _mfgInProcess.SetDoubleFieldValue(request.Value, "Knit Line Loc 1"); break;
                    case "gKnitLineLoc_2": _mfgInProcess.SetDoubleFieldValue(request.Value, "Knit Line Loc 2"); break;
                    case "gKnitLineLoc_3": _mfgInProcess.SetDoubleFieldValue(request.Value, "Knit Line Loc 3"); break;
                    case "gKnitLineLoc_4": _mfgInProcess.SetDoubleFieldValue(request.Value, "Knit Line Loc 4"); break;
                    case "gKnitLineLoc_5": _mfgInProcess.SetDoubleFieldValue(request.Value, "Knit Line Loc 5"); break;
                    case "gKnitLineLoc_6": _mfgInProcess.SetDoubleFieldValue(request.Value, "Knit Line Loc 6"); break;
                    case "gKnitLineLoc_7": _mfgInProcess.SetDoubleFieldValue(request.Value, "Knit Line Loc 7"); break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                _mfgInProcess.UpdateDataSet();

                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating knit line field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-board-perf")]
        public IActionResult UpdateBoardPerf([FromBody] IPBoolUpdateRequest request)
        {
            try
            {
                _mfgInProcess.bDataSetChanged = true;
                switch (request.Name)
                {
                    case "gTopBoardPerforated": _mfgInProcess.dr["Top Board Perforated"] = request.Value; break;
                    case "gBottomBoardPerforated": _mfgInProcess.dr["Bottom Board Perforated"] = request.Value; break;
                    case "gTopBoardPrintOK": _mfgInProcess.dr["Top Board Print OK"] = request.Value; break;
                    case "gBottomBoardPrintOK": _mfgInProcess.dr["Bottom Board Print OK"] = request.Value; break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                _mfgInProcess.UpdateDataSet();

                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating board perforation field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // ========== DTO BUILDER ==========

        private InProcessBoardDataDto BuildDto()
        {
            var mfg = _mfgInProcess;
            var defs = _cDefualts;
            var dto = new InProcessBoardDataDto();

            dto.GDataSetNextIsEnabled = _cbfile.iIDMfgIndex != 0;
            dto.GDataSetPrevIsEnabled = _cbfile.iIDMfgIndex != _mfgHome.dt.Rows.Count - 1;

            dto.GsLoc1A = dto.GsLoc1E = defs.sLocMfg1.ToUpper();
            dto.GsLoc2A = defs.sLocMfg2.ToUpper();
            dto.GsLoc3A = dto.GsLoc3E = dto.GsLoc3F = defs.sLocMfg3.ToUpper();
            dto.GsLoc1B = dto.GsLoc1C = dto.GsLoc1D = defs.sLocMfg1;
            dto.GsLoc2B = dto.GsLoc2C = dto.GsLoc2D = defs.sLocMfg2;
            dto.GsLoc3B = dto.GsLoc3C = dto.GsLoc3D = defs.sLocMfg3;

            if (mfg.dr == null) return dto;

            dto.GTestDateTime = mfg.dr["Test Date"] == DBNull.Value ? null : (DateTime)mfg.dr["Test Date"];
            dto.GID = mfg.dr["ID4ALL"]?.ToString() ?? string.Empty;
            dto.GOperatorSelectedItem = mfg.dr["Operator"] == DBNull.Value ? -1 : (int)mfg.dr["Operator"];
            dto.GTimeStampNotLegibleIsChecked = mfg.dr["Click box if time stamp is NOT legible"] != DBNull.Value && (bool)mfg.dr["Click box if time stamp is NOT legible"];
            dto.GShiftSelectedValue = mfg.dr["Shift"] == DBNull.Value ? -1 : (int)mfg.dr["Shift"];
            dto.GProdIDSelectedValue = mfg.dr["Product ID"] == DBNull.Value ? string.Empty : mfg.dr["Product ID"].ToString()!;
            dto.GPaperManufacturerSelectedValue = mfg.dr["Paper Manufacturer"] == DBNull.Value ? -1 : (int)mfg.dr["Paper Manufacturer"];
            dto.GRunningWetDesnsity = mfg.SetDoubleTextField("Running Wet Density");
            dto.GWarehouseTemp = mfg.SetDoubleTextField("Warehouse Temp");
            dto.GWarehouseHumidity = mfg.SetDoubleTextField("Warehouse Humidity");

            if (mfg.dr["IP Testing Complete"] == DBNull.Value)
            {
                dto.GInProcessDoneIsChecked = false;
                _sessionState.gInProcessDoneIsChecked = false;
            }
            else
            {
                dto.GInProcessDoneIsChecked = (bool)mfg.dr["IP Testing Complete"];
                _sessionState.gInProcessDoneIsChecked = dto.GInProcessDoneIsChecked;
            }

            dto.GRunTypeSelectedValue = mfg.dr["Run Type"] == DBNull.Value ? -1 : (int)mfg.dr["Run Type"];
            dto.GAbandonedIsChecked = mfg.dr["Abandoned"] != DBNull.Value && (bool)mfg.dr["Abandoned"];

            dto.GCoreDensityIPText = mfg.SetDoubleTextField("Core Density - IP", MfgInProcess.sOr);
            dto.GThicknessIPText = mfg.SetDoubleTextField("Thickness - IP", MfgInProcess.sOr);
            dto.GCompressiveIPText = mfg.SetDoubleTextField("Compressive Strength - IP", MfgInProcess.sOr);
            dto.GCompressiveIP5Text = mfg.SetDoubleTextField("Compressive Strength 5 - IP", MfgInProcess.sOr);
            dto.GThicknessValleyText = mfg.SetDoubleTextField("thickness IP- valleys", MfgInProcess.sOr);
            dto.GThicknessPeakText = mfg.SetDoubleTextField("thickness peaks IP", MfgInProcess.sOr);
            dto.GFlatnessText = mfg.SetDoubleTextField("Flatness IP", MfgInProcess.sOr);

            dto.GLengthText = mfg.SetDoubleTextField("Length");
            dto.GWidthText = mfg.SetDoubleTextField("Width");
            dto.GBundleHeightIPText = mfg.SetDoubleTextField("Bundle Height IP");
            dto.GDiagoanl1Text = mfg.SetDoubleTextField("IP Diagonal 1");
            dto.GDiagoanl2Text = mfg.SetDoubleTextField("IP Diagonal 2");
            dto.GDiagoanlDiffText = mfg.SetDoubleTextField("IP Diagonal Diff", MfgInProcess.sOr);

            for (int i = 1; i <= 17; i++)
            {
                var value = mfg.SetDoubleTextField($"Thickness IP - {i}");
                typeof(InProcessBoardDataDto).GetProperty($"GThicknessIP_{i}Text")!.SetValue(dto, value);
            }

            dto.GTopBoardPerforatedIsChecked = mfg.dr["Top Board Perforated"] != DBNull.Value && (bool)mfg.dr["Top Board Perforated"];
            dto.GBottomBoardPerforatedIsChecked = mfg.dr["Bottom Board Perforated"] != DBNull.Value && (bool)mfg.dr["Bottom Board Perforated"];
            dto.GTopBoardPrintOKIsChecked = mfg.dr["Top Board Print OK"] != DBNull.Value && (bool)mfg.dr["Top Board Print OK"];
            dto.GBottomBoardPrintOKIsChecked = mfg.dr["Bottom Board Print OK"] != DBNull.Value && (bool)mfg.dr["Bottom Board Print OK"];

            dto.GCompressiveIP_1Text = mfg.SetDoubleTextField("Compressive IP - 1");
            dto.GCompressiveIP_2Text = mfg.SetDoubleTextField("Compressive IP - 2");
            dto.GCompressiveIP_3Text = mfg.SetDoubleTextField("Compressive IP - 3");
            dto.GCompressiveIP_4Text = mfg.SetDoubleTextField("Compressive IP - 4");
            dto.GCompressiveIP_5Text = mfg.SetDoubleTextField("Compressive IP - 5");
            dto.GCompressiveIP_6Text = mfg.SetDoubleTextField("Compressive IP - 6");

            dto.GCompStrKnitPresent_1IsChecked = mfg.dr["Comp Str Knit Present 1"] != DBNull.Value && (bool)mfg.dr["Comp Str Knit Present 1"];
            dto.GCompStrKnitPresent_2IsChecked = mfg.dr["Comp Str Knit Present 2"] != DBNull.Value && (bool)mfg.dr["Comp Str Knit Present 2"];
            dto.GCompStrKnitPresent_3IsChecked = mfg.dr["Comp Str Knit Present 3"] != DBNull.Value && (bool)mfg.dr["Comp Str Knit Present 3"];
            dto.GCompStrKnitPresent_4IsChecked = mfg.dr["Comp Str Knit Present 4"] != DBNull.Value && (bool)mfg.dr["Comp Str Knit Present 4"];
            dto.GCompStrKnitPresent_5IsChecked = mfg.dr["Comp Str Knit Present 5"] != DBNull.Value && (bool)mfg.dr["Comp Str Knit Present 5"];
            dto.GCompStrKnitPresent_6IsChecked = mfg.dr["Comp Str Knit Present 6"] != DBNull.Value && (bool)mfg.dr["Comp Str Knit Present 6"];

            dto.GMass_1Text = mfg.SetDoubleTextField("Mass 1");
            dto.GL1_1Text = mfg.SetDoubleTextField("L1 1");
            dto.GW1_1Text = mfg.SetDoubleTextField("W1 1");
            dto.GT1_1Text = mfg.SetDoubleTextField("T1 1");
            dto.GT2_1Text = mfg.SetDoubleTextField("T2 1");
            dto.GT3_1Text = mfg.SetDoubleTextField("T3 1");
            dto.GT4_1Text = mfg.SetDoubleTextField("T4 1");
            dto.GT5_1Text = mfg.SetDoubleTextField("T5 1");
            dto.GCoreKnitPresent_1IsChecked = mfg.dr["Core Knit Present 1"] != DBNull.Value && (bool)mfg.dr["Core Knit Present 1"];
            dto.GCoreDensityIP_1Text = mfg.SetDoubleTextField("Core Density - IP 1", MfgInProcess.sOr);

            dto.GMass_2Text = mfg.SetDoubleTextField("Mass 2");
            dto.GL1_2Text = mfg.SetDoubleTextField("L1 2");
            dto.GW1_2Text = mfg.SetDoubleTextField("W1 2");
            dto.GT1_2Text = mfg.SetDoubleTextField("T1 2");
            dto.GT2_2Text = mfg.SetDoubleTextField("T2 2");
            dto.GT3_2Text = mfg.SetDoubleTextField("T3 2");
            dto.GT4_2Text = mfg.SetDoubleTextField("T4 2");
            dto.GT5_2Text = mfg.SetDoubleTextField("T5 2");
            dto.GCoreKnitPresent_2IsChecked = mfg.dr["Core Knit Present 2"] != DBNull.Value && (bool)mfg.dr["Core Knit Present 2"];
            dto.GCoreDensityIP_2Text = mfg.SetDoubleTextField("Core Density - IP 2", MfgInProcess.sOr);

            dto.GMass_3Text = mfg.SetDoubleTextField("Mass 3");
            dto.GL1_3Text = mfg.SetDoubleTextField("L1 3");
            dto.GW1_3Text = mfg.SetDoubleTextField("W1 3");
            dto.GT1_3Text = mfg.SetDoubleTextField("T1 3");
            dto.GT2_3Text = mfg.SetDoubleTextField("T2 3");
            dto.GT3_3Text = mfg.SetDoubleTextField("T3 3");
            dto.GT4_3Text = mfg.SetDoubleTextField("T4 3");
            dto.GT5_3Text = mfg.SetDoubleTextField("T5 3");
            dto.GCoreKnitPresent_3IsChecked = mfg.dr["Core Knit Present 3"] != DBNull.Value && (bool)mfg.dr["Core Knit Present 3"];
            dto.GCoreDensityIP_3Text = mfg.SetDoubleTextField("Core Density - IP 3", MfgInProcess.sOr);

            if (mfg.dr["Time of Pour Table QC Check"] != DBNull.Value)
            {
                dto.GTimePourTableQCCheckCustomFormat = mfg.sTimeFormat;
                dto.GTimePourTableQCCheckValue = (DateTime)mfg.dr["Time of Pour Table QC Check"];
            }
            dto.GSurfactantSelectedValue = mfg.dr["Surfactant Type"] == DBNull.Value ? -1 : (int)mfg.dr["Surfactant Type"];
            dto.GLayoutSelectedValue = mfg.dr["Pour Table Layout"] == DBNull.Value ? -1 : (int)mfg.dr["Pour Table Layout"];

            dto.GSplitterAgeText = mfg.SetIntTextField("Splitter Age (minutes)");
            dto.GHeadPlateAgeText = mfg.SetIntTextField("Headplate Age / Pour Run Time (minutes)");

            dto.GPourHeadPosition_1Text = mfg.SetDoubleTextField("Pour Head Position - 1");
            dto.GPourHeadPosition_2Text = mfg.SetDoubleTextField("Pour Head Position - 2");
            dto.GPourHeadPosition_3Text = mfg.SetDoubleTextField("Pour Head Position - 3");

            dto.GIRFacerTempLowerText = mfg.SetDoubleTextField("IR Facer Temp - Lower");
            dto.GIRFacerTempUpperText = mfg.SetDoubleTextField("IR Facer Temp - Upper");

            dto.GIRStreamTempPourHead_1Text = mfg.SetDoubleTextField("IR Stream Temp - Pour Head 1");
            dto.GIRStreamTempPourHead_2Text = mfg.SetDoubleTextField("IR Stream Temp - Pour Head 2");
            dto.GIRStreamTempPourHead_3Text = mfg.SetDoubleTextField("IR Stream Temp - Pour Head 3");

            dto.GIRNipRoll_1Text = mfg.SetDoubleTextField("IR Stream Temp - Nipp Roll 1");
            dto.GIRNipRoll_2Text = mfg.SetDoubleTextField("IR Stream Temp - Nipp Roll 2");
            dto.GIRNipRoll_3Text = mfg.SetDoubleTextField("IR Stream Temp - Nipp Roll 3");

            dto.GKnitLineLoc_1Text = mfg.SetDoubleTextField("Knit Line Loc 1");
            dto.GKnitLineLoc_2Text = mfg.SetDoubleTextField("Knit Line Loc 2");
            dto.GKnitLineLoc_3Text = mfg.SetDoubleTextField("Knit Line Loc 3");
            dto.GKnitLineLoc_4Text = mfg.SetDoubleTextField("Knit Line Loc 4");
            dto.GKnitLineLoc_5Text = mfg.SetDoubleTextField("Knit Line Loc 5");
            dto.GKnitLineLoc_6Text = mfg.SetDoubleTextField("Knit Line Loc 6");
            dto.GKnitLineLoc_7Text = mfg.SetDoubleTextField("Knit Line Loc 7");

            dto.GThicknessAvg1Text = mfg.SetDoubleTextField("Thickness IP Avg1", MfgInProcess.sOr);
            dto.GThicknessAvg2Text = mfg.SetDoubleTextField("Thickness IP Avg2", MfgInProcess.sOr);
            dto.GThicknessSlopeText = mfg.SetDoubleTextField("IP Thickness Slope", MfgInProcess.sOr);

            RunCheckLimits(dto);

            return dto;
        }

        private void RunCheckLimits(InProcessBoardDataDto dto)
        {
            var mfg = _mfgInProcess;
            var targets = _cipProdTargets;

            dto.GThicknessIPBackground = mfg.dr["Thickness - IP"] == DBNull.Value
                || targets.ThicknessWithinLimits((double)mfg.dr["Thickness - IP"]) != "Red";
            dto.GThicknessAvg1Background = mfg.dr["Thickness IP Avg1"] == DBNull.Value
                || targets.ThicknessAvg1WithinLimits((double)mfg.dr["Thickness IP Avg1"]) != "Red";
            dto.GThicknessAvg2Background = mfg.dr["Thickness IP Avg2"] == DBNull.Value
                || targets.ThicknessAvg2WithinLimits((double)mfg.dr["Thickness IP Avg2"]) != "Red";
            dto.GThicknessSlopeBackground = mfg.dr["IP Thickness Slope"] == DBNull.Value
                || targets.ThicknessSlopeWithinLimits((double)mfg.dr["IP Thickness Slope"]) != "Red";
            dto.GFlatnessBackground = mfg.dr["Flatness IP"] == DBNull.Value
                || targets.ThicknessProfileWithinLimits((double)mfg.dr["Flatness IP"]) != "Red";
            dto.GBundleHeightIPBackground = mfg.dr["Bundle Height IP"] == DBNull.Value
                || targets.BundleHeightWithinLimits((double)mfg.dr["Bundle Height IP"]) != "Red";
            dto.GLengthBackground = mfg.dr["Length"] == DBNull.Value
                || targets.BoardLengthWithinLimits((double)mfg.dr["Length"]) != "Red";
            dto.GWidthBackground = mfg.dr["Width"] == DBNull.Value
                || targets.BoardWidthWithinLimits((double)mfg.dr["Width"]) != "Red";
            dto.GDiagoanlDiffBackground = mfg.dr["IP Diagonal Diff"] == DBNull.Value
                || targets.BoardSquarenessWithinLimits((double)mfg.dr["IP Diagonal Diff"]) != "Red";
            dto.GCoreDensityIPBackground = mfg.dr["Core Density - IP"] == DBNull.Value
                || targets.CoreDensityWithinLimits((double)mfg.dr["Core Density - IP"]) != "Red";
            dto.GCoreDensityIP_1Background = mfg.dr["Core Density - IP 1"] == DBNull.Value
                || targets.CoreDensityWithinLimits((double)mfg.dr["Core Density - IP 1"]) != "Red";
            dto.GCoreDensityIP_2Background = mfg.dr["Core Density - IP 2"] == DBNull.Value
                || targets.CoreDensityWithinLimits((double)mfg.dr["Core Density - IP 2"]) != "Red";
            dto.GCoreDensityIP_3Background = mfg.dr["Core Density - IP 3"] == DBNull.Value
                || targets.CoreDensityWithinLimits((double)mfg.dr["Core Density - IP 3"]) != "Red";
            dto.GCompressiveIPBackground = mfg.dr["Compressive Strength - IP"] == DBNull.Value
                || targets.CompressionStrWithinLimits((double)mfg.dr["Compressive Strength - IP"]) != "Red";
            dto.GCompressiveIP5Background = mfg.dr["Compressive Strength 5 - IP"] == DBNull.Value
                || targets.CompressionStrWithinLimits((double)mfg.dr["Compressive Strength 5 - IP"]) != "Red";
        }
    }
}
