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
    public class MfgHome
    {
        public bool bInit = true;
        public string sSqlQuery;
        public SqlDataAdapter da;
        public DataTable dt = new DataTable();
        DataRow dr;

        public CLists CLists;
        public CDefualts cDefualts;
        public Cbfile cBfile;

        public MfgHome(CDefualts CDefualts, CLists cLists, Cbfile CBfile)
        {
//            Startup();
            bInit = false;
            CLists = cLists;
            cDefualts = CDefualts;
            cBfile = CBfile;
        }


        public bool SearchMfgDB()
        {
            //            DataTable dt1 = new DataTable();
            string sql = string.Empty, sql1 = string.Empty, sParamValue1 = string.Empty, sParam = string.Empty, sMsg;

            string sqlSearchDS = "SELECT TOP(50) RN.[ID4ALL], convert(varchar(10), RN.[Test Date], 1) as sDate,RN.[Shift], Format(RN.[Test Date], 'MM/dd/yy - hh:mm tt') as sTestTime,Format(R2.[Finished Board Time Stamp FG], 'MM/dd/yy - hh:mm tt') as sFGTestTime, RN.[Product ID], RN.[Product ID] +' - ' +  R1.[Product Description] as Product  " +
            " ,R4.slocation, R5.sName  FROM[In Process Identify] as RN Left Join[Product Matrix] as R1 on RN.[Product ID] = R1.[Product Code] " +
            " Left Join[Finished Goods] as R2 on RN.[ID4all] = R2.[ID4ALL FG] Left Join[Dimensional Stability] as R3 on RN.[ID4all] = R3.[ID4all]" +
            " Left join tblLocations as R4 on RN.Location = R4.ID Left join tblLists as R5 on RN.Shift = R5.ID   ";

            sql = GetSearchCriteria();

            //Final sql string
            if (!string.IsNullOrEmpty(sql)) sql = sqlSearchDS + " Where " + sql; else sql = sqlSearchDS;

            sql += " order by RN.[ID4All] DESC";

            try
            {
                da = new SqlDataAdapter(sql, cBfile.conAZ);
                dt.Clear();
                int itmp = da.Fill(dt);
                if (itmp < 1)
                {
                    sMsg = ("No Mfg Dataset was found to meet the search criteria.  Check the network connection and/or relax the search criteria");
                    // if (!bInit) MessageBox.Show(sMsg, cBfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
                //                dt = dt1.Copy();
            }
            catch (SqlException ex)
            {
                sMsg = "Error in searching the Mfg datasets for editing";
                // MessageBox.Show(sMsg, cBfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //CTelClient.TelException(ex, sMsg);
                return false;
            }
            return true;
        }

        public string GetSearchCriteria()
        {
            string sql = string.Empty, sql1 = string.Empty;
            DateTime dateTime1;
            //Location
            if (cDefualts.IDLocation != 3 && cDefualts.IDLocation != 4) sql = " RN.[Location] = " + cDefualts.IDLocation.ToString();
            //Product

            sql1 = string.Empty;
            if (CLists.drEmployee["Mfg Product Code"] != DBNull.Value)
            {
                if ((string)CLists.drEmployee["Mfg Product Code"] != "All Products") sql1 = " RN.[Product ID] = '" + (string)CLists.drEmployee["Mfg Product Code"] + "' ";
            }
            if (sql1 != string.Empty) { if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1; }

            //Dates
            if (CLists.drEmployee["MfgDate1"] != DBNull.Value)
            {
                dateTime1 = ((DateTime)CLists.drEmployee["MfgDate1"]).AddDays(1);
                sql1 = "RN.[Test Date] < '" + dateTime1.ToString() + "'";
                if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1;
            }

            if (CLists.drEmployee["MfgDate2"] != DBNull.Value)
            {
                dateTime1 = ((DateTime)CLists.drEmployee["MfgDate2"]);
                sql1 = "RN.[Test Date] >= '" + dateTime1.ToString() + "'";
                if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1;
            }


            // Testing stat
            sql1 = string.Empty;
            if (CLists.drEmployee["MfgIDTestingStatus"] != DBNull.Value)
            {
                if ((int)CLists.drEmployee["MfgIDTestingStatus"] == 2) sql1 = " (RN.[IP Testing Complete] = 'false' or RN.[IP Testing Complete] is null) and (RN.[Abandoned] is null or RN.[Abandoned] ='false' ) ";   //Testing in Progress
                else if ((int)CLists.drEmployee["MfgIDTestingStatus"] == 42) sql1 = " RN.[IP Testing Complete] = 'true'  and (RN.[Abandoned] is null or RN.[Abandoned] ='false' ) "; //In Process Complete
                else if ((int)CLists.drEmployee["MfgIDTestingStatus"] == 3) sql1 = " R2.[FG Testing Complete] = 'true'  and (RN.[Abandoned] is null or RN.[Abandoned] ='false' ) "; //FG Testing Complete
                else if ((int)CLists.drEmployee["MfgIDTestingStatus"] == 1) sql1 = " (RN.[Abandoned] is null or RN.[Abandoned] ='false' ) "; //All Active Datasets
                else if ((int)CLists.drEmployee["MfgIDTestingStatus"] == 41) sql1 = " RN.[Abandoned] = 'true'"; //All Active Datasets

            }
            if (sql1 != string.Empty) { if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1; }

            // Aged R Value Testing
            sql1 = string.Empty;
            if (CLists.drEmployee["MfgIDAgedTesting"] != DBNull.Value)
            {
                if ((int)CLists.drEmployee["MfgIDAgedTesting"] == 43) sql1 = " (R2.[FG Aged R Value Complete] is null or R2.[FG Aged R Value Complete] = 'false') ";
                else if ((int)CLists.drEmployee["MfgIDAgedTesting"] == 44) sql1 = " R2.[FG Aged R Value Complete] = 'true'";
            }
            if (sql1 != string.Empty) { if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1; }

            // Dimensional Stability
            sql1 = string.Empty;
            if (CLists.drEmployee["MfgIDDimStability"] != DBNull.Value)
            {
                if ((int)CLists.drEmployee["MfgIDDimStability"] == 46) sql1 = " (R3.[DS Testing Complete] is null or R3.[DS Testing Complete] = 'false') ";
                else if ((int)CLists.drEmployee["MfgIDDimStability"] == 47) sql1 = "R3.[DS Testing Complete] = 'true'";
            }
            if (sql1 != string.Empty) { if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1; }

            // Run Type
            sql1 = string.Empty;
            if (CLists.drEmployee["MfgIDRunType"] != DBNull.Value)
            {
                if ((int)CLists.drEmployee["MfgIDRunType"] == 4) sql1 = " RN.[Run Type] = 4";
                else if ((int)CLists.drEmployee["MfgIDRunType"] == 5) sql1 = " RN.[Run Type] = 5";
            }
            if (sql1 != string.Empty) { if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1; }

            //Shift 
            //           sql1 = "R5.sList = 'shift'";
            //           if (sql == string.Empty) sql = sql1; else sql = sql + " And " + sql1;

            return sql;
        }

        // move out...
        /*        private void gMfgSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                    string sMsg;
                    if (!cBfile.bCanSwitchRecord)
                    {
                        gMfgSearch.SelectedIndex = cBfile.iIDMfgIndex;
                        MessageBox.Show(cBfile.sNoRecSwitchMsg, cBfile.sAppName);
                        return;
                    }

                    if (gMfgSearch.SelectedIndex < 0 || gMfgSearch.SelectedIndex > gMfgSearch.Items.Count - 1) return;
                    if (dt.Rows[gMfgSearch.SelectedIndex]["ID4ALL"] == DBNull.Value)
                    {
                        sMsg = "The selected dataset does not have a valid ID and can not be selected.";
                        MessageBox.Show(sMsg, cBfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                        EnableMfgPages(false);
                        return;
                    }
                    cBfile.iIDMfgIndex = gMfgSearch.SelectedIndex;
                    cBfile.iIDMfg = (int)dt.Rows[gMfgSearch.SelectedIndex]["ID4ALL"];
                    EnableMfgPages(true);
                    GetAllMfgData();
                }
        */
        /*private void gNewDataSetMfg_Click(object sender, RoutedEventArgs e)
        {

            if (!CreateNewDataSet())
            {
                string sMsg = "Errors in creating new dataset.  The network may not be available";
                MessageBox.Show(sMsg, cBfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }*/

        public bool CreateNewDataSet()
        {
            //            DateTime dateDefault = new DateTime(1900, 01, 01);
            string sMsg = "Could not creat a new Mfg dataset.";
            SqlDataAdapter daM;
            DataRow drM; DataTable dtM = new DataTable();
            int iDummy = 0, itmp;
            string sql = "Select Next Value for [dbo].[IDMfgSeq]";

            //            using (cBfile.conAZ)
            {
                SqlCommand cmd = new SqlCommand(sql, cBfile.conAZ);
                try
                {
                    cBfile.conAZ.Open();
                    iDummy = (int)cmd.ExecuteScalar();
                    cBfile.conAZ.Close();
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(sMsg, cBfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                    sMsg = "Could not create a new sequence number for Mfg Dataset";
                    System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                    // CTelClient.TelException(ex, sMsg);
                    return false;
                }
            }

            try
            {
                dr = dt.NewRow();
                dr["ID4ALL"] = iDummy;
                dr["sDate"] = cBfile.dateDefault.ToString("MM/dd/yyyy");
                dr["sLocation"] = cDefualts.sLocation;

                dr[0] = iDummy;
                dr[1] = cBfile.dateDefault.ToString("MM/dd/yyyy");

                dt.Rows.InsertAt(dr, 0);


                sql = "Select * from [In Process Identify] where ID4ALL=" + iDummy.ToString();
                daM = new SqlDataAdapter(sql, cBfile.conAZ); dtM.Clear(); itmp = daM.Fill(dtM);
                if (itmp == 0)
                {
                    drM = dtM.NewRow();
                    drM["ID4ALL"] = drM["IDOld"] = iDummy;
                    drM["Location"] = cDefualts.IDLocation;
                    drM["Test Date"] = cBfile.dateDefault;
                    drM["Run Type"] = cDefualts.iRunType;
                    dtM.Rows.Add(drM);
                    new SqlCommandBuilder(daM);
                    daM.Update(dtM);
                }


                sql = "Select * from [Finished Goods] where [ID4ALL FG]=" + iDummy.ToString();
                daM = new SqlDataAdapter(sql, cBfile.conAZ); dtM.Clear(); itmp = daM.Fill(dtM);
                if (itmp == 0)
                {
                    drM = dtM.NewRow();
                    drM["ID4ALL FG"] = iDummy;
                    drM["IDOldFG"] = cBfile.iIDMfg;
                    drM["Finished Board Time Stamp FG"] = cBfile.dateDefault;
                    dtM.Rows.Add(drM);
                    new SqlCommandBuilder(daM);
                    daM.Update(dtM);
                }

                sql = "Select * from [Dimensional Stability] where [ID4All]=" + iDummy.ToString();
                daM = new SqlDataAdapter(sql, cBfile.conAZ); dtM.Clear(); itmp = daM.Fill(dtM);
                if (itmp == 0)
                {
                    drM = dtM.NewRow();
                    drM["ID4All"] = drM["ID4dimstab"] = iDummy;
                    dtM.Rows.Add(drM);
                    new SqlCommandBuilder(daM);
                    daM.Update(dtM);
                }

                sql = "Select * from [Process Data] where [ID4All]=" + iDummy.ToString();
                daM = new SqlDataAdapter(sql, cBfile.conAZ); dtM.Clear(); itmp = daM.Fill(dtM);
                if (itmp == 0)
                {
                    drM = dtM.NewRow();
                    drM["ID4All"] = iDummy;
                    dtM.Rows.Add(drM);
                    new SqlCommandBuilder(daM);
                    daM.Update(dtM);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(sMsg, cBfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                sMsg = "Could not create a new Mfg Dataset with ID ";
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                // CTelClient.TelException(ex, sMsg);
                return false;
            }

            // WHERE TO SET THESE VALUES.........!!!!!!!!!!!!!!!!!

            /* gMfgSearch.ItemsSource = dt.DefaultView;
            gMfgSearch.SelectedIndex = cBfile.iIDMfgIndex = 0;
            gMfgSearch.ScrollIntoView(gMfgSearch.Items[0]);
            cBfile.iIDMfg = iDummy;
*/
            return true;
        }




        /*  private void gCopy_Click(object sender, RoutedEventArgs e)
          {
              DataTable dt2 = new DataTable(); string sMsg, sFile;
              int icol, irow;
              Mouse.OverrideCursor = Cursors.Wait;

              string sqlSearchDS = "SELECT RN.[test date] as 'InProcess DateTime',  R5.sLocation, R7.sName as 'Shift', R6.Employees, R8.sName as 'Surfactant', R9.sName as 'Pour Head Layout', " +
                  " RN.*,'Finished Goods' as 'Finished Goods',R2.*, " +
              "'Dimensional Stability' as 'Dimensional Stability', R3.*, 'Process Data' as 'Process Data', R4.* FROM[In Process Identify] as RN " +
              " Left Join[Product Matrix] as R1 on RN.[Product ID] = R1.[Product Code] Left Join[Finished Goods] as R2 on RN.[ID4all] = R2.[ID4ALL FG] " +
              " Left Join[Dimensional Stability] as R3 on RN.[ID4all] = R3.[ID4all] Left Join[Process Data] as R4 on RN.[ID4all] = R4.[ID4all] " +
              " Left join tblLocations as R5 on RN.Location = R5.ID Left join Roster as R6 on RN.Operator = R6.ID " +
              " Left join tblLists as R7 on RN.Shift = R7.ID Left join tblLists as R8 on RN.[Surfactant Type] = R8.ID " +
              " Left join tblLists as R9 on RN.[Pour Table Layout] = R9.ID ";

              string sql = GetSearchCriteria();

              //Final sql string
              if (!string.IsNullOrEmpty(sql)) sql = sqlSearchDS + " Where " + sql; else sql = sqlSearchDS;

              sql += " order by RN.[ID4All] DESC";

              try
              {
                  da = new SqlDataAdapter(sql, cBfile.conAZ);
                  dt2.Clear();
                  int itmp = da.Fill(dt2);
                  if (itmp < 1)
                  {
                      sMsg = ("No Mfg Dataset was found to meet the search criteria.  Check the network connection and/or relax the search criteria");
                      MessageBox.Show(sMsg, cBfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Information);
                      //                   return false;
                  }
              }
              catch (SqlException ex)
              {
                  sMsg = "Error in searching the Mfg datasets for exporting";
                  MessageBox.Show(sMsg, cBfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                  System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                  CTelClient.TelException(ex, sMsg);
                  Mouse.OverrideCursor = null;

                  return;
              }


              try
              {
                  var sData = new StringBuilder();

                  sData.Append(dt2.Columns[0].ColumnName.ToString());
                  for (icol = 1; icol < dt2.Columns.Count; icol++) sData.Append("\t" + dt2.Columns[icol].ColumnName.ToString());
                  for (irow = 0; irow < dt2.Rows.Count; irow++)
                  {
                      sData.Append("\n" + (dt2.Rows[irow][0]).ToString());
                      for (icol = 1; icol < dt2.Columns.Count; icol++) sData.Append("\t" + (dt2.Rows[irow][icol]).ToString());
                  }

                  //               cBfile.SendEmail(sLines);
                  Clipboard.SetText(sData.ToString());

              }
              catch (Exception ex)
              {
                  sMsg = "Error in writting datasets to the file"; MessageBox.Show(sMsg, cBfile.sAppName); CTelClient.TelException(ex, sMsg);
                  Mouse.OverrideCursor = null;

              }
              Mouse.OverrideCursor = null;



              return;
          }*/

        public (MfgInProcess,MfgFinishedGoods, MfgDimStability, MfgPlantData,MfgJetMixing) GetAllMfgData(MfgInProcess mfgInProcess,MfgFinishedGoods mfgFinishedGoods, MfgDimStability mfgDimensionStability, MfgPlantData mfgPlantData, MfgJetMixing mfgJetMixing)
        {
            //if (CPages.PageInProcess_1 == null || CPages.PageFinishedGoods_1 == null || CPages.PageDimStability_1 == null || CPages.PagePlantData_1 == null) return;
            mfgInProcess.GetDataSet();
            mfgFinishedGoods.GetDataSet();
            mfgDimensionStability.GetDataSet();
            mfgPlantData.GetDataSet();
            mfgJetMixing.SetDefaultValues();
            // CPages.PagePlantData_1.GetDataSet();

            //mfgFinishedGoods.drIP = mfgDimensionStability.drIP = CPages.PagePlantData_1.drIP = mfgInProcess.dr;
            //  mfgInProcess.drFG = mfgDimensionStability.drFG = CPages.PagePlantData_1.drFG = mfgFinishedGoods.dr;

            mfgDimensionStability.drIP = mfgInProcess.dr;
            mfgDimensionStability.drFG = mfgFinishedGoods.dr;
            mfgPlantData.drIP =  mfgInProcess.dr;
            mfgPlantData.drFG =  mfgFinishedGoods.dr;

            return (mfgInProcess,mfgFinishedGoods,mfgDimensionStability, mfgPlantData, mfgJetMixing);
        }


    }
}
