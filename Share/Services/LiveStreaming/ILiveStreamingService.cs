using Share.DTO.Request;
using Share.DTO.Request.Video;
using Share.DTO.Response.Video;

namespace Share.Services.Video
{
    /// <summary>
    /// 直播影片服務介面 - 定義影片資料的查詢和管理操作
    /// </summary>
    public interface ILiveStreamingService
    {
        /// <summary>
        /// 取得所有影片列表
        /// </summary>
        /// <param name="request">搜尋請求，包含關鍵字和分頁資訊</param>
        /// <returns>包含影片列表和總頁數的回應物件</returns>
        GetAllVideoResponse GetAllVideo(SearchRequest request);

        /// <summary>
        /// 取得單一影片的詳細資訊
        /// </summary>
        /// <param name="guid">影片的唯一識別碼 (GUID)</param>
        /// <returns>影片詳細資訊，包含標題、URL、標籤列表等</returns>
        GetOneVideoInfoResponse GetOneVideoInfo(string guid);

        /// <summary>
        /// 更新影片資訊
        /// </summary>
        /// <param name="request">編輯影片請求，包含要更新的標題和標籤資料</param>
        void UpdateVideoInfo(EditVideoRequest request);
    }
}
