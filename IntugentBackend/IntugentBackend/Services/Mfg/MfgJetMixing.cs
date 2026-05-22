using Google.Api.Gax;
using IntugentBackend.Services.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IntugentBackend.Services.Data;

namespace IntugentBackend.Services.Mfg
{
    public class MfgJetMixing
    {
        public CJetMix JetMix1 = new CJetMix();
        public CUConv cUConv= new CUConv();
        public DataTable dtDetails = new DataTable();
        public const int nPts = 101;
        public double[] XA = new double[101], XB = new double[101], YA = new double[101], YB = new double[101];
        public CLists cLists;
        public MfgJetMixing(CLists cLists)
        {
            this.cLists=cLists;
            Startup();
        }
        public void Startup()
        {
            SetDefaultValues();
            dtDetails.Columns.Add("Description", typeof(string));
            dtDetails.Columns.Add("Jet_A", typeof(string));
            dtDetails.Columns.Add("Jet_B", typeof(string));
            for (int i = 0; i < 7; i++) dtDetails.Rows.Add();
           

            try
            {
                if (cLists.drEmployee["sJetMix"] == DBNull.Value) SetDefaultValues();
                else
                {
                    string js1 = (string)cLists.drEmployee["sJetMix"];
                    JetMix1 = (CJetMix)System.Text.Json.JsonSerializer.Deserialize(js1, typeof(CJetMix));
                }
            }
            catch { SetDefaultValues(); }

        }
        public void SetDefaultValues()
        {
            JetMix1.DiaJet_A = 0.00135;
            JetMix1.DiaJet_B = 0.00133;
            JetMix1.DiaNoz_A = 0.0025;
            JetMix1.DiaNoz_B = 0.0025;
            JetMix1.DiaMixChamb = 0.01;

            JetMix1.FRate_A = 0.33951;
            JetMix1.FRate_B = 0.24509;
            JetMix1.Temp_A = JetMix1.Temp_B = 298.15;
            JetMix1.Pres_A = 2000.0 * cUConv.ToSi_Pres;
            JetMix1.Pres_B = 2000.0 * cUConv.ToSi_Pres;
            JetMix1.Dens_A = 1278;
            JetMix1.Dens_B = 1106;
            JetMix1.Vis_A = 0.70; JetMix1.Vis_B = 5.68;
            JetMix1.VisO_A = 0.70; JetMix1.VisO_B = 1.5;
            //            JetMix1.VisN_A = 1.0; JetMix1.VisN_B = 0.98;
            JetMix1.VisE_A = 2000.0; JetMix1.VisE_B = 2500;
            JetMix1.Pres_Max = 4000 * cUConv.ToSi_Pres;
            JetMix1.Pres_Min = 1000 * cUConv.ToSi_Pres;
            JetMix1.ReNo_Min = 100.0;



        }
      }
    }
