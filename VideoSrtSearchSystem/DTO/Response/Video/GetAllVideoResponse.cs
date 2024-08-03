using System.Text.Json.Serialization;

namespace VideoSrtSearchSystem.DTO.Response.Video
{
    public class GetAllVideoResponse
    {
        [JsonPropertyName("guid")]
        public string VideoGuid { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string VideoTitle { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string VideoUrl { get; set; } = string.Empty;
    }
}
