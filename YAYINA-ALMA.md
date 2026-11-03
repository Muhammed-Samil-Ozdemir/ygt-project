# Yayına alma

Sitenin bir Linux sunucuda HTTPS ile yayına alınması. Sunucuda üç konteyner çalışır (SQL Server, API, web), önlerinde de
Caddy durur. Caddy alan adı için Let's Encrypt sertifikasını kendisi alır ve yeniler.

```
İnternet ──443/80──> Caddy (HTTPS) ──> web (nginx: site, /admin/, /api/) ──> api ──> mssql
```

Dışarıya sadece 80 ve 443 portları açılır. SQL Server, API ve nginx dışarıdan erişilemez.

## 1. Gerekenler

- **Sunucu:** Linux, 64 bit Intel/AMD işlemci (x86_64). SQL Server imajı ARM işlemcide çalışmaz.
  En az 2 GB RAM (SQL Server şartı), 4 GB önerilir. 20 GB boş disk yeter.
- **Docker:** Docker Engine ve Compose eklentisi (2.24 ya da yenisi). Kurulum: https://docs.docker.com/engine/install/
  (RHEL / Rocky / AlmaLinux için "RHEL", CentOS için "CentOS" sayfası). CentOS 7'nin desteği bittiği için `yum`
  depoları arşive (vault.centos.org) taşındı; `yum` "Could not retrieve mirrorlist" derse önce depolar arşive çevrilmeli.
  Kurduktan sonra:
  `sudo systemctl enable --now docker` ve kullanıcıyı docker grubuna ekleyip (`sudo usermod -aG docker $USER`) yeniden bağlanın.
  Kurulduğunu kontrol etmek için: `docker compose version`
- **Alan adı:** `yazilimgelistirme.ktun.edu.tr` için bir **A kaydı** sunucunun IP adresini göstermeli.
  Bu kayıt muhtemelen üniversitenin bilgi işlem birimi tarafından değiştirilir. Kayıt doğru olmadan sertifika alınamaz.
  Kontrol: `nslookup yazilimgelistirme.ktun.edu.tr` sunucunun IP'sini vermeli.
- **Güvenlik duvarı:** TCP 80 ve 443 dışarıdan açık olmalı (80, sertifika alırken ve HTTPS'e yönlendirmek için gerekli).
  Eski site bu sunucuda yayındaysa büyük ihtimalle zaten açıktır.
  RHEL ailesinde (firewalld): `sudo firewall-cmd --permanent --add-service=http --add-service=https && sudo firewall-cmd --reload`
  Ubuntu'da (ufw): `sudo ufw allow 80/tcp && sudo ufw allow 443/tcp` (SSH'ı kapatmamaya dikkat).

## 2. Kurulum

```bash
# Kod
sudo mkdir -p /opt/ygt-project && sudo chown "$USER" /opt/ygt-project
git clone https://github.com/Muhammed-Samil-Ozdemir/ygt-project.git /opt/ygt-project
cd /opt/ygt-project

# .env: şifreler ve JWT anahtarı rastgele üretilir, yönetici şifresi ekrana bir kez yazılır (not alın)
bash scripts/init-env.sh yazilimgelistirme.ktun.edu.tr <yönetici-e-postası>

# İmajları önceden derle (birkaç dakika sürer; eski site bu sırada açık kalır)
docker compose build
```

### Eski sistemi kapatma

Eski site (Apache + Node API, `/var/www` altında) bu sunucuda çalışıyorsa 80/443 portlarını o tutuyordur; yeni sistemin
Caddy'si başlamadan önce kapatılmalı. Eski veri taşınmıyor; dosyalar `/var/www`'de kalır, silmek gerekmez.

```bash
# Apache: kapatılır ve sunucu yeniden başlayınca açılmaz.
# Dikkat: Apache'de başka bir site daha varsa o da kapanır (ls /etc/httpd/conf.d/ ile kontrol edin).
sudo systemctl disable --now httpd

# Eski Node API: nasıl çalıştığına bakın
ps -eo user,cmd | grep -E 'node|pm2' | grep -v grep
#   pm2 ile çalışıyorsa:     pm2 delete all && pm2 save
#   systemd servisiyse:      sudo systemctl disable --now <servis-adı>

# Eski veritabanı artık gerekmiyorsa RAM boşaltmak için (veriler diskte kalır):
sudo systemctl disable --now mysqld 2>/dev/null || sudo systemctl disable --now mariadb

# 80 ve 443 boşaldı mı? (çıktı boş olmalı)
sudo ss -tlnp | grep -E ':(80|443)\s'
```

### Başlatma

```bash
docker compose up -d
```

Kesinti, eski sistemin kapatılmasından bu komutun bitmesine ve Caddy'nin sertifikayı almasına kadar geçen birkaç dakikadır.

Repo gizliyse `git clone` için GitHub girişi gerekir (ör. sunucuda `gh auth login` ya da bir deploy key).

`.env`'deki `COMPOSE_FILE` satırı sayesinde `docker compose` komutları production ekini (`docker-compose.prod.yml`)
kendiliğinden kullanır; `-f` vermek gerekmez. `.env`'i elle doldurmak isterseniz şablon `.env.production.example`.

## 3. Kontrol

