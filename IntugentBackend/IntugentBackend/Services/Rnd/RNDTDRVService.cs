using IntugentBackend.Services.Core;
using System.Data;

namespace IntugentBackend.Services.Rnd
{
    public class RNDTDRVService
    {
        private readonly RNDTDRV _rndTdrv;
        private readonly RNDHome _rndHome;
        private readonly RNDFormulations _rndFormulations;

        private static readonly string[] sFieldsK =
        {
            "", "", "K10D25FInit", "K10D40FInit", "K10D75FInit", "K10D110FInit", "", "K10D25FFinal", "K10D40FFinal", "K10D75FFinal", "K10D110FFinal", "", "",
            "", "K90D25FInit", "K90D40FInit", "K90D75FInit", "K90D110FInit", "", "K90D25FFinal", "K90D40FFinal", "K90D75FFinal", "K90D110FFinal", "", "",
            "", "K180D25FInit", "K180D40FInit", "K180D75FInit", "K180D110FInit", "", "K180D25FFinal", "K180D40FFinal", "K180D75FFinal", "K180D110FFinal"
        };

        private static readonly string[] sFieldsR =
        {
            "", "", "R10D25FInit", "R10D40FInit", "R10D75FInit", "R10D110FInit", "", "R10D25FFinal", "R10D40FFinal", "R10D75FFinal", "R10D110FFinal", "", "",
            "", "R90D25FInit", "R90D40FInit", "R90D75FInit", "R90D110FInit", "", "R90D25FFinal", "R90D40FFinal", "R90D75FFinal", "R90D110FFinal", "", "",
            "", "R180D25FInit", "R180D40FInit", "R180D75FInit", "R180D110FInit", "", "R180D25FFinal", "R180D40FFinal", "R180D75FFinal", "R180D110FFinal"
        };

        public RNDTDRVService(RNDTDRV rndTdrv, RNDHome rndHome, RNDFormulations rndFormulations)
        {
            _rndTdrv = rndTdrv;
            _rndHome = rndHome;
            _rndFormulations = rndFormulations;
        }

        // Mirrors the old page's OnGet.
        public bool Initialize()
        {
            if (_rndHome.IdSet <= 0) return false;

            _rndHome.GetDataSet(_rndHome.IdSet);
            _rndFormulations.ReadDataset();

            var dtF = _rndHome.dtF;
            var dtE = _rndTdrv.dtTdrvE;
            var dtC = _rndTdrv.dtTdrvC;

            for (int i = 0; i < dtF.Rows.Count && i < 8; i++)
            {
                FillBlock(dtF.Rows[i], dtE, dtC, i, 1,
                    "K10D25FInit", "K10D40FInit", "K10D75FInit", "K10D110FInit",
                    "K10D25FFinal", "K10D40FFinal", "K10D75FFinal", "K10D110FFinal",
                    "R10D25FInit", "R10D40FInit", "R10D75FInit", "R10D110FInit",
                    "R10D25FFinal", "R10D40FFinal", "R10D75FFinal", "R10D110FFinal");

                FillBlock(dtF.Rows[i], dtE, dtC, i, 13,
                    "K90D25FInit", "K90D40FInit", "K90D75FInit", "K90D110FInit",
                    "K90D25FFinal", "K90D40FFinal", "K90D75FFinal", "K90D110FFinal",
                    "R90D25FInit", "R90D40FInit", "R90D75FInit", "R90D110FInit",
                    "R90D25FFinal", "R90D40FFinal", "R90D75FFinal", "R90D110FFinal");

                FillBlock(dtF.Rows[i], dtE, dtC, i, 25,
                    "K180D25FInit", "K180D40FInit", "K180D75FInit", "K180D110FInit",
                    "K180D25FFinal", "K180D40FFinal", "K180D75FFinal", "K180D110FFinal",
                    "R180D25FInit", "R180D40FInit", "R180D75FInit", "R180D110FInit",
                    "R180D25FFinal", "R180D40FFinal", "R180D75FFinal", "R180D110FFinal");
            }

            return true;
        }

