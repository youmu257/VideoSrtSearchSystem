using MySqlConnector;
using Share.Tool.MySQL;
using Vogen;

namespace Share.Models.LiveStraming
{
    [ValueObject<uint>]
    public partial struct LstmId;

    /// <summary>
    /// 影片標籤
    /// </summary>
    public class LiveStreamingTagMappingModel : BaseModel
    {
        public static string TableName = "live_streaming_tag_mapping";

        public LstmId lstm_id { get; set; } = LstmId.From(0);

        /// <summary>
        /// 標籤類型
        /// </summary>
        public LsId lstm_ls_id { get; set; } = LsId.From(0);

        /// <summary>
        /// 標籤類型
        /// </summary>
        public LstId lstm_lst_id { get; set; } = LstId.From(0);

        public DateTime lstm_createtime { get; set; }

        public LiveStreamingTagMappingModel()
        {
        }

        public LiveStreamingTagMappingModel(MySqlDataReader dr)
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
                    case nameof(lstm_id):
                        lstm_id = LstmId.From(dr.GetUInt32(i));
                        break;
                    case nameof(lstm_ls_id):
                        lstm_ls_id = LsId.From(dr.GetUInt32(i));
                        break;
                    case nameof(lstm_lst_id):
                        lstm_lst_id = LstId.From(dr.GetUInt32(i));
                        break;
                    case nameof(lstm_createtime):
                        lstm_createtime = dr.GetDateTime(i);
                        break;
                }
            }
        }
    }
}
