using Microsoft.Extensions.Logging;
using Share.Const;
using Share.DTO.Request.Srt;
using Share.DTO.Request.Tag;
using Share.DTO.Response.Tag;
using Share.Exceptions;
using Share.Models.LiveStraming;
using Share.Repositorys.Tag;
using Share.Services.Video;
using Share.Tool;
using Share.Tool.MySQL;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Share.Services.Tag
{
    /// <summary>
    /// 標籤服務實作 - 負責標籤的查詢、新增、編輯和 JSON 格式匯出
    /// </summary>
    public class TagService(
        ILiveStreamingTagRepository _liveStreamingTagRepository,
        ILiveStreamingTagTypeRepository _liveStreamingTagTypeRepository,
        IMySQLConnectionProvider _mySQLConnectionProvider,
        ICommonTool _commonTool,
        ILogger<LiveStreamingService> _logger
    ) : ITagService
    {
        /// <summary>
        /// 每頁顯示的標籤數量
        /// </summary>
        private const int pageSize = 25;
        
        /// <summary>
        /// JSON 序列化選項（不跳脫中文字元）
        /// </summary>
        private readonly static JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 關鍵設定
            WriteIndented = false // 可選：是否要縮排格式化
        };

        /// <summary>
        /// 搜尋標籤（支援標籤類型和關鍵字篩選）
        /// </summary>
        /// <param name="request">搜尋標籤請求</param>
        /// <returns>標籤列表、標籤類型列表和總頁數</returns>
        public GetAllTagResponse SearchTags(SearchTagRequest request)
        {
            try
            {
                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 根據類型和關鍵字取得標籤列表（頁碼從 1 開始，轉換為從 0 開始的索引）
                var tagList = _liveStreamingTagRepository.GetByTypeAndKeyword(
                    request.Type, 
                    request.Keyword, 
                    request.Page - 1, 
                    pageSize, 
                    connection);
                
                // 取得符合條件的總標籤數
                var totalCount = _liveStreamingTagRepository.GetCount(request.Type, request.Keyword, connection);
                
                // 取得所有標籤類型列表
                var tagTypeList = _liveStreamingTagTypeRepository.GetAll(connection);
                
                // 建立標籤類型字典（用於快速查找類型名稱）
                var tagTypeDict = tagTypeList.ToDictionary(item => item.lstt_type, item => item.lstt_name);
                
                // 建構回傳物件
                return new GetAllTagResponse
                {
                    TotalPage = _commonTool.GetTotalPage(totalCount, pageSize),
                    TagList = tagList.Select(item => new TagResponse
                    {
                        Id = item.lst_id,
                        TagName = item.lst_name,
                        TagType = tagTypeDict[item.lst_type],
                    }).ToList(),
                    TagTypeList = tagTypeList.Select(item => new TagTypeResponse
                    {
                        Id = item.lstt_type,
                        TypeName = item.lstt_name,
                    }).ToList(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "搜尋標籤時發生錯誤: {Keyword}", request.Keyword);
                throw;
            }
        }

        /// <summary>
        /// 取得所有標籤映射列表（格式化為 TSV 格式，方便貼到 Excel）
        /// </summary>
        /// <returns>標籤映射的換行分隔字串（URL + Tab + JSON 格式標籤）</returns>
        public string GetTagsMappingList()
        {
            try
            {
                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 取得影片與標籤的映射列表
                var tagList = _liveStreamingTagRepository.GetByTypeMapping(connection);
                
                // 將每筆資料格式化為「YouTube URL + Tab + JSON 標籤」，方便複製到 Excel
                return string.Join(
                    "\n",
                    tagList.Select(item =>
                        string.Concat(
                            "https://www.youtube.com/watch?v=", item.ls_url,
                            "\t",
                            FormatTagListToJson(item.tags)
                        )
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得標籤映射列表時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 取得指定類型的標籤名稱列表（換行分隔）
        /// </summary>
        /// <param name="tagType">標籤類型 ID</param>
        /// <returns>標籤名稱的換行分隔字串</returns>
        public string GetTagsList(int tagType)
        {
            try
            {
                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 取得指定類型的標籤列表
                var tagList = _liveStreamingTagRepository.GetByType(tagType, connection);
                
                // 將標籤名稱用換行符號串接
                return string.Join("\n", tagList.Select(item => item.lst_name));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得標籤列表時發生錯誤: {TagType}", tagType);
                throw;
            }
        }

        /// <summary>
        /// 取得所有標籤類型列表
        /// </summary>
        /// <returns>標籤類型列表</returns>
        public List<TagTypeResponse> GetTagType()
        {
            try
            {
                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 取得所有標籤類型
                var tagTypeList = _liveStreamingTagTypeRepository.GetAll(connection);

                // 轉換為回應格式
                return tagTypeList.Select(item => new TagTypeResponse
                {
                    Id = item.lstt_type,
                    TypeName = item.lstt_name,
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得標籤類型列表時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 取得指定標籤的詳細資料
        /// </summary>
        /// <param name="tagId">標籤 ID</param>
        /// <returns>標籤資料模型</returns>
        public LiveStreamingTagModel GetTagData(LstId tagId)
        {
            try
            {
                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 取得標籤資料
                var tagData = _liveStreamingTagRepository.GetById(tagId, connection);
                tagData.lst_id = tagId;
                
                return tagData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得標籤資料時發生錯誤: {TagId}", tagId);
                throw;
            }
        }

        /// <summary>
        /// 新增標籤（如果標籤已存在則返回現有標籤 ID）
        /// </summary>
        /// <param name="request">新增標籤請求</param>
        /// <returns>新增或已存在的標籤 ID</returns>
        /// <exception cref="MyException">當標籤類型不存在時拋出</exception>
        public uint InsertTag(AddTagRequest request)
        {
            try
            {
                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 驗證標籤類型是否存在
                var tagTypeModel = _liveStreamingTagTypeRepository.GetByType(request.TagType, connection);
                if (tagTypeModel.lstt_type.Value == 0)
                {
                    throw new MyException(ResponseCode.TAG_TYPE_NOT_EXIST);
                }
                
                // 檢查標籤是否已存在（避免重複新增）
                var tagModel = _liveStreamingTagRepository.GetByKeyword(request.TagName, connection);
                if (tagModel.lst_id.Value > 0)
                {
                    return tagModel.lst_id.Value;
                }
                
                // 新增標籤到資料庫
                var insertModel = new LiveStreamingTagModel
                {
                    lst_name = request.TagName,
                    lst_type = request.TagType,
                };
                
                // 使用交易確保資料一致性
                var trans = connection.BeginTransaction();
                var insertId = _liveStreamingTagRepository.Insert(connection, trans, insertModel);
                trans.Commit();
                return insertId;
            }
            catch (MyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增標籤時發生錯誤: {TagName}", request.TagName);
                throw;
            }
        }

        /// <summary>
        /// 更新標籤資料
        /// </summary>
        /// <param name="request">編輯標籤請求</param>
        /// <exception cref="MyException">當標籤類型不存在或標籤名稱已被使用時拋出</exception>
        public void UpdateTag(EditTagRequest request)
        {
            try
            {
                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 驗證標籤類型是否存在
                var tagTypeModel = _liveStreamingTagTypeRepository.GetByType(request.TagType, connection);
                if (tagTypeModel.lstt_type.Value == 0)
                {
                    throw new MyException(ResponseCode.TAG_TYPE_NOT_EXIST);
                }
                
                // 檢查新的標籤名稱是否已被其他標籤使用
                var tagModel = _liveStreamingTagRepository.GetByKeyword(request.TagName, connection);
                if (tagModel.lst_id.Value > 0)
                {
                    throw new MyException(ResponseCode.TAG_IS_EXIST);
                }
                
                // 更新標籤資料
                var updateModel = new LiveStreamingTagModel
                {
                    lst_id = request.TagId,
                    lst_name = request.TagName,
                    lst_type = request.TagType,
                };
                
                // 使用交易確保資料一致性
                var trans = connection.BeginTransaction();
                _liveStreamingTagRepository.Update(connection, trans, updateModel);
                trans.Commit();
            }
            catch (MyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新標籤時發生錯誤: {TagId}", request.TagId);
                throw;
            }
        }

        /// <summary>
        /// 將標籤列表格式化為易讀的 JSON 字串
        /// </summary>
        /// <param name="tags">JSON 格式的標籤字串</param>
        /// <returns>格式化後的 JSON 字串（冒號和逗號後加入空格）</returns>
        private string FormatTagListToJson(string tags)
        {
            // 先反序列化再序列化，確保格式一致
            var json = JsonSerializer.Serialize(
                JsonSerializer.Deserialize<List<TagDTO>>(tags),
                options
            );
            
            // 在冒號和逗號後加入空格，提升可讀性
            return json.Replace("\":", "\": ").Replace("\",\"", "\", \"");
        }
    }
}
