using System;
using System.Text.Json;
using System.Data;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Admin;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Rnd;

namespace IntugentBackend.Services.Rnd
{
    public class RNDRValuesService
    {
        private readonly RNDRValues _rndRValues;
        private readonly RNDFormulations _rndFormulations;
        private readonly RNDHome _rndHome;

        public RNDRValuesService(RNDRValues rndRValues, RNDFormulations rndFormulations, RNDHome rndHome)
        {
            _rndRValues = rndRValues;
            _rndFormulations = rndFormulations;
            _rndHome = rndHome;
        }

        // Mirrors the old page's OnGet handler.
        public bool Initialize()
        {
            if (_rndHome.IdSet <= 0) return false;

            _rndHome.GetDataSet(_rndHome.IdSet);
            _rndFormulations.ReadDataset();
            _rndFormulations.FormDescriptors();

            for (int ifo = 0; ifo < Params.nFormMax; ifo++)
            {
                _rndRValues.RCalc.FoamDensityBase[ifo] = _rndFormulations.Forms.FormAr[ifo].FoamDensity;
                _rndRValues.RCalc.FoamDensityTT[ifo] = _rndFormulations.Forms.FormAr[ifo].FoamDensity;
            }
            _rndRValues.RCalc.dTempKTT = _rndRValues.RData.dTestTempC + 273.0;

            LoadDataFromEmployee();
            if (string.IsNullOrEmpty(_rndRValues.RData.sXaxisTag)) SetDefaultValues();

            CollectBlowGases();
            _rndRValues.RCalc.CalBlowGasesProps(_rndRValues.RData.dTestTempC);

            SetFields();
            SetView();

            return true;
        }

        public void LoadDataFromEmployee()
        {
            var rValues = _rndRValues;
            if (rValues?.CLists?.drEmployee != null)
            {
                if (rValues.CLists.drEmployee.Table.Columns.Contains("sRValueParams") &&
                    rValues.CLists.drEmployee["sRValueParams"] != DBNull.Value)
                {
                    string js1 = rValues.CLists.drEmployee["sRValueParams"].ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(js1))
                    {
                        rValues.RData = JsonSerializer.Deserialize<CRData>(js1) ?? new CRData();
                    }
                }
            }
        }

        // Collect all the blowing agent materials in the formulations and their moles. Add Air as first blowing agent.
        public void CollectBlowGases()
        {
            var rValues = _rndRValues;
            int iba, ifo, ico;

            rValues.RCalc.GasMats.Clear();
            iba = -1;
            for (ico = 0; ico < _rndFormulations.Forms.POMats.Count; ico++)
            {
                if (_rndFormulations.Forms.POMats[ico].GassToLiqWtRatio > 0)
                {
                    iba += 1;
                    rValues.RCalc.GasMats.Add(_rndFormulations.Forms.POMats[ico]);
                    for (ifo = 0; ifo < Params.nFormMax; ifo++)
                    {
                        rValues.RCalc.MoleFracs[ifo, iba] = _rndFormulations.Forms.FormAr[ifo].POMatPbw[ico] * rValues.RCalc.GasMats[iba].GassToLiqWtRatio / rValues.RCalc.GasMats[iba].GassMolWt;
                    }
                }
            }

            rValues.RCalc.IAirBAIndex = -1; // Add Air if it is not already included
            for (ico = 0; ico < rValues.RCalc.GasMats.Count; ico++)
            {
                if (rValues.RCalc.GasMats[ico].ID == Params.iAirDBId) { rValues.RCalc.IAirBAIndex = ico; break; }
            }

            if (rValues.RCalc.IAirBAIndex == -1)
            {
                for (ico = 0; ico < _rndFormulations.dtPO.Rows.Count; ico++)
                {
                    if (_rndFormulations.dtPO.Rows[ico]["ID"].ToString() == Params.iAirDBId.ToString())
                    {
                        iba += 1;
                        if (iba + 1 > rValues.RCalc.GasMats.Count) rValues.RCalc.GasMats.Add(new CMaterial());
                        _rndFormulations.ModifyPOIsoLists(iba, ref rValues.RCalc.GasMats, ico, _rndFormulations.dtPO);
                        rValues.RCalc.IAirBAIndex = iba;
                        break;
                    }
                }
            }

            rValues.RCalc.nBlowAg = iba + 1;

            // Calculate the mole fraction of each gas in the cell (gaseous phase)
            for (ifo = 0; ifo < Params.nFormMax; ifo++)
            {
                double temp1 = 0.0;
                for (iba = 0; iba < rValues.RCalc.nBlowAg; iba++) temp1 += rValues.RCalc.MoleFracs[ifo, iba];
                if (temp1 > 0.0)
                    for (iba = 0; iba < rValues.RCalc.nBlowAg; iba++)
                    {
                        rValues.RCalc.MoleFracs[ifo, iba] = rValues.RCalc.MoleFracs[ifo, iba] / temp1;
                        rValues.RCalc.MoleFracsTT[ifo, iba] = rValues.RCalc.MoleFracs[ifo, iba];
                    }
            }
        }

