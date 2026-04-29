namespace DarkKitchen.Domain.Orders.States;

public abstract class BaseOrderState : IOrderState
{
    public abstract OrderState State { get; }

    public virtual void Prepare(Order order)
    {
        throw new InvalidOperationException($"No se puede preparar un pedido en estado {State}");
    }

    public virtual void Cancel(Order order)
    {
        throw new InvalidOperationException($"No se puede cancelar un pedido en estado {State}");
    }

    public virtual void Ship(Order order)
    {
        throw new InvalidOperationException($"No se puede enviar un pedido en estado {State}");
    }

    public virtual void Deliver(Order order)
    {
        throw new InvalidOperationException($"No se puede entregar un pedido en estado {State}");
    }

    public virtual void NotDelivered(Order order)
    {
        throw new InvalidOperationException($"No se puede rechazar un pedido en estado {State}");
    }
}
