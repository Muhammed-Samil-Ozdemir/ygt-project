# Yapılacaklar

Veritabanı, API, site, yönetim paneli ve Docker kurulumu tamam. Kalan işler önem sırasına göre aşağıda.

## 1. Sıradaki aşama: Sitenin içeriğini panelden yönetmek

**Amaç:** Sitedeki bütün içerik (yazılar, fotoğraflar, ekip, bağlantılar) yönetim panelinden değiştirilebilsin ve değişiklik sitede hemen görünsün. Şu an panelden sadece başvurular, eğitimler, formlar, sertifikalar ve iki site ayarı (ana sayfa butonu, geri sayım) yönetiliyor. Geri kalan içerik kodda sabit.

**Şu an kodda sabit olan içerik:**

| İçerik | Dosya |
|---|---|
| Ana sayfa giriş bölümü (üst başlık, açıklama, butonlar) | `web/apps/ui/src/features/home/home.html` |
| Hakkımızda kartları ve "Bize katıl" metni | `web/apps/ui/src/features/home/home-content.ts` |
| Ekip listesi (ad, görev, fotoğraf) | `home-content.ts`, fotoğraflar `web/apps/ui/public/images/team/` |
| Teknoloji etiketleri | `home-content.ts` |
| Bağlantılar (Bize Katılın formu, sosyal medya, KTÜN) | `web/apps/ui/src/constants.ts` |
| Alt bilgi metni | `web/apps/ui/src/layout/layout.html` |
| Hackathon aydınlatma (KVKK) metni | `web/apps/ui/src/features/hackathon-application/kvkk-text.ts` |
| Başvuru sonrası teşekkür metni | `web/apps/ui/src/features/*-application/*.html` |
| Sayfa başlığı ve açıklaması (arama motorları için) | `web/apps/ui/src/index.html` |

**Önerilen yaklaşım** (taslak, aşamaya başlarken birlikte netleştirilecek):

- **Veritabanı:** Liste halindeki içerik için ayrı tablolar: ekip üyeleri (ad, görev, fotoğraf, sıra, görünür mü), hakkımızda kartları, teknolojiler, bağlantılar. Tek parça metinler (giriş başlığı, alt bilgi, KVKK metni, teşekkür metni) için anahtar/değer tablosu.
- **Fotoğraflar:** Sertifika görselleri için yazılan yükleme altyapısı genelleştirilir. Aynı `storage` volume'u kullanılır, boyut sınırı korunur.
- **API:** Site tek istekte bütün içeriği alır (ör. `GET /site/content`). Panel için içerik türü başına ekleme, düzenleme, silme ve sıralama uçları.
- **Panel:** "Site içeriği" bölümü: metinler, ekip (sıralama ve gizleme), hakkımızda, teknolojiler, bağlantılar.
- **Başlangıç verisi:** Bugün kodda duran içerik ilk kurulumda veritabanına yazılır, site boş açılmaz.
- **Güvenlik:** İçerik düz metin olarak saklanır ve gösterilir, HTML kabul edilmez.

**Bitti sayılır:** Logo dışında kodda sabit metin ve fotoğraf kalmaz. Panelden yapılan değişiklik, site yenilendiğinde görünür.

## 2. Yayına almadan önce

- [ ] `client/` klasörünü sil (Nx şablonundan kalma, kullanılmıyor).
- [ ] Sunucuya kurulum: Docker ile kurulum, HTTPS.
- [ ] `.env` içindeki geliştirme değerlerini değiştir: `MSSQL_SA_PASSWORD`, `JWT_SECRET_KEY`, `ADMIN_PASSWORD`.
- [ ] Alan adını (`yazilimgelistirme.ktun.edu.tr`) eski Node sitesinden yeni sisteme geçir.
- [ ] Yedekleme: `mssql-data` (veritabanı) ve `api-storage` (sertifika görselleri) volume'ları için düzenli yedek.
- [ ] Eski Node sistemi geçiş gününe kadar yayında kalacaksa:
  - `GET /getAllAppeal` başvuranların TC ve telefon bilgisini herkese açık döndürüyor; admin kontrolüne alınmalı.
  - Repoda açıkta duran şifreler (veritabanı, JWT anahtarı, admin) değiştirilmeli.

## 3. KVKK

- [ ] **Başvuruların silinmesi.** Hackathon aydınlatma metni verilerin en fazla 6 ay saklanacağını söylüyor, ama sistemde eski başvuruları silen bir özellik yok. Panelde elle silme ve/veya belirli bir süre sonra otomatik silme eklenmeli.

## 4. İsteğe bağlı iyileştirmeler

- [ ] **Testler ve CI:** Backend için entegrasyon testleri, GitHub Actions ile derleme ve Docker imajı kontrolü.
- [ ] **Sertifikalar:** Excel'den toplu ekleme, doğrulama linkinden QR kod üretimi.
- [ ] **Admin yönetimi:** Birden fazla admin, panelden şifre değiştirme (şu an tek admin var, şifresi `.env`'den geliyor).
- [ ] **Veri taşıma:** Eski MySQL verisinin (özellikle sertifikaların) taşınması. Altyapı hazır: eski sertifika ID'leri `Certificates.Code` sütununa yazılırsa eski doğrulama linkleri de çalışır.
- [ ] **Excel'i e-postayla gönderme:** Eski sistemde vardı, yeni sistemde başvurular doğrudan indiriliyor. İstenirse eklenebilir.
