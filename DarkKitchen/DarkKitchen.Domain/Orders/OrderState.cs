namespace DarkKitchen.Domain.Orders;

public enum OrderState
{
    /// <summary>
    ///     Order is created but not yet processed.
    /// </summary>
    Pending,

    /// <summary>
    ///     Order preparation is delayed.
    /// </summary>
    Delayed,

    /// <summary>
    ///     Order is being prepared.
    /// </summary>
    Prepared,

    /// <summary>
    ///     Order is out for delivery.
    /// </summary>
    Shipping,

    /// <summary>
    ///     Order has been delivered successfully.
    /// </summary>
    Delivered,

    /// <summary>
    ///     Order could not be delivered.
    /// </summary>
    NotDelivered,

    /// <summary>
    ///     Order has been cancelled.
    /// </summary>
    Cancelled
}
