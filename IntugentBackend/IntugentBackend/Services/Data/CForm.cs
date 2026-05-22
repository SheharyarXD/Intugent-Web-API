using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Data
{

    [Serializable]
    public class CForm
    {
        public const int nComps = Params.nComps;
        public double[] POMatPbw = new double[nComps];
        public double[] POMatWts = new double[nComps];
        public double[] POMatEqws = new double[nComps];

        public double[] IsoMatPbw = new double[nComps];
        public double[] IsoMatWts = new double[nComps];
        public double[] IsoMatEqws = new double[nComps];

        public double OHNumPOSide, TotalPbwPOSide, OHNumPolyol, TotalPbwPolyol, FuncAvPOSide, TotalPbwFuncPOSide, BasisPbwPOSide;
        /* OHNumPOSide, TotalPbwPOSide:- OH# of all components on the PO side and their total pbw
         * OHNumPolyol, TotalPbwPolyol:- OH# of all Polyols on the PO side and their total pbw
         * FuncAvPOSide, TotalPbwFuncPOSide:- Av. functionality of all components with func > 0 and their total pbsw.
         */
        public double NcoIsoSide, TotalPbwIsoSide, FuncAvIsoSide, TotalPbwFuncIsoSide; //,BasisPbwIsoSide ;

        public double NcoIndex, PolyolIsoRatio, IsoPolyolRatio;

        public double IsocyanuratePbw, CrosslinkDensity, FoamDensity, BlowingAgentWtFr, BlowingAgentWtFr1, WaterWtFr, CatalystWtFr, SurfactWtFr;

    }
}
