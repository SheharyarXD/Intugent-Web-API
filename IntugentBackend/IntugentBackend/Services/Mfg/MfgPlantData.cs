using IntugentBackend.Services.Core;
using System;
using Google.Cloud.BigQuery.V2;
using Google.Apis.Auth.OAuth2;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntugentBackend.Services.Admin;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Rnd;

namespace IntugentBackend.Services.Mfg
{
    public class MfgPlantData
    {
        string sSqlQuery;
        SqlDataAdapter da, da2;
       public DataTable dt = new DataTable(), dtPP = new DataTable();
        public DataRow dr, drIP, drFG;
        public DataTable dtPPChemDel = new DataTable(), dtPPChemDel1 = new DataTable(), dtPPPTable = new DataTable(), dtPPDBelt = new DataTable(), dtPPOthers = new DataTable(), dtNewInsData = new DataTable();
        public bool bDataSetChanged = false;
        public DateTime dateTime1, dateTime2, dtIPTime, dtFGTime, dtQCCheckTime;

        public Cbfile cBfile;
        public CLists clist;
        public CDefualts cDefualts;
        public MfgPlantData(Cbfile CBfile, CLists clist,CDefualts cDefualts)
        {
            this.cDefualts= cDefualts;
            this.clist = clist;
            cBfile = CBfile;
            string sql = "SELECT * FROM [tblProcessParams]";
            da = new SqlDataAdapter(sql, cBfile.conAZ);
            int itmp = da.Fill(clist.dtProcessParams);

            clist.dvProcssParams = clist.dtProcessParams.DefaultView;
            clist.dvProcssParams.RowFilter = "sGroup = 'Chemical Delivery'";
            clist.dvPPChemDel = clist.dvProcssParams.ToTable().DefaultView;
            clist.dvProcssParams.RowFilter = "sGroup = 'Chemical Delivery 1'";
            clist.dvPPChemDel1 = clist.dvProcssParams.ToTable().DefaultView;

            clist.dvProcssParams.RowFilter = "sGroup = 'Pour Table'";
            clist.dvPPPTable = clist.dvProcssParams.ToTable().DefaultView;
            clist.dvProcssParams.RowFilter = "sGroup = 'Double Belt'";
            clist.dvPPDBelt = clist.dvProcssParams.ToTable().DefaultView;
            clist.dvProcssParams.RowFilter = "sGroup = 'New Instrument data - temp'";
            clist.dvNewInsData = clist.dvProcssParams.ToTable().DefaultView;

            clist.dvProcssParams.RowFilter = "sGroup NOT IN  ('Pour Table', 'Double Belt','Chemical Delivery', 'Chemical Delivery 1','New Instrument data - temp') ";
            clist.dvPPOthers = clist.dvProcssParams.ToTable().DefaultView;
        }


