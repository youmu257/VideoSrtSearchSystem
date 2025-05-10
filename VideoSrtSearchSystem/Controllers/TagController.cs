using Microsoft.AspNetCore.Mvc;
using Share.Const;
using Share.DTO.Request.Tag;
using Share.Services.Tag;
using Share.Tool.Language;

namespace VideoSrtSearchSystem.Controllers
{
    [ApiController]
    [Route("api/tag")]
    public class TagController(
        ITagService _tagService,
        ILogger<TagController> _logger
    ) : BaseController<TagController>(_logger)
    {
        /// <summary>
        /// 查詢標籤
        /// </summary>
        [HttpGet]
        [Route("search")]
        public IActionResult SearchTag(SearchTagRequest request)
        {
            try
            {
                if (request.Page <= 0)
                {
                    return ParameterFormatError("Page");
                }
                if (request.Type <= 0)
                {
                    return ParameterFormatError("Type");
                }

                    var result = _tagService.SearchTags(request);
                return Ok(ResponseCode.SUCCESS, LangTool.GetTranslation("common_success"), result);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }
    }
}
