# YGT Project

KTÜN Yazılım Geliştirme Topluluğu sitesinin yeni sürümü. Eski Node.js/Express API'nin ve React ön yüzün yerini alır.

| Klasör / dosya | İçerik |
|---|---|
| `server/` | ASP.NET Core 10 Web API (`Ygt.WebAPI`), EF Core + SQL Server |
| `web/` | Angular 22 ön yüz, Nx monorepo: `apps/ui` (site), `apps/admin` (yönetim paneli), `libraries` (ortak kod) |
| `docker-compose.yml` | SQL Server + API + web (nginx) |

## Çalıştırma

### Docker ile

```bash
cp .env.example .env          # yerel geliştirme değerleri hazır gelir; sunucuda hepsini değiştirin
docker compose up -d --build
```

| Adres | İçerik |
|---|---|
| `http://localhost/` | Site |
| `http://localhost/admin/` | Yönetim paneli (`.env` içindeki `ADMIN_EMAIL` ve `ADMIN_PASSWORD` ile giriş) |
| `http://localhost/api/` | API. Dışarıya ayrıca açılmaz, nginx üzerinden erişilir. Sağlık kontrolü: `/api/health` |

80 portu doluysa `.env` içindeki `WEB_PORT` değiştirilir.

### Geliştirme

API ve veritabanı:

```bash
docker compose up -d mssql

cd server
dotnet tool restore           # dotnet-ef
dotnet user-secrets set "Seed:Admin:Password" "<admin şifresi>" --project Ygt.WebAPI
dotnet run --project Ygt.WebAPI
```

API `http://localhost:5146` adresinde açılır. OpenAPI dokümanı: `http://localhost:5146/openapi/v1.json` (sadece geliştirmede).

Ön yüz (API çalışırken, ayrı terminallerde):

```bash
cd web
npm install
npx nx serve ui               # http://localhost:4200
npx nx serve admin            # http://localhost:4201/admin/
```

Geliştirme sunucusu `/api` isteklerini `proxy.conf.json` ile `localhost:5146`'ya yönlendirir; Docker'daki nginx de aynı yolu kullanır.

API her açılışta bekleyen migration'ları uygular ve gerekli kayıtları ekler: site ayarları, ana sayfa ziyaret sayacı ve ilk kurulumda (hiç yönetici yokken) `.env`'deki bilgilerle ilk yönetici.
Sonrasında yöneticiler ve şifreler panelden ("Yöneticiler", "Hesabım") yönetilir; `.env`'deki şifreyi değiştirmek mevcut yöneticiyi etkilemez.
Şifre unutulursa `.env`'de `ADMIN_RESET_PASSWORD=true` yapıp `docker compose up -d` çalıştırılır: `ADMIN_EMAIL` yöneticisinin şifresi `ADMIN_PASSWORD` olur. Giriş yaptıktan sonra ayar `false`'a geri alınmalı.

### Testler

```bash
cd server
dotnet test --project Ygt.WebAPI.Tests
```

Entegrasyon testleri SQL Server'ı Testcontainers ile geçici bir Docker konteynerinde açar; Docker çalışıyor olmalı.
GitHub'a gönderilen her değişiklikte `.github/workflows/ci.yml` testleri, ön yüz derlemesini ve Docker imajlarını kontrol eder.

### Yedekleme

```bash
bash scripts/backup.sh                      # ./backups/<tarih> altına veritabanı (.bak) ve görseller
bash scripts/restore.sh backups/<tarih>     # yedeği geri yükler (mevcut verinin yerine geçer, onay ister)
```

Sunucuda her gece yedek için `crontab -e` ile: `0 3 * * * cd /opt/ygt-project && bash scripts/backup.sh >> backups/backup.log 2>&1`.
14 günden eski yedekler silinir (`KEEP_DAYS` ile değişir). Yedekleri ayrıca sunucu dışına da kopyalayın.

Site içeriği boş başlar: logo, sekme simgesi, bütün metinler, hakkımızda kartları, ekip, teknolojiler ve bağlantılar panelde "Site içeriği" bölümünden girilir.
Boş bırakılan yazı ve görsel sitede gösterilmez. Bölüm başlıkları gibi bazı alanlarda boşken genel bir ifade ("Hakkımızda") yazılır; bunlar `Utilities/SiteTextCatalog.cs`'te `Placeholder` olarak tanımlı.

## API

