namespace IntugentBackend.Models
{
    public class DimStabilityDataDto
    {
        public bool GDataSetNextIsEnabled { get; set; }
        public bool GDataSetPrevIsEnabled { get; set; }
        public string GID { get; set; } = string.Empty;
        public bool GDimStabilityDoneIsChecked { get; set; }
        public string GProductionDate { get; set; } = string.Empty;
        public string GProductionTime { get; set; } = string.Empty;
        public string GProductCode { get; set; } = string.Empty;
        public bool GEdgeH1IsChecked { get; set; }
        public bool GEdgeH2IsChecked { get; set; }
        public bool GEdgeC1IsChecked { get; set; }
        public bool GEdgeC2IsChecked { get; set; }

        // Oven / Freezer dimension grids: key = "{Init|Final}_{H1|H2|C1|C2}_{L|W|T}{n}"
        public Dictionary<string, string> Dims { get; set; } = new();

        public string GChangeOvenLength { get; set; } = string.Empty;
        public string GChangeOvenWidth { get; set; } = string.Empty;
        public string GChangeOvenThickness { get; set; } = string.Empty;
        public string GChangeFreezerLength { get; set; } = string.Empty;
        public string GChangeFreezerWidth { get; set; } = string.Empty;
        public string GChangeFreezerThickness { get; set; } = string.Empty;

        public bool GChangeOvenLengthBackground { get; set; } = true;
        public bool GChangeOvenWidthBackground { get; set; } = true;
        public bool GChangeOvenThicknessBackground { get; set; } = true;
        public bool GChangeFreezerLengthBackground { get; set; } = true;
        public bool GChangeFreezerWidthBackground { get; set; } = true;
        public bool GChangeFreezerThicknessBackground { get; set; } = true;

        // Walk-in Freezer major dims: key = "Init_WF_L1".."Init_WF_W3", "Final_WF_L1".."Final_WF_W3"
        public Dictionary<string, string> WFDims { get; set; } = new();
        public string GChangeWFLength { get; set; } = string.Empty;
        public string GChangeWFWidth { get; set; } = string.Empty;

        // Depths: key = "Side1_Depth1".."Side1_Depth5", "Side2_Depth1".."Side2_Depth5"
        public Dictionary<string, string> Depths { get; set; } = new();
        public string GAvgDepth { get; set; } = string.Empty;
        public string GMaxDepth { get; set; } = string.Empty;

        public DateTime? GTestDateTime { get; set; }
        public string GDeviation { get; set; } = string.Empty;
        public string GDevType { get; set; } = string.Empty;
    }

    public class DimNavigateRequest
    {
        public string Direction { get; set; } = string.Empty;
    }

    public class DimFieldUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    public class DimBoolUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public bool Value { get; set; }
    }

    public class DimDateTimeUpdateRequest
    {
        public DateTime? Value { get; set; }
    }
}
