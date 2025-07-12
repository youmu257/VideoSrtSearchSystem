# 使用 .NET 8 SDK 作為建置階段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製所有專案檔案並還原套件
COPY ["VideoSrtSearchSystem/VideoSrtSearchSystem.csproj", "VideoSrtSearchSystem/"]
COPY ["Share/Share.csproj", "Share/"]
COPY ["DbMigrations/DbMigrations.csproj", "DbMigrations/"]

# 還原依賴項目
RUN dotnet restore "VideoSrtSearchSystem/VideoSrtSearchSystem.csproj"

# 複製所有原始碼
COPY . .

# 建置解決方案
WORKDIR "/src"
RUN dotnet build "VideoSrtSearchSystem/VideoSrtSearchSystem.csproj" -c Release -o /app/build --no-restore

# 發布應用程式
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/VideoSrtSearchSystem"
RUN dotnet publish "VideoSrtSearchSystem.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    --self-contained false \
    --verbosity minimal

# 使用 .NET 8 Runtime 作為執行階段
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 安裝必要套件
RUN apt-get update && \
    apt-get install -y \
        tzdata \
        curl \
        && rm -rf /var/lib/apt/lists/*

# 設定時區為台北時間
ENV TZ=Asia/Taipei
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# 建立非 root 使用者
RUN groupadd -r appuser && useradd -r -g appuser appuser

# 複製發布的檔案
COPY --from=publish /app/publish .

# 建立日誌目錄並設定權限
RUN mkdir -p /app/logs && \
    chown -R appuser:appuser /app

# 設定環境變數
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

# 暴露 Port
EXPOSE 80

# 健康檢查
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# 切換到非 root 使用者
USER appuser

# 設定進入點
ENTRYPOINT ["dotnet", "VideoSrtSearchSystem.dll"]