#!/usr/bin/env bash
# backup.sh ile alınan yedeği geri yükler.
#
# Kullanım (ygt-project klasöründe, sistem docker compose ile çalışırken):
#   bash scripts/restore.sh backups/2026-11-01_03-00
#
# DİKKAT: Mevcut veritabanının yerine yedekteki konur; yedekten sonra girilen veriler kaybolur.
# Yedekteki görseller storage klasörüne kopyalanır (aynı adlı dosyaların üzerine yazılır).
set -euo pipefail

export MSYS_NO_PATHCONV=1

SOURCE="${1:?Kullanım: bash scripts/restore.sh <yedek klasörü>}"
SOURCE="$(cd "$SOURCE" && pwd)"
# Git Bash'te (Windows) Docker'a "/c/..." değil "C:/..." biçiminde yol verilmeli.
if command -v cygpath >/dev/null 2>&1; then
  SOURCE="$(cygpath -m "$SOURCE")"
fi
cd "$(dirname "$0")/.."

if [ ! -f "$SOURCE/ygt.bak" ]; then
  echo "HATA: $SOURCE/ygt.bak bulunamadı." >&2
  exit 1
fi

SA_PASSWORD="$(grep -E '^MSSQL_SA_PASSWORD=' .env | cut -d= -f2- | tr -d '\r')"

read -r -p "Mevcut veritabanı silinip $SOURCE yedeği yüklenecek. Devam edilsin mi? (evet/hayır) " answer
if [ "$answer" != "evet" ]; then
  echo "Vazgeçildi."
  exit 1
fi

# API veritabanına bağlıyken geri yükleme yapılamaz. Bir adım hata verse bile çıkarken API yeniden başlatılır.
docker compose stop api
trap 'docker compose start api >/dev/null' EXIT

echo "Veritabanı geri yükleniyor..."
docker compose exec -T mssql mkdir -p /var/opt/mssql/backup
docker compose cp "$SOURCE/ygt.bak" mssql:/var/opt/mssql/backup/restore.bak
docker compose exec -T mssql /opt/mssql-tools18/bin/sqlcmd -S 127.0.0.1 -U sa -P "$SA_PASSWORD" -C -b -Q "
  IF DB_ID('Ygt') IS NOT NULL ALTER DATABASE [Ygt] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
  RESTORE DATABASE [Ygt] FROM DISK = N'/var/opt/mssql/backup/restore.bak' WITH REPLACE;
  ALTER DATABASE [Ygt] SET MULTI_USER;"

docker compose start api

if [ -d "$SOURCE/storage" ]; then
  echo "Görseller geri yükleniyor..."
  docker compose cp "$SOURCE/storage/." api:/app/storage/
  # Kopyalanan dosyalar root'a ait olur; API root olmayan "app" kullanıcısıyla çalıştığı için sahipliği verilir.
  docker compose exec -T -u 0 api chown -R app:app /app/storage
fi

echo "Geri yükleme tamamlandı."
