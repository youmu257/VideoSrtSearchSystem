using FluentMigrator;
using Share.Models.LiveStraming;

namespace Share.Migrations
{
    [Migration(20250531000003)]
    public class CreateLiveStreamingTagMappingTable : Migration
    {
        public override void Up()
        {
            Create.Table(LiveStreamingTagMappingModel.TableName)
                .WithColumn(nameof(LiveStreamingTagMappingModel.lstm_id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(LiveStreamingTagMappingModel.lstm_ls_id)).AsInt32().NotNullable()
                .WithColumn(nameof(LiveStreamingTagMappingModel.lstm_lst_id)).AsInt32().NotNullable()
                .WithColumn(nameof(LiveStreamingTagMappingModel.lstm_createtime)).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            // 建立索引以提升查詢效能
            Create.Index($"index_{LiveStreamingTagMappingModel.TableName}_{nameof(LiveStreamingTagMappingModel.lstm_ls_id)}")
                .OnTable(LiveStreamingTagMappingModel.TableName)
                .OnColumn(nameof(LiveStreamingTagMappingModel.lstm_ls_id));

            Create.Index($"index_{LiveStreamingTagMappingModel.TableName}_{nameof(LiveStreamingTagMappingModel.lstm_lst_id)}")
                .OnTable(LiveStreamingTagMappingModel.TableName)
                .OnColumn(nameof(LiveStreamingTagMappingModel.lstm_lst_id));
        }

        public override void Down()
        {
            // 刪除索引
            Delete.Index($"index_{LiveStreamingTagMappingModel.TableName}_{nameof(LiveStreamingTagMappingModel.lstm_ls_id)}")
                .OnTable(LiveStreamingTagMappingModel.TableName);

            Delete.Index($"index_{LiveStreamingTagMappingModel.TableName}_{nameof(LiveStreamingTagMappingModel.lstm_lst_id)}")
                .OnTable(LiveStreamingTagMappingModel.TableName);

            // 刪除資料表
            Delete.Table(LiveStreamingTagMappingModel.TableName);
        }
    }
}
