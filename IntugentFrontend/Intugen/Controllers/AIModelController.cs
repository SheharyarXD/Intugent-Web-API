using Microsoft.AspNetCore.Mvc;
using IntugentClassLibrary.Classes;
using IntugentWebApp.Utilities;
using System.Data;

namespace Intugen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIModelController : ControllerBase
    {
        private readonly ObjectsService _objectsService;
        private CNNModel _nnModel;

        public AIModelController(ObjectsService objectsService)
        {
            _objectsService = objectsService;
            _nnModel = _objectsService.CNNData.GetModelData();
        }

        [HttpGet("data")]
        public IActionResult GetAIModelData()
        {
            try
            {
                _nnModel = _objectsService.CNNData.GetModelData();
                _nnModel.nInputNeurons = _objectsService.CNNData.nInputNeurons;
                _nnModel.Reset();

                var result = new AIModelDataDto
                {
                    MaxIter = _nnModel.nMaxIter.ToString(),
                    ConvTol = _nnModel.ConvTol.ToString("0.000E00"),
                    LearnRate = _nnModel.LearnRate.ToString("0.000"),
                    StepSizeMin = _nnModel.StepSizeMin.ToString("0.000"),
                    HiddenLayers = _nnModel.nHLayers.ToString(),
                    LayerType = _nnModel.HLayerType.ToString(),
                    InputNeurons = _nnModel.nInputNeurons.ToString(),
                    OutputNeurons = "1"
                };

                SetNeuronsTable(result);
                SetLayersList(result);
                
                result.LayerSelectedIndex = 0;
                SetWeightsTable(result, 1);

                if (_nnModel != null)
                {
                    _nnModel.Predict(_objectsService.CNNData);
                }

                SetViewModelData(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("update")]
        public IActionResult UpdateGenInfo([FromBody] GenInfoUpdateRequest request)
        {
            try
            {
                _nnModel = _objectsService.CNNData.GetModelData();

                switch (request.Name)
                {
                    case "gMaxIter":
                        if (int.TryParse(request.Value, out int itmp)) _nnModel.nMaxIter = itmp;
                        break;
                    case "gConvTol":
                        if (double.TryParse(request.Value, out double dtmp1)) _nnModel.ConvTol = dtmp1;
                        break;
                    case "gLearnRate":
                        if (double.TryParse(request.Value, out double dtmp2)) _nnModel.LearnRate = dtmp2;
                        break;
                    case "gStepSizeMin":
                        if (double.TryParse(request.Value, out double dtmp3)) _nnModel.StepSizeMin = dtmp3;
                        break;
                    case "gnHiddenLayers":
                        if (int.TryParse(request.Value, out int itmp2) && itmp2 != _nnModel.nHLayers)
                        {
                            _nnModel.nHLayers = itmp2;
                            _nnModel.ResetNeurons(_nnModel.nInputNeurons);
                            _nnModel.ResetWeights();
                        }
                        break;
                }

                SaveModel();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("train")]
        public IActionResult TrainModel()
        {
            try
            {
                _nnModel = _objectsService.CNNData.GetModelData();
                _nnModel.TrainModel(_objectsService.CNNData);
                _nnModel.Predict(_objectsService.CNNData);
                SaveModel();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("neurons")]
        public IActionResult UpdateNeurons([FromBody] NeuronsUpdateRequest request)
        {
            try
            {
                _nnModel = _objectsService.CNNData.GetModelData();

                if (request.RowId < _nnModel.nHLayers)
                {
                    if (request.ColId == 2)
                    {
                        if (int.TryParse(request.Text, out int itmp) && itmp != _nnModel.nNeuronsInLayers[request.RowId + 1])
                        {
                            _nnModel.nNeuronsInLayers[request.RowId + 1] = itmp;
                            _nnModel.ResetWeights();
                        }
                    }
                }

                _nnModel.Predict(_objectsService.CNNData);
                SaveModel();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("layer")]
        public IActionResult ChangeLayer([FromBody] int layerIndex)
        {
            try
            {
                _nnModel = _objectsService.CNNData.GetModelData();
                var result = new List<Dictionary<string, object>>();

                var weightsTable = new DataTable();
                int nCols = 0;

                for (int i = 0; i < _nnModel.nNeuronsInLayers.Length; i++)
                    if (nCols < _nnModel.nNeuronsInLayers[i]) nCols = _nnModel.nNeuronsInLayers[i];

                nCols += 2;

                weightsTable.Columns.Add("#", typeof(string));
                weightsTable.Columns.Add("Offset", typeof(string));
                for (int i = weightsTable.Columns.Count; i < nCols; i++)
                {
                    weightsTable.Columns.Add("#" + (i - 1).ToString(), typeof(string));
                }

                if (_nnModel.Weights != null)
                {
                    for (int iN = 1; iN < _nnModel.nNeuronsInLayers[layerIndex + 1] + 1; iN++)
                    {
                        DataRow row = weightsTable.NewRow();
                        row[0] = "#" + iN.ToString();
                        for (int iN1 = 0; iN1 < _nnModel.nNeuronsInLayers[layerIndex] + 1; iN1++)
                        {
                            row[iN1 + 1] = _nnModel.Weights[layerIndex + 1][iN][iN1].ToString("0.00");
                        }
                        weightsTable.Rows.Add(row);
                    }
                }

                foreach (DataRow row in weightsTable.Rows)
                {
                    var rowDict = new Dictionary<string, object>();
                    foreach (DataColumn col in weightsTable.Columns)
                    {
                        rowDict[col.ColumnName] = row[col];
                    }
                    result.Add(rowDict);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("layertype")]
        public IActionResult ChangeLayerType([FromBody] string layerType)
        {
            try
            {
                _nnModel = _objectsService.CNNData.GetModelData();
                _nnModel.HLayerType = layerType;
                SaveModel();
                _nnModel.Predict(_objectsService.CNNData);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private void SaveModel()
        {
            string sModel = System.Text.Json.JsonSerializer.Serialize(_nnModel);
            _objectsService.CDBase.dr["snnModel"] = sModel;
            _objectsService.CDBase.UpdateModel();
        }

        private void SetNeuronsTable(AIModelDataDto result)
        {
            var dtNeurons = new DataTable();
            dtNeurons.Columns.Add("Layer #", typeof(int));
            dtNeurons.Columns.Add("Description", typeof(string));
            dtNeurons.Columns.Add("# of Neurons", typeof(int));

            for (int i = 1; i < _nnModel.nNeuronsInLayers.Length - 1; i++)
            {
                dtNeurons.Rows.Add(i, "Hidden Layer", _nnModel.nNeuronsInLayers[i]);
            }

            result.NeuronsTable = dtNeurons.DefaultView;
        }

        private void SetLayersList(AIModelDataDto result)
        {
            result.Layers = new List<string>();
            for (int i = 0; i < _nnModel.nHLayers; i++)
                result.Layers.Add("#" + (i + 1));
            result.Layers.Add("Output");
        }

        private void SetWeightsTable(AIModelDataDto result, int layerIndex)
        {
            var weightsTable = new DataTable();
            int nCols = 0;

            for (int i = 0; i < _nnModel.nNeuronsInLayers.Length; i++)
                if (nCols < _nnModel.nNeuronsInLayers[i]) nCols = _nnModel.nNeuronsInLayers[i];

            nCols += 2;

            weightsTable.Columns.Add("#", typeof(string));
            weightsTable.Columns.Add("Offset", typeof(string));
            for (int i = weightsTable.Columns.Count; i < nCols; i++)
            {
                weightsTable.Columns.Add("#" + (i - 1).ToString(), typeof(string));
            }

            if (_nnModel.Weights != null)
            {
                for (int iN = 1; iN < _nnModel.nNeuronsInLayers[layerIndex] + 1; iN++)
                {
                    DataRow row = weightsTable.NewRow();
                    row[0] = "#" + iN.ToString();
                    for (int iN1 = 0; iN1 < _nnModel.nNeuronsInLayers[layerIndex - 1] + 1; iN1++)
                    {
                        row[iN1 + 1] = _nnModel.Weights[layerIndex][iN][iN1].ToString("0.00");
                    }
                    weightsTable.Rows.Add(row);
                }
            }

            result.WeightsTable = weightsTable;
        }

        private void SetViewModelData(AIModelDataDto result)
        {
            int n = _objectsService.CNNData.Output.Length;
            result.Yy = new double[n];
            result.Yyp = new double[n];
            result.Yth = new double[2];

            double dmin = _nnModel.YMin;
            double dtmp = _nnModel.YMax - dmin;
            result.Yth[0] = _nnModel.YMin;
            result.Yth[1] = _nnModel.YMax;

            if (dtmp == 0) dtmp = dmin;

            for (int i = 0; i < n; i++)
            {
                result.Yy[i] = _objectsService.CNNData.Output[i] * dtmp + dmin;
                result.Yyp[i] = _objectsService.CNNData.OutputPred[i] * dtmp + dmin;
            }

            result.ChartBottomTitle = _objectsService.CNNData.sOutputName + "_Exp.";
            result.ChartLeftTitle = _objectsService.CNNData.sOutputName + "_Pred.";
            result.RMS = _nnModel.ErrorRMSBase > 0 ? (100.0 * (1.0 - _nnModel.ErrorRMS / _nnModel.ErrorRMSBase)).ToString("0.00") : string.Empty;
        }
    }

    public class AIModelDataDto
    {
        public string MaxIter { get; set; } = "";
        public string ConvTol { get; set; } = "";
        public string LearnRate { get; set; } = "";
        public string StepSizeMin { get; set; } = "";
        public string HiddenLayers { get; set; } = "";
        public string LayerType { get; set; } = "";
        public string InputNeurons { get; set; } = "";
        public string OutputNeurons { get; set; } = "";
        public string RMS { get; set; } = "";
        public List<string> Layers { get; set; } = new();
        public int LayerSelectedIndex { get; set; }
        public DataView? NeuronsTable { get; set; }
        public DataTable? WeightsTable { get; set; }
        public double[] Yy { get; set; } = Array.Empty<double>();
        public double[] Yyp { get; set; } = Array.Empty<double>();
        public double[] Yth { get; set; } = Array.Empty<double>();
        public string ChartBottomTitle { get; set; } = "";
        public string ChartLeftTitle { get; set; } = "";
    }

    public class GenInfoUpdateRequest
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
    }

    public class NeuronsUpdateRequest
    {
        public int RowId { get; set; }
        public int ColId { get; set; }
        public string Text { get; set; } = "";
    }
}