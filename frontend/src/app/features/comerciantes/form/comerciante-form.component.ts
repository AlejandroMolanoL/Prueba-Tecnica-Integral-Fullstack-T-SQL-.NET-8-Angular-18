import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ComerciantesService } from '../../../core/services/comerciantes.service';
import { MunicipiosService } from '../../../core/services/municipios.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Municipio } from '../../../core/models/municipio.model';
import { Comerciante, ActualizarComerciante, CrearComerciante } from '../../../core/models/comerciante.model';
import { CurrencyCopPipe } from '../../../shared/pipes/currency-cop.pipe';

@Component({
  selector: 'app-comerciante-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, CurrencyCopPipe],
  templateUrl: './comerciante-form.component.html',
  styleUrls: ['./comerciante-form.component.scss']
})
export class ComercianteFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly comerciantesService = inject(ComerciantesService);
  private readonly municipiosService = inject(MunicipiosService);
  private readonly notificationService = inject(NotificationService);

  public isEditMode: boolean = false;
  public comercianteId: number | null = null;
  public comercianteActual: Comerciante | null = null;
  public isLoading: boolean = false;
  public isSaving: boolean = false;
  public isSubmitted: boolean = false;

  // Listas de valores para selectores
  public municipios: Municipio[] = [];
  public departamentos: string[] = [];
  public municipiosFiltrados: Municipio[] = [];

  // Totales de establecimientos para el footer en modo edicion (Reto 11)
  public totalIngresos: number = 0;
  public totalEmpleados: number = 0;

  public comercianteForm: FormGroup = this.fb.group({
    nombreRazonSocial: ['', [Validators.required, Validators.maxLength(150)]],
    departamento: ['', [Validators.required]],
    idMunicipio: ['', [Validators.required]],
    telefono: ['', [Validators.maxLength(30), Validators.pattern(/^[+0-9\s\-()]*$/)]],
    correoElectronico: ['', [Validators.email, Validators.maxLength(150)]],
    fechaRegistro: [new Date().toISOString().slice(0, 10), [Validators.required]],
    estado: ['Activo', [Validators.required]],
    poseeEstablecimientos: [false]
  });

  ngOnInit(): void {
    this.cargarMunicipios();
    this.verificarModoEdicion();
  }

  private cargarMunicipios(): void {
    this.municipiosService.getMunicipios().subscribe({
      next: (data) => {
        this.municipios = data;
        // Extraer departamentos unicos ordenados
        const depts = new Set<string>();
        data.forEach(m => depts.add(m.departamento));
        this.departamentos = Array.from(depts).sort();

        // Si ya hay un municipio seleccionado (en edicion), actualizar filtro de ciudades
        if (this.comercianteForm.get('departamento')?.value) {
          this.onDepartamentoChange(this.comercianteForm.get('departamento')?.value);
        }
      }
    });
  }

  private verificarModoEdicion(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEditMode = true;
      this.comercianteId = parseInt(idParam, 10);
      this.cargarDatosComerciante(this.comercianteId);
    }
  }

  private cargarDatosComerciante(id: number): void {
    this.isLoading = true;
    this.comerciantesService.getPorId(id).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success && response.data) {
          const c = response.data;
          this.comercianteActual = c;

          // Asignar totales para el footer de edicion (Reto 11)
          this.totalIngresos = c.totalIngresos || 0;
          this.totalEmpleados = c.cantidadEmpleados || 0;

          // Buscar el departamento del municipio actual
          const mun = this.municipios.find(m => m.idMunicipio === c.idMunicipio);
          const dept = mun ? mun.departamento : '';

          if (dept) {
            this.onDepartamentoChange(dept);
          }

          this.comercianteForm.patchValue({
            nombreRazonSocial: c.nombreRazonSocial,
            departamento: dept,
            idMunicipio: c.idMunicipio,
            telefono: c.telefono || '',
            correoElectronico: c.correoElectronico || '',
            fechaRegistro: c.fechaRegistro ? c.fechaRegistro.slice(0, 10) : '',
            estado: c.estado,
            poseeEstablecimientos: c.cantidadEstablecimientos > 0
          });
        }
      },
      error: () => {
        this.isLoading = false;
        this.notificationService.error('No se pudo cargar la información del comerciante.');
        this.router.navigate(['/comerciantes']);
      }
    });
  }

  public onDepartamentoChange(departamento: string): void {
    if (!departamento) {
      this.municipiosFiltrados = [];
      this.comercianteForm.patchValue({ idMunicipio: '' });
      return;
    }

    this.municipiosFiltrados = this.municipios.filter(m => m.departamento === departamento);
    
    // Si el municipio actual no pertenece al nuevo departamento seleccionado, resetear
    const currentMunId = this.comercianteForm.get('idMunicipio')?.value;
    const existsInDept = this.municipiosFiltrados.some(m => m.idMunicipio === Number(currentMunId));
    if (!existsInDept && this.municipiosFiltrados.length > 0) {
      this.comercianteForm.patchValue({ idMunicipio: this.municipiosFiltrados[0].idMunicipio });
    }
  }

  public onSubmit(): void {
    this.isSubmitted = true;

    if (this.comercianteForm.invalid) {
      this.notificationService.warning('Por favor corrija los errores marcados en el formulario.');
      return;
    }

    this.isSaving = true;
    const formVal = this.comercianteForm.value;

    if (this.isEditMode && this.comercianteId) {
      const dto: ActualizarComerciante = {
        nombreRazonSocial: formVal.nombreRazonSocial.trim(),
        idMunicipio: Number(formVal.idMunicipio),
        telefono: formVal.telefono ? formVal.telefono.trim() : null,
        correoElectronico: formVal.correoElectronico ? formVal.correoElectronico.trim() : null,
        fechaRegistro: formVal.fechaRegistro,
        estado: formVal.estado
      };

      this.comerciantesService.actualizar(this.comercianteId, dto).subscribe({
        next: () => {
          this.isSaving = false;
          this.router.navigate(['/comerciantes']);
        },
        error: () => {
          this.isSaving = false;
        }
      });
    } else {
      const dto: CrearComerciante = {
        nombreRazonSocial: formVal.nombreRazonSocial.trim(),
        idMunicipio: Number(formVal.idMunicipio),
        telefono: formVal.telefono ? formVal.telefono.trim() : null,
        correoElectronico: formVal.correoElectronico ? formVal.correoElectronico.trim() : null,
        fechaRegistro: formVal.fechaRegistro,
        estado: formVal.estado
      };

      this.comerciantesService.crear(dto).subscribe({
        next: () => {
          this.isSaving = false;
          this.router.navigate(['/comerciantes']);
        },
        error: () => {
          this.isSaving = false;
        }
      });
    }
  }

  public onCancelar(): void {
    this.router.navigate(['/comerciantes']);
  }
}
