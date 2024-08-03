using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoSrtSearchSystem.DTO.Request.Video;
using VideoSrtSearchSystem.DTO.Response.Video;
using VideoSrtSearchSystem.Models;
using VideoSrtSearchSystem.Services.Video;

namespace VideoSrtSearchSystem.Controllers
{
    //[EnableCors("AllOpen")]
    [ApiController]
    [Route("")]
    public class HomeController(
        IVideoService _videoService,
        ILogger<HomeController> _logger
    ) : Controller
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
