using Microsoft.AspNetCore.Mvc;
using Share.DTO.Request;
using Share.DTO.Request.Srt;
using Share.DTO.Request.Tag;
using Share.Models;
using Share.Models.LiveStraming;
using Share.Services.Srt;
using Share.Services.Tag;
using Share.Services.Video;
using System.Diagnostics;

namespace VideoSrtSearchSystem.Controllers
{
    //[EnableCors("AllOpen")]
    [ApiController]
    [Route("")]
    public class HomeController(
        ILiveStreamingService _liveStreamingService,
        ISrtService _srtService,
        ITagService _tagService
    ) : Controller
    {
        [HttpGet]
        public IActionResult Home(string keyword = "", int page = 1)
        {
            return SearchVideo(keyword, page);
        }

        [HttpGet]
        [Route("home")]
        public IActionResult SearchVideo(string keyword = "", int page = 1)
        {
            var request = new SearchRequest
            {
                Keyword = keyword,
                Page = page,
            };
            var videoResponse = _liveStreamingService.GetAllVideo(request);
            ViewData["VideoList"] = videoResponse.VideoList;
            ViewData["TotalPage"] = videoResponse.TotalPage;
            return View("home");
        }


        [HttpGet]
        [Route("srtSearch")]
        public IActionResult SrtSearch(string? keyword, int page = 1, string start = "", string end = "")
        {
            if (string.IsNullOrEmpty(keyword) == false)
            {
                var response = _srtService.SearchSrtByMemory(new SearchSrtRequest
                {
                    Keyword = keyword,
                    Page = page,
                    Start = start,
                    End = end,
                });
                ViewData["VideoList"] = response.VideoList;
                ViewData["TotalPage"] = response.TotalPage;
            }
            ViewData["Keyword"] = keyword;
            ViewData["Page"] = page;
            ViewData["Start"] = start;
            ViewData["End"] = end;

            return View("srtSearch");
        }

        [HttpGet]
        [Route("tagSearch")]
        public IActionResult TagSearch(uint type = 0, string keyword = "", int page = 1)
        {
            var request = new SearchTagRequest
            {
                Type = LsttType.From(type),
                Keyword = keyword,
                Page = page,
            };
            var tagResponse = _tagService.SearchTags(request);
            ViewData["TagList"] = tagResponse.TagList;
            ViewData["TagTypeList"] = tagResponse.TagTypeList;
            ViewData["TotalPage"] = tagResponse.TotalPage;
            ViewData["Keyword"] = keyword;
            ViewData["TagType"] = type;
            ViewData["Page"] = page;
            return View("tag");
        }

        [HttpGet]
        [Route("addTag")]
        public IActionResult AddTag()
        {
            var tagTypeList = _tagService.GetTagType();
            ViewData["TagTypeList"] = tagTypeList;
            return View("addTag");
        }

        [HttpGet]
        [Route("insertTag")]
        public IActionResult InsertTag(uint type, string name)
        {
            var request = new AddTagRequest
            {
                TagType = LsttType.From(type),
                TagName = name,
            };
            _tagService.InsertTag(request);
            return TagSearch(type, name);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
