namespace DarkKitchen.Domain.Users;

/// <summary>
///     Represents the different user roles in DarkKitchen.
/// </summary>
public enum Role
{
    /// <summary>
    ///     User that makes "Pedidos".
    /// </summary>
    Cliente,

    /// <summary>
    ///     User that manages the "Pedidos", stock and general administration.
    ///     Personal encargado de la gestión de pedidos, stock y administración general.
    /// </summary>
    Administrativo,

    /// <summary>
    ///     User that makers the "Products" for "Pedidos" fisically.
    /// </summary>
    Preparador
}
