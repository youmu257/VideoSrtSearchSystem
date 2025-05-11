using Share.DTO.Response.Tag;
using Share.Models.LiveStraming;
using System.Text.Json.Serialization;

namespace Share.DTO.Response.Video
{
    public class GetOneVideoInfoResponse
    {
        [JsonPropertyName("guid")]
        public string VideoGuid { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string VideoTitle { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string VideoUrl { get; set; } = string.Empty;

        [JsonPropertyName("tagList")]
        public List<TagEditDTO> TagList { get; set; } = new List<TagEditDTO>();

        [JsonPropertyName("tagTypeList")]
        public List<TagTypeResponse> TagTypeList { get; set; } = new List<TagTypeResponse>();
    }

    public class TagEditDTO
    {
        [JsonPropertyName("id")]
        public LstId Id { get; set; } = LstId.From(0);

        [JsonPropertyName("name")]
        public string TagName { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public LsttType TagType { get; set; } = LsttType.From(0);
    }
}
