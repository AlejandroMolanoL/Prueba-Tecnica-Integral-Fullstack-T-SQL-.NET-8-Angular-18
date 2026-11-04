export interface Comerciante {
  idComerciante: number;
  nombreRazonSocial: string;
  idMunicipio: number;
  municipioNombre: string;
  telefono?: string;
  correoElectronico?: string;
  fechaRegistro: string;
  estado: 'Activo' | 'Inactivo' | string;
  fechaActualizacion?: string;
  usuarioActualizacion?: string;
  cantidadEstablecimientos: number;
  totalIngresos: number;
  cantidadEmpleados: number;
}

export interface CrearComerciante {
  nombreRazonSocial: string;
  idMunicipio: number;
  telefono?: string;
  correoElectronico?: string;
  fechaRegistro: string;
  estado: string;
}

export interface ActualizarComerciante {
  nombreRazonSocial: string;
  idMunicipio: number;
  telefono?: string;
  correoElectronico?: string;
  fechaRegistro: string;
  estado: string;
}

export interface ComercianteFiltros {
  pageNumber: number;
  pageSize: number;
  nombreRazonSocial?: string;
  fechaRegistro?: string;
  estado?: string;
}

