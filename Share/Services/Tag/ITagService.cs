using Share.DTO.Request.Tag;
using Share.DTO.Response.Tag;
using Share.Models.LiveStraming;

namespace Share.Services.Tag
{
    public interface ITagService
    {
        GetAllTagResponse SearchTags(SearchTagRequest request);
        string GetTagsMappingList();
        List<TagTypeResponse> GetTagType();
        LiveStreamingTagModel GetTagData(LstId tagId);
        uint InsertTag(AddTagRequest request);
        void UpdateTag(EditTagRequest request);
    }
}
