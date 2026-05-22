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
    public class CProdTargets
    {
        public DataTable dt;
        public DataRow dr;
        public SqlDataAdapter da;
        public bool bTargetsObtained = false;
        public string sqlBase = " Select RN.ID4ALL, RN.[Product ID], R1.[Product Code Global], R2.* from dbo.[In Process Identify] as RN Join dbo.[Product Matrix] as R1 on R1.[Product Code] = RN.[Product ID] Join dbo.[Product Targets] as R2 on R1.[Product Code Global]  = R2.[Product Code Global] where RN.ID4ALL = ";
        public Cbfile cbfile;
        public CDefualts cDefualts;
        public CProdTargets(Cbfile cbfile, CDefualts cDefualts)
        {
            this.cbfile = cbfile;
            this.cDefualts = cDefualts;
        }
        public bool ProdTargetValue(string sVar, out double dVal)
        {
            string sql = string.Empty, sMsg = string.Empty;
            bool bNew = false;// bXLim = false;
                              //           double dtmp;
            dVal = double.PositiveInfinity;

            if (cbfile.iIDMfg < 1) return false;
            if (sVar == "") return false;

            if (dr == null) bNew = true;
            else if (dr["ID4ALL"] == DBNull.Value) bNew = true;
            else if ((int)dr["ID4ALL"] != cbfile.iIDMfg) bNew = true;

            try
            {
                if (bNew)
                {
                    sql = sqlBase + cbfile.iIDMfg.ToString(); //1943";  //3137
                    da = new SqlDataAdapter(sql, cbfile.conAZ);

                    if (dt == null) dt = new DataTable(); else dt.Clear();
                    int itmp = da.Fill(dt);
                    if (itmp < 1) { dr = null; return false; }
                    else dr = dt.Rows[0];
                }
                if (dr[sVar] == DBNull.Value) return false;
                dVal = (double)dr[sVar];
            }
            catch (SqlException ex)
            {
                sMsg = "Error in retrieving the Product Targets for dataset " + cbfile.iIDMfg.ToString();
                //MessageBox.Show(sMsg, cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //CPages.PageMfgHome_1.MfgDataNotFound();
                //CTelClient.TelException(ex, sMsg);
                return false;
            }
            return true;
        }
        public bool WithinLimits(string sValue)
        {
            double dtmp, dValue;
            if (!double.TryParse(sValue, out dValue)) return false;

            if (ProdTargetValue("Thickness lower limit", out dtmp)) if (dValue < dtmp) return false;
            if (ProdTargetValue("Thickness upper limit", out dtmp)) if (dValue > dtmp) return false;

            return true;
        }

        public bool GetProductTargets()
        {
            /*
             * function must be run on loading a page or anytime user changes the recordset or change the product in the recordset
            */
            string sql = string.Empty, sMsg = string.Empty;
            //          bool bNew = false, bXLim = false;
            //           double dtmp;

            bTargetsObtained = false;
            if (cbfile.iIDMfg < 1) { return false; }

            try
            {
                sql = sqlBase + cbfile.iIDMfg.ToString(); //1943";  //3137
                da = new SqlDataAdapter(sql, cbfile.conAZ);
                if (dt == null) dt = new DataTable(); else dt.Clear();
                int itmp = da.Fill(dt);
                if (itmp < 1) return false;
                else dr = dt.Rows[0];
                bTargetsObtained = true;
            }
            catch (SqlException ex)
            {
                sMsg = "Error in retrieving the Product Targets for dataset " + cbfile.iIDMfg.ToString();
                //MessageBox.Show(sMsg, cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //CPages.PageMfgHome_1.MfgDataNotFound();
                //CTelClient.TelException(ex, sMsg);
                return false;
            }
            return true;
        }

        public string ThicknessWithinLimits(double th)
        {
            //            double dval;
            if (!bTargetsObtained)
                if (!GetProductTargets()) return string.Empty;

            if (dr["Thickness lower limit (US)"] == DBNull.Value) return string.Empty;
            else if (th < (double)dr["Thickness lower limit (US)"]) return "N";

            if (dr["Thickness upper limit (US)"] == DBNull.Value) return string.Empty;
            else if (th > (double)dr["Thickness upper limit (US)"]) return "N";

            return "Y";
        }

        public string CompressionWithinLimits(double th)
        {
            //            double dval;
            if (!bTargetsObtained)
                if (!GetProductTargets()) return string.Empty;

            if (dr["Compression lower limit"] == DBNull.Value) return string.Empty;
            else if (th < (double)dr["Compression lower limit"]) return "N";
            return "Y";
        }

        public string LTTRWithinLimits(double th)
        {
            double dval;
            if (!bTargetsObtained)
                if (!GetProductTargets()) return string.Empty;

            if (dr["LTTR limit (aged)"] == DBNull.Value) return string.Empty;
            else if (th < (double)dr["LTTR limit (aged)"]) return "N";
            return "Y";
        }

        public string RValueAged90DWithinLimits(double th)
        {
            double dval;
            if (!bTargetsObtained)
                if (!GetProductTargets()) return string.Empty;

            if (dr["R-value limit per inch (aged)"] == DBNull.Value) return string.Empty;
            else if (th < (double)dr["R-value limit per inch (aged)"]) return "N";

            return "Y";
        }

        public string CoreDensWithinLimits(double th)
        {
            double dval;
            if (!bTargetsObtained)
                if (!GetProductTargets()) return string.Empty;

            if (dr["Core density MIN"] == DBNull.Value) return string.Empty;
            else if (th < (double)dr["Core density MIN"]) return "N";

            return "Y";
        }

        public  string WidthWithinLimits(double th)
        {
            double dval;
            if (!bTargetsObtained)
                if (!GetProductTargets()) return string.Empty;

            if (dr["Width lower limit (US)"] == DBNull.Value) return string.Empty;
            else if (th < (double)dr["Width lower limit (US)"]) return "N";

            if (dr["Width upper limit (US)"] == DBNull.Value) return string.Empty;
            else if (th > (double)dr["Width upper limit (US)"]) return "N";

            return "Y";
        }

        public  string LengthWithinLimits(double th)
        {
            double dval;
            if (!bTargetsObtained)
                if (!GetProductTargets()) return string.Empty;

            if (dr["Length lower limit (US)"] == DBNull.Value) return string.Empty;
            else if (th < (double)dr["Length lower limit (US)"]) return "N";

            if (dr["Length upper limit (US)"] == DBNull.Value) return string.Empty;
            else if (th > (double)dr["Length upper limit (US)"]) return "N";

            return "Y";
        }

        public  string SquarenessWithinLimits(double th)
        {
            double dval;
            if (!bTargetsObtained)
                if (!GetProductTargets()) return string.Empty;

            if (dr["Squareness limit MAX"] == DBNull.Value) return string.Empty;
            else if (th > (double)dr["Squareness limit MAX"]) return "N";

            return "Y";
        }

        public  string FacerPeelWithinLimits(double th)
        {
            double dval;
            if (!bTargetsObtained)
                if (!GetProductTargets()) return string.Empty;

            if (dr["Facer peel limit MIN"] == DBNull.Value) return string.Empty;
            else if (th < (double)dr["Facer peel limit MIN"]) return "N";

            return "Y";
        }

        public  string FacerAlignWithinLimits(double th)
        {
            double dval;
            if (!bTargetsObtained)
                if (!GetProductTargets()) return string.Empty;

            if (dr["Facer alignment MAX"] == DBNull.Value) return string.Empty;
            else if (th > (double)dr["Facer alignment MAX"]) return "N";

            return "Y";
        }

        public  string DimStabLengthWithinLimits(double th, string st)
        {

            double dmin = double.MaxValue;
            double dmax = double.MinValue;

            if (!bTargetsObtained)
                if (!GetProductTargets()) return string.Empty;

            switch (st)
            {
                case "cold":
                    if (dr["Dim stab cold Length (%) MIN"] == DBNull.Value) return string.Empty; else dmin = ((double)dr["Dim stab cold Length (%) MIN"]);
                    if (dr["Dim stab cold Length (%) MAX"] == DBNull.Value) return string.Empty; else dmax = ((double)dr["Dim stab cold Length (%) MAX"]);
                    break;
                case "hot":
                    if (dr["Dim stab hot Length (%) MIN"] == DBNull.Value) return string.Empty; else dmin = ((double)dr["Dim stab hot Length (%) MIN"]);
                    if (dr["Dim stab hot Length (%) MAX"] == DBNull.Value) return string.Empty; else dmax = ((double)dr["Dim stab hot Length (%) MAX"]);
                    break;
                default: return string.Empty;
            }

            if (th < dmin || th > dmax) return "N";
            return "Y";
        }


        public  string DimStabWidthWithinLimits(double th, string st)
        {
            double dmin = double.MaxValue;
            double dmax = double.MinValue;

            if (!bTargetsObtained)
                if (!GetProductTargets()) return string.Empty;

            switch (st)
            {
                case "cold":
                    if (dr["Dim stab cold Width (%) MIN"] == DBNull.Value) return string.Empty; else dmin = ((double)dr["Dim stab cold Width (%) MIN"]);
                    if (dr["Dim stab cold Width (%) MAX"] == DBNull.Value) return string.Empty; else dmax = ((double)dr["Dim stab cold Width (%) MAX"]);
                    break;
                case "hot":
                    if (dr["Dim stab hot Width (%) MIN"] == DBNull.Value) return string.Empty; else dmin = ((double)dr["Dim stab hot Width (%) MIN"]);
                    if (dr["Dim stab hot Width (%) MAX"] == DBNull.Value) return string.Empty; else dmax = ((double)dr["Dim stab hot Width (%) MAX"]);
                    break;
                default: return string.Empty;
            }

            if (th < dmin || th > dmax) return "N";
            return "Y";
        }


        public  string DimStabThicknessWithinLimits(double th, string st)
        {
            double dmin = double.MaxValue;
            double dmax = double.MinValue;

            if (!bTargetsObtained)
                if (!GetProductTargets()) return string.Empty;

            switch (st)
            {
                case "cold":
                    if (dr["Dim stab cold Thickness (%) MIN"] == DBNull.Value) return string.Empty; else dmin = ((double)dr["Dim stab cold Thickness (%) MIN"]);
                    break;
                case "hot":
                    if (dr["Dim stab hot Thickness (%) MIN"] == DBNull.Value) return string.Empty; else dmin = ((double)dr["Dim stab hot Thickness (%) MIN"]);
                    break;
                default: return string.Empty;
            }

            if (th < dmin) return "N";
            return "Y";
        }

        public  string BoardTimeStampsWithinLimits(DateTime dtIPTime, DateTime dtFGTime, string st)
        {
            DateTime Mindate = new DateTime(2000, 01, 01);

            string sMsg = "Green Board and FG Board time stamps must be greater than 01-01-2000 and must be within " + cDefualts.dDelTimeButton.ToString() + " minutes of each other";

            if (dtIPTime == null || dtIPTime <= Mindate) return "N";
            if (dtFGTime == null || dtFGTime <= Mindate) return "N";
            if (Math.Abs((dtIPTime - dtFGTime).TotalMinutes) > cDefualts.dDelTimeButton) return "NoProcData";
            return "Y";
        }

    }



}
