using IntugentBackend.Services.Data;
using System.Data;

namespace IntugentBackend.Services.Mfg
{
    public class MfgAnalysis2Service
    {
        private readonly CAnalysisData _data;

        public MfgAnalysis2Service(CAnalysisData data)
        {
            _data = data;
        }

        public void EnsureListsLoaded()
        {
            if (_data.dtGlobalProducts == null || _data.dtGlobalProducts.Rows.Count == 0)
                _data.GetLists();
        }

        /// <summary>
        /// Always queries the trailing 12 months across all products/locations — this page never exposed
        /// filter controls in the legacy app, so GetSearchCriteria() there always produced this fixed window.
        /// </summary>
        public void Load()
        {
            var date2 = DateTime.Now;
            var date1 = DateTime.Now.AddYears(-1);
            string sql = $"[Test Date] < '{date2.AddDays(1)}' And [Test Date] >= '{date1}'";
            _data.GetData(sql);
            DrawCharts();
        }

        public void UpdateAxes(string? x1, string? x2, string? y1, string? y2)
        {
            if (!string.IsNullOrEmpty(x1)) _data.X1SelectedValue = x1;
            if (!string.IsNullOrEmpty(x2)) _data.X2SelectedValue = x2;
            if (!string.IsNullOrEmpty(y1)) _data.Y1SelectedValue = y1;
            if (!string.IsNullOrEmpty(y2)) _data.Y2SelectedValue = y2;
            DrawCharts();
        }

        public IReadOnlyList<MfgAnalysisService.FilterOption> GetPropertyOptions()
        {
            var result = new List<MfgAnalysisService.FilterOption>();
            if (_data.dtProps == null) return result;
            foreach (DataRow row in _data.dtProps.Rows)
                result.Add(new MfgAnalysisService.FilterOption { Value = row["PropName"]?.ToString() ?? string.Empty, Name = row["PropName"]?.ToString() ?? string.Empty });
            return result;
        }

        private void DrawCharts()
        {
            var dt = _data.dtPropValues;

            (List<double> x, List<double> y) Pair(string colX, string colY)
            {
                if (!dt.Columns.Contains(colX) || !dt.Columns.Contains(colY)) return (new List<double>(), new List<double>());

                var xs = new double[dt.Rows.Count];
                var ys = new double[dt.Rows.Count];
                int n = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var vx = dt.Rows[i][colX];
                    var vy = dt.Rows[i][colY];
                    if (vx == DBNull.Value || vy == DBNull.Value) continue;
                    if (double.IsNaN((double)vx) || double.IsNaN((double)vy)) continue;
                    xs[n] = (double)vx;
                    ys[n] = (double)vy;
                    n++;
                }
                Array.Resize(ref xs, n);
                Array.Resize(ref ys, n);
                return (xs.ToList(), ys.ToList());
            }

            (_data.X1Y1_X, _data.X1Y1_Y) = Pair(_data.X1SelectedValue, _data.Y1SelectedValue);
            (_data.X1Y2_X, _data.X1Y2_Y) = Pair(_data.X1SelectedValue, _data.Y2SelectedValue);
            (_data.X2Y1_X, _data.X2Y1_Y) = Pair(_data.X2SelectedValue, _data.Y1SelectedValue);
            (_data.X2Y2_X, _data.X2Y2_Y) = Pair(_data.X2SelectedValue, _data.Y2SelectedValue);
        }
    }
}
