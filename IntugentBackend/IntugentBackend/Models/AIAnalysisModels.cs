namespace IntugentBackend.Models
{
    public class AiAnalysisDto
    {
        public string GStudyName { get; set; } = string.Empty;
        public string GDataFile { get; set; } = string.Empty;
        public string GSQL { get; set; } = string.Empty;
        public string GGroup { get; set; } = string.Empty;
        public string GProperty { get; set; } = string.Empty;
        public string GSource { get; set; } = string.Empty;
        public string GID { get; set; } = string.Empty;

        public List<string> DataColumns { get; set; } = new();
        public List<List<string>> DataRows { get; set; } = new();

        public List<string> StatColumns { get; set; } = new() { "Variable", "Average", "StdDev", "Correlation" };
        public List<List<string>> StatRows { get; set; } = new();

        public List<string> InputVars { get; set; } = new();
        public int InputVarSelectedIndex { get; set; }
        public string ChartLeftTitle { get; set; } = string.Empty;
        public string ChartBottomTitle { get; set; } = string.Empty;

        public double[] XX { get; set; } = Array.Empty<double>();
        public double[] YY { get; set; } = Array.Empty<double>();
    }

    public class AiAnalysisFieldRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    public class AiAnalysisFileRequest
    {
        public string FilePath { get; set; } = string.Empty;
    }

    public class AiAnalysisInputVarRequest
    {
        public int Index { get; set; }
    }
}
