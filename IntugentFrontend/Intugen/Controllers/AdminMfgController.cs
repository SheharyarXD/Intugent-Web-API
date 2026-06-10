using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using IntugentWebApp.Utilities;

namespace Intugen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminMfgController : ControllerBase
    {
        private readonly ObjectsService _objectsService;

        public AdminMfgController(ObjectsService objectsService)
        {
            _objectsService = objectsService;
        }

        [HttpPost("upload")]
        public IActionResult UploadIPTargets([FromBody] List<List<string>> clipboardData)
        {
            try
            {
                if (clipboardData == null || clipboardData.Count < 4 || clipboardData[0][0] != "Intugent PI - Green Product Targets")
                {
                    return BadRequest(new { success = false, message = "Invalid clipboard data" });
                }

                _objectsService.MfgAdmin.sql = "select * from [dbo].[IP Product Targets]";
                _objectsService.MfgAdmin.dt.Clear();
                _objectsService.MfgAdmin.da = new SqlDataAdapter(_objectsService.MfgAdmin.sql, _objectsService.MfgAdmin.Cbfile.conAZ);
                _objectsService.MfgAdmin.da.Fill(_objectsService.MfgAdmin.dt);
                _objectsService.MfgAdmin.dtCopy = _objectsService.MfgAdmin.dt.Clone();

                int irEx = 1, irF = 2;

                for (int ir = 3; ir < clipboardData.Count; ir++)
                {
                    if (ir >= clipboardData.Count) break;
                    
                    var row = clipboardData[ir];
                    if (row.Count == 0) continue;
                    
                    string stmp = row[0].Trim();
                    if (string.IsNullOrEmpty(stmp)) break;

                    _objectsService.MfgAdmin.sql = "Select * from [Product Matrix] where [Product Code] = '" + stmp + "'";
                    _objectsService.MfgAdmin.dtPr.Clear();
                    _objectsService.MfgAdmin.daPr = new SqlDataAdapter(_objectsService.MfgAdmin.sql, _objectsService.MfgAdmin.Cbfile.conAZ);
                    _objectsService.MfgAdmin.daPr.Fill(_objectsService.MfgAdmin.dtPr);

                    if (_objectsService.MfgAdmin.dtPr.Rows.Count == 0) continue;

                    DataRow dr;
                    var drs = _objectsService.MfgAdmin.dt.Select("[Product Code (Local)] = '" + stmp + "'");
                    if (drs.Length > 0)
                    {
                        dr = drs[0];
                    }
                    else
                    {
                        dr = _objectsService.MfgAdmin.dt.NewRow();
                        _objectsService.MfgAdmin.dt.Rows.Add(dr);
                        dr["Product Code (Local)"] = stmp;
                    }

                    for (int ic = 1; ic < row.Count; ic++)
                    {
                        if (ic < clipboardData[irEx].Count && clipboardData[irEx][ic] == "Extract")
                        {
                            if (double.TryParse(row[ic], out double dtmp) && ic < clipboardData[irF].Count)
                            {
                                dr[clipboardData[irF][ic]] = dtmp;
                            }
                        }
                    }
                    _objectsService.MfgAdmin.dtCopy.ImportRow(dr);
                }

                _objectsService.MfgAdmin.UpdateDataSet();

                var jsonResult = DataTableToJson(_objectsService.MfgAdmin.dtCopy);
                return Ok(jsonResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private List<Dictionary<string, object>> DataTableToJson(DataTable table)
        {
            var result = new List<Dictionary<string, object>>();
            foreach (DataRow row in table.Rows)
            {
                var rowDict = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    rowDict[col.ColumnName] = row[col];
                }
                result.Add(rowDict);
            }
            return result;
        }
    }
}