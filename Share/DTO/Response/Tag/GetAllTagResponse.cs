using Share.Models.LiveStraming;
using System.Text.Json.Serialization;

namespace Share.DTO.Response.Tag
{
    public class GetAllTagResponse
    {
        [JsonPropertyName("totalPage")]
        public int TotalPage { get; set; }

        [JsonPropertyName("tagList")]
        public List<TagResponse> TagList { get; set; } = new List<TagResponse>();

        [JsonPropertyName("tagTypeList")]
        public List<TagTypeResponse> TagTypeList { get; set; } = new List<TagTypeResponse>();
    }

    public class TagResponse
    {
        [JsonPropertyName("id")]
        public LstId Id { get; set; } = LstId.From(0);

        [JsonPropertyName("name")]
        public string TagName { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string TagType { get; set; } = string.Empty;
    }

    public class TagTypeResponse
    {
        [JsonPropertyName("id")]
        public LsttType Id { get; set; } = LsttType.From(0);

        [JsonPropertyName("name")]
        public string TypeName { get; set; } = string.Empty;
    }
}
