import { TestBed } from '@angular/core/testing';
import { AuthStore } from './auth.store';
import { LoginResponse } from '../models/user.model';

describe('AuthStore', () => {
  let store: AuthStore;

  beforeEach(() => {
    sessionStorage.clear();
    TestBed.configureTestingModule({
      providers: [AuthStore]
    });
    store = TestBed.inject(AuthStore);
  });

  afterEach(() => {
    sessionStorage.clear();
  });

  it('debe inicializarse sin usuario autenticado', () => {
    expect(store.isAuthenticated()).toBe(false);
    expect(store.user()).toBeNull();
    expect(store.token()).toBeNull();
    expect(store.isAdmin()).toBe(false);
  });

  it('debe actualizar el estado global y sessionStorage al invocar setLogin', () => {
    const mockResponse: LoginResponse = {
      token: 'jwt_mock_token',
      expiracion: new Date().toISOString(),
      usuario: {
        idUsuario: 1,
        nombre: 'Admin Tester',
        correoElectronico: 'admin@test.com',
        rol: 'Administrador'
      }
    };

    store.setLogin(mockResponse);

    expect(store.isAuthenticated()).toBe(true);
    expect(store.user()?.nombre).toBe('Admin Tester');
    expect(store.token()).toBe('jwt_mock_token');
    expect(store.isAdmin()).toBe(true);
    expect(sessionStorage.getItem('ol_auth_token')).toBe('jwt_mock_token');
  });

  it('debe limpiar el estado global y sessionStorage al invocar logout', () => {
    const mockResponse: LoginResponse = {
      token: 'jwt_mock_token',
      expiracion: new Date().toISOString(),
      usuario: {
        idUsuario: 2,
        nombre: 'Auxiliar Tester',
        correoElectronico: 'aux@test.com',
        rol: 'Auxiliar de Registro'
      }
    };

    store.setLogin(mockResponse);
    expect(store.isAuthenticated()).toBe(true);

    store.logout();

    expect(store.isAuthenticated()).toBe(false);
    expect(store.user()).toBeNull();
    expect(store.token()).toBeNull();
    expect(sessionStorage.getItem('ol_auth_token')).toBeNull();
  });
});
