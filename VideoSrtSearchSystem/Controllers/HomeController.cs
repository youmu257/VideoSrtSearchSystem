using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoSrtSearchSystem.DTO.Request.Video;
using VideoSrtSearchSystem.DTO.Response.Video;
using VideoSrtSearchSystem.Models;
using VideoSrtSearchSystem.Services.Srt;
using VideoSrtSearchSystem.Services.Video;

namespace VideoSrtSearchSystem.Controllers
{
    //[EnableCors("AllOpen")]
    [ApiController]
    [Route("")]
    public class HomeController(IVideoService _videoService, ISrtService _srtService) : Controller
    {
        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            var request = new GetAllVideoRequest
            {
                Page = page,
            };
            var videoResponse = _videoService.GetAllVideo(request);
            ViewData["VideoList"] = videoResponse.VideoList;
            ViewData["TotalPage"] = videoResponse.TotalPage;
            return View();
        }


        [HttpGet]
        [Route("srtSearch")]
        public IActionResult SrtSearch(string? keyword, int page = 1)
        {
            if (string.IsNullOrEmpty(keyword) == false)
            {
                var response = _srtService.SearchSrt(keyword, page);
                ViewData["VideoList"] = response.VideoList;
                ViewData["TotalPage"] = response.TotalPage;
            }
            ViewData["Keyword"] = keyword;
            ViewData["Page"] = page;

            return View("srtSearch");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
