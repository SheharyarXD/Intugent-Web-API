using IntugentBackend.Services.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using IntugentBackend.Services.Data;


namespace IntugentBackend.Services.Rnd
{
    public class RNDFormulations
    {
        public CForms Forms = new CForms();
        public CMaterial AirMaterial = new CMaterial();
        public string[] sMatNameListPO = { "Polyol A" }, sMatTypeListPO = { "Polyol" };
        public string[] sMatNameListIso = { "Polyol A" }, sMatTypeListIso = { "Polyol" };
        //        public CPOForm POForm1 = new CPOForm();
        public DataTable dtPO, dtIso;                       //  Data table for Iso and PO Materials from database
        public DataTable dtFormProp = new DataTable();
        public DataTable dtRecipe = new DataTable();

        string sSqlQuery;
        //       public SqlDataAdapter da;
        public SqlDataAdapter daMat;
        DataTable dt = new DataTable();
        //       public DataRow dr;
        public bool bDataSetChanged = false;
        public const string sDef = "0.0000", sOr = "0.00";
        public CDefualts cDefualts;
        public Cbfile cbfile;
        public RNDHome RNDHome;
        public RNDFormulations(CDefualts cDefualts,Cbfile cbfile,RNDHome  rNDHome) { 
                this.cDefualts = cDefualts;
            this.cbfile = cbfile;
            this.RNDHome = rNDHome;
            Startup();
        }
        public void Startup()
        {
            int id = 0, id1 = 0;

            // Formulation Array
            for (int i = 0; i <  Forms.nForm; i++)  Forms.FormAr[i] = new CForm();


            //Table of Properties          ;

             dtFormProp.Columns.Add("Descriptors", typeof(string));
            for (int i = 1; i <  Forms.nForm + 1; i++)
            {
                 dtFormProp.Columns.Add("#" + i, typeof(double));
            }
            for (int i = 0; i < 30; i++)  dtFormProp.Rows.Add();



            //Initialize Material lists on the PO and Iso Sides

            //            Forms.dtFormIso.Add("Descriptors", typeof(string));
            //           for (int i = 1; i <  Forms.nForm + 1; i++)


             Forms.POMats.Add(new CMaterial());
             Forms.POMats.Add(new CMaterial());

             Forms.IsoMats.Add(new CMaterial()
            {
                ID = 62,
                MatName = "Lupranat® M 20R",
                MatType = "Iso-PMDI",
                MatFunc = 2.7,
                MatNco = 31.5,
                //                Pbw1 = 50.0, Pbw8 = 10
            });




             Forms.NCOIndexMats.Add(new CMaterial()
            {
                MatName = "NCO Index (Equivalents of NCO per 100 Equivalents of Active-H",
                Pbw1 = "270",
                Pbw2 = "270",
                Pbw3 = "270",
                Pbw4 = "270",
                Pbw5 = "270",
                Pbw6 = "270",
                Pbw7 = "270",
                Pbw8 = "270"

            });
            for (int ifo = 0; ifo <  Forms.nForm; ifo++)
            {
                 Forms.FormAr[ifo].NcoIndex = 270;
                 Forms.FormAr[ifo].BasisPbwPOSide = 100;
            }

            //gPO  =  Forms.POMats;
            //gNco =  Forms.NCOIndexMats;
            //gIso =  Forms.IsoMats;

            //             Forms.dtFormIso.Add

            //            GetMatList();
             GetMatListSql();

             ModifyPOIsoLists(0, ref  Forms.IsoMats, 0,  dtIso);  //fill the grid with the 1st material on the list

            for (int i = 0; i <  dtPO.Rows.Count; i++)
            {
                if ( dtPO.Rows[i]["ID"].ToString() == "106") id = i;
                if ( dtPO.Rows[i]["ID"].ToString() == "89") id1 = i;

            }
             ModifyPOIsoLists(0, ref  Forms.POMats, id,  dtPO);
             ModifyPOIsoLists(1, ref  Forms.POMats, id1,  dtPO);


        }