Hatalarda gövde her zaman `{ "message": "..." }` biçimindedir ve mesaj Türkçedir, ön yüz doğrudan gösterebilir.
Admin uçları `Authorization: Bearer <token>` başlığını ister. Token `POST /auth/login` ile alınır ve 4 saat geçerlidir.

### Herkese açık

| Metot | Adres | Açıklama |
|---|---|---|
| POST | `/auth/login` | `{ email, password }` → `{ token, expiresAt }` |
| GET | `/site/settings` | Ana sayfa butonu (`course` / `form`) ve geri sayım |
| GET | `/site/content` | Sitenin bütün içeriği tek istekte: `texts` (anahtar -> metin; görsellerde dosya adı), `about`, `events`, `team`, `technologies`, `links`. Sadece görünür kayıtlar, panelde verilen sırayla; fotoğrafı olmayan etkinlik gelmez |
| GET | `/site/content/images/{dosya}` | Panelden yüklenen görsel (logo, ekip fotoğrafı, kart görseli...). Dosya adı her yüklemede değiştiği için uzun süre önbelleğe alınabilir |
| POST | `/site/visits/{page}` | Ziyaret sayacını artırır (şimdilik sadece `home`) |
| GET | `/courses/active` | Aktif eğitimler |
| POST | `/courses/{id}/applications` | Eğitim başvurusu |
| GET | `/forms/active` | Aktif etkinlik formu (`type`: `standard` / `hackathon`); yoksa 404 |
| POST | `/forms/{id}/applications` | Bireysel etkinlik başvurusu |
| POST | `/forms/{id}/hackathon-applications` | Takım başvurusu (`member1`, `member2`, isteğe bağlı `member3`) |
| GET | `/certificates/{code}` | Sertifika doğrulama |
| GET | `/certificates/{code}/image` | Sertifika görseli |

### Admin

