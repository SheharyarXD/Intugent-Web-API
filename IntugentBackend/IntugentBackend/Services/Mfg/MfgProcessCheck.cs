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

        public bool GetDataSet()
        {
            try
            {
                sSqlQuery = "SELECT top(1000) * FROM [dbo].[Process Check] where IDLocation = " + CDefault.IDLocation + " order by ID Desc";
                da = new SqlDataAdapter(sSqlQuery, cbfile.conAZ);

                if (dt == null) dt = new DataTable(); else dt.Clear();
                int itmp = da.Fill(dt);
                if (itmp < 1) return false;

                drIndex = 0;
                dr = dt.Rows[0];
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error retrieving Process Check data\n\n" + ex.Message);
                return false;
            }
        }

        public string SetDoubleTextField(string sField, string sForm = "")
        {
            if (dr == null || dr[sField] == DBNull.Value) return string.Empty;
            return ((double)dr[sField]).ToString(sForm);
        }

        public void SetDoubleFieldValue(string? value, string sField)
        {
            if (dr == null) return;
            if (string.IsNullOrEmpty(value)) { dr[sField] = DBNull.Value; return; }
            if (double.TryParse(value, out double dtmp)) dr[sField] = dtmp;
        }

        public void SetIntFieldValue(string? value, string sField)
        {
            if (dr == null) return;
            if (string.IsNullOrEmpty(value)) { dr[sField] = DBNull.Value; return; }
            if (int.TryParse(value, out int itmp)) dr[sField] = itmp;
        }

        public void SetStringFieldValue(string? value, string sField)
        {
            if (dr == null) return;
            dr[sField] = string.IsNullOrEmpty(value) ? (object)DBNull.Value : value;
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
