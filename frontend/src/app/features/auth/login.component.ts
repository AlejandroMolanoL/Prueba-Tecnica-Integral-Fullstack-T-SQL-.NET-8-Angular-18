import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { AuthStore } from '../../core/state/auth.store';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  public readonly authStore = inject(AuthStore);
  private readonly router = inject(Router);
  private readonly notificationService = inject(NotificationService);

  public loginForm: FormGroup = this.fb.group({
    correoElectronico: ['', [Validators.required, Validators.email]],
    contrasena: ['', [Validators.required, Validators.minLength(6)]],
    terminosCondiciones: [false, [Validators.requiredTrue]]
  });

  public isSubmitted = false;

  public onSubmit(): void {
    this.isSubmitted = true;

    if (this.loginForm.invalid) {
      if (!this.loginForm.get('terminosCondiciones')?.value) {
        this.notificationService.warning('Debe aceptar los términos y condiciones antes de iniciar sesión.');
      } else {
        this.notificationService.warning('Por favor complete todos los campos requeridos correctamente.');
      }
      return;
    }

    const { correoElectronico, contrasena } = this.loginForm.value;

    this.authService.login(correoElectronico, contrasena).subscribe({
      next: (response) => {
        if (response.success) {
          this.router.navigate(['/comerciantes']);
        }
      }
    });
  }

  // Métodos de ayuda para las credenciales de prueba rápida
  public llenarCredenciales(rol: 'admin' | 'auxiliar'): void {
    if (rol === 'admin') {
      this.loginForm.patchValue({
        correoElectronico: 'admin@olsoftware.com',
        contrasena: 'Admin123*',
        terminosCondiciones: true
      });
    } else {
      this.loginForm.patchValue({
        correoElectronico: 'auxiliar@olsoftware.com',
        contrasena: 'Auxiliar123*',
        terminosCondiciones: true
      });
    }
  }
}
