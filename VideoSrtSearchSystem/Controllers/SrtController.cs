using Microsoft.AspNetCore.Mvc;
using Share.Const;
using Share.DTO.Request.Srt;
using Share.Exceptions;
using Share.Services.Srt;
using Share.Tool;
using Share.Tool.Language;
using System.Web;

namespace VideoSrtSearchSystem.Controllers
{
    /// <summary>
    /// 字幕控制器 - 負責處理字幕的匯入、搜尋和下載功能
    /// </summary>
    /// <param name="_srtService">字幕服務介面</param>
    /// <param name="_commonTool">共用工具介面</param>
    /// <param name="_logger">日誌記錄器</param>
    [ApiController]
    [Route("api/srt")]
    public class SrtController(
        ISrtService _srtService,
        ICommonTool _commonTool,
        ILogger<SrtController> _logger) : BaseController<SrtController>(_logger)
    {
        /// <summary>
        /// 匯入單一影片字幕
        /// </summary>
        /// <param name="request">匯入字幕請求物件，包含影片標題、URL、直播時間和標籤列表</param>
        /// <returns>匯入結果</returns>
        [HttpPost]
        [Route("import")]
        public IActionResult ImportSrt(ImportSrtRequest request)
        {
            try
            {
                // 驗證必填欄位 - 影片標題
                if (string.IsNullOrWhiteSpace(request.VideoTitle))
                {
                    return ParameterIsRequired("VideoTitle");
                }
                
                // 驗證必填欄位 - 影片 URL
                if (string.IsNullOrWhiteSpace(request.VideoUrl))
                {
                    return ParameterIsRequired("VideoUrl");
                }
                
                // 驗證必填欄位 - 直播時間
                if (string.IsNullOrWhiteSpace(request.LiveTime))
                {
                    return ParameterIsRequired("LiveTime");
                }
                
                // 驗證必填欄位 - 標籤列表
                if (request.TagList == null || request.TagList.Count == 0)
                {
                    return ParameterIsRequired("Tags");
                }

                // 呼叫服務匯入字幕
                var responseCode = _srtService.ImportSrt(request);
                if (responseCode != ResponseCode.SUCCESS)
                {
                    throw new MyException(responseCode);
                }
                
                return Ok(ResponseCode.SUCCESS, LangTool.GetTranslation("common_success"));
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        /// <summary>
        /// 批次匯入多個影片字幕
        /// </summary>
        /// <param name="requests">匯入字幕請求列表</param>
        /// <returns>匯入結果，包含失敗的項目列表</returns>
        [HttpPost]
        [Route("import/list")]
        public IActionResult ImportSrtList(List<ImportSrtRequest> requests)
        {
            try
            {
                // 驗證請求列表
                if (requests == null || requests.Count == 0)
                {
                    return ParameterIsRequired("requests");
                }
                
                // 儲存失敗的項目
                var errorUrlIdList = new List<string>();
                
                // 逐一處理每個匯入請求
                foreach (var request in requests)
                {
                    var result = _srtService.ImportSrt(request);
                    if (result != ResponseCode.SUCCESS)
                    {
                        // 記錄失敗的項目
                        errorUrlIdList.Add(result);
                    }
                }
                
                // 返回成功結果和失敗項目列表
                return Ok(ResponseCode.SUCCESS, LangTool.GetTranslation("common_success"), errorUrlIdList);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        /// <summary>
        /// 從資料庫搜尋字幕內容
        /// </summary>
        /// <param name="request">搜尋請求，包含關鍵字和頁碼</param>
        /// <returns>符合關鍵字的影片列表</returns>
        /// <remarks>此方法直接從資料庫查詢，適合小量資料或精確查詢</remarks>
        [HttpGet]
        [Route("searchFromDb")]
        public IActionResult SearchSrtFromDb([FromQuery] SearchSrtRequest request)
        {
            try
            {
                // 驗證必填欄位 - 搜尋關鍵字
                if (string.IsNullOrWhiteSpace(request.Keyword))
                {
                    return ParameterIsRequired("Keyword");
                }

                // 從資料庫搜尋字幕
                var response = _srtService.SearchSrt(request.Keyword, request.Page);
                
                return Ok(ResponseCode.SUCCESS, LangTool.GetTranslation("common_success"), response.VideoList);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        /// <summary>
        /// 從記憶體搜尋字幕內容（高效能搜尋）
        /// </summary>
        /// <param name="request">搜尋請求，包含關鍵字、頁碼、開始日期和結束日期</param>
        /// <returns>符合關鍵字和日期範圍的影片列表</returns>
        /// <remarks>此方法使用記憶體快取，適合大量資料和頻繁查詢</remarks>
        [HttpGet]
        [Route("search")]
        public IActionResult SearchSrtFromMemory([FromQuery] SearchSrtRequest request)
        {
            try
            {
                // 驗證必填欄位 - 搜尋關鍵字
                if (string.IsNullOrWhiteSpace(request.Keyword))
                {
                    return ParameterIsRequired("Keyword");
                }
                
                // 驗證開始日期格式（如果有提供）
                if (!string.IsNullOrWhiteSpace(request.Start) && !_commonTool.IsValidDateFormat(request.Start))
                {
                    return ParameterFormatError("Start");
                }
                
                // 驗證結束日期格式（如果有提供）
                if (!string.IsNullOrWhiteSpace(request.End) && !_commonTool.IsValidDateFormat(request.End))
                {
                    return ParameterFormatError("End");
                }

                // 從記憶體搜尋字幕（高效能）
                var response = _srtService.SearchSrtByMemory(request);
                
                return Ok(ResponseCode.SUCCESS, LangTool.GetTranslation("common_success"), response.VideoList);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        /// <summary>
        /// 下載指定影片的字幕檔案
        /// </summary>
        /// <param name="videoGuid">影片 GUID</param>
        /// <returns>字幕檔案（SRT 格式）</returns>
        /// <remarks>回傳的檔案為 UTF-8 編碼的純文字格式</remarks>
        [HttpGet]
        [Route("download")]
        public IActionResult DownloadSrt([FromQuery] string videoGuid)
        {
            try
            {
                // 驗證必填欄位 - 影片 GUID
                if (string.IsNullOrWhiteSpace(videoGuid))
                {
                    return ParameterIsRequired("videoGuid");
                }

                // 從服務取得字幕資料
                var srtData = _srtService.DownloadSrt(videoGuid);
                
                // 對檔名進行 URL 編碼（處理中文檔名）
                var encodedFileName = HttpUtility.UrlEncode(srtData.FileName);

                // 回傳字幕檔案（Content-Type 設為 UTF-8 編碼的純文字）
                return File(srtData.SrtFile, "text/plain; charset=utf-8", encodedFileName);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }
    }
}
