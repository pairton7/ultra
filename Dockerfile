FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y libgdiplus
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/UltraBank.WebApi/UltraBank.WebApi.csproj", "src/UltraBank.WebApi/"]
COPY ["libs/UltraBank.AlertContext/UltraBank.AlertContext.csproj", "libs/UltraBank.AlertContext/"]
COPY ["libs/UltraBank.AuditableInfoContext/UltraBank.AuditableInfoContext.csproj", "libs/UltraBank.AuditableInfoContext/"]
COPY ["libs/UltraBank.NotificationContext/UltraBank.NotificationContext.csproj", "libs/UltraBank.NotificationContext/"]
COPY ["libs/UltraBank.ProcessResultContext/UltraBank.ProcessResultContext.csproj", "libs/UltraBank.ProcessResultContext/"]
COPY ["libs/UltraBank.ObservabilityContext/UltraBank.ObservabilityContext.csproj", "libs/UltraBank.ObservabilityContext/"]
COPY ["src/UltraBank.Application/UltraBank.Application.csproj", "src/UltraBank.Application/"]
COPY ["src/UltraBank.Domain/UltraBank.Domain.csproj", "src/UltraBank.Domain/"]
COPY ["src/UltraBank.Infrascructure/UltraBank.Infrascructure.csproj", "src/UltraBank.Infrascructure/"]
RUN dotnet restore "./src/UltraBank.WebApi/UltraBank.WebApi.csproj"
COPY . .
WORKDIR "/src/src/UltraBank.WebApi"
RUN dotnet build "./UltraBank.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UltraBank.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UltraBank.WebApi.dll"]