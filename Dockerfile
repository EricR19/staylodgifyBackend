# Dockerfile para BookingSite API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto
COPY ["BookingSite.API/BookingSite.API.csproj", "BookingSite.API/"]
COPY ["BookingSite.Application/BookingSite.Application.csproj", "BookingSite.Application/"]
COPY ["BookingSite.Domain/BookingSite.Domain.csproj", "BookingSite.Domain/"]
COPY ["BookingSite.Infrastructure/BookingSite.Infrastructure.csproj", "BookingSite.Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "BookingSite.API/BookingSite.API.csproj"

# Copiar todo el código
COPY . .

# Compilar
WORKDIR "/src/BookingSite.API"
RUN dotnet build "BookingSite.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookingSite.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Crear directorio para receipts
RUN mkdir -p /app/wwwroot/receipts

ENTRYPOINT ["dotnet", "BookingSite.API.dll"]

