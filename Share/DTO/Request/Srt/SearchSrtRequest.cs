using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Share.DTO.Request.Srt
{
    public class SearchSrtRequest : SearchRequest
    {
        /// <summary>
        /// 開始時間
        /// </summary>
        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;

        /// <summary>
        /// 結束時間
        /// </summary>
        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;
    }
}
