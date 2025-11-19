using Microsoft.AspNetCore.Mvc;
using Share.Const;
using Share.DTO.Request;
using Share.Services.Video;
using Share.Tool.Language;

namespace VideoSrtSearchSystem.Controllers
{
    /// <summary>
    /// 影片控制器 - 負責處理影片相關的 API 請求
    /// </summary>
    /// <param name="_videoService">影片服務介面</param>
    /// <param name="_logger">日誌記錄器</param>
    [ApiController]
    [Route("api/video")]
    public class VideoController(
        ILiveStreamingService _videoService,
        ILogger<VideoController> _logger) : BaseController<VideoController>(_logger)
    {
        /// <summary>
        /// 取得所有影片列表 - 支援分頁和關鍵字搜尋
        /// </summary>
        /// <param name="request">搜尋請求，包含關鍵字和頁碼</param>
        /// <returns>影片列表和分頁資訊</returns>
        /// <remarks>
        /// 此 API 用於查詢所有影片，支援：
        /// - 關鍵字搜尋（可選）
        /// - 分頁功能（必須提供有效的頁碼）
        /// </remarks>
        [HttpGet]
        [Route("all")]
        public IActionResult GetAllVideo(SearchRequest request)
        {
            try
            {
                // 驗證頁碼參數（必須大於 0）
                if (request.Page <= 0)
                {
                    return ParameterFormatError("Page");
                }

                // 呼叫服務取得影片列表
                var result = _videoService.GetAllVideo(request);
                
                return Ok(ResponseCode.SUCCESS, LangTool.GetTranslation("common_success"), result);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }
    }
}
