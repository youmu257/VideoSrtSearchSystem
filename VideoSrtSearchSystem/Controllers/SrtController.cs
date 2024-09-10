using Microsoft.AspNetCore.Mvc;
using System.Web;
using VideoSrtSearchSystem.Config;
using VideoSrtSearchSystem.DTO.Request.Srt;
using VideoSrtSearchSystem.Services.Srt;
using VideoSrtSearchSystem.Tool.Language;

namespace VideoSrtSearchSystem.Controllers
{
    [ApiController]
    [Route("api/srt")]
    public class SrtController(
        ISrtService _srtService,
        ILogger<SrtController> _logger) : BaseController<SrtController>(_logger)
    {
        /// <summary>
        /// 匯入字幕
        /// </summary>
        [HttpPost]
        [Route("import")]
        public IActionResult ImportSrt(ImportSrtRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SrtPath))
                {
                    return ParameterIsRequired("SrtPath");
                }
                if (string.IsNullOrEmpty(request.VideoTitle))
                {
                    return ParameterIsRequired("VideoTitle");
                }
                if (string.IsNullOrEmpty(request.VideoUrl))
                {
                    return ParameterIsRequired("VideoUrl");
                }

                _srtService.ImportSrt(request);
                return Ok(ResponseCode.SUCCESS, LangTool.GetTranslation("common_success"));
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        /// <summary>
        /// 查詢字幕功能
        /// </summary>
        [HttpGet]
        [Route("search")]
        public IActionResult SearchSrt([FromQuery] SearchSrtRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Keyword))
                {
                    return ParameterIsRequired("Keyword");
                }

                var response = _srtService.SearchSrt(request.Keyword, request.Page);
                return Ok(ResponseCode.SUCCESS, LangTool.GetTranslation("common_success"), response.VideoList);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        /// <summary>
        /// 下載字幕
        /// </summary>
        [HttpGet]
        [Route("download")]
        public IActionResult DownloadSrt([FromQuery] string videoGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(videoGuid))
                {
                    return ParameterIsRequired("guid");
                }

                var srtData = _srtService.DownloadSrt(videoGuid);
                // 對中文檔名編碼
                var encodedFileName = HttpUtility.UrlEncode(srtData.FileName);

                return File(srtData.SrtFile, "text/plain; charset=utf-8", encodedFileName);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }
    }
}
