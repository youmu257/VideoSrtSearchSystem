using Share.Models.LiveStraming;
using System.Text.Json.Serialization;

namespace Share.DTO.Request.Tag
{
    public class AddTagRequest
    {
        /// <summary>
        /// 標籤名字
        /// </summary>
        [JsonPropertyName("tagName")]
        public string TagName { get; set; } = string.Empty;

        /// <summary>
        /// 標籤類型
        /// </summary>
        [JsonPropertyName("tagType")]
        public LsttType TagType { get; set; } = LsttType.From(0);
    }
}
