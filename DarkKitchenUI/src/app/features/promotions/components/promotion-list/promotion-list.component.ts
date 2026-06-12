import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { PromotionService } from '../../services/promotion.service';
import { PromotionResponse } from '../../models/promotion.models';
import { PromotionFormComponent } from '../promotion-form/promotion-form.component';

import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-promotion-list',
  standalone: true,
  imports: [
    MatButtonModule,
    MatProgressSpinnerModule,
    MatDialogModule,
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

  get promotionList(): PromotionResponse[] {
    return this.promotions();
  }

  ngOnInit(): void {
    this.loadPromotions();
  }

  private loadPromotions(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.promotionService.getAll().subscribe({
      next: (data) => {
        this.promotionService.promotions.set(data);
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
