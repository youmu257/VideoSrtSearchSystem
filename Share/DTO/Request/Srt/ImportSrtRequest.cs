using Microsoft.AspNetCore.Mvc.ModelBinding;
using Share.Models.LiveStraming;
using System.Text.Json.Serialization;

namespace Share.DTO.Request.Srt
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

        /// <summary>
        /// 直播時間
        /// </summary>
        [JsonPropertyName("liveTime")]
        public string LiveTime { get; set; } = string.Empty;

        /// <summary>
        /// 直播標籤
        /// </summary>
        [JsonPropertyName("tags")]
        public List<TagDTO> TagList { get; set; } = new List<TagDTO>();
    }

    public class TagDTO
    {
        /// <summary>
        /// 標籤名字
        /// </summary>
        [JsonPropertyName("name")]
        public string TagName { get; set; } = string.Empty;

        /// <summary>
        /// 標籤類型
        /// </summary>
        [JsonPropertyName("type")]
        public LsttType TagType { get; set; } = LsttType.From(0);
    }
}
