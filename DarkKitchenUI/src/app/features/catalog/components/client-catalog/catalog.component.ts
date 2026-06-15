import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { HttpErrorResponse } from '@angular/common/http';

import { ProductService } from '../../../products/services/product.service';
import { ProductResponse } from '../../../products/models/product.models';
import { ShippingTypeService } from '../../../shipping/services/shipping-type.service';
import { ShippingTypeResponse } from '../../../shipping/models/shipping.models';
import { OrderService } from '../../../orders/services/order.service';
import { OrderCreateRequest } from '../../../orders/models/order.models';
import { PromotionService } from '../../../promotions/services/promotion.service';
import { PromotionResponse } from '../../../promotions/models/promotion.models';

interface CartItem {
  product: ProductResponse;
  quantity: number;
}

@Component({
  selector: 'app-catalog',
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatDividerModule,
  ],
  templateUrl: './catalog.component.html',
})
export class CatalogComponent implements OnInit {
  private productService = inject(ProductService);
  private shippingService = inject(ShippingTypeService);
  private orderService = inject(OrderService);
  private promotionService = inject(PromotionService);
  private router = inject(Router);

  products = signal<ProductResponse[]>([]);
  shippingTypes = signal<ShippingTypeResponse[]>([]);
  promotions = signal<PromotionResponse[]>([]);
  cart = signal<CartItem[]>([]);

  isLoadingProducts = signal(false);
  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);
  orderSuccess = signal(false);
  showCheckout = signal(false);

  street = '';
  number = '';
  apartment = '';
  city = '';
  country = '';
  selectedShippingType = '';

  cartTotal = computed(() =>
    this.cart().reduce((sum, item) => {
      const promo = this.getBestPromotion(item.product);
      const price = promo
        ? item.product.price * (1 - promo.discountPercentage / 100)
        : item.product.price;
      return sum + price * item.quantity;
    }, 0),
  );

  cartCount = computed(() => this.cart().reduce((sum, item) => sum + item.quantity, 0));

  ngOnInit(): void {
    this.loadProducts();
    this.loadShippingTypes();
    this.loadPromotions();
  }

  private loadProducts(): void {
    this.isLoadingProducts.set(true);
    this.productService.getAll().subscribe({
      next: (data) => {
        this.products.set(data.filter((p) => p.isActive));
        this.isLoadingProducts.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudieron cargar los productos.');
        this.isLoadingProducts.set(false);
      },
    });
  }

  private loadShippingTypes(): void {
    this.shippingService.getAll().subscribe({
      next: (data) => {
        this.shippingTypes.set(data);
        if (data.length > 0) this.selectedShippingType = data[0].name;
      },
    });
  }

  private loadPromotions(): void {
    const today = new Date().toISOString().split('T')[0];
    this.promotionService.getAll(today).subscribe({
      next: (data) => this.promotions.set(data),
      error: () => this.promotions.set([]),
    });
  }

  getBestPromotion(product: ProductResponse): PromotionResponse | null {
    const applicable = this.promotions().filter((p) => p.products.includes(product.code));
    if (!applicable.length) return null;
    return applicable.reduce((best, p) =>
      p.discountPercentage > best.discountPercentage ? p : best,
    );
  }

  getDiscountedPrice(product: ProductResponse): number {
    const promo = this.getBestPromotion(product);
    if (!promo) return product.price;
    return product.price * (1 - promo.discountPercentage / 100);
  }

  getCartQuantity(productId: string): number {
    return this.cart().find((i) => i.product.id === productId)?.quantity ?? 0;
  }

  addToCart(product: ProductResponse): void {
    this.cart.update((items) => {
      const existing = items.find((i) => i.product.id === product.id);
      if (existing) {
        return items.map((i) =>
          i.product.id === product.id ? { ...i, quantity: i.quantity + 1 } : i,
        );
      }
      return [...items, { product, quantity: 1 }];
    });
  }

  removeFromCart(productId: string): void {
    this.cart.update((items) => {
      const existing = items.find((i) => i.product.id === productId);
      if (existing && existing.quantity > 1) {
        return items.map((i) =>
          i.product.id === productId ? { ...i, quantity: i.quantity - 1 } : i,
        );
      }
      return items.filter((i) => i.product.id !== productId);
    });
  }

  clearCart(): void {
    this.cart.set([]);
    this.showCheckout.set(false);
  }

  confirmOrder(): void {
    if (!this.isCheckoutValid()) return;

    const request: OrderCreateRequest = {
      deliveryType: this.selectedShippingType,
      address: {
        street: this.street.trim(),
        number: this.number.trim(),
        apartment: this.apartment.trim() || null,
        city: this.city.trim(),
        country: this.country.trim(),
      },
      items: this.cart().map((i) => ({
        productId: i.product.id,
        quantity: i.quantity,
      })),
    };

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.orderService.createOrder(request).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.orderSuccess.set(true);
        this.cart.set([]);
        this.showCheckout.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err.error?.error || 'No se pudo crear el pedido.');
      },
    });
  }

  private isCheckoutValid(): boolean {
    if (!this.street || !this.number || !this.city || !this.country) {
      this.errorMessage.set('Completá todos los campos de dirección obligatorios.');
      return false;
    }
    if (!this.selectedShippingType) {
      this.errorMessage.set('Seleccioná un tipo de envío.');
      return false;
    }
    return true;
  }

  goToOrders(): void {
    this.router.navigate(['/dashboard/my-orders']);
  }
}
