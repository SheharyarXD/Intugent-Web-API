namespace IntugentBackend.Models
{
    public class MfgAnalysisDto
    {
        public List<AnalysisFilterOptionDto> ProductOptions { get; set; } = new();
        public List<AnalysisFilterOptionDto> LocationOptions { get; set; } = new();
        public List<AnalysisFilterOptionDto> PropertyOptions { get; set; } = new();

        public string? Prod1SelectedValue { get; set; }
        public string? MfgSiteSelectedValue { get; set; }
        public string Prop1SelectedValue { get; set; } = string.Empty;
        public DateTime? MfgDate1 { get; set; }
        public DateTime? MfgDate2 { get; set; }

        public List<double> XA { get; set; } = new();
        public List<double> YA { get; set; } = new();
        public List<double> XAvg1 { get; set; } = new();
        public List<double> YAvg1 { get; set; } = new();
        public List<double> YUCL1 { get; set; } = new();
        public List<double> YLCL1 { get; set; } = new();

        public List<CorrRowDto> Correlations { get; set; } = new();
    }

    public class AnalysisFilterOptionDto
    {
        public string Value { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class CorrRowDto
    {
        public string PropName { get; set; } = string.Empty;
        public double CorrValue { get; set; }
    }

    public class MfgAnalysisSearchRequest
    {
        public string? Prop1SelectedValue { get; set; }
        public string? Prod1SelectedValue { get; set; }
        public string? MfgSiteSelectedValue { get; set; }
        public DateTime? MfgDate1 { get; set; }
        public DateTime? MfgDate2 { get; set; }
    }

    public class MfgAnalysisPropRequest
    {
        public string Value { get; set; } = string.Empty;
    }
}
