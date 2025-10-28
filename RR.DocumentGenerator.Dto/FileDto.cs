using System.Text.Json.Serialization;

namespace RR.DocumentGenerator.Dto
{
    public class FileDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("base64")]
        public string Base64 { get; set; } = null!;

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; } = null!;

        [JsonPropertyName("size")]
        public long Size { get; set; }
    }
}
