using Microsoft.AspNetCore.Mvc;
using Share.DTO.Request;
using Share.DTO.Request.Srt;
using Share.DTO.Request.Tag;
using Share.DTO.Request.Video;
using Share.DTO.Response.Video;
using Share.Models;
using Share.Models.LiveStraming;
using Share.Services.Srt;
using Share.Services.Tag;
using Share.Services.Video;
using System.Diagnostics;

namespace VideoSrtSearchSystem.Controllers
{
    /// <summary>
    /// 主要控制器 - 負責處理影片、字幕和標籤的查詢、編輯和管理功能
    /// </summary>
    /// <param name="_liveStreamingService">影片服務介面</param>
    /// <param name="_srtService">字幕服務介面</param>
    /// <param name="_tagService">標籤服務介面</param>
    [ApiController]
    [Route("")]
    public class HomeController(
        ILiveStreamingService _liveStreamingService,
        ISrtService _srtService,
        ITagService _tagService
    ) : Controller
    {
        /// <summary>
        /// 首頁 - 顯示影片列表頁面
        /// </summary>
        /// <param name="keyword">搜尋關鍵字</param>
        /// <param name="page">當前頁碼</param>
        /// <returns>影片列表視圖</returns>
        [HttpGet]
        public IActionResult Home(string keyword = "", int page = 1)
        {
            return SearchVideo(keyword, page);
        }

        /// <summary>
        /// 搜尋影片 - 根據關鍵字搜尋影片列表
        /// </summary>
        /// <param name="keyword">搜尋關鍵字</param>
        /// <param name="page">頁碼，預設為 1</param>
        /// <returns>影片列表視圖</returns>
        [HttpGet]
        [Route("home")]
        public IActionResult SearchVideo(string keyword = "", int page = 1)
        {
            // 建立搜尋請求物件
            var request = new SearchRequest
            {
                Keyword = keyword,
                Page = page,
            };
            
            // 呼叫服務取得影片列表
            var videoResponse = _liveStreamingService.GetAllVideo(request);
            
            // 傳遞資料到視圖
            ViewData["VideoList"] = videoResponse.VideoList;
            ViewData["TotalPage"] = videoResponse.TotalPage;
            return View("home");
        }

        /// <summary>
        /// 顯示影片編輯頁面
        /// </summary>
        /// <param name="guid">影片 GUID</param>
        /// <returns>影片編輯視圖</returns>
        [HttpGet]
        [Route("edit")]
        public IActionResult Edit(string guid)
        {
            // 驗證影片 GUID
            if (string.IsNullOrWhiteSpace(guid))
            {
                return View();
            }
            
            // 取得影片詳細資訊
            var videoInfo = _liveStreamingService.GetOneVideoInfo(guid);
            ViewData["Video"] = videoInfo;
            
            return View("edit");
        }

        /// <summary>
        /// 更新影片資訊 - 儲存影片標題和標籤資訊
        /// </summary>
        /// <param name="guid">影片 GUID</param>
        /// <param name="title">影片標題</param>
        /// <param name="tagJson">標籤列表 JSON 字串</param>
        /// <returns>重導向到編輯頁面</returns>
        [HttpGet]
        [Route("editVideo")]
        public IActionResult EditVideo(string guid, string title, string tagJson)
        {
            // 驗證影片 GUID
            if (string.IsNullOrWhiteSpace(guid))
            {
                return View();
            }
            
            // 建立更新請求物件
            var request = new EditVideoRequest
            {
                VideoGuid = guid,
                VideoTitle = title,
                TagList = string.IsNullOrWhiteSpace(tagJson) 
                    ? new List<TagEditDTO>() 
                    : System.Text.Json.JsonSerializer.Deserialize<List<TagEditDTO>>(tagJson) ?? new List<TagEditDTO>(),
            };
            
            // 呼叫服務更新影片資訊
            _liveStreamingService.UpdateVideoInfo(request);
            var videoInfo = _liveStreamingService.GetOneVideoInfo(guid);
            ViewData["Video"] = videoInfo;
            return Edit(guid);
        }

        /// <summary>
        /// 字幕搜尋 - 根據關鍵字和日期範圍搜尋字幕內容
        /// </summary>
        /// <param name="keyword">搜尋關鍵字</param>
        /// <param name="page">頁碼，預設為 1</param>
        /// <param name="start">開始日期</param>
        /// <param name="end">結束日期</param>
        /// <returns>字幕搜尋結果視圖</returns>
        [HttpGet]
        [Route("srtSearch")]
        public IActionResult SrtSearch(string? keyword, int page = 1, string start = "", string end = "")
        {
            // 僅當有搜尋關鍵字時才執行搜尋
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                // 建立搜尋請求物件
                var response = _srtService.SearchSrtByMemory(new SearchSrtRequest
                {
                    Keyword = keyword,
                    Page = page,
                    Start = start,
                    End = end,
                });
                
                // 傳遞搜尋結果到視圖
                ViewData["VideoList"] = response.VideoList;
                ViewData["TotalPage"] = response.TotalPage;
            }
            
            // 傳遞搜尋參數到視圖（用於顯示搜尋條件）
            ViewData["Keyword"] = keyword;
            ViewData["Page"] = page;
            ViewData["Start"] = start;
            ViewData["End"] = end;

            return View("srtSearch");
        }

