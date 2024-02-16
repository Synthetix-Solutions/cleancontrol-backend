FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["cleancontrol-backend/cleancontrol-backend.csproj", "cleancontrol-backend/"]
RUN dotnet restore "cleancontrol-backend/cleancontrol-backend.csproj"
COPY . .
WORKDIR "/src/cleancontrol-backend"
RUN dotnet build "cleancontrol-backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "cleancontrol-backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "cleancontrol-backend.dll"]
