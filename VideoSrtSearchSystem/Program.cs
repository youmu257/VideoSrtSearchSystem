using Serilog;
using VideoSrtSearchSystem.Config;
using VideoSrtSearchSystem.Repository.LiveStraming;
using VideoSrtSearchSystem.Repository.Srt;
using VideoSrtSearchSystem.Services.Srt;
using VideoSrtSearchSystem.Tool;
using VideoSrtSearchSystem.Tool.Language;
using VideoSrtSearchSystem.Tool.MySQL;

LangTool.SetLang(Lang.ZH_TW);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("corsapp",
//        builder =>
//        {
//            builder//.WithOrigins("*")
//                .AllowAnyHeader()
//                .AllowAnyMethod()
//                .AllowCredentials();
//        });
//});
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
builder.Services.AddSingleton<IVoiceFileTool, VoiceFileTool>();
builder.Services.AddSingleton<IMySqlTool, MySqlTool>();
#endregion

#region Service 依賴注入
builder.Services.AddTransient<ISrtService, SrtService>();
#endregion

#region Repository 依賴注入
builder.Services.AddTransient<ILiveStreamingRepository, LiveStreamingRepository>();
builder.Services.AddTransient<ILiveStreamingSrtRepository, LiveStreamingSrtRepository>();
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

app.Run();