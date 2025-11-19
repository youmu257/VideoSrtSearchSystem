using Microsoft.AspNetCore.Mvc;
using Share.Const;
using Share.DTO.Request.Tag;
using Share.Services.Tag;
using Share.Tool.Language;

namespace VideoSrtSearchSystem.Controllers
{
    /// <summary>
    /// 標籤控制器 - 負責處理影片標籤的查詢、新增、編輯和列表功能
    /// </summary>
    /// <param name="_tagService">標籤服務介面</param>
    /// <param name="_logger">日誌記錄器</param>
    [ApiController]
    [Route("api/tag")]
    public class TagController(
        ITagService _tagService,
        ILogger<TagController> _logger
    ) : BaseController<TagController>(_logger)
    {
        /// <summary>
        /// 搜尋標籤 - 根據類型、關鍵字和頁碼搜尋標籤
        /// </summary>
        /// <param name="request">搜尋請求，包含標籤類型、關鍵字和頁碼</param>
        /// <returns>符合條件的標籤列表</returns>
        [HttpGet]
        [Route("search")]
        public IActionResult SearchTag(SearchTagRequest request)
        {
            try
            {
                // 驗證頁碼參數（必須大於 0）
                if (request.Page <= 0)
                {
                    return ParameterFormatError("Page");
                }
                
                // 驗證標籤類型參數（必須大於 0）
                if (request.Type.Value <= 0)
                {
                    return ParameterFormatError("Type");
                }

                // 呼叫服務搜尋標籤
                var result = _tagService.SearchTags(request);
                
                return Ok(ResponseCode.SUCCESS, LangTool.GetTranslation("common_success"), result);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        /// <summary>
        /// 新增標籤
        /// </summary>
        /// <param name="request">新增標籤請求，包含標籤名稱和類型</param>
        /// <returns>新增結果</returns>
        [HttpPost]
        [Route("add")]
        public IActionResult AddTag(AddTagRequest request)
        {
            try
            {
                // 驗證必填欄位 - 標籤名稱
                if (string.IsNullOrWhiteSpace(request.TagName))
                {
                    return ParameterIsRequired("TagName");
                }
                
                // 驗證必填欄位 - 標籤類型（必須大於 0）
                if (request.TagType.Value <= 0)
                {
                    return ParameterIsRequired("TagType");
                }

                // 呼叫服務新增標籤
                _tagService.InsertTag(request);
                
                return Ok(ResponseCode.SUCCESS, LangTool.GetTranslation("common_success"));
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        /// <summary>
        /// 編輯標籤
        /// </summary>
        /// <param name="request">編輯標籤請求，包含標籤 ID、名稱和類型</param>
        /// <returns>更新結果</returns>
        [HttpPost]
        [Route("edit")]
        public IActionResult EditTag(EditTagRequest request)
        {
            try
            {
                // 驗證必填欄位 - 標籤 ID（必須大於 0）
                if (request.TagId.Value <= 0)
                {
                    return ParameterIsRequired("TagId");
                }
                
                // 驗證必填欄位 - 標籤名稱
                if (string.IsNullOrWhiteSpace(request.TagName))
                {
                    return ParameterIsRequired("TagName");
                }
                
                // 驗證必填欄位 - 標籤類型（必須大於 0）
                if (request.TagType.Value <= 0)
                {
                    return ParameterIsRequired("TagType");
                }

                // 呼叫服務更新標籤
                _tagService.UpdateTag(request);
                
                return Ok(ResponseCode.SUCCESS, LangTool.GetTranslation("common_success"));
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        /// <summary>
        /// 取得標籤對應關係列表 - 返回影片與標籤的對應關係 JSON
        /// </summary>
        /// <returns>JSON 格式的標籤對應關係資料</returns>
        /// <remarks>用於查詢哪些影片具有哪些標籤</remarks>
        [HttpGet]
        [Route("list/mapping")]
        public IActionResult GetTagJsonList()
        {
            try
            {
                // 取得所有標籤對應關係資料
                var result = _tagService.GetTagsMappingList();
                
                // 返回純文字 JSON 格式
                return OkText(result);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        /// <summary>
        /// 取得指定類型的標籤列表 - 返回 JSON 格式的標籤資料
        /// </summary>
        /// <param name="type">標籤類型 ID，0 表示所有類型</param>
        /// <returns>JSON 格式的標籤列表</returns>
        [HttpGet]
        [Route("list")]
        public IActionResult GetTagList(int type)
        {
            try
            {
                // 根據類型取得標籤列表
                var result = _tagService.GetTagsList(type);
                
                // 返回純文字 JSON 格式
                return OkText(result);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }
    }
}