        public void GetMatListSql()
        {
            string sql;
            int itmp;
            string sMsg = "Error opening databases";



            try
            {
                sql = "select * from[dbo].[tblRawMaterials] where MatCat<> 'Iso Side' AND GAFMat = 'True' order by MatName";
                daMat = new SqlDataAdapter(sql, cbfile.conAZ);
                dtPO = new DataTable(); itmp = daMat.Fill(dtPO);

                Array.Resize(ref sMatNameListPO, dtPO.Rows.Count); Array.Resize(ref sMatTypeListPO, dtPO.Rows.Count);
                for (int i = 0; i < dtPO.Rows.Count; i++) { sMatNameListPO[i] = (dtPO.Rows[i]["MatName"].ToString()); sMatTypeListPO[i] = dtPO.Rows[i]["MatType"].ToString(); }

                sql = "select * from[dbo].[tblRawMaterials] where MatCat<> 'PO Side' AND GAFMat = 'True' order by MatName";
                daMat = new SqlDataAdapter(sql, cbfile.conAZ);
                dtIso = new DataTable(); itmp = daMat.Fill(dtIso);

                Array.Resize(ref sMatNameListIso, dtIso.Rows.Count); Array.Resize(ref sMatTypeListIso, dtIso.Rows.Count);
                for (int i = 0; i < dtIso.Rows.Count; i++) { sMatNameListIso[i] = (dtIso.Rows[i]["MatName"].ToString()); sMatTypeListIso[i] = dtIso.Rows[i]["MatType"].ToString(); }

            }
            catch (Exception ex)
            {
                sMsg = "Errors in obtaining the material lists for R&D Formulations";
                // MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //  CTelClient.TelException(ex, sMsg);
            }
        }
        public void ModifyPOIsoLists(int irow, ref ObservableCollection<CMaterial> Mats, int iSel, DataTable dt)
        {
            /*
            Make sure that the first material in the PO and Iso list declarled in intializing the page have all the following variables
            */

            double dtemp; int itemp;

            if (iSel < 0 || iSel > dt.Rows.Count - 1) return;

            int.TryParse(dt.Rows[iSel]["ID"].ToString(), out itemp); Mats[irow].ID = itemp;
            Mats[irow].MatName = dt.Rows[iSel]["MatName"].ToString();
            Mats[irow].MatType = dt.Rows[iSel]["MatType"].ToString();
            double.TryParse(dt.Rows[iSel]["MatFunc"].ToString(), out dtemp); Mats[irow].MatFunc = dtemp;
            double.TryParse(dt.Rows[iSel]["MatOH"].ToString(), out dtemp); Mats[irow].MatOHNum = dtemp;
            double.TryParse(dt.Rows[iSel]["MatIsoCont"].ToString(), out dtemp); Mats[irow].MatNco = dtemp;
            double.TryParse(dt.Rows[iSel]["MolWt"].ToString(), out dtemp); Mats[irow].MolWt = dtemp;

            Mats[irow].GasName = dt.Rows[iSel]["GasName"].ToString();
            double.TryParse(dt.Rows[iSel]["GassToLiqWtRatio"].ToString(), out dtemp); Mats[irow].GassToLiqWtRatio = dtemp;
            double.TryParse(dt.Rows[iSel]["GassMolWt"].ToString(), out dtemp); Mats[irow].GassMolWt = dtemp;
            double.TryParse(dt.Rows[iSel]["GasBoilingTemp"].ToString(), out dtemp); Mats[irow].GasBoilingTemp = dtemp;
            double.TryParse(dt.Rows[iSel]["GasCond_A"].ToString(), out dtemp); Mats[irow].GasCond_A = dtemp;
            double.TryParse(dt.Rows[iSel]["GasCond_B"].ToString(), out dtemp); Mats[irow].GasCond_B = dtemp;
            double.TryParse(dt.Rows[iSel]["GasVis_A"].ToString(), out dtemp); Mats[irow].GasVis_A = dtemp;
            double.TryParse(dt.Rows[iSel]["GasVis_B"].ToString(), out dtemp); Mats[irow].GasVis_B = dtemp;
            double.TryParse(dt.Rows[iSel]["GasVapPAtm_A"].ToString(), out dtemp); Mats[irow].GasVapPAtm_A = dtemp;
            double.TryParse(dt.Rows[iSel]["GasVapPAtm_B"].ToString(), out dtemp); Mats[irow].GasVapPAtm_B = dtemp;
            double.TryParse(dt.Rows[iSel]["GasVapPAtm_C"].ToString(), out dtemp); Mats[irow].GasVapPAtm_C = dtemp;

        }