        /// <summary>
        /// 標籤搜尋 - 根據類型和關鍵字搜尋標籤
        /// </summary>
        /// <param name="type">標籤類型 ID，0 表示所有類型</param>
        /// <param name="keyword">搜尋關鍵字</param>
        /// <param name="page">頁碼，預設為 1</param>
        /// <returns>標籤搜尋結果視圖</returns>
        [HttpGet]
        [Route("tagSearch")]
        public IActionResult TagSearch(uint type = 0, string keyword = "", int page = 1)
        {
            // 建立搜尋請求物件
            var request = new SearchTagRequest
            {
                Type = LsttType.From(type),
                Keyword = keyword,
                Page = page,
            };
            
            // 呼叫服務搜尋標籤
            var tagResponse = _tagService.SearchTags(request);
            
            // 傳遞資料到視圖
            ViewData["TagList"] = tagResponse.TagList;
            ViewData["TagTypeList"] = tagResponse.TagTypeList;
            ViewData["TotalPage"] = tagResponse.TotalPage;
            ViewData["Keyword"] = keyword;
            ViewData["TagType"] = type;
            ViewData["Page"] = page;
            
            return View("tag");
        }

        /// <summary>
        /// 顯示新增標籤頁面
        /// </summary>
        /// <returns>新增標籤視圖</returns>
        [HttpGet]
        [Route("addTag")]
        public IActionResult AddTag()
        {
            // 取得標籤類型列表
            var tagTypeList = _tagService.GetTagType();
            ViewData["TagTypeList"] = tagTypeList;
            
            return View("addTag");
        }

        /// <summary>
        /// 顯示標籤編輯頁面
        /// </summary>
        /// <param name="id">標籤 ID</param>
        /// <returns>標籤編輯視圖</returns>
        [HttpGet]
        [Route("editTag")]
        public IActionResult EditTag(LstId id)
        {
            // 取得標籤資料和類型列表
            var tagData = _tagService.GetTagData(id);
            var tagTypeList = _tagService.GetTagType();
            
            ViewData["TagTypeList"] = tagTypeList;
            ViewData["TagData"] = tagData;
            
            return View("editTag");
        }

        /// <summary>
        /// 更新標籤資訊
        /// </summary>
        /// <param name="id">標籤 ID</param>
        /// <param name="type">標籤類型 ID</param>
        /// <param name="name">標籤名稱</param>
        /// <returns>重導向到編輯頁面</returns>
        [HttpGet]
        [Route("updateTag")]
        public IActionResult UpdateTag(LstId id, uint type, string name)
        {
            // 建立更新請求物件
            _tagService.UpdateTag(new EditTagRequest
            {
                TagId = id,
                TagType = LsttType.From(type),
                TagName = name,
            });
            
            // 重導向到編輯頁面顯示更新後的資料
            return EditTag(id);
        }

        /// <summary>
        /// 新增標籤
        /// </summary>
        /// <param name="type">標籤類型 ID</param>
        /// <param name="name">標籤名稱</param>
        /// <returns>重導向到標籤搜尋頁面</returns>
        [HttpGet]
        [Route("insertTag")]
        public IActionResult InsertTag(uint type, string name)
        {
            // 建立新增請求物件
            var request = new AddTagRequest
            {
                TagType = LsttType.From(type),
                TagName = name,
            };
            
            // 呼叫服務新增標籤
            _tagService.InsertTag(request);
            
            // 重導向到搜尋頁面顯示新增的標籤
            return TagSearch(type, name);
        }

        /// <summary>
        /// 錯誤處理頁面
        /// </summary>
        /// <returns>錯誤視圖</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
    }
}
