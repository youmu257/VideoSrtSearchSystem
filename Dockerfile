FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Restore as distinct layers
# RUN dotnet restore
COPY ./ ./
# Build and publish a release
WORKDIR /app/VideoSrtSearchSystem
# RUN dotnet publish -c Release -o out
RUN dotnet build -c Release --self-contained false -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/VideoSrtSearchSystem/out .
COPY --from=build-env /app/docker-run.sh .

RUN fc-cache -f -v
ENTRYPOINT ["/bin/bash", "docker-run.sh"]