        public bool GetDataSet()
        {
            string sMsg, sn;

            if (dtPPChemDel != null) dtPPChemDel.Clear(); dtPPChemDel = clist.dvPPChemDel.ToTable();
            if (dtPPChemDel1 != null) dtPPChemDel1.Clear(); dtPPChemDel1 = clist.dvPPChemDel1.ToTable();
            if (dtPPPTable != null) dtPPPTable.Clear(); dtPPPTable = clist.dvPPPTable.ToTable();
            if (dtPPPTable != null) dtPPDBelt.Clear(); dtPPDBelt = clist.dvPPDBelt.ToTable();
            if (dtPPOthers != null) dtPPOthers.Clear(); dtPPOthers = clist.dvPPOthers.ToTable();
            if (dtNewInsData != null) dtNewInsData.Clear(); dtNewInsData = clist.dvNewInsData.ToTable();

            try
            {
                sSqlQuery = "Select * from [Process Data] where [ID4All]=" + cBfile.iIDMfg.ToString(); //1943";  //3137
                da = new SqlDataAdapter(sSqlQuery, cBfile.conAZ);

                dt.Clear();
                int itmp = da.Fill(dt);
                if (itmp < 1)
                {
                    sMsg = "Could not find the Process Data for the Selected Dataset";
                    //  MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                    System.Diagnostics.Trace.TraceError(sMsg);
                    //  CPages.PageMfgHome_1.MfgDataNotFound();
                    return false;

                }
                dr = dt.Rows[0];
                for (int ir = 0; ir < 2; ir++)
                {
                    sn = ((string)dtPPChemDel.Rows[ir]["sName"]);
                    if (dr[sn] == DBNull.Value) dtPPChemDel.Rows[ir]["sValue"] = string.Empty; else dtPPChemDel.Rows[ir]["sValue"] = dr[sn].ToString();
                }

                for (int ir = 2; ir < dtPPChemDel.Rows.Count; ir++)
                {
                    sn = ((string)dtPPChemDel.Rows[ir]["sName"]);
                    if (dr[sn] == DBNull.Value) dtPPChemDel.Rows[ir]["sValue"] = string.Empty; else dtPPChemDel.Rows[ir]["sValue"] = ((double)dr[sn]).ToString("0.000");
                }

                for (int ir = 0; ir < dtPPChemDel1.Rows.Count; ir++)
                {
                    sn = ((string)dtPPChemDel1.Rows[ir]["sName"]);
                    if (dr[sn] == DBNull.Value) dtPPChemDel1.Rows[ir]["sValue"] = string.Empty; else dtPPChemDel1.Rows[ir]["sValue"] = ((double)dr[sn]).ToString("0.000");
                }

                for (int ir = 0; ir < dtPPPTable.Rows.Count; ir++)
                {
                    sn = ((string)dtPPPTable.Rows[ir]["sName"]);
                    if (dr[sn] == DBNull.Value) dtPPPTable.Rows[ir]["sValue"] = string.Empty; else dtPPPTable.Rows[ir]["sValue"] = ((double)dr[sn]).ToString("0.000");
                }

                for (int ir = 0; ir < dtPPDBelt.Rows.Count; ir++)
                {
                    sn = ((string)dtPPDBelt.Rows[ir]["sName"]);
                    if (dr[sn] == DBNull.Value) dtPPDBelt.Rows[ir]["sValue"] = string.Empty; else dtPPDBelt.Rows[ir]["sValue"] = ((double)dr[sn]).ToString("0.000");
                }

                for (int ir = 0; ir < dtPPOthers.Rows.Count; ir++)
                {
                    sn = ((string)dtPPOthers.Rows[ir]["sName"]);
                    if (dr[sn] == DBNull.Value) dtPPOthers.Rows[ir]["sValue"] = string.Empty; else dtPPOthers.Rows[ir]["sValue"] = ((double)dr[sn]).ToString("0.000");
                }

                for (int ir = 0; ir < dtNewInsData.Rows.Count; ir++)
                {
                    sn = ((string)dtNewInsData.Rows[ir]["sName"]);
                    if (dr[sn] == DBNull.Value) dtNewInsData.Rows[ir]["sValue"] = string.Empty; else dtNewInsData.Rows[ir]["sValue"] = ((double)dr[sn]).ToString("0.000");
                }

                //                CPages.PageInProcess_1.GetDataSet();
                //               drIP = CPages.PageInProcess_1.dr;
                //                CPages.PageFinishedGoods_1.GetDataSet();
                //               drFG = CPages.PageFinishedGoods_1.dr;

            }
            catch (SqlException ex)
            {
                sMsg = "Error in retrieving the Plant Data for the Selected Dataset";
                //MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                // CPages.PageMfgHome_1.MfgDataNotFound();
                // CTelClient.TelException(ex, sMsg);
                return false;
            }



            return true;
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
                //  MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                //sMsg = "Could not save the the process dataset " + Cbfile.iIDMfg.ToString();
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //  CTelClient.TelException(ex, sMsg);
                return;
            }

