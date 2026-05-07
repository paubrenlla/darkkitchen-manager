using System.Text;
using DarkKitchen.Domain.Audit;
using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Promotions;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Handlers;

public class PromotionAuditHandler(IAuditRepository auditRepository) :
    IAuditEventHandler<EntityCreatedEvent<Promotion>>,
    IAuditEventHandler<EntityModifiedEvent<Promotion>>
{
    private readonly IAuditRepository _auditRepository = auditRepository;

    public void Handle(EntityCreatedEvent<Promotion> domainEvent)
    {
        var auditLog = new AuditLog
        {
            EntityId = domainEvent.EntityId,
            EntityName = domainEvent.EntityName,
            ResponsibleUser = domainEvent.ResponsibleUser,
            ChangeDescription = $"Promoción creada exitosamente.\nID Interno: {domainEvent.NewState.Id}\nNombre: {domainEvent.NewState.Name}\nDescuento: {domainEvent.NewState.DiscountPercentage}%\n"
        };

        _auditRepository.Save(auditLog);
    }

    public void Handle(EntityModifiedEvent<Promotion> domainEvent)
    {
        var sb = new StringBuilder();
        var old = domainEvent.OldState;
        var @new = domainEvent.NewState;

        if(old.Name != @new.Name)
        {
            sb.AppendLine($"Name cambió de '{old.Name}' a '{@new.Name}'");
        }

        if(old.DiscountPercentage != @new.DiscountPercentage)
        {
            sb.AppendLine($"DiscountPercentage cambió de '{old.DiscountPercentage}' a '{@new.DiscountPercentage}'");
        }

        if(old.StartDate != @new.StartDate)
        {
            sb.AppendLine($"StartDate cambió de '{old.StartDate}' a '{@new.StartDate}'");
        }

        if(old.EndDate != @new.EndDate)
        {
            sb.AppendLine($"EndDate cambió de '{old.EndDate}' a '{@new.EndDate}'");
        }

        if(!old.Products.Select(p => p.Code).SequenceEqual(@new.Products.Select(p => p.Code)))
        {
            sb.AppendLine("La lista de productos de la promoción fue modificada.");
        }

        var auditLog = new AuditLog
        {
            EntityId = domainEvent.EntityId,
            EntityName = domainEvent.EntityName,
            ResponsibleUser = domainEvent.ResponsibleUser,
            ChangeDescription = sb.ToString()
        };

        _auditRepository.Save(auditLog);
    }
}
