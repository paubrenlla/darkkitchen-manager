import {
  Component,
  OnInit,
  inject,
  signal,
  Inject,
  Optional,
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

import { MatIconModule } from '@angular/material/icon';
import { MatDialogRef, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { PromotionService } from '../../services/promotion.service';
import { PromotionCreateRequest, PromotionResponse } from '../../models/promotion.models';

import { ProductService } from '../../../products/services/product.service';
import { ProductResponse } from '../../../products/models/product.models';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { parseBackendErrors } from '../../../../core/utils/error-parser';
import { mapPromotionErrors } from '../../utils/promotion-error.mapper';

@Component({
  selector: 'app-promotion-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatIconModule,
  ],
  templateUrl: './promotion-form.component.html',
})
export class PromotionFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private promotionService = inject(PromotionService);
  private productService = inject(ProductService);

  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  fieldErrors = signal<Record<string, string>>({});

  products = signal<ProductResponse[]>([]);
  selectedCodes: string[] = [];
  isEditMode: boolean;

  form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
    discountPercentage: [1, [Validators.required, Validators.min(1)]],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
  });

  private dialogRef = inject(MatDialogRef<PromotionFormComponent>);

  constructor(
    @Optional() @Inject(MAT_DIALOG_DATA) public data: PromotionResponse | null,
  ) {
    this.isEditMode = !!data;
  }

  ngOnInit(): void {
    this.loadProducts();

    if (this.data) {
      this.form.patchValue({
        name: this.data.name,
        discountPercentage: this.data.discountPercentage,
        startDate: this.data.startDate.split('T')[0],
        endDate: this.data.endDate.split('T')[0],
      });
      this.selectedCodes = this.data.products.map((p: any) => p.code || p);
    }
  }

  private loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (products) => this.products.set(products.filter((p) => p.isActive)),
    });
  }

  toggleProduct(code: string): void {
    if (this.selectedCodes.includes(code)) {
      this.selectedCodes = this.selectedCodes.filter((c) => c !== code);
    } else {
      this.selectedCodes.push(code);
    }

    if (this.selectedCodes.length > 0 && this.fieldErrors()['productCodes']) {
      const current = { ...this.fieldErrors() };
      delete current['productCodes'];
      this.fieldErrors.set(current);
    }
  }

  isSelected(code: string): boolean {
    return this.selectedCodes.includes(code);
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onSubmit(): void {
    this.form.markAllAsTouched();

    if (this.form.invalid) return;

    if (this.selectedCodes.length === 0) {
      this.fieldErrors.set({ ...this.fieldErrors(), productCodes: 'Debe seleccionar al menos un producto.' });
      return;
    }

    const startDate = `${this.form.value.startDate}T00:00:00.000Z`;
    const endDate = `${this.form.value.endDate}T23:59:59.999Z`;

    if (new Date(startDate) > new Date(endDate)) {
      this.fieldErrors.set({ ...this.fieldErrors(), endDate: 'La fecha de fin debe ser posterior o igual a la fecha de inicio.' });
      return;
    }

    const request: PromotionCreateRequest = {
      name: this.form.value.name!,
      discountPercentage: Number(this.form.value.discountPercentage),
      startDate,
      endDate,
      productCodes: this.selectedCodes,
    };

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.fieldErrors.set({});

    const operation = this.isEditMode
      ? this.promotionService.update(this.data!.id, request)
      : this.promotionService.create(request);

    operation.subscribe({
      next: () => {
        this.isLoading.set(false);
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isLoading.set(false);

        const parsed = parseBackendErrors(err);
        this.errorMessage.set(parsed.global);

        const rawErrors: Record<string, string> = parsed.fields || {};

        if (err.error?.message) {
          const msg = err.error.message;
          if (msg.includes('Name')) rawErrors['name'] = msg;
          if (msg.includes('Discount')) rawErrors['discountPercentage'] = msg;
          if (msg.includes('date')) rawErrors['endDate'] = msg;
          if (msg.includes('at least one product')) rawErrors['productCodes'] = msg;
        }

        this.fieldErrors.set(mapPromotionErrors(rawErrors));
      },
    });
  }
}
