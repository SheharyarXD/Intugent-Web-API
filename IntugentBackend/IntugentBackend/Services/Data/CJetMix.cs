using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Data
{
    public class CJetMix
    {
        public double FRate_A { get; set; }
        public double FRate_B { get; set; }
        public double Temp_A { get; set; }
        public double Temp_B { get; set; }
        public double Pres_A { get; set; }
        public double Pres_B { get; set; }
        public double Dens_A { get; set; }
        public double Dens_B { get; set; }
        public double VisO_A { get; set; }
        public double VisO_B { get; set; }
        public double VisE_A { get; set; }
        public double VisE_B { get; set; }
        public double DiaMixChamb { get; set; }
        public double DiaNoz_A { get; set; }
        public double DiaNoz_B { get; set; }
        public double Pres_Max { get; set; }
        public double Pres_Min { get; set; }
        public double ReNo_Min { get; set; }


        public double Zeta, DiaJet_A, DiaJet_B;
        public double Vis_A, Vis_B;
        //      public double PreExp = 0.030947844;
        public double PreExp = 0.040947844;
        public double Factor = 0.76359427;



        public double VolRate_A, Vel_A, ReNo_A, KE_A;
        public double VolRate_B, Vel_B, ReNo_B, KE_B;

        public bool bPlantSim = true;
        public CJetMix()
        {
      
        }

        public void JetMixingSim()
        {
            double pi = Math.PI, T_A, T_B, MomRatio;
            double Cd_A, Cd_B;

            Vel_A = Math.Sqrt(2.0 * Pres_A / Dens_A);
            Vis_A = VisO_A * Math.Exp(VisE_A * (1 / Temp_A - 1.0 / 298.15));

            VolRate_A = FRate_A / Dens_A;
            DiaJet_A = Math.Sqrt(VolRate_A / Vel_A * 4.0 / pi);
            ReNo_A = Dens_A * Vel_A * DiaJet_A / Vis_A;
            Cd_A = Factor * (1.0 - Math.Exp(-PreExp * ReNo_A)); // Cd_A = 1.0;
            ReNo_A = Cd_A * ReNo_A;
            Vel_A = Cd_A * Vel_A;
            DiaJet_A = Math.Sqrt(VolRate_A / Vel_A * 4.0 / pi);

            KE_A = FRate_A * Vel_A * Vel_A;

            Vel_B = Math.Sqrt(2.0 * Pres_B / Dens_B);
            Vis_B = VisO_B * Math.Exp(VisE_B * (1 / Temp_B - 1.0 / 298.15));

            VolRate_B = FRate_B / Dens_B;
            DiaJet_B = Math.Sqrt(VolRate_B / Vel_B * 4.0 / pi);
            ReNo_B = Dens_B * Vel_B * DiaJet_B / Vis_B;
            Cd_B = Factor * (1.0 - Math.Exp(-PreExp * ReNo_B));// Cd_B = 1.0;
            ReNo_B = Cd_B * ReNo_B;
            Vel_B = Cd_B * Vel_B;
            DiaJet_B = Math.Sqrt(VolRate_B / Vel_B * 4.0 / pi);

            KE_B = FRate_B * Vel_B * Vel_B;

            //           MomRatio = Math.Sqrt(KE_B * ReNo_B * DiaJet_B / (KE_A * ReNo_A * DiaJet_A));
            //           T_A = 1.0 + 0.1 * ReNo_A * DiaJet_A / DiaMixChamb;
            //            T_B = 1.0 + 0.1 * ReNo_B * DiaJet_B / DiaMixChamb;

            //           Zeta = - (T_A * MomRatio - T_B) / (MomRatio + 1.0);

            /*             MomRatio = Math.Sqrt(KE_A * ReNo_A * DiaJet_A / (KE_B * ReNo_B * DiaJet_B));
                                            T_A = 1.0 + 0.1 * ReNo_A * DiaJet_A / DiaMixChamb;
                                             T_B = 1.0 + 0.1 * ReNo_B * DiaJet_B / DiaMixChamb;

                                                 Zeta = 0.5 * (T_A * MomRatio - T_B) / (MomRatio + 1.0);
                       */

            //           MomRatio = Math.Sqrt(KE_A * ReNo_A * DiaJet_A / (KE_B * ReNo_B * DiaJet_B));
            //           MomRatio = Math.Sqrt(KE_A / KE_B * Vis_B / Vis_A * Vel_B / Vel_A * DiaJet_A / DiaJet_B);
            MomRatio = (KE_A / KE_B);
            //            MomRatio = (Vel_A / Vel_B);

            T_A = 1.0;
            T_B = 1.0;

            Zeta = 0.5 * (T_B * MomRatio - T_A) / (MomRatio + 1.0);


        }
    }
}
