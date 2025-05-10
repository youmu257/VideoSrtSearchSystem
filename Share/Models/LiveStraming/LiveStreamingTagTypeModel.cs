using MySqlConnector;
using Share.Tool.MySQL;
using Vogen;

namespace Share.Models.LiveStraming
{
    [ValueObject<uint>]
    public partial struct LsttId;

    /// <summary>
    /// 影片標籤
    /// </summary>
    public class LiveStreamingTagTypeModel : BaseModel
    {
        public static string TableName = "live_streaming_tag_type";

        public LsttId lstt_type { get; set; } = LsttId.From(0);

        /// <summary>
        /// 標籤名稱
        /// </summary>
        public string lstt_name { get; set; } = string.Empty;

        public DateTime lstt_createtime { get; set; }

        public LiveStreamingTagTypeModel()
        {
        }

        public LiveStreamingTagTypeModel(MySqlDataReader dr)
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
                    case nameof(lstt_type):
                        lstt_type = LsttId.From(dr.GetUInt32(i));
                        break;
                    case nameof(lstt_name):
                        lstt_name = dr.GetString(i);
                        break;
                    case nameof(lstt_createtime):
                        lstt_createtime = dr.GetDateTime(i);
                        break;
                }
            }
        }
    }
}
