using MySqlConnector;
using Share.Tool.MySQL;

namespace Share.Models.LiveStraming
{
    public class LiveStreamingModel : BaseModel
    {
        public static string TableName = "livestreaming";

        public uint ls_id { get; set; }

        public string ls_guid { get; set; } = string.Empty;

        public string ls_title { get; set; } = string.Empty;

        public string ls_url { get; set; } = string.Empty;

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
                        ls_id = dr.GetUInt32(i);
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
                    case nameof(ls_createtime):
                        ls_createtime = dr.GetDateTime(i);
                        break;
                }
            }
        }
    }
}
