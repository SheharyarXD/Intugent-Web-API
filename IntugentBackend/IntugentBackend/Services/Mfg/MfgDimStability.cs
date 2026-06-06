using IntugentBackend.Services.Core;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Mfg
{
    public class MfgDimStability
    {
        string sSqlQuery;
        SqlDataAdapter da;
        DataTable dt = new DataTable();
        public DataRow dr, drIP, drFG;
        public bool bDataSetChanged = false;
        public const string sDef = "", sOr = "0.000";
        string sNull = " ", sTimeFormat = "hh:mm tt", sDateTimeFormat = "MM/dd/yyyy - hh:mm tt";
        public Cbfile cBfile;
        public MfgDimStability(Cbfile CBfile)
        {
            this.cBfile = CBfile;
        }


        public void GetDataSet()
        {
            string sMsg;
            try
            {
                sSqlQuery = "Select * from [Dimensional Stability] where [ID4All]=" + cBfile.iIDMfg.ToString(); //1943";  //3137
                da = new SqlDataAdapter(sSqlQuery, cBfile.conAZ);

                dt.Clear();
                int itmp = da.Fill(dt);
                if (itmp < 1)
                {
                    sMsg = "Could not find the Dimension Stability Data for the Selected Dataset";
                    //MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                    System.Diagnostics.Trace.TraceError(sMsg);
                   // CPages.PageMfgHome_1.MfgDataNotFound();
                    return;

                }
                dr = dt.Rows[0];
            }
            catch (SqlException ex)
            {
                sMsg = "Error in retrieving the Dimensional Stability Data for the Selected Dataset";
                //MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //CPages.PageMfgHome_1.MfgDataNotFound();
                //CTelClient.TelException(ex, sMsg);
                return;
            }
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
                sMsg = "Could not save the Dimensional Stability dataset " + cBfile.iIDMfg.ToString();
               // System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
               // CTelClient.TelException(ex, sMsg);

                return;
            }

          //  CStatusBar.SetText("Data Saved at " + DateTime.Now.ToString("hh:mm:ss tt"));

        }
        public string SetDoubleTextField(string sField, string sForm = "xxx")
        {
            string s = String.Empty;
            if (sForm == "xxx") sForm = string.Empty;
            if (!(dr[sField] == DBNull.Value)) s = ((double)dr[sField]).ToString(sForm);

            return s;
        }
    }
    }
