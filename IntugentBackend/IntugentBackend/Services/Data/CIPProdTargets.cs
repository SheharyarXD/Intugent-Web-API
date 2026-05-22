using Google.Api.Gax;
//using IntugentClassLbrary.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using IntugentClassLibrary.Pages.Mfg;

namespace IntugentBackend.Services.Data
{
    public  class CIPProdTargets
    {
        public DataTable dt;
        public DataRow dr;
        public SqlDataAdapter da;
        public bool bTargetsObtained = false;
        public Cbfile cbfile;
        public MfgInProcess MfgInProcess;
        public CIPProdTargets(Cbfile cbfile,MfgInProcess mfgInProcess)
        {
            this.cbfile = cbfile;
            this.MfgInProcess = mfgInProcess;
        }
        //        static string sqlBase = " Select RN.ID4ALL, RN.[Product ID], R1.[Product Code Global], R2.* from dbo.[In Process Identify] as RN Join dbo.[Product Matrix] as R1 on R1.[Product Code] = RN.[Product ID] Join dbo.[Product Targets] as R2 on R1.[Product Code Global]  = R2.[Product Code Global] where RN.ID4ALL = ";

        public string sqlBase = "select * from [dbo].[IP Product Targets] where [Product Code (Local)] = ";
        /*
                public static bool ProdTargetValue(string sVar, out double dVal)
                {
                    string sql = string.Empty, sMsg = string.Empty;
                    bool bNew = false, bXLim = false;
                    double dtmp;
                    dVal = double.PositiveInfinity;

                    if (Cbfile.iIDMfg < 1) return false;
                    if (sVar == "") return false;
           //         if(CPages)

                    if (dr == null) bNew = true;
                    else if (dr["ID4ALL"] == DBNull.Value) bNew = true;
                    else if ((int)dr["ID4ALL"] != Cbfile.iIDMfg) bNew = true;

                    try
                    {
                        if (bNew)
                        {
                            sql = sqlBase + Cbfile.iIDMfg.ToString(); //1943";  //3137
                            da = new SqlDataAdapter(sql, Cbfile.conAZ);

                            if (dt == null) dt = new DataTable(); else dt.Clear();
                            int itmp = da.Fill(dt);
                            if (itmp < 1) return false;
                            else dr = dt.Rows[0];
                        }
                        if (dr[sVar] == DBNull.Value) return false;
                        dVal = (double)dr[sVar];
                    }
                    catch (SqlException ex)
                    {
                        sMsg = "Error in retrieving the Product Targets for dataset " + Cbfile.iIDMfg.ToString();
                        MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                        System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                        CPages.PageMfgHome_1.MfgDataNotFound();
                        CTelClient.TelException(ex, sMsg);
                        return false;
                    }
                    return true;
                }
                public static bool WithinLimits(string sValue)
                {
                    double dtmp, dValue;
                    if (!double.TryParse(sValue, out dValue)) return false;

                    if (ProdTargetValue("Thickness lower limit", out dtmp)) if (dValue < dtmp) return false;
                    if (ProdTargetValue("Thickness upper limit", out dtmp)) if (dValue > dtmp) return false;

                    return true;
                }
        */
        public bool GetProductTargets()
        {
            /*
             * function must be run on loading a page or anytime user changes the recordset or change the product in the recordset
            */
            string sql = string.Empty, sMsg = string.Empty, sProd = string.Empty;
            bool bNew = false; // bXLim = false;
                               //        double dtmp;

            bTargetsObtained = false;
            if (cbfile.iIDMfg < 1) { return false; }
            else if (MfgInProcess.dr == null) return false;
            else if (MfgInProcess.dr["Product ID"] == DBNull.Value) return false;


            sProd = (string)MfgInProcess.dr["Product ID"];
            if (dr == null) bNew = true;
            else if (sProd == (string)dr["Product Code (Local)"]) return true;
            else bNew = true;

            try
            {
                sql = sqlBase + "'" + sProd + "'"; //1943";  //3137
                da = new SqlDataAdapter(sql, cbfile.conAZ);
                if (dt == null) dt = new DataTable(); else dt.Clear();
                int itmp = da.Fill(dt);
                if (itmp < 1) { dr = null; return false; }
                else dr = dt.Rows[0];
                bTargetsObtained = true;
            }
            catch (SqlException ex)
            {
                //sMsg = "Error in retrieving the Product Targets for dataset " + Cbfile.iIDMfg.ToString();
                //MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                //System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //CPages.PageMfgHome_1.MfgDataNotFound();
                //CTelClient.TelException(ex, sMsg);
                return false;
            }
            return true;
        }

