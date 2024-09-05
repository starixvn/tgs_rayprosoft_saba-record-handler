#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["SabaRecord.Handler/SabaRecord.Handler.csproj", "SabaRecord.Handler/"]
COPY ["SabaRecord.Repositories/SabaRecord.Repositories.csproj", "SabaRecord.Repositories/"]
COPY ["SabaRecord.Common/SabaRecord.Common.csproj", "SabaRecord.Common/"]
COPY ["SabaRecord.Core/SabaRecord.Core.csproj", "SabaRecord.Core/"]
RUN dotnet restore "./SabaRecord.Handler/./SabaRecord.Handler.csproj"
COPY . .
WORKDIR "/src/SabaRecord.Handler"
RUN dotnet build "./SabaRecord.Handler.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SabaRecord.Handler.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SabaRecord.Handler.dll"]