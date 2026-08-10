namespace IntugentBackend.Models
{
    public class ProcessCheckDataDto
    {
        public string GID { get; set; } = string.Empty;
        public bool GDataSetNextIsEnabled { get; set; }
        public bool GDataSetPrevIsEnabled { get; set; }
        public bool GHasData { get; set; }

        public string GLoc1 { get; set; } = string.Empty;
        public string GLoc2 { get; set; } = string.Empty;
        public string GLoc3 { get; set; } = string.Empty;

        public DateTime? GTestDate { get; set; }
        public string GProdIDSelected { get; set; } = string.Empty;
        public int GOperatorSelectedItem { get; set; }
        public int GTypeSelectedValue { get; set; }
        public int GTopBoardPrint { get; set; }
        public int GBottomBoardPrint { get; set; }
        public int GPerferation { get; set; }
        public int GFlipper { get; set; }
        public int GAdhesionSelected { get; set; }
        public int GEdgeCutSelected { get; set; }
        public int GHooderSelected { get; set; }
        public int GBoardQualitySelected { get; set; }

        public List<FilterOptionDto> GProdIDList { get; set; } = new();

        // Bundle 1
        public string GQuantity { get; set; } = string.Empty;
        public string GWidth { get; set; } = string.Empty;
        public string GTopLength { get; set; } = string.Empty;
        public string GMiddleLength { get; set; } = string.Empty;
        public string GBottomLength { get; set; } = string.Empty;
        public string GDiagonal1 { get; set; } = string.Empty;
        public string GDiagonal2 { get; set; } = string.Empty;
        public string GLengthAvg_1 { get; set; } = string.Empty;
        public string GSquareness_1 { get; set; } = string.Empty;
        public string GWidthAvg_1 { get; set; } = string.Empty;

        // Bundle 2
        public string GQuantity_2 { get; set; } = string.Empty;
        public string GWidth_2 { get; set; } = string.Empty;
        public string GTopLength_2 { get; set; } = string.Empty;
        public string GMiddleLength_2 { get; set; } = string.Empty;
        public string GBottomLength_2 { get; set; } = string.Empty;
        public string GDiagonal1_2 { get; set; } = string.Empty;
        public string GDiagonal2_2 { get; set; } = string.Empty;
        public string GLengthAvg_2 { get; set; } = string.Empty;
        public string GSquareness_2 { get; set; } = string.Empty;
        public string GWidthAvg_2 { get; set; } = string.Empty;

        // Board thickness
        public string GThickness1 { get; set; } = string.Empty;
        public string GThickness2 { get; set; } = string.Empty;
        public string GThickness3 { get; set; } = string.Empty;
        public string GThicknessAvg { get; set; } = string.Empty;
        public string GTaper { get; set; } = string.Empty;

        // Cup reactivity
        public string GEmptyCupMassG { get; set; } = string.Empty;
        public string GCreamTimeS { get; set; } = string.Empty;
        public string GGelTimeS { get; set; } = string.Empty;
        public string GTackFreeTimeS { get; set; } = string.Empty;
        public string GFullCupMassG { get; set; } = string.Empty;
        public string GFoamDensityPCF { get; set; } = string.Empty;

        // Board deviation
        public string GDeviationAbs { get; set; } = string.Empty;
        public string GDeviationType { get; set; } = string.Empty;
        public string GBoardDeviationRel { get; set; } = string.Empty;

        // Misc
        public bool GExcludeIsChecked { get; set; }
        public string GComment { get; set; } = string.Empty;
        public string GExposedFoam { get; set; } = string.Empty;

        public int GCopyData { get; set; }
    }

    public class ProcessCheckNavigateRequest
    {
        public string Direction { get; set; } = string.Empty;
    }

    public class ProcessCheckFieldUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    public class ProcessCheckDateTimeRequest
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Value { get; set; }
    }

    public class ProcessCheckCopyRequest
    {
        public int CopyData { get; set; }
    }
}
