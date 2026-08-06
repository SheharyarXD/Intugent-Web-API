using IntugentBackend.Services.Core;
using IntugentBackend.Services.Admin;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Rnd;

using System.Data;

namespace IntugentBackend.Services.Rnd
{
    public class RNDRValues
    {
        public int nComps { get; set; } = Params.nComps;
        public int nForms { get; set; } = Params.nFormMax;
        public CRCalc RCalc { get; set; } = new CRCalc();
        public CRData RData { get; set; } = new CRData();
        public CLists CLists { get; set; }
        public CUConv CUConv { get; set; } = new CUConv();
        public DataTable dtGasComp { get; set; } = new DataTable();
        public DataView? gGasComp { get; set; }

        public string gMeasureTemp { get; set; } = string.Empty;
        public string gCellSize { get; set; } = string.Empty;
        public string gCellPress { get; set; } = string.Empty;
        public string gPolDen { get; set; } = string.Empty;
        public string gPolCond { get; set; } = string.Empty;
        public string gFracStruts { get; set; } = string.Empty;

        public string? gXAxisSelectedItem { get; set; }
        public string? gXAxisSelectedValue { get; set; }
        public string? gYAxisSelectedItem { get; set; }
        public string? gYAxisSelectedValue { get; set; }

        public double[] dAr0 = Array.Empty<double>();
        public double[] dAr1 = Array.Empty<double>();
        public double[] dAr2 = Array.Empty<double>();
        public double[] dAr3 = Array.Empty<double>();
        public double[] dAr4 = Array.Empty<double>();
        public double[] dArX = Array.Empty<double>();

        public RNDRValues(CLists cLists)
        {
            CLists = cLists ?? throw new ArgumentNullException(nameof(cLists));
            InitializeDataTable();
        }

        private void InitializeDataTable()
        {
            dtGasComp.Columns.Add("GasName", typeof(string));
            for (int i = 1; i <= nForms; i++)
            {
                dtGasComp.Columns.Add("#" + i, typeof(double));
            }
            for (int i = 0; i < nComps; i++) dtGasComp.Rows.Add();
        }
    }
}