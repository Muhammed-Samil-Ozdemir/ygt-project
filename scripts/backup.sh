#!/usr/bin/env bash
# YGT yedeği: veritabanı (.bak) ve panelden yüklenen görseller (storage) tarihli bir klasöre alınır.
#
# Kullanım (ygt-project klasöründe, sistem docker compose ile çalışırken):
#   bash scripts/backup.sh                 # ./backups altına
#   bash scripts/backup.sh /yedek/ygt      # başka bir klasöre
#
# Sunucuda her gece 03.00'te çalışması için (crontab -e):
#   0 3 * * * cd /opt/ygt-project && bash scripts/backup.sh >> backups/backup.log 2>&1
#
# KEEP_DAYS gününden eski yedekler silinir (varsayılan 14). Yedekleri ayrıca sunucu dışına da kopyalayın;
# disk bozulursa aynı diskteki yedek de gider.
set -euo pipefail

# Git Bash'te (Windows) /var/... gibi yolların Windows yoluna çevrilmesini engeller; Linux'ta etkisi yok.
export MSYS_NO_PATHCONV=1

cd "$(dirname "$0")/.."

BACKUP_ROOT="${1:-./backups}"
KEEP_DAYS="${KEEP_DAYS:-14}"
TARGET="$BACKUP_ROOT/$(date +%Y-%m-%d_%H-%M)"

SA_PASSWORD="$(grep -E '^MSSQL_SA_PASSWORD=' .env | cut -d= -f2- | tr -d '\r')"
if [ -z "$SA_PASSWORD" ]; then
  echo "HATA: .env içinde MSSQL_SA_PASSWORD bulunamadı." >&2
  exit 1
fi

mkdir -p "$TARGET"
# Git Bash'te (Windows) Docker'a "/c/..." değil "C:/..." biçiminde yol verilmeli.
if command -v cygpath >/dev/null 2>&1; then
  TARGET="$(cygpath -m "$TARGET")"
fi

echo "Veritabanı yedekleniyor..."
docker compose exec -T mssql mkdir -p /var/opt/mssql/backup
docker compose exec -T mssql /opt/mssql-tools18/bin/sqlcmd -S 127.0.0.1 -U sa -P "$SA_PASSWORD" -C -b \
  -Q "BACKUP DATABASE [Ygt] TO DISK = N'/var/opt/mssql/backup/ygt.bak' WITH INIT, CHECKSUM"
docker compose cp mssql:/var/opt/mssql/backup/ygt.bak "$TARGET/ygt.bak"

echo "Görseller yedekleniyor..."
docker compose cp api:/app/storage "$TARGET/storage"

echo "Eski yedekler temizleniyor ($KEEP_DAYS günden eski)..."
find "$BACKUP_ROOT" -mindepth 1 -maxdepth 1 -type d -name '20*' -mtime +"$KEEP_DAYS" -exec rm -rf {} +

echo "Yedek alındı: $TARGET"
