using IntugentBackend.Models;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Mfg;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessCheckController : ControllerBase
    {
        private readonly MfgProcessCheck _svc;
        private readonly Cbfile _cbfile;
        private readonly CDefualts _cDefualts;
        private readonly CLists _cLists;
        private readonly ILogger<ProcessCheckController> _logger;

        public ProcessCheckController(MfgProcessCheck svc, Cbfile cbfile, CDefualts cDefualts, CLists cLists, ILogger<ProcessCheckController> logger)
        {
            _svc = svc;
            _cbfile = cbfile;
            _cDefualts = cDefualts;
            _cLists = cLists;
            _logger = logger;
        }

        [HttpGet("data")]
        public IActionResult GetData()
        {
            try
            {
                bool found = _svc.GetDataSet();
                return Ok(new { success = true, data = BuildDto(found) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Process Check data");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("navigate")]
        public IActionResult Navigate([FromBody] ProcessCheckNavigateRequest request)
        {
            try
            {
                if (_svc.dt == null || _svc.dt.Rows.Count == 0)
                    return Ok(new { success = false, error = "No records loaded." });

                switch (request.Direction)
                {
                    case "prev": _svc.drIndex += 1; break;
                    case "next": _svc.drIndex -= 1; break;
                    default: return BadRequest(new { success = false, error = "Invalid direction" });
                }

                if (_svc.drIndex < 0) _svc.drIndex = 0;
                if (_svc.drIndex > _svc.dt.Rows.Count - 1) _svc.drIndex = _svc.dt.Rows.Count - 1;

                _svc.dr = _svc.dt.Rows[_svc.drIndex];
                _svc.UpdateDataSet();

                return Ok(new { success = true, data = BuildDto(true) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating Process Check dataset");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("new-check-sheet")]
        public IActionResult NewCheckSheet()
        {
            try
            {
                int newId;
                using (var cmd = new SqlCommand("Select Next Value for [dbo].[IDProcessCheckSeq]", _cbfile.conAZ))
                {
                    bool opened = _cbfile.conAZ.State != ConnectionState.Open;
                    if (opened) _cbfile.conAZ.Open();
                    try { newId = (int)cmd.ExecuteScalar(); }
                    finally { if (opened) _cbfile.conAZ.Close(); }
                }

                _svc.dr = _svc.dt.NewRow();
                _svc.dr["ID"] = newId;
                _svc.dr["Operator"] = _cDefualts.IDEmployee;
                _svc.dr["IDLocation"] = _cDefualts.IDLocation;
                _svc.dr["Sample Date Time"] = DateTime.Now;
                _svc.dt.Rows.InsertAt(_svc.dr, 0);

                new SqlCommandBuilder(_svc.da);
                _svc.da.Update(_svc.dt);
                _svc.dr = _svc.dt.Rows[0];
                _svc.drIndex = 0;

                return Ok(new { success = true, data = BuildDto(true) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new Process Check sheet");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("copy-dataset")]
        public IActionResult CopyDataset([FromBody] ProcessCheckCopyRequest request)
        {
            try
            {
                if (request.CopyData < 0)
                    return Ok(new { success = false, error = "Choose an appropriate time window" });

                string sql = @"SELECT RN.ID, RN.[Product Code], RN.[Sample Date Time], R1.Employees, R2.sName as 'Check Type', R3.sName as 'Top Board Print', R4.sName as 'Bottom Board Print', R5.sName as 'Perferation', R6.sName as 'Flipper Operating', R7.sName as 'Facer Adhesion', R8.sName as 'Edge Cut', R9.sName as 'Hooder Quality', R10.sName as 'Board Quality', R11.sName as 'Process Check Type', RN.Comment, RN.[Exposed Foam], RN.[Bundle Quantity 1] as 'Bundle 1 - Board Quantity', RN.[Bundle Width 1] as 'Bundle 1 - Width', RN.[Top Board Length 1] as 'Bundle 1 - Top Board Length', RN.[Middle Board Length 1] as 'Bundle 1 - Middle Board Length', RN.[Bottom Board Length 1] as 'Bundle 1 - Bottom Board Length', RN.[Diagonal_1 1] as 'Bundle 1 - Diagonal 1', RN.[Diagonal_2 1] as 'Bundle 1 - Diagonal 2', RN.[Length Average 1] as 'Bundle 1 - Average Length', RN.[Width Average 1] as 'Bundle 1 - Average Width', RN.[Squareness 1] as 'Bundle 1 - Squareness', RN.[Bundle Quantity 2] as 'Bundle 2 - Board Quantity', RN.[Bundle Width 2] as 'Bundle 2 - Width', RN.[Top Board Length 2] as 'Bundle 2 - Top Board Length', RN.[Middle Board Length 2] as 'Bundle 2 - Middle Board Length', RN.[Bottom Board Length 2] as 'Bundle 2 - Bottom Board Length', RN.[Diagonal_1 2] as 'Bundle 2 - Diagonal 1', RN.[Diagonal_2 2] as 'Bundle 2 - Diagonal 2', RN.[Length Average 2] as 'Bundle 2 - Average Length', RN.[Width Average 2] as 'Bundle 2 - Average Width', RN.[Squareness 2] as 'Bundle 2 - Squareness', RN.ThicknessLoc1 as 'Board Thickness Location 1', RN.ThicknessLoc2 as 'Board Thickness Location 2', RN.ThicknessLoc3 as 'Board Thickness Location 3', RN.[Thickness Average] as 'Board Thickness Average', RN.Taper as 'Board Taper', R12.sLocation as 'Location', case when RN.bExclude = 1 then 'true' else 'false' end as 'Excluded from Analysis if 1'
FROM [dbo].[Process Check] as RN
Left Join [Roster] as R1 on RN.Operator = R1.ID
Left Join tblLists as R2 on RN.[Check Type] = R2.ID
Left Join tblLists as R3 on RN.[Top Board Print] = R3.ID
Left Join tblLists as R4 on RN.[Bottom Board Print] = R4.ID
Left Join tblLists as R5 on RN.Perferation = R5.ID
Left Join tblLists as R6 on RN.[Flipper Operating] = R6.ID
Left Join tblLists as R7 on RN.[Facer Adhesion] = R7.ID
Left Join tblLists as R8 on RN.[Edge Cut] = R8.ID
Left Join tblLists as R9 on RN.[Hooder Quality] = R9.ID
Left Join tblLists as R10 on RN.[Board Quality] = R10.ID
Left Join tblLists as R11 on RN.[Process Check Type] = R11.ID
Left Join tblLocations as R12 on RN.IDLocation = R12.ID";

                DateTime dt1 = DateTime.Now;
                switch (request.CopyData)
                {
                    case 0:
                        var today = dt1.Date;
                        sql += " Where [Sample Date Time] >= '" + today + "' And [Sample Date Time] < '" + today.AddDays(1) + "'";
                        break;
                    case 1: sql += " Where [Sample Date Time] <= '" + dt1 + "' And [Sample Date Time] > '" + dt1.AddDays(-1) + "'"; break;
                    case 2: sql += " Where [Sample Date Time] <= '" + dt1 + "' And [Sample Date Time] > '" + dt1.AddDays(-7) + "'"; break;
                    case 3: sql += " Where [Sample Date Time] <= '" + dt1 + "' And [Sample Date Time] > '" + dt1.AddMonths(-1) + "'"; break;
                    case 4: sql += " Where [Sample Date Time] <= '" + dt1 + "' And [Sample Date Time] > '" + dt1.AddMonths(-6) + "'"; break;
                    case 5: sql += " Where [Sample Date Time] <= '" + dt1 + "' And [Sample Date Time] > '" + dt1.AddYears(-1) + "'"; break;
                    case 6: sql += " Where 1=1"; break;
                    default: return BadRequest(new { success = false, error = "Invalid time window" });
                }
                sql += " and RN.IDLocation = " + _cDefualts.IDLocation + " order by [Sample Date Time] Desc";

                var dtCopy = new DataTable();
                var daCopy = new SqlDataAdapter(sql, _cbfile.conAZ);
                int itmp = daCopy.Fill(dtCopy);
                if (itmp < 1)
                    return Ok(new { success = false, error = "There is no Process Check Data for the selected time frame." });

                var sData = new StringBuilder();
                sData.Append(dtCopy.Columns[0].ColumnName);
                for (int icol = 1; icol < dtCopy.Columns.Count; icol++) sData.Append('\t' + dtCopy.Columns[icol].ColumnName);
                for (int irow = 0; irow < dtCopy.Rows.Count; irow++)
                {
                    sData.Append('\n' + (dtCopy.Rows[irow][0]?.ToString() ?? ""));
                    for (int icol = 1; icol < dtCopy.Columns.Count; icol++) sData.Append('\t' + (dtCopy.Rows[irow][icol]?.ToString() ?? ""));
                }

                return Ok(new { success = true, data = sData.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error copying Process Check data");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-bundle1")]
        public IActionResult UpdateBundle1([FromBody] ProcessCheckFieldUpdateRequest request)
        {
            try
            {
                bool width = false, diag = false, length = false;
                switch (request.Name)
                {
                    case "gQuantity": _svc.SetIntFieldValue(request.Value, "Bundle Quantity 1"); break;
                    case "gWidth": _svc.SetDoubleFieldValue(request.Value, "Bundle Width 1"); width = true; break;
                    case "gTopLength": _svc.SetDoubleFieldValue(request.Value, "Top Board Length 1"); length = true; break;
                    case "gMiddleLength": _svc.SetDoubleFieldValue(request.Value, "Middle Board Length 1"); length = true; break;
                    case "gBottomLength": _svc.SetDoubleFieldValue(request.Value, "Bottom Board Length 1"); length = true; break;
                    case "gDiagonal1": _svc.SetDoubleFieldValue(request.Value, "Diagonal_1 1"); diag = true; break;
                    case "gDiagonal2": _svc.SetDoubleFieldValue(request.Value, "Diagonal_2 1"); diag = true; break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }
                RecalcBundle(1, width, diag, length);
                _svc.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto(true) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bundle 1 field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-bundle2")]
        public IActionResult UpdateBundle2([FromBody] ProcessCheckFieldUpdateRequest request)
        {
            try
            {
                bool width = false, diag = false, length = false;
                switch (request.Name)
                {
                    case "gQuantity_2": _svc.SetIntFieldValue(request.Value, "Bundle Quantity 2"); break;
                    case "gWidth_2": _svc.SetDoubleFieldValue(request.Value, "Bundle Width 2"); width = true; break;
                    case "gTopLength_2": _svc.SetDoubleFieldValue(request.Value, "Top Board Length 2"); length = true; break;
                    case "gMiddleLength_2": _svc.SetDoubleFieldValue(request.Value, "Middle Board Length 2"); length = true; break;
                    case "gBottomLength_2": _svc.SetDoubleFieldValue(request.Value, "Bottom Board Length 2"); length = true; break;
                    case "gDiagonal1_2": _svc.SetDoubleFieldValue(request.Value, "Diagonal_1 2"); diag = true; break;
                    case "gDiagonal2_2": _svc.SetDoubleFieldValue(request.Value, "Diagonal_2 2"); diag = true; break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }
                RecalcBundle(2, width, diag, length);
                _svc.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto(true) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bundle 2 field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-thickness")]
        public IActionResult UpdateThickness([FromBody] ProcessCheckFieldUpdateRequest request)
        {
            try
            {
                switch (request.Name)
                {
                    case "gThickness1": _svc.SetDoubleFieldValue(request.Value, "ThicknessLoc1"); break;
                    case "gThickness2": _svc.SetDoubleFieldValue(request.Value, "ThicknessLoc2"); break;
                    case "gThickness3": _svc.SetDoubleFieldValue(request.Value, "ThicknessLoc3"); break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                int ncount = 0; double sum = 0;
                if (_svc.dr["ThicknessLoc1"] != DBNull.Value) { ncount++; sum += (double)_svc.dr["ThicknessLoc1"]; }
                if (_svc.dr["ThicknessLoc2"] != DBNull.Value) { ncount++; sum += (double)_svc.dr["ThicknessLoc2"]; }
                if (_svc.dr["ThicknessLoc3"] != DBNull.Value) { ncount++; sum += (double)_svc.dr["ThicknessLoc3"]; }
                _svc.dr["Thickness Average"] = ncount > 0 ? sum / ncount : DBNull.Value;

                if (_svc.dr["ThicknessLoc1"] != DBNull.Value && _svc.dr["ThicknessLoc3"] != DBNull.Value)
                    _svc.dr["Taper"] = Math.Abs((double)_svc.dr["ThicknessLoc3"] - (double)_svc.dr["ThicknessLoc1"]);
                else
                    _svc.dr["Taper"] = DBNull.Value;

                _svc.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto(true) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating thickness field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-cup-reactivity")]
        public IActionResult UpdateCupReactivity([FromBody] ProcessCheckFieldUpdateRequest request)
        {
            try
            {
                bool mass = false;
                switch (request.Name)
                {
                    case "gEmptyCupMassG": _svc.SetDoubleFieldValue(request.Value, "EmptyCupMassG"); mass = true; break;
                    case "gCreamTimeS": _svc.SetDoubleFieldValue(request.Value, "CreamTimeS"); break;
                    case "gGelTimeS": _svc.SetDoubleFieldValue(request.Value, "GelTimeS"); break;
                    case "gTackFreeTimeS": _svc.SetDoubleFieldValue(request.Value, "TackFreeTimeS"); break;
                    case "gFullCupMassG": _svc.SetDoubleFieldValue(request.Value, "FullCupMassG"); mass = true; break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                if (mass)
                {
                    if (_svc.dr["EmptyCupMassG"] != DBNull.Value && _svc.dr["FullCupMassG"] != DBNull.Value)
                    {
                        double density = ((double)_svc.dr["FullCupMassG"] - (double)_svc.dr["EmptyCupMassG"]) / 453.592 / 32 * 957.506;
                        _svc.dr["FoamDensityPCF"] = density;
                    }
                    else
                    {
                        _svc.dr["FoamDensityPCF"] = DBNull.Value;
                    }
                }

                _svc.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto(true) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cup reactivity field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-combo")]
        public IActionResult UpdateCombo([FromBody] ProcessCheckFieldUpdateRequest request)
        {
            try
            {
                switch (request.Name)
                {
                    case "gProdID": _svc.SetStringFieldValue(request.Value, "Product Code"); break;
                    case "gOperator": _svc.SetIntFieldValue(request.Value, "Operator"); break;
                    case "gType": _svc.SetIntFieldValue(request.Value, "Check Type"); break;
                    case "gTopBoardPrint": _svc.SetIntFieldValue(request.Value, "Top Board Print"); break;
                    case "gBottomBoardPrint": _svc.SetIntFieldValue(request.Value, "Bottom Board Print"); break;
                    case "gPerferation": _svc.SetIntFieldValue(request.Value, "Perferation"); break;
                    case "gFlipper": _svc.SetIntFieldValue(request.Value, "Flipper Operating"); break;
                    case "gAdhesion": _svc.SetIntFieldValue(request.Value, "Facer Adhesion"); break;
                    case "gEdgeCut": _svc.SetIntFieldValue(request.Value, "Edge Cut"); break;
                    case "gHooder": _svc.SetIntFieldValue(request.Value, "Hooder Quality"); break;
                    case "gBoardQuality": _svc.SetIntFieldValue(request.Value, "Board Quality"); break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                _svc.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto(true) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating combo field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-misc")]
        public IActionResult UpdateMisc([FromBody] ProcessCheckFieldUpdateRequest request)
        {
            try
            {
                switch (request.Name)
                {
                    case "gTestDate":
                        if (string.IsNullOrEmpty(request.Value)) { _svc.dr["Sample Date Time"] = DBNull.Value; break; }
                        if (!DateTime.TryParse(request.Value, out var newDate)) break;
                        if (_svc.dr["Sample Date Time"] == DBNull.Value) _svc.dr["Sample Date Time"] = newDate;
                        else _svc.dr["Sample Date Time"] = newDate.Date + ((DateTime)_svc.dr["Sample Date Time"]).TimeOfDay;
                        break;
                    case "gComment": _svc.SetStringFieldValue(request.Value, "Comment"); break;
                    case "gExclude": _svc.dr["bExclude"] = request.Value == "true"; break;
                    case "gExposedFoam": _svc.SetStringFieldValue(request.Value, "Exposed Foam"); break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                _svc.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto(true) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating misc field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-test-time")]
        public IActionResult UpdateTestTime([FromBody] ProcessCheckDateTimeRequest request)
        {
            try
            {
                if (_svc.dr["Sample Date Time"] == DBNull.Value)
                    _svc.dr["Sample Date Time"] = request.Value;
                else
                    _svc.dr["Sample Date Time"] = ((DateTime)_svc.dr["Sample Date Time"]).Date + request.Value.TimeOfDay;

                _svc.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto(true) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating test time");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("update-deviation")]
        public IActionResult UpdateDeviation([FromBody] ProcessCheckFieldUpdateRequest request)
        {
            try
            {
                switch (request.Name)
                {
                    case "gDeviationAbs": _svc.SetDoubleFieldValue(request.Value, "DeviationFromTableAbs"); break;
                    case "gDeviationType": _svc.SetStringFieldValue(request.Value, "DeviationType"); break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                if (_svc.dr["DeviationFromTableAbs"] != DBNull.Value && _svc.dr["DeviationType"] != DBNull.Value)
                {
                    double abs = (double)_svc.dr["DeviationFromTableAbs"];
                    _svc.dr["DeviationFromTableRel"] = (string)_svc.dr["DeviationType"] == "Up" ? abs : -1.0 * abs;
                }
                else
                {
                    _svc.dr["DeviationFromTableRel"] = DBNull.Value;
                }

                _svc.UpdateDataSet();
                return Ok(new { success = true, data = BuildDto(true) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating deviation field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // ========== HELPERS ==========

        private void RecalcBundle(int n, bool width, bool diag, bool length)
        {
            if (width)
            {
                _svc.dr[$"Width Average {n}"] = _svc.dr[$"Bundle Width {n}"];
            }
            else if (diag)
            {
                var d1 = _svc.dr[$"Diagonal_1 {n}"];
                var d2 = _svc.dr[$"Diagonal_2 {n}"];
                _svc.dr[$"Squareness {n}"] = d1 != DBNull.Value && d2 != DBNull.Value
                    ? Math.Abs((double)d1 - (double)d2)
                    : DBNull.Value;
            }
            else if (length)
            {
                int ncount = 0; double sum = 0;
                var top = _svc.dr[$"Top Board Length {n}"];
                var mid = _svc.dr[$"Middle Board Length {n}"];
                var bot = _svc.dr[$"Bottom Board Length {n}"];
                if (top != DBNull.Value) { ncount++; sum += (double)top; }
                if (mid != DBNull.Value) { ncount++; sum += (double)mid; }
                if (bot != DBNull.Value) { ncount++; sum += (double)bot; }
                _svc.dr[$"Length Average {n}"] = ncount > 0 ? sum / ncount : DBNull.Value;
            }
        }

        private ProcessCheckDataDto BuildDto(bool hasData)
        {
            var dto = new ProcessCheckDataDto { GHasData = hasData };

            dto.GLoc1 = _cDefualts.sLocMfg1;
            dto.GLoc2 = _cDefualts.sLocMfg2;
            dto.GLoc3 = _cDefualts.sLocMfg3;

            if (_cLists.dvComProdAll != null)
            {
                foreach (DataRowView row in _cLists.dvComProdAll)
                {
                    dto.GProdIDList.Add(new FilterOptionDto
                    {
                        Code = row["Product Code"]?.ToString(),
                        Name = row["Product"]?.ToString() ?? string.Empty
                    });
                }
            }

            if (!hasData || _svc.dt == null || _svc.dt.Rows.Count == 0)
            {
                dto.GDataSetNextIsEnabled = false;
                dto.GDataSetPrevIsEnabled = false;
                return dto;
            }

            dto.GDataSetNextIsEnabled = _svc.drIndex != 0;
            dto.GDataSetPrevIsEnabled = _svc.drIndex != _svc.dt.Rows.Count - 1;

            var dr = _svc.dr;
            if (dr == null) return dto;

            dto.GID = dr["ID"]?.ToString() ?? string.Empty;
            dto.GTestDate = dr["Sample Date Time"] != DBNull.Value ? (DateTime)dr["Sample Date Time"] : null;
            dto.GProdIDSelected = dr["Product Code"] != DBNull.Value ? dr["Product Code"].ToString()! : string.Empty;
            dto.GOperatorSelectedItem = dr["Operator"] != DBNull.Value ? (int)dr["Operator"] : -1;
            dto.GTypeSelectedValue = dr["Check Type"] != DBNull.Value ? (int)dr["Check Type"] : -1;
            dto.GTopBoardPrint = dr["Top Board Print"] != DBNull.Value ? (int)dr["Top Board Print"] : -1;
            dto.GBottomBoardPrint = dr["Bottom Board Print"] != DBNull.Value ? (int)dr["Bottom Board Print"] : -1;
            dto.GPerferation = dr["Perferation"] != DBNull.Value ? (int)dr["Perferation"] : -1;
            dto.GFlipper = dr["Flipper Operating"] != DBNull.Value ? (int)dr["Flipper Operating"] : -1;
            dto.GAdhesionSelected = dr["Facer Adhesion"] != DBNull.Value ? (int)dr["Facer Adhesion"] : -1;
            dto.GEdgeCutSelected = dr["Edge Cut"] != DBNull.Value ? (int)dr["Edge Cut"] : -1;
            dto.GHooderSelected = dr["Hooder Quality"] != DBNull.Value ? (int)dr["Hooder Quality"] : -1;
            dto.GBoardQualitySelected = dr["Board Quality"] != DBNull.Value ? (int)dr["Board Quality"] : -1;

            dto.GQuantity = dr["Bundle Quantity 1"]?.ToString() ?? string.Empty;
            dto.GWidth = _svc.SetDoubleTextField("Bundle Width 1");
            dto.GTopLength = _svc.SetDoubleTextField("Top Board Length 1");
            dto.GMiddleLength = _svc.SetDoubleTextField("Middle Board Length 1");
            dto.GBottomLength = _svc.SetDoubleTextField("Bottom Board Length 1");
            dto.GDiagonal1 = _svc.SetDoubleTextField("Diagonal_1 1");
            dto.GDiagonal2 = _svc.SetDoubleTextField("Diagonal_2 1");
            dto.GWidthAvg_1 = _svc.SetDoubleTextField("Width Average 1", "0.000");
            dto.GSquareness_1 = _svc.SetDoubleTextField("Squareness 1", "0.000");
            dto.GLengthAvg_1 = _svc.SetDoubleTextField("Length Average 1", "0.000");

            dto.GQuantity_2 = dr["Bundle Quantity 2"] != DBNull.Value ? dr["Bundle Quantity 2"].ToString()! : string.Empty;
            dto.GWidth_2 = _svc.SetDoubleTextField("Bundle Width 2");
            dto.GTopLength_2 = _svc.SetDoubleTextField("Top Board Length 2");
            dto.GMiddleLength_2 = _svc.SetDoubleTextField("Middle Board Length 2");
            dto.GBottomLength_2 = _svc.SetDoubleTextField("Bottom Board Length 2");
            dto.GDiagonal1_2 = _svc.SetDoubleTextField("Diagonal_1 2");
            dto.GDiagonal2_2 = _svc.SetDoubleTextField("Diagonal_2 2");
            dto.GWidthAvg_2 = _svc.SetDoubleTextField("Width Average 2", "0.000");
            dto.GSquareness_2 = _svc.SetDoubleTextField("Squareness 2", "0.000");
            dto.GLengthAvg_2 = _svc.SetDoubleTextField("Length Average 2", "0.000");

            dto.GThickness1 = _svc.SetDoubleTextField("ThicknessLoc1");
            dto.GThickness2 = _svc.SetDoubleTextField("ThicknessLoc2");
            dto.GThickness3 = _svc.SetDoubleTextField("ThicknessLoc3");
            dto.GThicknessAvg = _svc.SetDoubleTextField("Thickness Average", "0.000");
            dto.GTaper = _svc.SetDoubleTextField("Taper", "0.000");

            dto.GExcludeIsChecked = dr["bExclude"] != DBNull.Value && (bool)dr["bExclude"];
            dto.GComment = dr["Comment"] != DBNull.Value ? dr["Comment"].ToString()! : string.Empty;
            dto.GExposedFoam = dr["Exposed Foam"] != DBNull.Value ? dr["Exposed Foam"].ToString()! : string.Empty;

            dto.GEmptyCupMassG = _svc.SetDoubleTextField("EmptyCupMassG");
            dto.GCreamTimeS = _svc.SetDoubleTextField("CreamTimeS");
            dto.GGelTimeS = _svc.SetDoubleTextField("GelTimeS");
            dto.GTackFreeTimeS = _svc.SetDoubleTextField("TackFreeTimeS");
            dto.GFullCupMassG = _svc.SetDoubleTextField("FullCupMassG");
            dto.GFoamDensityPCF = _svc.SetDoubleTextField("FoamDensityPCF", "0.000");

            dto.GBoardDeviationRel = _svc.SetDoubleTextField("DeviationFromTableRel", "0.000");
            dto.GDeviationAbs = _svc.SetDoubleTextField("DeviationFromTableAbs");
            dto.GDeviationType = dr["DeviationType"] != DBNull.Value ? dr["DeviationType"].ToString()! : string.Empty;

            return dto;
        }
    }
}
