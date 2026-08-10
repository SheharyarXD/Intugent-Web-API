using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Admin
{
    public class AIModel
    {
        public DataTable dtNeurons = new DataTable();
        public DataTable dtWeigts = new DataTable();

        public DataTable gWeights = new DataTable();
        public string gInputNeurons = string.Empty;
        public string gOutputNeurons = string.Empty;
        public int gLayerSelectedIndex;
        public List<string> gLayer = new List<string>();

        public double[] yy = Array.Empty<double>();
        public double[] yyp = Array.Empty<double>();
        public double[] yth = Array.Empty<double>();
        public string gChartBottomTitle = string.Empty;
        public string gChartLeftTitle = string.Empty;
        public string gRMS = string.Empty;

        private readonly CDBase _cdBase;
        private readonly CNNData _cnnData;

        public AIModel(CDBase cdBase, CNNData cnnData)
        {
            _cdBase = cdBase;
            _cnnData = cnnData;

            dtNeurons.Columns.Add("Layer #", typeof(int));
            dtNeurons.Columns.Add("Description", typeof(string));
            dtNeurons.Columns.Add("# of Neurons", typeof(int));

            dtWeigts.Columns.Add("Node Layer i", typeof(string));
            dtWeigts.Columns.Add("#0", typeof(string));
        }

        public CNNModel Load()
        {
            var nnModel = _cnnData.GetModelData();
            nnModel.nInputNeurons = _cnnData.nInputNeurons;
            nnModel.Reset();

            if (nnModel.nHLayers < 1 || nnModel.nNeuronsInLayers == null) return nnModel;

            SetNeurons(nnModel);
            SetgLayer(nnModel);
            SetWeights(nnModel, 1);
            nnModel.Predict(_cnnData);
            SetView(nnModel);

            return nnModel;
        }

        public void SetNeurons(CNNModel nnModel)
        {
            dtNeurons.Clear();
            for (int i = 1; i < nnModel.nNeuronsInLayers.Length - 1; i++)
                dtNeurons.Rows.Add(i, "Hidden Layer", nnModel.nNeuronsInLayers[i]);

            gInputNeurons = nnModel.nNeuronsInLayers[0].ToString();
            nnModel.nNeuronsInLayers[nnModel.nNeuronsInLayers.Length - 1] = 1;
            gOutputNeurons = "1";
        }

        public void SetWeights(CNNModel nnModel, int iLayer)
        {
            const string sForm = "0.00";
            int nCols = 0;
            for (int i = 0; i < nnModel.nNeuronsInLayers.Length; i++)
                if (nCols < nnModel.nNeuronsInLayers[i]) nCols = nnModel.nNeuronsInLayers[i];
            nCols += 2;

            gWeights = new DataTable();
            gWeights.Columns.Add("#", typeof(string));
            gWeights.Columns.Add("Offset", typeof(string));
            for (int i = gWeights.Columns.Count; i < nCols; i++)
                gWeights.Columns.Add("#" + (i - 1), typeof(string));

            if (nnModel.Weights == null) return;
            if (iLayer < 1 || iLayer >= nnModel.nNeuronsInLayers.Length) return;

            for (int iN = 1; iN < nnModel.nNeuronsInLayers[iLayer] + 1; iN++)
            {
                DataRow row = gWeights.NewRow();
                row[0] = "#" + iN;
                for (int iN1 = 0; iN1 < nnModel.nNeuronsInLayers[iLayer - 1] + 1; iN1++)
                    row[iN1 + 1] = nnModel.Weights[iLayer][iN][iN1].ToString(sForm);
                gWeights.Rows.Add(row);
            }
        }

        public void SetgLayer(CNNModel nnModel)
        {
            gLayer.Clear();
            for (int i = 0; i < nnModel.nHLayers; i++) gLayer.Add("#" + (i + 1));
            gLayer.Add("Output");
            gLayerSelectedIndex = 0;
        }

        public void SetView(CNNModel nnModel)
        {
            if (_cnnData.Output == null) return;

            int n = _cnnData.Output.Length;
            yy = new double[n];
            yyp = new double[n];
            yth = new double[2];
            double dmin = nnModel.YMin;
            double dtmp = nnModel.YMax - dmin;
            yth[0] = nnModel.YMin;
            yth[1] = nnModel.YMax;
            if (dtmp == 0) dtmp = dmin;

            for (int i = 0; i < n; i++)
            {
                yy[i] = _cnnData.Output[i] * dtmp + dmin;
                yyp[i] = (_cnnData.OutputPred?[i] ?? 0) * dtmp + dmin;
            }

            gChartBottomTitle = _cnnData.sOutputName + "_Exp.";
            gChartLeftTitle = _cnnData.sOutputName + "_Pred.";
            gRMS = nnModel.ErrorRMSBase > 0
                ? (100.0 * (1.0 - nnModel.ErrorRMS / nnModel.ErrorRMSBase)).ToString("0.00")
                : string.Empty;
        }

        public void SaveModel(CNNModel nnModel)
        {
            if (_cdBase.dr == null) return;
            string sModel = System.Text.Json.JsonSerializer.Serialize(nnModel);
            _cdBase.dr["snnModel"] = sModel;
            _cdBase.UpdateModel();
        }
    }
}
