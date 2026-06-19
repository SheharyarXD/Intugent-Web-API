using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Shared.Models
{
    public class FinishedBoardDataDto
    {
        // General Info
        public string GID { get; set; } = string.Empty;
        public string GIPLength { get; set; } = string.Empty;
        public string GIPWidth { get; set; } = string.Empty;
        public string GTestDate { get; set; } = string.Empty;
        public string GProdCode { get; set; } = string.Empty;
        public string GBundleHeight { get; set; } = string.Empty;
        public bool GTestingPassed { get; set; }
        public bool GFinsihedGoodsDone { get; set; }
        public string GTimePourTableQC { get; set; } = string.Empty;
        public string GIPBoardTimeStamp { get; set; } = string.Empty;
        public bool GIPTimeNotLegible { get; set; }
        public DateTime? GFBTimeStamp { get; set; }
        public DateTime? GQCTimesDateTime { get; set; }

        // QC Summary
        public string GCoreDensity { get; set; } = string.Empty;
        public string GCompStrFG_Avg6 { get; set; } = string.Empty;
        public string GCompStrFG_Avg5 { get; set; } = string.Empty;
        public string GThickness { get; set; } = string.Empty;
        public string GFlatness { get; set; } = string.Empty;
        public string GRValue { get; set; } = string.Empty;
        public string GFacerPeelAvg_QC { get; set; } = string.Empty;

        // IP QC Summary
        public string GWarehouseTemp { get; set; } = string.Empty;
        public string GWarehouseHumidity { get; set; } = string.Empty;
        public string GBundleHeightIP { get; set; } = string.Empty;
        public string GThicknessIP { get; set; } = string.Empty;
        public string GCoreDensityIP { get; set; } = string.Empty;
        public string GCompressiveIP5 { get; set; } = string.Empty;
        public string GCompressiveIP { get; set; } = string.Empty;

        // Thickness (17 values)
        public string GThicknessFG_1 { get; set; } = string.Empty;
        public string GThicknessFG_2 { get; set; } = string.Empty;
        public string GThicknessFG_3 { get; set; } = string.Empty;
        public string GThicknessFG_4 { get; set; } = string.Empty;
        public string GThicknessFG_5 { get; set; } = string.Empty;
        public string GThicknessFG_6 { get; set; } = string.Empty;
        public string GThicknessFG_7 { get; set; } = string.Empty;
        public string GThicknessFG_8 { get; set; } = string.Empty;
        public string GThicknessFG_9 { get; set; } = string.Empty;
        public string GThicknessFG_10 { get; set; } = string.Empty;
        public string GThicknessFG_11 { get; set; } = string.Empty;
        public string GThicknessFG_12 { get; set; } = string.Empty;
        public string GThicknessFG_13 { get; set; } = string.Empty;
        public string GThicknessFG_14 { get; set; } = string.Empty;
        public string GThicknessFG_15 { get; set; } = string.Empty;
        public string GThicknessFG_16 { get; set; } = string.Empty;
        public string GThicknessFG_17 { get; set; } = string.Empty;

        // Location labels
        public string GLoc1F { get; set; } = string.Empty;
        public string GLoc3F { get; set; } = string.Empty;
        public string GLoc1A { get; set; } = string.Empty;
        public string GLoc2A { get; set; } = string.Empty;
        public string GLoc3A { get; set; } = string.Empty;
        public string GLoc1B { get; set; } = string.Empty;
        public string GLoc2B { get; set; } = string.Empty;
        public string GLoc3B { get; set; } = string.Empty;
        public string GLoc1C { get; set; } = string.Empty;
        public string GLoc2C { get; set; } = string.Empty;
        public string GLoc3C { get; set; } = string.Empty;
        public string GLoc1D { get; set; } = string.Empty;
        public string GLoc2D { get; set; } = string.Empty;
        public string GLoc3D { get; set; } = string.Empty;
        public string GLoc1E { get; set; } = string.Empty;
        public string GLoc2E { get; set; } = string.Empty;
        public string GLoc3E { get; set; } = string.Empty;

        // Compressive Strength
        public string GCompStrFG_1 { get; set; } = string.Empty;
        public string GCompStrFG_2 { get; set; } = string.Empty;
        public string GCompStrFG_3 { get; set; } = string.Empty;
        public string GCompStrFG_4 { get; set; } = string.Empty;
        public string GCompStrFG_5 { get; set; } = string.Empty;
        public string GCompStrFG_6 { get; set; } = string.Empty;
        public bool GCompStrFGKnit_1 { get; set; }
        public bool GCompStrFGKnit_2 { get; set; }
        public bool GCompStrFGKnit_3 { get; set; }
        public bool GCompStrFGKnit_4 { get; set; }
        public bool GCompStrFGKnit_5 { get; set; }
        public bool GCompStrFGKnit_6 { get; set; }
        public string GNotes { get; set; } = string.Empty;

        // Retest
        public bool GRestestFromSameBundle { get; set; }
        public string GCompStrFGRetest_1 { get; set; } = string.Empty;
        public string GCompStrFGRetest_2 { get; set; } = string.Empty;
        public string GCompStrFGRetest_3 { get; set; } = string.Empty;
        public string GCompStrFGRetest_4 { get; set; } = string.Empty;
        public string GCompStrFGRetest_5 { get; set; } = string.Empty;
        public string GCompStrFGRetest_6 { get; set; } = string.Empty;
        public bool GCompStrFGKnitRetest_1 { get; set; }
        public bool GCompStrFGKnitRetest_2 { get; set; }
        public bool GCompStrFGKnitRetest_3 { get; set; }
        public bool GCompStrFGKnitRetest_4 { get; set; }
        public bool GCompStrFGKnitRetest_5 { get; set; }
        public bool GCompStrFGKnitRetest_6 { get; set; }
        public string GCompStrFGRetest_Avg5 { get; set; } = string.Empty;
        public string GCompStrFGRetest_Avg6 { get; set; } = string.Empty;
        public DateTime? GRetestQCTime { get; set; }

        // k Factor / R Value
        public string GkFactor_1 { get; set; } = string.Empty;
        public string GkFactor_2 { get; set; } = string.Empty;
        public string GkFactor_3 { get; set; } = string.Empty;
        public string GkFactor_Avg { get; set; } = string.Empty;
        public bool GRValueKnitPresent3 { get; set; }
        public bool GRValueKnitPresent2 { get; set; }
        public bool GRValueKnitPresent1 { get; set; }
        public DateTime? GkFactorTime1 { get; set; }
        public DateTime? GkFactorTime2 { get; set; }
        public DateTime? GkFactorTime3 { get; set; }

        // Aged R Value
        public string GkFactor90_3 { get; set; } = string.Empty;
        public string GkFactor90_2 { get; set; } = string.Empty;
        public string GkFactor90_1 { get; set; } = string.Empty;
        public string GkFactor90 { get; set; } = string.Empty;
        public string GkFactor180_3 { get; set; } = string.Empty;
        public string GkFactor180_2 { get; set; } = string.Empty;
        public string GkFactor180_1 { get; set; } = string.Empty;
        public string GkFactor180 { get; set; } = string.Empty;
        public bool GAgedRValueDone { get; set; }
        public DateTime? GKFactor90Date_1 { get; set; }
        public DateTime? GKFactor90Date_2 { get; set; }
        public DateTime? GKFactor90Date_3 { get; set; }
        public DateTime? GKFactor180Date_1 { get; set; }
        public DateTime? GKFactor180Date_2 { get; set; }
        public DateTime? GKFactor180Date_3 { get; set; }

        // Bundle Temps
        public string GLoggerID { get; set; } = string.Empty;
        public string GInitProbeTemp { get; set; } = string.Empty;
        public string GMaxProbeTemp { get; set; } = string.Empty;
        public string GFinalProbeTemp { get; set; } = string.Empty;
        public DateTime? GInitProbeTime { get; set; }
        public DateTime? GMaxTempTimeInit { get; set; }
        public DateTime? GMaxTempTimeFinal { get; set; }
        public DateTime? GFinalProbeTime { get; set; }

        // Core Density
        public string GMass3 { get; set; } = string.Empty;
        public string GMass2 { get; set; } = string.Empty;
        public string GMass1 { get; set; } = string.Empty;
        public string GL1_3 { get; set; } = string.Empty;
        public string GL1_2 { get; set; } = string.Empty;
        public string GL1_1 { get; set; } = string.Empty;
        public string GW1_3 { get; set; } = string.Empty;
        public string GW1_2 { get; set; } = string.Empty;
        public string GW1_1 { get; set; } = string.Empty;
        public string GT1_3 { get; set; } = string.Empty;
        public string GT1_2 { get; set; } = string.Empty;
        public string GT1_1 { get; set; } = string.Empty;
        public string GT2_3 { get; set; } = string.Empty;
        public string GT2_2 { get; set; } = string.Empty;
        public string GT2_1 { get; set; } = string.Empty;
        public string GT3_3 { get; set; } = string.Empty;
        public string GT3_2 { get; set; } = string.Empty;
        public string GT3_1 { get; set; } = string.Empty;
        public string GT4_3 { get; set; } = string.Empty;
        public string GT4_2 { get; set; } = string.Empty;
        public string GT4_1 { get; set; } = string.Empty;
        public string GT5_3 { get; set; } = string.Empty;
        public string GT5_2 { get; set; } = string.Empty;
        public string GT5_1 { get; set; } = string.Empty;
        public string GCoreDens3 { get; set; } = string.Empty;
        public string GCoreDens2 { get; set; } = string.Empty;
        public string GCoreDens1 { get; set; } = string.Empty;
        public bool GCoreDensKnitLine3 { get; set; }
        public bool GCoreDensKnitLine2 { get; set; }
        public bool GCoreDensKnitLine1 { get; set; }

        // FG Board Dimensions
        public string GFGLength { get; set; } = string.Empty;
        public string GFGWidth { get; set; } = string.Empty;
        public string GFGDiagoanl1 { get; set; } = string.Empty;
        public string GFGDiagoanl2 { get; set; } = string.Empty;
        public string GFGDiagonalDiff { get; set; } = string.Empty;

        // Facer Peel
        public string GFacerPeel3 { get; set; } = string.Empty;
        public string GFacerPeel2 { get; set; } = string.Empty;
        public string GFacerPeel1 { get; set; } = string.Empty;
        public string GFacerPeelAvg { get; set; } = string.Empty;

        // Nail Pull
        public string GNailPull_3 { get; set; } = string.Empty;
        public string GNailPull_2 { get; set; } = string.Empty;
        public string GNailPull_1 { get; set; } = string.Empty;
        public string GNailPull { get; set; } = string.Empty;

        // Navigation
        public bool GDataSetPrev { get; set; }
        public bool GDataSetNext { get; set; }

        // Background colors (validation states)
        public bool GIPLengthBackground { get; set; } = true;
        public bool GIPWidthBackground { get; set; } = true;
        public bool GThicknessIPBackground { get; set; } = true;
        public bool GCoreDensityIPBackground { get; set; } = true;
        public bool GCompressiveIPBackground { get; set; } = true;
        public bool GCompressiveIP5Background { get; set; } = true;
        public bool GCoreDensityBackground { get; set; } = true;
        public bool GCoreDens1Background { get; set; } = true;
        public bool GCoreDens2Background { get; set; } = true;
        public bool GCoreDens3Background { get; set; } = true;
        public bool GCompStrFG_Avg6Background { get; set; } = true;
        public bool GCompStrFG_Avg5Background { get; set; } = true;
        public bool GThicknessBackground { get; set; } = true;
        public bool GRValueBackground { get; set; } = true;
        public bool GkFactor_AvgBackground { get; set; } = true;
        public bool GFacerPeelAvgBackground { get; set; } = true;
        public bool GFacerPeelAvg_QCBackground { get; set; } = true;
        public bool GCompStrFGRetest_Avg5Background { get; set; } = true;
        public bool GCompStrFGRetest_Avg6Background { get; set; } = true;
        public bool GkFactor90Background { get; set; } = true;
        public bool GkFactor180Background { get; set; } = true;
        public bool GFGDiagonalDiffBackground { get; set; } = true;
        public bool GFBTimeHostBackground { get; set; } = true;
    }
}
