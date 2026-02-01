import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatSidenavModule,
    MatListModule
  ],
  template: `
    <mat-sidenav-container class="app-container">
      <mat-sidenav #sidenav mode="side" opened class="sidenav">
        <div class="sidenav-header">
          <mat-icon>memory</mat-icon>
          <span>SAGA Monitor</span>
        </div>
        <mat-nav-list>
          <a mat-list-item routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}">
            <mat-icon matListItemIcon>dashboard</mat-icon>
            <span matListItemTitle>Dashboard</span>
          </a>
          <a mat-list-item routerLink="/create" routerLinkActive="active">
            <mat-icon matListItemIcon>add_circle</mat-icon>
            <span matListItemTitle>Create Trip</span>
          </a>
        </mat-nav-list>
        <div class="sidenav-footer">
          <p>MassTransit SAGA Pattern</p>
          <p class="version">v1.0.0</p>
        </div>
      </mat-sidenav>

      <mat-sidenav-content>
        <mat-toolbar color="primary">
          <button mat-icon-button (click)="sidenav.toggle()">
            <mat-icon>menu</mat-icon>
          </button>
          <span>Trip Booking SAGA Dashboard</span>
          <span class="spacer"></span>
          <button mat-icon-button matTooltip="Refresh">
            <mat-icon>refresh</mat-icon>
          </button>
        </mat-toolbar>

        <main class="main-content">
          <router-outlet />
        </main>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    .app-container {
      height: 100vh;
    }

    .sidenav {
      width: 260px;
      background: linear-gradient(180deg, #1a237e 0%, #283593 100%);
    }

    .sidenav-header {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 24px;
      color: white;
      font-size: 20px;
      font-weight: 500;
      border-bottom: 1px solid rgba(255,255,255,0.1);

      mat-icon {
        font-size: 32px;
        width: 32px;
        height: 32px;
      }
    }

    mat-nav-list {
      padding-top: 8px;

      a {
        color: rgba(255,255,255,0.8);
        margin: 4px 8px;
        border-radius: 8px;

        &:hover {
          background: rgba(255,255,255,0.1);
        }

        &.active {
          background: rgba(255,255,255,0.2);
          color: white;
        }

        mat-icon {
          color: inherit;
        }
      }
    }

    .sidenav-footer {
      position: absolute;
      bottom: 0;
      left: 0;
      right: 0;
      padding: 16px 24px;
      border-top: 1px solid rgba(255,255,255,0.1);
      color: rgba(255,255,255,0.5);
      font-size: 12px;

      p {
        margin: 0;
      }

      .version {
        margin-top: 4px;
        font-family: monospace;
      }
    }

    mat-toolbar {
      position: sticky;
      top: 0;
      z-index: 100;
    }

    .spacer {
      flex: 1;
    }

    .main-content {
      background: #f5f5f5;
      min-height: calc(100vh - 64px);
    }
  `]
})
export class App {}
