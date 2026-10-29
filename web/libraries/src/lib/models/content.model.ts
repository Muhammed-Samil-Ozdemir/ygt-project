/**
 * Sitenin panelden değiştirilen tek parça metinleri ve görselleri. Anahtarlar API'deki SiteTextCatalog ile aynı;
 * katalogda yeni alan tanımlanırsa buraya da eklenmeli. Boş bırakılmış metin "" gelir; genel bir ifadesi varsa
 * (ör. bölüm başlığı "Hakkımızda") onun yerine o gelir. Görsellerde değer dosya adıdır, adresi siteImageUrl() verir.
 */
export interface SiteTexts {
  siteName: string;
  /** Görsel */
  logo: string;
  /** Görsel */
  favicon: string;
  seoTitle: string;
  seoDescription: string;
  heroEyebrow: string;
  heroTitle: string;
  heroLead: string;
  heroSecondaryButton: string;
  /** Görsel */
  heroImage: string;
  aboutTitle: string;
  teamTitle: string;
  technologiesTitle: string;
  linksTitle: string;
  joinUrl: string;
  joinButton: string;
  joinTitle: string;
  joinText: string;
  membershipUrl: string;
  membershipTitle: string;
  membershipText: string;
  membershipButton: string;
  gamesUrl: string;
  gamesTitle: string;
  gamesText: string;
  gamesButton: string;
  /** Görsel */
  gamesImage: string;
  footerText: string;
  universityName: string;
  universityUrl: string;
  courseButton: string;
  formButton: string;
  noCourseText: string;
  applicationThanks: string;
  kvkkText: string;
  certificateNote: string;
  counterIdleTitle: string;
}

export interface AboutCard {
  title: string;
  text: string;
  /** Görselin dosya adı; adresi siteImageUrl() verir. Görsel yoksa null. */
  image: string | null;
}

export interface TeamMember {
  name: string;
  role: string;
  /** Fotoğrafın dosya adı; adresi siteImageUrl() verir. Fotoğraf yoksa null. */
  photo: string | null;
}

export interface SiteLink {
  label: string;
  url: string;
}

/** Sitenin bütün içeriği (GET /site/content). Sadece görünür kayıtlar, panelde verilen sırayla gelir. */
export interface SiteContent {
  texts: SiteTexts;
  about: AboutCard[];
  team: TeamMember[];
  technologies: string[];
  links: SiteLink[];
}

/** Panelden yüklenen görselin (logo, ekip fotoğrafı, kart görseli...) adresi. */
export function siteImageUrl(fileName: string): string {
  return `/api/site/content/images/${encodeURIComponent(fileName)}`;
}

// Yönetim paneli

export type SiteTextKind = 'text' | 'longText' | 'url' | 'image';

/** Panelde bir metin ya da görsel alanı: tanımı ve kayıtlı değeri ("" ise boş; görselde dosya adı). */
export interface SiteTextField {
  key: keyof SiteTexts;
  group: string;
  label: string;
  hint: string | null;
  kind: SiteTextKind;
  /** Görsellerde 0. */
  maxLength: number;
  /** Boş bırakılırsa sitede yazılan genel ifade. */
  placeholder: string | null;
  value: string;
}

export interface SiteTextsCommand {
  values: Partial<Record<keyof SiteTexts, string>>;
}

export interface AboutCardListItem extends AboutCard {
  id: string;
  isVisible: boolean;
}

export interface AboutCardCommand {
  title: string;
  text: string;
  isVisible: boolean;
}

export interface TeamMemberListItem extends TeamMember {
  id: string;
  isVisible: boolean;
}

export interface TeamMemberCommand {
  name: string;
  role: string;
  isVisible: boolean;
}

export interface TechnologyListItem {
  id: string;
  name: string;
}

export interface TechnologyCommand {
  name: string;
}

export interface SiteLinkListItem extends SiteLink {
  id: string;
  isVisible: boolean;
}

export interface SiteLinkCommand {
  label: string;
  url: string;
  isVisible: boolean;
}

/** Listenin yeni sırası: kayıt ID'leri ilk sıradan son sıraya. */
export interface OrderCommand {
  ids: string[];
}
