using DarkKitchen.Domain;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess;

public class SqlPromotionRepository(DarkKitchenContext context) : IPromotionRepository
{
    private readonly DarkKitchenContext _context = context;

    public void Add(Promotion promotion)
    {
        var trackedProducts = promotion.Products
            .Select(p => _context.Products.Find(p.Id) ?? p)
            .ToList();

        promotion.Update(
            promotion.Name,
            promotion.DiscountPercentage,
            promotion.StartDate,
            promotion.EndDate,
            trackedProducts);

        _context.Promotions.Add(promotion);
        _context.SaveChanges();
    }

    public IEnumerable<Promotion> GetAll()
    {
        return _context.Promotions
            .AsNoTracking()
            .Include(p => p.Products)
                .ThenInclude(p => p.Line)
            .Include(p => p.Products)
                .ThenInclude(p => p.Category)
            .Include(p => p.Products)
                .ThenInclude(p => p.Images)
            .ToList();
    }

    public Promotion? GetById(Guid id)
    {
        return _context.Promotions
            .Include(p => p.Products)
                .ThenInclude(p => p.Line)
            .Include(p => p.Products)
                .ThenInclude(p => p.Category)
            .Include(p => p.Products)
                .ThenInclude(p => p.Images)
            .FirstOrDefault(p => p.Id == id);
    }

    public void Update(Promotion promotion)
    {
        var existing = _context.Promotions
            .Include(p => p.Products)
            .FirstOrDefault(p => p.Id == promotion.Id)
            ?? throw new KeyNotFoundException($"Promoción {promotion.Id} no encontrada.");

        existing.Update(
            promotion.Name,
            promotion.DiscountPercentage,
            promotion.StartDate,
            promotion.EndDate,
            promotion.Products);

        var trackedProducts = promotion.Products
            .Select(p => _context.Products.Find(p.Id) ?? p)
            .ToList();

        existing.Products.Clear();
        foreach(var product in trackedProducts)
        {
            existing.Products.Add(product);
        }

        _context.SaveChanges();
    }
}
