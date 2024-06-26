﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CorreioElegante/CorreioElegante.csproj", "CorreioElegante/"]
RUN dotnet restore "CorreioElegante/CorreioElegante.csproj"
COPY . .
WORKDIR "/src/CorreioElegante"
RUN dotnet build "CorreioElegante.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CorreioElegante.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migration
WORKDIR /app
COPY --from=publish /app/publish .
# Command to run migrations
RUN dotnet ef database update

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CorreioElegante.dll"]
