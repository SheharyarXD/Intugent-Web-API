using IntugentBackend.Services.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntugentBackend.Services.Data;

namespace IntugentBackend.Services.Rnd
{
    public class RNDRValues
    {
        public int nComps = Params.nComps;
        public int nForms = Params.nFormMax;
        public CRCalc RCalc = new CRCalc();
        public CRData RData = new CRData();
        public CLists CLists;
        public CUConv CUConv = new CUConv();
        public RNDRValues(CLists cLists)
        {
            this.CLists = cLists ?? throw new ArgumentNullException(nameof(cLists));


            Startup();
        }

        public DataTable dtGasComp = new DataTable();
       public int nForm = 5;

        public void Startup()
        {
            int idpt;

            nForm = Params.nFormMax;

            dtGasComp.Columns.Add("GasName", typeof(string));
            for (int i = 1; i < nForm + 1; i++)
            {
                dtGasComp.Columns.Add("#" + i, typeof(double));
            }
            for (int i = 0; i < Params.nComps; i++) dtGasComp.Rows.Add();
            try
            {
                if (CLists.drEmployee["sRValueParams"] == DBNull.Value) { }
                else
                {
                    string js1 = (string)CLists.drEmployee["sRValueParams"];
                    RData = (CRData)System.Text.Json.JsonSerializer.Deserialize(js1, typeof(CRData));
                }
            }
            catch {  }

        }
    }
}
