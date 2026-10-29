# Yapılacaklar

Veritabanı, API, site, yönetim paneli, Docker kurulumu, sitenin içeriğinin panelden yönetilmesi ve KVKK silme işleri tamam. Kalan işler önem sırasına göre aşağıda.

## Tamamlananlar

- [x] **Sitenin içeriğini panelden yönetmek.** Logo, sekme simgesi, giriş görseli, bütün metinler (giriş bölümü, bölüm başlıkları, "Bize katıl", resmi üyelik kartı, oyunlar bölümü, alt bilgi, başvuru sayfaları, teşekkür ve KVKK metni, sertifika notu, sayfa başlığı ve açıklaması), hakkımızda kartları (görselli), ekip (fotoğraf, sıralama, gizleme), teknolojiler ve bağlantılar panelde "Site içeriği" bölümünden yönetiliyor. İçerik boş başlıyor, hazır veri ya da görsel yok. Kodda sadece arayüz yazıları (form etiketleri, butonlar gibi) var.
  - Sayfa başlığı ve açıklaması site açıldıktan sonra JavaScript ile değişiyor. Google bunu okuyor, ama bağlantı önizlemesi gösteren bazı uygulamalar (WhatsApp vb.) `web/apps/ui/src/index.html`'deki başlığı gösterebilir; topluluk adı değişirse orası da elle değiştirilmeli.
- [x] `client/` klasörünü sil (Nx şablonundan kalma).
- [x] **KVKK: başvuruların silinmesi.** Panelde başvurular tek tek ya da toplu silinebiliyor. Otomatik silme yok.

## 1. Yayına almadan önce

- [ ] Sunucuya kurulum: Docker ile kurulum, HTTPS.
- [ ] Site içeriğini panelden gir: logo, sekme simgesi, metinler, KVKK metni, resmi üyelik bağlantısı (`https://topluluk.ktun.edu.tr`), oyunlar bağlantısı (`https://ygt-mini-oyunlar.gt.tc`), ekip ve fotoğrafları, kartlar, teknolojiler, bağlantılar. Eski metinler ve KVKK metni `872b38b` commit'indeki `server/Ygt.WebAPI/Utilities/SiteTextCatalog.cs`'te, ekip fotoğrafları aynı commit'te `server/Ygt.WebAPI/SeedData/team` altında duruyor.
- [ ] `.env` içindeki geliştirme değerlerini değiştir: `MSSQL_SA_PASSWORD`, `JWT_SECRET_KEY`, `ADMIN_PASSWORD`.
- [ ] Alan adını (`yazilimgelistirme.ktun.edu.tr`) eski Node sitesinden yeni sisteme geçir.
- [ ] Yedekleme: `mssql-data` (veritabanı) ve `api-storage` (sertifika görselleri, panelden yüklenen site görselleri) volume'ları için düzenli yedek.
- [ ] Eski Node sistemi geçiş gününe kadar yayında kalacaksa:
  - `GET /getAllAppeal` başvuranların TC ve telefon bilgisini herkese açık döndürüyor; admin kontrolüne alınmalı.
  - Repoda açıkta duran şifreler (veritabanı, JWT anahtarı, admin) değiştirilmeli.
- [ ] Hackathon KVKK metni "verileriniz en fazla 6 ay saklanacak" diyor, ama başvurular otomatik silinmiyor. Ya metin panelden güncellenmeli ya da süresi dolan başvurular panelden elle silinmeli.

## 2. İsteğe bağlı iyileştirmeler

- [ ] **Testler ve CI:** Backend için entegrasyon testleri, GitHub Actions ile derleme ve Docker imajı kontrolü.
- [ ] **Sertifikalar:** Excel'den toplu ekleme, doğrulama linkinden QR kod üretimi.
- [ ] **Admin yönetimi:** Birden fazla admin, panelden şifre değiştirme (şu an tek admin var, şifresi `.env`'den geliyor).
- [ ] **Veri taşıma:** Eski MySQL verisinin (özellikle sertifikaların) taşınması. Altyapı hazır: eski sertifika ID'leri `Certificates.Code` sütununa yazılırsa eski doğrulama linkleri de çalışır.
- [ ] **Excel'i e-postayla gönderme:** Eski sistemde vardı, yeni sistemde başvurular doğrudan indiriliyor. İstenirse eklenebilir.
- [ ] **Site içeriği:** Sürükle-bırak sıralama (şu an yukarı/aşağı okları var), metinlerde değişiklik geçmişi.
