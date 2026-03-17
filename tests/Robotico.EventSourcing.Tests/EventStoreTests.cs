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

    private static readonly string[] Stream1Events = ["e1", "e2", "e3"];
    private static readonly string[] StreamLawEvents = ["e1", "e2"];
    private static readonly int[] SingleIntEvent = [1];
    private static readonly string[] SingleStringEvent = ["e"];

    [Fact]
    public async Task AppendAsync_then_ReadAsync_returns_same_events_in_order()
    {
        InMemoryEventStore<string, string> store = new();
        Robotico.Result.Result appendResult = await store.AppendAsync("stream1", Stream1Events);
        Assert.True(appendResult.IsSuccess());

        Robotico.Result.Result<IReadOnlyList<string>> readResult = await store.ReadAsync("stream1");
        Assert.True(readResult.IsSuccess(out IReadOnlyList<string>? events, out _));
        Assert.Equal(3, events!.Count);
        Assert.Equal("e1", events[0]);
        Assert.Equal("e2", events[1]);
        Assert.Equal("e3", events[2]);
    }

    [Fact]
    public async Task ReadAsync_missing_stream_returns_error()
    {
        InMemoryEventStore<string, int> store = new();
        Robotico.Result.Result<IReadOnlyList<int>> readResult = await store.ReadAsync("missing");
        Assert.True(readResult.IsError(out _));
    }

    [Fact]
    public async Task AppendAsync_null_streamId_throws()
    {
        InMemoryEventStore<string, int> store = new();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await store.AppendAsync(null!, SingleIntEvent));
    }

    [Fact]
    public async Task AppendAsync_null_events_throws()
    {
        InMemoryEventStore<string, int> store = new();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await store.AppendAsync("stream", null!));
    }

    [Fact]
    public async Task ReadAsync_null_streamId_throws()
    {
        InMemoryEventStore<string, int> store = new();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await store.ReadAsync(null!));
    }

    [Fact]
    public async Task AppendAsync_respects_cancellation()
    {
        using CancellationTokenSource cts = new();
        await cts.CancelAsync();
        InMemoryEventStore<string, string> store = new();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await store.AppendAsync("s", SingleStringEvent, cts.Token));
    }

    [Fact]
    public async Task ReadAsync_respects_cancellation()
    {
        using CancellationTokenSource cts = new();
        await cts.CancelAsync();
        InMemoryEventStore<string, int> store = new();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await store.ReadAsync("s", cts.Token));
    }

    /// <summary>
    /// Law: append events in order; read returns same events in same order.
    /// </summary>
    [Fact]
    public async Task EventStore_law_append_read_ordering()
    {
        InMemoryEventStore<string, string> store = new();
        Robotico.Result.Result appendResult = await store.AppendAsync("stream-law", StreamLawEvents);
        Assert.True(appendResult.IsSuccess());
        Robotico.Result.Result<IReadOnlyList<string>> readResult = await store.ReadAsync("stream-law");
        Assert.True(readResult.IsSuccess(out IReadOnlyList<string>? list, out _));
        Assert.Equal(2, list!.Count);
        Assert.Equal("e1", list[0]);
        Assert.Equal("e2", list[1]);
    }

    /// <summary>
    /// Law: append N events; read returns same N events in order (parameterized).
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task EventStore_law_append_read_ordering_count(int eventCount)
    {
        InMemoryEventStore<string, string> store = new();
        string[] events = Enumerable.Range(1, eventCount).Select(i => $"ev-{i}").ToArray();
        Robotico.Result.Result appendResult = await store.AppendAsync("stream-theory", events);
        Assert.True(appendResult.IsSuccess());
        Robotico.Result.Result<IReadOnlyList<string>> readResult = await store.ReadAsync("stream-theory");
        Assert.True(readResult.IsSuccess(out IReadOnlyList<string>? list, out _));
        Assert.Equal(eventCount, list!.Count);
        for (int i = 0; i < eventCount; i++)
        {
            Assert.Equal(events[i], list[i]);
        }
    }
}
