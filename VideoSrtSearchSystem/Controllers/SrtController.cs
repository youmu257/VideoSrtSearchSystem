﻿using Microsoft.AspNetCore.Mvc;
using Share.Const;
using Share.DTO.Request.Srt;
using Share.Exceptions;
using Share.Services.Srt;
using Share.Tool.Language;
using System.Web;

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
                if (string.IsNullOrEmpty(request.VideoTitle))
                {
                    return ParameterIsRequired("VideoTitle");
                }
                if (string.IsNullOrEmpty(request.VideoUrl))
                {
                    return ParameterIsRequired("VideoUrl");
                }

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
        /// 匯入字幕
        /// </summary>
        [HttpPost]
        [Route("import/list")]
        public IActionResult ImportSrtList(List<ImportSrtRequest> requests)
        {
            try
            {
                foreach (var request in requests)
                {
                    _srtService.ImportSrt(request);
                }
                return Ok(ResponseCode.SUCCESS, LangTool.GetTranslation("common_success"));
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        /// <summary>
        /// 查詢字幕功能 全部從DB
        /// </summary>
        [HttpGet]
        [Route("searchFromDb")]
        public IActionResult SearchSrtFromDb([FromQuery] SearchSrtRequest request)
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
        /// 查詢字幕功能 從記憶體
        /// </summary>
        [HttpGet]
        [Route("search")]
        public IActionResult SearchSrtFromMemory([FromQuery] SearchSrtRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Keyword))
                {
                    return ParameterIsRequired("Keyword");
                }

                var response = _srtService.SearchSrtByMemory(request.Keyword, request.Page);
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
