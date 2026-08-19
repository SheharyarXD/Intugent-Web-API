using IntugentBackend.Models;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using Microsoft.AspNetCore.Mvc;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DimStabilityController : ControllerBase
    {
        private readonly MfgDimStability _mfgDimStability;
        private readonly Cbfile _cbfile;
        private readonly MfgHome _mfgHome;
        private readonly CLists _cLists;
        private readonly MfgInProcess _mfgInProcess;
        private readonly MfgFinishedGoods _mfgFinishedGoods;
        private readonly MfgPlantData _mfgPlantData;
        private readonly MfgJetMixing _mfgJetMixing;
        private readonly CProdTargets _cProdTargets;
        private readonly ILogger<DimStabilityController> _logger;

        private static readonly string[] OvenFreezerLocs = { "H1", "H2", "C1", "C2" };

        public DimStabilityController(
            MfgDimStability mfgDimStability,
            Cbfile cbfile,
            MfgHome mfgHome,
            CLists cLists,
            MfgInProcess mfgInProcess,
            MfgFinishedGoods mfgFinishedGoods,
            MfgPlantData mfgPlantData,
            MfgJetMixing mfgJetMixing,
            CProdTargets cProdTargets,
            ILogger<DimStabilityController> logger)
        {
            _mfgDimStability = mfgDimStability;
            _cbfile = cbfile;
            _mfgHome = mfgHome;
            _cLists = cLists;
            _mfgInProcess = mfgInProcess;
            _mfgFinishedGoods = mfgFinishedGoods;
            _mfgPlantData = mfgPlantData;
            _mfgJetMixing = mfgJetMixing;
            _cProdTargets = cProdTargets;
            _logger = logger;
        }

        [HttpGet("data")]
        public IActionResult GetData()
        {
            try
            {
                // drIP/drFG depend on MfgInProcess/MfgFinishedGoods already having loaded a row for the
                // current dataset. Since these are singletons, that's only guaranteed if the user
                // happened to visit another Mfg page first — so load them explicitly here too.
                _mfgInProcess.GetDataSet();
                _mfgFinishedGoods.GetDataSet();
                _mfgDimStability.GetDataSet();
                LinkCrossReferences();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Dim Stability data");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("navigate")]
        public IActionResult Navigate([FromBody] DimNavigateRequest request)
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
                LinkCrossReferences();

                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating Dim Stability dataset");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Updates a single Oven/Freezer dimension cell. request.Name format: "Init_H1_L1", "Final_C2_T3", etc.
        /// </summary>
        [HttpPost("update-dimension")]
        public IActionResult UpdateDimension([FromBody] DimFieldUpdateRequest request)
        {
            try
            {
                var parts = request.Name.Split('_');
                if (parts.Length != 3 || (parts[0] != "Init" && parts[0] != "Final") || Array.IndexOf(OvenFreezerLocs, parts[1]) < 0)
                    return BadRequest(new { success = false, error = $"Unknown dimension field: {request.Name}" });

                string initFinal = parts[0] == "Init" ? "Initial" : "Final";
                string loc = parts[1];
                string dimPart = parts[2]; // e.g. L1, W2, T5
                char dim = dimPart[0];

                _mfgDimStability.bDataSetChanged = true;
                _mfgDimStability.SetDoubleFieldValue(request.Value, $"{initFinal} {loc} - {dimPart}");

                RecalcDimensionGroup(loc, dim);

                _mfgDimStability.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating dimension field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Updates a Walk-in Freezer major-dimension cell. request.Name format: "Init_WF_L1", "Final_WF_W3", etc.
        /// </summary>
        [HttpPost("update-wf-dim")]
        public IActionResult UpdateWfDim([FromBody] DimFieldUpdateRequest request)
        {
            try
            {
                var parts = request.Name.Split('_');
                if (parts.Length != 3 || (parts[0] != "Init" && parts[0] != "Final") || parts[1] != "WF")
                    return BadRequest(new { success = false, error = $"Unknown WF dimension field: {request.Name}" });

                string initFinal = parts[0] == "Init" ? "Initial" : "Final";
                string dimPart = parts[2]; // L1..L3 or W1..W3
                char dim = dimPart[0];

                _mfgDimStability.bDataSetChanged = true;
                _mfgDimStability.SetDoubleFieldValue(request.Value, $"{initFinal}WF-{dimPart}");

                int nCount = 0; double sumInit = 0, sumFinal = 0;
                for (int i = 1; i <= 3; i++)
                {
                    var initVal = _mfgDimStability.dr[$"InitialWF-{dim}{i}"];
                    var finalVal = _mfgDimStability.dr[$"FinalWF-{dim}{i}"];
                    if (initVal != DBNull.Value && finalVal != DBNull.Value)
                    {
                        nCount++;
                        sumInit += (double)initVal;
                        sumFinal += (double)finalVal;
                    }
                }

                string pctCol = dim == 'L' ? "%ChangeLengthWF" : "%ChangeWidthWF";
                if (nCount > 1)
                    _mfgDimStability.dr[pctCol] = 100.0 * (sumFinal - sumInit) / sumInit;
                else
                    _mfgDimStability.dr[pctCol] = DBNull.Value;

                _mfgDimStability.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating WF dimension field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Updates a Walk-in Freezer depth cell. request.Name format: "Side1_Depth1".."Side2_Depth5".
        /// </summary>
        [HttpPost("update-wf-depth")]
        public IActionResult UpdateWfDepth([FromBody] DimFieldUpdateRequest request)
        {
            try
            {
                var parts = request.Name.Split('_');
                if (parts.Length != 2 || (parts[0] != "Side1" && parts[0] != "Side2"))
                    return BadRequest(new { success = false, error = $"Unknown depth field: {request.Name}" });

                string loc = parts[0] == "Side1" ? "Loc1" : "Loc2";
                string depthPart = parts[1]; // Depth1..Depth5

                _mfgDimStability.bDataSetChanged = true;
                _mfgDimStability.SetDoubleFieldValue(request.Value, $"{loc}{depthPart}");

                int nCount1 = 0, nCount2 = 0;
                double sum = 0, max = double.MinValue;
                for (int i = 1; i <= 5; i++)
                {
                    var v1 = _mfgDimStability.dr[$"Loc1Depth{i}"];
                    if (v1 != DBNull.Value) { nCount1++; double d = (double)v1; sum += d; if (d > max) max = d; }

                    var v2 = _mfgDimStability.dr[$"Loc2Depth{i}"];
                    if (v2 != DBNull.Value) { nCount2++; double d = (double)v2; sum += d; if (d > max) max = d; }
                }

                if (nCount1 > 3 && nCount2 > 3)
                {
                    _mfgDimStability.dr["AvgDepth"] = sum / (nCount1 + nCount2);
                    _mfgDimStability.dr["MaxDepth"] = max;
                }
                else
                {
                    _mfgDimStability.dr["AvgDepth"] = DBNull.Value;
                    _mfgDimStability.dr["MaxDepth"] = DBNull.Value;
                }

                _mfgDimStability.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating WF depth field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-checkbox")]
        public IActionResult UpdateCheckbox([FromBody] DimBoolUpdateRequest request)
        {
            try
            {
                _mfgDimStability.bDataSetChanged = true;
                switch (request.Name)
                {
                    case "gEdgeH1": _mfgDimStability.dr["Edge H1"] = request.Value; break;
                    case "gEdgeH2": _mfgDimStability.dr["Edge H2"] = request.Value; break;
                    case "gEdgeC1": _mfgDimStability.dr["Edge C1"] = request.Value; break;
                    case "gEdgeC2": _mfgDimStability.dr["Edge C2"] = request.Value; break;
                    case "gDimStabilityDone": _mfgDimStability.dr["DS Testing Complete"] = request.Value; break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                _mfgDimStability.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating checkbox {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-datetime")]
        public IActionResult UpdateDateTime([FromBody] DimDateTimeUpdateRequest request)
        {
            try
            {
                _mfgDimStability.bDataSetChanged = true;
                _mfgDimStability.dr["DateSampleIn"] = request.Value ?? (object)DBNull.Value;
                _mfgDimStability.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating DateSampleIn");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-wf-info")]
        public IActionResult UpdateWfInfo([FromBody] DimFieldUpdateRequest request)
        {
            try
            {
                _mfgDimStability.bDataSetChanged = true;
                switch (request.Name)
                {
                    case "gDeviation": _mfgDimStability.SetStringFieldValue(request.Value, "DevFromTable"); break;
                    case "gDevType": _mfgDimStability.SetStringFieldValue(request.Value, "DevType"); break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                _mfgDimStability.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating WF info field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // ========== HELPERS ==========

        private void LinkCrossReferences()
        {
            _mfgDimStability.drIP = _mfgInProcess.dr;
            _mfgDimStability.drFG = _mfgFinishedGoods.dr;
        }

        private void RecalcDimensionGroup(string loc, char dim)
        {
            int count = dim == 'T' ? 5 : 3;
            int nCount = 0;
            double sumInit = 0, sumFinal = 0;

            for (int i = 1; i <= count; i++)
            {
                var initVal = _mfgDimStability.dr[$"Initial {loc} - {dim}{i}"];
                var finalVal = _mfgDimStability.dr[$"Final {loc} - {dim}{i}"];
                if (initVal != DBNull.Value && finalVal != DBNull.Value)
                {
                    nCount++;
                    sumInit += (double)initVal;
                    sumFinal += (double)finalVal;
                }
            }

            string pctCol = $"%change {dim} - {loc}";
            if (nCount == count && sumInit > 0)
                _mfgDimStability.dr[pctCol] = 100.0 * (sumFinal - sumInit) / sumInit;
            else
                _mfgDimStability.dr[pctCol] = DBNull.Value;

            bool isOven = loc == "H1" || loc == "H2";
            string group = (isOven ? "Oven" : "Freezer") + "-" + dim;
            DimensionChanges(group);
        }

        private void DimensionChanges(string sType)
        {
            var dr = _mfgDimStability.dr;

            void Average(string col1, string col2, string outCol)
            {
                int nCount = 0;
                double sum = 0;
                if (dr[col1] != DBNull.Value) { nCount++; sum += (double)dr[col1]; }
                if (dr[col2] != DBNull.Value) { nCount++; sum += (double)dr[col2]; }
                dr[outCol] = nCount == 2 ? sum / nCount : DBNull.Value;
            }

            switch (sType)
            {
                case "Oven-L": Average("%change L - H1", "%change L - H2", "% Change Length Oven"); break;
                case "Oven-W": Average("%change W - H1", "%change W - H2", "% Change Width Oven"); break;
                case "Oven-T": Average("%change T - H1", "%change T - H2", "% Change Thickness Oven"); break;
                case "Freezer-L": Average("%change L - C1", "%change L - C2", "% Change Length Freezer"); break;
                case "Freezer-W": Average("%change W - C1", "%change W - C2", "% Change Width Freezer"); break;
                case "Freezer-T": Average("%change T - C1", "%change T - C2", "% Change Thickness Freezer"); break;
            }
        }

        // ========== DTO BUILDER ==========

        private DimStabilityDataDto BuildDto()
        {
            var mfg = _mfgDimStability;
            var dto = new DimStabilityDataDto();

            dto.GDataSetNextIsEnabled = _cbfile.iIDMfgIndex != 0;
            dto.GDataSetPrevIsEnabled = _cbfile.iIDMfgIndex != _mfgHome.dt.Rows.Count - 1;

            if (mfg.dr == null) return dto;

            dto.GID = mfg.dr["ID4ALL"]?.ToString() ?? string.Empty;
            dto.GDimStabilityDoneIsChecked = mfg.dr["DS Testing Complete"] != DBNull.Value && (bool)mfg.dr["DS Testing Complete"];
            dto.GProductionDate = mfg.drIP != null && mfg.drIP["Test Date"] != DBNull.Value ? ((DateTime)mfg.drIP["Test Date"]).ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty;
            dto.GProductionTime = mfg.drFG != null && mfg.drFG["Finished Board Time Stamp FG"] != DBNull.Value ? ((DateTime)mfg.drFG["Finished Board Time Stamp FG"]).ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty;
            dto.GProductCode = mfg.drIP != null && mfg.drIP["Product ID"] != DBNull.Value ? mfg.drIP["Product ID"].ToString()! : string.Empty;

            dto.GEdgeH1IsChecked = mfg.dr["Edge H1"] != DBNull.Value && (bool)mfg.dr["Edge H1"];
            dto.GEdgeH2IsChecked = mfg.dr["Edge H2"] != DBNull.Value && (bool)mfg.dr["Edge H2"];
            dto.GEdgeC1IsChecked = mfg.dr["Edge C1"] != DBNull.Value && (bool)mfg.dr["Edge C1"];
            dto.GEdgeC2IsChecked = mfg.dr["Edge C2"] != DBNull.Value && (bool)mfg.dr["Edge C2"];

            foreach (var loc in OvenFreezerLocs)
            {
                foreach (var (dim, n) in DimSpecs())
                {
                    string key1 = $"Init_{loc}_{dim}{n}";
                    string key2 = $"Final_{loc}_{dim}{n}";
                    dto.Dims[key1] = mfg.SetDoubleTextField($"Initial {loc} - {dim}{n}");
                    dto.Dims[key2] = mfg.SetDoubleTextField($"Final {loc} - {dim}{n}");
                }
            }

            dto.GChangeOvenLength = mfg.SetDoubleTextField("% Change Length Oven", MfgDimStability.sOr);
            dto.GChangeOvenWidth = mfg.SetDoubleTextField("% Change Width Oven", MfgDimStability.sOr);
            dto.GChangeOvenThickness = mfg.SetDoubleTextField("% Change Thickness Oven", MfgDimStability.sOr);
            dto.GChangeFreezerLength = mfg.SetDoubleTextField("% Change Length Freezer", MfgDimStability.sOr);
            dto.GChangeFreezerWidth = mfg.SetDoubleTextField("% Change Width Freezer", MfgDimStability.sOr);
            dto.GChangeFreezerThickness = mfg.SetDoubleTextField("% Change Thickness Freezer", MfgDimStability.sOr);

            for (int i = 1; i <= 3; i++)
            {
                dto.WFDims[$"Init_WF_L{i}"] = mfg.SetDoubleTextField($"InitialWF-L{i}");
                dto.WFDims[$"Final_WF_L{i}"] = mfg.SetDoubleTextField($"FinalWF-L{i}");
                dto.WFDims[$"Init_WF_W{i}"] = mfg.SetDoubleTextField($"InitialWF-W{i}");
                dto.WFDims[$"Final_WF_W{i}"] = mfg.SetDoubleTextField($"FinalWF-W{i}");
            }
            dto.GChangeWFLength = mfg.SetDoubleTextField("%ChangeLengthWF", MfgDimStability.sOr);
            dto.GChangeWFWidth = mfg.SetDoubleTextField("%ChangeWidthWF", MfgDimStability.sOr);

            for (int i = 1; i <= 5; i++)
            {
                dto.Depths[$"Side1_Depth{i}"] = mfg.SetDoubleTextField($"Loc1Depth{i}");
                dto.Depths[$"Side2_Depth{i}"] = mfg.SetDoubleTextField($"Loc2Depth{i}");
            }
            dto.GAvgDepth = mfg.SetDoubleTextField("AvgDepth", MfgDimStability.sOr);
            dto.GMaxDepth = mfg.SetDoubleTextField("MaxDepth", MfgDimStability.sDef);

            dto.GTestDateTime = mfg.dr["DateSampleIn"] != DBNull.Value ? (DateTime)mfg.dr["DateSampleIn"] : null;
            dto.GDeviation = mfg.dr["DevFromTable"] != DBNull.Value ? mfg.dr["DevFromTable"].ToString()! : string.Empty;
            dto.GDevType = mfg.dr["DevType"] != DBNull.Value ? mfg.dr["DevType"].ToString()! : string.Empty;

            RunCheckLimits(dto);

            return dto;
        }

        private static IEnumerable<(char dim, int n)> DimSpecs()
        {
            foreach (var n in new[] { 1, 2, 3 }) yield return ('L', n);
            foreach (var n in new[] { 1, 2, 3 }) yield return ('W', n);
            foreach (var n in new[] { 1, 2, 3, 4, 5 }) yield return ('T', n);
        }

        private void RunCheckLimits(DimStabilityDataDto dto)
        {
            var mfg = _mfgDimStability;
            var targets = _cProdTargets;

            dto.GChangeOvenLengthBackground = mfg.dr["% Change Length Oven"] == DBNull.Value
                || targets.DimStabLengthWithinLimits((double)mfg.dr["% Change Length Oven"], "hot") != "N";
            dto.GChangeOvenWidthBackground = mfg.dr["% Change Width Oven"] == DBNull.Value
                || targets.DimStabWidthWithinLimits((double)mfg.dr["% Change Width Oven"], "hot") != "N";
            dto.GChangeOvenThicknessBackground = mfg.dr["% Change Thickness Oven"] == DBNull.Value
                || targets.DimStabThicknessWithinLimits((double)mfg.dr["% Change Thickness Oven"], "hot") != "N";
            dto.GChangeFreezerLengthBackground = mfg.dr["% Change Length Freezer"] == DBNull.Value
                || targets.DimStabLengthWithinLimits((double)mfg.dr["% Change Length Freezer"], "cold") != "N";
            dto.GChangeFreezerWidthBackground = mfg.dr["% Change Width Freezer"] == DBNull.Value
                || targets.DimStabWidthWithinLimits((double)mfg.dr["% Change Width Freezer"], "cold") != "N";
            dto.GChangeFreezerThicknessBackground = mfg.dr["% Change Thickness Freezer"] == DBNull.Value
                || targets.DimStabThicknessWithinLimits((double)mfg.dr["% Change Thickness Freezer"], "cold") != "N";
        }
    }
}
