using IntugentBackend.Services.Core;
using System.Data;

namespace IntugentBackend.Services.Rnd
{
    public class RndRawPropsService
    {
        private readonly RNDHome _rndHome;
        private readonly RNDRawProps _rndRawProps;

        public RndRawPropsService(RNDHome rndHome, RNDRawProps rndRawProps)
        {
            _rndHome = rndHome;
            _rndRawProps = rndRawProps;
        }

        // Mirrors the old page's OnGet: populate all 8 raw-property grids from the current dataset's formulations.
        public void Initialize()
        {
            var dtF = _rndHome.dtF;
            var rp = _rndRawProps;

            for (int i = 0; i < dtF.Rows.Count && i < 8; i++)
            {
                int i2p1 = 2 * i + 1, i2p2 = i2p1 + 1;

                SetCell(rp.dtDensityE, 0, i + 1, dtF.Rows[i], "DensT1");
                SetCell(rp.dtDensityE, 1, i + 1, dtF.Rows[i], "DensT2");
                SetCell(rp.dtDensityE, 2, i + 1, dtF.Rows[i], "DensT3");
                SetCell(rp.dtDensityE, 3, i + 1, dtF.Rows[i], "DensT4");
                SetCell(rp.dtDensityE, 4, i + 1, dtF.Rows[i], "DensT5");
                SetCell(rp.dtDensityE, 5, i + 1, dtF.Rows[i], "DensL1");
                SetCell(rp.dtDensityE, 6, i + 1, dtF.Rows[i], "DensL2");
                SetCell(rp.dtDensityE, 7, i + 1, dtF.Rows[i], "DensW1");
                SetCell(rp.dtDensityE, 8, i + 1, dtF.Rows[i], "DensW2");
                SetCell(rp.dtDensityE, 9, i + 1, dtF.Rows[i], "DensMass");

                SetCellFormatted(rp.dtDensityC, 0, i + 1, dtF.Rows[i], "DensAvgT", "0.000");
                SetCellFormatted(rp.dtDensityC, 1, i + 1, dtF.Rows[i], "DensAvgL", "0.000");
                SetCellFormatted(rp.dtDensityC, 2, i + 1, dtF.Rows[i], "DensAvgW", "0.000");
                SetCellFormatted(rp.dtDensityC, 3, i + 1, dtF.Rows[i], "Density", "0.000");

                SetCell(rp.dtCompStrE, 0, i + 1, dtF.Rows[i], "CompStr1");
                SetCell(rp.dtCompStrE, 1, i + 1, dtF.Rows[i], "CompStr2");
                SetCell(rp.dtCompStrE, 2, i + 1, dtF.Rows[i], "CompStr3");
                SetCell(rp.dtCompStrE, 3, i + 1, dtF.Rows[i], "CompStr4");
                SetCellFormatted(rp.dtCompStrC, 0, i + 1, dtF.Rows[i], "CompStr", "0.000");

                SetCell(rp.dtClosedCellE, 0, i + 1, dtF.Rows[i], "ClosedCellPer1");
                SetCell(rp.dtClosedCellE, 1, i + 1, dtF.Rows[i], "ClosedCellPer2");
                SetCell(rp.dtClosedCellE, 2, i + 1, dtF.Rows[i], "ClosedCellPer3");
                SetCellFormatted(rp.dtClosedCellC, 0, i + 1, dtF.Rows[i], "ClosedCellPer", "0.000");

                SetCell(rp.dtPoreScanE, 0, i + 1, dtF.Rows[i], "CellDiaTop");
                SetCell(rp.dtPoreScanE, 1, i + 1, dtF.Rows[i], "CellStDevTop");
                SetCell(rp.dtPoreScanE, 2, i + 1, dtF.Rows[i], "CellCountTop");
                SetCell(rp.dtPoreScanE, 3, i + 1, dtF.Rows[i], "CellDiaSide");
                SetCell(rp.dtPoreScanE, 4, i + 1, dtF.Rows[i], "CellStDevSide");
                SetCell(rp.dtPoreScanE, 5, i + 1, dtF.Rows[i], "CellCountSide");
                SetCellFormatted(rp.dtPoreScanC, 0, i + 1, dtF.Rows[i], "CellDia", "0");
                SetCellFormatted(rp.dtPoreScanC, 1, i + 1, dtF.Rows[i], "CellCount", "0");
                SetCellFormatted(rp.dtPoreScanC, 2, i + 1, dtF.Rows[i], "CellDiaIsotropy", "0.000");

                SetCell(rp.dtHotPlatesE, 0, i2p1, dtF.Rows[i], "HotPlateInitMass");
                SetCell(rp.dtHotPlatesE, 1, i2p1, dtF.Rows[i], "HotPlateInitH1");
                SetCell(rp.dtHotPlatesE, 2, i2p1, dtF.Rows[i], "HotPlateInitH2");
                SetCell(rp.dtHotPlatesE, 3, i2p1, dtF.Rows[i], "HotPlateInitH3");
                SetCell(rp.dtHotPlatesE, 4, i2p1, dtF.Rows[i], "HotPlateInitH4");
                SetCell(rp.dtHotPlatesE, 5, i2p1, dtF.Rows[i], "HotPlateInitH5");
                SetCellFormatted(rp.dtHotPlatesC1, 0, i2p1, dtF.Rows[i], "HotPlateInitH", "0.000");

                SetCell(rp.dtHotPlatesE, 0, i2p2, dtF.Rows[i], "HotPlateFinalMass");
                SetCell(rp.dtHotPlatesE, 1, i2p2, dtF.Rows[i], "HotPlateFinalH1");
                SetCell(rp.dtHotPlatesE, 2, i2p2, dtF.Rows[i], "HotPlateFinalH2");
                SetCell(rp.dtHotPlatesE, 3, i2p2, dtF.Rows[i], "HotPlateFinalH3");
                SetCell(rp.dtHotPlatesE, 4, i2p2, dtF.Rows[i], "HotPlateFinalH4");
                SetCell(rp.dtHotPlatesE, 5, i2p2, dtF.Rows[i], "HotPlateFinalH5");
                SetCellFormatted(rp.dtHotPlatesC1, 0, i2p2, dtF.Rows[i], "HotPlateFinalH", "0.000");

                SetCellFormatted(rp.dtHotPlatesC, 0, i + 1, dtF.Rows[i], "HotPlateRetainMass", "0.000");
                SetCellFormatted(rp.dtHotPlatesC, 1, i + 1, dtF.Rows[i], "HotPlateRetainThick", "0.000");
            }
        }

