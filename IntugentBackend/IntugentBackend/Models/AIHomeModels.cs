namespace IntugentBackend.Models
{
    public class AiModelListDto
    {
        public List<AiModelRowDto> Rows { get; set; } = new();
        public int SelectedId { get; set; }
    }

    public class AiModelRowDto
    {
        public int Id { get; set; }
        public string DateModel { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Property { get; set; } = string.Empty;
        public string DataSource { get; set; } = string.Empty;
    }

    public class SelectAiModelRequest
    {
        public int Id { get; set; }
    }
}
