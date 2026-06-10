import { Component, inject, signal, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { ProductResponse } from '../../models/product.models';

import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatTooltipModule,
  ],
  templateUrl: './product-list.component.html',
})
export class ProductListComponent implements OnInit {
  private productService = inject(ProductService);

  products = this.productService.products;
  isLoading = this.productService.isLoading;
  errorMessage = signal<string | null>(null);

  displayedColumns = ['code', 'name', 'line', 'category', 'price', 'status', 'actions'];

  ngOnInit(): void {
    this.loadProducts();
  }

  private loadProducts(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.productService.getAll().subscribe({
      next: (data) => {
        this.productService.products.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudieron cargar los productos.');
        this.isLoading.set(false);
      },
    });
  }

  onToggleActive(product: ProductResponse): void {
    this.productService.toggleActive(product).subscribe({
      next: (updated) => {
        this.productService.products.update((list) =>
          list.map((p) => (p.id === updated.id ? updated : p)),
        );
      },
      error: () => {
        this.errorMessage.set('No se pudo cambiar el estado del producto.');
      },
    });
  }
}
