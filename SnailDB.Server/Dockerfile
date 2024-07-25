# Base Image for ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
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

# Copy the database files to the volume directory
COPY SnailDB.Server/Data/SnailDB.db /app/Data/SnailDB.db
COPY SnailDB.Server/Data/SnailDB.db-shm /app/Data/SnailDB.db-shm
COPY SnailDB.Server/Data/SnailDB.db-wal /app/Data/SnailDB.db-wal

# Ensure the directories are writable
RUN chmod -R 777 /app/Data

# Define a volume for the database files
VOLUME /app/Data

# Copy the HTTPS Certificate
COPY SnailDB.Server/certificate.pfx /https/certificate.pfx

# Ensure the application is running as a non-root user
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

USER appuser

ENV ASPNETCORE_URLS="https://+:8081;http://+:8080"
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="Tinman1234"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/https/certificate.pfx"

ENTRYPOINT ["dotnet", "SnailDB.Server.dll"]