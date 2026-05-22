using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Data
{
    [Serializable]
    public class CRData
    {

        public double dTestTempC { get; set; }
        public double dCellSize { get; set; }
        public double dInitCellPress { get; set; }
        public double dPolDensity { get; set; }

        //       public double dAgingTempC = 25;
        public double dAgeTempC { get; set; }
        public double dPolCond { get; set; }
        public double dFracStrut { get; set; }
        public string sYaxisTag { get; set; }
        public string sXaxisTag { get; set; }
        //        public int iYAxis1Index = 0;
        //        public int iYAxis2Index = 2;
        //        public int iFormIndex = 1;

    }

}
