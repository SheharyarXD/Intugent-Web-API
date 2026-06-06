using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntugentBackend.Services.Core;

namespace IntugentBackend.Services.Mfg
{
    public class MfgProcessCheck
    {
     //   System.Windows.Media.Brush backColorCal, backColor, backColorWarn;
       public SqlDataAdapter da = null;
        public DataTable dt;
       public DataRow dr = null;
       public string sSqlQuery = string.Empty;
       public string sMsgData = string.Empty;
       public string sFormat = "0.000";
       public int drIndex;
        public Cbfile cbfile {  get; set; }
        public CDefualts CDefault {  get; set; }
        public MfgProcessCheck(Cbfile cbfile,CDefualts cDefualts) { 
            this.cbfile = cbfile;
            this.CDefault=cDefualts;

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
                sMsg = "Could not save the InProcess dataset " + cbfile.iIDMfg.ToString();
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
              //  CTelClient.TelException(ex, sMsg);
                return;
            }

           // CStatusBar.SetText("Data Saved at " + DateTime.Now.ToString("hh:mm:ss:tt"));

        }
    }
}
