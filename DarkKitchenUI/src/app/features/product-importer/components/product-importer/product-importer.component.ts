import { Component, OnInit, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ProductImporterService } from '../../services/product-importer.service';
import { ProductImportRequest, ProductImportResponse } from '../../models/product-importer.models';
import { ProductResponse } from '../../../products/models/product.models';
import { ProductListComponent } from '../../../products/components/product-list/product-list.component';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { HttpErrorResponse } from '@angular/common/http';
import { parseBackendErrors } from '../../../../core/utils/error-parser';

@Component({
  selector: 'app-product-importer',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatDividerModule,
    MatListModule,
    ProductListComponent
  ],
  templateUrl: './product-importer.component.html'
})
export class ProductImporterComponent implements OnInit {
  private importerService = inject(ProductImporterService);

  importers = signal<string[]>([]);
  
  selectedImporter = signal<string>('');
  filePath = signal<string>('');

  isLoading = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  fieldErrors = signal<Record<string, string>>({});
  
  importResult = signal<ProductImportResponse | null>(null);

  ngOnInit(): void {
    this.loadImporters();
  }

  private loadImporters(): void {
    this.importerService.getImporters().subscribe({
      next: (data) => {
        this.importers.set(data);
        if (data.length > 0) {
          this.selectedImporter.set(data[0]);
        }
      },
      error: () => {
        this.errorMessage.set('Error al cargar la lista de plugins. Por favor, intente más tarde.');
      }
    });
  }

  onSubmit(): void {
    if (!this.selectedImporter() || !this.filePath()) {
      this.errorMessage.set('Por favor, seleccione un plugin y especifique la ruta del archivo.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.fieldErrors.set({});
    this.importResult.set(null);

    const request: ProductImportRequest = {
      importerName: this.selectedImporter(),
      filePath: this.filePath()
    };

    this.importerService.importProducts(request).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        this.importResult.set(response);
      },
      error: (err: HttpErrorResponse) => {
        this.isLoading.set(false);
        const parsed = parseBackendErrors(err);
        this.errorMessage.set(parsed.global);
        this.fieldErrors.set(parsed.fields);
      }
    });
  }

  reset(): void {
    this.importResult.set(null);
    this.errorMessage.set(null);
    this.fieldErrors.set({});
    this.filePath.set('');
  }

  onProductModified(updatedProduct: ProductResponse): void {
    const currentResult = this.importResult();
    if (currentResult) {
      const updatedList = currentResult.importedProducts.map((p) =>
        p.id === updatedProduct.id ? updatedProduct : p
      );
      this.importResult.set({
        ...currentResult,
        importedProducts: updatedList
      });
    }
  }
}
