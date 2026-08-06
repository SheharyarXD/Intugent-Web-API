using IntugentBackend.Services.Core;
using IntugentBackend.Services.Data;
using System.Data;

namespace IntugentBackend.Services.Rnd
{
    public class FormulationsPageService
    {
        private readonly RNDHome _rndHome;
        private readonly RNDFormulations _rndFormulations;
        private readonly CLists _cLists;

        public FormulationsPageService(RNDHome rndHome, RNDFormulations rndFormulations, CLists cLists)
        {
            _rndHome = rndHome;
            _rndFormulations = rndFormulations;
            _cLists = cLists;
        }

        // Mirrors the old page's OnGet.
        public bool Initialize()
        {
            if (_rndHome.IdSet <= 0) return false;

            _rndHome.GetDataSet(_rndHome.IdSet);
            _rndFormulations.ReadDataset();
            SetView();

            return true;
        }

        public string GetId()
        {
            if (_rndHome.drS == null || _rndHome.drS["ID"] == DBNull.Value) return string.Empty;
            return ((int)_rndHome.drS["ID"]).ToString();
        }

        public string GetStudyName() => GetString("Study Name");
        public int GetChemist() => GetInt("Chemist", 24);
        public int GetOperator() => GetInt("Operator", 28);
        public string GetProductId() => _rndHome.drS?["Product ID"] == DBNull.Value ? "Experimental" : (_rndHome.drS?["Product ID"]?.ToString() ?? "Experimental");
        public int GetStudyType() => GetInt("Study Type", 1);
        public DateTime? GetDateCreated() => _rndHome.drS?["DateDSCreated"] == DBNull.Value ? null : (DateTime?)_rndHome.drS?["DateDSCreated"];
        public bool GetAbandoned() => _rndHome.drS?["Abandoned"] != DBNull.Value && (bool)(_rndHome.drS?["Abandoned"] ?? false);
        public string GetFoamatGm() => _rndFormulations.cDefualts.FoamatRunWt.ToString();

        private string GetString(string field)
            => _rndHome.drS?[field] == DBNull.Value ? string.Empty : (_rndHome.drS?[field]?.ToString() ?? string.Empty);

        private int GetInt(string field, int fallback)
            => _rndHome.drS?[field] == DBNull.Value ? fallback : Convert.ToInt32(_rndHome.drS?[field]);

