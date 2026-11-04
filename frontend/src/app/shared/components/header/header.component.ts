import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthStore } from '../../../core/state/auth.store';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <header class="app-header">
      <div class="header-left">
        <div class="brand-logo" routerLink="/comerciantes">
          <span class="logo-text">OL<span class="logo-accent">.</span></span>
          <span class="logo-subtext">Software &<br>Development</span>
        </div>
      </div>

      <nav class="header-nav">
        <a routerLink="/comerciantes" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" class="nav-link">
          <span class="nav-dot"></span>
          Lista Formulario
        </a>
        <span class="nav-divider">|</span>
        <a routerLink="/comerciantes/nuevo" routerLinkActive="active" class="nav-link">
          <span class="nav-dot"></span>
          Crear Formulario
        </a>
        <a class="nav-link nav-link-dimmed" href="javascript:void(0)">
          <span class="nav-dot"></span>
          Beneficios por renovar
        </a>
      </nav>

      <div class="header-right">
        @if (authStore.isAuthenticated()) {
          <div class="user-profile">
            <div class="user-avatar" title="{{ authStore.userDisplayName() }}">
              <svg viewBox="0 0 24 24" width="22" height="22" fill="currentColor">
                <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/>
              </svg>
            </div>
            <div class="user-info">
              <span class="welcome-text">¡Bienvenido!</span>
              <span class="user-name">{{ authStore.userDisplayName() }}</span>
              <span class="user-role-badge" [ngClass]="{'badge-admin': authStore.isAdmin()}">
                {{ authStore.userRole() }}
              </span>
            </div>
            <button type="button" class="btn-logout" (click)="onLogout()" title="Cerrar Sesión">
              <svg viewBox="0 0 24 24" width="18" height="18" fill="currentColor">
                <path d="M17 7l-1.41 1.41L18.17 11H8v2h10.17l-2.58 2.58L17 17l5-5zM4 5h8V3H4c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h8v-2H4V5z"/>
              </svg>
            </button>
          </div>
        }
      </div>
    </header>
  `,
  styles: [`
    .app-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      height: 64px;
      padding: 0 28px;
      background: #ffffff;
      border-bottom: 1px solid #e2e8f0;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
      position: sticky;
      top: 0;
      z-index: 1000;
    }

    .header-left {
      display: flex;
      align-items: center;
    }

    .brand-logo {
      display: flex;
      align-items: center;
      gap: 6px;
      cursor: pointer;
      text-decoration: none;
      user-select: none;

      .logo-text {
        font-size: 26px;
        font-weight: 800;
        color: #0284c7;
        line-height: 1;

        .logo-accent {
          color: #06b6d4;
        }
      }

      .logo-subtext {
        font-size: 9px;
        font-weight: 600;
        text-transform: uppercase;
        color: #64748b;
        line-height: 1.1;
      }
    }

    .header-nav {
      display: flex;
      align-items: center;
      gap: 18px;

      .nav-link {
        display: flex;
        align-items: center;
        gap: 8px;
        text-decoration: none;
        color: #475569;
        font-size: 13.5px;
        font-weight: 500;
        transition: color 0.2s;

        .nav-dot {
          width: 7px;
          height: 7px;
          border-radius: 50%;
          background: #94a3b8;
          transition: background 0.2s;
        }

        &:hover {
          color: #0284c7;
          .nav-dot { background: #0284c7; }
        }

        &.active {
          color: #0284c7;
          font-weight: 600;
          .nav-dot { background: #0284c7; }
        }

        &.nav-link-dimmed {
          color: #64748b;
        }
      }

      .nav-divider {
        color: #cbd5e1;
        font-size: 14px;
      }
    }

    .header-right {
      display: flex;
      align-items: center;
    }

    .user-profile {
      display: flex;
      align-items: center;
      gap: 12px;

      .user-avatar {
        width: 36px;
        height: 36px;
        border-radius: 50%;
        background: #f1f5f9;
        color: #475569;
        display: flex;
        align-items: center;
        justify-content: center;
      }

      .user-info {
        display: flex;
        flex-direction: column;
        line-height: 1.2;

        .welcome-text {
          font-size: 11px;
          color: #64748b;
          font-weight: 500;
        }

        .user-name {
          font-size: 13px;
          font-weight: 600;
          color: #1e293b;
        }

        .user-role-badge {
          font-size: 10px;
          color: #0369a1;
          font-weight: 600;

          &.badge-admin {
            color: #b91c1c;
          }
        }
      }

      .btn-logout {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 32px;
        height: 32px;
        border-radius: 6px;
        border: none;
        background: #f8fafc;
        color: #64748b;
        cursor: pointer;
        transition: all 0.2s;

        &:hover {
          background: #fee2e2;
          color: #ef4444;
        }
      }
    }
  `]
})
export class HeaderComponent {
  public readonly authStore = inject(AuthStore);
  private readonly authService = inject(AuthService);

  public onLogout(): void {
    this.authService.logout();
  }
}
