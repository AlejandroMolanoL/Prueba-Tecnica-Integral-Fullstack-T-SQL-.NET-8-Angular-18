import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import { ActualizarComerciante, Comerciante, ComercianteFiltros, CrearComerciante } from '../models/comerciante.model';
import { ComerciantesStore } from '../state/comerciantes.store';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root'
})
export class ComerciantesService {
  private readonly http = inject(HttpClient);
  private readonly store = inject(ComerciantesStore);
  private readonly notificationService = inject(NotificationService);

  private readonly apiUrl = `${environment.apiUrl}/comerciantes`;

  public getPaginado(filtros: ComercianteFiltros): Observable<ApiResponse<PagedResult<Comerciante>>> {
    this.store.setLoading(true);

    let params = new HttpParams()
      .set('pageNumber', filtros.pageNumber.toString())
      .set('pageSize', filtros.pageSize.toString());

    if (filtros.nombreRazonSocial && filtros.nombreRazonSocial.trim()) {
      params = params.set('nombreRazonSocial', filtros.nombreRazonSocial.trim());
    }

    if (filtros.fechaRegistro && filtros.fechaRegistro.trim()) {
      params = params.set('fechaRegistro', filtros.fechaRegistro.trim());
    }

    if (filtros.estado && filtros.estado.trim()) {
      params = params.set('estado', filtros.estado.trim());
    }

    return this.http.get<ApiResponse<PagedResult<Comerciante>>>(this.apiUrl, { params }).pipe(
      tap({
        next: (response) => {
          if (response.success && response.data) {
            this.store.setPagedResult(response.data);
          }
        },
        error: (err) => {
          this.store.setLoading(false);
          const errorMsg = err.error?.message || 'Error al consultar comerciantes.';
          this.notificationService.error(errorMsg);
        }
      })
    );
  }

  public getPorId(id: number): Observable<ApiResponse<Comerciante>> {
    return this.http.get<ApiResponse<Comerciante>>(`${this.apiUrl}/${id}`);
  }

  public crear(dto: CrearComerciante): Observable<ApiResponse<Comerciante>> {
    return this.http.post<ApiResponse<Comerciante>>(this.apiUrl, dto).pipe(
      tap({
        next: () => this.notificationService.success('Comerciante creado exitosamente.'),
        error: (err) => {
          const errorMsg = err.error?.message || 'Error al crear comerciante.';
          this.notificationService.error(errorMsg);
        }
      })
    );
  }

  public actualizar(id: number, dto: ActualizarComerciante): Observable<ApiResponse<Comerciante>> {
    return this.http.put<ApiResponse<Comerciante>>(`${this.apiUrl}/${id}`, dto).pipe(
      tap({
        next: () => this.notificationService.success('Comerciante actualizado exitosamente.'),
        error: (err) => {
          const errorMsg = err.error?.message || 'Error al actualizar comerciante.';
          this.notificationService.error(errorMsg);
        }
      })
    );
  }

  public modificarEstado(id: number, nuevoEstado: string): Observable<ApiResponse<Comerciante>> {
    return this.http.patch<ApiResponse<Comerciante>>(`${this.apiUrl}/${id}/estado`, { estado: nuevoEstado }).pipe(
      tap({
        next: () => {
          this.store.updateEstadoLocal(id, nuevoEstado);
          this.notificationService.success(`Estado del comerciante actualizado a ${nuevoEstado}.`);
        },
        error: (err) => {
          const errorMsg = err.error?.message || 'Error al cambiar estado del comerciante.';
          this.notificationService.error(errorMsg);
        }
      })
    );
  }

  public eliminar(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`).pipe(
      tap({
        next: () => {
          this.store.removeComercianteLocal(id);
          this.notificationService.success('Comerciante eliminado exitosamente.');
        },
        error: (err) => {
          const errorMsg = err.error?.message || 'Error al eliminar comerciante.';
          this.notificationService.error(errorMsg);
        }
      })
    );
  }

  public descargarReporteCsv(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/reporte-csv`, {
      responseType: 'blob'
    }).pipe(
      tap({
        next: (blob) => {
          // Disparar descarga en el navegador
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          a.download = `Reporte_Comerciantes_Activos_${new Date().toISOString().slice(0, 10)}.csv`;
          document.body.appendChild(a);
          a.click();
          document.body.removeChild(a);
          window.URL.revokeObjectURL(url);
          this.notificationService.success('Reporte CSV descargado con éxito.');
        },
        error: () => {
          this.notificationService.error('Error al descargar el reporte CSV.');
        }
      })
    );
  }
}
