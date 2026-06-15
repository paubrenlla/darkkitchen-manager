import { Component, inject, signal, OnInit, input, computed, output } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ProductService } from '../../services/product.service';
import { ProductResponse } from '../../models/product.models';
import { ProductFormComponent } from '../product-form/product-form.component';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

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
    MatDialogModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './product-list.component.html',
})
export class ProductListComponent implements OnInit {
  private productService = inject(ProductService);
  private dialog = inject(MatDialog);

  // Reusability inputs
  customProducts = input<ProductResponse[] | undefined>(undefined);
  hideHeader = input<boolean>(false);
  hideEditButton = input<boolean>(false);
  
  // Emit changes when a product is modified from a custom list
  productModified = output<ProductResponse>();

  // If customProducts is provided, use it. Otherwise, use the global service products.
  products = computed(() => {
    const custom = this.customProducts();
    return custom ? custom : this.productService.products();
  });

  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  // Filter signals
  filterName = signal('');
  filterLine = signal('');
  filterCategory = signal('');

  displayedColumns = ['code', 'name', 'line', 'category', 'price', 'status', 'actions'];

  ngOnInit(): void {
    if (!this.customProducts()) {
      this.loadProducts();
    }
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(ProductFormComponent, {
      disableClose: true,
      data: null,
    });

    dialogRef.afterClosed().subscribe((created) => {
      if (created) this.loadProducts();
    });
  }

  openEditDialog(product: ProductResponse): void {
    const dialogRef = this.dialog.open(ProductFormComponent, {
      disableClose: true,
      data: product,
    });

    dialogRef.afterClosed().subscribe((updated) => {
      if (updated) {
        if (this.customProducts()) {
          this.productModified.emit(updated);
        } else {
          this.loadProducts();
        }
      }
    });
  }

  onToggleActive(product: ProductResponse): void {
    this.productService.toggleActive(product).subscribe({
      next: (updated) => {
        this.productService.products.update((list) =>
          list.map((p) => (p.id === updated.id ? updated : p)),
        );
        if (this.customProducts()) {
          this.productModified.emit(updated);
        }
      },
      error: () => {
        this.errorMessage.set('No se pudo cambiar el estado del producto.');
      },
    });
  }

  onSearch(): void {
    this.loadProducts();
  }

  onClear(): void {
    this.filterName.set('');
    this.filterLine.set('');
    this.filterCategory.set('');
    this.loadProducts();
  }

  private loadProducts(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    const name = this.filterName().trim() || undefined;
    const line = this.filterLine().trim() || undefined;
    const category = this.filterCategory().trim() || undefined;

    this.productService.getAll(name, line, category).subscribe({
      next: (data) => {
        this.productService.products.set(data ?? []);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudieron cargar los productos.');
        this.isLoading.set(false);
      },
    });
  }
}
