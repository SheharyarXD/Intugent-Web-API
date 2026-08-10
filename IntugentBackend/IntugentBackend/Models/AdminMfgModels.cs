namespace IntugentBackend.Models
{
    public class IPTargetUploadRequest
    {
        public List<List<string>> Clipboard { get; set; } = new();
    }

    public class IPTargetRowDto
    {
        public Dictionary<string, string> Values { get; set; } = new();
    }
}