        public void SetDefaultValues()
        {
            var d = _rndRValues.RData;
            d.dTestTempC = 25;
            d.dCellSize = 250E-6;      // microns
            d.dInitCellPress = 0.8;    // atm
            d.dPolDensity = 1200.0;    // kg/m3
            d.dAgeTempC = 50;
            d.dPolCond = 0.225;
            d.dFracStrut = 0.7;
            d.sYaxisTag = "RV";
            d.sXaxisTag = "TE";
        }

        public void SetFields()
        {
            var rValues = _rndRValues;
            rValues.gMeasureTemp = (rValues.RData.dTestTempC * 1.8 + 32.0).ToString("0.0");
            rValues.gCellSize = (1.0E6 * rValues.RData.dCellSize).ToString("0.0");
            rValues.gCellPress = rValues.RData.dInitCellPress.ToString("0.00");
            rValues.gPolDen = (rValues.RData.dPolDensity / rValues.CUConv.ToSi_Dens).ToString("0.00");
            rValues.gPolCond = (1000.0 * rValues.RData.dPolCond).ToString("0.00");
            rValues.gFracStruts = (100.0 * rValues.RData.dFracStrut).ToString("0.00");
            rValues.gXAxisSelectedValue = rValues.RData.sXaxisTag;
            rValues.gYAxisSelectedValue = rValues.RData.sYaxisTag;
        }

