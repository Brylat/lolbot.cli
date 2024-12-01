﻿FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LolBot.Cli.csproj", "./"]
RUN dotnet restore "LolBot.Cli.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "LolBot.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LolBot.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LolBot.Cli.dll"]