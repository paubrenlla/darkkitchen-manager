import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';

import { PromotionService } from '../../services/promotion.service';
import { PromotionResponse } from '../../models/promotion.models';
import { PromotionFormComponent } from '../promotion-form/promotion-form.component';

@Component({
  selector: 'app-promotion-list',
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    DatePipe,
  ],
  templateUrl: './promotion-list.component.html',
})
export class PromotionListComponent implements OnInit {
  private promotionService = inject(PromotionService);
  private dialog = inject(MatDialog);

  readonly promotions = this.promotionService.promotions;
  readonly isLoading = this.promotionService.isLoading;

  errorMessage = signal<string | null>(null);

  filterDate = signal('');
  filterLine = signal('');
  filterProductCode = signal('');

  get promotionList(): PromotionResponse[] {
    return this.promotions();
  }

  ngOnInit(): void {
    this.loadPromotions();
  }

  onSearch(): void {
    this.loadPromotions();
  }

  onClear(): void {
    this.filterDate.set('');
    this.filterLine.set('');
    this.filterProductCode.set('');
    this.loadPromotions();
  }

  private loadPromotions(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    const date = this.filterDate().trim() || undefined;
    const line = this.filterLine().trim() || undefined;
    const productCode = this.filterProductCode().trim() || undefined;

    this.promotionService.getAll(date, line, productCode).subscribe({
      next: (data) => {
        const safe = (data ?? []).map(p => ({ ...p, products: p.products ?? [] }));
        this.promotionService.promotions.set(safe);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudieron cargar las promociones.');
        this.isLoading.set(false);
      },
    });
  }

  onCreate(): void {
    const dialogRef = this.dialog.open(PromotionFormComponent, {
      disableClose: true,
      data: null,
    });

    dialogRef.afterClosed().subscribe((created) => {
      if (created) this.loadPromotions();
    });
  }

  onEdit(promotion: PromotionResponse): void {
    const dialogRef = this.dialog.open(PromotionFormComponent, {
      disableClose: true,
      data: promotion,
    });

    dialogRef.afterClosed().subscribe((updated) => {
      if (updated) this.loadPromotions();
    });
  }
}
