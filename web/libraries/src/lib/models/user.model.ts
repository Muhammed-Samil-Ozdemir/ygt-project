export interface UserListItem {
  id: string;
  name: string;
  email: string;
  createdAt: string;
  /** Listeyi isteyen yöneticinin kendisi mi (kendini silemez). */
  isCurrent: boolean;
}

export interface UserCommand {
  name: string;
  email: string;
  password: string;
}

export interface UserUpdateCommand {
  name: string;
  email: string;
}

export interface CurrentUser {
  id: string;
  name: string;
  email: string;
}

export interface ChangePasswordCommand {
  currentPassword: string;
  newPassword: string;
}
