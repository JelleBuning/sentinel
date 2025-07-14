FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sentinel.Api/Sentinel.Api.csproj", "Sentinel.Api/"]
COPY ["Sentinel.Api.Application/Sentinel.Api.Application.csproj", "Sentinel.Api.Application/"]
COPY ["Sentinel.Api.Domain/Sentinel.Api.Domain.csproj", "Sentinel.Api.Domain/"]
COPY ["Sentinel.Common/Sentinel.Common.csproj", "Sentinel.Common/"]
COPY ["Sentinel.Api.Infrastructure/Sentinel.Api.Infrastructure.csproj", "Sentinel.Api.Infrastructure/"]
RUN dotnet restore "Sentinel.Api/Sentinel.Api.csproj"
COPY . .
WORKDIR "/src/Sentinel.Api"
RUN dotnet build "Sentinel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Sentinel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sentinel.Api.dll"]