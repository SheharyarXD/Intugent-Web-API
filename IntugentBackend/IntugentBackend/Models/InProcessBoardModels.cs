namespace IntugentBackend.Models
{
    public class InProcessBoardDataDto
    {
        // Navigation
        public bool GDataSetNextIsEnabled { get; set; }
        public bool GDataSetPrevIsEnabled { get; set; }
        public string GID { get; set; } = string.Empty;

        // Location labels
        public string GsLoc1A { get; set; } = string.Empty;
        public string GsLoc2A { get; set; } = string.Empty;
        public string GsLoc3A { get; set; } = string.Empty;
        public string GsLoc1E { get; set; } = string.Empty;
        public string GsLoc3E { get; set; } = string.Empty;
        public string GsLoc3F { get; set; } = string.Empty;
        public string GsLoc1B { get; set; } = string.Empty;
        public string GsLoc2B { get; set; } = string.Empty;
        public string GsLoc3B { get; set; } = string.Empty;
        public string GsLoc1C { get; set; } = string.Empty;
        public string GsLoc2C { get; set; } = string.Empty;
        public string GsLoc3C { get; set; } = string.Empty;
        public string GsLoc1D { get; set; } = string.Empty;
        public string GsLoc2D { get; set; } = string.Empty;
        public string GsLoc3D { get; set; } = string.Empty;

        // General Info (read-only, no writer in the legacy page)
        public DateTime? GTestDateTime { get; set; }
        public string GProdIDSelectedValue { get; set; } = string.Empty;
        public int GRunTypeSelectedValue { get; set; }
        public int GShiftSelectedValue { get; set; }
        public int GOperatorSelectedItem { get; set; }
        public int GPaperManufacturerSelectedValue { get; set; }
        public string GRunningWetDesnsity { get; set; } = string.Empty;
        public string GWarehouseTemp { get; set; } = string.Empty;
        public string GWarehouseHumidity { get; set; } = string.Empty;
        public bool GInProcessDoneIsChecked { get; set; }
        public bool GAbandonedIsChecked { get; set; }
        public bool GTimeStampNotLegibleIsChecked { get; set; }

        // Avg / QC summary (read-only)
        public string GCoreDensityIPText { get; set; } = string.Empty;
        public string GThicknessIPText { get; set; } = string.Empty;
        public string GCompressiveIPText { get; set; } = string.Empty;
        public string GCompressiveIP5Text { get; set; } = string.Empty;
        public string GThicknessValleyText { get; set; } = string.Empty;
        public string GThicknessPeakText { get; set; } = string.Empty;
        public string GFlatnessText { get; set; } = string.Empty;

        // Board Dimensions (editable)
        public string GLengthText { get; set; } = string.Empty;
        public string GWidthText { get; set; } = string.Empty;
        public string GBundleHeightIPText { get; set; } = string.Empty;
        public string GDiagoanl1Text { get; set; } = string.Empty;
        public string GDiagoanl2Text { get; set; } = string.Empty;
        public string GDiagoanlDiffText { get; set; } = string.Empty;

        // Thickness array (editable)
        public string GThicknessIP_1Text { get; set; } = string.Empty;
        public string GThicknessIP_2Text { get; set; } = string.Empty;
        public string GThicknessIP_3Text { get; set; } = string.Empty;
        public string GThicknessIP_4Text { get; set; } = string.Empty;
        public string GThicknessIP_5Text { get; set; } = string.Empty;
        public string GThicknessIP_6Text { get; set; } = string.Empty;
        public string GThicknessIP_7Text { get; set; } = string.Empty;
        public string GThicknessIP_8Text { get; set; } = string.Empty;
        public string GThicknessIP_9Text { get; set; } = string.Empty;
        public string GThicknessIP_10Text { get; set; } = string.Empty;
        public string GThicknessIP_11Text { get; set; } = string.Empty;
        public string GThicknessIP_12Text { get; set; } = string.Empty;
        public string GThicknessIP_13Text { get; set; } = string.Empty;
        public string GThicknessIP_14Text { get; set; } = string.Empty;
        public string GThicknessIP_15Text { get; set; } = string.Empty;
        public string GThicknessIP_16Text { get; set; } = string.Empty;
        public string GThicknessIP_17Text { get; set; } = string.Empty;
        public string GThicknessAvg1Text { get; set; } = string.Empty;
        public string GThicknessAvg2Text { get; set; } = string.Empty;
        public string GThicknessSlopeText { get; set; } = string.Empty;

        // Board Perforations (editable bool)
        public bool GTopBoardPerforatedIsChecked { get; set; }
        public bool GBottomBoardPerforatedIsChecked { get; set; }
        public bool GTopBoardPrintOKIsChecked { get; set; }
        public bool GBottomBoardPrintOKIsChecked { get; set; }

        // Compressive Strength (editable)
        public string GCompressiveIP_1Text { get; set; } = string.Empty;
        public string GCompressiveIP_2Text { get; set; } = string.Empty;
        public string GCompressiveIP_3Text { get; set; } = string.Empty;
        public string GCompressiveIP_4Text { get; set; } = string.Empty;
        public string GCompressiveIP_5Text { get; set; } = string.Empty;
        public string GCompressiveIP_6Text { get; set; } = string.Empty;
        public bool GCompStrKnitPresent_1IsChecked { get; set; }
        public bool GCompStrKnitPresent_2IsChecked { get; set; }
        public bool GCompStrKnitPresent_3IsChecked { get; set; }
        public bool GCompStrKnitPresent_4IsChecked { get; set; }
        public bool GCompStrKnitPresent_5IsChecked { get; set; }
        public bool GCompStrKnitPresent_6IsChecked { get; set; }

        // Core Density rows (read-only)
        public string GMass_1Text { get; set; } = string.Empty;
        public string GL1_1Text { get; set; } = string.Empty;
        public string GW1_1Text { get; set; } = string.Empty;
        public string GT1_1Text { get; set; } = string.Empty;
        public string GT2_1Text { get; set; } = string.Empty;
        public string GT3_1Text { get; set; } = string.Empty;
        public string GT4_1Text { get; set; } = string.Empty;
        public string GT5_1Text { get; set; } = string.Empty;
        public bool GCoreKnitPresent_1IsChecked { get; set; }
        public string GCoreDensityIP_1Text { get; set; } = string.Empty;

        public string GMass_2Text { get; set; } = string.Empty;
        public string GL1_2Text { get; set; } = string.Empty;
        public string GW1_2Text { get; set; } = string.Empty;
        public string GT1_2Text { get; set; } = string.Empty;
        public string GT2_2Text { get; set; } = string.Empty;
        public string GT3_2Text { get; set; } = string.Empty;
        public string GT4_2Text { get; set; } = string.Empty;
        public string GT5_2Text { get; set; } = string.Empty;
        public bool GCoreKnitPresent_2IsChecked { get; set; }
        public string GCoreDensityIP_2Text { get; set; } = string.Empty;

        public string GMass_3Text { get; set; } = string.Empty;
        public string GL1_3Text { get; set; } = string.Empty;
        public string GW1_3Text { get; set; } = string.Empty;
        public string GT1_3Text { get; set; } = string.Empty;
        public string GT2_3Text { get; set; } = string.Empty;
        public string GT3_3Text { get; set; } = string.Empty;
        public string GT4_3Text { get; set; } = string.Empty;
        public string GT5_3Text { get; set; } = string.Empty;
        public bool GCoreKnitPresent_3IsChecked { get; set; }
        public string GCoreDensityIP_3Text { get; set; } = string.Empty;

        // Pour Table (read-only)
        public string GTimePourTableQCCheckCustomFormat { get; set; } = string.Empty;
        public DateTime? GTimePourTableQCCheckValue { get; set; }
        public int GSurfactantSelectedValue { get; set; }
        public int GLayoutSelectedValue { get; set; }
        public string GSplitterAgeText { get; set; } = string.Empty;
        public string GHeadPlateAgeText { get; set; } = string.Empty;
        public string GPourHeadPosition_1Text { get; set; } = string.Empty;
        public string GPourHeadPosition_2Text { get; set; } = string.Empty;
        public string GPourHeadPosition_3Text { get; set; } = string.Empty;
        public string GIRFacerTempLowerText { get; set; } = string.Empty;
        public string GIRFacerTempUpperText { get; set; } = string.Empty;
        public string GIRStreamTempPourHead_1Text { get; set; } = string.Empty;
        public string GIRStreamTempPourHead_2Text { get; set; } = string.Empty;
        public string GIRStreamTempPourHead_3Text { get; set; } = string.Empty;
        public string GIRNipRoll_1Text { get; set; } = string.Empty;
        public string GIRNipRoll_2Text { get; set; } = string.Empty;
        public string GIRNipRoll_3Text { get; set; } = string.Empty;

        // Knit Line Locations (editable)
        public string GKnitLineLoc_1Text { get; set; } = string.Empty;
        public string GKnitLineLoc_2Text { get; set; } = string.Empty;
        public string GKnitLineLoc_3Text { get; set; } = string.Empty;
        public string GKnitLineLoc_4Text { get; set; } = string.Empty;
        public string GKnitLineLoc_5Text { get; set; } = string.Empty;
        public string GKnitLineLoc_6Text { get; set; } = string.Empty;
        public string GKnitLineLoc_7Text { get; set; } = string.Empty;

        // Background colors (validation states)
        public bool GThicknessIPBackground { get; set; } = true;
        public bool GThicknessAvg1Background { get; set; } = true;
        public bool GThicknessAvg2Background { get; set; } = true;
        public bool GThicknessSlopeBackground { get; set; } = true;
        public bool GFlatnessBackground { get; set; } = true;
        public bool GBundleHeightIPBackground { get; set; } = true;
        public bool GLengthBackground { get; set; } = true;
        public bool GWidthBackground { get; set; } = true;
        public bool GDiagoanlDiffBackground { get; set; } = true;
        public bool GCoreDensityIPBackground { get; set; } = true;
        public bool GCoreDensityIP_1Background { get; set; } = true;
        public bool GCoreDensityIP_2Background { get; set; } = true;
        public bool GCoreDensityIP_3Background { get; set; } = true;
        public bool GCompressiveIPBackground { get; set; } = true;
        public bool GCompressiveIP5Background { get; set; } = true;
    }

    // ========== REQUESTS ==========

    public class IPNavigateRequest
    {
        public string Direction { get; set; } = string.Empty; // "prev" or "next"
    }

    public class IPFieldUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    public class IPBoolUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public bool Value { get; set; }
    }
}