```bash
docker compose ps                                   # dört konteyner de "Up" olmalı, mssql "healthy"
docker compose logs caddy | grep -i certificate     # "certificate obtained successfully" görünmeli
curl -I https://yazilimgelistirme.ktun.edu.tr/api/health
```

Sonra tarayıcıdan:

1. `https://yazilimgelistirme.ktun.edu.tr` açılmalı; site eski sitenin metinleri, logosu, kartları ve bağlantılarıyla gelir.
2. `https://yazilimgelistirme.ktun.edu.tr/admin/` adresinde `init-env.sh`'ın verdiği bilgilerle giriş yapın.
   İsterseniz "Hesabım"dan şifreyi değiştirin, "Yöneticiler"den diğer yöneticileri ekleyin.
3. "Site içeriği"nden eksikleri tamamlayın: güncel ekip ve fotoğrafları, geçmiş etkinlikler. "Bize katıl" Google Form
   bağlantısını ve KVKK metnini kontrol edin.

## 4. Yedekleme

```bash
crontab -e
# şu satırı ekleyin: her gece 03.00'te yedek, 14 günden eskiler silinir
0 3 * * * cd /opt/ygt-project && bash scripts/backup.sh >> backups/backup.log 2>&1
```

Yedekler `backups/<tarih>` altına düşer (veritabanı ve yüklenen görseller). Disk bozulursa aynı diskteki yedek de gider;
`backups/` klasörünü düzenli olarak sunucu dışına kopyalayın (ör. `rsync` ile başka bir makineye).

Geri yükleme: `bash scripts/restore.sh backups/<tarih>` (mevcut verinin yerine geçer, onay ister).

## 5. Güncelleme

Yeni bir sürüm GitHub'a gönderildiğinde:

```bash
cd /opt/ygt-project
bash scripts/backup.sh       # önce yedek
git pull
docker compose up -d --build
```

Veritabanı değişiklikleri (migration) API açılırken kendiliğinden uygulanır. Veri ve görseller volume'lerde durduğu için
güncellemede kaybolmaz.

## 6. Sorun giderme

| Belirti | Neden / çözüm |
|---|---|
| Caddy logunda sertifika hatası, site açılmıyor | A kaydı henüz bu sunucuyu göstermiyor ya da 80/443 kapalı. DNS değişikliğinin yayılması saatler sürebilir; düzelince Caddy kendisi tekrar dener. |
| `docker compose up` "port is already allocated" ya da "address already in use" diyor | 80/443'ü hâlâ eski Apache (ya da başka bir program) tutuyor. `sudo ss -tlnp \| grep -E ':(80\|443)\s'` ile bakın, "Eski sistemi kapatma" adımını uygulayın. |
| `mssql` "unhealthy" | RAM 2 GB'tan az olabilir ya da `MSSQL_SA_PASSWORD` kurala uymuyor. `docker compose logs mssql` |
| API açılmıyor, logda SQL Server'a "pre-login handshake" zaman aşımı var (eski sunucu, ör. CentOS 7) | Eski çekirdekte Docker'ın sanal ağı konteynerler arasında veri iletmiyor. `.env`'de `COMPOSE_FILE=docker-compose.yml:docker-compose.prod.yml:docker-compose.hostnet.yml` yapın, sonra `docker compose down && docker compose up -d`. Servisler sunucunun ağını kullanır ve 127.0.0.1 üzerinden bağlanır; dışarıya yine sadece 80/443 açılır. |
| `mssql` hiç açılmıyor, sürekli yeniden başlıyor ya da API bağlanınca çöküyor (eski sunucu, ör. CentOS 7) | SQL Server 2025 ve 2022 eski Linux çekirdeğinde (3.10) çalışmıyor; 2019 çalışıyor. Veritabanı henüz boşken: `docker compose down -v`, `.env`'de `MSSQL_IMAGE=mcr.microsoft.com/mssql/server:2019-latest`, `docker compose up -d mssql api web`. |
| **CentOS 7 sunucusunda çalışan ayar (2026-11-03)** | `.env`'de `COMPOSE_FILE=docker-compose.yml:docker-compose.prod.yml:docker-compose.hostnet.yml` ve `MSSQL_IMAGE=mcr.microsoft.com/mssql/server:2019-latest`. |
| Panel şifresi unutuldu | `.env`'de `ADMIN_RESET_PASSWORD=true`, `docker compose up -d`, girişten sonra tekrar `false` ve `docker compose up -d`. |
| Loglara bakmak | `docker compose logs -f api` (loglar servis başına en fazla 50 MB tutulur, eskiler silinir) |

**Önemli:** `docker compose down -v` bütün veriyi, görselleri ve sertifikaları siler. Sunucuda kullanmayın;
durdurmak için `docker compose down` (volume'ler kalır) yeterli.

## Eski sistem hakkında

- Eski sistem bırakıldı; verisi (başvurular, sertifikalar) taşınmadı. Eski sertifikaların doğrulama linkleri ve QR kodları
  geçişten sonra "bulunamadı" der. Eski panel adresleri (`/api/admin/...`) de artık çalışmaz.
- Yeni sistem başka bir sunucuya kurulursa alan adının DNS kaydı oraya yönlendirilmeli. Değişiklikten bir gün önce
  kaydın TTL'ini düşürmek (ör. 300 saniye) geçişi hızlandırır.
