using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntugentBackend.Services.Core;

namespace IntugentBackend.Services.Admin
{
    public class MfgAdmin
    {
       public DataTable dt = new DataTable(), dtCopy = new DataTable(), dtPr = new DataTable();
       public SqlDataAdapter da, daPr; DataRow dr;
       public string sql;
        public Cbfile Cbfile;
        public MfgAdmin(Cbfile cbfile) {
            Cbfile = cbfile;
        }
       public void UpdateDataSet()
        {
            string sMsg = "Coult not save to the server";
            try
            {
                SqlCommandBuilder sb = new SqlCommandBuilder(da);
                sb.ConflictOption = ConflictOption.OverwriteChanges;
                int v = da.Update(dt);
            }
            catch (Exception ex)
            {
               // MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
               // sMsg = "Could not save the InProcess dataset " + Cbfile.iIDMfg.ToString();
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
              //  CTelClient.TelException(ex, sMsg);
                return;
            }

           // CStatusBar.SetText("Data Saved at " + DateTime.Now.ToString("hh:mm:ss:tt"));

        }
    }
}
