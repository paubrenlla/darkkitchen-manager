export function mapPromotionErrors(
  errors: Record<string, string>,
): Record<string, string> {

  const mapped = { ...errors };

  if (
    mapped['name'] ===
    'Name must be between 3 and 50 characters.'
  ) {
    mapped['name'] =
      'El nombre debe tener entre 3 y 50 caracteres.';
  }

  if (
    mapped['discountPercentage'] ===
    'Discount percentage must be greater than zero.'
  ) {
    mapped['discountPercentage'] =
      'El descuento debe ser mayor a 0.';
  }

  if (
    mapped['endDate'] ===
    'Start date must be before or equal to end date.'
  ) {
    mapped['endDate'] =
      'La fecha de fin debe ser posterior o igual a la fecha de inicio.';
  }

  if (
    mapped['productCodes'] ===
    'A promotion must contain at least one product.'
  ) {
    mapped['productCodes'] =
      'Debe seleccionar al menos un producto.';
  }

  return mapped;
}
