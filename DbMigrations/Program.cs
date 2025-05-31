using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace DbMigrations
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 設定資料庫連線字串
            var connectionString = "Server=localhost;User ID=root;Password=;Database=livestramingdatabase;Max Pool Size=100;Pooling=True;";

            // 建立 DI 容器
            var serviceProvider = CreateServices(connectionString);

            // 執行遷移
            using (var scope = serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp(); // 執行所有尚未執行的遷移
            }

            Console.WriteLine("遷移完成。");
        }

        private static IServiceProvider CreateServices(string connectionString)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddMySql5() // 根據你的 MySQL 版本選擇 AddMySql5 或 AddMySql8
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(Share.Migrations.CreateLiveStreamingTable).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }
    }
}
