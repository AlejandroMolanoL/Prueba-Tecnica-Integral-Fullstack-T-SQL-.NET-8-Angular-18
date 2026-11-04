import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login.component';
import { ComerciantesListComponent } from './features/comerciantes/list/comerciantes-list.component';
import { ComercianteFormComponent } from './features/comerciantes/form/comerciante-form.component';
import { authGuard, publicGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'comerciantes',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [publicGuard]
  },
  {
    path: 'comerciantes',
    component: ComerciantesListComponent,
    canActivate: [authGuard]
  },
  {
    path: 'comerciantes/nuevo',
    component: ComercianteFormComponent,
    canActivate: [authGuard]
  },
  {
    path: 'comerciantes/editar/:id',
    component: ComercianteFormComponent,
    canActivate: [authGuard]
  },
  {
    path: '**',
    redirectTo: 'comerciantes'
  }
];
