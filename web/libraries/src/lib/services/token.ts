const TOKEN_KEY = 'ygt-admin-token';

/** Admin girişinde alınan JWT tarayıcıda saklanır. */
export const tokenStore = {
  get: (): string | null => localStorage.getItem(TOKEN_KEY),
  set: (token: string) => localStorage.setItem(TOKEN_KEY, token),
  clear: () => localStorage.removeItem(TOKEN_KEY),
};
