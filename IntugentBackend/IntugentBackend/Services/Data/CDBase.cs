using Google.Api.Gax;
using IntugentBackend.Services.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Data
{
    public class CDBase
    {
        public static string sDBLocalConStr = @"Data Source=XD-1510\SQLEXPRESS; Initial Catalog= IntugentPI; Integrated Security=SSPI;";
        //        public static SqlConnection DBCon;
        public SqlDataAdapter da;
        public DataTable dt;
        public DataRow dr;
        public int IDModel = 1;
        public int IndexModel = 0;
        public string sSQL = "Select Top 20 * from dbo.tblAIModels";
        public Cbfile Cbfile;
        public CNNData CNNData;
        public CDBase(Cbfile cbfile)
        {
            this.Cbfile = cbfile;
            //CNNData = cNNData;
        }

        public bool SearchDatabase(string sSQLCustom)
        {
            string sMsg;
            if (sSQLCustom != string.Empty) sSQL = sSQLCustom;

            try
            {
                da = new SqlDataAdapter(sSQL, Cbfile.conAZ);
                dt = new DataTable();
                int itmp = da.Fill(dt);
                if (itmp < 1)
                {
                    sMsg = ("No AI Model was found to meet the search criteria.  Check the network connection and/or relax the search criteria");
                   // MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }
            catch (SqlException ex)
            {
                sMsg = "Error in searching the Mfg datasets for editing";
                //MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //                CTelClient.TelException(ex, sMsg);
                return false;
            }
            return true;

        }

        public void UpdateModel()
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
                sMsg = "Could not save the InProcess dataset " + IDModel.ToString();
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //                    CTelClient.TelException(ex, sMsg);
                return;
            }

           // CStatusBar.SetText("Data Saved at " + DateTime.Now.ToString("hh:mm:ss:tt"));

        }
        public  bool CreateNewModel()
        {

            string sMsg = "Could not create a new Mfg dataset.";
            int iDummy = 0, itmp;
            string sql = "Select Next Value for [dbo].[IDAIModels]";

            //            using (Cbfile.conAZ)
            {
                SqlCommand cmd = new SqlCommand(sql, Cbfile.conAZ);
                try
                {
                    Cbfile.conAZ.Open();
                    iDummy = (int)cmd.ExecuteScalar();
                    Cbfile.conAZ.Close();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                    sMsg = "Could not create a new sequence number for Mfg Dataset";
                    System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                    //                    CTelClient.TelException(ex, sMsg);
                    return false;
                }
            }

            try
            {
                dr = dt.NewRow();
                dr["ID"] = iDummy;
                dr["DateModel"] = DateTime.Now;
                dt.Rows.Add(dr);
                UpdateModel();
            }
            catch (Exception ex)
            {
              //  MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                sMsg = "Could not create a new Model with ID " + iDummy.ToString();
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //                CTelClient.TelException(ex, sMsg);
                return false;
            }

            IDModel = iDummy;
            IndexModel = 0;
            //CPages.PageModel_1.nnModel = new CNNModel();
           // CNNData.Reset();
            for (int i = 0; i < dt.Rows.Count; i++) if (IDModel == (int)dt.Rows[i]["ID"]) IndexModel = i;

            return true;
        }
    }

}
