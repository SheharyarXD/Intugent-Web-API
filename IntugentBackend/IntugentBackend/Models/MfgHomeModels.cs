namespace IntugentBackend.Models
{
    // ========== REQUESTS ==========

    public class SearchMfgRequest
    {
        public string? ProductCode { get; set; }
        public DateTime? DateFrom { get; set; }      // gMfgDate1 (Before or At)
        public DateTime? DateTo { get; set; }        // gMfgDate2 (After or At)
        public int TestingStatusId { get; set; }     // gTestStat1SelectedValue
        public int AgedRValueId { get; set; }          // gAgedRValueSelectedValue
        public int DimStabilityId { get; set; }        // gDimStabilitySelectedValue
        public int RunTypeId { get; set; }             // gRunType1SelectedValue
    }

    public class SelectDatasetRequest
    {
        public int SelectedIndex { get; set; }         // gMfgSearchSelectedIndex
        public int RowCount { get; set; }              // gMfgSearchRowsCount
        public int DatasetId { get; set; }             // gMfgSelectedDatasetID
    }

    // ========== RESPONSES ==========

    public class MfgSearchResultDto
    {
        public List<Dictionary<string, object?>> Rows { get; set; } = new();
        public List<string> Columns { get; set; } = new();
        public int SelectedIndex { get; set; }
        public int CurrentDatasetId { get; set; }
    }

    public class MfgFiltersDto
    {
        public List<FilterOptionDto> Products { get; set; } = new();
        public List<FilterOptionDto> TestingStatus { get; set; } = new();
        public List<FilterOptionDto> AgedRValue { get; set; } = new();
        public List<FilterOptionDto> DimStability { get; set; } = new();
        public List<FilterOptionDto> RunTypes { get; set; } = new();
        public string Location { get; set; } = string.Empty;
        public string DefaultProductCode { get; set; } = string.Empty;
        public int DefaultTestingStatusId { get; set; }
        public int DefaultAgedRValueId { get; set; }
        public int DefaultDimStabilityId { get; set; }
        public int DefaultRunTypeId { get; set; }
        public DateTime? DefaultDateFrom { get; set; }
        public DateTime? DefaultDateTo { get; set; }
    }

    public class FilterOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
    }
}