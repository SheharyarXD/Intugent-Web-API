using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Data
{
    public class Params 
    {
        public static double PolymerDensity = 1200; //kg/m3
        public static string sAppName = "I-DigiPro";
        public const int nFormMax = 8;
        public const int nComps = 30;
        public const int nDataPts = 51;
        public const int nDataPtMid = nDataPts / 2;
        public const int nTimePts = 501;
        public const int iAirDBId = 97;
        public const double RKConFactor = 1.0 / 6.933472;
    }

}
