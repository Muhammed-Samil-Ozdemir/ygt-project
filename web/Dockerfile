# YGT ön yüz imajı: site (ui) ve yönetim paneli (admin) derlenir, nginx ile sunulur.
# Kök dizindeki docker-compose.yml tarafından derlenir.
FROM node:22-alpine AS build
WORKDIR /app
ENV NG_CLI_ANALYTICS=false

# Önce sadece paket dosyaları; bağımlılıklar değişmedikçe bu katman önbellekten gelir.
COPY package.json package-lock.json ./
RUN npm ci --no-audit --no-fund

COPY . .
# Derleme Nx yerine doğrudan Angular CLI ile yapılır: Nx'in kullandığı SQLite veritabanı eski çekirdekli
# sunucularda (ör. CentOS 7) "disk I/O error" veriyor. Ayarlar apps/*/project.json'dan okunur (tools/angular-json.mjs).
# Bilgisayarda geliştirme ve CI yine Nx ile çalışır.
RUN node tools/angular-json.mjs ui admin \
 && npx ng build ui \
 && npx ng build admin

FROM nginx:1.29-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/apps/ui/browser /usr/share/nginx/html
COPY --from=build /app/dist/apps/admin/browser /usr/share/nginx/html/admin
