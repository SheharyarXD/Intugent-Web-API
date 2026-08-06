using IntugentBackend.Services.Core;
using System.Data;

namespace IntugentBackend.Services.Rnd
{
    public class RndPropertiesService
    {
        private readonly RNDHome _rndHome;
        private readonly RNDProperties _rndProperties;
        private readonly RNDFormulations _rndFormulations;
        private readonly CLists _cLists;

        public RndPropertiesService(RNDHome rndHome, RNDProperties rndProperties, RNDFormulations rndFormulations, CLists cLists)
        {
            _rndHome = rndHome;
            _rndProperties = rndProperties;
            _rndFormulations = rndFormulations;
            _cLists = cLists;
        }

        public List<string> GetProductList()
        {
            var list = new List<string>();
            if (_cLists.dvComProd == null) return list;
            foreach (DataRowView row in _cLists.dvComProd)
                list.Add(row["Product Code"]?.ToString() ?? string.Empty);
            return list;
        }

        // Mirrors the old page's OnGet.
        public bool Initialize()
        {
            if (_rndHome.IdSet <= 0) return false;

            _rndHome.GetDataSet(_rndHome.IdSet);
            _rndFormulations.ReadDataset();

            var dtF = _rndHome.dtF;
            var rp = _rndProperties;

            for (int i = 0; i < dtF.Rows.Count && i < 8; i++)
            {
                SetCell(rp.dtReacE, 0, i + 1, dtF.Rows[i], "ReactMixingTime");
                SetCell(rp.dtReacE, 1, i + 1, dtF.Rows[i], "React15PTime");
                SetCell(rp.dtReacE, 2, i + 1, dtF.Rows[i], "React50PTime");
                SetCell(rp.dtReacE, 3, i + 1, dtF.Rows[i], "React80PTime");
                SetCell(rp.dtReacE, 4, i + 1, dtF.Rows[i], "ReactCupEdgeTime");
                SetCell(rp.dtReacE, 5, i + 1, dtF.Rows[i], "React98PTime");
                SetCell(rp.dtReacE, 6, i + 1, dtF.Rows[i], "ReactMaxTempTime");
                SetCell(rp.dtReacE, 7, i + 1, dtF.Rows[i], "ReactMaxTemp");
                SetCell(rp.dtReacE, 8, i + 1, dtF.Rows[i], "ReactMaxHeight");
                SetCell(rp.dtReacE, 9, i + 1, dtF.Rows[i], "ReactSampleMass");

                SetCell(rp.dtPhotoE, 0, i + 1, dtF.Rows[i], "PhotoPirPur");
                SetCell(rp.dtPhotoE, 1, i + 1, dtF.Rows[i], "PhotoIso");
                SetCell(rp.dtPhotoE, 2, i + 1, dtF.Rows[i], "PhotoCarbo");
                SetCell(rp.dtPhotoE, 3, i + 1, dtF.Rows[i], "PhotoTrimer");

                SetCellFormatted(rp.dtPropsE, 1, i + 1, dtF.Rows[i], "CompStr", "0.000");
                SetCellFormatted(rp.dtPropsE, 2, i + 1, dtF.Rows[i], "ClosedCellPer", "0.000");
                SetCellFormatted(rp.dtPropsE, 3, i + 1, dtF.Rows[i], "CellDia", "0");
                SetCellFormatted(rp.dtPropsE, 4, i + 1, dtF.Rows[i], "CellCount", "0");
                SetCellFormatted(rp.dtPropsE, 5, i + 1, dtF.Rows[i], "CellDiaIsotropy", "0.000");
                SetCellFormatted(rp.dtPropsE, 8, i + 1, dtF.Rows[i], "HotPlateRetainMass", "0.000");
                SetCellFormatted(rp.dtPropsE, 9, i + 1, dtF.Rows[i], "HotPlateRetainThick", "0.000");

                SetCell(rp.dtDataFiles, 0, i + 1, dtF.Rows[i], "sFileFTIR");
                SetCell(rp.dtDataFiles, 1, i + 1, dtF.Rows[i], "sFileTGA");
                SetCell(rp.dtDataFiles, 2, i + 1, dtF.Rows[i], "sFileFoamat");

                rp.dtComProd.Rows[i][1] = dtF.Rows[i]["Product Code"] == DBNull.Value ? string.Empty : (string)dtF.Rows[i]["Product Code"];
                rp.dtNotes.Rows[i][1] = dtF.Rows[i]["sNote"] == DBNull.Value ? string.Empty : (string)dtF.Rows[i]["sNote"];
            }

            return true;
        }

