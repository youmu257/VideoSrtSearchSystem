using System.Text.Json.Serialization;

namespace VideoSrtSearchSystem.DTO.Response.Video
{
    public class GetAllVideoResponse
    {
        [JsonPropertyName("totalPage")]
        public int TotalPage { get; set; }

        [JsonPropertyName("videoList")]
        public List<VideoResponse> VideoList { get; set; } = new List<VideoResponse>();
    }

    public class VideoResponse
    {
        [JsonPropertyName("guid")]
        public string VideoGuid { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string VideoTitle { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string VideoUrl { get; set; } = string.Empty;
    }
}
