using IntugentBackend.Services.Core;
using System.Data;

namespace IntugentBackend.Services.Rnd
{
    public class RNDTDRVService
    {
        private readonly ObjectsService _objectsService;

        public RNDTDRVService(ObjectsService objectsService)
        {
            _objectsService = objectsService;
        }

        public void LoadData()
        {
            var rndTdrv = _objectsService.RNDTDRV;
            var rndHome = _objectsService.RNDHome;

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
            var dtF = _objectsService.RNDHome.dtF;
            var dtE = _objectsService.RNDTDRV.dtTdrvE;
            var dtC = _objectsService.RNDTDRV.dtTdrvC;

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
                _objectsService.RNDHome.dtF.Rows[icol1][sFieldR] = _objectsService.RNDHome.dtF.Rows[icol1][sField] = DBNull.Value;
                _objectsService.RNDTDRV.dtTdrvC.Rows[irow][icol1] = string.Empty;
                return true;
            }
            else if (double.TryParse(tb, out dtmp) && dtmp > 0)
            {
                _objectsService.RNDHome.dtF.Rows[icol1][sField] = dtmp;
                _objectsService.RNDHome.dtF.Rows[icol1][sFieldR] = 1.0 / dtmp;
                _objectsService.RNDTDRV.dtTdrvC.Rows[irow][icol1] = (1 / dtmp).ToString("0.###");
                return true;
            }
            return false;
        }
    }
}