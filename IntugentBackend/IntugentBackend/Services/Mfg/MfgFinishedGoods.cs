using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntugentBackend.Services.Core;


namespace IntugentBackend.Services.Mfg
{
    public class MfgFinishedGoods
    {
        string sSqlQuery;
        SqlDataAdapter da;
        DataTable dt = new DataTable();
        public DataRow dr, drIP;
        public bool bDataSetChanged = false;
       // System.Windows.Media.Brush backColorCal, backColor, backColorRed, backColorEsc;
        public const string sDef = "", sOr = "0.000";
        string sNull = " ", sTimeFormat = "hh:mm tt", sDateTimeFormat = "MM/dd/yyyy - hh:mm tt";


        public Cbfile cBfile;

        public MfgFinishedGoods(Cbfile CBfile)
        {
            cBfile = CBfile;
           // InitializeComponent();
           // backColor = gThicknessFG_1.Background;
           // backColorCal = gThickness.Background;
           // //           backColorRed = System.Windows.Media.Brushes.HotPink;
           // backColorEsc = (System.Windows.Media.SolidColorBrush)new System.Windows.Media.BrushConverter().ConvertFromString("#FF8FC7");
           // backColorRed = (System.Windows.Media.SolidColorBrush)new System.Windows.Media.BrushConverter().ConvertFromString("#F6007B");
        }

        public void GetDataSet()
        {
            string sMsg;
            try
            {
                sSqlQuery = "Select * from [Finished Goods] where [ID4ALL FG]=" + cBfile.iIDMfg.ToString(); //1943";  //3137
                da = new SqlDataAdapter(sSqlQuery, cBfile.conAZ);

                dt.Clear();
                int itmp = da.Fill(dt);
                if (itmp < 1)
                {
                    sMsg = "Could not find the Finished Good Data for the Selected Dataset";
                   // MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                    System.Diagnostics.Trace.TraceError(sMsg);
                   // CPages.PageMfgHome_1.MfgDataNotFound();
                    return;
                }
                dr = dt.Rows[0];
            }
            catch (SqlException ex)
            {
                sMsg = "Error in retrieving the Finished Good Data for the Selected Dataset";
               // MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
               // CPages.PageMfgHome_1.MfgDataNotFound();
              //  CTelClient.TelException(ex, sMsg);
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
                sMsg = "Could not save the Finished Goods dataset " + cBfile.iIDMfg.ToString();
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
              //  CTelClient.TelException(ex, sMsg);
                return;
            }

          //  CStatusBar.SetText("Data Saved at " + DateTime.Now.ToString("hh:mm:ss tt"));
        }

        public string SetDoubleField(string value, string sField)
        {
            double dtmp;
            string sText = value;

            if (String.IsNullOrEmpty(sText)){
                dr[sField] = DBNull.Value;
                return null;
            }
            else
            {
                if (double.TryParse(sText, out dtmp))
                {
                    dr[sField] = dtmp;
                    return dtmp.ToString();
                }
                else {
                    if (dr[sField] == DBNull.Value)
                    {
                        value = string.Empty;
                        return value;
                    }

                    else
                    {
                        value= ((double)dr[sField]).ToString();
                        return value;

                    }
                }
            }

        }
        public string SetDoubleTextField(string sField, string sForm = sDef)
        {
            string s = String.Empty;
            if (!(dr[sField] == DBNull.Value)) s = ((double)dr[sField]).ToString(sForm);
            return s;
        }
        public string SetIntTextField(string sField)
        {
            string s = String.Empty;
            if (!(dr[sField] == DBNull.Value)) s = ((int)dr[sField]).ToString();
            return s;
        }
    }
}
