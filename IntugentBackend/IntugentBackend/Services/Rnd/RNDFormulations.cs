using IntugentBackend.Services.Core;
using System;
using System.Data;
using System.Collections.ObjectModel;
using IntugentBackend.Services.Data;

namespace IntugentBackend.Services.Rnd
{
    public class RNDFormulations
    {
        public CForms Forms = new CForms();
        public DataTable dtFormProp = new DataTable();
        public Cbfile cbfile;
        public RNDHome RNDHome;

        public RNDFormulations(CDefualts cDefualts, Cbfile cbfile, RNDHome rNDHome)
        {
            this.cbfile = cbfile ?? throw new ArgumentNullException(nameof(cbfile));
            this.RNDHome = rNDHome ?? throw new ArgumentNullException(nameof(rNDHome));
            Startup();
        }

        public void Startup()
        {
            Forms.FormAr = new CForm[Forms.nForm];
            for (int i = 0; i < Forms.nForm; i++)
            {
                Forms.FormAr[i] = new CForm();
                Forms.FormAr[i].POMatPbw = new double[10];
            }

            dtFormProp = new DataTable();
            dtFormProp.Columns.Add("Descriptors", typeof(string));
            for (int i = 1; i <= Forms.nForm; i++) dtFormProp.Columns.Add("#" + i, typeof(double));
            for (int i = 0; i < 30; i++) dtFormProp.Rows.Add(dtFormProp.NewRow());
        }

        public void FormDescriptors()
        {
            // [Ensure your existing Math Logic here calculates Forms.FormAr properties]

            // Map the results into the table the API reads
            for (int row = 0; row < dtFormProp.Rows.Count; row++)
            {
                dtFormProp.Rows[row]["Descriptors"] = "Total Pbw PO Side";
                for (int col = 1; col <= Forms.nForm; col++)
                {
                    if (col - 1 < Forms.FormAr.Length)
                        dtFormProp.Rows[row]["#" + col] = Forms.FormAr[col - 1].TotalPbwPOSide;
                }
            }
        }

        public void ReadDataset() { /* Logic to populate Forms.FormAr from RNDHome.drS */ }
    }
}