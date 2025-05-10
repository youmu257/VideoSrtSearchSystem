using Share.DTO.Request.Tag;
using Share.DTO.Response.Tag;

namespace Share.Services.Tag
{
    public interface ITagService
    {
        GetAllTagResponse SearchTags(SearchTagRequest request);
        List<TagTypeResponse> GetTagType();
        void InsertTag(AddTagRequest request);
    }
}
