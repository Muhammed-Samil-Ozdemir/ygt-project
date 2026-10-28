export interface Course {
  id: string;
  title: string;
  content: string | null;
}

export interface CourseListItem extends Course {
  isActive: boolean;
  applicationCount: number;
  createdAt: string;
  updatedAt: string | null;
}

export interface CourseCommand {
  title: string;
  content: string;
}

export interface CourseApplicationCommand {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
}

export interface CourseApplication {
  id: string;
  firstName: string;
  lastName: string;
  tcNumber: string | null;
  email: string;
  phoneNumber: string | null;
  birthDate: string | null;
  createdAt: string;
}
