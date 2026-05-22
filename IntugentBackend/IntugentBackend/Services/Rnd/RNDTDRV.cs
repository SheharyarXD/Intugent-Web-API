using IntugentBackend.Services.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntugentBackend.Services.Admin;
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Rnd;

namespace IntugentBackend.Services.Rnd
{
    public class RNDTDRV
    {
        public DataTable dtTdrvE = new DataTable();
        public DataTable dtTdrvC = new DataTable();
        public DataTable dtTdrvP = new DataTable();
        public RNDTDRV()
        {

            dtTdrvE.Columns.Add("PropName", typeof(string));
            for (int i = 1; i < Params.nFormMax + 1; i++)
            {
                dtTdrvE.Columns.Add("PropE" + i, typeof(string));
                dtTdrvC.Columns.Add("PropC" + i, typeof(string));
                dtTdrvP.Columns.Add("PropP" + i, typeof(string));
            }

            for (int i = 0; i < 35; i++) { dtTdrvE.Rows.Add(); dtTdrvC.Rows.Add(); dtTdrvP.Rows.Add(); }

            int ir;

            ir = 0;
            dtTdrvE.Rows[ir]["PropName"] = "Thermals - 10 Day TDRV";
            dtTdrvE.Rows[ir + 1]["PropName"] = "   Initial R Value";
            dtTdrvE.Rows[ir + 2]["PropName"] = "      25°F";
            dtTdrvE.Rows[ir + 3]["PropName"] = "      40°F";
            dtTdrvE.Rows[ir + 4]["PropName"] = "      75°F";
            dtTdrvE.Rows[ir + 5]["PropName"] = "      110°F";
            dtTdrvE.Rows[ir + 6]["PropName"] = "   Final R Value";
            dtTdrvE.Rows[ir + 7]["PropName"] = "      25°F";
            dtTdrvE.Rows[ir + 8]["PropName"] = "      40°F";
            dtTdrvE.Rows[ir + 9]["PropName"] = "      75°F";
            dtTdrvE.Rows[ir + 10]["PropName"] = "      110°F";
            dtTdrvE.Rows[ir + 11]["PropName"] = "▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬";

            ir += 12;
            dtTdrvE.Rows[ir]["PropName"] = "Thermals - 90 Day TDRV";
            dtTdrvE.Rows[ir + 1]["PropName"] = "   Initial R Value";
            dtTdrvE.Rows[ir + 2]["PropName"] = "      25°F";
            dtTdrvE.Rows[ir + 3]["PropName"] = "      40°F";
            dtTdrvE.Rows[ir + 4]["PropName"] = "      75°F";
            dtTdrvE.Rows[ir + 5]["PropName"] = "      110°F";
            dtTdrvE.Rows[ir + 6]["PropName"] = "   Final R Value";
            dtTdrvE.Rows[ir + 7]["PropName"] = "      25°F";
            dtTdrvE.Rows[ir + 8]["PropName"] = "      40°F";
            dtTdrvE.Rows[ir + 9]["PropName"] = "      75°F";
            dtTdrvE.Rows[ir + 10]["PropName"] = "      110°F";
            dtTdrvE.Rows[ir + 11]["PropName"] = "▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬";

            ir += 12;
            dtTdrvE.Rows[ir]["PropName"] = "Thermals - 180 Day TDRV";
            dtTdrvE.Rows[ir + 1]["PropName"] = "   Initial R Value";
            dtTdrvE.Rows[ir + 2]["PropName"] = "      25°F";
            dtTdrvE.Rows[ir + 3]["PropName"] = "      40°F";
            dtTdrvE.Rows[ir + 4]["PropName"] = "      75°F";
            dtTdrvE.Rows[ir + 5]["PropName"] = "      110°F";
            dtTdrvE.Rows[ir + 6]["PropName"] = "   Final R Value";
            dtTdrvE.Rows[ir + 7]["PropName"] = "      25°F";
            dtTdrvE.Rows[ir + 8]["PropName"] = "      40°F";
            dtTdrvE.Rows[ir + 9]["PropName"] = "      75°F";
            dtTdrvE.Rows[ir + 10]["PropName"] = "      110°F";


            //gExpPropsE.ItemsSource = dtTdrvE.DefaultView;
            //gExpPropsC.ItemsSource = dtTdrvC.DefaultView;
            //gExpPropsP.ItemsSource = dtTdrvP.DefaultView;
        }

    }
}
