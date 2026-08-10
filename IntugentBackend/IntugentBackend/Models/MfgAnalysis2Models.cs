namespace IntugentBackend.Models
{
    public class MfgAnalysis2Dto
    {
        public List<AnalysisFilterOptionDto> PropertyOptions { get; set; } = new();

        public string X1SelectedValue { get; set; } = string.Empty;
        public string X2SelectedValue { get; set; } = string.Empty;
        public string Y1SelectedValue { get; set; } = string.Empty;
        public string Y2SelectedValue { get; set; } = string.Empty;

        public List<double> X1Y1_X { get; set; } = new();
        public List<double> X1Y1_Y { get; set; } = new();
        public List<double> X1Y2_X { get; set; } = new();
        public List<double> X1Y2_Y { get; set; } = new();
        public List<double> X2Y1_X { get; set; } = new();
        public List<double> X2Y1_Y { get; set; } = new();
        public List<double> X2Y2_X { get; set; } = new();
        public List<double> X2Y2_Y { get; set; } = new();
    }

    public class MfgAnalysis2AxesRequest
    {
        public string? X1 { get; set; }
        public string? X2 { get; set; }
        public string? Y1 { get; set; }
        public string? Y2 { get; set; }
    }
}