        private static void FillBlock(DataRow src, DataTable dtE, DataTable dtC, int i, int ir,
            string kInit1, string kInit2, string kInit3, string kInit4,
            string kFinal1, string kFinal2, string kFinal3, string kFinal4,
            string rInit1, string rInit2, string rInit3, string rInit4,
            string rFinal1, string rFinal2, string rFinal3, string rFinal4)
        {
            SetE(dtE, ir + 1, i, src, kInit1); SetE(dtE, ir + 2, i, src, kInit2); SetE(dtE, ir + 3, i, src, kInit3); SetE(dtE, ir + 4, i, src, kInit4);
            SetE(dtE, ir + 6, i, src, kFinal1); SetE(dtE, ir + 7, i, src, kFinal2); SetE(dtE, ir + 8, i, src, kFinal3); SetE(dtE, ir + 9, i, src, kFinal4);

            SetC(dtC, ir + 1, i, src, rInit1); SetC(dtC, ir + 2, i, src, rInit2); SetC(dtC, ir + 3, i, src, rInit3); SetC(dtC, ir + 4, i, src, rInit4);
            SetC(dtC, ir + 6, i, src, rFinal1); SetC(dtC, ir + 7, i, src, rFinal2); SetC(dtC, ir + 8, i, src, rFinal3); SetC(dtC, ir + 9, i, src, rFinal4);
        }

        private static void SetE(DataTable dtE, int row, int i, DataRow src, string field)
        {
            dtE.Rows[row][i + 1] = src[field] == DBNull.Value ? string.Empty : src[field].ToString();
        }

        private static void SetC(DataTable dtC, int row, int i, DataRow src, string field)
        {
            dtC.Rows[row][i] = src[field] == DBNull.Value ? string.Empty : ((double)src[field]).ToString("0.000");
        }

        public bool GetAgedTestingComplete()
        {
            if (_rndHome.drS == null || _rndHome.drS["AgedTestingComplete"] == DBNull.Value) return false;
            return (bool)_rndHome.drS["AgedTestingComplete"];
        }

        public void UpdateAgedTestingComplete(bool value)
        {
            if (_rndHome.drS == null) return;
            _rndHome.drS["AgedTestingComplete"] = value;
            _rndHome.UpdateDataSet();
        }

        // Mirrors the old page's OnPostOngExpPropsE.
        public void UpdateExpProp(int irow, int icol, string text)
        {
            if (icol == 0 || irow > 34) return;
            int icol1 = icol - 1;

            bool bGet = (irow > 1 && irow < 6) || (irow > 6 && irow < 11) ||
                        (irow > 13 && irow < 18) || (irow > 18 && irow < 23) ||
                        (irow > 25 && irow < 30) || (irow > 30 && irow < 35);
            if (!bGet) return;

            GetTDRVValues(irow, icol1, text);
            _rndHome.UpdateFormulatiions();
        }

        private void GetTDRVValues(int irow, int icol1, string tb)
        {
            string sField = sFieldsK[irow];
            string sFieldR = sFieldsR[irow];

            if (string.IsNullOrEmpty(tb))
            {
                _rndHome.dtF.Rows[icol1][sFieldR] = _rndHome.dtF.Rows[icol1][sField] = DBNull.Value;
                _rndTdrv.dtTdrvC.Rows[irow][icol1] = string.Empty;
            }
            else if (double.TryParse(tb, out double dtmp) && dtmp > 0)
            {
                _rndHome.dtF.Rows[icol1][sField] = dtmp;
                _rndHome.dtF.Rows[icol1][sFieldR] = 1.0 / dtmp;
                _rndTdrv.dtTdrvC.Rows[irow][icol1] = (1 / dtmp).ToString("0.###");
            }
        }
    }
}
