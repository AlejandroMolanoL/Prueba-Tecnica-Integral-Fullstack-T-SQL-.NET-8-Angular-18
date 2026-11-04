import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ComerciantesService } from '../../../core/services/comerciantes.service';
import { ComerciantesStore } from '../../../core/state/comerciantes.store';
import { AuthStore } from '../../../core/state/auth.store';
import { Comerciante, ComercianteFiltros } from '../../../core/models/comerciante.model';

@Component({
  selector: 'app-comerciantes-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './comerciantes-list.component.html',
  styleUrls: ['./comerciantes-list.component.scss']
})
export class ComerciantesListComponent implements OnInit {
  public readonly store = inject(ComerciantesStore);
  public readonly authStore = inject(AuthStore);
  private readonly service = inject(ComerciantesService);
  private readonly router = inject(Router);

  // Filtros de busqueda reactivos
  public filtroNombre: string = '';
  public filtroEstado: string = '';
  public filtroFecha: string = '';
  public pageSizeOptions: number[] = [5, 10, 15]; // Requisito: 5, 10 y 15

  // Modal de confirmacion para eliminacion
  public comercianteAEliminar: Comerciante | null = null;
  public isEliminando: boolean = false;

  ngOnInit(): void {
    this.cargarComerciantes();
  }

  public cargarComerciantes(pageNumber: number = 1): void {
    const filtros: ComercianteFiltros = {
      pageNumber,
      pageSize: this.store.pageSize(),
      nombreRazonSocial: this.filtroNombre,
      estado: this.filtroEstado,
      fechaRegistro: this.filtroFecha
    };

    this.service.getPaginado(filtros).subscribe();
  }

  public onBuscar(): void {
    this.cargarComerciantes(1);
  }

  public onLimpiarFiltros(): void {
    this.filtroNombre = '';
    this.filtroEstado = '';
    this.filtroFecha = '';
    this.cargarComerciantes(1);
  }

  public onCambiarPageSize(event: Event): void {
    const target = event.target as HTMLSelectElement;
    const newSize = parseInt(target.value, 10);
    this.store.setFiltros({ pageSize: newSize, pageNumber: 1 });
    this.cargarComerciantes(1);
  }

  public onCambiarPagina(page: number): void {
    if (page >= 1 && page <= this.store.totalPages() && page !== this.store.pageNumber()) {
      this.cargarComerciantes(page);
    }
  }

  public onEditar(comerciante: Comerciante): void {
    this.router.navigate(['/comerciantes/editar', comerciante.idComerciante]);
  }

  public onCrearNuevo(): void {
    this.router.navigate(['/comerciantes/nuevo']);
  }

  public onToggleEstado(comerciante: Comerciante): void {
    const nuevoEstado = comerciante.estado === 'Activo' ? 'Inactivo' : 'Activo';
    this.service.modificarEstado(comerciante.idComerciante, nuevoEstado).subscribe();
  }

  public onConfirmarEliminar(comerciante: Comerciante): void {
    this.comercianteAEliminar = comerciante;
  }

  public onCancelarEliminar(): void {
    this.comercianteAEliminar = null;
  }

  public onEjecutarEliminar(): void {
    if (!this.comercianteAEliminar) return;

    this.isEliminando = true;
    this.service.eliminar(this.comercianteAEliminar.idComerciante).subscribe({
      next: () => {
        this.isEliminando = false;
        this.comercianteAEliminar = null;
        this.cargarComerciantes(this.store.pageNumber());
      },
      error: () => {
        this.isEliminando = false;
      }
    });
  }

  public onDescargarReporteCsv(): void {
    this.service.descargarReporteCsv().subscribe();
  }

  public getArrayPaginas(): number[] {
    const total = this.store.totalPages();
    const current = this.store.pageNumber();
    const pages: number[] = [];

    for (let i = 1; i <= total; i++) {
      if (i === 1 || i === total || (i >= current - 2 && i <= current + 2)) {
        pages.push(i);
      }
    }

    return pages;
  }
}