        // Mirrors the old page's SetView (recomputes FormDescriptors and the dtFormProp grid).
        public void SetView()
        {
            var f = _rndFormulations;
            try
            {
                f.FormDescriptors();

                int ir = 1;
                f.dtFormProp.Rows[ir]["Descriptors"] = "Weights and Ratios";
                f.dtFormProp.Rows[ir + 1]["Descriptors"] = "   B Side - PBW of Polyol Side Materials";
                f.dtFormProp.Rows[ir + 2]["Descriptors"] = "   A+B Side - PBW Total of all Materials";
                f.dtFormProp.Rows[ir + 3]["Descriptors"] = "   %A - Percent Isocyanate";
                f.dtFormProp.Rows[ir + 4]["Descriptors"] = "   %B - Percent Polyol Side Materials";
                f.dtFormProp.Rows[ir + 5]["Descriptors"] = "   Foam - Cup Isocyanate (A) Side (gm)";
                f.dtFormProp.Rows[ir + 6]["Descriptors"] = "   Foam - Cup Polyol (B) Side (gm)";

                for (int ifo = 0; ifo < f.Forms.nForm; ifo++)
                {
                    double dSum = f.Forms.FormAr[ifo].TotalPbwPOSide + f.Forms.FormAr[ifo].TotalPbwIsoSide;
                    f.dtFormProp.Rows[ir + 1][ifo + 1] = f.Forms.FormAr[ifo].TotalPbwPOSide.ToString("0.0");
                    f.dtFormProp.Rows[ir + 2][ifo + 1] = dSum.ToString("0.0");
                    f.dtFormProp.Rows[ir + 3][ifo + 1] = (100.0 * f.Forms.FormAr[ifo].TotalPbwIsoSide / dSum).ToString("0.0");
                    f.dtFormProp.Rows[ir + 4][ifo + 1] = (100.0 * f.Forms.FormAr[ifo].TotalPbwPOSide / dSum).ToString("0.0");
                    f.dtFormProp.Rows[ir + 5][ifo + 1] = (f.cDefualts.FoamatRunWt * f.Forms.FormAr[ifo].TotalPbwIsoSide / dSum).ToString("0.0");
                    f.dtFormProp.Rows[ir + 6][ifo + 1] = (f.cDefualts.FoamatRunWt * f.Forms.FormAr[ifo].TotalPbwPOSide / dSum).ToString("0.0");
                }

                ir += 8;
                f.dtFormProp.Rows[ir]["Descriptors"] = "Polyol Info.";
                f.dtFormProp.Rows[ir + 1]["Descriptors"] = "   OH # for Polyol Mix";
                f.dtFormProp.Rows[ir + 2]["Descriptors"] = "   OH # for all Active H";
                f.dtFormProp.Rows[ir + 3]["Descriptors"] = "   Av. Polyol Func.";

                for (int ifo = 0; ifo < f.Forms.nForm; ifo++)
                {
                    f.dtFormProp.Rows[ir + 1][ifo + 1] = f.Forms.FormAr[ifo].OHNumPolyol.ToString("0.0");
                    f.dtFormProp.Rows[ir + 2][ifo + 1] = f.Forms.FormAr[ifo].OHNumPOSide.ToString("0.0");
                    f.dtFormProp.Rows[ir + 3][ifo + 1] = f.Forms.FormAr[ifo].FuncAvPOSide.ToString("0.00");
                }

                ir += 6;
                f.dtFormProp.Rows[ir]["Descriptors"] = "Iso Info.";
                f.dtFormProp.Rows[ir + 1]["Descriptors"] = "   Av. NCO content for Isocyanates ";
                f.dtFormProp.Rows[ir + 2]["Descriptors"] = "   Av. functionality of Isocyanates ";
                for (int ifo = 0; ifo < f.Forms.nForm; ifo++)
                {
                    f.dtFormProp.Rows[ir + 1][ifo + 1] = f.Forms.FormAr[ifo].NcoIsoSide.ToString("0.00");
                    f.dtFormProp.Rows[ir + 2][ifo + 1] = f.Forms.FormAr[ifo].FuncAvIsoSide.ToString("0.00");
                }

                ir += 4;
                f.dtFormProp.Rows[ir]["Descriptors"] = "Formulation Analysis";
                f.dtFormProp.Rows[ir + 1]["Descriptors"] = "   Water level (% Formulation)";
                f.dtFormProp.Rows[ir + 2]["Descriptors"] = "   Other Blowing Agent Wt. (% of polymer)";
                f.dtFormProp.Rows[ir + 3]["Descriptors"] = "   Catalyst level (% Formulation)";
                f.dtFormProp.Rows[ir + 4]["Descriptors"] = "   Surfactant level (% Formulation)";
                f.dtFormProp.Rows[ir + 6]["Descriptors"] = "   Foam: Theoretical Min. Density (lb/ft³) at STP";
                f.dtFormProp.Rows[ir + 7]["Descriptors"] = "   Wt. Percent of Isocyanurate in the polymer";
                f.dtFormProp.Rows[ir + 8]["Descriptors"] = "   Moles of crosslinks per kg of material";

                for (int ifo = 0; ifo < f.Forms.nForm; ifo++)
                {
                    f.dtFormProp.Rows[ir + 1][ifo + 1] = f.Forms.FormAr[ifo].WaterWtFr.ToString("0.00");
                    f.dtFormProp.Rows[ir + 2][ifo + 1] = f.Forms.FormAr[ifo].BlowingAgentWtFr1.ToString("0.00");
                    f.dtFormProp.Rows[ir + 3][ifo + 1] = f.Forms.FormAr[ifo].CatalystWtFr.ToString("0.00");
                    f.dtFormProp.Rows[ir + 4][ifo + 1] = f.Forms.FormAr[ifo].SurfactWtFr.ToString("0.00");
                    f.dtFormProp.Rows[ir + 6][ifo + 1] = (f.Forms.FormAr[ifo].FoamDensity / 16.018463).ToString("0.00");
                    f.dtFormProp.Rows[ir + 7][ifo + 1] = f.Forms.FormAr[ifo].IsocyanuratePbw.ToString("0.00");
                    f.dtFormProp.Rows[ir + 8][ifo + 1] = f.Forms.FormAr[ifo].CrosslinkDensity.ToString("0.000");
                }

                f.Forms.IsoMats[0].Pbw1 = f.Forms.FormAr[0].TotalPbwIsoSide.ToString("0.0");
                f.Forms.IsoMats[0].Pbw2 = f.Forms.FormAr[1].TotalPbwIsoSide.ToString("0.0");
                f.Forms.IsoMats[0].Pbw3 = f.Forms.FormAr[2].TotalPbwIsoSide.ToString("0.0");
                f.Forms.IsoMats[0].Pbw4 = f.Forms.FormAr[3].TotalPbwIsoSide.ToString("0.0");
                f.Forms.IsoMats[0].Pbw5 = f.Forms.FormAr[4].TotalPbwIsoSide.ToString("0.0");
                f.Forms.IsoMats[0].Pbw6 = f.Forms.FormAr[5].TotalPbwIsoSide.ToString("0.0");
                f.Forms.IsoMats[0].Pbw7 = f.Forms.FormAr[6].TotalPbwIsoSide.ToString("0.0");
                f.Forms.IsoMats[0].Pbw8 = f.Forms.FormAr[7].TotalPbwIsoSide.ToString("0.0");
            }
            catch
            {
                // Same behavior as the old page: swallow calculation errors and keep whatever was computed so far.
            }
        }

