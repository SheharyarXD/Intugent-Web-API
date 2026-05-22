using IntugentBackend.Services.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Mfg
{
    public class MfgInProcess
    {
        string sSqlQuery;
        public SqlDataAdapter da;
        DataTable dt = new DataTable();
        public DataRow dr, drFG;
        public bool bDataSetChanged = false;
        public string sNull = " ", sDateNullFormat = " "; public string sTimeFormat = "MM/dd/yyyy - hh:mm:tt";
       // System.Windows.Media.Brush backColorCal, backColor, backColorEsc, backColorRed;
        public const string sDef = "", sOr = "0.000";



        Double[] ThicknessIP = new double[17], CompressiveIP = new double[6];

        public Cbfile Cbfile;


        public MfgInProcess(Cbfile cbfile)
        {
            Cbfile = cbfile;
            GetDataSet();
        }

        private void OnPageLoaded()
        {
            /*gsLoc1A.Text = gsLoc1E.Text = CDefualts.sLocMfg1.ToUpper();
            gsLoc2A.Text = CDefualts.sLocMfg2.ToUpper();
            gsLoc3A.Text = gsLoc3E.Text = gsLoc3F.Text = CDefualts.sLocMfg3.ToUpper();

            gsLoc1B.Text = gsLoc1C.Text = gsLoc1D.Text = CDefualts.sLocMfg1;
            gsLoc2B.Text = gsLoc2C.Text = gsLoc2D.Text = CDefualts.sLocMfg2;
            gsLoc3B.Text = gsLoc3C.Text = gsLoc3D.Text = CDefualts.sLocMfg3;*/


            //SetView();
        }

        /*private void CheckLimits(string sF)
        {
            //Must be included in setview and   change products
            string sRet = string.Empty;
            //            if (sF == "All") CProdTargets.GetProductTargets();


            if (sF == "gThicknessIP" || sF == "All")
            {
                if (dr["Thickness - IP"] == DBNull.Value) gThicknessIP.Background = backColorCal;
                else
                {
                    sRet = CIPProdTargets.ThicknessWithinLimits((double)dr["Thickness - IP"]);
                    if (sRet == "Red") gThicknessIP.Background = backColorRed;
                    else if (sRet == "Esc") gThicknessIP.Background = backColorEsc;
                    else gThicknessIP.Background = backColorCal;
                }

            }

            if (sF == "gThicknessAvg1" || sF == "All")
            {
                if (dr["Thickness IP Avg1"] == DBNull.Value) gThicknessAvg1.Background = backColorCal;
                else
                {
                    sRet = CIPProdTargets.ThicknessAvg1WithinLimits((double)dr["Thickness IP Avg1"]);
                    if (sRet == "Red") gThicknessAvg1.Background = backColorRed;
                    else if (sRet == "Esc") gThicknessAvg1.Background = backColorEsc;
                    else gThicknessAvg1.Background = backColorCal;
                }

            }
            if (sF == "gThicknessAvg2" || sF == "All")
            {
                if (dr["Thickness IP Avg2"] == DBNull.Value) gThicknessAvg2.Background = backColorCal;
                else
                {
                    sRet = CIPProdTargets.ThicknessAvg2WithinLimits((double)dr["Thickness IP Avg2"]);
                    if (sRet == "Red") gThicknessAvg2.Background = backColorRed;
                    else if (sRet == "Esc") gThicknessAvg2.Background = backColorEsc;
                    else gThicknessAvg2.Background = backColorCal;
                }

            }
            if (sF == "gThicknessSlope" || sF == "All")
            {
                if (dr["IP Thickness Slope"] == DBNull.Value) gThicknessSlope.Background = backColorCal;
                else
                {
                    sRet = CIPProdTargets.ThicknessSlopeWithinLimits((double)dr["IP Thickness Slope"]);
                    if (sRet == "Red") gThicknessSlope.Background = backColorRed;
                    else if (sRet == "Esc") gThicknessSlope.Background = backColorEsc;
                    else gThicknessSlope.Background = backColorCal;
                }

            }

            if (sF == "gFlatness" || sF == "All")
            {
                if (dr["Flatness IP"] == DBNull.Value) gFlatness.Background = backColorCal;
                else
                {
                    sRet = CIPProdTargets.ThicknessProfileWithinLimits((double)dr["Flatness IP"]);
                    if (sRet == "Red") gFlatness.Background = backColorRed;
                    else if (sRet == "Esc") gFlatness.Background = backColorEsc;
                    else gFlatness.Background = backColorCal;
                }

            }
            if (sF == "gBundleHeightIP" || sF == "All")
            {
                if (dr["Bundle Height IP"] == DBNull.Value) gBundleHeightIP.Background = backColor;
                else
                {
                    sRet = CIPProdTargets.BundleHeightWithinLimits((double)dr["Bundle Height IP"]);
                    if (sRet == "Red") gBundleHeightIP.Background = backColorRed;
                    else if (sRet == "Esc") gBundleHeightIP.Background = backColorEsc;
                    else gBundleHeightIP.Background = backColor;
                }

            }

            if (sF == "gLength" || sF == "All")
            {
                if (dr["Length"] == DBNull.Value) gLength.Background = backColor;
                else
                {
                    sRet = CIPProdTargets.BoardLengthWithinLimits((double)dr["Length"]);
                    if (sRet == "Red") gLength.Background = backColorRed;
                    else if (sRet == "Esc") gLength.Background = backColorEsc;
                    else gLength.Background = backColor;
                }

            }
            if (sF == "gWidth" || sF == "All")
            {
                if (dr["Width"] == DBNull.Value) gWidth.Background = backColor;
                else
                {
                    sRet = CIPProdTargets.BoardWidthWithinLimits((double)dr["Width"]);
                    if (sRet == "Red") gWidth.Background = backColorRed;
                    else if (sRet == "Esc") gWidth.Background = backColorEsc;
                    else gWidth.Background = backColor;
                }

            }
            if (sF == "gDiagoanlDiff" || sF == "All")
            {
                if (dr["IP Diagonal Diff"] == DBNull.Value) gDiagoanlDiff.Background = backColorCal;
                else
                {
                    sRet = CIPProdTargets.BoardSquarenessWithinLimits((double)dr["IP Diagonal Diff"]);
                    if (sRet == "Red") gDiagoanlDiff.Background = backColorRed;
                    else if (sRet == "Esc") gDiagoanlDiff.Background = backColorEsc;
                    else gDiagoanlDiff.Background = backColor;
                }
            }

            if (sF == "gCoreDensityIP" || sF == "All")
            {
                if (dr["Core Density - IP"] == DBNull.Value) gCoreDensityIP.Background = backColorCal;
                else
                {
                    sRet = CIPProdTargets.CoreDensityWithinLimits((double)dr["Core Density - IP"]);
                    if (sRet == "Red") gCoreDensityIP.Background = backColorRed;
                    else if (sRet == "Esc") gCoreDensityIP.Background = backColorEsc;
                    else gCoreDensityIP.Background = backColorCal;
                }
            }
            if (sF == "gCoreDensityIP_1" || sF == "All")
            {
                if (dr["Core Density - IP 1"] == DBNull.Value) gCoreDensityIP_1.Background = backColorCal;
                else
                {
                    sRet = CIPProdTargets.CoreDensityWithinLimits((double)dr["Core Density - IP 1"]);
                    if (sRet == "Red") gCoreDensityIP_1.Background = backColorRed;
                    else if (sRet == "Esc") gCoreDensityIP_1.Background = backColorEsc;
                    else gCoreDensityIP_1.Background = backColorCal;
                }
            }
            if (sF == "gCoreDensityIP_2" || sF == "All")
            {
                if (dr["Core Density - IP 2"] == DBNull.Value) gCoreDensityIP_2.Background = backColorCal;
                else
                {
                    sRet = CIPProdTargets.CoreDensityWithinLimits((double)dr["Core Density - IP 2"]);
                    if (sRet == "Red") gCoreDensityIP_2.Background = backColorRed;
                    else if (sRet == "Esc") gCoreDensityIP_2.Background = backColorEsc;
                    else gCoreDensityIP_2.Background = backColorCal;
                }
            }
            if (sF == "gCoreDensityIP_3" || sF == "All")
            {
                if (dr["Core Density - IP 3"] == DBNull.Value) gCoreDensityIP_3.Background = backColorCal;
                else
                {
                    sRet = CIPProdTargets.CoreDensityWithinLimits((double)dr["Core Density - IP 3"]);
                    if (sRet == "Red") gCoreDensityIP_3.Background = backColorRed;
                    else if (sRet == "Esc") gCoreDensityIP_3.Background = backColorEsc;
                    else gCoreDensityIP_3.Background = backColorCal;
                }
            }
            if (sF == "gCompressiveIP" || sF == "All")
            {
                if (dr["Compressive Strength - IP"] == DBNull.Value) gCompressiveIP.Background = backColorCal;
                else
                {
                    sRet = CIPProdTargets.CompressionStrWithinLimits((double)dr["Compressive Strength - IP"]);
                    if (sRet == "Red") gCompressiveIP.Background = backColorRed;
                    else if (sRet == "Esc") gCompressiveIP.Background = backColorEsc;
                    else gCompressiveIP.Background = backColorCal;
                }
            }
            if (sF == "gCompressiveIP5" || sF == "All")
            {
                if (dr["Compressive Strength 5 - IP"] == DBNull.Value) gCompressiveIP5.Background = backColorCal;
                else
                {
                    sRet = CIPProdTargets.CompressionStrWithinLimits((double)dr["Compressive Strength 5 - IP"]);
                    if (sRet == "Red") gCompressiveIP5.Background = backColorRed;
                    else if (sRet == "Esc") gCompressiveIP5.Background = backColorEsc;
                    else gCompressiveIP5.Background = backColorCal;
                }
            }


            DateTime IPdate, FGdate;
            string sMsg = "Green Board and FG Board time stamps must be greater than 01-01-2000 and must be within " + CDefualts.dDelTimeButton.ToString() + " minutes of each other.";

            if (sF == "BoardTimeStamp" || sF == "All")
            {
                if (drFG["Finished Board Time Stamp FG"] == DBNull.Value || dr["Test Date"] == DBNull.Value)
                { gBoardTimeStamp.Background = backColorRed; if (sF == "BoardTimeStamp") MessageBox.Show(sMsg, Cbfile.sAppName); }
                else
                {
                    FGdate = (DateTime)drFG["Finished Board Time Stamp FG"];
                    IPdate = (DateTime)dr["Test Date"];
                    if (IPdate < new DateTime(2000, 01, 01) || Math.Abs((FGdate - IPdate).TotalMinutes) > CDefualts.dDelTimeButton)
                    { gBoardTimeStamp.Background = backColorRed; }
                    else gBoardTimeStamp.Background = backColor;
                    if (sF == "BoardTimeStamp")
                    {
                        if (gBoardTimeStamp.Background == backColor)
                        {
                            CStatusBar.SetText("Pulling process data for dataset " + Cbfile.iIDMfg.ToString());
                            CPages.PagePlantData_1.GetPlantDataBackground(FGdate);
                            //                           CStatusBar.SetText("Finished pulling process data for dataset " + Cbfile.iIDMfg.ToString());
                        }
                        else MessageBox.Show(sMsg, Cbfile.sAppName);

                    }
                }
            }

        }
        */
        #region Database Functions
        public void GetDataSet()
        {
            string sMsg;
            try
            {
                sSqlQuery = "Select * from [In Process Identify] where ID4ALL=" + Cbfile.iIDMfg.ToString(); //1943";  //3137
                da = new SqlDataAdapter(sSqlQuery, Cbfile.conAZ);

                dt.Clear();
                int itmp = da.Fill(dt);
                if (itmp < 1)
                {
                    sMsg = "Could not find the In Process Data for the Selected Dataset";
                    //MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                    //System.Diagnostics.Trace.TraceError(sMsg);
                    //CPages.PageMfgHome_1.MfgDataNotFound();
                    return;
                }
                dr = dt.Rows[0];
            }
            catch (SqlException ex)
            {
                sMsg = "Error in retrieving the In Process Data for the Selected Dataset";
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
                //MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                sMsg = "Could not save the InProcess dataset " + Cbfile.iIDMfg.ToString();
                //System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //CTelClient.TelException(ex, sMsg);
                return;
            }

           // CStatusBar.SetText("Data Saved at " + DateTime.Now.ToString("hh:mm:ss:tt"));

        }

        #endregion


        /*

        public string SaveData()
        {
            string sMsg;


            return gThicknessIP_1.Text + " - " + DateTime.Now.Second.ToString();

        }

        public void SetView()
        {
            if (Cbfile.iIDMfgIndex == 0) gDataSetNext.IsEnabled = false; else gDataSetNext.IsEnabled = true;
            if (Cbfile.iIDMfgIndex == CPages.PageMfgHome_1.dt.Rows.Count - 1) gDataSetPrev.IsEnabled = false; else gDataSetPrev.IsEnabled = true;

            //           GetDataSet();
            //           CPages.PageFinishedGoods_1.GetDataSet();
            //           drFG = CPages.PageFinishedGoods_1.dr;

            #region date, time, and datetime controls.


            //            if (dr["Test Time"] == DBNull.Value) gTestTime.CustomFormat = sTimeNullFormat;
            //            else { gTestTime.CustomFormat = sTimeFormat; gTestTime.Value = (DateTime)dr["Test Time"]; }

            if (dr["Test Date"] == DBNull.Value) gTestDateTime.Value = Cbfile.dateDefault;
            else { gTestDateTime.Value = (DateTime)dr["Test Date"]; }

            #endregion

            #region Gen. Info, Av. Values, Board Dimensions

            gID.Text = dr["ID4ALL"].ToString();
            if ((dr["Operator"] == DBNull.Value)) gOperator.Text = String.Empty; else gOperator.SelectedValue = dr["Operator"].ToString();
            //           gTestDate.Text = "        ";// SetDateTimeField("Test Date");

            if (!(dr["Click box if time stamp is NOT legible"] == DBNull.Value)) gTimeStampNotLegible.IsChecked = (bool)dr["Click box if time stamp is NOT legible"]; else gTimeStampNotLegible.IsChecked = false;
            if (dr["Shift"] == DBNull.Value) gShift.SelectedValue = null; else gShift.SelectedValue = dr["Shift"].ToString();
            if ((dr["Product ID"] == DBNull.Value)) gProdID.SelectedValue = String.Empty; else gProdID.SelectedValue = dr["Product ID"].ToString();
            if ((dr["Paper Manufacturer"] == DBNull.Value)) gPaperManufacturer.Text = String.Empty; else gPaperManufacturer.SelectedValue = dr["Paper Manufacturer"];
            gRunningWetDesnsity.Text = SetDoubleTextField("Running Wet Density");
            gWarehouseTemp.Text = SetDoubleTextField("Warehouse Temp");
            gWarehouseHumidity.Text = SetDoubleTextField("Warehouse Humidity");

            if ((dr["IP Testing Complete"] == DBNull.Value)) gInProcessDone.IsChecked = false; else gInProcessDone.IsChecked = (bool)dr["IP Testing Complete"];
            if (dr["Run Type"] == DBNull.Value) gRunType.SelectedValue = null; else gRunType.SelectedValue = dr["Run Type"].ToString();
            if ((dr["Abandoned"] == DBNull.Value)) gAbandoned.IsChecked = false; else gAbandoned.IsChecked = (bool)dr["Abandoned"];

            gCoreDensityIP.Text = SetDoubleTextField("Core Density - IP", sOr);
            gThicknessIP.Text = SetDoubleTextField("Thickness - IP", sOr);
            gCompressiveIP.Text = SetDoubleTextField("Compressive Strength - IP", sOr);
            gCompressiveIP5.Text = SetDoubleTextField("Compressive Strength 5 - IP", sOr);

            gThicknessValley.Text = SetDoubleTextField("thickness IP- valleys", sOr);
            gThicknessPeak.Text = SetDoubleTextField("thickness peaks IP", sOr);
            gFlatness.Text = SetDoubleTextField("Flatness IP", sOr);

            gLength.Text = SetDoubleTextField("Length");
            gWidth.Text = SetDoubleTextField("Width");
            gBundleHeightIP.Text = SetDoubleTextField("Bundle Height IP");
            gDiagoanl1.Text = SetDoubleTextField("IP Diagonal 1");
            gDiagoanl2.Text = SetDoubleTextField("IP Diagonal 2");
            gDiagoanlDiff.Text = SetDoubleTextField("IP Diagonal Diff", sOr);

            #endregion

            #region thickness

            gThicknessIP_1.Text = SetDoubleTextField("Thickness IP - 1");
            gThicknessIP_2.Text = SetDoubleTextField("Thickness IP - 2");
            gThicknessIP_3.Text = SetDoubleTextField("Thickness IP - 3");
            gThicknessIP_4.Text = SetDoubleTextField("Thickness IP - 4");
            gThicknessIP_5.Text = SetDoubleTextField("Thickness IP - 5");
            gThicknessIP_6.Text = SetDoubleTextField("Thickness IP - 6");
            gThicknessIP_7.Text = SetDoubleTextField("Thickness IP - 7");
            gThicknessIP_8.Text = SetDoubleTextField("Thickness IP - 8");
            gThicknessIP_9.Text = SetDoubleTextField("Thickness IP - 9");
            gThicknessIP_10.Text = SetDoubleTextField("Thickness IP - 10");
            gThicknessIP_11.Text = SetDoubleTextField("Thickness IP - 11");
            gThicknessIP_12.Text = SetDoubleTextField("Thickness IP - 12");
            gThicknessIP_13.Text = SetDoubleTextField("Thickness IP - 13");
            gThicknessIP_14.Text = SetDoubleTextField("Thickness IP - 14");
            gThicknessIP_15.Text = SetDoubleTextField("Thickness IP - 15");
            gThicknessIP_16.Text = SetDoubleTextField("Thickness IP - 16");
            gThicknessIP_17.Text = SetDoubleTextField("Thickness IP - 17");

            #endregion

            #region Board Performation, Compressive Strength

            if (!(dr["Top Board Perforated"] == DBNull.Value)) gTopBoardPerforated.IsChecked = (bool)dr["Top Board Perforated"]; else gTopBoardPerforated.IsChecked = false;
            if (!(dr["Bottom Board Perforated"] == DBNull.Value)) gBottomBoardPerforated.IsChecked = (bool)dr["Bottom Board Perforated"]; else gBottomBoardPerforated.IsChecked = false;
            if (!(dr["Top Board Print OK"] == DBNull.Value)) gTopBoardPrintOK.IsChecked = (bool)dr["Top Board Print OK"]; else gTopBoardPrintOK.IsChecked = false;
            if (!(dr["Bottom Board Print OK"] == DBNull.Value)) gBottomBoardPrintOK.IsChecked = (bool)dr["Bottom Board Print OK"]; else gBottomBoardPrintOK.IsChecked = false;

            gCompressiveIP_1.Text = SetDoubleTextField("Compressive IP - 1");
            gCompressiveIP_2.Text = SetDoubleTextField("Compressive IP - 2");
            gCompressiveIP_3.Text = SetDoubleTextField("Compressive IP - 3");
            gCompressiveIP_4.Text = SetDoubleTextField("Compressive IP - 4");
            gCompressiveIP_5.Text = SetDoubleTextField("Compressive IP - 5");
            gCompressiveIP_6.Text = SetDoubleTextField("Compressive IP - 6");

            if (dr["Comp Str Knit Present 1"] == DBNull.Value) gCompStrKnitPresent_1.IsChecked = false; else gCompStrKnitPresent_1.IsChecked = (bool)dr["Comp Str Knit Present 1"];
            if (dr["Comp Str Knit Present 2"] == DBNull.Value) gCompStrKnitPresent_2.IsChecked = false; else gCompStrKnitPresent_2.IsChecked = (bool)dr["Comp Str Knit Present 2"];
            if (dr["Comp Str Knit Present 3"] == DBNull.Value) gCompStrKnitPresent_3.IsChecked = false; else gCompStrKnitPresent_3.IsChecked = (bool)dr["Comp Str Knit Present 3"];
            if (dr["Comp Str Knit Present 4"] == DBNull.Value) gCompStrKnitPresent_4.IsChecked = false; else gCompStrKnitPresent_4.IsChecked = (bool)dr["Comp Str Knit Present 4"];
            if (dr["Comp Str Knit Present 5"] == DBNull.Value) gCompStrKnitPresent_5.IsChecked = false; else gCompStrKnitPresent_5.IsChecked = (bool)dr["Comp Str Knit Present 5"];
            if (dr["Comp Str Knit Present 6"] == DBNull.Value) gCompStrKnitPresent_6.IsChecked = false; else gCompStrKnitPresent_6.IsChecked = (bool)dr["Comp Str Knit Present 6"];

            #endregion

            #region core density
            gMass_1.Text = SetDoubleTextField("Mass 1");
            gL1_1.Text = SetDoubleTextField("L1 1");
            //            gL2_1.Text = SetDoubleTextField("L2 1");
            gW1_1.Text = SetDoubleTextField("W1 1");
            //           gW2_1.Text = SetDoubleTextField("W2 1");
            gT1_1.Text = SetDoubleTextField("T1 1");
            gT2_1.Text = SetDoubleTextField("T2 1");
            gT3_1.Text = SetDoubleTextField("T3 1");
            gT4_1.Text = SetDoubleTextField("T4 1");
            gT5_1.Text = SetDoubleTextField("T5 1");
            if (dr["Core Knit Present 1"] == DBNull.Value) gCoreKnitPresent_1.IsChecked = false; else gCoreKnitPresent_1.IsChecked = (bool)dr["Core Knit Present 1"];
            gCoreDensityIP_1.Text = SetDoubleTextField("Core Density - IP 1", sOr);

            gMass_2.Text = SetDoubleTextField("Mass 2");
            gL1_2.Text = SetDoubleTextField("L1 2");
            //            gL2_2.Text = SetDoubleTextField("L2 2");
            gW1_2.Text = SetDoubleTextField("W1 2");
            //            gW2_2.Text = SetDoubleTextField("W2 2");
            gT1_2.Text = SetDoubleTextField("T1 2");
            gT2_2.Text = SetDoubleTextField("T2 2");
            gT3_2.Text = SetDoubleTextField("T3 2");
            gT4_2.Text = SetDoubleTextField("T4 2");
            gT5_2.Text = SetDoubleTextField("T5 2");
            if (dr["Core Knit Present 2"] == DBNull.Value) gCoreKnitPresent_2.IsChecked = false; else gCoreKnitPresent_2.IsChecked = (bool)dr["Core Knit Present 2"];
            gCoreDensityIP_2.Text = SetDoubleTextField("Core Density - IP 2", sOr);

            gMass_3.Text = SetDoubleTextField("Mass 3");
            gL1_3.Text = SetDoubleTextField("L1 3");
            //          gL2_3.Text = SetDoubleTextField("L2 3");
            gW1_3.Text = SetDoubleTextField("W1 3");
            //          gW2_3.Text = SetDoubleTextField("W2 3");
            gT1_3.Text = SetDoubleTextField("T1 3");
            gT2_3.Text = SetDoubleTextField("T2 3");
            gT3_3.Text = SetDoubleTextField("T3 3");
            gT4_3.Text = SetDoubleTextField("T4 3");
            gT5_3.Text = SetDoubleTextField("T5 3");
            if (dr["Core Knit Present 3"] == DBNull.Value) gCoreKnitPresent_3.IsChecked = false; else gCoreKnitPresent_3.IsChecked = (bool)dr["Core Knit Present 3"];
            gCoreDensityIP_3.Text = SetDoubleTextField("Core Density - IP 3", sOr);

            #endregion

            #region Pour Table

            if (dr["Time of Pour Table QC Check"] == DBNull.Value) gTimePourTableQCCheck.CustomFormat = sNull;
            else { gTimePourTableQCCheck.CustomFormat = sTimeFormat; gTimePourTableQCCheck.Value = (DateTime)dr["Time of Pour Table QC Check"]; }
            //           gTimePourTableQCCheck.Value = SetDateTimeField("Time of Pour Table QC Check");
            if ((dr["Surfactant Type"] == DBNull.Value)) gSurfactant.Text = String.Empty; else gSurfactant.SelectedValue = dr["Surfactant Type"].ToString();
            if ((dr["Pour Table Layout"] == DBNull.Value)) gLayout.Text = String.Empty; else gLayout.SelectedValue = dr["Pour Table Layout"].ToString();

            gSplitterAge.Text = SetIntTextField("Splitter Age (minutes)");
            gHeadPlateAge.Text = SetIntTextField("Headplate Age / Pour Run Time (minutes)");

            gPourHeadPosition_1.Text = SetDoubleTextField("Pour Head Position - 1");
            gPourHeadPosition_2.Text = SetDoubleTextField("Pour Head Position - 2");
            gPourHeadPosition_3.Text = SetDoubleTextField("Pour Head Position - 3");

            gIRFacerTempLower.Text = SetDoubleTextField("IR Facer Temp - Lower");
            gIRFacerTempUpper.Text = SetDoubleTextField("IR Facer Temp - Upper");

            gIRStreamTempPourHead_1.Text = SetDoubleTextField("IR Stream Temp - Pour Head 1");
            gIRStreamTempPourHead_2.Text = SetDoubleTextField("IR Stream Temp - Pour Head 2");
            gIRStreamTempPourHead_3.Text = SetDoubleTextField("IR Stream Temp - Pour Head 3");

            gIRNipRoll_1.Text = SetDoubleTextField("IR Stream Temp - Nipp Roll 1");
            gIRNipRoll_2.Text = SetDoubleTextField("IR Stream Temp - Nipp Roll 2");
            gIRNipRoll_3.Text = SetDoubleTextField("IR Stream Temp - Nipp Roll 3");

            #endregion

            #region Knit Line Location
            gKnitLineLoc_1.Text = SetDoubleTextField("Knit Line Loc 1");
            gKnitLineLoc_2.Text = SetDoubleTextField("Knit Line Loc 2");
            gKnitLineLoc_3.Text = SetDoubleTextField("Knit Line Loc 3");
            gKnitLineLoc_4.Text = SetDoubleTextField("Knit Line Loc 4");
            gKnitLineLoc_5.Text = SetDoubleTextField("Knit Line Loc 5");
            gKnitLineLoc_6.Text = SetDoubleTextField("Knit Line Loc 6");
            gKnitLineLoc_7.Text = SetDoubleTextField("Knit Line Loc 7");

            #endregion

            #region Thickness Avgs for each side
            gThicknessAvg1.Text = SetDoubleTextField("Thickness IP Avg1", sOr);
            gThicknessAvg2.Text = SetDoubleTextField("Thickness IP Avg2", sOr);
            gThicknessSlope.Text = SetDoubleTextField("IP Thickness Slope", sOr);

            #endregion
            CheckLimits("All");

        }



        private void DataSetNavi_Click(object sender, RoutedEventArgs e)
        {
            string sFld = String.Empty;
            Button tbx = sender as Button;
            string sName = tbx.Name;

            if (!Cbfile.bCanSwitchRecord) { MessageBox.Show(Cbfile.sNoRecSwitchMsg, Cbfile.sAppName); return; }
            switch (sName)
            {
                case "gDataSetPrev": Cbfile.iIDMfgIndex += 1; break;
                case "gDataSetNext": Cbfile.iIDMfgIndex -= 1; break;
            }

            if (Cbfile.iIDMfgIndex < 0) Cbfile.iIDMfgIndex = 0;
            if (Cbfile.iIDMfgIndex > CPages.PageMfgHome_1.dt.Rows.Count - 1) Cbfile.iIDMfgIndex = CPages.PageMfgHome_1.dt.Rows.Count - 1;

            Cbfile.iIDMfg = (int)CPages.PageMfgHome_1.dt.Rows[Cbfile.iIDMfgIndex]["ID4ALL"];
            CLists.drEmployee["MfgIDSelected"] = Cbfile.iIDMfg; CLists.UpdateEmployee();

            CPages.PageMfgHome_1.GetAllMfgData();

            SetView();

            *//*
                        if (Cbfile.iIDMfgIndex > (CPages.PageMfgHome_1.dt.Rows.Count-2))
                        {
                            gDataSetNext.IsEnabled = false;
                            if (Cbfile.iIDMfgIndex > 0) { gDataSetPrev.IsEnabled = true; }
                           gDataSetPrev.IsEnabled = false; }
                        }


                        Cbfile.iIDMfg = (int) CPages.PageMfgHome_1.dt.Rows[Cbfile.iIDMfgIndex]["ID"];
            *//*

        }

        */
        public string SetDoubleField(string value, string sField)
        {
            double dtmp;
            string sText = value;

            if (String.IsNullOrEmpty(sText))
            {
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
                else
                {
                    if (dr[sField] == DBNull.Value)
                        return string.Empty;
                    else
                        return ((double)dr[sField]).ToString();
                }
            }

        }

        /*

        public void SetIntField(TextBox tb, string sField)
        {
            int itmp;

            string sText = tb.Text;
            if (String.IsNullOrEmpty(sText)) dr[sField] = DBNull.Value;
            else
            {
                if (int.TryParse(sText, out itmp)) dr[sField] = itmp;
                else { MessageBox.Show("Improper Value. New Value is not accepted."); if (dr[sField] == DBNull.Value) tb.Text = string.Empty; else tb.Text = ((int)dr[sField]).ToString(); }
            }

        }
        */

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
        /*

        #region Preview Function
*//*
        private void PreviewPositiveRealNumber(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9 .]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PreviewRealNumber(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9 . -+ --]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PreviewInteger(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }*//*
        #endregion

        #region Lost Focus and Other Actions


        private void gCheckBoxes_LF(object sender, RoutedEventArgs e)
        {
            string sFld = String.Empty;
            Control ctrl = sender as Control;
            string sName = ctrl.Name;

            bDataSetChanged = true;

            switch (sName)
            {
                case "gCoreKnitPresent_1": if (gCoreKnitPresent_1.IsChecked == null) dr["Core Knit Present 1"] = DBNull.Value; else dr["Core Knit Present 1"] = gCoreKnitPresent_1.IsChecked; break;
                case "gCoreKnitPresent_2": if (gCoreKnitPresent_2.IsChecked == null) dr["Core Knit Present 2"] = DBNull.Value; else dr["Core Knit Present 2"] = gCoreKnitPresent_2.IsChecked; break;
                case "gCoreKnitPresent_3": if (gCoreKnitPresent_3.IsChecked == null) dr["Core Knit Present 3"] = DBNull.Value; else dr["Core Knit Present 3"] = gCoreKnitPresent_3.IsChecked; break;

                case "gCompStrKnitPresent_1": if (gCompStrKnitPresent_1.IsChecked == null) dr["Comp Str Knit Present 1"] = DBNull.Value; else dr["Comp Str Knit Present 1"] = gCompStrKnitPresent_1.IsChecked; break;
                case "gCompStrKnitPresent_2": if (gCompStrKnitPresent_2.IsChecked == null) dr["Comp Str Knit Present 2"] = DBNull.Value; else dr["Comp Str Knit Present 2"] = gCompStrKnitPresent_2.IsChecked; break;
                case "gCompStrKnitPresent_3": if (gCompStrKnitPresent_3.IsChecked == null) dr["Comp Str Knit Present 3"] = DBNull.Value; else dr["Comp Str Knit Present 3"] = gCompStrKnitPresent_3.IsChecked; break;
                case "gCompStrKnitPresent_4": if (gCompStrKnitPresent_4.IsChecked == null) dr["Comp Str Knit Present 4"] = DBNull.Value; else dr["Comp Str Knit Present 4"] = gCompStrKnitPresent_4.IsChecked; break;
                case "gCompStrKnitPresent_5": if (gCompStrKnitPresent_5.IsChecked == null) dr["Comp Str Knit Present 5"] = DBNull.Value; else dr["Comp Str Knit Present 5"] = gCompStrKnitPresent_5.IsChecked; break;
                case "gCompStrKnitPresent_6": if (gCompStrKnitPresent_6.IsChecked == null) dr["Comp Str Knit Present 6"] = DBNull.Value; else dr["Comp Str Knit Present 6"] = gCompStrKnitPresent_6.IsChecked; break;


            }
            UpdateDataSet();
        }

        private void gBoardDimensions_LF(object sender, RoutedEventArgs e)
        {
            bool bDia = false;
            double dtmp;
            string sFld = String.Empty;
            TextBox tbx = sender as TextBox;
            string sText = tbx.Text;
            string sName = tbx.Name;

            bDataSetChanged = true;

            switch (sName)
            {
                case "gLength": SetDoubleField(gLength, "Length"); CheckLimits("gLength"); CheckLimits("gLength"); break;
                case "gWidth": SetDoubleField(gWidth, "Width"); CheckLimits("gWidth"); CheckLimits("gWidth"); break;
                case "gBundleHeightIP": SetDoubleField(gBundleHeightIP, "Bundle Height IP"); CheckLimits("gBundleHeightIP"); break;
                case "gDiagoanl1": SetDoubleField(gDiagoanl1, "IP Diagonal 1"); bDia = true; break;
                case "gDiagoanl2": SetDoubleField(gDiagoanl2, "IP Diagonal 2"); bDia = true; break;

            }

            if (bDia && !(dr["IP Diagonal 1"] == DBNull.Value) && !(dr["IP Diagonal 2"] == DBNull.Value))
            { dtmp = Math.Abs((double)dr["IP Diagonal 1"] - (double)dr["IP Diagonal 2"]); dr["IP Diagonal Diff"] = dtmp; gDiagoanlDiff.Text = dtmp.ToString(sOr); }

            UpdateDataSet();

            if (bDia) CheckLimits("gDiagoanlDiff");
        }

        private void gThickness_LF(object sender, RoutedEventArgs e)
        {
            string sFld = String.Empty;
            TextBox tbx = sender as TextBox;
            string sText = tbx.Text;
            string sName = tbx.Name;

            bDataSetChanged = true;

            switch (sName)
            {
                case "gThicknessIP_1": SetDoubleField(gThicknessIP_1, "Thickness IP - 1"); break;
                case "gThicknessIP_2": SetDoubleField(gThicknessIP_2, "Thickness IP - 2"); break;
                case "gThicknessIP_3": SetDoubleField(gThicknessIP_3, "Thickness IP - 3"); break;
                case "gThicknessIP_4": SetDoubleField(gThicknessIP_4, "Thickness IP - 4"); break;
                case "gThicknessIP_5": SetDoubleField(gThicknessIP_5, "Thickness IP - 5"); break;
                case "gThicknessIP_6": SetDoubleField(gThicknessIP_6, "Thickness IP - 6"); break;
                case "gThicknessIP_7": SetDoubleField(gThicknessIP_7, "Thickness IP - 7"); break;
                case "gThicknessIP_8": SetDoubleField(gThicknessIP_8, "Thickness IP - 8"); break;
                case "gThicknessIP_9": SetDoubleField(gThicknessIP_9, "Thickness IP - 9"); break;
                case "gThicknessIP_10": SetDoubleField(gThicknessIP_10, "Thickness IP - 10"); break;
                case "gThicknessIP_11": SetDoubleField(gThicknessIP_11, "Thickness IP - 11"); break;
                case "gThicknessIP_12": SetDoubleField(gThicknessIP_12, "Thickness IP - 12"); break;
                case "gThicknessIP_13": SetDoubleField(gThicknessIP_13, "Thickness IP - 13"); break;
                case "gThicknessIP_14": SetDoubleField(gThicknessIP_14, "Thickness IP - 14"); break;
                case "gThicknessIP_15": SetDoubleField(gThicknessIP_15, "Thickness IP - 15"); break;
                case "gThicknessIP_16": SetDoubleField(gThicknessIP_16, "Thickness IP - 16"); break;
                case "gThicknessIP_17": SetDoubleField(gThicknessIP_17, "Thickness IP - 17"); break;

            }

            int nCount = 0; double dSum = 0, dtmp;
            string sField = "Thickness IP - ";
            double dmin = double.MaxValue, dmax = double.MinValue;

            for (int i = 1; i < 18; i++)
            {
                sField = "Thickness IP - " + i.ToString();
                if (!(dr[sField] == DBNull.Value))
                {
                    dtmp = (double)dr[sField]; dSum += dtmp; nCount += 1;
                    if (dtmp < dmin) dmin = dtmp; if (dtmp > dmax) dmax = dtmp;
                }
            }


            dtmp = dSum / (double)nCount;
            if (nCount < 17 || Double.IsNaN(dtmp))
            {
                gThicknessIP.Text = gThicknessValley.Text = gThicknessPeak.Text = gFlatness.Text = string.Empty;
                dr["Thickness - IP"] = dr["thickness IP- valleys"] = dr["thickness peaks IP"] = dr["Flatness IP"] = DBNull.Value;
            }
            else
            {
                gThicknessIP.Text = dtmp.ToString(sOr); dr["Thickness - IP"] = dtmp;
                gThicknessValley.Text = dmin.ToString(sOr); dr["thickness IP- valleys"] = dmin;
                gThicknessPeak.Text = dmax.ToString(sOr); dr["thickness peaks IP"] = dmax;
                gFlatness.Text = (dmin - dmax).ToString(sOr); dr["Flatness IP"] = dmin - dmax;
            }

            nCount = 0;
            dSum = 0.0;
            for (int i = 1; i < 9; i++)
            {
                sField = "Thickness IP - " + i.ToString();
                if (!(dr[sField] == DBNull.Value)) { dtmp = (double)dr[sField]; dSum += dtmp; nCount += 1; }
            }
            dtmp = dSum / (double)nCount;
            if (nCount < 8 || Double.IsNaN(dtmp)) { gThicknessAvg1.Text = string.Empty; dr["Thickness IP Avg1"] = DBNull.Value; }
            else { gThicknessAvg1.Text = dtmp.ToString(sOr); dr["Thickness IP Avg1"] = dtmp; }

            nCount = 0;
            dSum = 0.0;
            for (int i = 10; i < 18; i++)
            {
                sField = "Thickness IP - " + i.ToString();
                if (!(dr[sField] == DBNull.Value)) { dtmp = (double)dr[sField]; dSum += dtmp; nCount += 1; }
            }
            dtmp = dSum / (double)nCount;
            if (nCount < 8 || Double.IsNaN(dtmp)) { gThicknessAvg2.Text = string.Empty; dr["Thickness IP Avg2"] = DBNull.Value; }
            else { gThicknessAvg2.Text = dtmp.ToString(sOr); dr["Thickness IP Avg2"] = dtmp; }

            if (dr["Thickness IP Avg1"] != DBNull.Value && dr["Thickness IP Avg2"] != DBNull.Value)
            {
                dtmp = Math.Abs((double)dr["Thickness IP Avg1"] - (double)dr["Thickness IP Avg2"]); gThicknessSlope.Text = dtmp.ToString(sOr); dr["IP Thickness Slope"] = dtmp;
            }
            else gThicknessSlope.Text = string.Empty;


            UpdateDataSet();
            CheckLimits("gThicknessIP");
            CheckLimits("gThicknessAvg1");
            CheckLimits("gThicknessAvg2");
            CheckLimits("gThicknessSlope");
            CheckLimits("gFlatness");



        }

        private void CoreDenAv1_LF(object sender, RoutedEventArgs e)
        {
            string sFld = String.Empty;
            TextBox tbx = sender as TextBox;
            string sText = tbx.Text;
            string sName = tbx.Name;
            int nm, nl, nw, nt;

            bDataSetChanged = true;

            switch (sName)
            {
                case "gMass_1": SetDoubleField(gMass_1, "Mass 1"); break;
                case "gL1_1": SetDoubleField(gL1_1, "L1 1"); break;
                //                case "gL2_1": SetDoubleField(sText, "L2 1"); break;
                case "gW1_1": SetDoubleField(gW1_1, "W1 1"); break;
                //               case "gW2_1": SetDoubleField(sText, "W2 1"); break;
                case "gT1_1": SetDoubleField(gT1_1, "T1 1"); break;
                case "gT2_1": SetDoubleField(gT2_1, "T2 1"); break;
                case "gT3_1": SetDoubleField(gT3_1, "T3 1"); break;
                case "gT4_1": SetDoubleField(gT4_1, "T4 1"); break;
                case "gT5_1": SetDoubleField(gT5_1, "T5 1"); break;
            }

            int nCount = 0; double dSum = 0, dtmp;
            double dm = 0, dl, dw, dt;

            nm = nl = nw = nt = 0;
            dm = dl = dw = dt = 0.0;

            if (!(dr["Mass 1"] == DBNull.Value)) { dm = (double)dr["Mass 1"]; nm = 1; }

            nCount = 0; dSum = 0;
            if (!(dr["L1 1"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["L1 1"]; }
            //            if (!(dr["L2 1"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["L2 1"]; }

            if (nCount > 0) { dl = dSum / (double)nCount; nl = 2; }



            nCount = 0; dSum = 0;
            if (!(dr["W1 1"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["W1 1"]; }
            //           if (!(dr["W2 1"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["W2 1"]; }
            if (nCount > 0) { dw = dSum / (double)nCount; nw = 2; }

            nCount = 0; dSum = 0;
            if (!(dr["T1 1"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T1 1"]; }
            if (!(dr["T2 1"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T2 1"]; }
            if (!(dr["T3 1"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T3 1"]; }
            if (!(dr["T4 1"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T4 1"]; }
            if (!(dr["T5 1"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T5 1"]; }

            if (nCount > 3) { dt = dSum / (double)nCount; nt = 5; }

            if (nm * nl * nw * nt > 0 && dl * dw * dt > 0.0) { dtmp = dm / (dl * dw * dt) * 3.809590998; gCoreDensityIP_1.Text = dtmp.ToString(sOr); dr["Core Density - IP 1"] = dtmp; }
            else { gCoreDensityIP_1.Text = String.Empty; dr["Core Density - IP 1"] = DBNull.Value; }


            AvCoreDensity();
            UpdateDataSet();
        }

        private void CoreDenAv2_LF(object sender, RoutedEventArgs e)
        {
            string sFld = String.Empty;
            TextBox tbx = sender as TextBox;
            string sText = tbx.Text;
            string sName = tbx.Name;

            bDataSetChanged = true;

            switch (sName)
            {
                case "gMass_2": SetDoubleField(gMass_2, "Mass 2"); break;
                case "gL1_2": SetDoubleField(gL1_2, "L1 2"); break;
                //               case "gL2_2": SetDoubleField(sText, "L2 2"); break;
                case "gW1_2": SetDoubleField(gW1_2, "W1 2"); break;
                //               case "gW2_2": SetDoubleField(sText, "W2 2"); break;
                case "gT1_2": SetDoubleField(gT1_2, "T1 2"); break;
                case "gT2_2": SetDoubleField(gT2_2, "T2 2"); break;
                case "gT3_2": SetDoubleField(gT3_2, "T3 2"); break;
                case "gT4_2": SetDoubleField(gT4_2, "T4 2"); break;
                case "gT5_2": SetDoubleField(gT5_2, "T5 2"); break;
            }


            int nCount = 0; double dSum = 0, dtmp;
            int nm, nl, nw, nt;
            double dm = 0, dl, dw, dt;

            nm = nl = nw = nt = 0;
            dm = dl = dw = dt = 0.0;

            if (!(dr["Mass 2"] == DBNull.Value)) { dm = (double)dr["Mass 2"]; nm = 1; }

            nCount = 0; dSum = 0;
            if (!(dr["L1 2"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["L1 2"]; }
            //           if (!(dr["L2 2"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["L2 2"]; }

            if (nCount > 0) { dl = dSum / (double)nCount; nl = 2; }



            nCount = 0; dSum = 0;
            if (!(dr["W1 2"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["W1 2"]; }
            //           if (!(dr["W2 2"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["W2 2"]; }
            if (nCount > 0) { dw = dSum / (double)nCount; nw = 2; }

            nCount = 0; dSum = 0;
            if (!(dr["T1 2"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T1 2"]; }
            if (!(dr["T2 2"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T2 2"]; }
            if (!(dr["T3 2"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T3 2"]; }
            if (!(dr["T4 2"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T4 2"]; }
            if (!(dr["T5 2"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T5 2"]; }

            if (nCount > 3) { dt = dSum / (double)nCount; nt = 5; }

            if (nm * nl * nw * nt > 0 && dl * dw * dt > 0.0) { dtmp = dm / (dl * dw * dt) * 3.809590998; gCoreDensityIP_2.Text = dtmp.ToString(sOr); dr["Core Density - IP 2"] = dtmp; }
            else { gCoreDensityIP_2.Text = String.Empty; dr["Core Density - IP 2"] = DBNull.Value; }


            AvCoreDensity();

            UpdateDataSet();
        }

        private void CoreDenAv3_LF(object sender, RoutedEventArgs e)
        {
            string sFld = String.Empty;
            TextBox tbx = sender as TextBox;
            string sText = tbx.Text;
            string sName = tbx.Name;

            bDataSetChanged = true;

            switch (sName)
            {
                case "gMass_3": SetDoubleField(gMass_3, "Mass 3"); break;
                case "gL1_3": SetDoubleField(gL1_3, "L1 3"); break;
                //               case "gL2_3": SetDoubleField(sText, "L2 3"); break;
                case "gW1_3": SetDoubleField(gW1_3, "W1 3"); break;
                //                case "gW2_3": SetDoubleField(sText, "W2 3"); break;
                case "gT1_3": SetDoubleField(gT1_3, "T1 3"); break;
                case "gT2_3": SetDoubleField(gT2_3, "T2 3"); break;
                case "gT3_3": SetDoubleField(gT3_3, "T3 3"); break;
                case "gT4_3": SetDoubleField(gT4_3, "T4 3"); break;
                case "gT5_3": SetDoubleField(gT5_3, "T5 3"); break;
            }


            int nCount = 0; double dSum = 0, dtmp;
            int nm, nl, nw, nt;
            double dm = 0, dl, dw, dt;

            nm = nl = nw = nt = 0;
            dm = dl = dw = dt = 0.0;

            if (!(dr["Mass 3"] == DBNull.Value)) { dm = (double)dr["Mass 3"]; nm = 1; }

            nCount = 0; dSum = 0;
            if (!(dr["L1 3"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["L1 3"]; }
            //           if (!(dr["L2 3"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["L2 3"]; }

            if (nCount > 0) { dl = dSum / (double)nCount; nl = 2; }



            nCount = 0; dSum = 0;
            if (!(dr["W1 3"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["W1 3"]; }
            //           if (!(dr["W2 3"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["W2 3"]; }
            if (nCount > 0) { dw = dSum / (double)nCount; nw = 2; }

            nCount = 0; dSum = 0;
            if (!(dr["T1 3"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T1 3"]; }
            if (!(dr["T2 3"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T2 3"]; }
            if (!(dr["T3 3"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T3 3"]; }
            if (!(dr["T4 3"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T4 3"]; }
            if (!(dr["T5 3"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["T5 3"]; }

            if (nCount > 3) { dt = dSum / (double)nCount; nt = 5; }

            if (nm * nl * nw * nt > 0 && dl * dw * dt > 0.0) { dtmp = dm / (dl * dw * dt) * 3.809590998; gCoreDensityIP_3.Text = dtmp.ToString(sOr); dr["Core Density - IP 3"] = dtmp; }
            else { gCoreDensityIP_3.Text = String.Empty; dr["Core Density - IP 3"] = DBNull.Value; }

            AvCoreDensity();

            UpdateDataSet();
        }

        private void AvCoreDensity()
        {
            int nCount = 0; double dSum = 0, dtmp;

            if (!(dr["Core Density - IP 1"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["Core Density - IP 1"]; }
            if (!(dr["Core Density - IP 2"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["Core Density - IP 2"]; }
            if (!(dr["Core Density - IP 3"] == DBNull.Value)) { nCount += 1; dSum += (double)dr["Core Density - IP 3"]; }

            dtmp = dSum / (double)nCount;
            if (nCount == 0 || Double.IsNaN(dtmp)) { gCoreDensityIP.Text = String.Empty; dr["Core Density - IP"] = DBNull.Value; }
            else { gCoreDensityIP.Text = dtmp.ToString(sOr); dr["Core Density - IP"] = dtmp; }

            CheckLimits("gCoreDensityIP"); CheckLimits("gCoreDensityIP_1"); CheckLimits("gCoreDensityIP_2"); CheckLimits("gCoreDensityIP_3");

        }

        private void gBoardPerfs_LF(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            string sName = ctrl.Name;

            bDataSetChanged = true;

            switch (sName)
            {
                case "gTopBoardPerforated": if (gTopBoardPerforated.IsChecked == null) dr["Top Board Perforated"] = DBNull.Value; else dr["Top Board Perforated"] = gTopBoardPerforated.IsChecked; break;
                case "gBottomBoardPerforated": if (gBottomBoardPerforated.IsChecked == null) dr["Bottom Board Perforated"] = DBNull.Value; else dr["Bottom Board Perforated"] = gBottomBoardPerforated.IsChecked; break;
                case "gTopBoardPrintOK": if (gTopBoardPrintOK.IsChecked == null) dr["Top Board Print OK"] = DBNull.Value; else dr["Top Board Print OK"] = gTopBoardPrintOK.IsChecked; break;
                case "gBottomBoardPrintOK": if (gBottomBoardPrintOK.IsChecked == null) dr["Bottom Board Print OK"] = DBNull.Value; else dr["Bottom Board Print OK"] = gBottomBoardPrintOK.IsChecked; break;

            }
            UpdateDataSet();
        }

        private void CompressiveStr_LF(object sender, RoutedEventArgs e)
        {
            string sFld = String.Empty;
            TextBox tbx = sender as TextBox;
            string sText = tbx.Text;
            string sName = tbx.Name;

            bDataSetChanged = true;

            switch (sName)
            {
                case "gCompressiveIP_1": SetDoubleField(gCompressiveIP_1, "Compressive IP - 1"); break;
                case "gCompressiveIP_2": SetDoubleField(gCompressiveIP_2, "Compressive IP - 2"); break;
                case "gCompressiveIP_3": SetDoubleField(gCompressiveIP_3, "Compressive IP - 3"); break;
                case "gCompressiveIP_4": SetDoubleField(gCompressiveIP_4, "Compressive IP - 4"); break;
                case "gCompressiveIP_5": SetDoubleField(gCompressiveIP_5, "Compressive IP - 5"); break;
                case "gCompressiveIP_6": SetDoubleField(gCompressiveIP_6, "Compressive IP - 6"); break;
            }


            int nCount = 0;
            double dSum = 0.0, dtmp, dMin = double.MaxValue;

            if (!(dr["Compressive IP - 1"] == DBNull.Value)) { nCount += 1; dtmp = (double)dr["Compressive IP - 1"]; dSum += dtmp; if (dMin > dtmp) dMin = dtmp; }
            if (!(dr["Compressive IP - 2"] == DBNull.Value)) { nCount += 1; dtmp = (double)dr["Compressive IP - 2"]; dSum += dtmp; if (dMin > dtmp) dMin = dtmp; }
            if (!(dr["Compressive IP - 3"] == DBNull.Value)) { nCount += 1; dtmp = (double)dr["Compressive IP - 3"]; dSum += dtmp; if (dMin > dtmp) dMin = dtmp; }
            if (!(dr["Compressive IP - 4"] == DBNull.Value)) { nCount += 1; dtmp = (double)dr["Compressive IP - 4"]; dSum += dtmp; if (dMin > dtmp) dMin = dtmp; }
            if (!(dr["Compressive IP - 5"] == DBNull.Value)) { nCount += 1; dtmp = (double)dr["Compressive IP - 5"]; dSum += dtmp; if (dMin > dtmp) dMin = dtmp; }
            if (!(dr["Compressive IP - 6"] == DBNull.Value)) { nCount += 1; dtmp = (double)dr["Compressive IP - 6"]; dSum += dtmp; if (dMin > dtmp) dMin = dtmp; }

            if (nCount == 6)
            {
                dr["Compressive Strength - IP"] = dSum / 6.0; gCompressiveIP.Text = (dSum / 6.0).ToString(sOr);
                dr["Compressive Strength 5 - IP"] = (dSum - dMin) / 5.0; gCompressiveIP5.Text = ((dSum - dMin) / 5.0).ToString(sOr);
            }
            else if (nCount == 5)
            {
                dr["Compressive Strength - IP"] = DBNull.Value; gCompressiveIP.Text = String.Empty;
                dr["Compressive Strength 5 - IP"] = dSum / 5.0; gCompressiveIP5.Text = (dSum / 5.0).ToString(sOr);
            }

            else
            {
                dr["Compressive Strength - IP"] = DBNull.Value; gCompressiveIP.Text = String.Empty;
                dr["Compressive Strength 5 - IP"] = DBNull.Value; gCompressiveIP5.Text = String.Empty;
            }


            UpdateDataSet();
            CheckLimits("gCompressiveIP"); CheckLimits("gCompressiveIP5");

        }

        private void gPourTable_LF(object sender, EventArgs e)
        {

            Control ctrl = sender as Control;
            string sName = ctrl.Name;
            bDataSetChanged = true;

            switch (sName)
            {
                //                case "gTimePourTableQCCheck": if (gTimePourTableQCCheck == null) dr["Time of Pour Table QC Check"] = DBNull.Value; else dr["Time of Pour Table QC Check"] = gTimePourTableQCCheck.Value; break;
                case "gSurfactant": if (gSurfactant.SelectedValue == null) dr["Surfactant Type"] = DBNull.Value; else { dr["Surfactant Type"] = gSurfactant.SelectedValue; } break;
                case "gLayout": if (gLayout.SelectedValue == null) dr["Pour Table Layout"] = DBNull.Value; else { dr["Pour Table Layout"] = gLayout.SelectedValue; } break;

                case "gSplitterAge": SetIntField(gSplitterAge, "Splitter Age (minutes)"); break;
                case "gHeadPlateAge": SetIntField(gHeadPlateAge, "Headplate Age / Pour Run Time (minutes)"); break;

                case "gPourHeadPosition_1": SetDoubleField(gPourHeadPosition_1, "Pour Head Position - 1"); break;
                case "gPourHeadPosition_2": SetDoubleField(gPourHeadPosition_2, "Pour Head Position - 2"); break;
                case "gPourHeadPosition_3": SetDoubleField(gPourHeadPosition_3, "Pour Head Position - 3"); break;

                case "gIRFacerTempLower": SetDoubleField(gIRFacerTempLower, "IR Facer Temp - Lower"); break;
                case "gIRFacerTempUpper": SetDoubleField(gIRFacerTempUpper, "IR Facer Temp - Upper"); break;

                case "gIRStreamTempPourHead_1": SetDoubleField(gIRStreamTempPourHead_1, "IR Stream Temp - Pour Head 1"); break;
                case "gIRStreamTempPourHead_2": SetDoubleField(gIRStreamTempPourHead_2, "IR Stream Temp - Pour Head 2"); break;
                case "gIRStreamTempPourHead_3": SetDoubleField(gIRStreamTempPourHead_3, "IR Stream Temp - Pour Head 3"); break;

                case "gIRNipRoll_1": SetDoubleField(gIRNipRoll_1, "IR Stream Temp - Nipp Roll 1"); break;
                case "gIRNipRoll_2": SetDoubleField(gIRNipRoll_2, "IR Stream Temp - Nipp Roll 2"); break;
                case "gIRNipRoll_3": SetDoubleField(gIRNipRoll_3, "IR Stream Temp - Nipp Roll 3"); break;
            }
            UpdateDataSet();
        }

        private void gNewDataSetMfg2_Click(object sender, RoutedEventArgs e)
        {

            if (!CPages.PageMfgHome_1.CreateNewDataSet())
            {
                string sMsg = "Errors in creating new dataset.  The network may not be available";
                MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SetView();

        }



        private void gKintLineLocs_LF(object sender, RoutedEventArgs e)
        {
            string sFld = String.Empty;
            TextBox tbx = sender as TextBox;
            string sText = tbx.Text;
            string sName = tbx.Name;

            bDataSetChanged = true;

            switch (sName)
            {
                case "gKnitLineLoc_1": SetDoubleField(gKnitLineLoc_1, "Knit Line Loc 1"); break;
                case "gKnitLineLoc_2": SetDoubleField(gKnitLineLoc_2, "Knit Line Loc 2"); break;
                case "gKnitLineLoc_3": SetDoubleField(gKnitLineLoc_3, "Knit Line Loc 3"); break;
                case "gKnitLineLoc_4": SetDoubleField(gKnitLineLoc_4, "Knit Line Loc 4"); break;
                case "gKnitLineLoc_5": SetDoubleField(gKnitLineLoc_5, "Knit Line Loc 5"); break;
                case "gKnitLineLoc_6": SetDoubleField(gKnitLineLoc_6, "Knit Line Loc 6"); break;
                case "gKnitLineLoc_7": SetDoubleField(gKnitLineLoc_7, "Knit Line Loc 7"); break;
            }
            UpdateDataSet();
        }

        private void gDateTime_LF(object sender, EventArgs e)
        {
            bool bGetProcessData = false;

            if (gTestDateTime.CustomFormat == sNull) dr["Test Date"] = DBNull.Value;
            else if (gTestDateTime.Value == null) dr["Test Date"] = DBNull.Value;
            else { dr["Test Date"] = gTestDateTime.Value; bGetProcessData = true; }
            UpdateDataSet();
            if (bGetProcessData) CheckLimits("BoardTimeStamp");
            CPages.PageMfgHome_1.dt.Rows[Cbfile.iIDMfgIndex]["sDate"] = ((DateTime)dr["Test Date"]).ToString("MM/dd/yyyy");
            CPages.PageMfgHome_1.dt.Rows[Cbfile.iIDMfgIndex]["sTestTime"] = ((DateTime)dr["Test Date"]).ToString("MM/dd/yy - hh:mm tt");
            CPages.PageMfgHome_1.gMfgSearch.ItemsSource = CPages.PageMfgHome_1.dt.DefaultView;
        }

        private void gProdComboBox_SC(object sender, SelectionChangedEventArgs e)
        {
            string sField = String.Empty;

            ComboBox cmx = sender as ComboBox;
            sField = "Product ID";

            if (cmx.SelectedValue == null)
            {
                dr[sField] = DBNull.Value;
                CPages.PageMfgHome_1.dt.Rows[Cbfile.iIDMfgIndex]["Product"] = string.Empty;
            }
            else
            {
                dr[sField] = cmx.SelectedValue;
                CPages.PageMfgHome_1.dt.Rows[Cbfile.iIDMfgIndex]["Product"] = ((DataRowView)(cmx.SelectedItem))["Product"].ToString();
            }
            CPages.PageMfgHome_1.gMfgSearch.ItemsSource = CPages.PageMfgHome_1.dt.DefaultView;
            CheckLimits("All");
            UpdateDataSet();
        }

        private void gTime_GotFocus(object sender, EventArgs e)
        {
            System.Windows.Forms.DateTimePicker dtp = sender as System.Windows.Forms.DateTimePicker;
            if (dtp.CustomFormat == sNull) { dtp.CustomFormat = sTimeFormat; dtp.Value = DateTime.Now; }
        }

        private void gPourTable_QCTime_LF(object sender, EventArgs e)
        {
            bDataSetChanged = true;
            if (gTimePourTableQCCheck.Value == null) dr["Time of Pour Table QC Check"] = DBNull.Value; else dr["Time of Pour Table QC Check"] = gTimePourTableQCCheck.Value;
            UpdateDataSet();

        }
        #endregion
*/
    }
}

