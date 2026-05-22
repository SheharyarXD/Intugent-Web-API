using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Core
{
    [Serializable]
    public class CDefualts
    {
        public  string sLocMfg1 = "East", sLocMfg2 = "Center", sLocMfg3 = "West";
        //        public  string sLocMfg = "Gainsville, TX", sLocRND = $"Global R&D";
        public  string sLocMfgAll = "All Locations";  // Search Locations
        
        //public  string sLocRNDAll = sLocMfgAll; TEMP FIX !!!!!!!!!!!!!!!!!!!!!!!
        public  string sLocRNDAll = "All Locations";

        public  string sProdMfg = "4710GA";
        public string sProdRND = "Experimental";
        public string sProdMfgAll = "All Products";

        //public string sProdRNDAll = sProdMfgAll;    TEMP FIX !!!!!!!!!!!!!!!!!!!!!!!
        public string sProdRNDAll = "All Products";


        public  string sTestingStatAll = "All Data";
        public  int iMfgTestingStat = 1;
        public  int iMfgAgedRValue = 45;
        public  int iMfgDimStability = 48;
        public  int iMfgRunType = 50;
        public  string sSurfact = "Dow Vorasurf 504";
        public  string sLayout = "3 Heads 6 Streams";
        public  string sPaper = "Paper-1";
        public  string sShift = "A";
        public  string sOperator = "Harrison Maxwell";
        public  string sLocation = "Gainsville, TX";
        public  string sGroup = string.Empty;

        public  int IDLocation = 1;
        public  int IDEmployee = 23;
        public  string sEmployee = "AShafi";
        public  string sDomain = "GAF.COM";
        public  int iRunType = 1;
        public  int iRNDTestingStat = 52;  //Testing in Progress
        public  int iRNDRunType = 59;    // R&D Study
        public  double dDelTimeButton = 5.0;
        public  double dDelTimeCalc = 5.0;

        public  int irowD = 5, ictiD = 1, icteD = 3, icheD = 2, icprD = 4;  //defualt foamat data columns
        public  bool bFoamatSt = true, bPlotAll = true;  //standard foamat file, plotallfoamat data files    
        public  double FoamatRunWt = 225.0;
    }
}
