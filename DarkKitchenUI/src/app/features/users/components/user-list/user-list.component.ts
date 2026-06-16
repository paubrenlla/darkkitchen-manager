import { Component, inject, signal, OnInit } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';

import { UserService } from '../../services/user.service';
import { UserResponse } from '../../models/user.models';
import { UserFormComponent } from '../user-form/user-form.component';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatFormFieldModule,
    MatInputModule,
    MatDialogModule,
  ],
  templateUrl: './user-list.component.html',
})
export class UserListComponent implements OnInit {
  private userService = inject(UserService);
  private dialog = inject(MatDialog);

  users = this.userService.users;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  filterName = signal('');
  filterSurname = signal('');

  displayedColumns = ['name', 'email', 'phone', 'role', 'actions'];

  ngOnInit(): void {
    this.loadUsers();
  }

  onSearch(): void {
    this.loadUsers();
  }

  onClear(): void {
    this.filterName.set('');
    this.filterSurname.set('');
    this.loadUsers();
  }

  openCreateDialog(): void {
    const ref = this.dialog.open(UserFormComponent, {
      disableClose: true,
      data: null,
    });
    ref.afterClosed().subscribe((created) => {
      if (created) this.loadUsers();
    });
  }

  openEditDialog(user: UserResponse): void {
    const ref = this.dialog.open(UserFormComponent, {
      disableClose: true,
      data: user,
    });
    ref.afterClosed().subscribe((updated) => {
      if (updated) this.loadUsers();
    });
  }

  openDeleteDialog(user: UserResponse): void {
    const ref = this.dialog.open(UserDeleteConfirmComponent, {
      data: user,
    });
    ref.afterClosed().subscribe((confirmed) => {
      if (!confirmed) return;
      this.userService.delete(user.id).subscribe({
        next: () => this.loadUsers(),
        error: () => this.errorMessage.set('No se pudo eliminar el usuario.'),
      });
    });
  }

  getRoleClass(role: string): string {
    switch (role) {
      case 'Administrativo': return 'bg-indigo-50 text-indigo-700 border border-indigo-100';
      case 'Preparador': return 'bg-amber-50 text-amber-700 border border-amber-100';
      case 'Cliente': return 'bg-emerald-50 text-emerald-700 border border-emerald-100';
      default: return 'bg-slate-50 text-slate-600 border border-slate-200';
    }
  }

  private loadUsers(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    const name = this.filterName().trim() || undefined;
    const surname = this.filterSurname().trim() || undefined;
    this.userService.getAll(name, surname).subscribe({
      next: (data) => {
        this.userService.users.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudieron cargar los usuarios.');
        this.isLoading.set(false);
      },
    });
  }
}

// ─── Inline delete confirmation dialog ────────────────────────────────────────
import { Component as Comp, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Comp({
  selector: 'app-user-delete-confirm',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule],
  template: `
    <h2 mat-dialog-title class="!text-lg !font-bold" style="color: var(--text-color)">Eliminar usuario</h2>
    <mat-dialog-content class="!py-4">
      <p class="text-sm text-slate-600">
        ¿Estás seguro de que querés eliminar a
        <span class="font-semibold text-slate-800">{{ data.name }} {{ data.surname }}</span>?
        Esta acción no se puede deshacer.
      </p>
    </mat-dialog-content>
    <mat-dialog-actions align="end" class="!px-6 !pb-4 gap-2">
      <button mat-stroked-button [mat-dialog-close]="false" class="!rounded-lg">Cancelar</button>
      <button mat-flat-button color="warn" [mat-dialog-close]="true" class="!rounded-lg">
        <mat-icon>delete</mat-icon>
        <span>Eliminar</span>
      </button>
    </mat-dialog-actions>
  `,
})
export class UserDeleteConfirmComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: UserResponse) {}
}
