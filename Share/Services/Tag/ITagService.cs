using Share.DTO.Request.Tag;
using Share.DTO.Response.Tag;
using Share.Models.LiveStraming;

namespace Share.Services.Tag
{
    /// <summary>
    /// 標籤服務介面 - 負責標籤的查詢、新增、編輯和管理
    /// </summary>
    public interface ITagService
    {
        /// <summary>
        /// 搜尋標籤（支援關鍵字和標籤類型篩選）
        /// </summary>
        /// <param name="request">搜尋標籤請求，包含關鍵字和標籤類型</param>
        /// <returns>標籤列表和總頁數</returns>
        GetAllTagResponse SearchTags(SearchTagRequest request);

        /// <summary>
        /// 取得所有標籤映射列表（JSON 格式）
        /// </summary>
        /// <returns>標籤映射的 JSON 字串</returns>
        string GetTagsMappingList();

        /// <summary>
        /// 取得指定類型的標籤列表（JSON 格式）
        /// </summary>
        /// <param name="tagType">標籤類型 ID</param>
        /// <returns>標籤列表的 JSON 字串</returns>
        string GetTagsList(int tagType);

        /// <summary>
        /// 取得所有標籤類型列表
        /// </summary>
        /// <returns>標籤類型列表</returns>
        List<TagTypeResponse> GetTagType();

        /// <summary>
        /// 取得指定標籤的詳細資料
        /// </summary>
        /// <param name="tagId">標籤 ID</param>
        /// <returns>標籤資料模型</returns>
        LiveStreamingTagModel GetTagData(LstId tagId);

        /// <summary>
        /// 新增標籤
        /// </summary>
        /// <param name="request">新增標籤請求，包含標籤類型和名稱</param>
        /// <returns>新增的標籤 ID</returns>
        uint InsertTag(AddTagRequest request);

        /// <summary>
        /// 更新標籤資料
        /// </summary>
        /// <param name="request">編輯標籤請求，包含標籤 ID、類型和名稱</param>
        void UpdateTag(EditTagRequest request);
    }
}