        public bool GetPropTestingComplete()
        {
            if (_rndHome.drS == null || _rndHome.drS["PropTestingComplete"] == DBNull.Value) return false;
            return (bool)_rndHome.drS["PropTestingComplete"];
        }

        private static void SetCell(DataTable dt, int row, int col, DataRow src, string field)
        {
            dt.Rows[row][col] = src[field] == DBNull.Value ? string.Empty : src[field].ToString();
        }

        private static void SetCellFormatted(DataTable dt, int row, int col, DataRow src, string field, string format)
        {
            dt.Rows[row][col] = src[field] == DBNull.Value ? string.Empty : ((double)src[field]).ToString(format);
        }

        public void UpdateReactionData(int irow, int icol, string text)
        {
            string[] sFields = { "ReactMixingTime", "React15PTime", "React50PTime", "React80PTime", "ReactCupEdgeTime", "React98PTime", "ReactMaxTempTime", "ReactMaxTemp", "ReactMaxHeight", "ReactSampleMass" };
            if (icol == 0 || irow > 9) return;
            int icol1 = icol - 1;

            UpdateField(sFields[irow], icol1, text);
            _rndProperties.dtReacE.Rows[irow][icol] = text;
            _rndHome.UpdateFormulatiions();
        }

        public void UpdatePhotoData(int irow, int icol, string text)
        {
            string[] sFields = { "PhotoPirPur", "PhotoIso", "PhotoCarbo", "PhotoTrimer" };
            if (icol == 0 || irow > 3) return;
            int icol1 = icol - 1;

            UpdateField(sFields[irow], icol1, text);
            _rndProperties.dtPhotoE.Rows[irow][icol] = text;
            _rndHome.UpdateFormulatiions();
        }

        public void UpdateDataFile(int irow, int icol, string text)
        {
            if (icol == 0 || irow > 9) return;
            int icol1 = icol - 1;

            string field = irow switch { 0 => "sFileFTIR", 1 => "sFileTGA", 2 => "sFileFoamat", _ => null! };
            if (field == null) return;

            _rndHome.dtF.Rows[icol1][field] = string.IsNullOrEmpty(text) ? DBNull.Value : text;
            _rndProperties.dtDataFiles.Rows[irow][icol] = text;
            _rndHome.UpdateFormulatiions();
        }

        public void UpdateNote(int irow, string text)
        {
            if (irow < 0 || irow >= _rndHome.dtF.Rows.Count) return;

            string value = text ?? string.Empty;
            if (value.Length > 255) value = value.Substring(0, 255);

            _rndHome.dtF.Rows[irow]["sNote"] = string.IsNullOrEmpty(value) ? DBNull.Value : value;
            _rndProperties.dtNotes.Rows[irow][1] = value;
            _rndHome.UpdateFormulatiions();
        }

        public void UpdateProductCode(int irow, string text)
        {
            if (irow < 0 || irow >= _rndHome.dtF.Rows.Count) return;

            _rndHome.dtF.Rows[irow]["Product Code"] = string.IsNullOrEmpty(text) ? DBNull.Value : text;
            _rndProperties.dtComProd.Rows[irow][1] = text ?? string.Empty;
            _rndHome.UpdateFormulatiions();
        }

        public void UpdatePropTestingComplete(bool value)
        {
            if (_rndHome.drS == null) return;
            _rndHome.drS["PropTestingComplete"] = value;
            _rndHome.UpdateDataSet();
        }

        private void UpdateField(string fieldName, int rowIndex, string value)
        {
            if (string.IsNullOrEmpty(value))
                _rndHome.dtF.Rows[rowIndex][fieldName] = DBNull.Value;
            else if (double.TryParse(value, out double dtmp))
                _rndHome.dtF.Rows[rowIndex][fieldName] = dtmp;
        }
    }
}
