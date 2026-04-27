using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess;

public class InMemoryPromotionRepository : IPromotionRepository
{
    private readonly List<Promotion> _promotions = [];

    public void Add(Promotion promotion)
    {
        _promotions.Add(promotion);
    }

    public IEnumerable<Promotion> GetAll()
    {
        return _promotions;
    }

    public Promotion? GetById(Guid id)
    {
        return _promotions.FirstOrDefault(p => p.Id == id);
    }

    public void Update(Promotion promotion)
    {
    }
}
