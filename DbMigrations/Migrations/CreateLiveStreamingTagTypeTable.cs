using FluentMigrator;
using Share.Models.LiveStraming;

namespace Share.Migrations
{
    [Migration(20250531000005)]
    public class CreateLiveStreamingTagTypeTable : Migration
    {
        public override void Up()
        {
            Create.Table(LiveStreamingTagTypeModel.TableName)
                .WithColumn(nameof(LiveStreamingTagTypeModel.lstt_type)).AsInt32().PrimaryKey()
                .WithColumn(nameof(LiveStreamingTagTypeModel.lstt_name)).AsString(255).NotNullable()
                .WithColumn(nameof(LiveStreamingTagTypeModel.lstt_createtime)).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            // 建立索引以提升查詢效能
            Create.Index($"index_{LiveStreamingTagTypeModel.TableName}_{nameof(LiveStreamingTagTypeModel.lstt_name)}")
                .OnTable(LiveStreamingTagTypeModel.TableName)
                .OnColumn(nameof(LiveStreamingTagTypeModel.lstt_name));

            // 寫入初始資料
            Insert.IntoTable(LiveStreamingTagTypeModel.TableName).Row(new
            {
                lstt_type = 1,
                lstt_name = "類型",
                lstt_createtime = SystemMethods.CurrentDateTime
            });
            Insert.IntoTable(LiveStreamingTagTypeModel.TableName).Row(new
            {
                lstt_type = 2,
                lstt_name = "遊戲",
                lstt_createtime = SystemMethods.CurrentDateTime
            });
            Insert.IntoTable(LiveStreamingTagTypeModel.TableName).Row(new
            {
                lstt_type = 3,
                lstt_name = "歌曲",
                lstt_createtime = SystemMethods.CurrentDateTime
            });
            Insert.IntoTable(LiveStreamingTagTypeModel.TableName).Row(new
            {
                lstt_type = 4,
                lstt_name = "人員",
                lstt_createtime = SystemMethods.CurrentDateTime
            });
            Insert.IntoTable(LiveStreamingTagTypeModel.TableName).Row(new
            {
                lstt_type = 5,
                lstt_name = "場次",
                lstt_createtime = SystemMethods.CurrentDateTime
            });
        }

        public override void Down()
        {
            // 刪除索引
            Delete.Index($"index_{LiveStreamingTagTypeModel.TableName}_{nameof(LiveStreamingTagTypeModel.lstt_name)}")
                .OnTable(LiveStreamingTagTypeModel.TableName);

            // 刪除資料表
            Delete.Table(LiveStreamingTagTypeModel.TableName);
        }
    }
}
