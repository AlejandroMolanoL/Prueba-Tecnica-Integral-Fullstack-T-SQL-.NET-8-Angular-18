import { Injectable, computed, signal } from '@angular/core';
import { LoginResponse, User } from '../models/user.model';

interface AuthState {
  user: User | null;
  token: string | null;
  isLoading: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AuthStore {
  private readonly TOKEN_KEY = 'ol_auth_token';
  private readonly USER_KEY = 'ol_auth_user';

  private readonly state = signal<AuthState>({
    user: null,
    token: null,
    isLoading: false
  });

  // Selectores computados (Estado Global Reactivo)
  public readonly user = computed(() => this.state().user);
  public readonly token = computed(() => this.state().token);
  public readonly isLoading = computed(() => this.state().isLoading);
  public readonly isAuthenticated = computed(() => !!this.state().token);
  public readonly isAdmin = computed(() => this.state().user?.rol === 'Administrador');
  public readonly userDisplayName = computed(() => this.state().user?.nombre || 'Usuario');
  public readonly userRole = computed(() => this.state().user?.rol || '');

  constructor() {
    this.initFromStorage();
  }

  public setLogin(response: LoginResponse): void {
    sessionStorage.setItem(this.TOKEN_KEY, response.token);
    sessionStorage.setItem(this.USER_KEY, JSON.stringify(response.usuario));

    this.state.set({
      user: response.usuario,
      token: response.token,
      isLoading: false
    });
  }

  public logout(): void {
    sessionStorage.removeItem(this.TOKEN_KEY);
    sessionStorage.removeItem(this.USER_KEY);

    this.state.set({
      user: null,
      token: null,
      isLoading: false
    });
  }

  public setLoading(loading: boolean): void {
    this.state.update(s => ({ ...s, isLoading: loading }));
  }

  public getToken(): string | null {
    return this.state().token;
  }

  private initFromStorage(): void {
    const token = sessionStorage.getItem(this.TOKEN_KEY);
    const userJson = sessionStorage.getItem(this.USER_KEY);

    if (token && userJson) {
      try {
        const user = JSON.parse(userJson) as User;
        this.state.set({
          user,
          token,
          isLoading: false
        });
      } catch {
        this.logout();
      }
    }
  }
}

