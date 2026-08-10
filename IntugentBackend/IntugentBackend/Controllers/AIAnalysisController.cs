using IntugentBackend.Models;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIAnalysisController : ControllerBase
    {
        private readonly CDBase _cdBase;
        private readonly CNNData _cnnData;
        private readonly cMatrix _cMatrix;
        private readonly SessionState _sessionState;
        private readonly ILogger<AIAnalysisController> _logger;

        public AIAnalysisController(CDBase cdBase, CNNData cnnData, cMatrix cMatrix, SessionState sessionState, ILogger<AIAnalysisController> logger)
        {
            _cdBase = cdBase;
            _cnnData = cnnData;
            _cMatrix = cMatrix;
            _sessionState = sessionState;
            _logger = logger;
        }

        [HttpGet("data")]
        public IActionResult GetData()
        {
            try
            {
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading AI Analysis view");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("upload-data")]
        public IActionResult UploadData()
        {
            try
            {
                if (_cdBase.dr == null)
                    return Ok(new { success = false, error = "No AI model is selected. Select a model on the AI Home page first." });

                var source = _cdBase.dr.Table.Columns.Contains("sDataSource") && _cdBase.dr["sDataSource"] != DBNull.Value
                    ? _cdBase.dr["sDataSource"].ToString() : null;
                if (string.IsNullOrEmpty(source))
                    return Ok(new { success = false, error = "Choose a value for Data Source." });
                if (source == "SQL")
                    return Ok(new { success = false, error = "SQL query data source is not implemented yet." });

                bool ok = _cnnData.ReadData(_cdBase);
                if (!ok)
                    return Ok(new { success = false, error = "Could not read the data file. Check the file path." });

                _cnnData.nnModel.nInputNeurons = _cnnData.nInputNeurons;
                _sessionState.gInputIndex = 0;

                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading AI Analysis data");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("gen-info")]
        public IActionResult GenInfo([FromBody] AiAnalysisFieldRequest request)
        {
            try
            {
                if (_cdBase.dr == null)
                    return Ok(new { success = false, error = "No AI model is selected." });

                switch (request.Name)
                {
                    case "gStudyName": _cdBase.dr["sNote"] = request.Value ?? (object)DBNull.Value; break;
                    case "gGroup": _cdBase.dr["sGroup"] = request.Value ?? (object)DBNull.Value; break;
                    case "gProperty": _cdBase.dr["sProperty"] = request.Value ?? (object)DBNull.Value; break;
                    case "gSource": _cdBase.dr["sDataSource"] = request.Value ?? (object)DBNull.Value; break;
                    case "gSQL": _cdBase.dr["sSQL"] = request.Value ?? (object)DBNull.Value; break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                _cdBase.UpdateModel();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating AI Analysis field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("browse-file")]
        public IActionResult BrowseFile([FromBody] AiAnalysisFileRequest request)
        {
            try
            {
                if (_cdBase.dr == null)
                    return Ok(new { success = false, error = "No AI model is selected." });

                _cdBase.dr["sFilePath"] = string.IsNullOrEmpty(request.FilePath) ? (object)DBNull.Value : request.FilePath;
                _cdBase.UpdateModel();

                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting AI Analysis file path");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("input-var-change")]
        public IActionResult InputVarChange([FromBody] AiAnalysisInputVarRequest request)
        {
            try
            {
                if (request.Index < 0)
                    return Ok(new { success = false, error = "Invalid variable selection." });

                _sessionState.gInputIndex = request.Index;
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing selected input variable");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // ========== DTO BUILDER ==========

        private AiAnalysisDto BuildDto()
        {
            var dto = new AiAnalysisDto();
            var dr = _cdBase.dr;

            if (dr != null)
            {
                dto.GID = dr.Table.Columns.Contains("ID") && dr["ID"] != DBNull.Value ? dr["ID"].ToString()! : string.Empty;
                dto.GStudyName = ColStr(dr, "sNote");
                dto.GDataFile = ColStr(dr, "sFilePath");
                dto.GSQL = ColStr(dr, "sSQL");
                dto.GGroup = ColStr(dr, "sGroup");
                dto.GProperty = ColStr(dr, "sProperty");
                dto.GSource = ColStr(dr, "sDataSource");
            }

            if (_cnnData.dt != null)
            {
                foreach (DataColumn col in _cnnData.dt.Columns) dto.DataColumns.Add(col.ColumnName);
                foreach (DataRow row in _cnnData.dt.Rows)
                    dto.DataRows.Add(dto.DataColumns.Select(c => row[c]?.ToString() ?? string.Empty).ToList());
            }

            if (_cnnData.dtXCorr != null)
            {
                foreach (DataRow row in _cnnData.dtXCorr.Rows)
                {
                    dto.StatRows.Add(new List<string>
                    {
                        row["Variable"]?.ToString() ?? string.Empty,
                        row["Average"] is double a ? a.ToString("0.000") : string.Empty,
                        row["StdDev"] is double s ? s.ToString("0.000") : string.Empty,
                        row["Correlation"] is double c ? c.ToString("0.000") : string.Empty
                    });
                }
            }

            dto.InputVars = _cnnData.sInputNames?.ToList() ?? new List<string>();
            dto.ChartLeftTitle = _cnnData.sOutputName ?? string.Empty;

            int selIdx = _sessionState.gInputIndex;
            if (selIdx < 0 || selIdx >= dto.InputVars.Count) selIdx = 0;
            dto.InputVarSelectedIndex = selIdx;
            if (dto.InputVars.Count > 0) dto.ChartBottomTitle = dto.InputVars[selIdx];

            if (_cnnData.data != null && _cnnData.nInputNeurons > selIdx && _cnnData.Output != null)
            {
                var nnModel = _cnnData.nnModel;
                var x = _cMatrix.GetColumn2D(_cnnData.data, selIdx);
                int n = _cnnData.Output.Length;
                var xx = new double[n];
                var yy = new double[n];

                double xMin = nnModel.XMin != null && nnModel.XMin.Length > selIdx ? nnModel.XMin[selIdx] : 0;
                double xMax = nnModel.XMax != null && nnModel.XMax.Length > selIdx ? nnModel.XMax[selIdx] : 1;
                double yMin = nnModel.YMin;
                double yMax = nnModel.YMax;

                for (int i = 0; i < n; i++)
                {
                    xx[i] = x[i] * (xMax - xMin) + xMin;
                    yy[i] = _cnnData.Output[i] * (yMax - yMin) + yMin;
                }
                dto.XX = xx;
                dto.YY = yy;
            }

            return dto;
        }

        private static string ColStr(DataRow dr, string col) =>
            dr.Table.Columns.Contains(col) && dr[col] != DBNull.Value ? dr[col].ToString()! : string.Empty;
    }
}
