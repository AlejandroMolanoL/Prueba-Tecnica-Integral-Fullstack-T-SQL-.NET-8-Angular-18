export interface User {
  idUsuario: number;
  nombre: string;
  correoElectronico: string;
  rol: 'Administrador' | 'Auxiliar de Registro' | string;
}

export interface LoginRequest {
  correoElectronico: string;
  contrasena: string;
}

export interface LoginResponse {
  token: string;
  expiracion: string;
  usuario: User;
}

