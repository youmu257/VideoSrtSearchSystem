using Share.DTO.Response.Video;
using System.Text.Json.Serialization;

namespace Share.DTO.Request.Video
{
    public class EditVideoRequest
    {
        /// <summary>
        /// 影片 guid
        /// </summary>
        [JsonPropertyName("videoGuid")]
        public string VideoGuid { get; set; } = string.Empty;

        /// <summary>
        /// 影片標題
        /// </summary>
        [JsonPropertyName("videoTitle")]
        public string VideoTitle { get; set; } = string.Empty;

        /// <summary>
        /// 標籤名字
        /// </summary>
        [JsonPropertyName("tagList")]
        public List<TagEditDTO> TagList { get; set; } = new List<TagEditDTO>();
    }
}
