FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY src/ ./
RUN dotnet restore "Oneview.AuthService.RestApi/Oneview.AuthService.RestApi.csproj"

COPY . .
WORKDIR "/src/Oneview.AuthService.RestApi"
RUN dotnet build "Oneview.AuthService.RestApi.csproj" -c Release --no-restore -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Oneview.AuthService.RestApi.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime-base
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
USER $APP_UID
# Set the entry point to start the application
ENTRYPOINT ["/app/instrument.sh", "dotnet", "Oneview.AuthService.RestApi.dll"]
