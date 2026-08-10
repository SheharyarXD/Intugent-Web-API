using IntugentBackend.Models;
using IntugentBackend.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminMfgController : ControllerBase
    {
        private const string ExpectedMarker = "Intugent PI - Green Product Targets";

        private readonly MfgAdmin _svc;
        private readonly ILogger<AdminMfgController> _logger;

        public AdminMfgController(MfgAdmin svc, ILogger<AdminMfgController> logger)
        {
            _svc = svc;
            _logger = logger;
        }

        /// <summary>
        /// Bulk-imports "IP Product Targets" from a grid pasted (tab/newline delimited) from Excel.
        /// Row 0 must be the marker title; row 1 flags which columns to import ("Extract"); row 2 holds
        /// the target DB column name for each flagged column; rows 3+ are data, keyed by Product Code.
        /// </summary>
        [HttpPost("upload")]
        public IActionResult Upload([FromBody] IPTargetUploadRequest request)
        {
            try
            {
                var clip = request.Clipboard;
                if (clip == null || clip.Count < 4 || clip[0].Count == 0 || clip[0][0] != ExpectedMarker)
                    return Ok(new { success = false, error = "Invalid clipboard data" });

                const int irEx = 1, irF = 2;

                _svc.sql = "select * from [dbo].[IP Product Targets]";
                _svc.dt.Clear();
                _svc.da = new SqlDataAdapter(_svc.sql, _svc.Cbfile.conAZ);
                _svc.da.Fill(_svc.dt);
                _svc.dtCopy = _svc.dt.Clone();

                for (int ir = 3; ir < clip.Count; ir++)
                {
                    if (clip[ir].Count == 0) break;
                    string productCode = clip[ir][0].Trim();
                    if (productCode == string.Empty) break;

                    _svc.sql = "Select * from [Product Matrix] where [Product Code] = '" + productCode.Replace("'", "''") + "'";
                    _svc.dtPr.Clear();
                    _svc.daPr = new SqlDataAdapter(_svc.sql, _svc.Cbfile.conAZ);
                    int found = _svc.daPr.Fill(_svc.dtPr);
                    if (found == 0) continue;

                    var existing = _svc.dt.Select("[Product Code (Local)] = '" + productCode.Replace("'", "''") + "'");
                    DataRow dr;
                    if (existing.Length > 0)
                    {
                        dr = existing[0];
                    }
                    else
                    {
                        dr = _svc.dt.NewRow();
                        _svc.dt.Rows.Add(dr);
                        dr["Product Code (Local)"] = productCode;
                    }

                    for (int ic = 1; ic < clip[ir].Count; ic++)
                    {
                        if (ic >= clip[irEx].Count || ic >= clip[irF].Count) break;
                        if (clip[irEx][ic] == "Extract")
                        {
                            if (double.TryParse(clip[ir][ic], out double dtmp))
                                dr[clip[irF][ic]] = dtmp;
                        }
                    }
                    _svc.dtCopy.ImportRow(dr);
                }

                _svc.UpdateDataSet();

                return Ok(new { success = true, data = ToRows(_svc.dtCopy) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading clipboard data and saving IP Product Targets");
                return Ok(new { success = false, error = "Error in reading clipboard data and saving it to database" });
            }
        }

        private static List<IPTargetRowDto> ToRows(DataTable dt)
        {
            var rows = new List<IPTargetRowDto>();
            foreach (DataRow row in dt.Rows)
            {
                var rowDto = new IPTargetRowDto();
                foreach (DataColumn col in dt.Columns)
                    rowDto.Values[col.ColumnName] = row[col] == DBNull.Value ? string.Empty : row[col].ToString() ?? string.Empty;
                rows.Add(rowDto);
            }
            return rows;
        }
    }
}
