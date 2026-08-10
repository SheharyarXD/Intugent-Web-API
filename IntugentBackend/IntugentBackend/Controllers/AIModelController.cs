using IntugentBackend.Models;
using IntugentBackend.Services.Admin;
using IntugentBackend.Services.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIModelController : ControllerBase
    {
        private readonly AIModel _svc;
        private readonly CNNData _cnnData;
        private readonly CDBase _cdBase;
        private readonly ILogger<AIModelController> _logger;

        public AIModelController(AIModel svc, CNNData cnnData, CDBase cdBase, ILogger<AIModelController> logger)
        {
            _svc = svc;
            _cnnData = cnnData;
            _cdBase = cdBase;
            _logger = logger;
        }

        [HttpGet("data")]
        public IActionResult GetData()
        {
            try
            {
                var nnModel = _svc.Load();
                return Ok(new { success = true, data = BuildDto(nnModel) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading AI Model view");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("gen-info")]
        public IActionResult GenInfo([FromBody] AiModelFieldRequest request)
        {
            try
            {
                var nnModel = _cnnData.GetModelData();

                switch (request.Name)
                {
                    case "gMaxIter":
                        if (string.IsNullOrEmpty(request.Value)) return Ok(new { success = false, error = "Maximum number of iterations must be greater than zero" });
                        if (int.TryParse(request.Value, out int maxIter)) nnModel.nMaxIter = maxIter;
                        break;
                    case "gConvTol":
                        if (string.IsNullOrEmpty(request.Value)) return Ok(new { success = false, error = "The Convergence Tolerance must be greater than zero" });
                        if (double.TryParse(request.Value, out double convTol)) nnModel.ConvTol = convTol;
                        break;
                    case "gLearnRate":
                        if (string.IsNullOrEmpty(request.Value)) return Ok(new { success = false, error = "Learning rate must be greater than zero" });
                        if (double.TryParse(request.Value, out double learnRate)) nnModel.LearnRate = learnRate;
                        break;
                    case "gStepSizeMin":
                        if (string.IsNullOrEmpty(request.Value)) return Ok(new { success = false, error = "Learning acceleration must be greater than zero" });
                        if (double.TryParse(request.Value, out double stepMin)) nnModel.StepSizeMin = stepMin;
                        break;
                    case "gnHiddenLayers":
                        if (string.IsNullOrEmpty(request.Value)) return Ok(new { success = false, error = "Number of Hidden Layers must be greater than zero" });
                        if (int.TryParse(request.Value, out int nLayers) && nLayers != nnModel.nHLayers)
                        {
                            nnModel.nHLayers = nLayers;
                            nnModel.ResetNeurons(nnModel.nInputNeurons);
                            _svc.SetNeurons(nnModel);
                            nnModel.ResetWeights();
                            _svc.SetWeights(nnModel, 1);
                            _svc.gLayerSelectedIndex = 0;
                        }
                        break;
                    default:
                        return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                _svc.SaveModel(nnModel);
                return Ok(new { success = true, data = BuildDto(nnModel) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating AI Model field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("node-editing")]
        public IActionResult NodeEditing([FromBody] AiModelNodeEditRequest request)
        {
            try
            {
                var nnModel = _cnnData.GetModelData();

                if (request.RowId > nnModel.nHLayers - 1)
                    return Ok(new { success = false, error = "Too many hidden layers" });

                if (string.IsNullOrEmpty(request.Text))
                    return Ok(new { success = false, error = "Number of neurons must be greater than zero" });

                if (int.TryParse(request.Text, out int itmp) && itmp != nnModel.nNeuronsInLayers[request.RowId + 1])
                {
                    nnModel.nNeuronsInLayers[request.RowId + 1] = itmp;
                    nnModel.ResetWeights();
                    _svc.SetWeights(nnModel, _svc.gLayerSelectedIndex + 1);
                }

                nnModel.Predict(_cnnData);
                _svc.SetView(nnModel);
                _svc.SaveModel(nnModel);
                _cnnData.ReadData(_cdBase);

                return Ok(new { success = true, data = BuildDto(nnModel) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing neuron count");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("layer-changed")]
        public IActionResult LayerChanged([FromBody] AiModelLayerRequest request)
        {
            try
            {
                var nnModel = _cnnData.GetModelData();
                _svc.gLayerSelectedIndex = request.Index;
                if (request.Index >= 0) _svc.SetWeights(nnModel, request.Index + 1);
                nnModel.Predict(_cnnData);
                _svc.SetView(nnModel);

                return Ok(new { success = true, data = BuildDto(nnModel) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing selected layer");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("train")]
        public IActionResult Train()
        {
            try
            {
                var nnModel = _cnnData.GetModelData();
                nnModel.TrainModel(_cnnData);
                nnModel.Predict(_cnnData);
                _svc.SetView(nnModel);
                _svc.SaveModel(nnModel);
                _svc.SetWeights(nnModel, _svc.gLayerSelectedIndex + 1);

                return Ok(new { success = true, data = BuildDto(nnModel) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error training AI model");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("hlayer-type-changed")]
        public IActionResult HLayerTypeChanged([FromBody] AiModelFieldRequest request)
        {
            try
            {
                var nnModel = _cnnData.GetModelData();
                nnModel.HLayerType = request.Value ?? "Tanh";
                _svc.SaveModel(nnModel);
                nnModel.Predict(_cnnData);
                _svc.SetView(nnModel);

                return Ok(new { success = true, data = BuildDto(nnModel) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing activation function");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        private AiModelViewDto BuildDto(Services.Data.CNNModel nnModel)
        {
            var dto = new AiModelViewDto
            {
                GMaxIter = nnModel.nMaxIter.ToString(),
                GConvTol = nnModel.ConvTol.ToString("0.000E00"),
                GLearnRate = nnModel.LearnRate.ToString("0.000"),
                GStepSizeMin = nnModel.StepSizeMin.ToString("0.000"),
                GnHiddenLayers = nnModel.nHLayers.ToString(),
                GHLayerType = nnModel.HLayerType,
                GRMS = _svc.gRMS,
                GInputNeurons = _svc.gInputNeurons,
                GOutputNeurons = _svc.gOutputNeurons,
                LayerOptions = _svc.gLayer,
                LayerSelectedIndex = _svc.gLayerSelectedIndex,
                YY = _svc.yy,
                YYP = _svc.yyp,
                YTh = _svc.yth,
                ChartBottomTitle = _svc.gChartBottomTitle,
                ChartLeftTitle = _svc.gChartLeftTitle
            };

            foreach (DataRow row in _svc.dtNeurons.Rows)
            {
                dto.NeuronRows.Add(new NeuronRowDto
                {
                    LayerNo = (int)row["Layer #"],
                    Description = row["Description"].ToString() ?? string.Empty,
                    NeuronCount = (int)row["# of Neurons"]
                });
            }

            foreach (DataColumn col in _svc.gWeights.Columns)
                dto.WeightColumns.Add(col.ColumnName);

            foreach (DataRow row in _svc.gWeights.Rows)
            {
                var cells = new List<string>();
                foreach (DataColumn col in _svc.gWeights.Columns)
                    cells.Add(row[col]?.ToString() ?? string.Empty);
                dto.WeightRows.Add(cells);
            }

            return dto;
        }
    }
}
