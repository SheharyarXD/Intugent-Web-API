using Google.Api.Gax;
using IntugentBackend.Services.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Data
{
    public class CNNData                        // this data is not saved
    {
         int nMaxNeurons = 10, nMaxDataPts = 5000;
        public int nNeurons, nDataPts;                            // No. of Neurons in the input layer and # of data points
        public double[,]? data;
        public double[]? Output;
        public double[]? OutputPred;
        public string[]? sInputNames;
        public string? sOutputName;
        public double OutputAvg, OutputSD;
        public double[]? InputAvg;
        public double[]? InputSD;
        public double[]? InOutCorr;
        public int nInputNeurons;
        public DataTable? dt;
        public DataTable? dtXCorr;
        public Cbfile Cbfile;
        public CDBase CDBase;
        public CNNModel nnModel;

        public CNNData(Cbfile cbfile)
        {
            Cbfile = cbfile;
            nnModel=new CNNModel();
        }
        public void Reset()
        {
            nMaxNeurons = 1; nMaxDataPts = 5000;
            nNeurons = nDataPts = 0;                            // No. of Neurons in the input layer and # of data points
            data = new double[1, 1];
            Output = new double[1];
            OutputPred = new double[1];
            sInputNames = new string[1];
            sOutputName = string.Empty;
            OutputAvg = OutputSD = 0.0;
            InputAvg = new double[1];
            InputSD = new double[1];
            InOutCorr = new double[1];
            nInputNeurons = 0;
            dt = new DataTable();
            dtXCorr = new DataTable();
        }
        public bool ReadData(CDBase CDBase)
        {
            string sMsg, sFile = string.Empty;
            int nDataPt, ntmp;
            string[] strParts;


            Reset();

            if (CDBase.dr["sDataSource"] == DBNull.Value)
            {
                //MessageBox.Show("You must choose a value for Data Source", Cbfile.sAppName);
                return false; }
            else if ((string)CDBase.dr["sDataSource"] == "SQL")
            {
                {
                    //MessageBox.Show("SQL query data source is not implemented yet", Cbfile.sAppName);
                    return false; }

            }
            else
            {
                if (CDBase.dr["sFilePath"] == DBNull.Value)
                {
                    //MessageBox.Show("File name is missing", Cbfile.sAppName);
                    return false; }
                else sFile = (string)CDBase.dr["sFilePath"];
                if (!File.Exists(sFile))
                {
                    //MessageBox.Show("Could not find the data file " + sFile, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            try
            {

                string[] strLines = File.ReadAllLines(sFile);
                /*
                                strParts = strLines[0].Split('\t'); CnnModel.nLayers = int.Parse(strParts[1]);
                                strParts = strLines[2].Split('\t'); CnnModel.nMaxIter = int.Parse(strParts[1]);
                                strParts = strLines[3].Split('\t'); CnnModel.LearnRate = double.Parse(strParts[1]);
                                strParts = strLines[4].Split('\t'); CnnModel.LearnAccel = double.Parse(strParts[1]);
                                strParts = strLines[5].Split('\t'); CnnModel.ConvTol = double.Parse(strParts[1]);

                                CnnModel.nNeuronsInLayers = new int[CnnModel.nLayers];
                                strParts = strLines[1].Split('\t'); for (int i = 0; i < CnnModel.nLayers; i++) CnnModel.nNeuronsInLayers[i] = int.Parse(strParts[i + 1]);
                */

                strParts = strLines[0].Split('\t', ','); nInputNeurons = strParts.Length - 1; // nInputNeurons = int.Parse(strParts[1]);
                sInputNames = new string[nInputNeurons];
                //               strParts = strLines[1].Split('\t');
                for (int i = 0; i < nInputNeurons; i++) sInputNames[i] = strParts[i];
                sOutputName = strParts[nInputNeurons];

                nDataPts = strLines.Length - 1;
                data = new double[nDataPts, nInputNeurons];
                Output = new double[nDataPts];
                OutputPred = new double[nDataPts];


                for (int ir = 0; ir < nDataPts; ir++)
                {
                    strParts = strLines[ir + 1].Split('\t', ',');
                    for (int i = 0; i < nInputNeurons; i++) data[ir, i] = double.Parse(strParts[i]);
                    Output[ir] = double.Parse(strParts[nInputNeurons]);

                }


            }

            catch (Exception ex)
            {
               // Mouse.OverrideCursor = null;
                sMsg = "Errors in reading file \n" + ex.ToString() + "\b\n Accept the Data?";
                //if (MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)

                  //  Mouse.OverrideCursor = null;
                return false;
            }

            Correlation();  //Correllations calculated before scaling the data.
           nnModel.nInputNeurons = nInputNeurons;

            /* Make the datatable to display data and correllation */


            if (dt != null) dt.Columns.Clear();
            dt.Columns.Add("#", typeof(string)); dt.Columns.Add(sOutputName, typeof(int));
            for (int i = 0; i < nInputNeurons; i++) dt.Columns.Add(sInputNames[i], typeof(double));

            for (int i = 0; i < nDataPts; i++)
            {
                dt.Rows.Add();
                dt.Rows[i][0] = i.ToString();
                dt.Rows[i][1] = Output[i];
                for (int j = 0; j < nInputNeurons; j++) dt.Rows[i][j + 2] = data[i, j];
            }

            if (dtXCorr == null) dtXCorr = new DataTable(); else dtXCorr.Columns.Clear();
            dtXCorr.Columns.Add("Variable", typeof(string)); dtXCorr.Columns.Add("Average", typeof(double));
            dtXCorr.Columns.Add("StdDev", typeof(double)); dtXCorr.Columns.Add("Correlation", typeof(double));

            dtXCorr.Rows.Add(); dtXCorr.Rows[0]["Variable"] =sOutputName; dtXCorr.Rows[0]["Average"] =OutputAvg;
            dtXCorr.Rows[0]["StdDev"] = OutputSD; dtXCorr.Rows[0]["Correlation"] = 1.0;

            for (int i = 0; i <nInputNeurons; i++)
            {
                dtXCorr.Rows.Add(); dtXCorr.Rows[i + 1]["Variable"] = sInputNames[i]; dtXCorr.Rows[i + 1]["Average"] = InputAvg[i];
                dtXCorr.Rows[i + 1]["StdDev"] = InputSD[i]; dtXCorr.Rows[i + 1]["Correlation"] = InOutCorr[i];
            }
            /* Scale the data */

            double dmin = double.MaxValue; double dmax = double.MinValue; double dtmp;
            for (int i = 0; i < nDataPts; i++)
            {
                if (Output[i] < dmin) dmin = Output[i];
                if (Output[i] > dmax) dmax = Output[i];
            }
            if (dmax == dmin) dtmp = 1; else dtmp = 1.0 / (dmax - dmin);
            for (int i = 0; i < nDataPts; i++) Output[i] = (Output[i] - dmin) * dtmp;
           nnModel.YMin = dmin; nnModel.YMax = dmax;


             nnModel.XMin = new double[nInputNeurons];
             nnModel.XMax = new double[nInputNeurons];
            for (int j = 0; j < nInputNeurons; j++)
            {
                dmin = double.MaxValue; dmax = double.MinValue; dtmp = 1;
                for (int i = 0; i < nDataPts; i++)
                {
                    if (data[i, j] < dmin) dmin = data[i, j];
                    if (data[i, j] > dmax) dmax = data[i, j];
                }
                if (dmax == dmin) dtmp = 1; else dtmp = 1.0 / (dmax - dmin);
                for (int i = 0; i < nDataPts; i++) data[i, j] = (data[i, j] - dmin) * dtmp;
               nnModel.XMin[j] = dmin;
               nnModel.XMax[j] = dmax;
            }



          // Mouse.OverrideCursor = null;

            return true;
        }
        public CNNModel GetModelData()
        {
            return nnModel;
        }

        public void Correlation()
        {
            double sum, dtmp;

            InputAvg = new double[nInputNeurons];
            InputSD = new double[nInputNeurons];
            InOutCorr = new double[nInputNeurons];


            sum = 0;
            for (int ir = 0; ir < nDataPts; ir++) sum += Output[ir];
            OutputAvg = sum / (double)nDataPts;

            for (int ic = 0; ic < nInputNeurons; ic++)
            {
                sum = 0;
                for (int ir = 0; ir < nDataPts; ir++) sum += data[ir, ic];
                InputAvg[ic] = sum / (double)nDataPts;
            }


            sum = 0;
            for (int ir = 0; ir < nDataPts; ir++) { dtmp = Output[ir] - OutputAvg; sum += dtmp * dtmp; }
            OutputSD = Math.Sqrt(sum / (double)nDataPts);

            for (int ic = 0; ic < nInputNeurons; ic++)
            {
                sum = 0;
                for (int ir = 0; ir < nDataPts; ir++) { dtmp = data[ir, ic] - InputAvg[ic]; sum += dtmp * dtmp; }
                InputSD[ic] = Math.Sqrt(sum / (double)nDataPts);
            }

            for (int ic = 0; ic < nInputNeurons; ic++)
            {
                sum = 0;
                for (int ir = 0; ir < nDataPts; ir++) { sum += (data[ir, ic] - InputAvg[ic]) * (Output[ir] - OutputAvg); }
                InOutCorr[ic] = sum / ((double)nDataPts * InputSD[ic] * OutputSD);

            }
        }
    }

}
