using System.Text.Json.Serialization;

namespace Share.DTO.Request.Tag
{
    public class SearchTagRequest : SearchRequest
    {
        /// <summary>
        /// 標籤類型
        /// </summary>
        [JsonPropertyName("type")]
        public int Type { get; set; }
    }
}