        public void SetView()
        {
            var rValues = _rndRValues;
            var RCalc = rValues.RCalc;
            var RData = rValues.RData;
            int idpt, ncount, iba, ifo;
            double AvFoamDen;

            rValues.dAr0 = new double[Params.nDataPts];
            rValues.dAr1 = new double[Params.nDataPts];
            rValues.dAr2 = new double[Params.nDataPts];
            rValues.dAr3 = new double[Params.nDataPts];
            rValues.dAr4 = new double[Params.nDataPts];
            rValues.dArX = new double[Params.nDataPts];

            // Reset Lambdas and vap press to base values
            RCalc.dTempKTT = RData.dTestTempC + 273.0;
            RCalc.dCellPressTT = RData.dInitCellPress;
            RCalc.dCellSizeTT = RData.dCellSize;
            RCalc.dPolyDenTT = RData.dPolDensity;
            RCalc.dPolyCondTT = RData.dPolCond;
            RCalc.dFracStrut = RData.dFracStrut;
            for (ifo = 0; ifo < Params.nFormMax; ifo++) RCalc.FoamDensityTT[ifo] = RCalc.FoamDensityBase[ifo];

            // Calculate the k values at the standard condition. Fill the data table with conductivities and mole fractions
            for (iba = 0; iba < RCalc.nBlowAg; iba++)
            {
                RCalc.VapPresTT[iba] = RCalc.VapPresBase[iba];
                RCalc.LambdaTT[iba] = RCalc.LambdaBase[iba];
            }
            RCalc.AdjMoleFracs();
            RCalc.KValuesFn();

            for (ifo = 0; ifo < rValues.nForms; ifo++)
            {
                RCalc.KValuesBase[ifo] = RCalc.KOutTT[ifo];
                RCalc.RValuesBase[ifo] = Params.RKConFactor / RCalc.KOutTT[ifo];
            }

            rValues.dtGasComp.Clear();
            for (int i = 0; i < Params.nComps; i++) rValues.dtGasComp.Rows.Add();

            int ir = 0;
            rValues.dtGasComp.Rows[ir][0] = "Thermal Properties";
            rValues.dtGasComp.Rows[ir + 1][0] = "   Thermal Conductivity (mW/m-K)";
            rValues.dtGasComp.Rows[ir + 2][0] = "   RValue (°F-ft2-hr/Btu)";
            for (ifo = 0; ifo < Params.nFormMax; ifo++)
            {
                rValues.dtGasComp.Rows[ir + 1][ifo + 1] = (RCalc.KValuesBase[ifo] * 1000.0).ToString("0.0");
                rValues.dtGasComp.Rows[ir + 2][ifo + 1] = RCalc.RValuesBase[ifo].ToString("0.0");
            }

            ir += 4;
            rValues.dtGasComp.Rows[ir][0] = "Gas Composition (Mole Fraction)";
            for (iba = 0; iba < RCalc.nBlowAg; iba++)
            {
                rValues.dtGasComp.Rows[ir + 1 + iba]["GasName"] = "   " + RCalc.GasMats[iba].GasName;
                for (ifo = 0; ifo < Params.nFormMax; ifo++)
                    rValues.dtGasComp.Rows[ir + 1 + iba][ifo + 1] = (RCalc.MoleFracs[ifo, iba] * 100.0).ToString("0.000");
            }

            rValues.gGasComp = rValues.dtGasComp.DefaultView;

            switch (RData.sXaxisTag)
            {
                case "CS": // Plot R/K Values with Cell Size
                    for (idpt = 0; idpt < Params.nDataPts; idpt++)
                    {
                        RCalc.dCellSizeTT = 0.5 * (1.0 + 2.0 * idpt / (double)Params.nDataPts) * RData.dCellSize;
                        rValues.dArX[idpt] = 1.0E6 * RCalc.dCellSizeTT;

                        RCalc.AdjMoleFracs();
                        RCalc.KValuesFn();

                        for (ifo = 0; ifo < rValues.nForms; ifo++)
                            RCalc.KValues[ifo, idpt] = RCalc.KOutTT[ifo];
                    }
                    break;

                case "DE": // Plot R/K Values with Density
                    ncount = 0;
                    double dsum1 = 0.0;
                    for (ifo = 0; ifo < rValues.nForms; ifo++)
                        if (RCalc.FoamDensityBase[ifo] > 0 && RCalc.FoamDensityBase[ifo] < 0.9 * Params.PolymerDensity)
                        { ncount += 1; dsum1 += RCalc.FoamDensityBase[ifo]; }
                    AvFoamDen = ncount > 0 ? dsum1 / ncount : 0;

                    for (idpt = 0; idpt < Params.nDataPts; idpt++)
                    {
                        rValues.dArX[idpt] = 0.5 * (1.0 + 2.0 * idpt / (double)Params.nDataPts) * AvFoamDen;
                        for (ifo = 0; ifo < rValues.nForms; ifo++) RCalc.FoamDensityTT[ifo] = rValues.dArX[idpt];

                        RCalc.AdjMoleFracs();
                        RCalc.KValuesFn();

                        for (ifo = 0; ifo < rValues.nForms; ifo++)
                            RCalc.KValues[ifo, idpt] = RCalc.KOutTT[ifo];
                    }
                    break;

                default: // Plot R/K Values with Temperature
                    RCalc.dCellPressTT = RData.dInitCellPress;
                    RCalc.dCellSizeTT = RData.dCellSize;
                    for (idpt = 0; idpt < Params.nDataPts; idpt++)
                    {
                        RCalc.dTempKTT = RCalc.TempK[idpt];
                        rValues.dArX[idpt] = RCalc.TempC[idpt] * 1.8 + 32;
                        for (iba = 0; iba < RCalc.nBlowAg; iba++)
                        {
                            RCalc.LambdaTT[iba] = RCalc.Lambda[iba, idpt];
                            RCalc.VapPresTT[iba] = RCalc.VapPres[iba, idpt];
                        }

                        RCalc.AdjMoleFracs();
                        RCalc.KValuesFn();

                        for (ifo = 0; ifo < rValues.nForms; ifo++)
                            RCalc.KValues[ifo, idpt] = RCalc.KOutTT[ifo];
                    }
                    break;
            }

            if (RData.sYaxisTag == "KV")
            {
                for (idpt = 0; idpt < Params.nDataPts; idpt++)
                {
                    rValues.dAr0[idpt] = RCalc.KValues[0, idpt] * 1000.0;
                    rValues.dAr1[idpt] = RCalc.KValues[1, idpt] * 1000.0;
                    rValues.dAr2[idpt] = RCalc.KValues[2, idpt] * 1000.0;
                    rValues.dAr3[idpt] = RCalc.KValues[3, idpt] * 1000.0;
                    rValues.dAr4[idpt] = RCalc.KValues[4, idpt] * 1000.0;
                }
            }
            else
            {
                for (idpt = 0; idpt < Params.nDataPts; idpt++)
                {
                    rValues.dAr0[idpt] = Params.RKConFactor / RCalc.KValues[0, idpt];
                    rValues.dAr1[idpt] = Params.RKConFactor / RCalc.KValues[1, idpt];
                    rValues.dAr2[idpt] = Params.RKConFactor / RCalc.KValues[2, idpt];
                    rValues.dAr3[idpt] = Params.RKConFactor / RCalc.KValues[3, idpt];
                    rValues.dAr4[idpt] = Params.RKConFactor / RCalc.KValues[4, idpt];
                }
            }
        }

