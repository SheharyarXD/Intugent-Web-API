using IntugentBackend;
using IntugentBackend.Models;
using IntugentBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using IntugentBackend.Services.Mfg;
namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinishedBoardPropertiesController : ControllerBase
    {
        private readonly ObjectsService _objectsService;
        private readonly ILogger<FinishedBoardPropertiesController> _logger;

        public FinishedBoardPropertiesController(ObjectsService objectsService, ILogger<FinishedBoardPropertiesController> logger)
        {
            _objectsService = objectsService;
            _logger = logger;
        }

        /// <summary>
        /// Get all data for the Finished Board Properties page
        /// </summary>
        [HttpGet("data")]
        public IActionResult GetData()
        {
            try
            {
                if (_objectsService.MfgFinishedGoods == null)
                {
                    return BadRequest(new { success = false, error = "MfgFinishedGoods not initialized." });
                }

                // Ensure data is loaded
                _objectsService.MfgFinishedGoods.GetDataSet();
                var data = BuildDto();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Finished Board Properties data");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Navigate to prev/next dataset
        /// </summary>
        [HttpPost("navigate")]
        public IActionResult Navigate([FromBody] NavigateRequest request)
        {
            try
            {
                if (!_objectsService.Cbfile.bCanSwitchRecord)
                {
                    return Ok(new { success = false, error = _objectsService.Cbfile.sNoRecSwitchMsg });
                }

                switch (request.Direction)
                {
                    case "prev":
                        _objectsService.Cbfile.iIDMfgIndex += 1;
                        break;
                    case "next":
                        _objectsService.Cbfile.iIDMfgIndex -= 1;
                        break;
                    default:
                        return BadRequest(new { success = false, error = "Invalid direction" });
                }

                // Clamp bounds
                if (_objectsService.Cbfile.iIDMfgIndex < 0)
                    _objectsService.Cbfile.iIDMfgIndex = 0;
                if (_objectsService.Cbfile.iIDMfgIndex > _objectsService.MfgHome.dt.Rows.Count - 1)
                    _objectsService.Cbfile.iIDMfgIndex = _objectsService.MfgHome.dt.Rows.Count - 1;

                // Update dataset view
                UpdateDataSetView();

                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating dataset");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // ========== FIELD UPDATES ==========

        /// <summary>
        /// Update a text/number field (Thickness, CompStr, etc.)
        /// </summary>
        [HttpPost("update-field")]
        public IActionResult UpdateField([FromBody] FieldUpdateRequest request)
        {
            try
            {
                _objectsService.MfgFinishedGoods.bDataSetChanged = true;
                string result = string.Empty;

                switch (request.Name)
                {
                    // Thickness
                    case "gThicknessFG_1": SetField("Thickness FG - 1", request.Value); break;
                    case "gThicknessFG_2": SetField("Thickness FG - 2", request.Value); break;
                    case "gThicknessFG_3": SetField("Thickness FG - 3", request.Value); break;
                    case "gThicknessFG_4": SetField("Thickness FG - 4", request.Value); break;
                    case "gThicknessFG_5": SetField("Thickness FG - 5", request.Value); break;
                    case "gThicknessFG_6": SetField("Thickness FG - 6", request.Value); break;
                    case "gThicknessFG_7": SetField("Thickness FG - 7", request.Value); break;
                    case "gThicknessFG_8": SetField("Thickness FG - 8", request.Value); break;
                    case "gThicknessFG_9": SetField("Thickness FG - 9", request.Value); break;
                    case "gThicknessFG_10": SetField("Thickness FG - 10", request.Value); break;
                    case "gThicknessFG_11": SetField("Thickness FG - 11", request.Value); break;
                    case "gThicknessFG_12": SetField("Thickness FG - 12", request.Value); break;
                    case "gThicknessFG_13": SetField("Thickness FG - 13", request.Value); break;
                    case "gThicknessFG_14": SetField("Thickness FG - 14", request.Value); break;
                    case "gThicknessFG_15": SetField("Thickness FG - 15", request.Value); break;
                    case "gThicknessFG_16": SetField("Thickness FG - 16", request.Value); break;
                    case "gThicknessFG_17": SetField("Thickness FG - 17", request.Value); break;

                    // Compressive Strength
                    case "gCompStrFG_1": SetField("Compressive FG - 1", request.Value); break;
                    case "gCompStrFG_2": SetField("Compressive FG - 2", request.Value); break;
                    case "gCompStrFG_3": SetField("Compressive FG - 3", request.Value); break;
                    case "gCompStrFG_4": SetField("Compressive FG - 4", request.Value); break;
                    case "gCompStrFG_5": SetField("Compressive FG - 5", request.Value); break;
                    case "gCompStrFG_6": SetField("Compressive FG - 6", request.Value); break;

                    // Retest
                    case "gCompStrFGRetest_1": SetField("Retest - Comp 1 FG", request.Value); break;
                    case "gCompStrFGRetest_2": SetField("Retest - Comp 2 FG", request.Value); break;
                    case "gCompStrFGRetest_3": SetField("Retest - Comp 3 FG", request.Value); break;
                    case "gCompStrFGRetest_4": SetField("Retest - Comp 4 FG", request.Value); break;
                    case "gCompStrFGRetest_5": SetField("Retest - Comp 5 FG", request.Value); break;
                    case "gCompStrFGRetest_6": SetField("Retest - Comp 6 FG", request.Value); break;

                    // k Factor
                    case "gkFactor_1": SetField("k Factor 1 FG", request.Value); break;
                    case "gkFactor_2": SetField("k Factor 2 FG", request.Value); break;
                    case "gkFactor_3": SetField("k Factor 3 FG", request.Value); break;

                    // Aged k Factor 90
                    case "gkFactor90_1": SetField("k Factor 90 FG 1", request.Value); break;
                    case "gkFactor90_2": SetField("k Factor 90 FG 2", request.Value); break;
                    case "gkFactor90_3": SetField("k Factor 90 FG 3", request.Value); break;

                    // Aged k Factor 180
                    case "gkFactor180_1": SetField("k Factor 180 FG 1", request.Value); break;
                    case "gkFactor180_2": SetField("k Factor 180 FG 2", request.Value); break;
                    case "gkFactor180_3": SetField("k Factor 180 FG 3", request.Value); break;

                    // Board Dimensions
                    case "gFGLength": SetField("Length FG", request.Value); break;
                    case "gFGWidth": SetField("Width FG", request.Value); break;
                    case "gFGDiagoanl1": SetField("Diagonal FG 1", request.Value); break;
                    case "gFGDiagoanl2": SetField("Diagonal FG 2", request.Value); break;

                    // Bundle Temps
                    case "gLoggerID": SetField("Logger ID # FG", request.Value); break;
                    case "gInitProbeTemp": SetField("Initial Probe Temp FG", request.Value); break;
                    case "gMaxProbeTemp": SetField("Max Probe Temp FG", request.Value); break;
                    case "gFinalProbeTemp": SetField("Final Probe Temp FG", request.Value); break;

                    // Nail Pull
                    case "gNailPull_1": SetField("Nail Pull FG 1", request.Value); break;
                    case "gNailPull_2": SetField("Nail Pull FG 2", request.Value); break;
                    case "gNailPull_3": SetField("Nail Pull FG 3", request.Value); break;

                    // Facer Peel
                    case "gFacerPeel1": SetField("Facer Peel 1 FG", request.Value); break;
                    case "gFacerPeel2": SetField("Facer Peel 2 FG", request.Value); break;
                    case "gFacerPeel3": SetField("Facer Peel 3 FG", request.Value); break;

                    // Core Density 1
                    case "gMass1": SetField("Mass 1 FG", request.Value); break;
                    case "gL1_1": SetField("L1 1 FG", request.Value); break;
                    case "gW1_1": SetField("W1 1 FG", request.Value); break;
                    case "gT1_1": SetField("T1 1 FG", request.Value); break;
                    case "gT2_1": SetField("T2 1 FG", request.Value); break;
                    case "gT3_1": SetField("T3 1 FG", request.Value); break;
                    case "gT4_1": SetField("T4 1 FG", request.Value); break;
                    case "gT5_1": SetField("T5 1 FG", request.Value); break;

                    // Core Density 2
                    case "gMass2": SetField("Mass 2 FG", request.Value); break;
                    case "gL1_2": SetField("L1 2 FG", request.Value); break;
                    case "gW1_2": SetField("W1 2 FG", request.Value); break;
                    case "gT1_2": SetField("T1 2 FG", request.Value); break;
                    case "gT2_2": SetField("T2 2 FG", request.Value); break;
                    case "gT3_2": SetField("T3 2 FG", request.Value); break;
                    case "gT4_2": SetField("T4 2 FG", request.Value); break;
                    case "gT5_2": SetField("T5 2 FG", request.Value); break;

                    // Core Density 3
                    case "gMass3": SetField("Mass 3 FG", request.Value); break;
                    case "gL1_3": SetField("L1 3 FG", request.Value); break;
                    case "gW1_3": SetField("W1 3 FG", request.Value); break;
                    case "gT1_3": SetField("T1 3 FG", request.Value); break;
                    case "gT2_3": SetField("T2 3 FG", request.Value); break;
                    case "gT3_3": SetField("T3 3 FG", request.Value); break;
                    case "gT4_3": SetField("T4 3 FG", request.Value); break;
                    case "gT5_3": SetField("T5 3 FG", request.Value); break;

                    // General
                    case "gBundleHeight": SetField("Bundle Height FG", request.Value); break;

                    default:
                        return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                // Recalculate and save
                Recalculate(request.Name);
                _objectsService.MfgFinishedGoods.UpdateDataSet();

                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Update checkbox/boolean fields
        /// </summary>
        [HttpPost("update-bool")]
        public IActionResult UpdateBool([FromBody] BoolUpdateRequest request)
        {
            try
            {
                _objectsService.MfgFinishedGoods.bDataSetChanged = true;

                switch (request.Name)
                {
                    case "gCoreDensKnitLine1": _objectsService.MfgFinishedGoods.dr["Core Knit Present FG 1"] = request.Value; break;
                    case "gCoreDensKnitLine2": _objectsService.MfgFinishedGoods.dr["Core Knit Present FG 2"] = request.Value; break;
                    case "gCoreDensKnitLine3": _objectsService.MfgFinishedGoods.dr["Core Knit Present FG 3"] = request.Value; break;

                    case "gRValueKnitPresent1": _objectsService.MfgFinishedGoods.dr["R Value - Knit Present FG 1"] = request.Value; break;
                    case "gRValueKnitPresent2": _objectsService.MfgFinishedGoods.dr["R Value - Knit Present FG 2"] = request.Value; break;
                    case "gRValueKnitPresent3": _objectsService.MfgFinishedGoods.dr["R Value - Knit Present FG 3"] = request.Value; break;

                    case "gAgedrValueDone": _objectsService.MfgFinishedGoods.dr["FG Aged R Value Complete"] = request.Value; break;
                    case "gTestingPassed": _objectsService.MfgFinishedGoods.dr["QC Test Passed"] = request.Value; break;

                    case "gFinsihedGoodsDone":
                        if (!request.Value || !_objectsService.gInProcessDoneIsChecked)
                            _objectsService.MfgFinishedGoods.dr["FG Testing Complete"] = false;
                        else
                            _objectsService.MfgFinishedGoods.dr["FG Testing Complete"] = true;
                        break;

                    case "gCompStrFGKnit_1": _objectsService.MfgFinishedGoods.dr["Comp 1 Knit Present FG"] = request.Value; break;
                    case "gCompStrFGKnit_2": _objectsService.MfgFinishedGoods.dr["Comp 2 Knit Present FG"] = request.Value; break;
                    case "gCompStrFGKnit_3": _objectsService.MfgFinishedGoods.dr["Comp 3 Knit Present FG"] = request.Value; break;
                    case "gCompStrFGKnit_4": _objectsService.MfgFinishedGoods.dr["Comp 4 Knit Present FG"] = request.Value; break;
                    case "gCompStrFGKnit_5": _objectsService.MfgFinishedGoods.dr["Comp 5 Knit Present FG"] = request.Value; break;
                    case "gCompStrFGKnit_6": _objectsService.MfgFinishedGoods.dr["Comp 6 Knit Present FG"] = request.Value; break;

                    case "gCompStrFGKnitRetest_1": _objectsService.MfgFinishedGoods.dr["Comp 1 Retest Knit Present FG"] = request.Value; break;
                    case "gCompStrFGKnitRetest_2": _objectsService.MfgFinishedGoods.dr["Comp 2 Retest Knit Present FG"] = request.Value; break;
                    case "gCompStrFGKnitRetest_3": _objectsService.MfgFinishedGoods.dr["Comp 3 Retest Knit Present FG"] = request.Value; break;
                    case "gCompStrFGKnitRetest_4": _objectsService.MfgFinishedGoods.dr["Comp 4 Retest Knit Present FG"] = request.Value; break;
                    case "gCompStrFGKnitRetest_5": _objectsService.MfgFinishedGoods.dr["Comp 5 Retest Knit Present FG"] = request.Value; break;
                    case "gCompStrFGKnitRetest_6": _objectsService.MfgFinishedGoods.dr["Comp 6 Retest Knit Present FG"] = request.Value; break;

                    case "gIPTimeNotLegible": _objectsService.MfgFinishedGoods.dr["IP Time Stamp Not Legible"] = request.Value; break;
                    case "gRestestFromSameBundle": _objectsService.MfgFinishedGoods.dr["Is Retest From Same Bundle? FG"] = request.Value; break;

                    default:
                        return BadRequest(new { success = false, error = $"Unknown bool field: {request.Name}" });
                }

                _objectsService.MfgFinishedGoods.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bool field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Update datetime fields
        /// </summary>
        [HttpPost("update-datetime")]
        public IActionResult UpdateDateTime([FromBody] DateTimeUpdateRequest request)
        {
            try
            {
                _objectsService.MfgFinishedGoods.bDataSetChanged = true;

                switch (request.Name)
                {
                    case "gFBTimeStamp":
                        _objectsService.MfgFinishedGoods.dr["Finished Board Time Stamp FG"] = request.Value ?? (object)DBNull.Value;
                        if (request.Value != null) CheckBoardTimeStamp();
                        break;
                    case "gQCTimesDateTime":
                        _objectsService.MfgFinishedGoods.dr["Next Day QC Collection Time FG"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gkFactorTime1":
                        _objectsService.MfgFinishedGoods.dr["k Factor DateTime FG 1"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gkFactorTime2":
                        _objectsService.MfgFinishedGoods.dr["k Factor DateTime FG 2"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gkFactorTime3":
                        _objectsService.MfgFinishedGoods.dr["k Factor DateTime FG 3"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gInitProbeTime":
                        _objectsService.MfgFinishedGoods.dr["Initial Probe Time FG"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gMaxTempTimeInit":
                        _objectsService.MfgFinishedGoods.dr["Max Probe Time - Initial FG"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gMaxTempTimeFinal":
                        _objectsService.MfgFinishedGoods.dr["Max Probe Time - Final FG"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gFinalProbeTime":
                        _objectsService.MfgFinishedGoods.dr["Final Probe Time FG"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gRetestQCTime":
                        _objectsService.MfgFinishedGoods.dr["Retest QC Collection Time FG"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gkFactor90Date_1":
                        _objectsService.MfgFinishedGoods.dr["k Factor 90 Date FG 1"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gkFactor90Date_2":
                        _objectsService.MfgFinishedGoods.dr["k Factor 90 Date FG 2"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gkFactor90Date_3":
                        _objectsService.MfgFinishedGoods.dr["k Factor 90 Date FG 3"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gkFactor180Date_1":
                        _objectsService.MfgFinishedGoods.dr["k Factor 180 Date FG 1"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gkFactor180Date_2":
                        _objectsService.MfgFinishedGoods.dr["k Factor 180 Date FG 2"] = request.Value ?? (object)DBNull.Value;
                        break;
                    case "gkFactor180Date_3":
                        _objectsService.MfgFinishedGoods.dr["k Factor 180 Date FG 3"] = request.Value ?? (object)DBNull.Value;
                        break;
                    default:
                        return BadRequest(new { success = false, error = $"Unknown datetime field: {request.Name}" });
                }

                _objectsService.MfgFinishedGoods.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating datetime field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Update notes text area
        /// </summary>
        [HttpPost("update-notes")]
        public IActionResult UpdateNotes([FromBody] FieldUpdateRequest request)
        {
            try
            {
                _objectsService.MfgFinishedGoods.bDataSetChanged = true;
                _objectsService.MfgFinishedGoods.dr["Notes FG"] = string.IsNullOrEmpty(request.Value)
                    ? (object)DBNull.Value
                    : request.Value;

                _objectsService.MfgFinishedGoods.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notes");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // ========== PRIVATE HELPERS ==========

        private void SetField(string columnName, string? value)
        {
            _objectsService.MfgFinishedGoods.dr[columnName] = string.IsNullOrEmpty(value)
                ? (object)DBNull.Value
                : value;
        }

        private void UpdateDataSetView()
        {
            _objectsService.Cbfile.iIDMfg = (int)_objectsService.MfgHome.dt.Rows[_objectsService.Cbfile.iIDMfgIndex]["ID4All"];
            _objectsService.CLists.drEmployee["MfgIDSelected"] = _objectsService.Cbfile.iIDMfg;
            _objectsService.CLists.UpdateEmployee();

            (_objectsService.MfgInProcess, _objectsService.MfgFinishedGoods,
             _objectsService.MfgDimStability, _objectsService.MfgPlantData,
             _objectsService.MfgJetMixing) = _objectsService.MfgHome.GetAllMfgData(
                 _objectsService.MfgInProcess, _objectsService.MfgFinishedGoods,
                 _objectsService.MfgDimStability, _objectsService.MfgPlantData,
                 _objectsService.MfgJetMixing);
        }

        private void Recalculate(string fieldName)
        {
            // Thickness recalculation
            if (fieldName.StartsWith("gThicknessFG_"))
            {
                RecalcThickness();
            }
            // Compressive Strength
            else if (fieldName.StartsWith("gCompStrFG_") && !fieldName.Contains("Retest"))
            {
                RecalcCompStr();
            }
            // Retest
            else if (fieldName.StartsWith("gCompStrFGRetest_"))
            {
                RecalcCompStrRetest();
            }
            // k Factor
            else if (fieldName.StartsWith("gkFactor_"))
            {
                RecalckFactor();
            }
            // k Factor 90
            else if (fieldName.StartsWith("gkFactor90_"))
            {
                RecalckFactor90();
            }
            // k Factor 180
            else if (fieldName.StartsWith("gkFactor180_"))
            {
                RecalckFactor180();
            }
            // Board Dims
            else if (fieldName.StartsWith("gFG"))
            {
                RecalcBoardDims();
            }
            // Nail Pull
            else if (fieldName.StartsWith("gNailPull_"))
            {
                RecalcNailPull();
            }
            // Facer Peel
            else if (fieldName.StartsWith("gFacerPeel"))
            {
                RecalcFacerPeel();
            }
            // Core Density
            else if (fieldName.StartsWith("gMass") || fieldName.StartsWith("gL1_") ||
                     fieldName.StartsWith("gW1_") || fieldName.StartsWith("gT"))
            {
                RecalcCoreDensity(fieldName);
            }
        }

        private void RecalcThickness()
        {
            int nCount = 0; double dSum = 0, dtmp, dmin = double.MaxValue, dmax = double.MinValue;
            for (int i = 1; i <= 17; i++)
            {
                var val = _objectsService.MfgFinishedGoods.dr[$"Thickness FG - {i}"];
                if (val != DBNull.Value)
                {
                    dtmp = (double)val; dSum += dtmp; nCount++;
                    if (dtmp < dmin) dmin = dtmp;
                    if (dtmp > dmax) dmax = dtmp;
                }
            }
            dtmp = dSum / nCount;
            if (double.IsNaN(dtmp) || nCount < 17)
            {
                _objectsService.MfgFinishedGoods.dr["Thickness Avg FG"] = DBNull.Value;
                _objectsService.MfgFinishedGoods.dr["Flatness FG"] = DBNull.Value;
            }
            else
            {
                _objectsService.MfgFinishedGoods.dr["Thickness Avg FG"] = dtmp;
                _objectsService.MfgFinishedGoods.dr["Flatness FG"] = dmin - dmax;
                _objectsService.MfgFinishedGoods.dr["thickness valleys FG"] = dmin;
                _objectsService.MfgFinishedGoods.dr["thickness peaks FG"] = dmax;
            }
        }

        private void RecalcCompStr()
        {
            int nCount = 0; double dSum = 0, dMin = double.MaxValue, dtmp;
            for (int i = 1; i <= 6; i++)
            {
                var val = _objectsService.MfgFinishedGoods.dr[$"Compressive FG - {i}"];
                if (val != DBNull.Value)
                {
                    nCount++; dtmp = (double)val; dSum += dtmp;
                    if (dMin > dtmp) dMin = dtmp;
                }
            }
            if (nCount == 6)
            {
                _objectsService.MfgFinishedGoods.dr["Compressive Strength (6) FG"] = dSum / 6.0;
                _objectsService.MfgFinishedGoods.dr["Compressive Strength (5) FG"] = (dSum - dMin) / 5.0;
            }
            else if (nCount == 5)
            {
                _objectsService.MfgFinishedGoods.dr["Compressive Strength (6) FG"] = DBNull.Value;
                _objectsService.MfgFinishedGoods.dr["Compressive Strength (5) FG"] = dSum / 5.0;
            }
            else
            {
                _objectsService.MfgFinishedGoods.dr["Compressive Strength (6) FG"] = DBNull.Value;
                _objectsService.MfgFinishedGoods.dr["Compressive Strength (5) FG"] = DBNull.Value;
            }
        }

        private void RecalcCompStrRetest()
        {
            int nCount = 0; double dSum = 0, dMin = double.MaxValue, dtmp;
            for (int i = 1; i <= 6; i++)
            {
                var val = _objectsService.MfgFinishedGoods.dr[$"Retest - Comp {i} FG"];
                if (val != DBNull.Value)
                {
                    nCount++; dtmp = (double)val; dSum += dtmp;
                    if (dMin > dtmp) dMin = dtmp;
                }
            }
            if (nCount == 6)
            {
                _objectsService.MfgFinishedGoods.dr["Retest - AVG Comp Strength (6) FG"] = dSum / 6.0;
                _objectsService.MfgFinishedGoods.dr["Retest - AVG Comp Strength (5) FG"] = (dSum - dMin) / 5.0;
            }
            else if (nCount == 5)
            {
                _objectsService.MfgFinishedGoods.dr["Retest - AVG Comp Strength (6) FG"] = DBNull.Value;
                _objectsService.MfgFinishedGoods.dr["Retest - AVG Comp Strength (5) FG"] = dSum / 5.0;
            }
            else
            {
                _objectsService.MfgFinishedGoods.dr["Retest - AVG Comp Strength (6) FG"] = DBNull.Value;
                _objectsService.MfgFinishedGoods.dr["Retest - AVG Comp Strength (5) FG"] = DBNull.Value;
            }
        }

        private void RecalckFactor()
        {
            int nCount = 0; double dSum = 0, dSumR = 0, dtmp, dtmpR;
            for (int i = 1; i <= 3; i++)
            {
                var val = _objectsService.MfgFinishedGoods.dr[$"k Factor {i} FG"];
                if (val != DBNull.Value)
                {
                    nCount++; dSum += (double)val; dSumR += 1.0 / (double)val;
                }
            }
            if (nCount > 1)
            {
                dtmp = dSum / nCount;
                dtmpR = dSumR / nCount;
                _objectsService.MfgFinishedGoods.dr["k Factor FG"] = dtmp;
                _objectsService.MfgFinishedGoods.dr["R Value - AVG FG"] = dtmpR;
            }
            else
            {
                _objectsService.MfgFinishedGoods.dr["k Factor FG"] = DBNull.Value;
                _objectsService.MfgFinishedGoods.dr["R Value - AVG FG"] = DBNull.Value;
            }
        }

        private void RecalckFactor90()
        {
            int nCount = 0; double dSum = 0, dSumR = 0, dtmp, dtmpR;
            for (int i = 1; i <= 3; i++)
            {
                var val = _objectsService.MfgFinishedGoods.dr[$"k Factor 90 FG {i}"];
                if (val != DBNull.Value)
                {
                    nCount++; dSum += (double)val; dSumR += 1.0 / (double)val;
                }
            }
            if (nCount > 1)
            {
                dtmp = dSum / nCount; dtmpR = dSumR / nCount;
                _objectsService.MfgFinishedGoods.dr["k Factor 90 FG"] = dtmp;
                _objectsService.MfgFinishedGoods.dr["R Value 90 - AVG FG"] = dtmpR;
            }
            else
            {
                _objectsService.MfgFinishedGoods.dr["k Factor 90 FG"] = DBNull.Value;
                _objectsService.MfgFinishedGoods.dr["R Value 90 - AVG FG"] = DBNull.Value;
            }
        }

        private void RecalckFactor180()
        {
            int nCount = 0; double dSum = 0, dSumR = 0, dtmp, dtmpR;
            for (int i = 1; i <= 3; i++)
            {
                var val = _objectsService.MfgFinishedGoods.dr[$"k Factor 180 FG {i}"];
                if (val != DBNull.Value)
                {
                    nCount++; dSum += (double)val; dSumR += 1.0 / (double)val;
                }
            }
            if (nCount > 1)
            {
                dtmp = dSum / nCount; dtmpR = dSumR / nCount;
                _objectsService.MfgFinishedGoods.dr["k Factor 180 FG"] = dtmp;
                _objectsService.MfgFinishedGoods.dr["R Value 180 - AVG FG"] = dtmpR;
            }
            else
            {
                _objectsService.MfgFinishedGoods.dr["k Factor 180 FG"] = DBNull.Value;
                _objectsService.MfgFinishedGoods.dr["R Value 180 - AVG FG"] = DBNull.Value;
            }
        }

        private void RecalcBoardDims()
        {
            var d1 = _objectsService.MfgFinishedGoods.dr["Diagonal FG 1"];
            var d2 = _objectsService.MfgFinishedGoods.dr["Diagonal FG 2"];
            if (d1 != DBNull.Value && d2 != DBNull.Value)
            {
                double dtmp = Math.Abs((double)d1 - (double)d2);
                _objectsService.MfgFinishedGoods.dr["Diagonal Diff FG"] = dtmp;
            }
            else
            {
                _objectsService.MfgFinishedGoods.dr["Diagonal Diff FG"] = DBNull.Value;
            }
        }

        private void RecalcNailPull()
        {
            int nCount = 0; double dSum = 0, dtmp;
            for (int i = 1; i <= 3; i++)
            {
                var val = _objectsService.MfgFinishedGoods.dr[$"Nail Pull FG {i}"];
                if (val != DBNull.Value) { nCount++; dSum += (double)val; }
            }
            if (nCount == 3) { dtmp = dSum / nCount; _objectsService.MfgFinishedGoods.dr["Nail Pull FG"] = dtmp; }
            else { _objectsService.MfgFinishedGoods.dr["Nail Pull FG"] = DBNull.Value; }
        }

        private void RecalcFacerPeel()
        {
            int nCount = 0; double dSum = 0, dtmp;
            for (int i = 1; i <= 3; i++)
            {
                var val = _objectsService.MfgFinishedGoods.dr[$"Facer Peel {i} FG"];
                if (val != DBNull.Value) { nCount++; dSum += (double)val; }
            }
            if (nCount == 3)
            {
                dtmp = dSum / nCount;
                _objectsService.MfgFinishedGoods.dr["Facer Peel FG"] = dtmp;
            }
            else { _objectsService.MfgFinishedGoods.dr["Facer Peel FG"] = DBNull.Value; }
        }

        private void RecalcCoreDensity(string fieldName)
        {
            // Determine which row (1, 2, or 3) from field name
            int row = fieldName.Contains("_1") ? 1 : fieldName.Contains("_2") ? 2 : 3;

            double dm = 0, dl = 0, dw = 0, dt = 0;
            int nm = 0, nl = 0, nw = 0, nt = 0;

            var mass = _objectsService.MfgFinishedGoods.dr[$"Mass {row} FG"];
            if (mass != DBNull.Value) { dm = (double)mass; nm = 1; }

            var l1 = _objectsService.MfgFinishedGoods.dr[$"L1 {row} FG"];
            if (l1 != DBNull.Value) { dl = (double)l1; nl = 2; }

            var w1 = _objectsService.MfgFinishedGoods.dr[$"W1 {row} FG"];
            if (w1 != DBNull.Value) { dw = (double)w1; nw = 2; }

            double dSum = 0; int nCount = 0;
            for (int i = 1; i <= 5; i++)
            {
                var t = _objectsService.MfgFinishedGoods.dr[$"T{i} {row} FG"];
                if (t != DBNull.Value) { nCount++; dSum += (double)t; }
            }
            if (nCount > 3) { dt = dSum / nCount; nt = 5; }

            if (nm * nl * nw * nt > 0 && dl * dw * dt > 0.0)
            {
                double dtmp = dm / (dl * dw * dt) * 3.809590998;
                _objectsService.MfgFinishedGoods.dr[$"FG Core Density {row}"] = dtmp;
            }
            else
            {
                _objectsService.MfgFinishedGoods.dr[$"FG Core Density {row}"] = DBNull.Value;
            }

            // Recalc average
            AvCoreDensity();
        }

        private void AvCoreDensity()
        {
            int nCount = 0; double dSum = 0, dtmp;
            for (int i = 1; i <= 3; i++)
            {
                var val = _objectsService.MfgFinishedGoods.dr[$"FG Core Density {i}"];
                if (val != DBNull.Value) { nCount++; dSum += (double)val; }
            }
            dtmp = dSum / nCount;
            if (double.IsNaN(dtmp) || nCount == 0)
                _objectsService.MfgFinishedGoods.dr["FG Core Density"] = DBNull.Value;
            else
                _objectsService.MfgFinishedGoods.dr["FG Core Density"] = dtmp;
        }

        private void CheckBoardTimeStamp()
        {
            // Validation logic from old code
            var fbTime = _objectsService.MfgFinishedGoods.dr["Finished Board Time Stamp FG"];
            var ipTime = _objectsService.MfgFinishedGoods.drIP["Test Date"];
            if (fbTime == DBNull.Value || ipTime == DBNull.Value) return;

            DateTime fgDate = (DateTime)fbTime;
            DateTime ipDate = (DateTime)ipTime;

            if (fgDate < new DateTime(2000, 01, 01) ||
                Math.Abs((fgDate - ipDate).TotalMinutes) > _objectsService.CDefualts.dDelTimeButton)
            {
                // Invalid - handled in CheckLimits via background color
            }
        }

        private void CheckLimits(string sF)
        {
            // Ported from old CheckLimits - simplified for brevity
            // Full implementation would include all limit checks from original code
        }

        // ========== DTO BUILDER ==========

        private FinishedBoardDataDto BuildDto()
        {
            var mfg = _objectsService.MfgFinishedGoods;
            var defs = _objectsService.CDefualts;

            var dto = new FinishedBoardDataDto();

            // Navigation
            dto.GDataSetNext = _objectsService.Cbfile.iIDMfgIndex == 0 ? false : true;
            dto.GDataSetPrev = _objectsService.Cbfile.iIDMfgIndex >= _objectsService.MfgHome.dt.Rows.Count - 1 ? false : true;

            // Location labels
            dto.GLoc1A = defs.sLocMfg1.ToUpper();
            dto.GLoc2A = defs.sLocMfg2.ToUpper();
            dto.GLoc3A = defs.sLocMfg3.ToUpper();
            dto.GLoc1B = dto.GLoc1C = dto.GLoc1D = dto.GLoc1E = dto.GLoc1F = defs.sLocMfg1;
            dto.GLoc2B = dto.GLoc2C = dto.GLoc2D = dto.GLoc2E = defs.sLocMfg2;
            dto.GLoc3B = dto.GLoc3C = dto.GLoc3D = dto.GLoc3E = dto.GLoc3F = defs.sLocMfg3;

            // General Info
            dto.GID = mfg.dr["ID4ALL FG"]?.ToString() ?? string.Empty;
            dto.GFinsihedGoodsDone = mfg.dr["FG Testing Complete"] == DBNull.Value ? false : (bool)mfg.dr["FG Testing Complete"];
            dto.GTestingPassed = mfg.dr["QC Test Passed"] == DBNull.Value ? false : (bool)mfg.dr["QC Test Passed"];

            // Product Code lookup
            if (mfg.drIP["Product ID"] != DBNull.Value)
            {
                string stmp = mfg.drIP["Product ID"].ToString()!;
                _objectsService.CLists.dvComProd.RowFilter = $"[Product Code] = '{stmp}'";
                var dtxxx = _objectsService.CLists.dvComProd.ToTable();
                if (dtxxx.Rows.Count > 0 && dtxxx.Rows[0]["Product"] != DBNull.Value)
                    dto.GProdCode = dtxxx.Rows[0]["Product"].ToString()!;
                _objectsService.CLists.dvComProd.RowFilter = null;
            }

            // Dates
            dto.GTestDate = mfg.drIP["Test Date"] == DBNull.Value ? string.Empty : ((DateTime)mfg.drIP["Test Date"]).ToString("yyyy-MM-ddTHH:mm:ss");
            dto.GFBTimeStamp = mfg.dr["Finished Board Time Stamp FG"] == DBNull.Value ? null : (DateTime)mfg.dr["Finished Board Time Stamp FG"];
            dto.GQCTimesDateTime = mfg.dr["Next Day QC Collection Time FG"] == DBNull.Value ? null : (DateTime)mfg.dr["Next Day QC Collection Time FG"];
            dto.GkFactorTime1 = mfg.dr["k Factor DateTime FG 1"] == DBNull.Value ? null : (DateTime)mfg.dr["k Factor DateTime FG 1"];
            dto.GkFactorTime2 = mfg.dr["k Factor DateTime FG 2"] == DBNull.Value ? null : (DateTime)mfg.dr["k Factor DateTime FG 2"];
            dto.GkFactorTime3 = mfg.dr["k Factor DateTime FG 3"] == DBNull.Value ? null : (DateTime)mfg.dr["k Factor DateTime FG 3"];
            dto.GInitProbeTime = mfg.dr["Initial Probe Time FG"] == DBNull.Value ? null : (DateTime)mfg.dr["Initial Probe Time FG"];
            dto.GMaxTempTimeInit = mfg.dr["Max Probe Time - Initial FG"] == DBNull.Value ? null : (DateTime)mfg.dr["Max Probe Time - Initial FG"];
            dto.GMaxTempTimeFinal = mfg.dr["Max Probe Time - Final FG"] == DBNull.Value ? null : (DateTime)mfg.dr["Max Probe Time - Final FG"];
            dto.GFinalProbeTime = mfg.dr["Final Probe Time FG"] == DBNull.Value ? null : (DateTime)mfg.dr["Final Probe Time FG"];
            dto.GRetestQCTime = mfg.dr["Retest QC Collection Time FG"] == DBNull.Value ? null : (DateTime)mfg.dr["Retest QC Collection Time FG"];

            // Aged dates
            dto.GKFactor90Date_1 = mfg.dr["k Factor 90 Date FG 1"] == DBNull.Value ? null : (DateTime)mfg.dr["k Factor 90 Date FG 1"];
            dto.GKFactor90Date_2 = mfg.dr["k Factor 90 Date FG 2"] == DBNull.Value ? null : (DateTime)mfg.dr["k Factor 90 Date FG 2"];
            dto.GKFactor90Date_3 = mfg.dr["k Factor 90 Date FG 3"] == DBNull.Value ? null : (DateTime)mfg.dr["k Factor 90 Date FG 3"];
            dto.GKFactor180Date_1 = mfg.dr["k Factor 180 Date FG 1"] == DBNull.Value ? null : (DateTime)mfg.dr["k Factor 180 Date FG 1"];
            dto.GKFactor180Date_2 = mfg.dr["k Factor 180 Date FG 2"] == DBNull.Value ? null : (DateTime)mfg.dr["k Factor 180 Date FG 2"];
            dto.GKFactor180Date_3 = mfg.dr["k Factor 180 Date FG 3"] == DBNull.Value ? null : (DateTime)mfg.dr["k Factor 180 Date FG 3"];

            // General Info fields
            dto.GBundleHeight = mfg.SetDoubleTextField("Bundle Height FG");
            dto.GIPLength = mfg.drIP["Length"] == DBNull.Value ? string.Empty : ((double)mfg.drIP["Length"]).ToString(MfgFinishedGoods.sOr);
            dto.GIPWidth = mfg.drIP["Width"] == DBNull.Value ? string.Empty : ((double)mfg.drIP["Width"]).ToString(MfgFinishedGoods.sOr);
            dto.GTimePourTableQC = mfg.drIP["Time of Pour Table QC Check"] == DBNull.Value ? string.Empty : ((DateTime)mfg.drIP["Time of Pour Table QC Check"]).ToString("HH:mm:ss");
            dto.GIPBoardTimeStamp = mfg.drIP["Test Date"] == DBNull.Value ? string.Empty : ((DateTime)mfg.drIP["Test Date"]).ToString("yyyy-MM-ddTHH:mm:ss");
            dto.GIPTimeNotLegible = mfg.dr["IP Time Stamp Not Legible"] == DBNull.Value ? false : (bool)mfg.dr["IP Time Stamp Not Legible"];

            // QC Summary
            dto.GCoreDensity = mfg.SetDoubleTextField("FG Core Density", MfgFinishedGoods.sOr);
            dto.GCompStrFG_Avg6 = mfg.SetDoubleTextField("Compressive Strength (6) FG", MfgFinishedGoods.sOr);
            dto.GCompStrFG_Avg5 = mfg.SetDoubleTextField("Compressive Strength (5) FG", MfgFinishedGoods.sOr);
            dto.GThickness = mfg.SetDoubleTextField("Thickness Avg FG", MfgFinishedGoods.sOr);
            dto.GFlatness = mfg.SetDoubleTextField("Flatness FG", MfgFinishedGoods.sOr);
            dto.GRValue = mfg.SetDoubleTextField("R Value - AVG FG", MfgFinishedGoods.sOr);
            dto.GFacerPeelAvg_QC = mfg.SetDoubleTextField("Facer Peel FG", MfgFinishedGoods.sOr);

            // IP QC Summary
            dto.GWarehouseTemp = mfg.drIP["Warehouse Temp"] == DBNull.Value ? string.Empty : ((double)mfg.drIP["Warehouse Temp"]).ToString(MfgFinishedGoods.sOr);
            dto.GWarehouseHumidity = mfg.drIP["Warehouse Humidity"] == DBNull.Value ? string.Empty : ((double)mfg.drIP["Warehouse Humidity"]).ToString(MfgFinishedGoods.sOr);
            dto.GBundleHeightIP = mfg.drIP["Bundle Height IP"] == DBNull.Value ? string.Empty : ((double)mfg.drIP["Bundle Height IP"]).ToString(MfgFinishedGoods.sOr);
            dto.GThicknessIP = mfg.drIP["Thickness - IP"] == DBNull.Value ? string.Empty : ((double)mfg.drIP["Thickness - IP"]).ToString(MfgFinishedGoods.sOr);
            dto.GCoreDensityIP = mfg.drIP["Core Density - IP"] == DBNull.Value ? string.Empty : ((double)mfg.drIP["Core Density - IP"]).ToString(MfgFinishedGoods.sOr);
            dto.GCompressiveIP = mfg.drIP["Compressive Strength - IP"] == DBNull.Value ? string.Empty : ((double)mfg.drIP["Compressive Strength - IP"]).ToString(MfgFinishedGoods.sOr);
            dto.GCompressiveIP5 = mfg.drIP["Compressive Strength 5 - IP"] == DBNull.Value ? string.Empty : ((double)mfg.drIP["Compressive Strength 5 - IP"]).ToString(MfgFinishedGoods.sOr);

            // Thickness
            for (int i = 1; i <= 17; i++)
                typeof(FinishedBoardDataDto).GetProperty($"GThicknessFG_{i}")!.SetValue(dto, mfg.SetDoubleTextField($"Thickness FG - {i}"));

            // Comp Strength
            dto.GCompStrFG_1 = mfg.SetDoubleTextField("Compressive FG - 1");
            dto.GCompStrFG_2 = mfg.SetDoubleTextField("Compressive FG - 2");
            dto.GCompStrFG_3 = mfg.SetDoubleTextField("Compressive FG - 3");
            dto.GCompStrFG_4 = mfg.SetDoubleTextField("Compressive FG - 4");
            dto.GCompStrFG_5 = mfg.SetDoubleTextField("Compressive FG - 5");
            dto.GCompStrFG_6 = mfg.SetDoubleTextField("Compressive FG - 6");

            dto.GCompStrFGKnit_1 = mfg.dr["Comp 1 Knit Present FG"] == DBNull.Value ? false : (bool)mfg.dr["Comp 1 Knit Present FG"];
            dto.GCompStrFGKnit_2 = mfg.dr["Comp 2 Knit Present FG"] == DBNull.Value ? false : (bool)mfg.dr["Comp 2 Knit Present FG"];
            dto.GCompStrFGKnit_3 = mfg.dr["Comp 3 Knit Present FG"] == DBNull.Value ? false : (bool)mfg.dr["Comp 3 Knit Present FG"];
            dto.GCompStrFGKnit_4 = mfg.dr["Comp 4 Knit Present FG"] == DBNull.Value ? false : (bool)mfg.dr["Comp 4 Knit Present FG"];
            dto.GCompStrFGKnit_5 = mfg.dr["Comp 5 Knit Present FG"] == DBNull.Value ? false : (bool)mfg.dr["Comp 5 Knit Present FG"];
            dto.GCompStrFGKnit_6 = mfg.dr["Comp 6 Knit Present FG"] == DBNull.Value ? false : (bool)mfg.dr["Comp 6 Knit Present FG"];

            dto.GNotes = mfg.dr["Notes FG"] == DBNull.Value ? string.Empty : (string)mfg.dr["Notes FG"];

            // Retest
            dto.GRestestFromSameBundle = mfg.dr["Is Retest From Same Bundle? FG"] == DBNull.Value ? false : (bool)mfg.dr["Is Retest From Same Bundle? FG"];
            dto.GCompStrFGRetest_1 = mfg.SetDoubleTextField("Retest - Comp 1 FG");
            dto.GCompStrFGRetest_2 = mfg.SetDoubleTextField("Retest - Comp 2 FG");
            dto.GCompStrFGRetest_3 = mfg.SetDoubleTextField("Retest - Comp 3 FG");
            dto.GCompStrFGRetest_4 = mfg.SetDoubleTextField("Retest - Comp 4 FG");
            dto.GCompStrFGRetest_5 = mfg.SetDoubleTextField("Retest - Comp 5 FG");
            dto.GCompStrFGRetest_6 = mfg.SetDoubleTextField("Retest - Comp 6 FG");

            dto.GCompStrFGKnitRetest_1 = mfg.dr["Comp 1 Retest Knit Present FG"] == DBNull.Value ? false : (bool)mfg.dr["Comp 1 Retest Knit Present FG"];
            dto.GCompStrFGKnitRetest_2 = mfg.dr["Comp 2 Retest Knit Present FG"] == DBNull.Value ? false : (bool)mfg.dr["Comp 2 Retest Knit Present FG"];
            dto.GCompStrFGKnitRetest_3 = mfg.dr["Comp 3 Retest Knit Present FG"] == DBNull.Value ? false : (bool)mfg.dr["Comp 3 Retest Knit Present FG"];
            dto.GCompStrFGKnitRetest_4 = mfg.dr["Comp 4 Retest Knit Present FG"] == DBNull.Value ? false : (bool)mfg.dr["Comp 4 Retest Knit Present FG"];
            dto.GCompStrFGKnitRetest_5 = mfg.dr["Comp 5 Retest Knit Present FG"] == DBNull.Value ? false : (bool)mfg.dr["Comp 5 Retest Knit Present FG"];
            dto.GCompStrFGKnitRetest_6 = mfg.dr["Comp 6 Retest Knit Present FG"] == DBNull.Value ? false : (bool)mfg.dr["Comp 6 Retest Knit Present FG"];

            dto.GCompStrFGRetest_Avg5 = mfg.SetDoubleTextField("Retest - AVG Comp Strength (5) FG", MfgFinishedGoods.sOr);
            dto.GCompStrFGRetest_Avg6 = mfg.SetDoubleTextField("Retest - AVG Comp Strength (6) FG", MfgFinishedGoods.sOr);

            // k Factor
            dto.GkFactor_1 = mfg.SetDoubleTextField("k Factor 1 FG");
            dto.GkFactor_2 = mfg.SetDoubleTextField("k Factor 2 FG");
            dto.GkFactor_3 = mfg.SetDoubleTextField("k Factor 3 FG");
            dto.GkFactor_Avg = mfg.SetDoubleTextField("R Value - AVG FG", MfgFinishedGoods.sOr);

            dto.GRValueKnitPresent1 = mfg.dr["R Value - Knit Present FG 1"] == DBNull.Value ? false : (bool)mfg.dr["R Value - Knit Present FG 1"];
            dto.GRValueKnitPresent2 = mfg.dr["R Value - Knit Present FG 2"] == DBNull.Value ? false : (bool)mfg.dr["R Value - Knit Present FG 2"];
            dto.GRValueKnitPresent3 = mfg.dr["R Value - Knit Present FG 3"] == DBNull.Value ? false : (bool)mfg.dr["R Value - Knit Present FG 3"];

            // Aged
            dto.GkFactor90_1 = mfg.SetDoubleTextField("k Factor 90 FG 1");
            dto.GkFactor90_2 = mfg.SetDoubleTextField("k Factor 90 FG 2");
            dto.GkFactor90_3 = mfg.SetDoubleTextField("k Factor 90 FG 3");
            dto.GkFactor90 = mfg.SetDoubleTextField("R Value 90 - AVG FG", MfgFinishedGoods.sOr);

            dto.GkFactor180_1 = mfg.SetDoubleTextField("k Factor 180 FG 1");
            dto.GkFactor180_2 = mfg.SetDoubleTextField("k Factor 180 FG 2");
            dto.GkFactor180_3 = mfg.SetDoubleTextField("k Factor 180 FG 3");
            dto.GkFactor180 = mfg.SetDoubleTextField("R Value 180 - AVG FG", MfgFinishedGoods.sOr);

            dto.GAgedRValueDone = mfg.dr["FG Aged R Value Complete"] == DBNull.Value ? false : (bool)mfg.dr["FG Aged R Value Complete"];

            // Bundle Temps
            dto.GLoggerID = mfg.SetIntTextField("Logger ID # FG");
            dto.GInitProbeTemp = mfg.SetDoubleTextField("Initial Probe Temp FG");
            dto.GMaxProbeTemp = mfg.SetDoubleTextField("Max Probe Temp FG");
            dto.GFinalProbeTemp = mfg.SetDoubleTextField("Final Probe Temp FG");

            // Core Density
            dto.GMass1 = mfg.SetDoubleTextField("Mass 1 FG");
            dto.GL1_1 = mfg.SetDoubleTextField("L1 1 FG");
            dto.GW1_1 = mfg.SetDoubleTextField("W1 1 FG");
            dto.GT1_1 = mfg.SetDoubleTextField("T1 1 FG");
            dto.GT2_1 = mfg.SetDoubleTextField("T2 1 FG");
            dto.GT3_1 = mfg.SetDoubleTextField("T3 1 FG");
            dto.GT4_1 = mfg.SetDoubleTextField("T4 1 FG");
            dto.GT5_1 = mfg.SetDoubleTextField("T5 1 FG");
            dto.GCoreDens1 = mfg.SetDoubleTextField("FG Core Density 1", MfgFinishedGoods.sOr);
            dto.GCoreDensKnitLine1 = mfg.dr["Core Knit Present FG 1"] == DBNull.Value ? false : (bool)mfg.dr["Core Knit Present FG 1"];

            dto.GMass2 = mfg.SetDoubleTextField("Mass 2 FG");
            dto.GL1_2 = mfg.SetDoubleTextField("L1 2 FG");
            dto.GW1_2 = mfg.SetDoubleTextField("W1 2 FG");
            dto.GT1_2 = mfg.SetDoubleTextField("T1 2 FG");
            dto.GT2_2 = mfg.SetDoubleTextField("T2 2 FG");
            dto.GT3_2 = mfg.SetDoubleTextField("T3 2 FG");
            dto.GT4_2 = mfg.SetDoubleTextField("T4 2 FG");
            dto.GT5_2 = mfg.SetDoubleTextField("T5 2 FG");
            dto.GCoreDens2 = mfg.SetDoubleTextField("FG Core Density 2", MfgFinishedGoods.sOr);
            dto.GCoreDensKnitLine2 = mfg.dr["Core Knit Present FG 2"] == DBNull.Value ? false : (bool)mfg.dr["Core Knit Present FG 2"];

            dto.GMass3 = mfg.SetDoubleTextField("Mass 3 FG");
            dto.GL1_3 = mfg.SetDoubleTextField("L1 3 FG");
            dto.GW1_3 = mfg.SetDoubleTextField("W1 3 FG");
            dto.GT1_3 = mfg.SetDoubleTextField("T1 3 FG");
            dto.GT2_3 = mfg.SetDoubleTextField("T2 3 FG");
            dto.GT3_3 = mfg.SetDoubleTextField("T3 3 FG");
            dto.GT4_3 = mfg.SetDoubleTextField("T4 3 FG");
dto.GT5_3 = mfg.SetDoubleTextField("T5 3 FG");
dto.GCoreDens3 = mfg.SetDoubleTextField("FG Core Density 3", MfgFinishedGoods.sOr);
dto.GCoreDensKnitLine3 = mfg.dr["Core Knit Present FG 3"] == DBNull.Value ? false : (bool)mfg.dr["Core Knit Present FG 3"];

// Board Dimensions
dto.GFGLength = mfg.SetDoubleTextField("Length FG");
dto.GFGWidth = mfg.SetDoubleTextField("Width FG");
dto.GFGDiagoanl1 = mfg.SetDoubleTextField("Diagonal FG 1");
dto.GFGDiagoanl2 = mfg.SetDoubleTextField("Diagonal FG 2");
dto.GFGDiagonalDiff = mfg.SetDoubleTextField("Diagonal Diff FG", MfgFinishedGoods.sOr);

// Facer Peel
dto.GFacerPeel3 = mfg.SetDoubleTextField("Facer Peel 3 FG");
dto.GFacerPeel2 = mfg.SetDoubleTextField("Facer Peel 2 FG");
dto.GFacerPeel1 = mfg.SetDoubleTextField("Facer Peel 1 FG");
dto.GFacerPeelAvg = mfg.SetDoubleTextField("Facer Peel FG", MfgFinishedGoods.sOr);

// Nail Pull
dto.GNailPull_3 = mfg.SetDoubleTextField("Nail Pull FG 3");
dto.GNailPull_2 = mfg.SetDoubleTextField("Nail Pull FG 2");
dto.GNailPull_1 = mfg.SetDoubleTextField("Nail Pull FG 1");
dto.GNailPull = mfg.SetDoubleTextField("Nail Pull FG", MfgFinishedGoods.sOr);

// Background colors - run CheckLimits logic
RunCheckLimits(dto);

return dto;
        }

        private void RunCheckLimits(FinishedBoardDataDto dto)
{
    var mfg = _objectsService.MfgFinishedGoods;
    var targets = _objectsService.CProdTargets;
    var ipTargets = _objectsService.CIPProdTargets;

    // IP checks
    if (mfg.drIP["Length"] != DBNull.Value)
        dto.GIPLengthBackground = ipTargets.BoardLengthWithinLimits((double)mfg.drIP["Length"]) != "Red";
    if (mfg.drIP["Width"] != DBNull.Value)
        dto.GIPWidthBackground = ipTargets.BoardWidthWithinLimits((double)mfg.drIP["Width"]) != "Red";
    if (mfg.drIP["Thickness - IP"] != DBNull.Value)
        dto.GThicknessIPBackground = ipTargets.ThicknessWithinLimits((double)mfg.drIP["Thickness - IP"]) != "Red";
    if (mfg.drIP["Core Density - IP"] != DBNull.Value)
        dto.GCoreDensityIPBackground = ipTargets.CoreDensityWithinLimits((double)mfg.drIP["Core Density - IP"]) != "Red";
    if (mfg.drIP["Compressive Strength - IP"] != DBNull.Value)
        dto.GCompressiveIPBackground = ipTargets.CompressionStrWithinLimits((double)mfg.drIP["Compressive Strength - IP"]) != "Red";
    if (mfg.drIP["Compressive Strength 5 - IP"] != DBNull.Value)
        dto.GCompressiveIP5Background = ipTargets.CompressionStrWithinLimits((double)mfg.drIP["Compressive Strength 5 - IP"]) != "Red";

    // FG checks
    if (mfg.dr["FG Core Density"] != DBNull.Value)
        dto.GCoreDensityBackground = targets.CoreDensWithinLimits((double)mfg.dr["FG Core Density"]) != "N";
    if (mfg.dr["FG Core Density 1"] != DBNull.Value)
        dto.GCoreDens1Background = targets.CoreDensWithinLimits((double)mfg.dr["FG Core Density 1"]) != "N";
    if (mfg.dr["FG Core Density 2"] != DBNull.Value)
        dto.GCoreDens2Background = targets.CoreDensWithinLimits((double)mfg.dr["FG Core Density 2"]) != "N";
    if (mfg.dr["FG Core Density 3"] != DBNull.Value)
        dto.GCoreDens3Background = targets.CoreDensWithinLimits((double)mfg.dr["FG Core Density 3"]) != "N";
    if (mfg.dr["Compressive Strength (6) FG"] != DBNull.Value)
        dto.GCompStrFG_Avg6Background = targets.CompressionWithinLimits((double)mfg.dr["Compressive Strength (6) FG"]) != "N";
    if (mfg.dr["Compressive Strength (5) FG"] != DBNull.Value)
        dto.GCompStrFG_Avg5Background = targets.CompressionWithinLimits((double)mfg.dr["Compressive Strength (5) FG"]) != "N";
    if (mfg.dr["Thickness Avg FG"] != DBNull.Value)
        dto.GThicknessBackground = targets.ThicknessWithinLimits((double)mfg.dr["Thickness Avg FG"]) != "N";
    if (mfg.dr["R Value - AVG FG"] != DBNull.Value)
        dto.GRValueBackground = dto.GkFactor_AvgBackground = targets.RValueAged90DWithinLimits((double)mfg.dr["R Value - AVG FG"]) != "N";
    if (mfg.dr["Facer Peel FG"] != DBNull.Value)
        dto.GFacerPeelAvgBackground = dto.GFacerPeelAvg_QCBackground = targets.FacerPeelWithinLimits((double)mfg.dr["Facer Peel FG"]) != "N";
    if (mfg.dr["Retest - AVG Comp Strength (6) FG"] != DBNull.Value)
        dto.GCompStrFGRetest_Avg6Background = targets.CompressionWithinLimits((double)mfg.dr["Retest - AVG Comp Strength (6) FG"]) != "N";
    if (mfg.dr["Retest - AVG Comp Strength (5) FG"] != DBNull.Value)
        dto.GCompStrFGRetest_Avg5Background = targets.CompressionWithinLimits((double)mfg.dr["Retest - AVG Comp Strength (5) FG"]) != "N";
    if (mfg.dr["R Value 90 - AVG FG"] != DBNull.Value)
        dto.GkFactor90Background = targets.RValueAged90DWithinLimits((double)mfg.dr["R Value 90 - AVG FG"]) != "N";
    if (mfg.dr["R Value 180 - AVG FG"] != DBNull.Value)
        dto.GkFactor180Background = targets.RValueAged90DWithinLimits((double)mfg.dr["R Value 180 - AVG FG"]) != "N";
    if (mfg.dr["Diagonal Diff FG"] != DBNull.Value)
        dto.GFGDiagonalDiffBackground = targets.SquarenessWithinLimits((double)mfg.dr["Diagonal Diff FG"]) != "N";

    // Board timestamp check
    if (mfg.dr["Finished Board Time Stamp FG"] == DBNull.Value || mfg.drIP["Test Date"] == DBNull.Value)
    {
        dto.GFBTimeHostBackground = false;
    }
    else
    {
        DateTime fgDate = (DateTime)mfg.dr["Finished Board Time Stamp FG"];
        DateTime ipDate = (DateTime)mfg.drIP["Test Date"];
        dto.GFBTimeHostBackground = fgDate >= new DateTime(2000, 01, 01) &&
            Math.Abs((fgDate - ipDate).TotalMinutes) <= _objectsService.CDefualts.dDelTimeButton;
    }
}
    }
}