        // Mirrors OnPostGenInfo_LF.
        public void UpdateGenInfo(string name, string value)
        {
            if (_rndHome.drS == null) return;
            _rndFormulations.bDataSetChanged = true;

            switch (name)
            {
                case "gStudyName":
                    _rndHome.drS["Study Name"] = value ?? (object)DBNull.Value;
                    if (_rndHome.indSet >= 0 && _rndHome.indSet < _rndHome.dt.Rows.Count)
                        _rndHome.dt.Rows[_rndHome.indSet]["Study Name"] = value ?? string.Empty;
                    break;

                case "gChemist":
                    if (int.TryParse(value, out int chemist))
                    {
                        _rndHome.drS["Chemist"] = chemist;
                        if (_rndHome.indSet >= 0 && _rndHome.indSet < _rndHome.dt.Rows.Count)
                            _rndHome.dt.Rows[_rndHome.indSet]["Operator"] = chemist;
                    }
                    break;

                case "gOperator":
                    if (int.TryParse(value, out int op))
                    {
                        _rndHome.drS["Operator"] = op;
                        if (_rndHome.indSet >= 0 && _rndHome.indSet < _rndHome.dt.Rows.Count)
                            _rndHome.dt.Rows[_rndHome.indSet]["Operator"] = op;
                    }
                    break;

                case "gProductID":
                    _rndHome.drS["Product ID"] = string.IsNullOrEmpty(value) ? (object)DBNull.Value : value;
                    if (_rndHome.indSet >= 0 && _rndHome.indSet < _rndHome.dt.Rows.Count)
                        _rndHome.dt.Rows[_rndHome.indSet]["Product ID"] = value ?? string.Empty;
                    break;

                case "gStudyType":
                    if (int.TryParse(value, out int studyType))
                        _rndHome.drS["Study Type"] = studyType;
                    break;

                case "gDateDSCreated":
                    if (DateTime.TryParse(value, out var dt))
                    {
                        _rndHome.drS["DateDSCreated"] = dt;
                        if (_rndHome.indSet >= 0 && _rndHome.indSet < _rndHome.dt.Rows.Count)
                            _rndHome.dt.Rows[_rndHome.indSet]["DateDSCreated"] = dt;
                    }
                    break;

                case "gAbandoned":
                    _rndHome.drS["Abandoned"] = bool.TryParse(value, out var ab) && ab;
                    break;
            }

            _rndHome.UpdateDataSet();
        }

        // Mirrors OnPostGNcoCellEditEnding.
        public void UpdateNcoCell(int irow, int icol, string text)
        {
            if (irow != 0 || icol <= 0 || icol >= _rndFormulations.Forms.nForm + 1) { SetView(); return; }

            double current = _rndFormulations.Forms.FormAr[icol - 1].NcoIndex;
            if (double.TryParse(text, out double dtmp))
            {
                _rndFormulations.Forms.FormAr[icol - 1].NcoIndex = dtmp;
            }

            SetView();
            _rndHome.dtF.Rows[icol - 1]["NCOIndex"] = _rndFormulations.Forms.FormAr[icol - 1].NcoIndex;
            _rndHome.UpdateFormulatiions();
        }

        // Mirrors OnPostGPOCellEditEnding. icol: 0 = material select, 2 = OH#, 4-11 = form pbw columns.
        public void UpdatePOCell(int irow, int icol, string text)
        {
            var forms = _rndFormulations.Forms;
            if (irow > forms.nComps - 2) return;

            if (icol == 0)
            {
                if (int.TryParse(text, out int iSel) && iSel > -1)
                {
                    _rndFormulations.ModifyPOIsoLists(irow, ref forms.POMats, iSel, _rndFormulations.dtPO);

                    string sMatIds = forms.POMats[0].ID.ToString();
                    for (int j = 1; j < forms.POMats.Count; j++) sMatIds += "," + forms.POMats[j].ID;
                    _rndHome.drS["POMats"] = sMatIds;
                    _rndHome.UpdateDataSet();

                    string sOh = forms.POMats[0].MatOHNum.ToString();
                    for (int j = 1; j < forms.POMats.Count; j++) sOh += "," + forms.POMats[j].MatOHNum;
                    _rndHome.drS["sPOMatsOH"] = sOh;
                    _rndHome.UpdateDataSet();
                }
            }
            else if (icol == 2)
            {
                double.TryParse(text, out double dtmp);
                forms.POMats[irow].MatOHNum = dtmp;

                string sOh = forms.POMats[0].MatOHNum.ToString();
                for (int j = 1; j < forms.POMats.Count; j++) sOh += "," + forms.POMats[j].MatOHNum;
                _rndHome.drS["sPOMatsOH"] = sOh;
                _rndHome.UpdateDataSet();
            }
            else if (icol > 3 && icol < 12)
            {
                double current = forms.FormAr[icol - 4].POMatPbw[irow];
                if (string.IsNullOrEmpty(text))
                {
                    forms.FormAr[icol - 4].POMatPbw[irow] = 0.0;
                }
                else if (double.TryParse(text, out double dtmp))
                {
                    forms.FormAr[icol - 4].POMatPbw[irow] = dtmp;
                }

                string js1 = System.Text.Json.JsonSerializer.Serialize(forms.FormAr[icol - 4].POMatPbw);
                _rndHome.dtF.Rows[icol - 4]["POPbws"] = js1;
                _rndHome.UpdateFormulatiions();
            }

            if (!string.IsNullOrEmpty(forms.POMats[irow].MatName))
                SetView();
        }

