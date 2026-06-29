using Microsoft.ApplicationInsights;
using Microsoft.Data.SqlClient;
using IntugentBackend;

namespace IntugentBackend.Services.Core
{
    public class Cbfile
    {

        public readonly ObjectsService _objectService;
        public bool bConAz = false;
        public bool bCanSwitchRecord = true;
        public bool bChanged;
        public string sAppName = "Intugent PI";
        public string sDomain = "GAF.COM";
        public string sDomain2 = "corp.gaf.com";
        public TelemetryClient telClient;
        public DateTime dateDefault = new DateTime(1900, 01, 01);
        public string sVersion = "1.0.0";
        public string sFileName = null;
        public string sFileExt = ".ipr";
        public string strLiscenceFilePath = "";
        public string sDBFile = @"\Database\IntugentInsulationFoam.mdb";
        public string sHelpFile = @"IntugentPI.pdf";
        public string sIntDir = @"C:\Program Files\InsulationFoams";
        public string sOptionsFile = @"\Options.text";
        public char[] delimiterChars = { ' ', ',', ':', '\t' };

        public SqlConnection conAZ;

        public Cbfile()
        {
            // IMPORTANT: Initialize the connection object here to prevent NullReferenceException
            string connectionString = ObjectsService.ConnectionString;
            conAZ = new SqlConnection(connectionString);
        }

        public string sDBConn()
        {
            string sCon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=";
            return sCon + sIntDir + sDBFile;
        }
        public string sConSql = ObjectsService.ConnectionString;
        public int iIDMfg = 1943;
        public int iIndexRND = 0;
        public int iIDMfgIndex = 0;
        public string sHost = "azsmtpgw.gaf.com";
        public string sSender = "donotreply@gaf.com";
        public string sNoRecSwitchMsg = "Intugent PI is pulling process data...";
    }
}