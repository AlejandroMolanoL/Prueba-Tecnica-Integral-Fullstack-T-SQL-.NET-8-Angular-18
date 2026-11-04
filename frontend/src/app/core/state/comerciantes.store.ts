import { Injectable, computed, signal } from '@angular/core';
import { Comerciante, ComercianteFiltros } from '../models/comerciante.model';
import { PagedResult } from '../models/api-response.model';

interface ComerciantesState {
  items: Comerciante[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  isLoading: boolean;
  filtros: ComercianteFiltros;
}

@Injectable({
  providedIn: 'root'
})
export class ComerciantesStore {
  private readonly state = signal<ComerciantesState>({
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize: 5,
    totalPages: 0,
    isLoading: false,
    filtros: {
      pageNumber: 1,
      pageSize: 5,
      nombreRazonSocial: '',
      fechaRegistro: '',
      estado: ''
    }
  });

  // Selectores computados (Estado Global)
  public readonly comerciantes = computed(() => this.state().items);
  public readonly totalCount = computed(() => this.state().totalCount);
  public readonly pageNumber = computed(() => this.state().pageNumber);
  public readonly pageSize = computed(() => this.state().pageSize);
  public readonly totalPages = computed(() => this.state().totalPages);
  public readonly isLoading = computed(() => this.state().isLoading);
  public readonly filtros = computed(() => this.state().filtros);

  public setPagedResult(result: PagedResult<Comerciante>): void {
    this.state.update(s => ({
      ...s,
      items: result.items,
      totalCount: result.totalCount,
      pageNumber: result.pageNumber,
      pageSize: result.pageSize,
      totalPages: result.totalPages,
      isLoading: false
    }));
  }

  public setLoading(isLoading: boolean): void {
    this.state.update(s => ({ ...s, isLoading }));
  }

  public setFiltros(filtros: Partial<ComercianteFiltros>): void {
    this.state.update(s => ({
      ...s,
      filtros: { ...s.filtros, ...filtros }
    }));
  }

  public updateEstadoLocal(id: number, nuevoEstado: string): void {
    this.state.update(s => ({
      ...s,
      items: s.items.map(c => c.idComerciante === id ? { ...c, estado: nuevoEstado } : c)
    }));
  }

  public removeComercianteLocal(id: number): void {
    this.state.update(s => ({
      ...s,
      items: s.items.filter(c => c.idComerciante !== id),
      totalCount: Math.max(0, s.totalCount - 1)
    }));
  }
}

