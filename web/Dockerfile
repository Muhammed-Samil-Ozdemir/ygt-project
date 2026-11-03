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
# Angular CLI bazen derlemeyi bitirip dosyaları yazdıktan sonra kapanmadan takılı kalıyor. Bu yüzden her derleme
# süre sınırıyla çalışır; süre dolduğunda çıktı yazılmışsa derleme başarılı sayılır, yazılmamışsa hata verir.
RUN node tools/angular-json.mjs ui admin \
 && for app in ui admin; do \
      timeout 600 npx ng build "$app" \
      || test -f "dist/apps/$app/browser/index.html" \
      || { echo "HATA: $app derlenemedi" >&2; exit 1; }; \
    done

FROM nginx:1.29-alpine
# nginx.conf bir şablon: açılışta bu değişkenlerle doldurulup conf.d/default.conf olarak yazılır.
# Filtre, nginx'in kendi $değişkenlerine dokunulmamasını garanti eder.
ENV NGINX_LISTEN=80 \
    API_UPSTREAM=api:8080 \
    NGINX_ENVSUBST_FILTER="^(NGINX_LISTEN|API_UPSTREAM)$"
COPY nginx.conf /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist/apps/ui/browser /usr/share/nginx/html
COPY --from=build /app/dist/apps/admin/browser /usr/share/nginx/html/admin
