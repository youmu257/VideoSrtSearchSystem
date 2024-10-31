using System.Text.Json.Serialization;

namespace Share.DTO.Response.Video
{
    public class GetOneVideoInfoResponse
    {
        [JsonPropertyName("guid")]
        public string VideoGuid { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string VideoTitle { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string VideoUrl { get; set; } = string.Empty;
    }
}
