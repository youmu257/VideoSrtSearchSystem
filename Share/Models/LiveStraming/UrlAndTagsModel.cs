using MySqlConnector;
using Share.Tool.MySQL;

namespace Share.Models.LiveStraming
{

    public class UrlAndTagsModel : BaseModel
    {
        /// <summary>
        /// 影片網址
        /// </summary>
        public string ls_url { get; set; } = string.Empty;

        /// <summary>
        /// 標籤 JSON
        /// </summary>
        public string tags { get; set; } = string.Empty;

        public UrlAndTagsModel()
        {
        }

        public UrlAndTagsModel(MySqlDataReader dr)
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
                    case nameof(ls_url):
                        ls_url = dr.GetString(i);
                        break;
                    case nameof(tags):
                        tags = dr.GetString(i);
                        break;
                }
            }
        }
    }
}
