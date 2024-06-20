# Base Image for ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# SDK image with Node.js installed for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS with-node
RUN apt-get update
RUN apt-get install curl
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash
RUN apt-get -y install nodejs

# Building the application
FROM with-node AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SnailDB.Server/SnailDB.Server.csproj", "SnailDB.Server/"]
COPY ["snaildb.client/snaildb.client.esproj", "snaildb.client/"]
RUN dotnet restore "./SnailDB.Server/SnailDB.Server.csproj"
COPY . .
WORKDIR "/src/SnailDB.Server"
RUN dotnet build "./SnailDB.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publishing the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SnailDB.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY SnailDB.Server/Data/SnailDB.db /app/Data/SnailDB.db
ENTRYPOINT ["dotnet", "SnailDB.Server.dll"]
