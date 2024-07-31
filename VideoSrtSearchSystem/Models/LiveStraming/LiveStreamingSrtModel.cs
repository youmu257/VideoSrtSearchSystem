using MySqlConnector;
using VideoSrtSearchSystem.Tool.MySQL;

namespace VideoSrtSearchSystem.Models.LiveStraming
{
    public class LiveStreamingSrtModel : BaseModel
    {
        public static string TableName = "livestreamingsrt";

        public uint lss_id { get; set; }

        public uint lss_ls_id { get; set; }

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
                        lss_id = dr.GetUInt32(i);
                        break;
                    case nameof(lss_ls_id):
                        lss_ls_id = dr.GetUInt32(i);
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
