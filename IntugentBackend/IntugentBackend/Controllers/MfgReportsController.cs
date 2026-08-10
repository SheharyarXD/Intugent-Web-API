using IntugentBackend.Models;
using IntugentBackend.Services.Mfg;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MfgReportsController : ControllerBase
    {
        private readonly MfgReports _mfgReports;
        private readonly ILogger<MfgReportsController> _logger;

        public MfgReportsController(MfgReports mfgReports, ILogger<MfgReportsController> logger)
        {
            _mfgReports = mfgReports;
            _logger = logger;
        }

        [HttpGet("data")]
        public IActionResult GetData()
        {
            try
            {
                var date = DateTime.Now;
                bool ok = _mfgReports.MfgReport(date);
                return Ok(new { success = ok, data = ToRows(_mfgReports.dt) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Mfg Reports data");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("date")]
        public IActionResult ByDate([FromBody] MfgReportDateRequest request)
        {
            try
            {
                bool ok = _mfgReports.MfgReport(request.Date);
                return Ok(new { success = ok, data = ToRows(_mfgReports.dt) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running Mfg Reports for date {Date}", request.Date);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        private static List<MfgReportRowDto> ToRows(DataTable dt)
        {
            var rows = new List<MfgReportRowDto>();
            if (dt == null) return rows;

            string Fmt(DataRow r, string col)
            {
                if (!dt.Columns.Contains(col) || r[col] == DBNull.Value) return string.Empty;
                var val = r[col];
                if (val is double d) return d.ToString("0.###");
                if (val is DateTime dt2) return dt2.ToString("MM/dd/yyyy hh:mm tt");
                return val.ToString() ?? string.Empty;
            }

            foreach (DataRow row in dt.Rows)
            {
                if (dt.Columns.Contains("QC_Test_Date") && row["QC_Test_Date"] == DBNull.Value) continue;

                rows.Add(new MfgReportRowDto
                {
                    Id = Fmt(row, "ID"),
                    MfgTime = Fmt(row, "Mfg_Time"),
                    QcTestDate = Fmt(row, "QC_Test_Date"),
                    ProductType = Fmt(row, "Product_Type"),
                    Product = Fmt(row, "Product"),
                    PassFail = Fmt(row, "PassFail"),
                    Note = Fmt(row, "Note"),
                    Thickness = Fmt(row, "Thickness"),
                    ThicknessFlag = Fmt(row, "Thickness_P"),
                    RValue = Fmt(row, "R_Value"),
                    RValueFlag = Fmt(row, "R_Value_P"),
                    CompStrength = Fmt(row, "Comp_Strength"),
                    CsFlag = Fmt(row, "CS_P"),
                    CoreDens = Fmt(row, "Core_Dens"),
                    CoreDensFlag = Fmt(row, "Core_Dens_P"),
                    Squareness = Fmt(row, "Squareness"),
                    SquarenessFlag = Fmt(row, "Squareness_P"),
                    Length = Fmt(row, "Length"),
                    LengthFlag = Fmt(row, "Length_P"),
                    Width = Fmt(row, "Width"),
                    WidthFlag = Fmt(row, "Width_P")
                });
            }

            return rows;
        }
    }
}
