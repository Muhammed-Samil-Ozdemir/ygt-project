# YGT ön yüz imajı: site (ui) ve yönetim paneli (admin) derlenir, nginx ile sunulur.
# Kök dizindeki docker-compose.yml tarafından derlenir.
FROM node:22-alpine AS build
WORKDIR /app
ENV NX_DAEMON=false NX_NO_CLOUD=true

# Önce sadece paket dosyaları; bağımlılıklar değişmedikçe bu katman önbellekten gelir.
COPY package.json package-lock.json ./
RUN npm ci --no-audit --no-fund

COPY . .
RUN npx nx run-many -t build -p ui admin

FROM nginx:1.29-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/apps/ui/browser /usr/share/nginx/html
COPY --from=build /app/dist/apps/admin/browser /usr/share/nginx/html/admin
