using System;
using System.Text.Json;
using System.Data; // Required for DataColumn and DataTable
using IntugentBackend.Services.Core;
using IntugentBackend.Services.Admin;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using IntugentBackend.Services.Rnd;

namespace IntugentBackend.Services.Rnd
{
    public class RNDRValuesService
    {
        private readonly ObjectsService _objectsService;

        public RNDRValuesService(ObjectsService objectsService)
        {
            _objectsService = objectsService;
        }

        public void LoadDataFromEmployee()
        {
            var rValues = _objectsService.RNDRValues;
            // Defensive check for CLists and drEmployee
            if (rValues?.CLists?.drEmployee != null)
            {
                // Check if column exists before reading
                if (rValues.CLists.drEmployee.Table.Columns.Contains("sRValueParams") &&
                    rValues.CLists.drEmployee["sRValueParams"] != DBNull.Value)
                {
                    string js1 = rValues.CLists.drEmployee["sRValueParams"].ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(js1))
                    {
                        rValues.RData = JsonSerializer.Deserialize<CRData>(js1) ?? new CRData();
                    }
                }
            }
        }

        public void CollectBlowGases()
        {
            var rValues = _objectsService.RNDRValues;
            rValues.RCalc.GasMats.Clear();
            int iba = -1;

            foreach (var mat in _objectsService.RNDFormulations.Forms.POMats)
            {
                if (mat.GassToLiqWtRatio > 0)
                {
                    iba++;
                    rValues.RCalc.GasMats.Add(mat); // Simplified adding logic
                    for (int ifo = 0; ifo < Params.nFormMax; ifo++)
                    {
                        int matIndex = _objectsService.RNDFormulations.Forms.POMats.IndexOf(mat);
                        rValues.RCalc.MoleFracs[ifo, iba] = _objectsService.RNDFormulations.Forms.FormAr[ifo].POMatPbw[matIndex]
                            * mat.GassToLiqWtRatio / mat.GassMolWt;
                    }
                }
            }
            rValues.RCalc.nBlowAg = iba + 1;
        }

        public void UpdateDataset()
        {
            var rValues = _objectsService.RNDRValues;

            if (rValues?.CLists?.drEmployee == null)
            {
                System.Diagnostics.Debug.WriteLine("UpdateDataset skipped: drEmployee is null.");
                return;
            }

            // --- CORRECTION: Schema-aware update ---
            DataTable table = rValues.CLists.drEmployee.Table;

            // Add column if it is missing from the table memory
            if (!table.Columns.Contains("sRValueParams"))
            {
                System.Diagnostics.Debug.WriteLine("Column 'sRValueParams' missing. Adding dynamically.");
                table.Columns.Add("sRValueParams", typeof(string));
            }

            string js1 = JsonSerializer.Serialize(rValues.RData);
            rValues.CLists.drEmployee["sRValueParams"] = js1;
            rValues.CLists.UpdateEmployee();
        }
    }
}