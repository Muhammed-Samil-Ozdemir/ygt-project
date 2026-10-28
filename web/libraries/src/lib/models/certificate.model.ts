export interface Certificate {
  code: string;
  courseName: string;
  firstName: string;
  lastName: string;
  hasImage: boolean;
}

export interface CertificateListItem extends Certificate {
  id: string;
  email: string | null;
  phoneNumber: string | null;
  createdAt: string;
}

export interface CertificateCommand {
  courseName: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
}
