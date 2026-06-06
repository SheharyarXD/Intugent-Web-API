using System;
using System.Data;
using Microsoft.Data.SqlClient; // Ensure you are using Microsoft.Data.SqlClient

namespace IntugentBackend.Services.Core
{
    [Serializable]
    public class CLists
    {
        // DataTables
        public DataTable dtLoc = new DataTable();
        public DataTable dtComProd = new DataTable();
        public DataTable dtLists = new DataTable();
        public DataTable dt = new DataTable();
        public DataTable dtProcessParams = new DataTable();
        public DataTable dtEmployees = new DataTable();
        public DataTable dtEmployee = new DataTable(); // Legacy support
        public DataTable dtUserGroups = new DataTable();

        // DataViews
        public DataView dvLoc = new DataView();
        public DataView dvLocMfg = new DataView();
        public DataView dvLocMfgAll = new DataView();
        public DataView dvLocRND = new DataView();
        public DataView dvLocRNDAll = new DataView();
        public DataView dvComProd = new DataView();
        public DataView dvComProdAll = new DataView();
        public DataView dvLists = new DataView();
        public DataView dvRunType2 = new DataView();
        public DataView dvRunType = new DataView();
        public DataView dvRunTypeRND = new DataView();
        public DataView dvRunTypeRND2 = new DataView();
        public DataView dvTestingStat = new DataView();
        public DataView dvTestingStatAll = new DataView();
        public DataView dvSurfactants = new DataView();
        public DataView dvLayout = new DataView();
        public DataView dvPaper = new DataView();
        public DataView dvShift = new DataView();
        public DataView dvOperator = new DataView();
        public DataView dvProcssParams = new DataView();
        public DataView dvProcessParams = new DataView();
        public DataView dvPPChemDel = new DataView();
        public DataView dvPPChemDel1 = new DataView();
        public DataView dvPPPTable = new DataView();
        public DataView dvPPDBelt = new DataView();
        public DataView dvPPOthers = new DataView();
        public DataView dvNewInsData = new DataView();
        public DataView dvTestingStatRND = new DataView();
        public DataView dvEmployees = new DataView();
        public DataView dvEmployeesMfg = new DataView();
        public DataView dvEmployeesRND = new DataView();
        public DataView dvPropsRND = new DataView();
        public DataView dvPCType = new DataView();
        public DataView dvPCTopBoard = new DataView();
        public DataView dvPCBottomBoard = new DataView();
        public DataView dvPCPerferation = new DataView();
        public DataView dvPCFlipper = new DataView();
        public DataView dvPCFacerAdh = new DataView();
        public DataView dvPCEdgeCut = new DataView();
        public DataView dvPCHooderQuality = new DataView();
        public DataView dvPCBoardQuality = new DataView();

        public DataRow? drEmployee;
        public SqlDataAdapter? daEmployee;

        public CLists()
        {
            InitializeEmployeeTableSchema();
        }

        public void InitializeEmployeeTableSchema()
        {
            if (dtEmployees.Columns.Count == 0)
            {
                dtEmployees.Columns.Add("RndDate1", typeof(DateTime));
                dtEmployees.Columns.Add("RndDate2", typeof(DateTime));
                dtEmployees.Columns.Add("RNDNameSearch", typeof(string));
                dtEmployees.Columns.Add("Rnd Product Code", typeof(string));
                dtEmployees.Columns.Add("RndIDTestingStatus", typeof(int));
                dtEmployees.Columns.Add("RndIDStudyType", typeof(int));
                dtEmployees.Columns.Add("RndIDSelected", typeof(int));
            }
        }

        public void UpdateEmployee()
        {
            try
            {
                if (daEmployee != null)
                {
                    SqlCommandBuilder sb = new SqlCommandBuilder(daEmployee);
                    sb.ConflictOption = ConflictOption.OverwriteChanges;
                    daEmployee.Update(dtEmployees);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Errors in saving User preferences: " + ex.Message);
                throw;
            }
        }
    }
}