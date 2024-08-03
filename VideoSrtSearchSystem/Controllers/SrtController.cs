using Microsoft.AspNetCore.Mvc;
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

    }
}
