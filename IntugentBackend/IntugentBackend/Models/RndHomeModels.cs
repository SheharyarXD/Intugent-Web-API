namespace IntugentBackend.Models
{
    // ========== REQUESTS ==========

    public class SearchRndRequest
    {
        public string? ProductCode { get; set; }
        public DateTime? DateFrom { get; set; }      // gRndDate1 (Before or At)
        public DateTime? DateTo { get; set; }        // gRndDate2 (After or At)
        public int TestingStatusId { get; set; }     // gTestStat1SelectedValue
        public int StudyTypeId { get; set; }           // gStudyTypeSelectedValue
        public string? NameSearch { get; set; }      // gRNDNameSearch
    }

    public class SelectRndDatasetRequest
    {
        public int SelectedIndex { get; set; }
        public int RowCount { get; set; }
        public int DatasetId { get; set; }
    }

    // ========== RESPONSES ==========

    public class RndSearchResultDto
    {
        public List<Dictionary<string, object?>> Rows { get; set; } = new();
        public List<string> Columns { get; set; } = new();
        public int SelectedIndex { get; set; }
        public int CurrentDatasetId { get; set; }
    }

    public class RndFiltersDto
    {
        public List<FilterOptionDto> Products { get; set; } = new();
        public List<FilterOptionDto> TestingStatus { get; set; } = new();
        public List<FilterOptionDto> StudyTypes { get; set; } = new();
        public string Location { get; set; } = string.Empty;
        public string DefaultProductCode { get; set; } = string.Empty;
        public int DefaultTestingStatusId { get; set; }
        public int DefaultStudyTypeId { get; set; }
        public DateTime? DefaultDateFrom { get; set; }
        public DateTime? DefaultDateTo { get; set; }
        public string? DefaultNameSearch { get; set; }
    }

    //public class FilterOptionDto
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public string? Code { get; set; }
    //}

    //public class ApiResponse<T>
    //{
    //    public bool Success { get; set; }
    //    public T? Data { get; set; }
    //    public string? Error { get; set; }
    //}

    public class NewDatasetResponseDto
    {
        public List<string[]>? Rows { get; set; }
        public int SelectedIndex { get; set; }
        public int DatasetId { get; set; }
    }
}