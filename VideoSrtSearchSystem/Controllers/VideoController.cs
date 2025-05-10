using Microsoft.AspNetCore.Mvc;
using Share.Const;
using Share.DTO.Request;
using Share.Services.Video;
using Share.Tool.Language;

namespace VideoSrtSearchSystem.Controllers
{
    [ApiController]
    [Route("api/video")]
    public class VideoController(
        ILiveStreamingService _videoService,
        ILogger<VideoController> _logger) : BaseController<VideoController>(_logger)
    {
        /// <summary>
        /// 取得所有影片
        /// </summary>
        [HttpGet]
        [Route("all")]
        public IActionResult GetAllVideo(SearchRequest request)
        {
            try
            {
                if (request.Page <= 0)
                {
                    return ParameterFormatError("Page");
                }

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
