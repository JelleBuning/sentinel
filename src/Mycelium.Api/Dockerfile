FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Mycelium.Api/Mycelium.Api.csproj", "Mycelium.Api/"]
COPY ["Mycelium.Api.Application/Mycelium.Api.Application.csproj", "Mycelium.Api.Application/"]
COPY ["Mycelium.Api.Domain/Mycelium.Api.Domain.csproj", "Mycelium.Api.Domain/"]
COPY ["Mycelium.Common/Mycelium.Common.csproj", "Mycelium.Common/"]
COPY ["Mycelium.Api.Infrastructure/Mycelium.Api.Infrastructure.csproj", "Mycelium.Api.Infrastructure/"]
RUN dotnet restore "Mycelium.Api/Mycelium.Api.csproj"
COPY . .
WORKDIR "/src/Mycelium.Api"
RUN dotnet build "Mycelium.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Mycelium.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mycelium.Api.dll"]