        private static void SetCell(DataTable dt, int row, int col, DataRow src, string field)
        {
            dt.Rows[row][col] = src[field] == DBNull.Value ? string.Empty : src[field].ToString();
        }

        private static void SetCellFormatted(DataTable dt, int row, int col, DataRow src, string field, string format)
        {
            dt.Rows[row][col] = src[field] == DBNull.Value ? string.Empty : ((double)src[field]).ToString(format);
        }

        public bool GetDoubleFromGrid(string[] sFields, int irow, int icol1, string tb)
        {
            string sField = sFields[irow];
            if (string.IsNullOrEmpty(tb))
            {
                _rndHome.dtF.Rows[icol1][sField] = DBNull.Value;
                return true;
            }
            if (double.TryParse(tb, out double dtmp))
            {
                _rndHome.dtF.Rows[icol1][sField] = dtmp;
                return true;
            }
            return false;
        }

        public void CalculateDensity(int icol, int icol1)
        {
            var dtF = _rndHome.dtF.Rows[icol1];
            var rp = _rndRawProps;

            double dSum = 0, dtemp1; int nCount = 0;
            if (dtF["DensT1"] != DBNull.Value) { nCount++; dSum += (double)dtF["DensT1"]; }
            if (dtF["DensT2"] != DBNull.Value) { nCount++; dSum += (double)dtF["DensT2"]; }
            if (dtF["DensT3"] != DBNull.Value) { nCount++; dSum += (double)dtF["DensT3"]; }
            if (dtF["DensT4"] != DBNull.Value) { nCount++; dSum += (double)dtF["DensT4"]; }
            if (dtF["DensT5"] != DBNull.Value) { nCount++; dSum += (double)dtF["DensT5"]; }
            if (nCount > 0) { dtemp1 = dSum / nCount; dtF["DensAvgT"] = dtemp1; rp.dtDensityC.Rows[0][icol] = dtemp1.ToString("0.###"); }
            else { dtF["DensAvgT"] = DBNull.Value; rp.dtDensityC.Rows[0][icol] = string.Empty; }

            dSum = 0; nCount = 0;
            if (dtF["DensL1"] != DBNull.Value) { nCount++; dSum += (double)dtF["DensL1"]; }
            if (dtF["DensL2"] != DBNull.Value) { nCount++; dSum += (double)dtF["DensL2"]; }
            if (nCount > 0) { dtemp1 = dSum / nCount; dtF["DensAvgL"] = dtemp1; rp.dtDensityC.Rows[1][icol] = dtemp1.ToString("0.###"); }
            else { dtF["DensAvgL"] = DBNull.Value; rp.dtDensityC.Rows[1][icol] = string.Empty; }

            dSum = 0; nCount = 0;
            if (dtF["DensW1"] != DBNull.Value) { nCount++; dSum += (double)dtF["DensW1"]; }
            if (dtF["DensW2"] != DBNull.Value) { nCount++; dSum += (double)dtF["DensW2"]; }
            if (nCount > 0) { dtemp1 = dSum / nCount; dtF["DensAvgW"] = dtemp1; rp.dtDensityC.Rows[2][icol] = dtemp1.ToString("0.###"); }
            else { dtF["DensAvgW"] = DBNull.Value; rp.dtDensityC.Rows[2][icol] = string.Empty; }

            if (dtF["DensAvgT"] == DBNull.Value || dtF["DensAvgL"] == DBNull.Value || dtF["DensAvgW"] == DBNull.Value || dtF["DensMass"] == DBNull.Value)
            {
                dtF["Density"] = DBNull.Value; rp.dtDensityC.Rows[3][icol] = string.Empty;
            }
            else
            {
                double vol = 0.000578704 * (double)dtF["DensAvgT"] * (double)dtF["DensAvgL"] * (double)dtF["DensAvgW"];
                if (vol > 0)
                {
                    double dens = 0.00220462 * (double)dtF["DensMass"] / vol;
                    dtF["Density"] = dens;
                    rp.dtDensityC.Rows[3][icol] = dens.ToString("0.###");
                }
                else { dtF["Density"] = DBNull.Value; rp.dtDensityC.Rows[3][icol] = string.Empty; }
            }
        }

