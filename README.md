# YGT Project

KTÜN Yazılım Geliştirme Topluluğu sitesinin yeni sürümü. Eski Node.js/Express API'nin ve React ön yüzün yerini alır.

| Klasör / dosya | İçerik |
|---|---|
| `server/` | ASP.NET Core 10 Web API (`Ygt.WebAPI`), EF Core + SQL Server |
| `client/` | Angular ön yüz (sonraki aşamalarda) |
| `docker-compose.yml` | Şimdilik sadece SQL Server; API ve ön yüz sonraki aşamalarda eklenecek |

## Yerelde çalıştırma

Gereksinimler: .NET 10 SDK, Docker.

```bash
cp .env.example .env          # yerel geliştirme değerleri hazır gelir
docker compose up -d          # SQL Server

cd server
dotnet tool restore           # dotnet-ef
dotnet user-secrets set "Seed:Admin:Password" "<admin şifresi>" --project Ygt.WebAPI
dotnet run --project Ygt.WebAPI
```

API açılırken bekleyen migration'ları uygular ve başlangıç verilerini ekler: admin kullanıcısı, site ayarları ve ana sayfa ziyaret sayacı.
Admin şifresi ayardan gelir. Ayardaki şifre değişirse bir sonraki açılışta veritabanındaki şifre de güncellenir.

## Veritabanı

| Tablo | Eski tablo (MySQL) | Not |
|---|---|---|
| `Users` | `users` | Sadece adminler. Eski bcrypt özetleri aynen çalışır. |
| `Courses` | `courses` | |
| `CourseApplications` | `appeals` | `Courses`'a bağlı. Aynı kursa aynı e-postayla tek başvuru. |
| `Forms` | `forms` | `Type` (Standard / Hackathon) eklendi. Aynı anda en fazla bir aktif form. |
| `FormApplications` | `formAppeals` | Aynı forma aynı telefonla tek başvuru. |
| `HackathonApplications` | `hackathonForms` | Aynı formda takım adı tekrar edemez. |
| `Certificates` | `certificates` | Eski sertifika ID'leri `Code` sütununda, doğrulama linkleri bunu kullanır. |
| `SiteSettings` | `activePanels` + `activityCounters` | Tek satır: ana sayfa butonu ve geri sayım. |
| `VisitorCounters` | `counters` | |
| — | `sessions` | Kaldırıldı. Form spam koruması başka yolla yapılacak. |

Yeni migration eklemek için:

```bash
cd server
dotnet ef migrations add <migration_adi> --project Ygt.WebAPI
```

Not: `sqlcmd` ile veri yazarken `-I` parametresi gerekir. `Forms` tablosundaki filtreli index `QUOTED_IDENTIFIER` ayarının açık olmasını istiyor.

## Geçiş durumu

- [x] Veritabanı: şema, migration, seed, Docker'da SQL Server
