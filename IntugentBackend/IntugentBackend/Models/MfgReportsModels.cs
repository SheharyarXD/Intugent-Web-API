namespace IntugentBackend.Models
{
    public class MfgReportRowDto
    {
        public string Id { get; set; } = string.Empty;
        public string MfgTime { get; set; } = string.Empty;
        public string QcTestDate { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public string PassFail { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        public string Thickness { get; set; } = string.Empty;
        public string ThicknessFlag { get; set; } = string.Empty;
        public string RValue { get; set; } = string.Empty;
        public string RValueFlag { get; set; } = string.Empty;
        public string CompStrength { get; set; } = string.Empty;
        public string CsFlag { get; set; } = string.Empty;
        public string CoreDens { get; set; } = string.Empty;
        public string CoreDensFlag { get; set; } = string.Empty;
        public string Squareness { get; set; } = string.Empty;
        public string SquarenessFlag { get; set; } = string.Empty;
        public string Length { get; set; } = string.Empty;
        public string LengthFlag { get; set; } = string.Empty;
        public string Width { get; set; } = string.Empty;
        public string WidthFlag { get; set; } = string.Empty;
    }

    public class MfgReportDateRequest
    {
        public DateTime Date { get; set; }
    }
}
