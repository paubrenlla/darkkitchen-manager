import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';

import { PromotionService } from '../../services/promotion.service';
import { PromotionResponse } from '../../models/promotion.models';

@Component({
  selector: 'app-promotion-catalog',
  standalone: true,
  imports: [
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatChipsModule,
    DatePipe,
  ],
  templateUrl: './promotion-catalog.component.html',
})
export class PromotionCatalogComponent implements OnInit {
  private promotionService = inject(PromotionService);

  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  promotions = signal<PromotionResponse[]>([]);

  filterDate = signal('');
  filterLine = signal('');
  filterProductCode = signal('');

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
        this.promotions.set(
          (data ?? []).map(p => ({ ...p, products: p.products ?? [] }))
        );
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudieron cargar las promociones.');
        this.isLoading.set(false);
      },
    });
  }
}
