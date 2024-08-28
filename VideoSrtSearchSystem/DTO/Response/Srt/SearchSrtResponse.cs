using System.Text.Json.Serialization;

namespace VideoSrtSearchSystem.DTO.Response.Srt
{
    public class SearchSrtResponse
    {
        [JsonPropertyName("title")]
        public string VideoTitle { get; set; } = string.Empty;

        [JsonPropertyName("srts")]
        public List<SrtResponse> SrtList { get; set; } = new List<SrtResponse>();
    }

    public class SrtResponse
    {
        [JsonPropertyName("context")]
        public string Context { get; set; } = string.Empty;

        [JsonPropertyName("start")]
        public string SrtStartTime { get; set; } = string.Empty;

        [JsonPropertyName("end")]
        public string SrtEndTime { get; set; } = string.Empty;
    }
}