        public void UpdateAxisSelection(string name, string value, string item)
        {
            var rValues = _rndRValues;
            switch (name)
            {
                case "gXAxis":
                    rValues.gXAxisSelectedItem = item;
                    rValues.gXAxisSelectedValue = value;
                    rValues.RData.sXaxisTag = value;
                    rValues.RCalc.sXAxisTitle = item;
                    break;

                case "gYAxis":
                    rValues.gYAxisSelectedItem = item;
                    rValues.gYAxisSelectedValue = value;
                    rValues.RData.sYaxisTag = value;
                    rValues.RCalc.sYAxisTitle = item;
                    break;
            }

            SetView();
            UpdateDataset();
        }

        public void UpdateLostFocusField(string name, string value)
        {
            var rValues = _rndRValues;
            var RData = rValues.RData;
            double dtemp;

            switch (name)
            {
                case "gMeasureTemp":
                    if (double.TryParse(value, out dtemp)) { RData.dTestTempC = (dtemp - 32.0) / 1.8; rValues.RCalc.CalBlowGasesProps(RData.dTestTempC); }
                    else rValues.gMeasureTemp = RData.dTestTempC > -459 ? (RData.dTestTempC * 1.8 + 32.0).ToString("0.000") : string.Empty;
                    break;

                case "gCellSize":
                    if (double.TryParse(value, out dtemp)) RData.dCellSize = 1.0E-6 * dtemp;
                    else rValues.gCellSize = RData.dCellSize > 0 ? (RData.dCellSize * 1.0E+6).ToString("0.000") : string.Empty;
                    break;

                case "gCellPress":
                    if (double.TryParse(value, out dtemp)) RData.dInitCellPress = dtemp;
                    else rValues.gCellPress = RData.dInitCellPress > 0 ? RData.dInitCellPress.ToString("0.000") : string.Empty;
                    break;

                case "gPolDen":
                    if (double.TryParse(value, out dtemp)) RData.dPolDensity = dtemp * rValues.CUConv.ToSi_Dens;
                    else rValues.gPolDen = RData.dPolDensity > 0 ? (RData.dPolDensity / rValues.CUConv.ToSi_Dens).ToString("0.000") : string.Empty;
                    break;

                case "gPolCond":
                    if (double.TryParse(value, out dtemp)) RData.dPolCond = 0.001 * dtemp;
                    else rValues.gPolCond = RData.dPolCond > 0 ? (RData.dPolCond * 1000.0).ToString("0.000") : string.Empty;
                    break;

                case "gFracStruts":
                    if (double.TryParse(value, out dtemp)) RData.dFracStrut = 0.01 * dtemp;
                    else rValues.gFracStruts = RData.dFracStrut > 0 ? (RData.dFracStrut * 100.0).ToString("0.000") : string.Empty;
                    break;
            }

            SetView();
            UpdateDataset();
        }

        // Mirrors the old page's "Set Default Values" button (OnPostExportData).
        public void ResetToDefaultValues()
        {
            SetDefaultValues();
            SetFields();
            SetView();
            UpdateDataset();
        }

        public void UpdateDataset()
        {
            var rValues = _rndRValues;
            if (rValues?.CLists?.drEmployee == null) return;

            DataTable table = rValues.CLists.drEmployee.Table;
            if (!table.Columns.Contains("sRValueParams"))
                table.Columns.Add("sRValueParams", typeof(string));

            string js1 = JsonSerializer.Serialize(rValues.RData);
            rValues.CLists.drEmployee["sRValueParams"] = js1;
            rValues.CLists.UpdateEmployee();
        }
    }
}