        public void UpdateCompStr(int irow, int icol, string text)
        {
            string[] sFields = { "CompStr1", "CompStr2", "CompStr3", "CompStr4" };
            int icol1 = icol - 1;
            if (icol == 0 || irow > 3) return;

            GetDoubleFromGrid(sFields, irow, icol1, text);

            var dtF = _rndHome.dtF.Rows[icol1];
            double dSum = 0, dtemp1; int nCount = 0;
            if (dtF["CompStr1"] != DBNull.Value) { nCount++; dSum += (double)dtF["CompStr1"]; }
            if (dtF["CompStr2"] != DBNull.Value) { nCount++; dSum += (double)dtF["CompStr2"]; }
            if (dtF["CompStr3"] != DBNull.Value) { nCount++; dSum += (double)dtF["CompStr3"]; }
            if (dtF["CompStr4"] != DBNull.Value) { nCount++; dSum += (double)dtF["CompStr4"]; }

            if (nCount > 0) { dtemp1 = dSum / nCount; dtF["CompStr"] = dtemp1; _rndRawProps.dtCompStrC.Rows[0][icol] = dtemp1.ToString("0.###"); }
            else { dtF["CompStr"] = DBNull.Value; _rndRawProps.dtCompStrC.Rows[0][icol] = string.Empty; }

            _rndHome.UpdateFormulatiions();
        }

        public void UpdateClosedCell(int irow, int icol, string text)
        {
            string[] sFields = { "ClosedCellPer1", "ClosedCellPer2", "ClosedCellPer3" };
            int icol1 = icol - 1;
            if (icol == 0 || irow > 2) return;

            GetDoubleFromGrid(sFields, irow, icol1, text);

            var dtF = _rndHome.dtF.Rows[icol1];
            double dSum = 0, dtemp1; int nCount = 0;
            if (dtF["ClosedCellPer1"] != DBNull.Value) { nCount++; dSum += (double)dtF["ClosedCellPer1"]; }
            if (dtF["ClosedCellPer2"] != DBNull.Value) { nCount++; dSum += (double)dtF["ClosedCellPer2"]; }
            if (dtF["ClosedCellPer3"] != DBNull.Value) { nCount++; dSum += (double)dtF["ClosedCellPer3"]; }

            if (nCount > 0) { dtemp1 = dSum / nCount; dtF["ClosedCellPer"] = dtemp1; _rndRawProps.dtClosedCellC.Rows[0][icol] = dtemp1.ToString("0.###"); }
            else { dtF["ClosedCellPer"] = DBNull.Value; _rndRawProps.dtClosedCellC.Rows[0][icol] = string.Empty; }

            _rndHome.UpdateFormulatiions();
        }

