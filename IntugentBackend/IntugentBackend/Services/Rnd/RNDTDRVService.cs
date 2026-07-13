using IntugentBackend.Services.Core;
using System.Data;

namespace IntugentBackend.Services.Rnd
{
    public class RNDTDRVService
    {
        private readonly RNDTDRV _rndTdrv;
        private readonly RNDHome _rndHome;

        public RNDTDRVService(RNDTDRV rndTdrv, RNDHome rndHome)
        {
            _rndTdrv = rndTdrv;
            _rndHome = rndHome;
        }

        public void LoadData()
        {
            var rndTdrv = _rndTdrv;
            var rndHome = _rndHome;

            for (int i = 0; i < 8; i++)
            {
                // Logic moved from OnGet: updating data tables based on RNDHome.dtF
                UpdateTableCells(i, 1);
                UpdateTableCells(i, 13);
                UpdateTableCells(i, 25);
            }
        }

        private void UpdateTableCells(int i, int ir)
        {
            var dtF = _rndHome.dtF;
            var dtE = _rndTdrv.dtTdrvE;
            var dtC = _rndTdrv.dtTdrvC;

            // Simplified mapping for your DataTable rows
            string[] kCols = { "K10D25FInit", "K10D40FInit", "K10D75FInit", "K10D110FInit" };
            for (int j = 0; j < kCols.Length; j++)
            {
                var val = dtF.Rows[i][kCols[j]];
                dtE.Rows[ir + j + 1][i + 1] = val == DBNull.Value ? string.Empty : val.ToString();
            }
            // ... (Repeat for other columns as needed)
        }

        public bool GetTDRVValues(string[] sFieldsK, int irow, int icol1, string tb, string[] sFieldsR)
        {
            double dtmp;
            string sField = sFieldsK[irow];
            string sFieldR = sFieldsR[irow];

            if (string.IsNullOrEmpty(tb))
            {
                _rndHome.dtF.Rows[icol1][sFieldR] = _rndHome.dtF.Rows[icol1][sField] = DBNull.Value;
                _rndTdrv.dtTdrvC.Rows[irow][icol1] = string.Empty;
                return true;
            }
            else if (double.TryParse(tb, out dtmp) && dtmp > 0)
            {
                _rndHome.dtF.Rows[icol1][sField] = dtmp;
                _rndHome.dtF.Rows[icol1][sFieldR] = 1.0 / dtmp;
                _rndTdrv.dtTdrvC.Rows[irow][icol1] = (1 / dtmp).ToString("0.###");
                return true;
            }
            return false;
        }
    }
}