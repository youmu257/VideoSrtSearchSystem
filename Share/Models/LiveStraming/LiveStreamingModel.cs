using MySqlConnector;
using Share.Tool.MySQL;
using Vogen;

namespace Share.Models.LiveStraming
{
    [ValueObject<uint>]
    public partial struct LsId;

    public class LiveStreamingModel : BaseModel
    {
        public static string TableName = "live_streaming";

        public LsId ls_id { get; set; } = LsId.From(0);

        /// <summary>
        /// 影片 guid
        /// </summary>
        public string ls_guid { get; set; } = string.Empty;

        /// <summary>
        /// 影片標題
        /// </summary>
        public string ls_title { get; set; } = string.Empty;

        /// <summary>
        /// 影片網址
        /// </summary>
        public string ls_url { get; set; } = string.Empty;

        /// <summary>
        /// 直播時間
        /// </summary>
        public DateTime ls_livetime { get; set; }

        public DateTime ls_createtime { get; set; }

        public LiveStreamingModel()
        {
        }

        public LiveStreamingModel(MySqlDataReader dr)
        {
            Set(dr);
        }

        public override void Set(MySqlDataReader dr)
        {
            for (var i = 0; i < dr.FieldCount; i++)
            {
                if (dr.IsDBNull(i))//null值不做事
                    continue;
                switch (dr.GetName(i))
                {
                    case nameof(ls_id):
                        ls_id = LsId.From(dr.GetUInt32(i));
                        break;
                    case nameof(ls_guid):
                        ls_guid = dr.GetString(i);
                        break;
                    case nameof(ls_title):
                        ls_title = dr.GetString(i);
                        break;
                    case nameof(ls_url):
                        ls_url = dr.GetString(i);
                        break;
                    case nameof(ls_livetime):
                        ls_livetime = dr.GetDateTime(i);
                        break;
                    case nameof(ls_createtime):
                        ls_createtime = dr.GetDateTime(i);
                        break;
                }
            }
        }
    }
}
