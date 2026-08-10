namespace IntugentBackend.Models
{
    public class JetMixingDto
    {
        public string GFRate_A { get; set; } = string.Empty;
        public string GFRate_B { get; set; } = string.Empty;
        public string GTemp_A { get; set; } = string.Empty;
        public string GTemp_B { get; set; } = string.Empty;
        public string GPres_A { get; set; } = string.Empty;
        public string GPres_B { get; set; } = string.Empty;
        public string GDens_A { get; set; } = string.Empty;
        public string GDens_B { get; set; } = string.Empty;
        public string GVisO_A { get; set; } = string.Empty;
        public string GVisO_B { get; set; } = string.Empty;
        public string GVisE_A { get; set; } = string.Empty;
        public string GVisE_B { get; set; } = string.Empty;

        public string GDiaMixChamb { get; set; } = string.Empty;
        public string GDiaNoz_A { get; set; } = string.Empty;
        public string GDiaNoz_B { get; set; } = string.Empty;
        public string GPres_Max { get; set; } = string.Empty;
        public string GPres_Min { get; set; } = string.Empty;
        public string GReNo_Min { get; set; } = string.Empty;

        public string GMsg { get; set; } = string.Empty;

        public List<JetDetailRow> GDetails { get; set; } = new();

        public double[] XA { get; set; } = Array.Empty<double>();
        public double[] YA { get; set; } = Array.Empty<double>();
        public double[] XB { get; set; } = Array.Empty<double>();
        public double[] YB { get; set; } = Array.Empty<double>();
    }

    public class JetDetailRow
    {
        public string Description { get; set; } = string.Empty;
        public string JetA { get; set; } = string.Empty;
        public string JetB { get; set; } = string.Empty;
    }

    public class JetFieldUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    public class JetImportRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
