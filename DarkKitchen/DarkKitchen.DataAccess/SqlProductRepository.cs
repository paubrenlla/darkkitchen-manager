using DarkKitchen.Domain.Products;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess;

public class SqlProductRepository(DarkKitchenContext context) : IProductRepository
{
    private readonly DarkKitchenContext _context = context;

    public IEnumerable<Product> GetAll()
    {
        return _context.Products
            .AsNoTracking()
            .Include(p => p.Line)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .ToList();
    }

    public Product? GetById(Guid id)
    {
        return _context.Products
            .AsNoTracking()
            .Include(p => p.Line)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .FirstOrDefault(p => p.Id == id);
    }

    public void Add(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public void Update(Guid id, Product product)
    {
        var existing = _context.Products
                           .Include(p => p.Line)
                           .Include(p => p.Category)
                           .Include(p => p.Images)
                           .FirstOrDefault(p => p.Id == id)
                       ?? throw new KeyNotFoundException($"Producto {id} no encontrado.");

        var line = _context.ProductLines.FirstOrDefault(l => l.Name == product.Line.Name)
                   ?? product.Line;
        var category = _context.ProductCategories.FirstOrDefault(c => c.Name == product.Category.Name)
                       ?? product.Category;

        var newImages = product.Images
            .Select(i => new ProductImage(i.Url, i.SizeInBytes))
            .ToList();

        _context.Set<ProductImage>().AddRange(newImages);

        existing.UpdateDetails(
            product.Name,
            product.Description,
            line,
            category,
            product.Price,
            newImages);

        if(product.IsActive)
        {
            existing.Activate();
        }
        else
        {
            existing.Deactivate();
        }

        _context.SaveChanges();
    }

    public IEnumerable<ProductLine> GetAllLines()
    {
        return _context.ProductLines.ToList();
    }

    public IEnumerable<ProductCategory> GetAllCategories()
    {
        return _context.ProductCategories.ToList();
    }
}
