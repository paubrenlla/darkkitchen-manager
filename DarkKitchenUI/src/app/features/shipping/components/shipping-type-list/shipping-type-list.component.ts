import { Component, inject, signal, OnInit } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';

import { ShippingTypeService } from '../../services/shipping-type.service';
import { ShippingTypeResponse } from '../../models/shipping.models';
import { ShippingTypeFormComponent } from '../shipping-type-form/shipping-type-form.component';
import { ShippingTypeDeleteConfirmComponent } from './shipping-type-delete-confirm.component';

@Component({
  selector: 'app-shipping-type-list',
  standalone: true,
  imports: [
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDialogModule,
  ],
  templateUrl: './shipping-type-list.component.html',
})
export class ShippingTypeListComponent implements OnInit {
  private shippingTypeService = inject(ShippingTypeService);
  private dialog = inject(MatDialog);

  shippingTypes = this.shippingTypeService.shippingTypes;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadShippingTypes();
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(ShippingTypeFormComponent, {
      disableClose: true,
      data: null,
    });

    dialogRef.afterClosed().subscribe((created) => {
      if (created) this.loadShippingTypes();
    });
  }

  openEditDialog(shippingType: ShippingTypeResponse): void {
    const dialogRef = this.dialog.open(ShippingTypeFormComponent, {
      disableClose: true,
      data: shippingType,
    });

    dialogRef.afterClosed().subscribe((updated) => {
      if (updated) this.loadShippingTypes();
    });
  }

  openDeleteDialog(shippingType: ShippingTypeResponse): void {
    const ref = this.dialog.open(
      ShippingTypeDeleteConfirmComponent,
      {
        data: shippingType,
      }
    );

    ref.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.shippingTypeService.delete(shippingType.id).subscribe({
        next: () => this.loadShippingTypes(),
        error: (err: HttpErrorResponse) => {
          this.errorMessage.set(
            err.error?.error || 'No se pudo eliminar el tipo de envío.'
          );
        },
      });
    });
  }

  private loadShippingTypes(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.shippingTypeService.getAll().pipe(
      finalize(() => this.isLoading.set(false))
    ).subscribe({
      next: (data) => {
        this.shippingTypeService.shippingTypes.set(data ?? []);
      },
      error: () => {
        this.errorMessage.set('Error al cargar los tipos de envío.');
      },
    });
  }
}
