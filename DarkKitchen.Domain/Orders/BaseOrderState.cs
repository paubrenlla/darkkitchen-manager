namespace DarkKitchen.Domain.Orders;

public abstract class BaseOrderState : IOrderState
{
    protected BaseOrderState()
    {
        TransitionDate = DateTime.Now;
    }

    public abstract string Name { get; }
    public DateTime TransitionDate { get; }

    public virtual void Prepare(Order order)
    {
        throw new InvalidOperationException($"No se puede preparar un pedido en estado {Name}");
    }

    public virtual void Cancel(Order order)
    {
        throw new InvalidOperationException($"No se puede cancelar un pedido en estado {Name}");
    }

    public virtual void Ship(Order order)
    {
        throw new InvalidOperationException($"No se puede enviar un pedido en estado {Name}");
    }

    public virtual void Deliver(Order order)
    {
        throw new InvalidOperationException($"No se puede entregar un pedido en estado {Name}");
    }

    public virtual void NotDelivered(Order order)
    {
        throw new InvalidOperationException($"No se puede rechazar un pedido en estado {Name}");
    }
}
