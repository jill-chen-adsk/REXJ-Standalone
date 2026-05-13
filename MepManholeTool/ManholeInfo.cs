using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MepManholeTool.Models
{
    public class ManholeMapping
    {
        [JsonPropertyName("Family")]
        public string Family { get; set; }

        [JsonPropertyName("Mapping")]
        public List<ParameterMapping> Mapping { get; set; }
    }

    public class ParameterMapping
    {
        [JsonPropertyName("Category")]
        public string Category { get; set; }

        [JsonPropertyName("FromParameter")]
        public string FromParameter { get; set; }

        [JsonPropertyName("ToParameter")]
        public string ToParameter { get; set; }

        [JsonPropertyName("Required")]
        public bool Required { get; set; }
    }
}
