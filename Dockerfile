FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MauNyuci.Api/MauNyuci.Api.csproj", "MauNyuci.Api/"]
RUN dotnet restore "MauNyuci.Api/MauNyuci.Api.csproj"
COPY . .
WORKDIR "/src/MauNyuci.Api"
RUN dotnet build "MauNyuci.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MauNyuci.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Environment vars untuk memastikan berjalan di Production
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MauNyuci.Api.dll"]
