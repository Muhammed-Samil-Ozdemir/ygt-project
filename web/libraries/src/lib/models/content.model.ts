/**
 * Sitenin panelden değiştirilen tek parça metinleri. Anahtarlar API'deki SiteTextCatalog ile aynı;
 * katalogda yeni metin tanımlanırsa buraya da eklenmeli. İsteğe bağlı metin boş bırakılmışsa "" gelir.
 */
export interface SiteTexts {
  siteName: string;
  seoTitle: string;
  seoDescription: string;
  heroEyebrow: string;
  heroTitle: string;
  heroLead: string;
  heroSecondaryButton: string;
  joinButton: string;
  joinUrl: string;
  joinTitle: string;
  joinText: string;
  footerText: string;
  universityName: string;
  universityUrl: string;
  applicationThanks: string;
  kvkkText: string;
}

export interface AboutCard {
  title: string;
  text: string;
}

export interface TeamMember {
  name: string;
  role: string;
  /** Fotoğrafın dosya adı; adresi teamPhotoUrl() verir. Fotoğraf yoksa null. */
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

export function teamPhotoUrl(photo: string): string {
  return `/api/site/content/photos/${encodeURIComponent(photo)}`;
}

// Yönetim paneli

export type SiteTextKind = 'text' | 'longText' | 'url';

/** Panelde bir metin alanı: tanımı ve şu anki değeri. */
export interface SiteTextField {
  key: keyof SiteTexts;
  group: string;
  label: string;
  hint: string | null;
  kind: SiteTextKind;
  maxLength: number;
  isRequired: boolean;
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
