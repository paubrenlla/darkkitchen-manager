using System.Text;
using DarkKitchen.Domain.Audit;
using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Products;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Handlers;

public class ProductAuditHandler(IAuditRepository auditRepository) :
    IAuditEventHandler<EntityCreatedEvent<Product>>,
    IAuditEventHandler<EntityModifiedEvent<Product>>,
    IAuditEventHandler<EntityDeactivatedEvent<Product>>,
    IAuditEventHandler<EntityActivatedEvent<Product>>
{
    private readonly IAuditRepository _auditRepository = auditRepository;

    public void Handle(EntityCreatedEvent<Product> domainEvent)
    {
        var auditLog = new AuditLog
        {
            EntityId = domainEvent.EntityId,
            EntityName = domainEvent.EntityName,
            ResponsibleUser = domainEvent.ResponsibleUser,
            ChangeDescription = $"Producto creado exitosamente.\nID Interno: {domainEvent.NewState.Id}\nCódigo: {domainEvent.NewState.Code}\nNombre: {domainEvent.NewState.Name}\n"
        };

        _auditRepository.Save(auditLog);
    }

    public void Handle(EntityModifiedEvent<Product> domainEvent)
    {
        var sb = new StringBuilder();
        var old = domainEvent.OldState;
        var @new = domainEvent.NewState;

        if(old.Name != @new.Name)
        {
            sb.AppendLine($"Name cambió de '{old.Name}' a '{@new.Name}'");
        }

        if(old.Description != @new.Description)
        {
            sb.AppendLine($"Description cambió de '{old.Description}' a '{@new.Description}'");
        }

        if(old.Price != @new.Price)
        {
            sb.AppendLine($"Price cambió de '{old.Price}' a '{@new.Price}'");
        }

        if(old.IsActive != @new.IsActive)
        {
            sb.AppendLine($"IsActive cambió de '{old.IsActive}' a '{@new.IsActive}'");
        }

        if(old.Line.Name != @new.Line.Name)
        {
            sb.AppendLine($"Line cambió de '{old.Line.Name}' a '{@new.Line.Name}'");
        }

        if(old.Category.Name != @new.Category.Name)
        {
            sb.AppendLine($"Category cambió de '{old.Category.Name}' a '{@new.Category.Name}'");
        }

        if(!old.Images.Select(i => i.Url).SequenceEqual(@new.Images.Select(i => i.Url)))
        {
            sb.AppendLine("Las imágenes del producto fueron modificadas.");
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

    public void Handle(EntityDeactivatedEvent<Product> domainEvent)
    {
        var auditLog = new AuditLog
        {
            EntityId = domainEvent.EntityId,
            EntityName = domainEvent.EntityName,
            ResponsibleUser = domainEvent.ResponsibleUser,
            ChangeDescription = $"Producto dado de baja.\nID Interno: {domainEvent.OldState.Id}\nCódigo: {domainEvent.OldState.Code}\nNombre: {domainEvent.OldState.Name}\n"
        };

        _auditRepository.Save(auditLog);
    }

    public void Handle(EntityActivatedEvent<Product> domainEvent)
    {
        var auditLog = new AuditLog
        {
            EntityId = domainEvent.EntityId,
            EntityName = domainEvent.EntityName,
            ResponsibleUser = domainEvent.ResponsibleUser,
            ChangeDescription = $"Producto dado de alta.\nID Interno: {domainEvent.NewState.Id}\nCódigo: {domainEvent.NewState.Code}\nNombre: {domainEvent.NewState.Name}\n"
        };

        _auditRepository.Save(auditLog);
    }
}
