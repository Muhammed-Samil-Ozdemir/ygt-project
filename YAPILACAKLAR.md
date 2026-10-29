# Yapılacaklar

Veritabanı, API, site, yönetim paneli, Docker kurulumu, sitenin içeriğinin panelden yönetilmesi ve KVKK silme işleri tamam. Kalan işler önem sırasına göre aşağıda.

## Tamamlananlar

- [x] **Sitenin içeriğini panelden yönetmek.** Metinler (giriş bölümü, "Bize katıl", alt bilgi, teşekkür ve KVKK metni, sayfa başlığı ve açıklaması), hakkımızda kartları, ekip (fotoğraf, sıralama, gizleme), teknolojiler ve bağlantılar panelde "Site içeriği" bölümünden yönetiliyor. Bugünkü içerik ilk kurulumda veritabanına yazılıyor. Kodda logo ve arayüz yazıları (form etiketleri, bölüm başlıkları gibi) dışında sabit içerik kalmadı.
  - Sayfa başlığı ve açıklaması site açıldıktan sonra JavaScript ile değişiyor. Google bunu okuyor, ama bağlantı önizlemesi gösteren bazı uygulamalar (WhatsApp vb.) `index.html`'deki varsayılan değeri gösterebilir.
- [x] `client/` klasörünü sil (Nx şablonundan kalma).
- [x] **KVKK: başvuruların silinmesi.** Panelde başvurular tek tek ya da toplu silinebiliyor. Otomatik silme yok.

## 1. Yayına almadan önce

- [ ] Sunucuya kurulum: Docker ile kurulum, HTTPS.
- [ ] `.env` içindeki geliştirme değerlerini değiştir: `MSSQL_SA_PASSWORD`, `JWT_SECRET_KEY`, `ADMIN_PASSWORD`.
- [ ] Alan adını (`yazilimgelistirme.ktun.edu.tr`) eski Node sitesinden yeni sisteme geçir.
- [ ] Yedekleme: `mssql-data` (veritabanı) ve `api-storage` (sertifika görselleri, ekip fotoğrafları) volume'ları için düzenli yedek.
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
