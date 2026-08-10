namespace IntugentBackend.Models
{
    public class AiModelViewDto
    {
        public string GMaxIter { get; set; } = string.Empty;
        public string GConvTol { get; set; } = string.Empty;
        public string GLearnRate { get; set; } = string.Empty;
        public string GStepSizeMin { get; set; } = string.Empty;
        public string GnHiddenLayers { get; set; } = string.Empty;
        public string GHLayerType { get; set; } = string.Empty;
        public string GRMS { get; set; } = string.Empty;
        public string GInputNeurons { get; set; } = string.Empty;
        public string GOutputNeurons { get; set; } = string.Empty;

        public List<NeuronRowDto> NeuronRows { get; set; } = new();
        public List<string> WeightColumns { get; set; } = new();
        public List<List<string>> WeightRows { get; set; } = new();

        public List<string> LayerOptions { get; set; } = new();
        public int LayerSelectedIndex { get; set; }

        public double[] YY { get; set; } = Array.Empty<double>();
        public double[] YYP { get; set; } = Array.Empty<double>();
        public double[] YTh { get; set; } = Array.Empty<double>();
        public string ChartBottomTitle { get; set; } = string.Empty;
        public string ChartLeftTitle { get; set; } = string.Empty;
    }

    public class NeuronRowDto
    {
        public int LayerNo { get; set; }
        public string Description { get; set; } = string.Empty;
        public int NeuronCount { get; set; }
    }

    public class AiModelFieldRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    public class AiModelNodeEditRequest
    {
        public int RowId { get; set; }
        public int ColId { get; set; }
        public string? Text { get; set; }
    }

    public class AiModelLayerRequest
    {
        public int Index { get; set; }
    }
}
