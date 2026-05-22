using IntugentBackend.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Data
{
    [Serializable]
    public class CNNModel
    {
        public int nMaxHLayers = 10; public int nMaxNeurons = 10;


        //      int nInputNeurons { get; set; } //Number of Input Variables

        public int nHLayers { get; set; } = 1; //Number of  Layers incluing input layer (not the output)
        public int nMaxIter { get; set; } = 500;
        public int[] nNeuronsInLayers { get; set; } = new int[1];
        public double[] XMin { get; set; } = { 0.0 };
        public double[] XMax { get; set; } = { 1.0 };
        public double[] XAvg { get; set; } = { 0.5 };
        public double YMin { get; set; } = 0.0;
        public double YMax { get; set; } = 1.0;
        public double YAvg { get; set; } = 0.5;
        public double LearnRate { get; set; } = 0.3;//Learning Rate(usually kept const)
        public double StepSizeMin { get; set; } = 1.0E-4; //Learning Acceleration(usually kept const)
        public double ConvTol { get; set; } //Learning Acceleration(usually kept const)
        public string HLayerType { get; set; } = "ReLu";
        public double[][][]? Weights { get; set; } // [nLayers, # of Neurons in layer i, # of Neurons in Layer i-1];
        public double[][]? bias { get; set; }          //bias = new double[nMaxNeurons, nMaxHLayers];
        public int nInputNeurons { get; set; }

        public double ErrorRMS { get; set; }
        public double ErrorRMSBase { get; set; }

        /* not saved variables in json */

        double[,,]? dEdW;
        double[,]? Delta;
        double[,] NeuronValues;

        public CNNModel(CNNData data)
        {

        }
        public CNNModel()
        {
        }
        public bool Reset()
        {
            bool bNew = false;
            int iL, iN, iN1;
            double dtmp;
            if (nInputNeurons < 2) {
                //MessageBox.Show("At least 2 input variables required", Cbfile.sAppName);
                return false; }
            if (nHLayers < 1) nHLayers = 1;
            if (nMaxIter < 1) nMaxIter = 1000;
            if (ConvTol < 1E-20) ConvTol = 0.001;
            if (LearnRate <= 0.0) LearnRate = 0.3;
            if (LearnRate <= 0.0) StepSizeMin = 1.0e-4;


            if (nNeuronsInLayers == null) bNew = true;
            else if (nNeuronsInLayers.Length < nHLayers + 2) bNew = true;
            if (bNew)
            {
                nNeuronsInLayers = new int[nHLayers + 2];
                for (int i = 1; i < nNeuronsInLayers.Length - 1; i++) if (nNeuronsInLayers[i] < 1) nNeuronsInLayers[i] = (int)(Math.Pow(0.9, (double)(i)) * nInputNeurons) + 1;
                nNeuronsInLayers[nNeuronsInLayers.Length - 1] = 1;
            }
            nNeuronsInLayers[0] = nInputNeurons;


            bNew = false;
            if (Weights == null) bNew = true;
            else if (Weights.Length != nHLayers + 2) bNew = true;
            if (bNew) Weights = new double[nHLayers + 2][][];

            for (iL = 1; iL < nHLayers + 1; iL++)
            {
                if (Weights[iL] == null) bNew = true;
                else if (Weights[iL].Length != (nNeuronsInLayers[iL] + 1)) bNew = true;
                if (bNew) Weights[iL] = new double[nNeuronsInLayers[iL] + 1][];

                for (iN = 1; iN < nNeuronsInLayers[iL] + 1; iN++)
                {
                    if (Weights[iL][iN] == null) bNew = true;
                    else if (Weights[iL][iN].Length != (nNeuronsInLayers[iL - 1] + 1)) bNew = true;
                    if (bNew)
                    {
                        Weights[iL][iN] = new double[nNeuronsInLayers[iL - 1] + 1];
                        dtmp = 1.0 / ((double)nNeuronsInLayers[iL - 1] + 1);
                        //                    dtmp = 1.0 ;
                        for (iN1 = 0; iN1 < nNeuronsInLayers[iL - 1] + 1; iN1++) Weights[iL][iN][iN1] = dtmp;
                    }
                }

            }
            iL = nHLayers + 1;
            iN = 1;

            if (Weights[iL] == null) bNew = true;
            else if (Weights[iL].Length != (nNeuronsInLayers[iL] + 1)) bNew = true;
            if (bNew) Weights[iL] = new double[nNeuronsInLayers[iL] + 1][];

            if (Weights[iL][iN] == null) bNew = true;
            else if (Weights[iL][iN].Length != (nNeuronsInLayers[iL - 1] + 1)) bNew = true;

            if (bNew)
            {
                Weights[iL][iN] = new double[nNeuronsInLayers[iL - 1] + 1];
                dtmp = 1.0 / ((double)nNeuronsInLayers[iL - 1] + 1);
                //                    dtmp = 1.0 ;
                for (iN1 = 0; iN1 < nNeuronsInLayers[iL - 1] + 1; iN1++) Weights[iL][iN][iN1] = dtmp;
            }

            return true;

        }


        public bool ResetNeurons(int nInputNeurons)
        {
            if (nInputNeurons < 2) { 
                //MessageBox.Show("Number of input neurons must be greater than zero", Cbfile.sAppName); 
                return false; }
            int[] itmp = nNeuronsInLayers;
            nNeuronsInLayers = new int[nHLayers + 2];
            int n = nNeuronsInLayers.Length;
            nNeuronsInLayers[0] = nInputNeurons;
            if (n > itmp.Length) n = itmp.Length;
            for (int i = 1; i < n; i++) nNeuronsInLayers[i] = itmp[i];
            for (int i = 1; i < nNeuronsInLayers.Length; i++) if (nNeuronsInLayers[i] < 1) nNeuronsInLayers[i] = 1;
            nNeuronsInLayers[nNeuronsInLayers.Length - 1] = 1;   //No offset needed for the output layer

            if (HLayerType != "Tanh" && HLayerType != "ReLu") HLayerType = "Tanh";
            return true;
        }

        public void ResetWeights()
        {
            bool bReset = true; string sMsg;
            double dtmp;
            int iL, iN, iN1;
            /* 
             * iL = layer for which weight functions are being calculated.
             * iN = neuron counter for layer IL
             * iN1 = neuron counter for layer iL - 1
             * */


            if (nHLayers == null || nHLayers < 1)
            {
                sMsg = "Number of Hidden layers must be greater than 0";
                //MessageBox.Show(sMsg, Cbfile.sAppName);
                Weights = null; return;
            }
            if (nNeuronsInLayers == null || nNeuronsInLayers[0] < 1)
            {
                sMsg = "Number of Neurons in the input layer must be greater than 0";
                //MessageBox.Show(sMsg, Cbfile.sAppName);
                Weights = null; return;
            }

            for (iL = 1; iL < nHLayers + 1; iL++)
                if (nNeuronsInLayers == null || nNeuronsInLayers[0] < 1)
                {
                    sMsg = "Number of Neurons in the Hidden Layer " + iL.ToString() + " must be greater than 0";
                    //MessageBox.Show(sMsg, Cbfile.sAppName);
                    Weights = null; return;
                }

            /* Weights = new double[nHLayers + 2][][];
                        for (iL = 1; iL < nHLayers + 1; iL++)
                        {
                            Weights[iL] = new double[nNeuronsInLayers[iL] + 1][];
                            for (iN = 1; iN < nNeuronsInLayers[iL] + 1; iN++)
                            {
                                Weights[iL][iN] = new double[nNeuronsInLayers[iL - 1] + 1];
                                dtmp = 1.0 / ((double)nNeuronsInLayers[iL - 1] + 1);
                                //                    dtmp = 1.0 ;
                                for (iN1 = 0; iN1 < nNeuronsInLayers[iL - 1] + 1; iN1++) Weights[iL][iN][iN1] = dtmp;
                            }

                        }
                        iL = nHLayers + 1;
                        iN = 1;
                        Weights[iL] = new double[nNeuronsInLayers[iL] + 1][];
                        Weights[iL][iN] = new double[nNeuronsInLayers[iL - 1] + 1];
                        dtmp = 1.0 / ((double)nNeuronsInLayers[iL - 1] + 1);
                        for (iN1 = 0; iN1 < nNeuronsInLayers[iL - 1] + 1; iN1++) Weights[iL][iN][iN1] = dtmp;
            */
            var rand = new Random();
            Weights = new double[nHLayers + 2][][];
            for (iL = 1; iL < nHLayers + 2; iL++)
            {
                Weights[iL] = new double[nNeuronsInLayers[iL] + 1][];
                for (iN = 1; iN < nNeuronsInLayers[iL] + 1; iN++)
                {
                    Weights[iL][iN] = new double[nNeuronsInLayers[iL - 1] + 1];


                    dtmp = 1.0 / Math.Sqrt((double)nNeuronsInLayers[iL - 1] + 1);
                    iN1 = 0; Weights[iL][iN][iN1] = 0.0;
                    for (iN1 = 1; iN1 < nNeuronsInLayers[iL - 1] + 1; iN1++) Weights[iL][iN][iN1] = (2.0 * rand.NextDouble() - 1.0) * dtmp;
                }
            }
            return;


        }


        public void Predict(CNNData CNNDatas)
        {

            int i, itmp = 0, id, il = 0, ine, ine1;
            double dtmp = 0;
            double dLeak = 0.1;

            /* Initiate neuron values matrix and assign 1 to the very first neuron in each layer for offset (constant) value */

            //           if (y == null) y = new double[CNNData.nDataPts];
            //           else if (y.Length != CNNData.nDataPts) Array.Resize(ref y, CNNData.nDataPts);

            for (il = 0; il < nNeuronsInLayers.Length; il++) if (itmp < nNeuronsInLayers[il]) itmp = nNeuronsInLayers[il];
            NeuronValues = new double[nNeuronsInLayers.Length, itmp + 1];
            for (il = 0; il < nNeuronsInLayers.Length; il++) NeuronValues[il, 0] = 1.0;

            /* Caluculate output value for each set of input values */
            ErrorRMS = 0.0;
            for (id = 0; id < CNNDatas.nDataPts; id++)
            {
                for (ine = 1; ine < CNNDatas.nInputNeurons + 1; ine++) NeuronValues[0, ine] = CNNDatas.data[id, ine - 1];
                /*
                                for (il = 1; il < nHLayers + 1; il++)
                                {
                                    for (ine = 1; ine < nNeuronsInLayers[il] + 1; ine++)
                                    {
                                        dtmp = Weights[il][ine][0];
                                        for (ine1 = 1; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dtmp += NeuronValues[il - 1, ine1] * Weights[il][ine][ine1];
                                        //                        NeuronValues[il, ine] = 1.0 / (1.0 + Math.Exp(-dtmp));
                                        NeuronValues[il, ine] = Math.Tanh(dtmp);
                                    }
                                }
                */
                if (HLayerType == "ReLu")
                {
                    for (il = 1; il < nHLayers + 1; il++)   //THe output layer is nHLayers + 1
                    {
                        for (ine = 1; ine < nNeuronsInLayers[il] + 1; ine++)            //zeroeth neuron has alway a value of 1 and is not calculated here.
                        {
                            dtmp = Weights[il][ine][0];          //offset is used as zeroeth neuron and always have a value of 1. 
                            for (ine1 = 1; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dtmp += NeuronValues[il - 1, ine1] * Weights[il][ine][ine1];
                            if (dtmp < 0) NeuronValues[il, ine] = dLeak * dtmp; else NeuronValues[il, ine] = dtmp;
                        }
                    }
                }
                else
                {
                    for (il = 1; il < nHLayers + 1; il++)   //THe output layer is nHLayers + 1
                    {
                        for (ine = 1; ine < nNeuronsInLayers[il] + 1; ine++)            //zeroeth neuron has alway a value of 1 and is not calculated here.
                        {
                            dtmp = Weights[il][ine][0];          //offset is used as zeroeth neuron and always have a value of 1. 
                            for (ine1 = 1; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dtmp += NeuronValues[il - 1, ine1] * Weights[il][ine][ine1];
                            //                        NeuronValues[il, ine] = 1.0 / (1.0 + Math.Exp(-dtmp));
                            NeuronValues[il, ine] = Math.Tanh(dtmp);
                        }
                    }
                }

                il = nHLayers + 1;
                ine = 1;
                dtmp = Weights[il][ine][0];
                //                for (ine1 = 1; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dtmp += NeuronValues[il - 1, ine1] * Weights[il][ine][ine1];
                //                for (ine1 = 1; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dtmp += Weights[il][ine][ine1] * Math.Pow(NeuronValues[il - 1, ine1], (double)ine1);  //Power series of neuron values
                for (ine1 = 1; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dtmp += Weights[il][ine][ine1] * NeuronValues[il - 1, ine1];  //Power series of neuron values
                CNNDatas.OutputPred[id] = dtmp;
                ErrorRMS += (CNNDatas.Output[id] - dtmp) * (CNNDatas.Output[id] - dtmp);

            }
            ErrorRMS = ErrorRMS / (double)CNNDatas.nDataPts;

        }
        public void TrainModel(CNNData CNNDatas)
        {
            int i, iter = 0, itmp = 0, id, il = 0, ine, ine1, ine1f;  //ine1 node # for nodes 1 layer behing.  Ine1f is node # for nodes one layer forward
            double dtmp = 0; double RMSError1 = 100;
            double dedwRMS, wtRMS;
            double ErrorRMS2 = double.MaxValue, Epsilon = LearnRate;
            double OutputAvg;
            double dLeak = 0.1;

            /* Get the ErrorRMSBase (from average) */
            dtmp = 0.0;
            for (i = 0; i < CNNDatas.nDataPts; i++) dtmp += CNNDatas.Output[i];
            OutputAvg = dtmp / CNNDatas.nDataPts;
            dtmp = 0.0;
            ErrorRMSBase = 0.0;
            for (i = 0; i < CNNDatas.nDataPts; i++) { dtmp = (CNNDatas.Output[i] - OutputAvg); ErrorRMSBase += dtmp * dtmp; }
            ErrorRMSBase = ErrorRMSBase / (double)CNNDatas.nDataPts;

            /* Initiate neuron values matrix and assign 1 to the very first neuron in each layer for offset (constant) value */

            for (il = 0; il < nNeuronsInLayers.Length; il++) if (itmp < nNeuronsInLayers[il]) itmp = nNeuronsInLayers[il];
            itmp += 1;
            double[,,] dEdW = new double[nHLayers + 2, itmp, itmp];
            double[,,] Weights2 = new double[nHLayers + 2, itmp, itmp];
            NeuronValues = new double[nNeuronsInLayers.Length, itmp + 1];

            string sData = "Iter#, Error1, Error2, Epsilon";

            for (il = 0; il < nNeuronsInLayers.Length; il++) NeuronValues[il, 0] = 1.0;


            /* Initiate derivaties of weights and delta (for each neuron.  No need to save values for each datapoint */

            dEdW = new double[Weights.Length, itmp, itmp];
            Weights2 = new double[Weights.Length, itmp, itmp];


            Delta = new double[nHLayers + 2, itmp];

            /* Forward propagation to calculate the neuron values for each data point */

            //           nMaxIter = 1000;
            Epsilon = LearnRate;
            iter = 1;

            //Divide data into batches
            int nBatchSize = nNeuronsInLayers[0] * nNeuronsInLayers[1];
            for (il = 2; il < nNeuronsInLayers.Length; ++il) nBatchSize += nNeuronsInLayers[il - 1] * nNeuronsInLayers[il];
            int nBatches = CNNDatas.nDataPts / (3 * nBatchSize);
            int[] iBatch = new int[CNNDatas.nDataPts]; int[] iBatchSize = new int[nBatches];
            var rand = new Random();
            for (id = 0; id < CNNDatas.nDataPts; ++id) iBatch[id] = rand.Next(0, nBatches);
            for (int ib = 0; ib < nBatches; ++ib) iBatchSize[ib] = 0;
            for (int ib = 0; ib < nBatches; ++ib)
                for (id = 0; id < CNNDatas.nDataPts; id++) if (iBatch[id] == ib) iBatchSize[ib] += 1;

            int IB = 0;
            do
            {
                ErrorRMS = 0; ///* Initialize the error and error derivative */
                for (id = 0; id < CNNDatas.nDataPts; id++)
                {
                    //                  if (iBatch[id] != IB) continue;
                    for (ine = 1; ine < CNNDatas.nInputNeurons + 1; ine++) NeuronValues[0, ine] = CNNDatas.data[id, ine - 1];  //Set zeroth layer to the input values

                    if (HLayerType == "ReLu")
                    {
                        for (il = 1; il < nHLayers + 1; il++)   //THe output layer is nHLayers + 1
                        {
                            for (ine = 1; ine < nNeuronsInLayers[il] + 1; ine++)            //zeroeth neuron has alway a value of 1 and is not calculated here.
                            {
                                dtmp = Weights[il][ine][0];          //offset is used as zeroeth neuron and always have a value of 1. 
                                for (ine1 = 1; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dtmp += NeuronValues[il - 1, ine1] * Weights[il][ine][ine1];
                                if (dtmp < 0) NeuronValues[il, ine] = dLeak * dtmp; else NeuronValues[il, ine] = dtmp;
                            }
                        }
                    }
                    else
                    {
                        for (il = 1; il < nHLayers + 1; il++)   //THe output layer is nHLayers + 1
                        {
                            for (ine = 1; ine < nNeuronsInLayers[il] + 1; ine++)            //zeroeth neuron has alway a value of 1 and is not calculated here.
                            {
                                dtmp = Weights[il][ine][0];          //offset is used as zeroeth neuron and always have a value of 1. 
                                for (ine1 = 1; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dtmp += NeuronValues[il - 1, ine1] * Weights[il][ine][ine1];
                                //                        NeuronValues[il, ine] = 1.0 / (1.0 + Math.Exp(-dtmp));
                                NeuronValues[il, ine] = Math.Tanh(dtmp);
                            }
                        }
                    }


                    il = nHLayers + 1;  //The output layer with only one neuron.
                    ine = 1;
                    dtmp = Weights[il][ine][0]; //offset is used as zeroeth neuron and always have a value of 1. 
                                                //                 for (ine1 = 1; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dtmp += Weights[il][ine][ine1] * Math.Pow(NeuronValues[il - 1, ine1], (double)ine1);  //Power series of neuron values
                    for (ine1 = 1; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dtmp += Weights[il][ine][ine1] * NeuronValues[il - 1, ine1];  //Linear Combination                                                                                                                          
                    NeuronValues[il, ine] = CNNDatas.OutputPred[id] = dtmp;

                    /* Backpropagation to calculate derivatives */

                    Delta[il, ine] = (dtmp - CNNDatas.Output[id]);
                    ErrorRMS += Delta[il, ine] * Delta[il, ine];//((CNNData.Output[id] + 0.2)* (CNNData.Output[id] + 0.2));


                    //                    derivative of wts in the output layer

                    dEdW[il, ine, 0] += Delta[il, ine];
                    //                  for (ine1 = 1; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dEdW[il, ine, ine1] += Delta[il, ine] * NeuronValues[il - 1, ine1];   // Linear Combination
                    //                   for (ine1 = 1; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dEdW[il, ine, ine1] += Delta[il, ine] * Math.Pow(NeuronValues[il - 1, ine1], (double)ine1);   //Power series of neuron values

                    for (ine1 = 1; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dEdW[il, ine, ine1] += Delta[il, ine] * NeuronValues[il - 1, ine1]; //Linear Comb

                    if (HLayerType == "ReLu")     //Hidden Layers
                    {
                        for (il = nHLayers; il > 0; il--)
                        {
                            for (ine = 1; ine < nNeuronsInLayers[il] + 1; ine++)
                            {
                                dtmp = 0;
                                //                           dtmp = 1;
                                for (ine1f = 1; ine1f < nNeuronsInLayers[il + 1] + 1; ine1f++) dtmp += Delta[il + 1, ine1f] * Weights[il + 1][ine1f][ine];  //offset has no value of delta. 
                                if (NeuronValues[il, ine] < 0) Delta[il, ine] = dtmp * dLeak; else Delta[il, ine] = dtmp;
                                for (ine1 = 0; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dEdW[il, ine, ine1] += Delta[il, ine] * NeuronValues[il - 1, ine1];
                            }
                        }
                    }
                    else
                    {
                        for (il = nHLayers; il > 0; il--)
                        {
                            for (ine = 1; ine < nNeuronsInLayers[il] + 1; ine++)
                            {
                                dtmp = 0;
                                //                           dtmp = 1;
                                for (ine1f = 1; ine1f < nNeuronsInLayers[il + 1] + 1; ine1f++) dtmp += Delta[il + 1, ine1f] * Weights[il + 1][ine1f][ine];  //offset has no value of delta.

                                Delta[il, ine] = dtmp * (1 - NeuronValues[il, ine] * NeuronValues[il, ine]);
                                for (ine1 = 0; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) dEdW[il, ine, ine1] += Delta[il, ine] * NeuronValues[il - 1, ine1];
                            }
                        }
                    }


                }

                dtmp = (double)CNNDatas.nDataPts;
                //                dtmp = (double)iBatchSize[IB];
                ErrorRMS = ErrorRMS / dtmp;                 /*Adjust errors to # of points */

                if (iter > 1 && ErrorRMS > ErrorRMS2) //if error is greater than the error in last step, reset the weights and reduce epsilon
                {
                    Epsilon = Epsilon * 0.5;
                    for (il = 1; il < nHLayers + 2; il++)
                        for (ine = 1; ine < nNeuronsInLayers[il] + 1; ine++)
                            for (ine1 = 0; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) Weights[il][ine][ine1] = Weights2[il, ine, ine1];
                }
                else
                {
                    for (il = 1; il < nHLayers + 2; il++)
                        for (ine = 1; ine < nNeuronsInLayers[il] + 1; ine++)
                            for (ine1 = 0; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) Weights2[il, ine, ine1] = Weights[il][ine][ine1];

                    dedwRMS = 0.0;
                    for (il = 1; il < nHLayers + 2; il++)
                        for (ine = 1; ine < nNeuronsInLayers[il] + 1; ine++)
                            for (ine1 = 0; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++)
                            {
                                dEdW[il, ine, ine1] = dEdW[il, ine, ine1] / dtmp;
                                dedwRMS += dEdW[il, ine, ine1] * dEdW[il, ine, ine1];
                            }

                    dedwRMS = Math.Sqrt(dedwRMS);

                    for (il = 1; il < nHLayers + 2; il++)
                        for (ine = 1; ine < nNeuronsInLayers[il] + 1; ine++)
                            for (ine1 = 0; ine1 < nNeuronsInLayers[il - 1] + 1; ine1++) Weights[il][ine][ine1] = Weights[il][ine][ine1] - Epsilon * dEdW[il, ine, ine1] / dedwRMS;
                    iter += 1;
                    //                    Epsilon = Epsilon * 1.2;

                }
                if (ErrorRMS2 < 0.8 * ErrorRMS) Epsilon = Epsilon * 1.1;
                //               IB += 1; if (IB >= nBatches) IB = 0;

                sData += "\n" + iter.ToString() + "," + ErrorRMS.ToString("0.000E00") + "," + ErrorRMS2.ToString("0.000E00") + "," + Epsilon.ToString("0.000E00");
                ErrorRMS2 = ErrorRMS;


            } while ((iter < nMaxIter) && (ErrorRMS > ConvTol) && (Epsilon > 1.0E-6));
            //Clipboard.SetText(sData);

        }

    }


}