        public void UpdatePoreScan(int irow, int icol, string text)
        {
            string[] sFields = { "CellDiaTop", "CellStDevTop", "CellCountTop", "CellDiaSide", "CellStDevSide", "CellCountSide" };
            int icol1 = icol - 1;
            if (icol == 0 || irow > 5) return;

            bool ok = GetDoubleFromGrid(sFields, irow, icol1, text);
            if (!ok) return;

            bool bDi = irow == 0 || irow == 3;
            bool bC = irow == 2 || irow == 5;

            var dtF = _rndHome.dtF.Rows[icol1];
            double dSum = 0, dtemp1, num = 0.0, denom = 0.0; int nCount = 0;

            if (bDi)
            {
                if (dtF["CellDiaTop"] != DBNull.Value) { nCount++; num = (double)dtF["CellDiaTop"]; dSum += num; }
                if (dtF["CellDiaSide"] != DBNull.Value) { nCount++; denom = (double)dtF["CellDiaSide"]; dSum += denom; }
                if (nCount > 0) { dtemp1 = dSum / nCount; dtF["CellDia"] = dtemp1; _rndRawProps.dtPoreScanC.Rows[0][icol] = dtemp1.ToString("0"); }
                else { dtF["CellDia"] = DBNull.Value; _rndRawProps.dtPoreScanC.Rows[0][icol] = string.Empty; }

                if (dtF["CellDiaTop"] == DBNull.Value || dtF["CellDiaSide"] == DBNull.Value)
                { dtF["CellDiaIsotropy"] = DBNull.Value; _rndRawProps.dtPoreScanC.Rows[2][icol] = string.Empty; }
                else if (denom > 0)
                { dtemp1 = num / denom; dtF["CellDiaIsotropy"] = dtemp1; _rndRawProps.dtPoreScanC.Rows[2][icol] = dtemp1.ToString("0.###"); }
                else { dtF["CellDiaIsotropy"] = DBNull.Value; _rndRawProps.dtPoreScanC.Rows[2][icol] = string.Empty; }
            }
            else if (bC)
            {
                if (dtF["CellCountTop"] != DBNull.Value) { nCount++; dSum += (double)dtF["CellCountTop"]; }
                if (dtF["CellCountSide"] != DBNull.Value) { nCount++; dSum += (double)dtF["CellCountSide"]; }
                if (nCount > 0) { dtemp1 = dSum / nCount; dtF["CellCount"] = dtemp1; _rndRawProps.dtPoreScanC.Rows[1][icol] = dtemp1.ToString("0"); }
                else { dtF["CellCount"] = DBNull.Value; _rndRawProps.dtPoreScanC.Rows[1][icol] = string.Empty; }
            }

            _rndHome.UpdateFormulatiions();
        }

