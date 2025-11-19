using FluentMigrator;
using Share.Models.LiveStraming;

namespace Share.Migrations
{
    [Migration(20250531000004)]
    public class CreateLiveStreamingTagTable : Migration
    {
        public override void Up()
        {
            Create.Table(LiveStreamingTagModel.TableName)
                .WithColumn(nameof(LiveStreamingTagModel.lst_id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(LiveStreamingTagModel.lst_name)).AsCustom("VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci").NotNullable()
                .WithColumn(nameof(LiveStreamingTagModel.lst_type)).AsInt32().NotNullable()
                .WithColumn(nameof(LiveStreamingTagModel.lst_createtime)).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            // 建立索引以提升查詢效能
            Create.Index($"index_{LiveStreamingTagModel.TableName}_{nameof(LiveStreamingTagModel.lst_type)}")
                .OnTable(LiveStreamingTagModel.TableName)
                .OnColumn(nameof(LiveStreamingTagModel.lst_type));
        }

        public override void Down()
        {
            // 刪除索引
            Delete.Index($"index_{LiveStreamingTagModel.TableName}_{nameof(LiveStreamingTagModel.lst_type)}")
                .OnTable(LiveStreamingTagModel.TableName);

            // 刪除資料表
            Delete.Table(LiveStreamingTagModel.TableName);
        }
    }
}
