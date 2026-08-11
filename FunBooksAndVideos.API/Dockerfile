# Use ASP.NET runtime as base
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

# Don't expose HTTPS for simplicity in development
# EXPOSE 5001

# Use SDK to build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["FunBooksAndVideos.API/FunBooksAndVideos.API.csproj", "FunBooksAndVideos.API/"]
COPY ["FunBooksAndVideos.Domain/FunBooksAndVideos.Domain.csproj", "FunBooksAndVideos.Domain/"]
COPY ["FunBooksAndVideos.Application/FunBooksAndVideos.Application.csproj", "FunBooksAndVideos.Application/"]
COPY ["FunBooksAndVideos.Infrastructure/FunBooksAndVideos.Infrastructure.csproj", "FunBooksAndVideos.Infrastructure/"]
RUN dotnet restore "./FunBooksAndVideos.API/FunBooksAndVideos.API.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/FunBooksAndVideos.API"
RUN dotnet build "./FunBooksAndVideos.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "./FunBooksAndVideos.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FunBooksAndVideos.API.dll"]