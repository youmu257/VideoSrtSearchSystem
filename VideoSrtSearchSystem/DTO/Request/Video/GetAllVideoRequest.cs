using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace VideoSrtSearchSystem.DTO.Request.Video
{
    [BindRequired]
    public class GetAllVideoRequest
    {
        /// <summary>
        /// 標題查詢
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