        public void UpdateHotPlates(int irow, int icol, string text)
        {
            if (icol == 0 || icol == 17 || irow > 17) return;

            string[] sFieldInit = { "HotPlateInitMass", "HotPlateInitH1", "HotPlateInitH2", "HotPlateInitH3", "HotPlateInitH4", "HotPlateInitH5" };
            string[] sFieldFinal = { "HotPlateFinalMass", "HotPlateFinalH1", "HotPlateFinalH2", "HotPlateFinalH3", "HotPlateFinalH4", "HotPlateFinalH5" };

            int icol1 = icol - 1;
            int ic = icol1 / 2;
            int itest = icol1 - 2 * ic;
            bool bi = itest == 0;

            var dtF = _rndHome.dtF.Rows[ic];
            double dSum = 0, dtemp1, num = 0.0, denom = 0.0; int nCount = 0;
            bool bH;

            if (bi)
            {
                bool ok = GetDoubleFromGrid(sFieldInit, irow, ic, text);
                if (!ok) return;
                bH = irow != 0;

                if (bH)
                {
                    if (dtF["HotPlateInitH1"] != DBNull.Value) { nCount++; dSum += (double)dtF["HotPlateInitH1"]; }
                    if (dtF["HotPlateInitH2"] != DBNull.Value) { nCount++; dSum += (double)dtF["HotPlateInitH2"]; }
                    if (dtF["HotPlateInitH3"] != DBNull.Value) { nCount++; dSum += (double)dtF["HotPlateInitH3"]; }
                    if (dtF["HotPlateInitH4"] != DBNull.Value) { nCount++; dSum += (double)dtF["HotPlateInitH4"]; }
                    if (dtF["HotPlateInitH5"] != DBNull.Value) { nCount++; dSum += (double)dtF["HotPlateInitH5"]; }

                    if (nCount > 0) { dtemp1 = dSum / nCount; dtF["HotPlateInitH"] = dtemp1; _rndRawProps.dtHotPlatesC1.Rows[0][icol] = dtemp1.ToString("0.###"); }
                    else { dtF["HotPlateInitH"] = DBNull.Value; _rndRawProps.dtHotPlatesC1.Rows[0][icol] = string.Empty; }
                }
            }
            else
            {
                bool ok = GetDoubleFromGrid(sFieldFinal, irow, ic, text);
                if (!ok) return;
                bH = irow != 0;

                if (bH)
                {
                    if (dtF["HotPlateFinalH1"] != DBNull.Value) { nCount++; dSum += (double)dtF["HotPlateFinalH1"]; }
                    if (dtF["HotPlateFinalH2"] != DBNull.Value) { nCount++; dSum += (double)dtF["HotPlateFinalH2"]; }
                    if (dtF["HotPlateFinalH3"] != DBNull.Value) { nCount++; dSum += (double)dtF["HotPlateFinalH3"]; }
                    if (dtF["HotPlateFinalH4"] != DBNull.Value) { nCount++; dSum += (double)dtF["HotPlateFinalH4"]; }
                    if (dtF["HotPlateFinalH5"] != DBNull.Value) { nCount++; dSum += (double)dtF["HotPlateFinalH5"]; }

                    if (nCount > 0) { dtemp1 = dSum / nCount; dtF["HotPlateFinalH"] = dtemp1; _rndRawProps.dtHotPlatesC1.Rows[0][icol] = dtemp1.ToString("0.###"); }
                    else { dtF["HotPlateFinalH"] = DBNull.Value; _rndRawProps.dtHotPlatesC1.Rows[0][icol] = string.Empty; }
                }
            }

            if (bH)
            {
                if (dtF["HotPlateInitH"] == DBNull.Value || dtF["HotPlateFinalH"] == DBNull.Value)
                { dtF["HotPlateRetainThick"] = DBNull.Value; _rndRawProps.dtHotPlatesC.Rows[1][ic + 1] = string.Empty; }
                else
                {
                    denom = (double)dtF["HotPlateInitH"];
                    num = (double)dtF["HotPlateFinalH"];
                }
                if (denom > 0) { dtF["HotPlateRetainThick"] = 100.0 * num / denom; _rndRawProps.dtHotPlatesC.Rows[1][ic + 1] = (100.0 * num / denom).ToString("0.###"); }
                else { dtF["HotPlateRetainThick"] = DBNull.Value; _rndRawProps.dtHotPlatesC.Rows[1][ic + 1] = string.Empty; }
            }
            else
            {
                if (dtF["HotPlateInitMass"] == DBNull.Value || dtF["HotPlateFinalMass"] == DBNull.Value)
                { dtF["HotPlateRetainMass"] = DBNull.Value; _rndRawProps.dtHotPlatesC.Rows[0][ic + 1] = string.Empty; }
                else
                {
                    denom = (double)dtF["HotPlateInitMass"];
                    num = (double)dtF["HotPlateFinalMass"];
                }
                if (denom > 0) { dtF["HotPlateRetainMass"] = 100.0 * num / denom; _rndRawProps.dtHotPlatesC.Rows[0][ic + 1] = (100.0 * num / denom).ToString("0.###"); }
                else { dtF["HotPlateRetainMass"] = DBNull.Value; _rndRawProps.dtHotPlatesC.Rows[0][ic + 1] = string.Empty; }
            }

            _rndHome.UpdateFormulatiions();
        }
    }
}
