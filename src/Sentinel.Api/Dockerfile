FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Sentinel.Api/Sentinel.Api.csproj", "src/Sentinel.Api/"]
COPY ["src/Sentinel.Api.Application/Sentinel.Api.Application.csproj", "src/Sentinel.Api.Application/"]
COPY ["src/Sentinel.Api.Domain/Sentinel.Api.Domain.csproj", "src/Sentinel.Api.Domain/"]
COPY ["src/Sentinel.Common/Sentinel.Common.csproj", "src/Sentinel.Common/"]
COPY ["src/Sentinel.Api.Infrastructure/Sentinel.Api.Infrastructure.csproj", "src/Sentinel.Api.Infrastructure/"]
COPY ["src/Sentinel.WorkerService.Common/Sentinel.WorkerService.Common.csproj", "src/Sentinel.WorkerService.Common/"]
RUN dotnet restore "src/Sentinel.Api/Sentinel.Api.csproj"
COPY . .
WORKDIR "/src/src/Sentinel.Api"
RUN dotnet build "Sentinel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Sentinel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sentinel.Api.dll"]
