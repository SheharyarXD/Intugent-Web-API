using Microsoft.AspNetCore.Mvc;
using IntugentClassLibrary.Classes;
using IntugentWebApp.Utilities;
using System.Data;

namespace Intugen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIAnalysisController : ControllerBase
    {
        private readonly ObjectsService _objectsService;
        private readonly CNNModel _nnModel;

        public AIAnalysisController(ObjectsService objectsService)
        {
            _objectsService = objectsService;
            _nnModel = _objectsService.CNNData.GetModelData();
        }

        [HttpGet("data")]
        public IActionResult GetAIAnalysisData()
        {
            try
            {
                var result = new AIAnalysisDataDto
                {
                    StudyName = _objectsService.CDBase.dr["sNote"] == DBNull.Value ? "" : (string)_objectsService.CDBase.dr["sNote"],
                    DataFile = _objectsService.CDBase.dr["sFilePath"] == DBNull.Value ? "" : (string)_objectsService.CDBase.dr["sFilePath"],
                    SQL = _objectsService.CDBase.dr["sSQL"] == DBNull.Value ? "" : (string)_objectsService.CDBase.dr["sSQL"],
                    Group = _objectsService.CDBase.dr["sGroup"] == DBNull.Value ? "" : (string)_objectsService.CDBase.dr["sGroup"],
                    Property = _objectsService.CDBase.dr["sProperty"] == DBNull.Value ? "" : (string)_objectsService.CDBase.dr["sProperty"],
                    Source = _objectsService.CDBase.dr["sDataSource"] == DBNull.Value ? "" : (string)_objectsService.CDBase.dr["sDataSource"],
                    ID = _objectsService.CDBase.dr["ID"] == DBNull.Value ? "" : _objectsService.CDBase.dr["ID"].ToString()
                };

                _objectsService.CNNData.ReadData(_objectsService.CDBase);
                result.DataTable = _objectsService.CNNData.dt;
                result.StatTable = _objectsService.CNNData.dtXCorr;

                if (_objectsService.CNNData.data != null)
                {
                    result.InputVars = _objectsService.CNNData.sInputNames.ToList();
                    result.SelectedInputVar = result.InputVars.Count > 0 ? result.InputVars[0] : "";
                    
                    var x = _objectsService.cMatrix.GetColumn2D(_objectsService.CNNData.data, 0);
                    var dXmin = _nnModel.XMin.Length > 0 ? _nnModel.XMin[0] : 0;
                    var dXdelta = (_nnModel.XMax.Length > 0 ? _nnModel.XMax[0] : 1) - dXmin;
                    if (dXdelta == 0) dXdelta = dXmin;

                    var dYmin = _nnModel.YMin;
                    var dYdelta = _nnModel.YMax - dYmin;
                    if (dYdelta == 0) dYdelta = dYmin;

                    var n = _objectsService.CNNData.Output.Length;
                    result.Xx = new double[n];
                    result.Yy = new double[n];

                    for (int i = 0; i < n; i++)
                    {
                        if (x != null) result.Xx[i] = x[i] * dXdelta + dXmin;
                        result.Yy[i] = _objectsService.CNNData.Output[i] * dYdelta + dYmin;
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("update")]
        public IActionResult UpdateField([FromBody] UpdateFieldRequest request)
        {
            try
            {
                switch (request.Name)
                {
                    case "gStudyName":
                        _objectsService.CDBase.dr["sNote"] = string.IsNullOrEmpty(request.Value) ? DBNull.Value : request.Value;
                        break;
                    case "gDataFile":
                        _objectsService.CDBase.dr["sFilePath"] = string.IsNullOrEmpty(request.Value) ? DBNull.Value : request.Value;
                        break;
                    case "gSQL":
                        _objectsService.CDBase.dr["sSQL"] = string.IsNullOrEmpty(request.Value) ? DBNull.Value : request.Value;
                        break;
                    case "gGroup":
                        _objectsService.CDBase.dr["sGroup"] = string.IsNullOrEmpty(request.Value) ? DBNull.Value : request.Value;
                        break;
                    case "gProperty":
                        _objectsService.CDBase.dr["sProperty"] = string.IsNullOrEmpty(request.Value) ? DBNull.Value : request.Value;
                        break;
                    case "gSource":
                        _objectsService.CDBase.dr["sDataSource"] = string.IsNullOrEmpty(request.Value) ? DBNull.Value : request.Value;
                        break;
                }
                _objectsService.CDBase.UpdateModel();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("upload")]
        public IActionResult UploadData()
        {
            try
            {
                _objectsService.CNNData.ReadData(_objectsService.CDBase);
                _nnModel.nInputNeurons = _objectsService.CNNData.nInputNeurons;
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("browse")]
        public IActionResult BrowseFile([FromBody] string filePath)
        {
            try
            {
                _objectsService.CDBase.dr["sFilePath"] = filePath;
                _objectsService.CDBase.UpdateModel();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("inputvar")]
        public IActionResult ChangeInputVar([FromBody] int index)
        {
            try
            {
                var x = _objectsService.cMatrix.GetColumn2D(_objectsService.CNNData.data, index);
                _objectsService.gInputIndex = index;
                return Ok(new { index = index });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class AIAnalysisDataDto
    {
        public string StudyName { get; set; } = "";
        public string DataFile { get; set; } = "";
        public string SQL { get; set; } = "";
        public string Group { get; set; } = "";
        public string Property { get; set; } = "";
        public string Source { get; set; } = "";
        public string ID { get; set; } = "";
        public DataTable? DataTable { get; set; }
        public DataTable? StatTable { get; set; }
        public List<string> InputVars { get; set; } = new();
        public string SelectedInputVar { get; set; } = "";
        public double[] Xx { get; set; } = Array.Empty<double>();
        public double[] Yy { get; set; } = Array.Empty<double>();
    }

    public class UpdateFieldRequest
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
    }
}