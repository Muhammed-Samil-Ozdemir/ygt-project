# Yapılacaklar

Veritabanı, API, site, yönetim paneli, Docker kurulumu, sitenin içeriğinin panelden yönetilmesi, hazır başlangıç içeriği,
KVKK silme işleri, yönetici yönetimi, sertifika araçları, yedekleme, testler ve CI tamam. Kalan işler önem sırasına göre aşağıda.

## Tamamlananlar

- [x] **Sitenin içeriğini panelden yönetmek.** Logo, sekme simgesi, giriş görseli, bütün metinler (giriş bölümü, bölüm başlıkları, "Bize katıl", resmi üyelik kartı, oyunlar bölümü, alt bilgi, başvuru sayfaları, teşekkür ve KVKK metni, sertifika notu, sayfa başlığı ve açıklaması), hakkımızda kartları (görselli), etkinlikler (fotoğraflı; pano, öne çıkan ve geleneksel görünümleri), ekip (fotoğraf, sıralama, gizleme), teknolojiler ve bağlantılar panelde "Site içeriği" bölümünden yönetiliyor. Kodda sadece arayüz yazıları (form etiketleri, butonlar gibi) var.
  - Sayfa başlığı ve açıklaması site açıldıktan sonra JavaScript ile değişiyor. Google bunu okuyor, ama bağlantı önizlemesi gösteren bazı uygulamalar (WhatsApp vb.) `web/apps/ui/src/index.html`'deki başlığı gösterebilir; topluluk adı değişirse orası da elle değiştirilmeli.
- [x] `client/` klasörünü sil (Nx şablonundan kalma).
- [x] **KVKK: başvuruların silinmesi.** Panelde başvurular tek tek ya da toplu silinebiliyor. Otomatik silme yok.
- [x] **Yedekleme betikleri.** `scripts/backup.sh` veritabanını (.bak) ve görselleri tarihli klasöre alıyor, eski yedekleri siliyor; `scripts/restore.sh` geri yüklüyor. İkisi de denendi. Sunucuda cron'a eklenmesi aşağıda.
- [x] **Admin yönetimi.** Birden fazla yönetici, panelden yönetici ekleme, silme, başkasına şifre belirleme ve "Hesabım"dan kendi şifresini değiştirme. Silinen yöneticinin token'ı hemen geçersiz olur. `.env`'deki şifre artık sadece ilk kurulumda kullanılıyor; unutulursa `ADMIN_RESET_PASSWORD=true` ile sıfırlanıyor.
- [x] **Sertifikalar.** Excel şablonundan toplu ekleme (hatalı satır varsa hiçbiri eklenmez), sertifikaları doğrulama linkleriyle Excel'e aktarma, doğrulama linkinin QR kodunu PNG olarak indirme.
- [x] **Site içeriği: sürükle-bırak sıralama** (hakkımızda, etkinlikler, ekip, teknolojiler, bağlantılar). Dokunmatik ekranda oklar kullanılıyor.
- [x] **Testler ve CI.** `server/Ygt.WebAPI.Tests`: girdi doğrulama birim testleri ve gerçek SQL Server'la (Testcontainers) entegrasyon testleri. `.github/workflows/ci.yml`: API testleri, ön yüz derlemesi, Docker imajları.
- [x] **Hazır başlangıç içeriği.** İlk kurulumda (production dahil) eski sitenin içeriği gelir: metinler (giriş, "Bize katıl", alt bilgi, KVKK, sertifika notu, sayfa başlığı ve açıklaması), resmi üyelik ve oyunlar bağlantıları, logo ve sekme simgesi, hakkımızda kartları, teknolojiler, sosyal medya bağlantıları. Ekip ve etkinlikler boş başlar. Geliştirmede (ya da `.env`'de `SEED_SAMPLE_CONTENT=true` ile) ek olarak örnek veri gelir: eski ekip ve galeri fotoğraflarıyla etkinlikler, örnek eğitim, formlar, başvurular, sertifikalar. İçerik sadece veritabanı ilk kurulurken bir kez eklenir; panelden silinen geri gelmez, mevcut kurulumlar değişmez.
- [x] **Eski Node sistemindeki açık (kod).** `getAllAppeal` admin kontrolüne alındı (`ygt-api-main/restAPI/index.js`). Bu dosya bu reponun dışında; eski sunucuya ayrıca yüklenmesi gerekiyor (aşağıda).

## 1. Yayına almadan önce

- [ ] Sunucuya kurulum: Docker ile kurulum, HTTPS.
- [ ] Site içeriğini panelden tamamla. Hazır gelenleri kontrol et: "Bize katıl" Google Form bağlantısı hâlâ geçerli mi, KVKK metni güncel mi. Boş başlayanları gir: güncel ekip ve fotoğrafları, geçmiş etkinlikler ve fotoğrafları. İstenirse üyelik ve oyunlar bölümlerine açıklama, oyunlara görsel eklenebilir. Eski ekip ve galeri fotoğrafları `server/Ygt.WebAPI/SeedData/sample` altında duruyor.
- [ ] `.env` içindeki geliştirme değerlerini değiştir: `MSSQL_SA_PASSWORD`, `JWT_SECRET_KEY`, `ADMIN_PASSWORD`.
- [ ] Alan adını (`yazilimgelistirme.ktun.edu.tr`) eski Node sitesinden yeni sisteme geçir.
- [ ] Yedeklemeyi sunucuda zamanla: `crontab -e` ile `0 3 * * * cd /opt/ygt-project && bash scripts/backup.sh >> backups/backup.log 2>&1`. Yedekleri sunucu dışına da kopyala.
- [ ] Eski Node sistemi geçiş gününe kadar yayında kalacaksa:
  - `getAllAppeal` düzeltmesini (`restAPI/index.js`) eski sunucuya yükle.
  - Repoda açıkta duran şifreleri (veritabanı, JWT anahtarı, admin) değiştir.
- [ ] Hackathon KVKK metni "verileriniz en fazla 6 ay saklanacak" diyor, ama başvurular otomatik silinmiyor. Ya metin panelden güncellenmeli ya da süresi dolan başvurular panelden elle silinmeli.
- [ ] Repo GitHub'a gönderildiğinde CI'ın ilk çalışmasının geçtiğini kontrol et (Actions sekmesi).

## 2. İsteğe bağlı iyileştirmeler

- [ ] **Veri taşıma:** Eski MySQL verisinin (özellikle sertifikaların) taşınması. Altyapı hazır: eski sertifika ID'leri `Certificates.Code` sütununa yazılırsa eski doğrulama linkleri de çalışır.
- [ ] **Excel'i e-postayla gönderme:** Eski sistemde vardı, yeni sistemde başvurular doğrudan indiriliyor. İstenirse eklenebilir (SMTP bilgisi gerekir).
- [ ] **Site içeriği: metinlerde değişiklik geçmişi** (eski bir metne geri dönebilmek için; yeni tablo ve migration gerekir).
- [ ] **Şifre değişince eski oturumların kapanması:** Şu an şifre değişse de önceden alınmış token süresi (4 saat) dolana kadar geçerli; silinen yöneticinin token'ı ise hemen geçersiz oluyor.
