﻿using SubtitlesParser.Classes.Parsers;
using System.Text;
using VideoSrtSearchSystem.Config;
using VideoSrtSearchSystem.DTO.Request.Srt;
using VideoSrtSearchSystem.Exceptions;
using VideoSrtSearchSystem.Models.LiveStraming;
using VideoSrtSearchSystem.Repository.LiveStraming;
using VideoSrtSearchSystem.Repository.Srt;
using VideoSrtSearchSystem.Tool.MySQL;

namespace VideoSrtSearchSystem.Services.Srt
{
    public class SrtService(
        ILiveStreamingRepository _liveStreamingRepository,
        ILiveStreamingSrtRepository _liveStreamingSrtRepository,
        IMySQLConnectionProvider _mySQLConnectionProvider,
        ILogger<SrtService> _logger,
        IWebHostEnvironment appEnvironment
    ) : ISrtService
    {
        private readonly string outputDirectory = Path.Combine(appEnvironment.WebRootPath, "output");

        public void ImportSrt(ImportSrtRequest request)
        {
            try
            {
                if (!File.Exists(request.SrtPath))
                {
                    throw new MyException(ResponseCode.FILE_NOT_FOUND);
                }
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 檢查影片是否已存在
                var liveStramingModel = _liveStreamingRepository.GetByUrl(request.VideoUrl, connection);
                bool noVideo = string.IsNullOrEmpty(liveStramingModel.ls_guid);
                // 影片 Guid
                var videoGuid = noVideo ?
                    Guid.NewGuid().ToString() :
                    liveStramingModel.ls_guid;

                var parser = new SrtParser();
                using (var fileStream = File.OpenRead(request.SrtPath))
                {
                    var items = parser.ParseStream(fileStream, Encoding.UTF8);
                    uint index = 0;
                    var insertSrtList = items.Select(item => new LiveStreamingSrtModel
                    {
                        lss_ls_id = liveStramingModel.ls_id,
                        lss_num = index++,
                        lss_start = TimeSpan.FromMilliseconds(item.StartTime).ToString(@"hh\:mm\:ss\.fff"),
                        lss_end = TimeSpan.FromMilliseconds(item.EndTime).ToString(@"hh\:mm\:ss\.fff"),
                        lss_text = string.Join(" ", item.PlaintextLines),
                    }).ToList();
                    var trans = connection.BeginTransaction();
                    try
                    {
                        if (noVideo)
                        {
                            // 寫入影片資訊
                            liveStramingModel.ls_id = _liveStreamingRepository.Insert(connection, trans, new LiveStreamingModel
                            {
                                ls_guid = videoGuid,
                                ls_title = request.VideoTitle,
                                ls_url = request.VideoUrl,
                            });
                            insertSrtList.ForEach(item => item.lss_ls_id = liveStramingModel.ls_id);
                        }
                        // 寫入字幕資訊
                        _liveStreamingSrtRepository.Insert(connection, trans, insertSrtList);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                        trans.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}