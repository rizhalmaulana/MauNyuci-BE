# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["MauNyuci.Api.csproj", "./"]
RUN dotnet restore "MauNyuci.Api.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "MauNyuci.Api.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 8080 (default for .NET 8 containers)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MauNyuci.Api.dll"]
