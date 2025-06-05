using FluentMigrator;
using Share.Models.LiveStraming;

namespace Share.Migrations
{
    [Migration(20250531000001)]
    public class CreateLiveStreamingTable : FluentMigrator.Migration
    {
        public override void Up()
        {
            Create.Table(LiveStreamingModel.TableName)
                .WithColumn(nameof(LiveStreamingModel.ls_id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(LiveStreamingModel.ls_guid)).AsString(36).NotNullable().Unique()
                .WithColumn(nameof(LiveStreamingModel.ls_title)).AsCustom("VARCHAR(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci").NotNullable()
                .WithColumn(nameof(LiveStreamingModel.ls_url)).AsString(1024).NotNullable()
                .WithColumn(nameof(LiveStreamingModel.ls_all_srt)).AsCustom("LONGTEXT").Nullable()
                .WithColumn(nameof(LiveStreamingModel.ls_livetime)).AsDateTime().NotNullable()
                .WithColumn(nameof(LiveStreamingModel.ls_createtime)).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            // 添加索引以提高查詢效能
            Create.Index($"index_{LiveStreamingModel.TableName}_{nameof(LiveStreamingModel.ls_guid)}")
                .OnTable(LiveStreamingModel.TableName)
                .OnColumn(nameof(LiveStreamingModel.ls_guid));

            Create.Index($"index_{LiveStreamingModel.TableName}_{nameof(LiveStreamingModel.ls_livetime)}")
                .OnTable(LiveStreamingModel.TableName)
                .OnColumn(nameof(LiveStreamingModel.ls_livetime));
        }

        public override void Down()
        {
            // 移除索引
            Delete.Index($"index_{LiveStreamingModel.TableName}_{nameof(LiveStreamingModel.ls_guid)}")
                .OnTable(LiveStreamingModel.TableName);

            Delete.Index($"index_{LiveStreamingModel.TableName}_{nameof(LiveStreamingModel.ls_livetime)}")
                .OnTable(LiveStreamingModel.TableName);

            // 移除資料表
            Delete.Table(LiveStreamingModel.TableName);
        }
    }
}
