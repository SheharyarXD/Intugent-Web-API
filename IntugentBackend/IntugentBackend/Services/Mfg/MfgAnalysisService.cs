using IntugentBackend.Services.Data;
using System.Data;

namespace IntugentBackend.Services.Mfg
{
    public class MfgAnalysisService
    {
        private readonly CAnalysisData _data;

        public MfgAnalysisService(CAnalysisData data)
        {
            _data = data;
        }

        public void EnsureListsLoaded()
        {
            if (_data.dtGlobalProducts == null || _data.dtGlobalProducts.Rows.Count == 0)
                _data.GetLists();
        }

        public void Search()
        {
            _data.GetData(GetSearchCriteria());
            SetView();
        }

        public void ChangeProperty(string value)
        {
            _data.Prop1SelectedValue = value;
            SetView();
        }

        private void SetView()
        {
            if (string.IsNullOrEmpty(_data.Prop1SelectedValue)) return;
            _data.CorrMatrix(_data.Prop1SelectedValue);
            _data.dtCorr.DefaultView.Sort = "AbsValue DESC";
            PlotSpc();
        }

        private void PlotSpc()
        {
            var dt = _data.dtPropValues;
            var x1 = new double[dt.Rows.Count];
            var y1 = new double[dt.Rows.Count];

            _data.XAvg1 = new(); _data.YAvg1 = new(); _data.YUCL1 = new(); _data.YLCL1 = new();

            const string scn1 = "Green-Board Time Stamp";
            string scn2 = _data.Prop1SelectedValue;

            if (!dt.Columns.Contains(scn1) || !dt.Columns.Contains(scn2))
            {
                _data.XA = new(); _data.YA = new();
                return;
            }

            int ncount = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][scn1] == DBNull.Value || dt.Rows[i][scn2] == DBNull.Value) continue;
                if (double.IsNaN((double)dt.Rows[i][scn1]) || double.IsNaN((double)dt.Rows[i][scn2])) continue;
                x1[ncount] = (double)dt.Rows[i][scn1];
                y1[ncount] = (double)dt.Rows[i][scn2];
                ncount++;
            }

            if (ncount < 1)
            {
                _data.XA = x1.ToList();
                _data.YA = y1.ToList();
                return;
            }

            Array.Resize(ref x1, ncount);
            Array.Resize(ref y1, ncount);

            double davg = 0.0, dSig = 0.0;
            for (int i = 0; i < y1.Length; i++) davg += y1[i];
            davg /= ncount;
            for (int i = 0; i < y1.Length; i++) dSig += (y1[i] - davg) * (y1[i] - davg);
            dSig = Math.Sqrt(dSig / ncount);

            _data.XA = x1.ToList();
            _data.YA = y1.ToList();
            _data.XAvg1 = new List<double> { x1[0], x1[^1] };
            _data.YAvg1 = new List<double> { davg, davg };
            _data.YUCL1 = new List<double> { davg + 2.0 * dSig, davg + 2.0 * dSig };
            _data.YLCL1 = new List<double> { davg - 2.0 * dSig, davg - 2.0 * dSig };
        }

        private string GetSearchCriteria()
        {
            string sql = string.Empty, sql1;

            if (!string.IsNullOrEmpty(_data.MfgSiteSelectedValue) && _data.MfgSiteSelectedValue != "All Locations")
                sql = $"sLocation = '{_data.MfgSiteSelectedValue}'";

            if (!string.IsNullOrEmpty(_data.Prod1SelectedValue) && _data.Prod1SelectedValue != "All Products")
            {
                sql1 = $"[Product Code Global] = '{_data.Prod1SelectedValue}'";
                sql = sql == string.Empty ? sql1 : sql + " And " + sql1;
            }

            if (_data.MfgDate2 != null)
            {
                var d = _data.MfgDate2.Value.AddDays(1);
                sql1 = $"[Test Date] < '{d}'";
                sql = sql == string.Empty ? sql1 : sql + " And " + sql1;
            }

            if (_data.MfgDate1 != null)
            {
                sql1 = $"[Test Date] >= '{_data.MfgDate1.Value}'";
                sql = sql == string.Empty ? sql1 : sql + " And " + sql1;
            }

            return sql;
        }

        public IReadOnlyList<FilterOption> GetProductOptions() => ExtractOptions(_data.dtGlobalProducts, "Product Code Global", "GProd");
        public IReadOnlyList<FilterOption> GetLocationOptions() => ExtractOptions(_data.dtLocations, "sLocation", "sLocation");
        public IReadOnlyList<FilterOption> GetPropertyOptions() => ExtractOptions(_data.dtProps, "PropName", "PropName");

        public IReadOnlyList<CorrRow> GetCorrelationRows()
        {
            var rows = new List<CorrRow>();
            if (_data.dtCorr == null) return rows;
            foreach (DataRowView row in _data.dtCorr.DefaultView)
            {
                rows.Add(new CorrRow
                {
                    PropName = row["PropName"]?.ToString() ?? string.Empty,
                    CorrValue = row["CorrValue"] != DBNull.Value ? Convert.ToDouble(row["CorrValue"]) : 0
                });
            }
            return rows;
        }

        private static List<FilterOption> ExtractOptions(DataTable? dt, string valueCol, string nameCol)
        {
            var result = new List<FilterOption>();
            if (dt == null) return result;
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new FilterOption
                {
                    Value = row[valueCol]?.ToString() ?? string.Empty,
                    Name = row[nameCol]?.ToString() ?? string.Empty
                });
            }
            return result;
        }

        public class FilterOption
        {
            public string Value { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        public class CorrRow
        {
            public string PropName { get; set; } = string.Empty;
            public double CorrValue { get; set; }
        }
    }
}
