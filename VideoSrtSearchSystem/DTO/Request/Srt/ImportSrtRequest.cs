using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace VideoSrtSearchSystem.DTO.Request.Srt
{
    [BindRequired]
    public class ImportSrtRequest
    {
        /// <summary>
        /// 字幕檔路徑
        /// </summary>
        [JsonPropertyName("srtPath")]
        public string SrtPath { get; set; } = string.Empty;

        /// <summary>
        /// 影片標題
        /// </summary>
        [JsonPropertyName("videoTitle")]
        public string VideoTitle { get; set; } = string.Empty;

        /// <summary>
        /// 影片連結
        /// </summary>
        [JsonPropertyName("videoUrl")]
        public string VideoUrl { get; set; } = string.Empty;
    }
}
