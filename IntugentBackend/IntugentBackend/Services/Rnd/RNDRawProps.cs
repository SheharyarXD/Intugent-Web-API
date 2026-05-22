using IntugentBackend.Services.Core;
using System;
using System.Collections.Generic;
using System.Data;
using IntugentBackend.Services.Admin;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Rnd;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Rnd
{
    public class RNDRawProps
    {
        public DataTable dtDensityE = new DataTable();
        public DataTable dtDensityC = new DataTable();
        public DataTable dtCompStrE = new DataTable();
        public DataTable dtCompStrC = new DataTable();
        public DataTable dtPoreScanE = new DataTable();
        public DataTable dtPoreScanC = new DataTable();
        public DataTable dtClosedCellE = new DataTable();
        public DataTable dtClosedCellC = new DataTable();
        //        public DataTable dtClosedCellC1 = new DataTable();
        public DataTable dtHotPlatesE = new DataTable();
        public DataTable dtHotPlatesC = new DataTable();
        public DataTable dtHotPlatesC1 = new DataTable();
        public RNDRawProps()
        {

                #region  // Density data and calculations
                dtDensityE.Columns.Add("PropName", typeof(string));
                dtDensityC.Columns.Add("PropName", typeof(string));

                for (int i = 1; i < Params.nFormMax + 1; i++)
                {
                    dtDensityE.Columns.Add("PropE" + i, typeof(string));
                    dtDensityC.Columns.Add("PropC" + i, typeof(string));

                }

                for (int i = 0; i < 10; i++) dtDensityE.Rows.Add();
                dtDensityE.Rows[0]["PropName"] = "   T1 (in)";
                dtDensityE.Rows[1]["PropName"] = "   T2 (in)";
                dtDensityE.Rows[2]["PropName"] = "   T3 (in)";
                dtDensityE.Rows[3]["PropName"] = "   T4 (in)";
                dtDensityE.Rows[4]["PropName"] = "   T5 (in)";
                dtDensityE.Rows[5]["PropName"] = "   L1 (in)";
                dtDensityE.Rows[6]["PropName"] = "   L2 (in)";
                dtDensityE.Rows[7]["PropName"] = "   W1 (in)";
                dtDensityE.Rows[8]["PropName"] = "   W2 (in)";
                dtDensityE.Rows[9]["PropName"] = "   Mass (gm)";

                for (int i = 0; i < 4; i++) dtDensityC.Rows.Add();
                dtDensityC.Rows[0]["PropName"] = " Avg. T (in)";
                dtDensityC.Rows[1]["PropName"] = " Avg. L (in)";
                dtDensityC.Rows[2]["PropName"] = " Avg. W (in)";
                dtDensityC.Rows[3]["PropName"] = "*Density (lb/ft3)*";

                //gDensityE.ItemsSource = dtDensityE.DefaultView;
                //gDensityC.ItemsSource = dtDensityC.DefaultView;
                #endregion

                #region // Compressive Strength

                dtCompStrE.Columns.Add("PropName", typeof(string));
                dtCompStrC.Columns.Add("PropName", typeof(string));

                for (int i = 1; i < Params.nFormMax + 1; i++)
                {
                    dtCompStrE.Columns.Add("PropE" + i, typeof(string));
                    dtCompStrC.Columns.Add("PropC" + i, typeof(string));
                }

                for (int i = 0; i < 4; i++) dtCompStrE.Rows.Add();
                dtCompStrE.Rows[0]["PropName"] = "   1 (psi)";
                dtCompStrE.Rows[1]["PropName"] = "   2 (psi)";
                dtCompStrE.Rows[2]["PropName"] = "   3 (psi)";
                dtCompStrE.Rows[3]["PropName"] = "   4 (psi)";

                for (int i = 0; i < 1; i++) dtCompStrC.Rows.Add();
                dtCompStrC.Rows[0]["PropName"] = "*Average (psi)*";

                //gCompStrE.ItemsSource = dtCompStrE.DefaultView;
                //gCompStrC.ItemsSource = dtCompStrC.DefaultView;
                #endregion

                #region // PoreScan

                dtPoreScanE.Columns.Add("PropName", typeof(string));
                dtPoreScanC.Columns.Add("PropName", typeof(string));

                for (int i = 1; i < Params.nFormMax + 1; i++)
                {
                    dtPoreScanE.Columns.Add("PropE" + i, typeof(string));
                    dtPoreScanC.Columns.Add("PropC" + i, typeof(string));
                }

                for (int i = 0; i < 6; i++) dtPoreScanE.Rows.Add();
                dtPoreScanE.Rows[0]["PropName"] = "Top Cell Dia.(micron)";
                dtPoreScanE.Rows[1]["PropName"] = "   Std.Dev.";
                dtPoreScanE.Rows[2]["PropName"] = "   # Cells";
                dtPoreScanE.Rows[3]["PropName"] = "Side Cell Dia.(micron)";
                dtPoreScanE.Rows[4]["PropName"] = "   Std.Dev.";
                dtPoreScanE.Rows[5]["PropName"] = "   # Cells";

                for (int i = 0; i < 3; i++) dtPoreScanC.Rows.Add();
                dtPoreScanC.Rows[0]["PropName"] = "*Avg.Size (micron)*";
                dtPoreScanC.Rows[1]["PropName"] = "*Avg.Count";
                dtPoreScanC.Rows[2]["PropName"] = "*Isotropy*";

                //gPoreScanE.ItemsSource = dtPoreScanE.DefaultView;
                //gPoreScanC.ItemsSource = dtPoreScanC.DefaultView;
                #endregion


                #region // Percent Close Cells

                dtClosedCellE.Columns.Add("PropName", typeof(string));
                dtClosedCellC.Columns.Add("PropName", typeof(string));

                for (int i = 1; i < Params.nFormMax + 1; i++)
                {
                    dtClosedCellE.Columns.Add("PropE" + i, typeof(string));



                    dtClosedCellC.Columns.Add("PropC" + i, typeof(string));
                }

                for (int i = 0; i < 3; i++) dtClosedCellE.Rows.Add();
                dtClosedCellE.Rows[0]["PropName"] = "   1 (%)";
                dtClosedCellE.Rows[1]["PropName"] = "   2 (%)";
                dtClosedCellE.Rows[2]["PropName"] = "   3 (%)";


                for (int i = 0; i < 1; i++) dtClosedCellC.Rows.Add();
                dtClosedCellC.Rows[0]["PropName"] = "*Average (%)*";

               // gCloseCellE.ItemsSource = dtClosedCellE.DefaultView;
                //gClosedCellC.ItemsSource = dtClosedCellC.DefaultView;
                #endregion


                #region // Hot Plates Data

                dtHotPlatesE.Columns.Add("PropName", typeof(string));
                dtHotPlatesC.Columns.Add("PropName", typeof(string));
                dtHotPlatesC1.Columns.Add("PropName", typeof(string));

                for (int i = 1; i < Params.nFormMax + 1; i++)
                {
                    dtHotPlatesE.Columns.Add("PropEi" + i, typeof(string));
                    dtHotPlatesE.Columns.Add("PropEf" + i, typeof(string));
                    dtHotPlatesC1.Columns.Add("PropEi" + i, typeof(string));
                    dtHotPlatesC1.Columns.Add("PropEf" + i, typeof(string));
                    dtHotPlatesC.Columns.Add("PropC" + i, typeof(string));
                }

                for (int i = 0; i < 6; i++) dtHotPlatesE.Rows.Add();
                dtHotPlatesE.Rows[0]["PropName"] = "   Mass (gm)";
                dtHotPlatesE.Rows[1]["PropName"] = "   H1 (in)";
                dtHotPlatesE.Rows[2]["PropName"] = "   H2 (in)";
                dtHotPlatesE.Rows[3]["PropName"] = "   H3 (in)";
                dtHotPlatesE.Rows[4]["PropName"] = "   H4 (in)";
                dtHotPlatesE.Rows[5]["PropName"] = "   H5 (in)";

                for (int i = 0; i < 1; i++) dtHotPlatesC1.Rows.Add();
                dtHotPlatesC1.Rows[0]["PropName"] = "Average H (in)";

                for (int i = 0; i < 2; i++) dtHotPlatesC.Rows.Add();
                dtHotPlatesC.Rows[0]["PropName"] = "Mass Retained (%)";
                dtHotPlatesC.Rows[1]["PropName"] = "Thickness Retained (%)";

                //gHotPlatesE.ItemsSource = dtHotPlatesE.DefaultView;
                //gHotPlatesC1.ItemsSource = dtHotPlatesC1.DefaultView;
                //gHotPlatesC.ItemsSource = dtHotPlatesC.DefaultView;
                #endregion


            }

        }
  }
