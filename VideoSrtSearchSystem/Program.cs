using Serilog;
using Share.Config.Appsettings;
using Share.Const;
using Share.Repositorys.LiveStraming;
using Share.Repositorys.Srt;
using Share.Repositorys.Tag;
using Share.Services.Srt;
using Share.Services.Tag;
using Share.Services.Video;
using Share.Tool;
using Share.Tool.Language;
using Share.Tool.MySQL;

LangTool.SetLang(Lang.ZH_TW);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// 新增 CORS 設定，允許 localhost
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policyBuilder =>
        {
            policyBuilder.WithOrigins(
                "http://localhost",
                "http://localhost:3000",
                "http://localhost:5000",
                "http://localhost:5173",
                "http://localhost:4200",
                "http://localhost:8000",
                "https://youmu257.github.io"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});
builder.Services.AddOpenApiDocument();

#region config
builder.Services.Configure<SrtConfig>(builder.Configuration.GetSection("SrtConfig"));
#endregion

#region log
builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Services.AddLogging(loggingBuilder =>
          loggingBuilder.AddSerilog(dispose: true));
#endregion

#region Tool 依賴注入
builder.Services.AddTransient<IMySQLConnectionProvider, MySQLConnectionProvider>();
builder.Services.AddSingleton<IMySqlTool, MySqlTool>();
builder.Services.AddSingleton<ICommonTool, CommonTool>();
#endregion

#region Service 依賴注入
builder.Services.AddSingleton<ISrtService, SrtService>();
builder.Services.AddSingleton<ILiveStreamingService, LiveStreamingService>();
builder.Services.AddSingleton<ITagService, TagService>();
#endregion

#region Repository 依賴注入
builder.Services.AddSingleton<ILiveStreamingRepository, LiveStreamingRepository>();
builder.Services.AddSingleton<ILiveStreamingSrtRepository, LiveStreamingSrtRepository>();
builder.Services.AddSingleton<ILiveStreamingTagRepository, LiveStreamingTagRepository>();
builder.Services.AddSingleton<ILiveStreamingTagTypeRepository, LiveStreamingTagTypeRepository>();
builder.Services.AddSingleton<ILiveStreamingTagMappingRepository, LiveStreamingTagMappingRepository>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
//app cors
//app.UseCors("corsapp");

// 啟用 CORS
app.UseCors("AllowLocalhost");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

    await next();
});

app.MapControllers();
// 啟動 OpenAPI 文件
app.UseOpenApi();
// 啟動 Swagger UI
app.UseSwaggerUi(settings =>
{
    //settings.Path = "/api-docs"; // 自定義 Swagger UI 的路徑
    settings.DocumentPath = "/swagger.json"; // 指定 OpenAPI 文件的路徑
});

app.Run();
