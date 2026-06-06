using IntugentBackend.Services.Core;
using System.Data;

namespace IntugentBackend.Services.Rnd
{
    public class RndRawPropsService
    {
        private readonly ObjectsService _os;
        public RndRawPropsService(ObjectsService os) => _os = os;

        public bool GetDoubleFromGrid(string[] sFields, int irow, int icol1, string tb)
        {
            string sField = sFields[irow];
            if (string.IsNullOrEmpty(tb))
            {
                _os.RNDHome.dtF.Rows[icol1][sField] = DBNull.Value;
                return true;
            }
            if (double.TryParse(tb, out double dtmp))
            {
                _os.RNDHome.dtF.Rows[icol1][sField] = dtmp;
                return true;
            }
            return false;
        }

        public void CalculateDensity(int icol, int icol1)
        {
            var dtF = _os.RNDHome.dtF.Rows[icol1];
            if (dtF["DensAvgT"] != DBNull.Value && dtF["DensAvgL"] != DBNull.Value &&
                dtF["DensAvgW"] != DBNull.Value && dtF["DensMass"] != DBNull.Value)
            {
                double vol = 0.000578704 * (double)dtF["DensAvgT"] * (double)dtF["DensAvgL"] * (double)dtF["DensAvgW"];
                if (vol > 0)
                {
                    double dens = 0.00220462 * (double)dtF["DensMass"] / vol;
                    dtF["Density"] = dens;
                    _os.RNDRawProps.dtDensityC.Rows[3][icol] = dens.ToString("0.###");
                }
            }
        }
    }
}