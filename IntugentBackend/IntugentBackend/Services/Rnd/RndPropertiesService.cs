using IntugentBackend.Services.Core;
using System.Data;
using IntugentBackend.Services.Rnd;
namespace IntugentBackend.Services.Rnd
{
    public class RndPropertiesService
    {
        private readonly RNDHome _os;
        public RndPropertiesService(RNDHome rndHome) => _os = rndHome;

        public void UpdateReactionData(int rowId, int colId, string text)
        {
            string[] sFields = { "ReactMixingTime", "React15PTime", "React50PTime", "React80PTime",
                                 "ReactCupEdgeTime", "React98PTime", "ReactMaxTempTime", "ReactMaxTemp",
                                 "ReactMaxHeight", "ReactSampleMass" };

            UpdateField(sFields[rowId], colId - 1, text);
        }

        public void UpdatePhotoData(int rowId, int colId, string text)
        {
            string[] sFields = { "PhotoPirPur", "PhotoIso", "PhotoCarbo", "PhotoTrimer" };
            UpdateField(sFields[rowId], colId - 1, text);
        }

        private void UpdateField(string fieldName, int rowIndex, string value)
        {
            if (string.IsNullOrEmpty(value))
                _os.dtF.Rows[rowIndex][fieldName] = DBNull.Value;
            else if (double.TryParse(value, out double dtmp))
                _os.dtF.Rows[rowIndex][fieldName] = dtmp;
            else
                _os.dtF.Rows[rowIndex][fieldName] = value;
        }
    }
}