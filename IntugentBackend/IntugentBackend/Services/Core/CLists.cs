using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace IntugentBackend.Services.Core
{
    [Serializable]
    public class CLists
    {
        public  DataTable dtLoc = new DataTable(),  //Also used for Employees
                        dtComProd = new DataTable(),
                        dtLists = new DataTable(),
                        dt = new DataTable(),
                        dtProcessParams = new DataTable();


        public  DataView dvLoc = new DataView(),
            dvLocMfg = new DataView(),
            dvLocMfgAll = new DataView(),
            dvLocRND = new DataView(),
            dvLocRNDAll = new DataView(),
            dvComProd = new DataView(),
            dvComProdAll = new DataView(),
            dvLists = new DataView(),
            //            dvRunTypeAll = new DataView(),
            dvRunType2 = new DataView(),
            dvRunType = new DataView(),
            dvRunTypeRND = new DataView(),
            dvRunTypeRND2 = new DataView(),
            dvTestingStat = new DataView(),
            dvTestingStatAll = new DataView(),
            dvSurfactants = new DataView(),
            dvLayout = new DataView(),
            dvPaper = new DataView(),
            dvShift = new DataView(),
            dvOperator = new DataView(),
            dvProcssParams = new DataView(),
            dvPPChemDel = new DataView(),
            dvPPChemDel1 = new DataView(),
            dvPPPTable = new DataView(),
            dvPPDBelt = new DataView(),
            dvPPOthers = new DataView(),
            dvNewInsData = new DataView(),
            dvTestingStatRND = new DataView(),
            dvEmployees = new DataView(),
            dvEmployeesMfg = new DataView(),
            dvEmployeesRND = new DataView(),
            dvPropsRND = new DataView(),


        dvPCType = new DataView(),
        dvPCTopBoard = new DataView(),
        dvPCBottomBoard = new DataView(),
        dvPCPerferation = new DataView(),
        dvPCFlipper = new DataView(),
        dvPCFacerAdh = new DataView(),
        dvPCEdgeCut = new DataView(),
        dvPCHooderQuality = new DataView(),
        dvPCBoardQuality = new DataView();



        public  DataRow drEmployee;
        public  DataTable dtEmployee = new DataTable(), dtUserGroups = new DataTable();
        public SqlDataAdapter daEmployee;
        public void UpdateEmployee()                     //Used to save user choices
        {
            //            SqlDataAdapter da = new SqlDataAdapter();

            string sMsg = "Coult not save to the server";

            try
            {
                SqlCommandBuilder sb = new SqlCommandBuilder(daEmployee);
                sb.ConflictOption = ConflictOption.OverwriteChanges;
                daEmployee.Update(dtEmployee);
            }
            catch (Exception ex)
            {
                sMsg = "Errors in saving User preferences";
                //MessageBox.Show(sMsg, Cbfile.sAppName, MessageBoxButton.OK, MessageBoxImage.Stop);
                System.Diagnostics.Trace.TraceError(sMsg + "\n\n" + ex.Message);
                //CTelClient.TelException(ex, sMsg);
                throw;
            }
            //CStatusBar.SetText("User Preferences Saved at " + DateTime.Now.ToString("hh:mm:ss:tt"));


        }
    }
}