| Metot | Adres | Açıklama |
|---|---|---|
| PUT | `/site/settings` | Ana sayfa butonu ve geri sayımı değiştirir |
| GET | `/site/visits` | Ziyaret sayaçları |
| GET, POST | `/courses` | Tüm eğitimler (başvuru sayısıyla), yeni eğitim |
| PUT | `/courses/{id}` | Başlık ve açıklama |
| POST | `/courses/{id}/close`, `/courses/{id}/reopen` | Başvuruları kapat / aç |
| GET | `/courses/{id}/applications` | Başvurular |
| GET | `/courses/{id}/applications/export` | Başvurular Excel (.xlsx) olarak |
| GET, POST | `/forms` | Tüm formlar, yeni form (önceki aktif form kapanır) |
| PUT | `/forms/{id}` | Başlık, açıklama; tür sadece başvuru yokken değişir |
| POST | `/forms/{id}/close`, `/forms/{id}/reopen` | Formu kapat / yeniden aç |
| GET | `/forms/{id}/applications`, `/forms/{id}/hackathon-applications` | Başvurular |
| GET | `/forms/{id}/applications/export` | Başvurular Excel olarak (türüne göre) |
| DELETE | `/courses/{id}/applications`, `/courses/{id}/applications/{basvuruId}` | Başvuruların hepsini / birini kalıcı olarak siler (KVKK) |
| DELETE | `/forms/{id}/applications` | Formun bütün başvurularını siler (türüne bakılmaz) |
| DELETE | `/forms/{id}/applications/{basvuruId}`, `/forms/{id}/hackathon-applications/{basvuruId}` | Tek başvuruyu / takımı siler |
| GET, POST | `/certificates` | Tüm sertifikalar, yeni sertifika (doğrulama kodu otomatik) |
| PUT, DELETE | `/certificates/{id}` | Düzenle, sil |
| PUT | `/certificates/{id}/image` | Görsel yükle (`multipart/form-data`, alan adı `file`, PNG/JPG/WEBP, en fazla 5 MB) |
| GET | `/certificates/import/template` | Toplu ekleme için boş Excel şablonu (Eğitim adı, Ad, Soyad, E-posta, Telefon) |
| POST | `/certificates/import` | Excel'den toplu ekleme (`multipart/form-data`, alan adı `file`, en fazla 1000 satır). Hatalı satır varsa hiçbiri eklenmez, satır numaralarıyla hata döner |
| GET | `/certificates/export?siteUrl=...` | Bütün sertifikalar doğrulama linkleriyle Excel olarak; linkler `siteUrl` ile başlar |
| GET, POST | `/users` | Yöneticiler (`isCurrent`: isteyen kişi), yeni yönetici (`{ name, email, password }`, şifre en az 8 karakter) |
| PUT, DELETE | `/users/{id}` | Ad ve e-posta / yöneticiyi sil (kişi kendini silemez; silinenin token'ı hemen geçersiz olur) |
| PUT | `/users/{id}/password` | Başka bir yöneticiye yeni şifre belirle |
| GET | `/auth/me` | Giriş yapmış yöneticinin bilgileri |
| PUT | `/auth/password` | Kendi şifresini değiştir (`{ currentPassword, newPassword }`; giriş gibi istek sınırına tabi) |
| GET, PUT | `/site/content/texts` | Tek parça metinler ve görseller: tanımları (etiket, tür, sınır, boşken yazılan ifade) ve değerleri / `{ values: { anahtar: metin } }` ile metinleri değiştirme |
| PUT, DELETE | `/site/content/texts/{anahtar}/image` | Görsel alanına (`logo`, `favicon`, `heroImage`) görsel yükle (`certificates/{id}/image` ile aynı kurallar) / kaldır |
| GET, POST | `/site/content/about`, `/events`, `/team`, `/technologies`, `/links` | İçerik listesi (gizliler dahil), listenin sonuna yeni kayıt |
| PUT, DELETE | `/site/content/{liste}/{id}` | Kaydı düzenle, sil |
| PUT | `/site/content/{liste}/order` | Sıralama: `{ ids: [...] }` ilk sıradan son sıraya |
| PUT, DELETE | `/site/content/team/{id}/photo`, `/site/content/about/{id}/image` | Ekip fotoğrafı / kart görseli yükle (aynı kurallar) / kaldır |
| POST | `/site/content/events/{id}/photos` | Etkinliğe fotoğraf ekle (aynı kurallar, etkinlik başına en fazla 12). Etkinliğin güncel hâli döner |
| DELETE | `/site/content/events/{id}/photos/{fotoğrafId}` | Fotoğrafı sil |
| PUT | `/site/content/events/{id}/photos/order` | Fotoğraf sırası; ilk fotoğraf kapak |

### Kurallar

- Telefonlar `5551234567` biçimine çevrilip saklanır. `0555 123 45 67`, `+90 555...` gibi yazımların hepsi kabul edilir ve aynı numara sayılır.
- Aynı eğitime aynı e-posta, telefon veya TC ile ikinci başvuru yapılamaz. Etkinlik formunda bu kontrol telefon veya e-postayla yapılır.
- Hackathon: takım adı formda tektir. Üye e-postaları `.edu` içermeli. Bir kişi aynı forma iki takımla başvuramaz. 3. üyenin bir alanı doluysa hepsi zorunludur.
- Aynı anda tek aktif form olur. Yeni form açmak ya da formu yeniden açmak, açık olan formu kapatır.
- Eğitim oluşturmak ana sayfa butonunu `course`, form açmak `form` yapar. Form kapanınca buton `course`'a döner (eski sistemdeki davranış).
- İstek sınırı (IP başına, dakikada): 60 başvuru, 5 giriş denemesi. `appsettings.json` > `RateLimiting` ile değiştirilebilir.
- Başvurular otomatik silinmez. KVKK gereği silinmesi gerekenler panelden tek tek ya da toplu silinir (eğitim ve formun kendisi kalır).
- Site içeriği düz metindir, HTML olarak yorumlanmaz. Bağlantılar sadece `http(s)` ve `mailto` olabilir.
- İçerik listeleri panelde sürükle-bırakla ya da oklarla sıralanır (dokunmatik ekranda sadece oklar).
- Bütün yöneticilerin yetkisi aynıdır. Sertifikanın QR kodu panelde tarayıcıda üretilip PNG olarak indirilir.

## Veritabanı

| Tablo | Eski tablo (MySQL) | Not |
|---|---|---|
| `Users` | `users` | Sadece adminler. Eski bcrypt özetleri aynen çalışır. |
| `Courses` | `courses` | |
| `CourseApplications` | `appeals` | `Courses`'a bağlı. Aynı kursa aynı e-postayla tek başvuru. |
| `Forms` | `forms` | `Type` (Standard / Hackathon) eklendi. Aynı anda en fazla bir aktif form. |
| `FormApplications` | `formAppeals` | Aynı forma aynı telefonla tek başvuru. |
| `HackathonApplications` | `hackathonForms` | Aynı formda takım adı tekrar edemez. |
| `Certificates` | `certificates` | Doğrulama kodu `Code` sütununda (eski ID'ler taşınırsa buraya yazılır). Görsel `storage/certificates` altında. |
| `SiteSettings` | `activePanels` + `activityCounters` | Tek satır: ana sayfa butonu ve geri sayım. |
| `VisitorCounters` | `counters` | |
| `SiteTexts` | — | Sitedeki tek parça metinler ve görseller (logo, sekme simgesi, giriş görseli), anahtar/değer. Anahtarlar `Utilities/SiteTextCatalog.cs`'te tanımlı. |
| `Events`, `EventPhotos` | — | Ana sayfadaki etkinlikler ve fotoğrafları. `Style`: `Wall` (etkinlik panosu), `Featured` (öne çıkan), `Recurring` (geleneksel). Etkinlik silinince fotoğrafları da silinir. Başvuru alınan `Forms`'tan ayrıdır. |
| `AboutCards`, `TeamMembers`, `Technologies`, `SiteLinks` | — | Ana sayfa listeleri. `SortOrder` ile sıralanır; teknolojiler dışındakiler gizlenebilir (`IsVisible`). Kartlarda isteğe bağlı görsel var. |
| — | `sessions` | Kaldırıldı. Spam koruması istek sınırıyla yapılıyor. |

Yeni migration eklemek için:

```bash
cd server
dotnet ef migrations add <migration_adi> --project Ygt.WebAPI
```

Not: `sqlcmd` ile veri yazarken `-I` parametresi gerekir. `Forms` tablosundaki filtreli index `QUOTED_IDENTIFIER` ayarının açık olmasını istiyor.

## Backend klasör yapısı

```
server/Ygt.WebAPI/
  Abstractions/     BaseEntity (Id, CreatedAt, UpdatedAt), ISortable (panelden sıralanan listeler)
  Models/           Entity'ler
  Enums/            FormType, HomeButton, SiteTextKind
  Configurations/   EF Core tablo ayarları (IEntityTypeConfiguration)
  Context/          AppDbContext, açılışta migration + seed
  Migrations/
  Dtos/             İstek ve yanıt kayıtları (record)
  Services/         İş kuralları; DbContext'i doğrudan kullanır
  Controllers/      İnce katman: servisi çağırır
  Options/          JWT ve dosya saklama ayarları
  Providers/        JWT, Excel üretimi, görsel saklama (sertifika görselleri ve sitenin görselleri)
  Middlewares/      Hataları { message } olarak döndüren handler
  Extensions/       Controller, hata biçimi ve istek sınırı kurulumu
  Utilities/        Girdi doğrulama, Türkiye saati, site metinlerinin tanımları, sıralama
```

## Ön yüz klasör yapısı

```
web/
  apps/ui/          Site: ana sayfa, eğitim / etkinlik / hackathon başvurusu, geri sayım, sertifika doğrulama.
                    Logo dahil bütün içerik API'den gelir; kodda sadece arayüz yazıları (form etiketleri gibi) var.
  apps/admin/       Yönetim paneli: giriş, panel ve site ayarları, eğitimler, formlar, sertifikalar, site içeriği,
                    yöneticiler ve hesabım
  libraries/        Ortak kod: HttpService, hata bildirimleri, token, modeller, simgeler, ortak stiller ve fontlar
  nginx.conf        Docker'da siteyi, paneli ve /api yönlendirmesini sunar
```

Eski sitenin adresleri (`/appeal`, `/form`, `/hackathon`, `/counter`) korunur. Eski sertifika linkleri (`/api/certs?certID=...`) yeni doğrulama sayfasına yönlenir.

## Geçiş durumu

- [x] Veritabanı: şema, migration, seed
- [x] Backend API: herkese açık ve admin uçları, JWT, istek sınırı, Excel, sertifika görseli, Docker
- [x] Angular ön yüz: site ve yönetim paneli
- [x] Tek Docker kurulumu: nginx ön yüzü sunar, `/api` isteklerini API'ye yönlendirir
- [x] Sitenin içeriğini panelden yönetmek
- [x] KVKK: başvuruları panelden silme
- [x] Yöneticiler ve şifre değiştirme, sertifikada Excel'den toplu ekleme ve QR kod, sürükle-bırak sıralama
- [x] Yedekleme betikleri, testler ve CI
- [ ] Yayına alma ve diğer işler: [YAPILACAKLAR.md](YAPILACAKLAR.md)
