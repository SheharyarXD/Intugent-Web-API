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