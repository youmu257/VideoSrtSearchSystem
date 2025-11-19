using Share.DTO.Request.Srt;
using Share.DTO.Response.Srt;

namespace Share.Services.Srt
{
    /// <summary>
    /// 字幕服務介面 - 負責字幕檔案的匯入、搜尋和下載
    /// </summary>
    public interface ISrtService
    {
        /// <summary>
        /// 匯入字幕檔案
        /// </summary>
        /// <param name="request">匯入字幕請求，包含影片 GUID 和字幕內容</param>
        /// <returns>匯入結果訊息</returns>
        string ImportSrt(ImportSrtRequest request);

        /// <summary>
        /// 從資料庫搜尋字幕（支援分頁）
        /// </summary>
        /// <param name="keyword">搜尋關鍵字</param>
        /// <param name="page">頁碼（從 1 開始）</param>
        /// <returns>字幕搜尋結果，包含字幕列表和分頁資訊</returns>
        SearchSrtResponse SearchSrt(string keyword, int page);

        /// <summary>
        /// 從記憶體快取搜尋字幕
        /// </summary>
        /// <param name="request">搜尋請求，包含關鍵字和日期範圍</param>
        /// <returns>字幕搜尋結果</returns>
        SearchSrtResponse SearchSrtByMemory(SearchSrtRequest request);

        /// <summary>
        /// 下載指定影片的字幕檔案
        /// </summary>
        /// <param name="videoGuid">影片 GUID</param>
        /// <returns>字幕下載資訊，包含檔案路徑</returns>
        DownloadSrtResponse DownloadSrt(string videoGuid);
    }
}