        public string ThicknessWithinLimits(double th)
        {
            //            double dval;
            string sret = string.Empty;
            string sFBelowEsc = "IP Thickness At or Below - ESCALATE";
            string sFBelowRed = "IP Thickness At or Below - RED TAG";
            string sFAboveExc = "IP Thickness At or Above - ESCALATE";
            string sFAboveRed = "IP Thickness At or Above - RED TAG";

            if (!GetProductTargets()) return string.Empty;

            if (dr[sFAboveRed] != DBNull.Value && (double)dr[sFAboveRed] <= th) return "Red";
            else if (dr[sFBelowRed] != DBNull.Value && (double)dr[sFBelowRed] >= th) return "Red";
            else if (dr[sFAboveExc] != DBNull.Value && (double)dr[sFAboveExc] <= th) return "Esc";
            else if (dr[sFBelowEsc] != DBNull.Value && (double)dr[sFBelowEsc] >= th) return "Esc";

            return string.Empty;
        }
        public  string ThicknessAvg1WithinLimits(double th)
        {
            //            double dval;
            string sret = string.Empty;
            string sFBelowRed = "IP Thickness 1-8 At or Below - RED TAG";
            string sFBelowEsc = "IP Thickness 1-8 At or Below - ESCALATE";
            string sFAboveExc = "IP Thickness 1-8 At or Above - ESCALATE";
            string sFAboveRed = "IP Thickness 1-8 At or Above - RED TAG";

            if (!GetProductTargets()) return string.Empty;

            if (dr[sFAboveRed] != DBNull.Value && (double)dr[sFAboveRed] <= th) return "Red";
            else if (dr[sFBelowRed] != DBNull.Value && (double)dr[sFBelowRed] >= th) return "Red";
            else if (dr[sFAboveExc] != DBNull.Value && (double)dr[sFAboveExc] <= th) return "Esc";
            else if (dr[sFBelowEsc] != DBNull.Value && (double)dr[sFBelowEsc] >= th) return "Esc";

            return string.Empty;
        }
        public  string ThicknessAvg2WithinLimits(double th)
        {
            //            double dval;
            string sret = string.Empty;
            string sFBelowRed = "IP Thickness 10-17 At or Below - RED TAG";
            string sFBelowEsc = "IP Thickness 10-17 At or Below - ESCALATE";
            string sFAboveExc = "IP Thickness 10-17 At or Above - ESCALATE";
            string sFAboveRed = "IP Thickness 10-17 At or Above - RED TAG";

            if (!GetProductTargets()) return string.Empty;

            if (dr[sFAboveRed] != DBNull.Value && (double)dr[sFAboveRed] <= th) return "Red";
            else if (dr[sFBelowRed] != DBNull.Value && (double)dr[sFBelowRed] >= th) return "Red";
            else if (dr[sFAboveExc] != DBNull.Value && (double)dr[sFAboveExc] <= th) return "Esc";
            else if (dr[sFBelowEsc] != DBNull.Value && (double)dr[sFBelowEsc] >= th) return "Esc";

            return string.Empty;
        }
        public  string ThicknessSlopeWithinLimits(double th)
        {
            double dval;
            string sret = string.Empty;
            string sFBelowRed = "IP Thickness Slope At or Below - RED TAG";
            string sFBelowEsc = "IP Thickness Slope At or Below - ESCALATE";
            string sFAboveExc = "IP Thickness Slope At or Above - ESCALATE";
            string sFAboveRed = "IP Thickness Slope At or Above - RED TAG";



            if (!GetProductTargets()) return string.Empty;

            if (dr[sFAboveRed] != DBNull.Value && (double)dr[sFAboveRed] <= th) return "Red";
            else if (dr[sFBelowRed] != DBNull.Value && (double)dr[sFBelowRed] >= th) return "Red";
            else if (dr[sFAboveExc] != DBNull.Value && (double)dr[sFAboveExc] <= th) return "Esc";
            else if (dr[sFBelowEsc] != DBNull.Value && (double)dr[sFBelowEsc] >= th) return "Esc";

            return string.Empty;
        }
        public  string ThicknessProfileWithinLimits(double th)
        {
            double dval;
            string sret = string.Empty;
            string sFBelowRed = "IP Thickness Profile At or Below - RED TAG";
            string sFBelowEsc = "IP Thickness Profile At or Below - ESCALATE";
            string sFAboveExc = "IP Thickness Profile At or Above - ESCALATE";
            string sFAboveRed = "IP Thickness Profile At or Above - RED TAG";

            if (!GetProductTargets()) return string.Empty;
            if (dr[sFAboveRed] != DBNull.Value && (double)dr[sFAboveRed] <= th) return "Red";
            else if (dr[sFBelowRed] != DBNull.Value && (double)dr[sFBelowRed] >= th) return "Red";
            else if (dr[sFAboveExc] != DBNull.Value && (double)dr[sFAboveExc] <= th) return "Esc";
            else if (dr[sFBelowEsc] != DBNull.Value && (double)dr[sFBelowEsc] >= th) return "Esc";

            return string.Empty;
        }
        public  string BundleHeightWithinLimits(double th)
        {
            double dval;
            string sret = string.Empty;
            string sFBelowRed = "Bundle Height At or Below - RED TAG";
            string sFBelowEsc = "Bundle Height At or Below - ESCALATE";
            string sFAboveExc = "Bundle Height At or Above - ESCALATE";
            string sFAboveRed = "Bundle Height At or Above - RED TAG";

            if (!GetProductTargets()) return string.Empty;
            if (dr[sFAboveRed] != DBNull.Value && (double)dr[sFAboveRed] <= th) return "Red";
            else if (dr[sFBelowRed] != DBNull.Value && (double)dr[sFBelowRed] >= th) return "Red";
            else if (dr[sFAboveExc] != DBNull.Value && (double)dr[sFAboveExc] <= th) return "Esc";
            else if (dr[sFBelowEsc] != DBNull.Value && (double)dr[sFBelowEsc] >= th) return "Esc";

            return string.Empty;
        }
        public  string BoardLengthWithinLimits(double th)
        {
            //           double dval;
            string sret = string.Empty;
            string sFBelowRed = "IP Length At or Below - RED TAG";
            string sFBelowEsc = "IP Length At or Below - ESCALATE";
            string sFAboveExc = "IP Length At or Above - ESCALATE";
            string sFAboveRed = "IP Length At or Above - RED TAG";

            if (!GetProductTargets()) return string.Empty;
            if (dr[sFAboveRed] != DBNull.Value && (double)dr[sFAboveRed] <= th) return "Red";
            else if (dr[sFBelowRed] != DBNull.Value && (double)dr[sFBelowRed] >= th) return "Red";
            else if (dr[sFAboveExc] != DBNull.Value && (double)dr[sFAboveExc] <= th) return "Esc";
            else if (dr[sFBelowEsc] != DBNull.Value && (double)dr[sFBelowEsc] >= th) return "Esc";

            return string.Empty;
        }
        public  string BoardWidthWithinLimits(double th)
        {
            double dval;
            string sret = string.Empty;
            string sFBelowRed = "IP Width At or Below - RED TAG";
            string sFBelowEsc = "IP Width At or Below - ESCALATE";
            string sFAboveExc = "IP Width At or Above - ESCALATE";
            string sFAboveRed = "IP Width At or Above - RED TAG";

            if (!GetProductTargets()) return string.Empty;
            if (dr[sFAboveRed] != DBNull.Value && (double)dr[sFAboveRed] <= th) return "Red";
            else if (dr[sFBelowRed] != DBNull.Value && (double)dr[sFBelowRed] >= th) return "Red";
            else if (dr[sFAboveExc] != DBNull.Value && (double)dr[sFAboveExc] <= th) return "Esc";
            else if (dr[sFBelowEsc] != DBNull.Value && (double)dr[sFBelowEsc] >= th) return "Esc";

            return string.Empty;
        }
        public  string BoardSquarenessWithinLimits(double th)
        {
            double dval;
            string sret = string.Empty;
            //            string sFBelowRed = "IP Width At or Below - RED TAG";
            //            string sFBelowEsc = "IP Width At or Below - ESCALATE";

            string sFAboveExc = "IP Squareness At or Above - ESCALATE";
            string sFAboveRed = "IP Squareness At or Above - RED TAG";

            if (!GetProductTargets()) return string.Empty;
            if (dr[sFAboveRed] != DBNull.Value && (double)dr[sFAboveRed] <= th) return "Red";
            //            else if (dr[sFBelowRed] != DBNull.Value && (double)dr[sFBelowRed] >= th) return "Red";
            else if (dr[sFAboveExc] != DBNull.Value && (double)dr[sFAboveExc] <= th) return "Esc";
            //           else if (dr[sFBelowEsc] != DBNull.Value && (double)dr[sFBelowEsc] >= th) return "Esc";

            return string.Empty;
        }
        public  string CoreDensityWithinLimits(double th)
        {
            double dval;
            string sret = string.Empty;
            string sFBelowRed = "IP Core At or Below - RED TAG";
            string sFBelowEsc = "IP Core At or Below - ESCALATE";
            //            string sFAboveExc = "IP Squareness At or Above - ESCALATE";
            //            string sFAboveRed = "IP Squareness At or Above - RED TAG";

            if (!GetProductTargets()) return string.Empty;
            //            if (dr[sFAboveRed] != DBNull.Value && (double)dr[sFAboveRed] <= th) return "Red";
            if (dr[sFBelowRed] != DBNull.Value && (double)dr[sFBelowRed] >= th) return "Red";
            //           else if (dr[sFAboveExc] != DBNull.Value && (double)dr[sFAboveExc] <= th) return "Esc";
            else if (dr[sFBelowEsc] != DBNull.Value && (double)dr[sFBelowEsc] >= th) return "Esc";

            return string.Empty;
        }
        public  string CompressionStrWithinLimits(double th)
        {
            double dval;
            string sret = string.Empty;
            string sFBelowRed = "IP Compression At or Below - RED TAG";
            string sFBelowEsc = "IP Compression At or Below - ESCALATE";
            //            string sFAboveExc = "IP Squareness At or Above - ESCALATE";
            //            string sFAboveRed = "IP Squareness At or Above - RED TAG";

            if (!GetProductTargets()) return string.Empty;
            //            if (dr[sFAboveRed] != DBNull.Value && (double)dr[sFAboveRed] <= th) return "Red";
            if (dr[sFBelowRed] != DBNull.Value && (double)dr[sFBelowRed] >= th) return "Red";
            //           else if (dr[sFAboveExc] != DBNull.Value && (double)dr[sFAboveExc] <= th) return "Esc";
            else if (dr[sFBelowEsc] != DBNull.Value && (double)dr[sFBelowEsc] >= th) return "Esc";

            return string.Empty;
        }
    }

}
