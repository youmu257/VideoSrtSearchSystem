using System.Text.Json.Serialization;

namespace Share.DTO.Response.Srt
{
    public class SearchSrtResponse
    {
        public int TotalPage { get; set; }

        public List<SearchSrtVideoResponse> VideoList { get; set; } = new List<SearchSrtVideoResponse>();
    }

    public class SearchSrtVideoResponse
    {
        [JsonPropertyName("title")]
        public string VideoTitle { get; set; } = string.Empty;

        [JsonPropertyName("guid")]
        public string VideoGuid { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string VideoUrl { get; set; } = string.Empty;

        [JsonPropertyName("srts")]
        public List<SrtResponse> SrtList { get; set; } = new List<SrtResponse>();
    }

    public class SrtResponse
    {
        [JsonPropertyName("context")]
        public string Context { get; set; } = string.Empty;

        [JsonPropertyName("startTimestamp")]
        public int SrtStartTimeSeconds { get; set; }

        [JsonPropertyName("start")]
        public string SrtStartTime { get; set; } = string.Empty;

        [JsonPropertyName("end")]
        public string SrtEndTime { get; set; } = string.Empty;
    }
}