            //            CStatusBar.SetText("Data Saved at " + DateTime.Now.ToString("hh:mm:ss tt"));
        }

        public string GetPlantData(DateTime dateFG)
        {
            BigQueryResults results, results1, results2, results3, results4; DataTable dtBigQ = new DataTable(); DataTable dtBigQ2 = new DataTable();
            string sMsg = "Big Query Data\n\n\n"; string s = string.Empty, sn, sColumn, sql, sql1, stemp;
            int ir;
            bool bNull = true;
            double dtemp; bool bparse;
            BigQueryTable table;
            int nPAsk1 = 0, nPAsk2 = 0, nPRec1 = 0, nPRec2 = 0;
            string sRet = string.Empty;

            // Refresh process dataset if needed.
            //           if (dr == null || (int)dr["ID4All"] != Cbfile.iIDMfg) { GetDataSet(); dtFGTime = (DateTime)drFG["Finished Board Time Stamp FG"]; MessageBox.Show("Data Refreshed"); }

            JsonCredentialParameters jcp = new JsonCredentialParameters();
            jcp.Type = CDataLake.AccountType;
            jcp.ProjectId = CDataLake.ProjectId;
            jcp.PrivateKeyId = CDataLake.PrivateKeyId;
            jcp.PrivateKey = (CDataLake.PrivateKey);
            jcp.ClientEmail = CDataLake.ClientEmail;
            jcp.ClientId = CDataLake.ClientId;

            var credentials = GoogleCredential.FromJsonParameters(jcp);
            var client = BigQueryClient.Create("si-p-dl-storage", credentials);

            if (cDefualts.IDLocation == 1) { table = client.GetTable("si-p-dl-storage", "raw_gaf_ot_ingestion_gai", "gaf_gai_process"); sColumn = "Gainsville, TX"; }
            else if (cDefualts.IDLocation == 2) { table = client.GetTable("si-p-dl-storage", "raw_gaf_ot_ingestion_cci", "gaf_cci_process"); sColumn = "Cedar City"; }
            else if (cDefualts.IDLocation == 5) { table = client.GetTable("si-p-dl-storage", "raw_gaf_ot_ingestion_sbi", "gaf_sbi_process"); sColumn = "Statesboro"; }
            else if (cDefualts.IDLocation == 6) { table = client.GetTable("si-p-dl-storage", "raw_gaf_ot_ingestion_nci", "gaf_nci_process"); sColumn = "New Columbia"; }
            //           else { MessageBox.Show("No process data for your location", Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop); return false; }
            else { sRet = "No process data for your location"; return sRet; }

            dateTime1 = dateFG.AddMinutes(0.5 * cDefualts.dDelTimeCalc);
            dateTime2 = dateFG.AddMinutes(-0.5 * cDefualts.dDelTimeCalc);
            var sTime1 = "'" + dateTime1.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            var sTime2 = "'" + dateTime2.ToString("yyyy-MM-dd HH:mm:ss") + "'";


            //           MessageBox.Show(sTime1 + "\n" + sTime2);
            /*
            var sql = $"SELECT * FROM( SELECT Tag, cast(Value as FLOAT64) as Value  FROM  {table}  WHERE Timestamp < {sTime1} AND Timestamp > {sTime2} ) PIVOT (avg(Value) as Value for Tag in ('" + (string)dtPPChemDel.Rows[0]["Gainsville, TX"] + "'";
            var sql1 = $"SELECT Tag, avg(cast(Value as FLOAT64)) as Value  FROM  {table}  WHERE Timestamp < {sTime1} AND Timestamp > {sTime2}   and Tag = 'Wet Density' group by Tag";
            var sql2 = $"SELECT Tag, TimeStamp, Value  FROM  {table}  WHERE Timestamp < {sTime1} AND Timestamp > {sTime2}   and Tag = 'Pour_Run_Time' and value != 'null' order by timestamp desc";
*/

            //           Mouse.OverrideCursor = Cursors.Wait;

            sql = string.Empty;

            string sql2A = $" (SELECT Tag, Value FROM  {table}  WHERE Value is not NULL AND Timestamp < {sTime1} And Tag = ";
            string sql2B = " Order by Timestamp Limit 1)";
            string sql2 = string.Empty;
            try
            {
                for (ir = 0; ir < dtPPChemDel.Rows.Count; ir++) // for query seeking averages and query seeking last non null value
                {
                    if (dtPPChemDel.Rows[ir][sColumn] != DBNull.Value)
                    {
                        if (ir > 1) { sql += ", '" + (string)dtPPChemDel.Rows[ir][sColumn] + "'"; nPAsk1++; }
                        if (sql2 != string.Empty) sql2 += " UNION DISTINCT ";       // Added to fech one non null value that will be used if the first query returns null 
                        sql2 += sql2A + " '" + (string)dtPPChemDel.Rows[ir][sColumn] + "' " + sql2B;
                        nPAsk2++;
                    }
                }
                for (ir = 0; ir < dtPPChemDel1.Rows.Count; ir++)
                {
                    if (dtPPChemDel1.Rows[ir][sColumn] != DBNull.Value)
                    {
                        sql += ", '" + (string)dtPPChemDel1.Rows[ir][sColumn] + "'";
                        if (sql2 != string.Empty) sql2 += " UNION DISTINCT ";       // Added to fech one non null value that will be used if the first query returns null 
                        sql2 += sql2A + " '" + (string)dtPPChemDel1.Rows[ir][sColumn] + "' " + sql2B;
                        nPAsk1++; nPAsk2++;

                    }
                }
                for (ir = 0; ir < dtPPPTable.Rows.Count; ir++)
                {
                    if (dtPPPTable.Rows[ir][sColumn] != DBNull.Value)
                    {
                        if (ir == 9) continue;
                        sql += ", '" + (string)dtPPPTable.Rows[ir][sColumn] + "'";
                        if (sql2 != string.Empty) sql2 += " UNION DISTINCT ";       // Added to fech one non null value that will be used if the first query returns null 
                        sql2 += sql2A + " '" + (string)dtPPPTable.Rows[ir][sColumn] + "' " + sql2B;
                        nPAsk1++; nPAsk2++;

                    }
                }
                for (ir = 0; ir < dtPPDBelt.Rows.Count; ir++)
                {
                    if (dtPPDBelt.Rows[ir][sColumn] != DBNull.Value)
                    {
                        sql += ", '" + (string)dtPPDBelt.Rows[ir][sColumn] + "'";
                        if (sql2 != string.Empty) sql2 += " UNION DISTINCT ";       // Added to fech one non null value that will be used if the first query returns null 
                        sql2 += sql2A + " '" + (string)dtPPDBelt.Rows[ir][sColumn] + "' " + sql2B;
                        nPAsk1++; nPAsk2++;

                    }
                }
                for (ir = 0; ir < dtPPOthers.Rows.Count; ir++)
                {
                    if (dtPPOthers.Rows[ir][sColumn] != DBNull.Value)
                    {
                        sql += ", '" + (string)dtPPOthers.Rows[ir][sColumn] + "'";
                        if (sql2 != string.Empty) sql2 += " UNION DISTINCT ";       // Added to fech one non null value that will be used if the first query returns null 
                        sql2 += sql2A + " '" + (string)dtPPOthers.Rows[ir][sColumn] + "' " + sql2B;
                        nPAsk1++; nPAsk2++;
                    }
                }
                for (ir = 0; ir < dtNewInsData.Rows.Count; ir++)
                {
                    if (dtNewInsData.Rows[ir][sColumn] != DBNull.Value)
                    {
                        sql += ", '" + (string)dtNewInsData.Rows[ir][sColumn] + "'";
                        if (sql2 != string.Empty) sql2 += " UNION DISTINCT ";       // Added to fech one non null value that will be used if the first query returns null 
                        sql2 += sql2A + " '" + (string)dtNewInsData.Rows[ir][sColumn] + "' " + sql2B;
                        nPAsk1++; nPAsk2++;
                    }
                }

                sql = sql.Substring(2);

                sql = $"SELECT Tag,  AVG(cast(Value as FLOAT64)) as Value FROM  {table}  WHERE Timestamp < {sTime1} AND Timestamp > {sTime2} and Tag in ({sql}) Group by Tag";


                results = client.ExecuteQuery(sql, parameters: null);

                if (results == null || results.TotalRows < 2) { sMsg = "Process Data could not be retrieved.";
                  //  MessageBox.Show(sMsg, Cbfile.sAppName);
                    return sMsg; }

                if (results != null)
                {
                    if (dtBigQ != null) dtBigQ.Reset();
                    dtBigQ.Rows.Add();

                    foreach (BigQueryRow row in results)
                    {
                        if (row[1] == null) continue;
                        s = (string)row[0]; dtemp = (double)row[1];
                        dtBigQ.Columns.Add(s, typeof(double));
                        dtBigQ.Rows[0][s] = dtemp;
                    }

                }

                results1 = client.ExecuteQuery(sql2, parameters: null);
                if (results1 != null)
                {
                    if (dtBigQ2 != null) dtBigQ2.Reset();
                    dtBigQ2.Rows.Add();

                    foreach (BigQueryRow row in results1)
                    {
                        if (row[1] == null) continue;
                        s = (string)row[0]; stemp = (string)row[1];
                        dtBigQ2.Columns.Add(s, typeof(string));
                        dtBigQ2.Rows[0][s] = stemp;
                    }
                }
            }
            catch (Exception ex)
            {
                sMsg = "Error in writing the process data from datalake to the page";
                sMsg = "Error in obtaining process data from datalake";
              //  MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                //              System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //              Mouse.OverrideCursor = null;
               // CTelClient.TelException(ex, sMsg);
                return sMsg;
            }

            //            sMsg = "Query-Average Received/Asked = " + results.TotalRows.ToString() + "/" + (nPAsk1).ToString() + "\n";
            //            sMsg += "Query-No nNull Received/Asked = " + results1.TotalRows.ToString() + "/" + (nPAsk2).ToString();

            //           MessageBox.Show(sMsg);
            // First non null value for head_position and niproll postion
            try
            {
                /* Not needed as non null query can take care of it.
                 * 
                for (ir = 0; ir < 2; ir++)
                {
                    if (dtPPPTable.Rows[ir][sColumn] == DBNull.Value) continue;
                    s = (string)dtPPPTable.Rows[ir][sColumn];
                    sql = $"SELECT cast(Value as FLOAT64) as Value FROM  {table}  WHERE Timestamp < {sTime1} AND Tag = '{s}' and Value != '' order by Timestamp Desc  limit 1";
                    results = client.ExecuteQuery(sql, parameters: null);
                    if (results != null)
                        foreach (BigQueryRow row in results)
                        {
                            dtemp = (double)row[0];
                            dtBigQ.Columns.Add(s, typeof(double));
                            dtBigQ.Rows[0][s] = dtemp;
                        }
                }
*/
                //Special treatment for the headplate age as the value is stored in time format and not the float format

                ir = 9;
                if (dtPPPTable.Rows[ir][sColumn] != DBNull.Value)
                {
                    sql = (string)dtPPPTable.Rows[ir][sColumn];
                    sql = $"SELECT Tag, TimeStamp, Value  FROM  {table}  WHERE Timestamp < {sTime1} AND Timestamp > {sTime2}   and Tag = '{sql}' and value != 'null' order by timestamp desc";
                    results = client.ExecuteQuery(sql, parameters: null);
                    if (results != null)
                    {
                        DateTime dt1 = DateTime.MinValue; string stime = string.Empty;
                        DateTime tt1;
                        foreach (BigQueryRow row in results)
                        {
                            s = (string)row["Tag"];
                            stime = (string)row["Value"];
                            dt1 = (DateTime)row["TimeStamp"]; break;
                        }
                        if (DateTime.TryParse(stime, out tt1))
                        {
                            dtemp = (tt1.Hour) * 60 + tt1.Minute;
                            dtemp += (dateFG - dt1).TotalMinutes;
                            dtBigQ.Columns.Add(s, typeof(double));
                            dtBigQ.Rows[0][s] = dtemp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sMsg = "Error in writing the process data from datalake to the page";
                sMsg = "Error in obtaining Carriage or Nip Roll position, or Head Plate Age";
               // MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //               Mouse.OverrideCursor = null;
               // CTelClient.TelException(ex, sMsg);
            }

            //Write process data to the database
            int iParams = 0;

            try
            {

                for (ir = 0; ir < 2; ir++)   //1st two rows contain string (not double) values
                {
                    bNull = true;
                    sn = ((string)dtPPChemDel.Rows[ir]["sName"]);
                    if (dtPPChemDel.Rows[ir][sColumn] != DBNull.Value)
                    {
                        s = (string)dtPPChemDel.Rows[ir][sColumn];
                        if (dtBigQ2.Columns.Contains(s) && dtBigQ2.Rows[0][s] != DBNull.Value)
                        { dtPPChemDel.Rows[ir]["sValue"] = dr[sn] = dtBigQ2.Rows[0][s].ToString(); bNull = false; iParams++; continue; }

                    }
                    if (bNull) { dtPPChemDel.Rows[ir]["sValue"] = string.Empty; dr[sn] = DBNull.Value; }
                }


                for (ir = 2; ir < dtPPChemDel.Rows.Count; ir++)
                {
                    bNull = true;
                    sn = ((string)dtPPChemDel.Rows[ir]["sName"]);
                    if (dtPPChemDel.Rows[ir][sColumn] != DBNull.Value)
                    {
                        s = (string)dtPPChemDel.Rows[ir][sColumn];
                        if (dtBigQ.Columns.Contains(s) && dtBigQ.Rows[0][s] != DBNull.Value)
                            if (double.TryParse(dtBigQ.Rows[0][s].ToString(), out dtemp))
                            { dtPPChemDel.Rows[ir]["sValue"] = dtemp.ToString("0.000"); dr[sn] = dtemp; bNull = false; continue; }

                        if (dtBigQ2.Columns.Contains(s) && dtBigQ2.Rows[0][s] != DBNull.Value)
                            if (double.TryParse(dtBigQ2.Rows[0][s].ToString(), out dtemp))
                            { dtPPChemDel.Rows[ir]["sValue"] = dtemp.ToString("0.000"); dr[sn] = dtemp; bNull = false; iParams++; continue; }
                    }

                    if (bNull) { dtPPChemDel.Rows[ir]["sValue"] = string.Empty; dr[sn] = DBNull.Value; }
                }

                for (ir = 0; ir < dtPPChemDel1.Rows.Count; ir++)
                {
                    bNull = true;
                    sn = ((string)dtPPChemDel1.Rows[ir]["sName"]);
                    if (dtPPChemDel1.Rows[ir][sColumn] != DBNull.Value)
                    {
                        s = (string)dtPPChemDel1.Rows[ir][sColumn];
                        if (dtBigQ.Columns.Contains(s) && dtBigQ.Rows[0][s] != DBNull.Value)
                            if (double.TryParse(dtBigQ.Rows[0][s].ToString(), out dtemp))
                            { dtPPChemDel1.Rows[ir]["sValue"] = dtemp.ToString("0.000"); dr[sn] = dtemp; bNull = false; continue; }

                        if (dtBigQ2.Columns.Contains(s) && dtBigQ2.Rows[0][s] != DBNull.Value)
                            if (double.TryParse(dtBigQ2.Rows[0][s].ToString(), out dtemp))
                            { dtPPChemDel1.Rows[ir]["sValue"] = dtemp.ToString("0.000"); dr[sn] = dtemp; bNull = false; iParams++; continue; }
                    }

                    if (bNull) { dtPPChemDel1.Rows[ir]["sValue"] = string.Empty; dr[sn] = DBNull.Value; }
                }

                for (ir = 0; ir < dtPPPTable.Rows.Count; ir++)
                {
                    bNull = true;
                    sn = ((string)dtPPPTable.Rows[ir]["sName"]);
                    if (dtPPPTable.Rows[ir][sColumn] != DBNull.Value)
                    {
                        s = (string)dtPPPTable.Rows[ir][sColumn];
                        if (dtBigQ.Columns.Contains(s) && dtBigQ.Rows[0][s] != DBNull.Value)
                            if (double.TryParse(dtBigQ.Rows[0][s].ToString(), out dtemp))
                            { dtPPPTable.Rows[ir]["sValue"] = dtemp.ToString("0.000"); dr[sn] = dtemp; bNull = false; continue; }

                        if (dtBigQ2.Columns.Contains(s) && dtBigQ2.Rows[0][s] != DBNull.Value)
                            if (double.TryParse(dtBigQ2.Rows[0][s].ToString(), out dtemp))
                            { dtPPPTable.Rows[ir]["sValue"] = dtemp.ToString("0.000"); dr[sn] = dtemp; bNull = false; iParams++; continue; }
                    }

                    if (bNull) { dtPPPTable.Rows[ir]["sValue"] = string.Empty; dr[sn] = DBNull.Value; }
                }

                for (ir = 0; ir < dtPPOthers.Rows.Count; ir++)
                {
                    bNull = true;
                    sn = ((string)dtPPOthers.Rows[ir]["sName"]);
                    if (dtPPOthers.Rows[ir][sColumn] != DBNull.Value)
                    {
                        s = (string)dtPPOthers.Rows[ir][sColumn];
                        if (dtBigQ.Columns.Contains(s) && dtBigQ.Rows[0][s] != DBNull.Value)
                            if (double.TryParse(dtBigQ.Rows[0][s].ToString(), out dtemp))
                            { dtPPOthers.Rows[ir]["sValue"] = dtemp.ToString("0.000"); dr[sn] = dtemp; bNull = false; continue; }

                        if (dtBigQ2.Columns.Contains(s) && dtBigQ2.Rows[0][s] != DBNull.Value)
                            if (double.TryParse(dtBigQ2.Rows[0][s].ToString(), out dtemp))
                            { dtPPOthers.Rows[ir]["sValue"] = dtemp.ToString("0.000"); dr[sn] = dtemp; bNull = false; iParams++; continue; }
                    }

                    if (bNull) { dtPPOthers.Rows[ir]["sValue"] = string.Empty; dr[sn] = DBNull.Value; }
                }

                for (ir = 0; ir < dtPPDBelt.Rows.Count; ir++)
                {
                    bNull = true;
                    sn = ((string)dtPPDBelt.Rows[ir]["sName"]);
                    if (dtPPDBelt.Rows[ir][sColumn] != DBNull.Value)
                    {
                        s = (string)dtPPDBelt.Rows[ir][sColumn];
                        if (dtBigQ.Columns.Contains(s) && dtBigQ.Rows[0][s] != DBNull.Value)
                            if (double.TryParse(dtBigQ.Rows[0][s].ToString(), out dtemp))
                            { dtPPDBelt.Rows[ir]["sValue"] = dtemp.ToString("0.000"); dr[sn] = dtemp; bNull = false; continue; }

                        if (dtBigQ2.Columns.Contains(s) && dtBigQ2.Rows[0][s] != DBNull.Value)
                            if (double.TryParse(dtBigQ2.Rows[0][s].ToString(), out dtemp))
                            { dtPPDBelt.Rows[ir]["sValue"] = dtemp.ToString("0.000"); dr[sn] = dtemp; bNull = false; iParams++; continue; }
                    }

                    if (bNull) { dtPPDBelt.Rows[ir]["sValue"] = string.Empty; dr[sn] = DBNull.Value; }
                }

                for (ir = 0; ir < dtNewInsData.Rows.Count; ir++)
                {
                    bNull = true;
                    sn = ((string)dtNewInsData.Rows[ir]["sName"]);
                    if (dtNewInsData.Rows[ir][sColumn] != DBNull.Value)
                    {
                        s = (string)dtNewInsData.Rows[ir][sColumn];
                        if (dtBigQ.Columns.Contains(s) && dtBigQ.Rows[0][s] != DBNull.Value)
                            if (double.TryParse(dtBigQ.Rows[0][s].ToString(), out dtemp))
                            { dtNewInsData.Rows[ir]["sValue"] = dtemp.ToString("0.000"); dr[sn] = dtemp; bNull = false; continue; }

                        if (dtBigQ2.Columns.Contains(s) && dtBigQ2.Rows[0][s] != DBNull.Value)
                            if (double.TryParse(dtBigQ2.Rows[0][s].ToString(), out dtemp))
                            { dtNewInsData.Rows[ir]["sValue"] = dtemp.ToString("0.000"); dr[sn] = dtemp; bNull = false; iParams++; continue; }
                    }

                    if (bNull) { dtNewInsData.Rows[ir]["sValue"] = string.Empty; dr[sn] = DBNull.Value; }
                }

                //               MessageBox.Show("Data brought in non-null query = " + iParams.ToString());

            }
            catch (Exception ex)
            {
                sMsg = "Error in writing the process data from datalake to the page";
              //  MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //              Mouse.OverrideCursor = null;
               // CTelClient.TelException(ex, sMsg);
            }

            #region code for 1st two rows of PPChemDel Table.  Not needed any more dtBigQ2 table does the job
            /*            
                                    try
                                    {
                                        for (ir = 0; ir < 2; ir++)
                                        {
                                            bNull = true;
                                            sn = ((string)dtPPChemDel.Rows[ir]["sName"]);
                                            if (dtPPChemDel.Rows[ir][sColumn] != DBNull.Value)
                                            {
                                                s = (string)dtPPChemDel.Rows[ir][sColumn];
                                                sql = $"SELECT  Value FROM  {table}  WHERE Timestamp < {sTime1} AND Tag = '{s}' and Value != '' order by Timestamp Desc  limit 1";
                                                results = client.ExecuteQuery(sql, parameters: null);
                                                if (results != null)
                                                    foreach (BigQueryRow row in results)
                                                    { dtPPChemDel.Rows[ir]["sValue"] = row[0]; dr[sn] = row[0]; bNull = false; continue; }
                                            }

                                            if (bNull) { dtPPChemDel.Rows[ir]["sValue"] = string.Empty; dr[sn] = DBNull.Value; }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        sMsg = "Error in obtaining Product Code Data.";
                                        MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                                        System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                                        Mouse.OverrideCursor = null;
                                        CTelClient.TelException(ex, sMsg);
                                    }
                   */
            #endregion

            //          Mouse.OverrideCursor = null;
            //            SetView();
            UpdateDataSet();
            return sRet;
        }

        public async void GetPlantDataBackground(DateTime dateTime)
        {
            cBfile.bCanSwitchRecord = false;
            // CStatusBar.SetText("Pulling process data for dataset " + _objectsService.Cbfile.iIDMfg.ToString());
            string sRet = await Task.Run(() => GetPlantData(dateTime));
           cBfile.bCanSwitchRecord = true;
            // CStatusBar.SetText("Finished pulling process data for dataset " + _objectsService.Cbfile.iIDMfg.ToString());
            return;
        }

    }
}