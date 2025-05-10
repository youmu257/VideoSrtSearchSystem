using MySqlConnector;
using Share.Tool.MySQL;
using Vogen;

namespace Share.Models.LiveStraming
{
    [ValueObject<uint>]
    public partial struct LstId;

    /// <summary>
    /// 影片標籤
    /// </summary>
    public class LiveStreamingTagModel : BaseModel
    {
        public static string TableName = "live_streaming_tag";

        public LstId lst_id { get; set; } = LstId.From(0);

        /// <summary>
        /// 標籤名稱
        /// </summary>
        public string lst_name { get; set; } = string.Empty;

        /// <summary>
        /// 標籤類型
        /// 1: 類型
        /// 2: 遊戲
        /// 3: 歌曲
        /// 4: 人員
        /// 5: 場次
        /// </summary>
        public LsttType lst_type { get; set; } = LsttType.From(0);

        public DateTime lst_createtime { get; set; }

        public LiveStreamingTagModel()
        {
        }

        public LiveStreamingTagModel(MySqlDataReader dr)
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
                    case nameof(lst_id):
                        lst_id = LstId.From(dr.GetUInt32(i));
                        break;
                    case nameof(lst_name):
                        lst_name = dr.GetString(i);
                        break;
                    case nameof(lst_type):
                        lst_type = LsttType.From(dr.GetUInt32(i));
                        break;
                    case nameof(lst_createtime):
                        lst_createtime = dr.GetDateTime(i);
                        break;
                }
            }
        }
    }
}
