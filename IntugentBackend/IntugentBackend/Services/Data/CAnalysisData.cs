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
    public class CAnalysisData
    {
        public  DataTable dtLocations, dtGlobalProducts, dtProps, dtPropValues = new DataTable(), dtCorr = new DataTable();

         DataRow dr;
         SqlDataAdapter da;
         DataTable dt = new DataTable();
        public  DateTime refDate;
        public Cbfile cbfile;
        public CDefualts CDefualts;
        public CAnalysisData(Cbfile cbfile,CDefualts cDefualts)
        {
            this.cbfile = cbfile;
            this.CDefualts = cDefualts;
            dtCorr.Columns.Add("PropName", typeof(string));
            dtCorr.Columns.Add("CorrValue", typeof(double));
            dtCorr.Columns.Add("AbsValue", typeof(double));
        }
        public  void GetLists()
        {
            try
            {
                string sql = "Select CONCAT([Product Code Global],' - ', [Item Description]) as GProd, [Product Code Global] from [Product Targets]  order by GProd";
                da = new SqlDataAdapter(sql, cbfile.conAZ);
                if (dtGlobalProducts == null) dtGlobalProducts = new DataTable(); else dtGlobalProducts.Clear();
                int itmp = da.Fill(dtGlobalProducts);
                dr = dtGlobalProducts.NewRow();
                dr["Product Code Global"] = dr["GProd"] = CDefualts.sProdMfgAll;
                dtGlobalProducts.Rows.InsertAt(dr, 0);

                sql = "select sLocation, [Group]  from tblLocations where [Group] like '%_Mfg_%' order by sLocation";
                da = new SqlDataAdapter(sql, cbfile.conAZ);
                if (dtLocations == null) dtLocations = new DataTable(); else dtLocations.Clear();
                itmp = da.Fill(dtLocations);
                dr = dtLocations.NewRow();
                dr["sLocation"] = dr["Group"] = "All Locations";
                dtLocations.Rows.InsertAt(dr, 0);



                sql = "select PropName, CONCAT(PropTable,'.',PropField) as PropField,PropField as PropField1   from tblProperties order by PropName";
                da = new SqlDataAdapter(sql, cbfile.conAZ);
                if (dtProps == null) dtProps = new DataTable(); else dtProps.Clear();
                itmp = da.Fill(dtProps);
            }
            catch (Exception e)
            {
              //  MessageBox.Show(e.Message);
            }
        }
        public void CorrMatrix(string scn1)
        {
            int ncount = 0, itmp;
            double dsum, dsum1, dsum2, dAvg1, dAvg2, dtmp, dtmp1, dtmp2, dCovar, dVar1, dVar2;
            string scn2;

            dtCorr.Rows.Clear();
            DataColumn c1 = dtPropValues.Columns[scn1]; if (c1 == null) return;
            if (c1.DataType.ToString().Contains("String")) return;

            foreach (DataColumn c in dtPropValues.Columns)
            {
                ncount = 0;
                dsum1 = dsum2 = 0;
                if (c.DataType.ToString().Contains("String")) continue;

                scn2 = c.ColumnName.ToString();

                for (int i = 0; i < dtPropValues.Rows.Count; i++)
                {
                    if (dtPropValues.Rows[i][scn1] == DBNull.Value || dtPropValues.Rows[i][scn2] == DBNull.Value) continue;
                    if (double.IsNaN((double)dtPropValues.Rows[i][scn1]) || double.IsNaN((double)dtPropValues.Rows[i][scn2])) continue;
                    dsum1 += (double)dtPropValues.Rows[i][scn1]; dsum2 += (double)dtPropValues.Rows[i][scn2]; ncount++;
                }
                if (ncount == 0) continue;
                dAvg1 = dsum1 / (double)ncount; dAvg2 = dsum2 / (double)ncount;

                ncount = 0;
                dsum = dsum1 = dsum2 = 0;
                dCovar = dVar1 = dVar2 = 0.0;

                for (int i = 0; i < dtPropValues.Rows.Count; i++)
                {
                    if (dtPropValues.Rows[i][scn1] == DBNull.Value || dtPropValues.Rows[i][scn2] == DBNull.Value) continue;
                    if (double.IsNaN((double)dtPropValues.Rows[i][scn1]) || double.IsNaN((double)dtPropValues.Rows[i][scn2])) continue;

                    dtmp1 = (double)dtPropValues.Rows[i][scn1] - dAvg1;
                    dtmp2 = (double)dtPropValues.Rows[i][scn2] - dAvg2;

                    dCovar += dtmp1 * dtmp2;
                    dVar1 += dtmp1 * dtmp1;
                    dVar2 += dtmp2 * dtmp2;

                }
                if (dVar1 * dVar2 > 0)
                {
                    dtmp = dCovar / Math.Sqrt(dVar1 * dVar2);
                    dtCorr.Rows.Add();
                    itmp = dtCorr.Rows.Count - 1;
                    dtCorr.Rows[itmp]["PropName"] = scn2;
                    dtCorr.Rows[itmp]["CorrValue"] = dtmp;
                    dtCorr.Rows[itmp]["AbsValue"] = Math.Abs(dtmp);
                }
            }
        }


        public  void GetData(string sql1)
        {
            DataTable dt = new DataTable();
            string stmp;
            double dtmp;
            DateTime ddtmp;
            //            string sFields = dtProps.Rows[0]["PropTable"].ToString() + "." + dtProps.Rows[0]["PropField"].ToString();
            //            for (int i = 0; i < dtProps.Rows.Count; i++) sFields += ", " + dtProps.Rows[i]["PropTable"].ToString() + "." + dtProps.Rows[i]["PropField"].ToString();


            try
            {
                string sFields = "Select " + dtProps.Rows[0]["PropField"].ToString() + " AS '" + dtProps.Rows[0]["PropName"].ToString() + "' ";
                for (int i = 1; i < dtProps.Rows.Count; i++) sFields += ", " + dtProps.Rows[i]["PropField"].ToString() + " AS '" + dtProps.Rows[i]["PropName"].ToString() + "' ";

                string sql = sFields + ", tblLocations.sLocation, [Product Matrix].[Product Code Global] FROM [In Process Identify] " +
                " Left Join [Product Matrix] on [In Process Identify].[Product ID] = [Product Matrix].[Product Code] Left Join [Finished Goods] on [In Process Identify].[ID4all] = [Finished Goods].[ID4ALL FG] " +
                " Left Join [Dimensional Stability] on [In Process Identify].[ID4all] = [Dimensional Stability].[ID4all] Left Join [Process Data] on [In Process Identify].[ID4all] = [Process Data].[ID4all] " +
                " Left join tblLocations on [In Process Identify].Location = tblLocations.ID Left join Roster on [In Process Identify].Operator = Roster.ID ";
  
                if (!string.IsNullOrEmpty(sql1)) sql = sql + " Where " + sql1;
                sql += "Order by [Test Date]";

                da = new SqlDataAdapter(sql, cbfile.conAZ);
                // if (dtPropValues == null) dtPropValues = new DataTable(); else dtPropValues.Clear();
                int itmp = da.Fill(dt);
                Console.WriteLine("SQL Query: " + sql);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
              //  MessageBox.Show(e.Message);
            }
            dtPropValues.Columns.Clear();
            dtPropValues.Clear();
            refDate = DateTime.Today;
            for (int i = 0; i < dt.Rows.Count; i++) dtPropValues.Rows.Add();
            foreach (DataColumn c in dt.Columns)
            {
                stmp = c.ColumnName.ToString();
                //                MessageBox.Show(c.DataType.ToString());
                if (c.DataType.ToString().Contains("Double") || c.DataType.ToString().Contains("Int"))
                {
                    dtPropValues.Columns.Add(stmp, typeof(double));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][stmp] == DBNull.Value) dtPropValues.Rows[i][stmp] = double.NaN;
                        else if (double.TryParse(dt.Rows[i][stmp].ToString(), out dtmp)) dtPropValues.Rows[i][stmp] = dtmp;
                        else dtPropValues.Rows[i][stmp] = double.NaN;
                    }
                }
                else if (c.DataType.ToString().Contains("DateTime"))
                {
                    dtPropValues.Columns.Add(stmp, typeof(double));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][stmp] == DBNull.Value) dtPropValues.Rows[i][stmp] = double.NaN;
                        else if (DateTime.TryParse(dt.Rows[i][stmp].ToString(), out ddtmp)) dtPropValues.Rows[i][stmp] = (ddtmp - refDate).TotalDays;
                        else dtPropValues.Rows[i][stmp] = double.NaN;
                    }

                }

                else if (c.DataType.ToString().Contains("String"))
                {
                    dtPropValues.Columns.Add(stmp, typeof(string));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][stmp] == DBNull.Value) dtPropValues.Rows[i][stmp] = string.Empty;
                        else dtPropValues.Rows[i][stmp] = dt.Rows[i][stmp].ToString();
                    }

                }

                //               foreach (DataColumn c in dtPropValues.Columns)
                //                   MessageBox.Show(c.ColumnName.ToString() + "\n" + ColAvg(c.ColumnName.ToString()).ToString("0.0###"));
            }
        }
    }


}
