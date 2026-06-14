import { Component, Inject } from '@angular/core';

import {
  MAT_DIALOG_DATA,
  MatDialogModule
} from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { ShippingTypeResponse } from '../../models/shipping.models';

@Component({
  selector: 'app-shipping-type-delete-confirm',
  standalone: true,
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
  ],
  template: `
    <h2
      mat-dialog-title
      class="!text-lg !font-bold"
      style="color: var(--text-color)">
      Eliminar tipo de envío
    </h2>

    <mat-dialog-content class="!py-4">
      <p class="text-sm text-slate-600">
        ¿Estás seguro de que querés eliminar el tipo de envío
        <span class="font-semibold text-slate-800">
          {{ data.name }}
        </span>?
      </p>

      <p class="text-sm text-rose-600 mt-2">
        Esta acción no se puede deshacer.
      </p>
    </mat-dialog-content>

    <mat-dialog-actions align="end" class="!px-6 !pb-4 gap-2">
      <button
        mat-stroked-button
        [mat-dialog-close]="false">
        Cancelar
      </button>

      <button
        mat-flat-button
        color="warn"
        [mat-dialog-close]="true">
        <mat-icon>delete</mat-icon>
        Eliminar
      </button>
    </mat-dialog-actions>
  `,
})
export class ShippingTypeDeleteConfirmComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: ShippingTypeResponse
  ) {}
}
