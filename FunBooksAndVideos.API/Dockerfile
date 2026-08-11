# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

# SDK image used for building and migrations
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Install EF Core CLI
RUN dotnet tool install --global dotnet-ef

ENV PATH="${PATH}:/root/.dotnet/tools"

# Copy project files first for better Docker layer caching
COPY ["FunBooksAndVideos.API/FunBooksAndVideos.API.csproj", "FunBooksAndVideos.API/"]
COPY ["FunBooksAndVideos.Domain/FunBooksAndVideos.Domain.csproj", "FunBooksAndVideos.Domain/"]
COPY ["FunBooksAndVideos.Application/FunBooksAndVideos.Application.csproj", "FunBooksAndVideos.Application/"]
COPY ["FunBooksAndVideos.Infrastructure/FunBooksAndVideos.Infrastructure.csproj", "FunBooksAndVideos.Infrastructure/"]

RUN dotnet restore "./FunBooksAndVideos.API/FunBooksAndVideos.API.csproj"

# Copy source
COPY . .

# Build
WORKDIR "/src/FunBooksAndVideos.API"
RUN dotnet build "./FunBooksAndVideos.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "./FunBooksAndVideos.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "FunBooksAndVideos.API.dll"]