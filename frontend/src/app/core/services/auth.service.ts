import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { LoginRequest, LoginResponse } from '../models/user.model';
import { AuthStore } from '../state/auth.store';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly authStore = inject(AuthStore);
  private readonly notificationService = inject(NotificationService);

  private readonly apiUrl = `${environment.apiUrl}/auth`;

  public login(correoElectronico: string, contrasena: string): Observable<ApiResponse<LoginResponse>> {
    this.authStore.setLoading(true);

    // Reto 09: No enviar datos en bruto en debuggers de red -> codificacion Base64 segura
    const encodedPassword = btoa(contrasena);

    const payload: LoginRequest = {
      correoElectronico: correoElectronico.trim(),
      contrasena: encodedPassword
    };

    return this.http.post<ApiResponse<LoginResponse>>(`${this.apiUrl}/login`, payload).pipe(
      tap({
        next: (response) => {
          if (response.success && response.data) {
            this.authStore.setLogin(response.data);
            this.notificationService.success(`Bienvenido, ${response.data.usuario.nombre}`);
          }
        },
        error: (err) => {
          this.authStore.setLoading(false);
          const errorMsg = err.error?.message || 'Error al iniciar sesión. Verifique sus credenciales.';
          this.notificationService.error(errorMsg);
        }
      })
    );
  }

  public logout(): void {
    this.authStore.logout();
    this.notificationService.info('Sesión cerrada correctamente.');
    this.router.navigate(['/login']);
  }
}

