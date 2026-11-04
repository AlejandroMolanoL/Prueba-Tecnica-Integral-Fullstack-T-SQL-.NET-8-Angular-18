import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([])
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('el formulario debe ser inválido cuando está vacío', () => {
    expect(component.loginForm.valid).toBe(false);
  });

  it('debe validar que el correo tenga formato electrónico válido', () => {
    const emailControl = component.loginForm.get('correoElectronico');
    emailControl?.setValue('correo-invalido');
    expect(emailControl?.valid).toBe(false);
    expect(emailControl?.errors?.['email']).toBeTruthy();

    emailControl?.setValue('admin@olsoftware.com');
    expect(emailControl?.valid).toBe(true);
  });

  it('debe requerir aceptar términos y condiciones para ser válido', () => {
    component.loginForm.patchValue({
      correoElectronico: 'admin@olsoftware.com',
      contrasena: 'Admin123*',
      terminosCondiciones: false
    });

    expect(component.loginForm.valid).toBe(false);

    component.loginForm.patchValue({ terminosCondiciones: true });
    expect(component.loginForm.valid).toBe(true);
  });
});

