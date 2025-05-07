using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Share.DTO.Request.Srt
{
    [BindRequired]
    public class SearchSrtRequest
    {
        /// <summary>
        /// 關鍵字
        /// </summary>
        [JsonPropertyName("keyword")]
        public string Keyword { get; set; } = string.Empty;

        /// <summary>
        /// 分頁
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; set; } = 1;

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
