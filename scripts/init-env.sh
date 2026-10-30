#!/usr/bin/env bash
# Sunucuda .env dosyasını oluşturur: .env.production.example'ı kopyalar, şifreleri ve JWT anahtarını rastgele üretir.
#
# Kullanım (ygt-project klasöründe):
#   bash scripts/init-env.sh yazilimgelistirme.ktun.edu.tr yonetici@ornek.com
#
# Var olan .env'in üzerine yazmaz. Üretilen yönetici şifresi sadece bir kez ekrana yazılır; not alın,
# girişten sonra panelde "Hesabım"dan değiştirebilirsiniz. SQL Server şifresi ve JWT anahtarı .env'de kalır.
set -euo pipefail

cd "$(dirname "$0")/.."

usage="Kullanım: bash scripts/init-env.sh <alan-adı> <yönetici-e-postası>"
domain="${1:?$usage}"
admin_email="${2:?$usage}"

if [ -e .env ]; then
  echo "HATA: .env zaten var; üzerine yazılmadı. Baştan oluşturmak için önce silin ya da adını değiştirin." >&2
  exit 1
fi

if ! command -v openssl >/dev/null 2>&1; then
  echo "HATA: openssl bulunamadı (Ubuntu/Debian: sudo apt install openssl)." >&2
  exit 1
fi

# SQL Server kuralı: büyük harf, küçük harf, rakam ve sembol. Bağlantı cümlesini bozacak karakter (;) içermez.
sa_password="Ygt_$(openssl rand -hex 16)"
admin_password="Ygt-$(openssl rand -hex 10)"
jwt_secret_key="$(openssl rand -base64 64 | tr -d '\n')"

sed \
  -e "s|^DOMAIN=.*|DOMAIN=${domain}|" \
  -e "s|^MSSQL_SA_PASSWORD=.*|MSSQL_SA_PASSWORD=${sa_password}|" \
  -e "s|^JWT_SECRET_KEY=.*|JWT_SECRET_KEY=${jwt_secret_key}|" \
  -e "s|^ADMIN_EMAIL=.*|ADMIN_EMAIL=${admin_email}|" \
  -e "s|^ADMIN_PASSWORD=.*|ADMIN_PASSWORD=${admin_password}|" \
  .env.production.example > .env

# .env şifre içerdiği için sadece sahibi okuyabilsin.
chmod 600 .env

echo ".env oluşturuldu (alan adı: ${domain})."
echo
echo "Panel girişi (bir kez gösterilir, not alın):"
echo "  E-posta: ${admin_email}"
echo "  Şifre:   ${admin_password}"
echo
echo "Sonraki adım: docker compose up -d --build"
