using Robotico.EventSourcing;
using Xunit;

namespace Robotico.EventSourcing.Tests;

public sealed class EventStoreTests
{
    [Fact]
    public void IEventStore_contract_exists()
    {
        Assert.True(typeof(IEventStore<object, object>).IsInterface);
    }
}