        // Mirrors OnPostGIsoCellEditEnding. icol: 0 = material select, 2 = %NCO, 4-11 = form pbw columns.
        public void UpdateIsoCell(int irow, int icol, string text)
        {
            var forms = _rndFormulations.Forms;
            if (irow > forms.nComps - 2) return;

            if (icol == 0)
            {
                if (irow > forms.IsoMats.Count - 1)
                    forms.IsoMats.Add(new CMaterial());

                if (int.TryParse(text, out int iSel) && iSel > -1 && irow == 0)
                {
                    _rndFormulations.ModifyPOIsoLists(irow, ref forms.IsoMats, iSel, _rndFormulations.dtIso);
                    _rndHome.drS["IsoMats"] = forms.IsoMats[0].ID;
                    _rndHome.UpdateDataSet();
                }
            }
            else if (icol == 2)
            {
                double.TryParse(text, out double dtemp);
                forms.IsoMats[irow].MatNco = dtemp;
                _rndHome.drS["sIsoMatsNCO"] = forms.IsoMats[0].MatNco.ToString();
                _rndHome.UpdateDataSet();
            }
            else if (icol > 3 && icol < 11)
            {
                double.TryParse(text, out forms.FormAr[icol - 4].IsoMatPbw[irow]);
            }

            if (!string.IsNullOrEmpty(forms.IsoMats[irow].MatName))
                SetView();
        }

        // Mirrors OnPostGPO_AddARow.
        public bool AddPORow()
        {
            var forms = _rndFormulations.Forms;
            if (forms.POMats.Count > 28) return false;

            forms.POMats.Add(new CMaterial());
            _rndHome.drS["PORows"] = forms.POMats.Count;
            _rndHome.UpdateDataSet();
            return true;
        }

        // Mirrors OnPostGPO_Paste. Rows are tab/comma separated pbw values, one row per material, one column per form (1-8).
        public void PastePOData(List<string[]> rows)
        {
            var forms = _rndFormulations.Forms;
            if (rows == null) return;

            for (int ir = 0; ir < rows.Count; ir++)
            {
                if (ir >= forms.POMats.Count) break;
                var cols = rows[ir];
                for (int ic = 0; ic < cols.Length && ic < 8; ic++)
                {
                    string stmp = cols[ic];
                    bool ok = double.TryParse(stmp, out double dtmp);
                    forms.FormAr[ic].POMatPbw[ir] = ok ? dtmp : 0;
                    string val = ok ? stmp : string.Empty;

                    switch (ic)
                    {
                        case 0: forms.POMats[ir].Pbw1 = val; break;
                        case 1: forms.POMats[ir].Pbw2 = val; break;
                        case 2: forms.POMats[ir].Pbw3 = val; break;
                        case 3: forms.POMats[ir].Pbw4 = val; break;
                        case 4: forms.POMats[ir].Pbw5 = val; break;
                        case 5: forms.POMats[ir].Pbw6 = val; break;
                        case 6: forms.POMats[ir].Pbw7 = val; break;
                        case 7: forms.POMats[ir].Pbw8 = val; break;
                    }
                }
            }

            SetView();

            for (int ic = 0; ic < 8; ic++)
            {
                string js1 = System.Text.Json.JsonSerializer.Serialize(forms.FormAr[ic].POMatPbw);
                _rndHome.dtF.Rows[ic]["POPbws"] = js1;
            }
            _rndHome.UpdateFormulatiions();
        }

        // Mirrors OnPostOngFoamatGmLostFocus.
        public void UpdateFoamatGm(string value)
        {
            double.TryParse(value, out _rndFormulations.cDefualts.FoamatRunWt);
            SetView();
            _rndHome.drS["FoamatGm"] = _rndFormulations.cDefualts.FoamatRunWt;
            _rndHome.UpdateDataSet();
        }
    }
}
