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
    public class MfgReports
    {
        string sSqlQuery;
        SqlDataAdapter da;
       public DataTable dt = new DataTable();
        DataRow dr;
       public bool binit = true;
        public Cbfile Cbfile { get; set; }
        public CDefualts CDefaults {  get; set; }
        public MfgReports(Cbfile cbfile, CDefualts cDefaults)
        {
            Cbfile = cbfile;
            CDefaults = cDefaults;
        }
        public bool MfgReport(DateTime gReportDate)
    {
        string sDate1, sDate2;
        sDate1 = "'" + gReportDate.ToString() + "'";
        sDate2 = "'" + (((DateTime)gReportDate).AddDays(1)).ToString() + "'";
        string sqlBase = "Select RN.[ID4ALL FG] as ID, R1.Location, Format(R1.[Test Date], 'G') as 'Mfg_Time', Format(R1.[Test Date], 'MM/dd/yyyy') as 'Mfg_Date', R1.[Test Time] as 'MfgDateTime','' as PassFail, RN.[Next Day QC Collection Time FG] as 'QC_Test_Date' , R1.[Product ID],R1.[Product ID]+' - ' +  R2.[Product Description] as 'Product' , RN.[Notes FG] as 'Note', RN.[QC Test Passed] as 'QC_Passed', RN.[Thickness Avg FG] as 'Thickness',  RN.[R Value - AVG FG] as 'R_Value',  RN.[Compressive Strength (5) FG] as 'Comp_Strength', RN.[FG Core Density] as 'Core_Dens',' ' as 'Thickness_P', ' ' as 'R_Value_P', ' ' as 'CS_P', ' ' as 'Core_Dens_P', ' ' as 'Length_P', ' ' as 'Width_P', ' ' as 'Squareness_P',  R1.Length, R1.Width, R1.[IP Diagonal Diff] as 'Squareness', R3.[Product type] as 'Product_Type', R3.* from dbo.[Finished Goods] as RN Join dbo.[In Process Identify] as R1 on R1.ID4ALL = RN.[ID4ALL FG] Join dbo.[Product Matrix] as R2 on R2.[Product Code] = R1.[Product ID] join dbo.[Product Targets] as R3 on R3.[Product Code Global] = R2.[Product Code Global] where R1.Location = " + CDefaults.IDLocation;

        if (gReportDate != null) sqlBase += " and RN.[Next Day QC Collection Time FG] <  " + sDate2 + "  and RN.[Next Day QC Collection Time FG]  >= " + sDate1;
        sSqlQuery = sqlBase + "Order by R1.[Test Date] Desc";

        try
        {
            da = new SqlDataAdapter(sSqlQuery, Cbfile.conAZ);
            dt.Clear();
            int itmp = da.Fill(dt);

            bool bb = true;
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                if (dt.Rows[i]["QC_Test_Date"] == DBNull.Value) continue;

                if (dt.Rows[i]["QC_Passed"] == DBNull.Value) dt.Rows[i]["PassFail"] = "Fail";
                else if ((bool)dt.Rows[i]["QC_Passed"]) dt.Rows[i]["PassFail"] = "Pass";
                else dt.Rows[i]["PassFail"] = "Fail";

                if (dt.Rows[i]["Thickness"] == DBNull.Value) dt.Rows[i]["Thickness_P"] = "";
                //                   if (dt.Rows[i]["Thickness"] == DBNull.Value) dt.Rows[i]["Thickness_P"] = "F";
                else
                {
                    if (dt.Rows[i]["Thickness lower limit (US)"] != DBNull.Value)
                        if ((double)dt.Rows[i]["Thickness"] < (double)dt.Rows[i]["Thickness lower limit (US)"]) { dt.Rows[i]["Thickness_P"] = "F"; }

                    if (dt.Rows[i]["Thickness upper limit (US)"] != DBNull.Value)
                        if ((double)dt.Rows[i]["Thickness"] > (double)dt.Rows[i]["Thickness upper limit (US)"]) dt.Rows[i]["Thickness_P"] = "F";
                }

                if (dt.Rows[i]["Comp_Strength"] == DBNull.Value) dt.Rows[i]["CS_P"] = "";
                //                   if (dt.Rows[i]["Comp_Strength"] == DBNull.Value) dt.Rows[i]["CS_P"] = "F";
                else
                {
                    if (dt.Rows[i]["Compression lower limit"] != DBNull.Value)
                        if ((double)dt.Rows[i]["Comp_Strength"] < (double)dt.Rows[i]["Compression lower limit"]) { dt.Rows[i]["CS_P"] = "F"; }
                }

                if (dt.Rows[i]["R_Value"] == DBNull.Value) dt.Rows[i]["R_Value_P"] = "";
                //                   if (dt.Rows[i]["R_Value"] == DBNull.Value) dt.Rows[i]["R_Value_P"] = "F";
                else
                {
                    if (dt.Rows[i]["R-value limit per inch (aged)"] != DBNull.Value)
                        if ((double)dt.Rows[i]["R_Value"] < (double)dt.Rows[i]["R-value limit per inch (aged)"]) { dt.Rows[i]["R_Value_P"] = "F"; }
                }

                if (dt.Rows[i]["Core_Dens"] == DBNull.Value) dt.Rows[i]["Core_Dens_P"] = "";
                //                    if (dt.Rows[i]["Core_Dens"] == DBNull.Value) dt.Rows[i]["Core_Dens_P"] = "F";
                else
                {
                    if (dt.Rows[i]["Core density MIN"] != DBNull.Value)
                        if ((double)dt.Rows[i]["Core_Dens"] < (double)dt.Rows[i]["Core density MIN"]) { dt.Rows[i]["Core_Dens_P"] = "F"; }
                }

                if (dt.Rows[i]["Width"] == DBNull.Value) dt.Rows[i]["Width_P"] = "";
                //                    if (dt.Rows[i]["Width"] == DBNull.Value) dt.Rows[i]["Width_P"] = "F";
                else
                {
                    if (dt.Rows[i]["Width lower limit (US)"] != DBNull.Value)
                        if ((double)dt.Rows[i]["Width"] < (double)dt.Rows[i]["Width lower limit (US)"]) { dt.Rows[i]["Width_P"] = "F"; }

                    if (dt.Rows[i]["Width upper limit (US)"] != DBNull.Value)
                        if ((double)dt.Rows[i]["Width"] > (double)dt.Rows[i]["Width upper limit (US)"]) dt.Rows[i]["Width_P"] = "F";
                }

                if (dt.Rows[i]["Length"] == DBNull.Value) dt.Rows[i]["Length_P"] = "";
                //                    if (dt.Rows[i]["Length"] == DBNull.Value) dt.Rows[i]["Length_P"] = "F";
                else
                {
                    if (dt.Rows[i]["Length lower limit (US)"] != DBNull.Value)
                        if ((double)dt.Rows[i]["Length"] < (double)dt.Rows[i]["Length lower limit (US)"]) { dt.Rows[i]["Length_P"] = "F"; }

                    if (dt.Rows[i]["Length upper limit (US)"] != DBNull.Value)
                        if ((double)dt.Rows[i]["Length"] > (double)dt.Rows[i]["Length upper limit (US)"]) dt.Rows[i]["Length_P"] = "F";
                }


                if (dt.Rows[i]["Squareness"] == DBNull.Value) dt.Rows[i]["Squareness_P"] = "";
                //                    if (dt.Rows[i]["Squareness"] == DBNull.Value) dt.Rows[i]["Squareness_P"] = "F";
                else
                {

                    if (dt.Rows[i]["Squareness limit MAX"] != DBNull.Value)
                        if ((double)dt.Rows[i]["Squareness"] > (double)dt.Rows[i]["Squareness limit MAX"]) dt.Rows[i]["Squareness_P"] = "F";
                }

            //    gReport.ItemsSource = dt.DefaultView;

            }
        }
        catch (SqlException ex)
        {
            string sMsg = "Error in searching the Mfg datasets for Reports";
          //  MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
            System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
          //  CTelClient.TelException(ex, sMsg);
            return false;
        }


        //            MessageBox.Show(itmp.ToString());

        return true;
    }
}
}