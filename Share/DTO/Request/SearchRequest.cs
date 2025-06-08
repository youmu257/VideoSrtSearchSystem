using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Share.DTO.Request
{
    [BindRequired]
    public class SearchRequest
    {
        /// <summary>
        /// 關鍵字查詢
        /// </summary>
        [JsonPropertyName("keyword")]
        public string Keyword { get; set; } = string.Empty;

        /// <summary>
        /// 標籤關鍵字查詢
        /// </summary>
        [JsonPropertyName("tagKeyword")]
        public string TagKeyword { get; set; } = string.Empty;

        /// <summary>
        /// 分頁
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; set; } = 1;
    }
}
