# YGT API imajı. Kök dizindeki docker-compose.yml tarafından derlenir.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Önce sadece proje dosyası kopyalanır; paketler değişmedikçe restore katmanı önbellekten gelir.
COPY Ygt.WebAPI/Ygt.WebAPI.csproj Ygt.WebAPI/
RUN dotnet restore Ygt.WebAPI/Ygt.WebAPI.csproj

COPY Ygt.WebAPI/ Ygt.WebAPI/
RUN dotnet publish Ygt.WebAPI/Ygt.WebAPI.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Sertifika görselleri bu klasöre yazılır (volume). Uygulama root olmayan kullanıcıyla çalıştığı için sahipliği veriliyor.
RUN mkdir -p /app/storage && chown "$APP_UID" /app/storage

COPY --from=build /app/publish .

USER $APP_UID
ENTRYPOINT ["dotnet", "Ygt.WebAPI.dll"]
