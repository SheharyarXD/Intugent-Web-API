using Google.Api.Gax;
//using IntugentClassLibrary.Pages.Rnd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Data
{
    public class CRCalc
    {
        public int nBlowAg, IAirBAIndex; //# of blowing agents, and position of air ba in the array.

        public ObservableCollection<CMaterial> GasMats = new ObservableCollection<CMaterial>();

        //        public CMaterial[] GasMats = new CMaterial[Params.nComps];
        //        public double[,] RValueForms = new double[Params.nFormMax, Params.nComps];
        public double[,] MoleFracs = new double[Params.nFormMax, Params.nComps];   //Original
        public double[,] RValues = new double[Params.nFormMax, Params.nDataPts];
        public double[,] KValues = new double[Params.nFormMax, Params.nDataPts];
        public double[] TempK = new double[Params.nDataPts];
        public double[] TempC = new double[Params.nDataPts];
        public double[,] Lambda = new double[Params.nComps, Params.nDataPts];  // At various temperatures
                                                                               //        public double[,] Viscosity = new double[Params.nComps, Params.nDataPts]; // At various temperatures
        public double[,] VapPres = new double[Params.nComps, Params.nDataPts]; // At various temperatures

        public double dTempKTT = 298;
        public double dCellSizeTT = 300;  // microns
        public double dCellPressTT = 0.8;
        public double dPolyDenTT = 1200; //Kg/m3
        public double dPolyCondTT = 0.225; //w/m-K
        public double dFracStrut = 0.8;  //Fraction of polymer in struts
        public double[,] AijTT = new double[Params.nComps, Params.nComps]; // At test temperature. Used for all temps and concs
        public double[,] MoleFracsTT = new double[Params.nFormMax, Params.nComps];  // At test temperature
        public double[] LambdaTT = new double[Params.nComps];  // At test temperature
        public double[] ViscosityTT = new double[Params.nComps]; // At test temperature
        public double[] VapPresTT = new double[Params.nFormMax];  //R value at test temperature
        public double[] KOutTT = new double[Params.nFormMax];  //KR value array for temporary storing the values
        public double[,] MoleFracsAdjTT = new double[Params.nFormMax, Params.nComps];
        public double[] FoamDensityTT = new double[Params.nFormMax];  // At test temperature


        public double dTempKBase = 298;
        public double[] KValuesBase = new double[Params.nFormMax];  //K value at test temperature
        public double[] RValuesBase = new double[Params.nFormMax];  //R value at test temperature
        public double[] LambdaBase = new double[Params.nComps];  // At test temperature
        public double[] VapPresBase = new double[Params.nComps]; // At test temperature
        public double[,] MoleFracsAdjBase = new double[Params.nFormMax, Params.nComps];   //Original
        public double[] FoamDensityBase = new double[Params.nFormMax];  // At test temperature

        public string sXAxisTitle, sYAxisTitle, sAgedYAxis2Title;


 
        public void CalBlowGasesProps(double dTempC)
        {
            double dTerm1, dTerm2, dTerm3, dTempK;
            int iba, jba, idpt;

            dTempK = dTempC + 273.0;

            #region //Calculate the temperature range. Calculate the gaseous phase properties of all the blowing agents at all the temperatues

            for (idpt = 0; idpt < Params.nDataPts; idpt++)
            { TempK[idpt] = dTempK - 50.0 + idpt * 2.0; TempC[idpt] = TempK[idpt] - 273.0; }

            for (iba = 0; iba < nBlowAg; iba++)
            {
                for (idpt = 0; idpt < Params.nDataPts; idpt++)
                {
                    Lambda[iba, idpt] = GasMats[iba].GasCond_A * Math.Pow(TempK[idpt] / 273, GasMats[iba].GasCond_B);
                    VapPres[iba, idpt] = Math.Exp(GasMats[iba].GasVapPAtm_A - GasMats[iba].GasVapPAtm_B / TempK[idpt]);
                }
            }

            #endregion

            #region //Calculate base case lambda, vap press and Aijs, and mole fraction adjusted to vapor pressure

            idpt = Params.nDataPts / 2;  // //Calculate the Aij at mid temperature

            for (iba = 0; iba < nBlowAg; iba++)
            {
                ViscosityTT[iba] = GasMats[iba].GasVis_A * Math.Pow(dTempKTT / 273, GasMats[iba].GasVis_B);
                LambdaBase[iba] = Lambda[iba, idpt];
                VapPresBase[iba] = VapPres[iba, idpt];
            }

            for (iba = 0; iba < nBlowAg; iba++)
                for (jba = 0; jba < nBlowAg; jba++)
                {
                    dTerm1 = ViscosityTT[iba] / ViscosityTT[jba] * Math.Sqrt(GasMats[jba].MolWt / GasMats[iba].MolWt);
                    dTerm2 = 8.0 * (1.0 + GasMats[iba].MolWt / GasMats[jba].MolWt);
                    dTerm3 = (1.0 + Math.Sqrt(dTerm1));
                    AijTT[iba, jba] = dTerm3 * dTerm3 / Math.Sqrt(dTerm2);
                }



            #endregion

        }

        public void KValuesFn()   //Calculate K values for all the formulation using TT variables
        {
            double dSum1, dSum2, dExtincCoeff;
            double StefBolzConst = 5.7E-8;   // σ Stefan Bolzman’s constant (5.7·10-8 W·m-2·K-4)
            const double dStrutCont = 0.5, dWallCont = 0.8, dEmissivity = 0.95;
            int iba, ifo, jba;


            for (ifo = 0; ifo < Params.nFormMax; ifo++)   // Gas Conductivity 
            {
                dSum1 = 0.0;
                for (iba = 0; iba < nBlowAg; iba++)
                {
                    dSum2 = 0.0;
                    for (jba = 0; jba < nBlowAg; jba++)
                        dSum2 += AijTT[iba, jba] * MoleFracsAdjTT[ifo, jba];
                    dSum1 += LambdaTT[iba] * MoleFracsAdjTT[ifo, iba] / dSum2;

                }
                /*
                                dExtincCoeff = 3.76 * FoamDensityTT[ifo] + 0.0878 * Math.Sqrt(FoamDensityTT[ifo]) / dCellSizeTT;                     //[Nielsen 1998].

                                dSum1 += 5.3333 / dExtincCoeff * StefBolzConst * dTempKTT * dTempKTT * dTempKTT; //Add radiation
                */
                dSum1 += 16.0 * dEmissivity / (2.0 - dEmissivity) * StefBolzConst * dTempKTT * dTempKTT * dTempKTT * dCellSizeTT; //Add radiation
                dSum1 += (dFracStrut * dStrutCont + (1.0 - dFracStrut) * dWallCont) * FoamDensityTT[ifo] / dPolyDenTT * dPolyCondTT; //Add polym cond

                KOutTT[ifo] = dSum1;

            }


        }

        public void AdjMoleFracs()
        {
            double dSum1, dPr, dtemp;
            int iba, ifo;

            dPr = dCellPressTT * dTempKTT / dTempKBase;

            for (ifo = 0; ifo < Params.nFormMax; ifo++)
            {
                dSum1 = 0.0;
                for (iba = 0; iba < nBlowAg; iba++)
                {
                    dtemp = dPr * MoleFracsTT[ifo, iba];
                    if (VapPresTT[iba] > 0.0 && dtemp > VapPresTT[iba]) dtemp = VapPresTT[iba];
                    dSum1 += dtemp;
                    MoleFracsAdjTT[ifo, iba] = dtemp;
                }

                for (iba = 0; iba < nBlowAg; iba++)
                    MoleFracsAdjTT[ifo, iba] = MoleFracsAdjTT[ifo, iba] / dSum1;
            }
        }
    }

}
