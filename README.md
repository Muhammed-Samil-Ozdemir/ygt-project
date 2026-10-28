# YGT Project

KTÜN Yazılım Geliştirme Topluluğu sitesinin yeni sürümü. Eski Node.js/Express API'nin ve React ön yüzün yerini alır.

| Klasör / dosya | İçerik |
|---|---|
| `server/` | ASP.NET Core 10 Web API (`Ygt.WebAPI`), EF Core + SQL Server |
| `client/` | Angular ön yüz (sonraki aşamada) |
| `docker-compose.yml` | SQL Server + API; ön yüz sonraki aşamada eklenecek |

## Çalıştırma

### Docker ile

```bash
cp .env.example .env          # yerel geliştirme değerleri hazır gelir; sunucuda hepsini değiştirin
docker compose up -d --build
```

API `http://localhost:5080` adresinde açılır (ön yüz eklenene kadar geçici). Sağlık kontrolü: `/health`.
Admin girişi `.env` içindeki `ADMIN_EMAIL` ve `ADMIN_PASSWORD` ile yapılır.

### Geliştirme (API Docker dışında)

```bash
docker compose up -d mssql

cd server
dotnet tool restore           # dotnet-ef
dotnet user-secrets set "Seed:Admin:Password" "<admin şifresi>" --project Ygt.WebAPI
dotnet run --project Ygt.WebAPI
```

API `http://localhost:5146` adresinde açılır. OpenAPI dokümanı: `http://localhost:5146/openapi/v1.json` (sadece geliştirmede).

API her açılışta bekleyen migration'ları uygular ve başlangıç verilerini ekler: admin kullanıcısı, site ayarları ve ana sayfa ziyaret sayacı.
Ayardaki admin şifresi değişirse bir sonraki açılışta veritabanındaki şifre de güncellenir.

## API

Hatalarda gövde her zaman `{ "message": "..." }` biçimindedir ve mesaj Türkçedir, ön yüz doğrudan gösterebilir.
Admin uçları `Authorization: Bearer <token>` başlığını ister. Token `POST /auth/login` ile alınır ve 4 saat geçerlidir.

### Herkese açık

| Metot | Adres | Açıklama |
|---|---|---|
| POST | `/auth/login` | `{ email, password }` → `{ token, expiresAt }` |
| GET | `/site/settings` | Ana sayfa butonu (`course` / `form`) ve geri sayım |
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
| GET, POST | `/certificates` | Tüm sertifikalar, yeni sertifika (doğrulama kodu otomatik) |
| PUT, DELETE | `/certificates/{id}` | Düzenle, sil |
| PUT | `/certificates/{id}/image` | Görsel yükle (`multipart/form-data`, alan adı `file`, PNG/JPG/WEBP, en fazla 5 MB) |

### Kurallar

- Telefonlar `5551234567` biçimine çevrilip saklanır. `0555 123 45 67`, `+90 555...` gibi yazımların hepsi kabul edilir ve aynı numara sayılır.
- Aynı eğitime aynı e-posta, telefon veya TC ile ikinci başvuru yapılamaz. Etkinlik formunda bu kontrol telefon veya e-postayla yapılır.
- Hackathon: takım adı formda tektir. Üye e-postaları `.edu` içermeli. Bir kişi aynı forma iki takımla başvuramaz. 3. üyenin bir alanı doluysa hepsi zorunludur.
- Aynı anda tek aktif form olur. Yeni form açmak ya da formu yeniden açmak, açık olan formu kapatır.
- Eğitim oluşturmak ana sayfa butonunu `course`, form açmak `form` yapar. Form kapanınca buton `course`'a döner (eski sistemdeki davranış).
- İstek sınırı (IP başına, dakikada): 60 başvuru, 5 giriş denemesi. `appsettings.json` > `RateLimiting` ile değiştirilebilir.

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
  Abstractions/     BaseEntity (Id, CreatedAt, UpdatedAt)
  Models/           Entity'ler
  Enums/            FormType, HomeButton
  Configurations/   EF Core tablo ayarları (IEntityTypeConfiguration)
  Context/          AppDbContext, açılışta migration + seed
  Migrations/
  Dtos/             İstek ve yanıt kayıtları (record)
  Services/         İş kuralları; DbContext'i doğrudan kullanır
  Controllers/      İnce katman: servisi çağırır
  Options/          JWT ve dosya saklama ayarları
  Providers/        JWT ve Excel üretimi
  Middlewares/      Hataları { message } olarak döndüren handler
  Extensions/       Controller, hata biçimi ve istek sınırı kurulumu
  Utilities/        Girdi doğrulama, Türkiye saati
```

## Geçiş durumu

- [x] Veritabanı: şema, migration, seed
- [x] Backend API: herkese açık ve admin uçları, JWT, istek sınırı, Excel, sertifika görseli, Docker
- [ ] Angular ön yüz (site + admin paneli)
- [ ] Tek Docker kurulumu: nginx ön yüzü sunar, `/api` isteklerini API'ye yönlendirir
