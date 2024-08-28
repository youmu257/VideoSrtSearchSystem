using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace VideoSrtSearchSystem.DTO.Request.Srt
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
    }
}
