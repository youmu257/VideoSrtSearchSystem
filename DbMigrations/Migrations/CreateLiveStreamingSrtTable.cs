using FluentMigrator;
using Share.Models.LiveStraming;

namespace Share.Migrations
{
    [Migration(20250531000002)]
    public class CreateLiveStreamingSrtTable : Migration
    {
        public override void Up()
        {
            Create.Table(LiveStreamingSrtModel.TableName)
                .WithColumn(nameof(LiveStreamingSrtModel.lss_id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(LiveStreamingSrtModel.lss_ls_id)).AsInt32().NotNullable()
                .WithColumn(nameof(LiveStreamingSrtModel.lss_num)).AsInt32().NotNullable()
                .WithColumn(nameof(LiveStreamingSrtModel.lss_start)).AsCustom("VARCHAR(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci").NotNullable()
                .WithColumn(nameof(LiveStreamingSrtModel.lss_end)).AsCustom("VARCHAR(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci").NotNullable()
                .WithColumn(nameof(LiveStreamingSrtModel.lss_text)).AsCustom("VARCHAR(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci").NotNullable()
                .WithColumn(nameof(LiveStreamingSrtModel.lss_createtime)).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            // 建立索引以提升查詢效能
            Create.Index($"index_{LiveStreamingSrtModel.TableName}_{nameof(LiveStreamingSrtModel.lss_ls_id)}")
                .OnTable(LiveStreamingSrtModel.TableName)
                .OnColumn(nameof(LiveStreamingSrtModel.lss_ls_id));

            Create.Index($"index_{LiveStreamingSrtModel.TableName}_{nameof(LiveStreamingSrtModel.lss_num)}")
                .OnTable(LiveStreamingSrtModel.TableName)
                .OnColumn(nameof(LiveStreamingSrtModel.lss_num));
        }

        public override void Down()
        {
            // 刪除索引
            Delete.Index($"index_{LiveStreamingSrtModel.TableName}_{nameof(LiveStreamingSrtModel.lss_ls_id)}")
                .OnTable(LiveStreamingSrtModel.TableName);

            Delete.Index($"index_{LiveStreamingSrtModel.TableName}_{nameof(LiveStreamingSrtModel.lss_num)}")
                .OnTable(LiveStreamingSrtModel.TableName);

            // 刪除資料表
            Delete.Table(LiveStreamingSrtModel.TableName);
        }
    }
}
