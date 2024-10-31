using MySqlConnector;
using Share.Tool.MySQL;
using Vogen;

namespace Share.Models.LiveStraming
{
    [ValueObject<uint>]
    public partial struct LssId;

    public class LiveStreamingSrtModel : BaseModel
    {
        public static string TableName = "live_streaming_srt";

        public LssId lss_id { get; set; } = LssId.From(0);

        public LsId lss_ls_id { get; set; } = LsId.From(0);

        public uint lss_num { get; set; }

        public string lss_start { get; set; } = string.Empty;

        public string lss_end { get; set; } = string.Empty;

        public string lss_text { get; set; } = string.Empty;

        public DateTime lss_createtime { get; set; }

        public LiveStreamingSrtModel()
        {
        }

        public LiveStreamingSrtModel(MySqlDataReader dr)
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
                    case nameof(lss_id):
                        lss_id = LssId.From(dr.GetUInt32(i));
                        break;
                    case nameof(lss_ls_id):
                        lss_ls_id = LsId.From(dr.GetUInt32(i));
                        break;
                    case nameof(lss_num):
                        lss_num = dr.GetUInt32(i);
                        break;
                    case nameof(lss_start):
                        lss_start = dr.GetString(i);
                        break;
                    case nameof(lss_end):
                        lss_end = dr.GetString(i);
                        break;
                    case nameof(lss_text):
                        lss_text = dr.GetString(i);
                        break;
                    case nameof(lss_createtime):
                        lss_createtime = dr.GetDateTime(i);
                        break;
                }
            }
        }
    }
}
