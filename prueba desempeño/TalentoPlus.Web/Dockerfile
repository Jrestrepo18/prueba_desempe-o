FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copiar archivos del proyecto
COPY . .

# Restaurar y compilar
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

# Publicar
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copiar archivos compilados
COPY --from=build /app/build .

# Exponer puerto
EXPOSE 80
EXPOSE 443

# Ejecutar la aplicación
ENTRYPOINT ["dotnet", "TalentoPlus.Web.dll"]
