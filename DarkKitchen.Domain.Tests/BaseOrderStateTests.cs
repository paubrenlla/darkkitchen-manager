using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Tests;

[TestClass]
public class OrderStateTests
{
    private Order _order = null!;

    [TestInitialize]
    public void Setup()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        _order = new Order(address);
    }

    [TestMethod]
    public void BaseOrderState_WhenCreated_ShouldSetTransitionDateToNow()
    {
        DateTime before = DateTime.Now;
        var state = new PendingState();
        DateTime after = DateTime.Now;

        Assert.IsTrue(state.TransitionDate >= before);
        Assert.IsTrue(state.TransitionDate <= after);
    }
}
