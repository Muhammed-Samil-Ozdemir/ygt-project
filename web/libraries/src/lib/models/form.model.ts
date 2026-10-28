export type FormType = 'standard' | 'hackathon';

export interface Form {
  id: string;
  title: string;
  content: string | null;
  type: FormType;
}

export interface FormListItem extends Form {
  isActive: boolean;
  applicationCount: number;
  createdAt: string;
  updatedAt: string | null;
}

export interface FormCommand {
  title: string;
  content: string;
  type: FormType;
}

export interface FormApplicationCommand {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
}

export interface FormApplication {
  id: string;
  firstName: string;
  lastName: string;
  email: string | null;
  phoneNumber: string;
  createdAt: string;
}

export interface HackathonMember {
  name: string;
  phone: string;
  email: string;
}

export interface HackathonApplicationCommand {
  teamName: string;
  member1: HackathonMember;
  member2: HackathonMember;
  /** İsteğe bağlı: alanları boş gönderilirse takım 2 kişi sayılır. */
  member3: HackathonMember;
}

export interface HackathonApplication {
  id: string;
  teamName: string;
  member1: HackathonMember;
  member2: HackathonMember;
  member3: HackathonMember | null;
  createdAt: string;
}
