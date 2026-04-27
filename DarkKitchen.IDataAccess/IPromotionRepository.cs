using DarkKitchen.Domain;

namespace DarkKitchen.IDataAccess;

public interface IPromotionRepository
{
    void Add(Promotion promotion);
    IEnumerable<Promotion> GetAll();
    Promotion? GetById(Guid id);
    void Update(Promotion promotion);
}
