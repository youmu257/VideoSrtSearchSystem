using Share.Models.LiveStraming;
using System.Text.Json.Serialization;

namespace Share.DTO.Request.Tag
{
    public class EditTagRequest : AddTagRequest
    {
        /// <summary>
        /// 標籤 ID
        /// </summary>
        [JsonPropertyName("id")]
        public LstId TagId { get; set; } = LstId.From(0);
    }
}
