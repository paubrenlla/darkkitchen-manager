import { Component, inject, signal, Inject, Optional } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpErrorResponse } from '@angular/common/http';

import { ProductService } from '../../services/product.service';
import {
  ProductCreateRequest,
  ProductUpdateRequest,
  ProductResponse,
} from '../../models/product.models';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './product-form.component.html',
})
export class ProductFormComponent {
  private productService = inject(ProductService);
  private dialogRef = inject(MatDialogRef<ProductFormComponent>);

  isEditMode: boolean;

  code = '';
  name = '';
  description = '';
  line = '';
  category = '';
  price: number | null = null;
  images: { url: string; sizeInBytes: number | null }[] = [{ url: '', sizeInBytes: null }];

  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  constructor(@Optional() @Inject(MAT_DIALOG_DATA) public data: ProductResponse | null) {
    this.isEditMode = !!data;

    if (data) {
      this.code = data.code;
      this.name = data.name;
      this.description = data.description;
      this.line = data.line;
      this.category = data.category;
      this.price = data.price;
      this.images = data.images.map((i) => ({ url: i.url, sizeInBytes: i.sizeInBytes }));
    }
  }

  addImage(): void {
    this.images.push({ url: '', sizeInBytes: null });
  }

  removeImage(index: number): void {
    if (this.images.length > 1) {
      this.images.splice(index, 1);
    }
  }

  onSubmit(): void {
    if (!this.isFormValid()) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    const obs = this.isEditMode
      ? this.productService.update(this.data!.id, this.buildUpdateRequest())
      : this.productService.create(this.buildCreateRequest());

    obs.subscribe({
      next: () => {
        this.isLoading.set(false);
        this.dialogRef.close(true);
      },
      error: (err: HttpErrorResponse) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.error || 'No se pudo guardar el producto.');
      },
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  private buildCreateRequest(): ProductCreateRequest {
    return {
      code: this.code.trim(),
      name: this.name.trim(),
      description: this.description.trim(),
      line: this.line.trim(),
      category: this.category.trim(),
      price: this.price!,
      images: this.images.map((i) => ({ url: i.url.trim(), sizeInBytes: i.sizeInBytes! })),
    };
  }

  private buildUpdateRequest(): ProductUpdateRequest {
    return {
      name: this.name.trim(),
      description: this.description.trim(),
      line: this.line.trim(),
      category: this.category.trim(),
      price: this.price!,
      images: this.images.map((i) => ({ url: i.url.trim(), sizeInBytes: i.sizeInBytes! })),
      isActive: this.data!.isActive,
    };
  }

  private isFormValid(): boolean {
    if (!this.name || !this.description || !this.line || !this.category) {
      this.errorMessage.set('Por favor, completá todos los campos obligatorios.');
      return false;
    }

    if (!this.isEditMode && !this.code) {
      this.errorMessage.set('Por favor, completá todos los campos obligatorios.');
      return false;
    }

    if (!this.price || this.price <= 0) {
      this.errorMessage.set('El precio debe ser mayor a cero.');
      return false;
    }

    const invalidImage = this.images.some((i) => !i.url || !i.sizeInBytes || i.sizeInBytes <= 0);
    if (invalidImage) {
      this.errorMessage.set('Cada imagen debe tener una URL y un tamaño mayor a cero.');
      return false;
    }

    return true;
  }
}