        public void FormDescriptors()
        {
            int i, ico, ifo;
            double dsumPbw, dsumPbwPO, dsumOH, dsumOHPO, dsumFunc, dsumFunc1, dsumNco, dsumBlowingAg, dsumBlowingAg1, dsumWater, dsumSurfac, dsumCatalyst;
            double temp1, temp2, temp3;
            string sMsg;

            //Calculates Equivs of polyols.   

            try
            {
                for (ifo = 0; ifo <  Forms.nForm; ifo++)
                {

                    dsumPbw = dsumPbwPO = dsumOH = dsumOHPO = dsumFunc = dsumFunc1 = dsumBlowingAg = dsumBlowingAg1 = dsumWater = dsumSurfac = dsumCatalyst = 0.0;
                    for (ico = 0; ico <  Forms.POMats.Count; ico++)
                    {
                        if (string.IsNullOrEmpty( Forms.POMats[ico].MatName))
                            if ( Forms.FormAr[ifo].POMatPbw[ico] > 0)
                            {
                                sMsg = "You must choose the material for the row " + (ico + 1).ToString() + " of the Polyol Side Materials table";
                                // MessageBox.Show(sMsg, Params.sAppName, MessageBoxButton.OK, MessageBoxImage.Hand);
                                /*
                                                                if (ico >  Forms.POMats.Count - 1)  Forms.POMats.Add(new CMaterial());
                                                                    ModifyPOIsoLists(ico, ref  Forms.POMats, 0, dtPO);
                                */
                                return;
                            }
                            else continue;

                        else
                        {
                            dsumPbw = dsumPbw +  Forms.FormAr[ifo].POMatPbw[ico];
                            dsumOH = dsumOH +  Forms.FormAr[ifo].POMatPbw[ico] *  Forms.POMats[ico].MatOHNum;
                            if ( Forms.POMats[ico].MatType.Contains("Polyol"))
                            {
                                dsumPbwPO = dsumPbwPO +  Forms.FormAr[ifo].POMatPbw[ico];
                                dsumOHPO = dsumOHPO +  Forms.FormAr[ifo].POMatPbw[ico] *  Forms.POMats[ico].MatOHNum;
                            }
                            if ( Forms.POMats[ico].MatFunc > 0)
                            {
                                dsumFunc = dsumFunc +  Forms.FormAr[ifo].POMatPbw[ico] *  Forms.POMats[ico].MatOHNum *  Forms.POMats[ico].MatFunc;
                                dsumFunc1 = dsumFunc1 +  Forms.FormAr[ifo].POMatPbw[ico] *  Forms.POMats[ico].MatOHNum;
                            }
                            if ( Forms.POMats[ico].GassToLiqWtRatio > 0.0)
                            {
                                dsumBlowingAg = dsumBlowingAg +  Forms.FormAr[ifo].POMatPbw[ico] *  Forms.POMats[ico].GassToLiqWtRatio;
                                dsumBlowingAg1 = dsumBlowingAg1 +  Forms.FormAr[ifo].POMatPbw[ico];
                            }
                            if ( Forms.POMats[ico].MatType.Contains("Catalyst")) dsumCatalyst = dsumCatalyst +  Forms.FormAr[ifo].POMatPbw[ico];
                            if ( Forms.POMats[ico].MatType.Contains("Surfactant")) dsumSurfac = dsumSurfac +  Forms.FormAr[ifo].POMatPbw[ico];
                            if ( Forms.POMats[ico].ID == 74) dsumWater = dsumWater +  Forms.FormAr[ifo].POMatPbw[ico];
                        }

                    }

                    if (dsumPbw > 0)  Forms.FormAr[ifo].OHNumPOSide = dsumOH / dsumPbw; else  Forms.FormAr[ifo].OHNumPOSide = 0;
                    if (dsumPbwPO > 0)  Forms.FormAr[ifo].OHNumPolyol = dsumOHPO / dsumPbwPO; else  Forms.FormAr[ifo].OHNumPolyol = 0;
                    if (dsumFunc1 > 0)  Forms.FormAr[ifo].FuncAvPOSide = dsumFunc / dsumFunc1; else  Forms.FormAr[ifo].FuncAvPOSide = 0;
                     Forms.FormAr[ifo].TotalPbwPOSide = dsumPbw;
                     Forms.FormAr[ifo].TotalPbwPolyol = dsumPbwPO;
                     Forms.FormAr[ifo].TotalPbwFuncPOSide = dsumFunc1;
                     Forms.FormAr[ifo].BlowingAgentWtFr = dsumBlowingAg;  //Total wt of blowing agent in PO side.  Will be normalized later
                     Forms.FormAr[ifo].SurfactWtFr = dsumSurfac;    //Total wt. will be normalized later
                     Forms.FormAr[ifo].CatalystWtFr = dsumCatalyst;
                     Forms.FormAr[ifo].WaterWtFr = dsumWater;
                     Forms.FormAr[ifo].BlowingAgentWtFr1 = dsumBlowingAg1 - dsumWater;

                    /*
                    dsumPbw = dsumNco = dsumFunc = dsumFunc1 = dsumBlowingAg = 0.0;
                    for (ico = 0; ico < Forms.IsoMats.Count; ico++)
                    {

                        if (Forms.FormAr[ifo].IsoMatPbw[ico] > 0 && string.IsNullOrEmpty( Forms.IsoMats[ico].MatName))
                        {
                            sMsg = "You must choose the material for the row " + (ico + 1).ToString() + " of the Iso Side Material table";
                            MessageBox.Show(sMsg, Params.sAppName, MessageBoxButton.OK, MessageBoxImage.Hand);
                            return;
                        }
                        else
                        {
                            dsumPbw = dsumPbw +  Forms.FormAr[ifo].IsoMatPbw[ico];
                            dsumNco = dsumNco +  Forms.FormAr[ifo].IsoMatPbw[ico] *  Forms.IsoMats[ico].MatNco;

                            if ( Forms.IsoMats[ico].MatFunc > 0)
                            {
                                dsumFunc = dsumFunc +  Forms.FormAr[ifo].IsoMatPbw[ico] *  Forms.IsoMats[ico].MatNco *  Forms.IsoMats[ico].MatFunc;
                                dsumFunc1 = dsumFunc1 +  Forms.FormAr[ifo].IsoMatPbw[ico] *  Forms.IsoMats[ico].MatNco;

                            }

                        }
                    }

                    if (dsumPbw > 0)  Forms.FormAr[ifo].NcoIsoSide = dsumNco / dsumPbw; else  Forms.FormAr[ifo].NcoIsoSide = 0;
                    if (dsumFunc1 > 0)  Forms.FormAr[ifo].FuncAvIsoSide = dsumFunc / dsumFunc1; else  Forms.FormAr[ifo].FuncAvIsoSide = 0;

                     Forms.FormAr[ifo].TotalPbwIsoSide = dsumPbw;
                     Forms.FormAr[ifo].TotalPbwFuncIsoSide = dsumFunc1;
                     */
                     Forms.FormAr[ifo].NcoIsoSide =  Forms.IsoMats[0].MatNco;
                     Forms.FormAr[ifo].FuncAvIsoSide =  Forms.IsoMats[0].MatFunc;

                }

                // PO and Iso side weights calculations, Isocyanurate trimer and crosslink density calculations

                for (ifo = 0; ifo <  Forms.nForm; ifo++)
                {
                    /*
                     *  Forms.FormAr[ifo].BasisPbwPOSide = 100.0;
                                       if ( Forms.FormAr[ifo].NcoIsoSide > 0)
                                            Forms.FormAr[ifo].BasisPbwIsoSide =  Forms.FormAr[ifo].BasisPbwPOSide *  Forms.FormAr[ifo].NcoIndex / 100.0 *  Forms.FormAr[ifo].OHNumPOSide / 56100.0 * 4200.0 /  Forms.FormAr[ifo].NcoIsoSide;
                                       if ( Forms.FormAr[ifo].BasisPbwIsoSide > 0.0)
                                            Forms.FormAr[ifo].PolyolIsoRatio =  Forms.FormAr[ifo].BasisPbwPOSide /  Forms.FormAr[ifo].BasisPbwIsoSide * 100.0;
                                       else  Forms.FormAr[ifo].PolyolIsoRatio = 0.0;
                                       if ( Forms.FormAr[ifo].BasisPbwPOSide > 0.0)
                                            Forms.FormAr[ifo].IsoPolyolRatio =  Forms.FormAr[ifo].BasisPbwIsoSide /  Forms.FormAr[ifo].BasisPbwPOSide * 100.0;
                                       else  Forms.FormAr[ifo].IsoPolyolRatio = 0.0;
                    */

                     Forms.FormAr[ifo].NcoIsoSide =  Forms.IsoMats[0].MatNco;
                     Forms.FormAr[ifo].FuncAvIsoSide =  Forms.IsoMats[0].MatFunc;

                     Forms.FormAr[ifo].TotalPbwIsoSide =  Forms.FormAr[ifo].TotalPbwPOSide *  Forms.FormAr[ifo].OHNumPOSide / 56100 * 4200.0 /  Forms.FormAr[ifo].NcoIsoSide *  Forms.FormAr[ifo].NcoIndex * 0.01;
                     Forms.FormAr[ifo].IsoMatPbw[0] =  Forms.FormAr[ifo].TotalPbwIsoSide;


                     Forms.FormAr[ifo].CrosslinkDensity = 0;
                    if ( Forms.FormAr[ifo].NcoIndex <= 100.0)
                    {
                         Forms.FormAr[ifo].IsocyanuratePbw = 0;

                        temp1 = 0.01 *  Forms.FormAr[ifo].NcoIndex * ( Forms.FormAr[ifo].FuncAvPOSide - 2.0);
                        if (temp1 > 0.0)
                             Forms.FormAr[ifo].CrosslinkDensity = 0.5 *  Forms.FormAr[ifo].TotalPbwPOSide *  Forms.FormAr[ifo].OHNumPOSide / 56100.0 /  Forms.FormAr[ifo].FuncAvPOSide * temp1;
                        temp2 =  Forms.FormAr[ifo].FuncAvIsoSide - 2.0;
                        if (temp2 > 0)
                             Forms.FormAr[ifo].CrosslinkDensity += 0.5 *  Forms.FormAr[ifo].TotalPbwIsoSide *  Forms.FormAr[ifo].NcoIsoSide / 4200.0 /  Forms.FormAr[ifo].FuncAvIsoSide * temp2;

                        temp1 =  Forms.FormAr[ifo].TotalPbwPOSide +  Forms.FormAr[ifo].TotalPbwIsoSide;
                         Forms.FormAr[ifo].CrosslinkDensity =  Forms.FormAr[ifo].CrosslinkDensity / temp1 * 1000.0;
                    }

                    else
                    {
                         Forms.FormAr[ifo].CrosslinkDensity = 0;
                        dsumPbw = ( Forms.FormAr[ifo].TotalPbwPOSide +  Forms.FormAr[ifo].TotalPbwIsoSide);
                        if (dsumPbw > 0)  Forms.FormAr[ifo].IsocyanuratePbw = 100.0 * (1.0 - 100.0 /  Forms.FormAr[ifo].NcoIndex) *  Forms.FormAr[ifo].TotalPbwIsoSide / dsumPbw;
                        else  Forms.FormAr[ifo].IsocyanuratePbw = 0;

                        temp1 =  Forms.FormAr[ifo].FuncAvPOSide - 2.0;
                        if (temp1 > 0.0)
                             Forms.FormAr[ifo].CrosslinkDensity += 0.5 *  Forms.FormAr[ifo].TotalPbwPOSide *  Forms.FormAr[ifo].OHNumPOSide / 56100.0 /  Forms.FormAr[ifo].FuncAvPOSide * temp1;
                        temp2 = ( Forms.FormAr[ifo].FuncAvIsoSide - 2.0) * 100 /  Forms.FormAr[ifo].NcoIndex;
                        if (temp2 > 0)
                             Forms.FormAr[ifo].CrosslinkDensity += 0.5 *  Forms.FormAr[ifo].TotalPbwIsoSide *  Forms.FormAr[ifo].NcoIsoSide / 4200.0 /  Forms.FormAr[ifo].FuncAvIsoSide * temp2;
                        temp3 = 3 * ( Forms.FormAr[ifo].FuncAvIsoSide - 1.0) * (1 - 100 /  Forms.FormAr[ifo].NcoIndex) *  Forms.FormAr[ifo].TotalPbwIsoSide;
                        if (temp3 > 0.0)
                             Forms.FormAr[ifo].CrosslinkDensity += 0.5 * temp3 *  Forms.FormAr[ifo].NcoIsoSide / 4200.0 /  Forms.FormAr[ifo].FuncAvIsoSide / 3.0;

                         Forms.FormAr[ifo].CrosslinkDensity =  Forms.FormAr[ifo].CrosslinkDensity / dsumPbw * 1000.0;

                    }

                    /*

                                        if ( Forms.FormAr[ifo].NcoIndex <= 100.0)
                                        {
                                             Forms.FormAr[ifo].IsocyanuratePbw100 = 0;

                                            temp1 = 0.01 *  Forms.FormAr[ifo].NcoIndex *  Forms.FormAr[ifo].FuncAvPOSide - 2.0;
                                            if (temp1 > 0.0)
                                                 Forms.FormAr[ifo].CrosslinkDensity = 0.5 *  Forms.FormAr[ifo].BasisPbwPOSide *  Forms.FormAr[ifo].OHNumPOSide / 56100.0 /  Forms.FormAr[ifo].FuncAvPOSide * temp1;
                                            temp2 =  Forms.FormAr[ifo].FuncAvIsoSide - 2.0;
                                            if (temp2 > 0)
                                                 Forms.FormAr[ifo].CrosslinkDensity = 0.5 *  Forms.FormAr[ifo].BasisPbwIsoSide *  Forms.FormAr[ifo].NcoIsoSide / 4200.0 /  Forms.FormAr[ifo].FuncAvIsoSide * temp2;

                                            temp1 =  Forms.FormAr[ifo].BasisPbwPOSide +  Forms.FormAr[ifo].BasisPbwIsoSide;
                                             Forms.FormAr[ifo].CrosslinkDensity =  Forms.FormAr[ifo].CrosslinkDensity / temp1 * 1000.0;
                                        }

                                        else
                                        {
                                            dsumPbw = ( Forms.FormAr[ifo].BasisPbwPOSide +  Forms.FormAr[ifo].BasisPbwIsoSide);
                                            if (dsumPbw > 0)  Forms.FormAr[ifo].IsocyanuratePbw100 = 100.0 * (1.0 - 100.0 /  Forms.FormAr[ifo].NcoIndex) *  Forms.FormAr[ifo].BasisPbwIsoSide / dsumPbw;
                                            else  Forms.FormAr[ifo].IsocyanuratePbw100 = 0;

                                            temp1 =  Forms.FormAr[ifo].FuncAvPOSide - 2.0;
                                            if (temp1 > 0.0)
                                                 Forms.FormAr[ifo].CrosslinkDensity = 0.5 *  Forms.FormAr[ifo].BasisPbwPOSide *  Forms.FormAr[ifo].OHNumPOSide / 56100.0 /  Forms.FormAr[ifo].FuncAvPOSide * temp1;
                                            temp2 = ( Forms.FormAr[ifo].FuncAvIsoSide - 2.0) * 100 /  Forms.FormAr[ifo].NcoIndex;
                                            if (temp2 > 0)
                                                 Forms.FormAr[ifo].CrosslinkDensity = 0.5 *  Forms.FormAr[ifo].BasisPbwIsoSide *  Forms.FormAr[ifo].NcoIsoSide / 4200.0 /  Forms.FormAr[ifo].FuncAvIsoSide * temp2;
                                            temp3 = 3 * ( Forms.FormAr[ifo].FuncAvIsoSide - 1.0) * (1 - 100 /  Forms.FormAr[ifo].NcoIndex) *  Forms.FormAr[ifo].BasisPbwIsoSide;
                                            if (temp3 > 0.0)
                                                 Forms.FormAr[ifo].CrosslinkDensity = 0.5 * temp3 *  Forms.FormAr[ifo].NcoIsoSide / 4200.0 /  Forms.FormAr[ifo].FuncAvIsoSide / 3.0;

                                            temp1 =  Forms.FormAr[ifo].BasisPbwPOSide +  Forms.FormAr[ifo].BasisPbwIsoSide;
                                             Forms.FormAr[ifo].CrosslinkDensity =  Forms.FormAr[ifo].CrosslinkDensity / temp1 * 1000.0;

                                        }
                    */


                }

                //Calculate the density

                for (ifo = 0; ifo <  Forms.nForm; ifo++)
                {
                    temp1 = temp2 = temp3 = 0;
                    for (ico = 0; ico <  Forms.POMats.Count; ico++)
                    {
                        if ( Forms.POMats[ico].GassToLiqWtRatio > 0)
                        {
                            temp2 = temp2 +  Forms.FormAr[ifo].POMatPbw[ico] *  Forms.POMats[ico].GassToLiqWtRatio; // gas weight
                            temp3 = temp3 + 413.0 / 273.0 * 22.41 *  Forms.FormAr[ifo].POMatPbw[ico] *  Forms.POMats[ico].GassToLiqWtRatio /  Forms.POMats[ico].GassMolWt; //STP Gas vol = 22.41 m3/kg-mole
                        }

                    }


                     Forms.FormAr[ifo].FoamDensity = Params.PolymerDensity;
                    temp1 =  Forms.FormAr[ifo].TotalPbwPOSide +  Forms.FormAr[ifo].TotalPbwIsoSide;    //Total wt
                    temp3 = temp3 + (temp1 - temp2) / Params.PolymerDensity;   //gas volume + polymer volume
                    if (temp3 > 0.0)  Forms.FormAr[ifo].FoamDensity = temp1 / temp3;

                    if (temp1 *  Forms.FormAr[ifo].TotalPbwPOSide > 0.0)
                    {
                         Forms.FormAr[ifo].BlowingAgentWtFr =  Forms.FormAr[ifo].BlowingAgentWtFr / temp1 * 100.0;  //Convert Blowing Agent wt to fraction
                         Forms.FormAr[ifo].CatalystWtFr =  Forms.FormAr[ifo].CatalystWtFr / temp1 * 100.0;  //Convert Blowing Agent wt to fraction
                         Forms.FormAr[ifo].SurfactWtFr =  Forms.FormAr[ifo].SurfactWtFr / temp1 * 100.0;  //Convert Blowing Agent wt to fraction
                         Forms.FormAr[ifo].WaterWtFr =  Forms.FormAr[ifo].WaterWtFr / temp1 * 100.0;  //Convert Blowing Agent wt to fraction
                         Forms.FormAr[ifo].BlowingAgentWtFr1 =  Forms.FormAr[ifo].BlowingAgentWtFr1 / temp1 * 100.0;  //Convert Blowing Agent wt to fraction

                    }

                }


            }
            catch (Exception ex)
            {
                sMsg = "Error in calculating formulation descriptors";
                //MessageBox.Show(sMsg, Cbfile.sAppName); CTelClient.TelException(ex, sMsg); 
            }
        }
        public void ReadDataset()
        {
            int id, iSel, j, nRows, itmp;
            double dtmp;

            char[] delimiterChars = new char[] { ' ', ',', ':', '\t' };

            if (RNDHome.drS["PORows"] != DBNull.Value)
                nRows = (int)RNDHome.drS["PORows"];
            else nRows = 3;
           Forms.POMats.Clear();
            for (int i = 0; i < nRows; i++)Forms.POMats.Add(new CMaterial());

            //Read PO dataset and PO side formulation
            if (RNDHome.drS["POMats"] != DBNull.Value)
            {
                string[] strParts = RNDHome.drS["POMats"].ToString().Split(delimiterChars);
                if (nRows > strParts.Length) nRows = strParts.Length;
                for (int i = 0; i < nRows; i++) //Grid by default adds an extra row sometime
                {
                    id = int.Parse(strParts[i]);
                    iSel = FindIndex(id,dtPO);
                   ModifyPOIsoLists(i, ref Forms.POMats, iSel,dtPO);
                }
                /*
                                id = int.Parse(strParts[nRows-1]);
                                iSel = FindIndex(id, dtPO);
                                if(iSel > 0)
                                {
                                   Forms.POMats.Add(new CMaterial());
                                    ModifyPOIsoLists(Forms.POMats.Count-1, refForms.POMats, iSel, dtPO);
                                }
                */
            }

            //Ajust for the user assigned OH numbers

            if (RNDHome.drS["sPOMatsOH"] != DBNull.Value)
            {
                string[] strParts = RNDHome.drS["sPOMatsOH"].ToString().Split(delimiterChars);
                if (nRows > strParts.Length) nRows = strParts.Length;
                for (int i = 0; i < nRows - 1; i++) //Grid by default adds an extra row sometime
                {
                    if (double.TryParse(strParts[i], out dtmp))Forms.POMats[i].MatOHNum = dtmp;
                }
            }

            //Set pbws


            string sPOf = "0.0", stmp;
            for (int i = 0; i < RNDHome.dtF.Rows.Count; i++)
            {
                for (j = 0; j <Forms.FormAr[i].POMatPbw.Length; j++)Forms.FormAr[i].POMatPbw[j] = 0.0;
                if (RNDHome.dtF.Rows[i]["POPbws"] != DBNull.Value)
                {
                   Forms.FormAr[i].POMatPbw = System.Text.Json.JsonSerializer.Deserialize<double[]>((string)RNDHome.dtF.Rows[i]["POPbws"]);
                    switch (i)
                    {

                        case (0):
                            for (j = 0; j < nRows; j++)
                            { if (Forms.FormAr[0].POMatPbw[j] > 0) stmp = (Forms.FormAr[0].POMatPbw[j]).ToString(sPOf); else stmp = string.Empty;Forms.POMats[j].Pbw1 = stmp; }
                            break;

                        case (1):
                            for (j = 0; j < nRows; j++)
                            { if (Forms.FormAr[1].POMatPbw[j] > 0) stmp = (Forms.FormAr[1].POMatPbw[j]).ToString(sPOf); else stmp = string.Empty;Forms.POMats[j].Pbw2 = stmp; }
                            break;
                        case (2):
                            for (j = 0; j < nRows; j++)
                            { if (Forms.FormAr[2].POMatPbw[j] > 0) stmp = (Forms.FormAr[2].POMatPbw[j]).ToString(sPOf); else stmp = string.Empty;Forms.POMats[j].Pbw3 = stmp; }
                            break;
                        case (3):
                            for (j = 0; j < nRows; j++)
                            { if (Forms.FormAr[3].POMatPbw[j] > 0) stmp = (Forms.FormAr[3].POMatPbw[j]).ToString(sPOf); else stmp = string.Empty;Forms.POMats[j].Pbw4 = stmp; }
                            break;
                        case (4):
                            for (j = 0; j < nRows; j++)
                            { if (Forms.FormAr[4].POMatPbw[j] > 0) stmp = (Forms.FormAr[4].POMatPbw[j]).ToString(sPOf); else stmp = string.Empty;Forms.POMats[j].Pbw5 = stmp; }
                            break;
                        case (5):
                            for (j = 0; j < nRows; j++)
                            { if (Forms.FormAr[5].POMatPbw[j] > 0) stmp = (Forms.FormAr[5].POMatPbw[j]).ToString(sPOf); else stmp = string.Empty;Forms.POMats[j].Pbw6 = stmp; }
                            break;
                        case (6):
                            for (j = 0; j < nRows; j++)
                            { if (Forms.FormAr[6].POMatPbw[j] > 0) stmp = (Forms.FormAr[6].POMatPbw[j]).ToString(sPOf); else stmp = string.Empty;Forms.POMats[j].Pbw7 = stmp; }
                            break;
                        case (7):
                            for (j = 0; j < nRows; j++)
                            { if (Forms.FormAr[7].POMatPbw[j] > 0) stmp = (Forms.FormAr[7].POMatPbw[j]).ToString(sPOf); else stmp = string.Empty;Forms.POMats[j].Pbw8 = stmp; }
                            break;

                            /*
                                                    case (0): for (j = 0; j < nRows; j++)Forms.POMats[j].Pbw1 =Forms.FormAr[0].POMatPbw[j]; break;
                                                    case (1): for (j = 0; j < nRows; j++)Forms.POMats[j].Pbw2 =Forms.FormAr[1].POMatPbw[j]; break;
                                                    case (2): for (j = 0; j < nRows; j++)Forms.POMats[j].Pbw3 =Forms.FormAr[2].POMatPbw[j]; break;
                                                    case (3): for (j = 0; j < nRows; j++)Forms.POMats[j].Pbw4 =Forms.FormAr[3].POMatPbw[j]; break;
                                                    case (4): for (j = 0; j < nRows; j++)Forms.POMats[j].Pbw5 =Forms.FormAr[4].POMatPbw[j]; break;
                                                    case (5): for (j = 0; j < nRows; j++)Forms.POMats[j].Pbw6 =Forms.FormAr[5].POMatPbw[j]; break;
                                                    case (6): for (j = 0; j < nRows; j++)Forms.POMats[j].Pbw7 =Forms.FormAr[6].POMatPbw[j]; break;
                                                    case (7): for (j = 0; j < nRows; j++)Forms.POMats[j].Pbw8 =Forms.FormAr[7].POMatPbw[j]; break;
                            */
                    }
                }
            }

            //Read Iso Section

            nRows = 1;
           Forms.IsoMats.Clear();
            for (int i = 0; i < nRows; i++)Forms.IsoMats.Add(new CMaterial());

            if (RNDHome.drS["IsoMats"] != DBNull.Value)
            {
                id = (int)RNDHome.drS["IsoMats"];
                iSel = FindIndex(id,dtIso);
               ModifyPOIsoLists(0, ref Forms.IsoMats, iSel,dtIso);
            }
            if (RNDHome.drS["sIsoMatsNCO"] != DBNull.Value)  //Adjust NCO for user entered #
                if (double.TryParse(RNDHome.drS["sIsoMatsNCO"].ToString(), out dtmp))Forms.IsoMats[0].MatNco = dtmp;

            for (int i = 0; i < RNDHome.dtF.Rows.Count; i++)
            {
                if (RNDHome.dtF.Rows[i]["NCOIndex"] != DBNull.Value)Forms.FormAr[i].NcoIndex = (double)RNDHome.dtF.Rows[i]["NCOIndex"];
                else Forms.FormAr[i].NcoIndex = 270;
            } 
           Forms.NCOIndexMats[0].Pbw1 = (Forms.FormAr[0].NcoIndex).ToString(sPOf);
           Forms.NCOIndexMats[0].Pbw2 = (Forms.FormAr[1].NcoIndex).ToString(sPOf);
           Forms.NCOIndexMats[0].Pbw3 = (Forms.FormAr[2].NcoIndex).ToString(sPOf);
           Forms.NCOIndexMats[0].Pbw4 = (Forms.FormAr[3].NcoIndex).ToString(sPOf);
           Forms.NCOIndexMats[0].Pbw5 = (Forms.FormAr[4].NcoIndex).ToString(sPOf);
           Forms.NCOIndexMats[0].Pbw6 = (Forms.FormAr[5].NcoIndex).ToString(sPOf);
           Forms.NCOIndexMats[0].Pbw7 = (Forms.FormAr[6].NcoIndex).ToString(sPOf);
           Forms.NCOIndexMats[0].Pbw8 = (Forms.FormAr[7].NcoIndex).ToString(sPOf);

        }
        public static int FindIndex(int id, DataTable dt)
        {
            int index = -1;
            for (int i = 0; i < dt.Rows.Count; i++)
                if ((int)dt.Rows[i]["ID"] == id) return i;
            return index;
        }
    }
}
