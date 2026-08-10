namespace IntugentBackend.Models
{
    public class PlantDataDto
    {
        public bool GDataSetNextIsEnabled { get; set; }
        public bool GDataSetPrevIsEnabled { get; set; }
        public string GID { get; set; } = string.Empty;
        public string GProductionDate { get; set; } = string.Empty;
        public string GProductionTime { get; set; } = string.Empty;
        public string GQCCheckTime { get; set; } = string.Empty;
        public string GFBTime { get; set; } = string.Empty;
        public string GProductCode { get; set; } = string.Empty;
        public string GDelTimeButton { get; set; } = string.Empty;
        public bool GGetPlantDataIsEnabled { get; set; }

        public List<ProcessParamRow> GChemDel { get; set; } = new();
        public List<ProcessParamRow> GChemDel1 { get; set; } = new();
        public List<ProcessParamRow> GPTable { get; set; } = new();
        public List<ProcessParamRow> GDBelt { get; set; } = new();
        public List<ProcessParamRow> GOthers { get; set; } = new();
        public List<ProcessParamRow> GNewInsData { get; set; } = new();

        public string? Message { get; set; }
    }

    public class ProcessParamRow
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class PlantDataNavigateRequest
    {
        public string Direction { get; set; } = string.Empty;
    }
}
