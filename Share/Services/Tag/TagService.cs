using Microsoft.Extensions.Logging;
using Share.Const;
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
    public class TagService(
        ILiveStreamingTagRepository _liveStreamingTagRepository,
        ILiveStreamingTagTypeRepository _liveStreamingTagTypeRepository,
        IMySQLConnectionProvider _mySQLConnectionProvider,
        ICommonTool _commonTool,
        ILogger<LiveStreamingService> _logger
    ) : ITagService
    {
        private const int pageSize = 25;
        private readonly static JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 關鍵設定
            WriteIndented = false // 可選：是否要縮排格式化
        };

        public GetAllTagResponse SearchTags(SearchTagRequest request)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 取得標籤列表
                var tagList = _liveStreamingTagRepository.GetByTypeAndKeyword(request.Type, request.Keyword, request.Page - 1, pageSize, connection);
                var totalCount = _liveStreamingTagRepository.GetCount(request.Type, request.Keyword, connection);
                // 取得標邊類型列表
                var tagTypeList = _liveStreamingTagTypeRepository.GetAll(connection);
                var tagTypeDict = tagTypeList.ToDictionary(item => item.lstt_type, item => item.lstt_name);
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
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public string GetTagsMappingList()
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 取得標籤列表
                var tagList = _liveStreamingTagRepository.GetByTypeMapping(connection);
                // 把字串每筆換行間隔
                return string.Join(
                    "\n",
                    tagList.Select(item =>
                        // 把資料先串成 tab 分隔的字串，方便貼到 excel
                        string.Concat(
                            string.Concat("https://www.youtube.com/watch?v=", item.ls_url),
                            "\t",
                            JsonSerializer.Serialize(
                                JsonSerializer.Deserialize<List<AddTagRequest>>(item.tags),
                                options
                            )
                        )
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public List<TagTypeResponse> GetTagType()
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 取得標邊類型列表
                var tagTypeList = _liveStreamingTagTypeRepository.GetAll(connection);

                return tagTypeList.Select(item => new TagTypeResponse
                {
                    Id = item.lstt_type,
                    TypeName = item.lstt_name,
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public LiveStreamingTagModel GetTagData(LstId tagId)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 取得標籤料
                var tagData = _liveStreamingTagRepository.GetById(tagId, connection);
                tagData.lst_id = tagId;
                return tagData;
                ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }


        public uint InsertTag(AddTagRequest request)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 檢查標籤類型
                var tagTypeModel = _liveStreamingTagTypeRepository.GetByType(request.TagType, connection);
                if (tagTypeModel.lstt_type.Value == 0)
                {
                    throw new MyException(ResponseCode.TAG_TYPE_NOT_EXIST);
                }
                // 檢查標籤是否已存在
                var tagModel = _liveStreamingTagRepository.GetByKeyword(request.TagName, connection);
                if (tagModel.lst_id.Value > 0)
                {
                    return tagModel.lst_id.Value;
                }
                // 新增標籤
                var insertModel = new LiveStreamingTagModel
                {
                    lst_name = request.TagName,
                    lst_type = request.TagType,
                };
                uint insertId = 0;
                var trans = connection.BeginTransaction();
                insertId = _liveStreamingTagRepository.Insert(connection, trans, insertModel);
                trans.Commit();
                return insertId;
            }
            catch (MyException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public void UpdateTag(EditTagRequest request)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 檢查標籤類型
                var tagTypeModel = _liveStreamingTagTypeRepository.GetByType(request.TagType, connection);
                if (tagTypeModel.lstt_type.Value == 0)
                {
                    throw new MyException(ResponseCode.TAG_TYPE_NOT_EXIST);
                }
                // 檢查標籤是否已存在
                var tagModel = _liveStreamingTagRepository.GetByKeyword(request.TagName, connection);
                if (tagModel.lst_id.Value > 0)
                {
                    throw new MyException(ResponseCode.TAG_IS_EXIST);
                }
                // 編輯標籤
                var updateModel = new LiveStreamingTagModel
                {
                    lst_id = request.TagId,
                    lst_name = request.TagName,
                    lst_type = request.TagType,
                };
                var trans = connection.BeginTransaction();
                _liveStreamingTagRepository.Update(connection, trans, updateModel);
                trans.Commit();
            }
            catch (MyException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
