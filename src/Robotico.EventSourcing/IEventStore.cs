namespace Robotico.EventSourcing;

/// <summary>
/// Abstraction for appending and reading domain events (event store). Used by event-sourced aggregates.
/// </summary>
/// <typeparam name="TId">Stream or aggregate id type.</typeparam>
/// <typeparam name="TEvent">Event type (e.g. interface or base).</typeparam>
public interface IEventStore<TId, TEvent>
{
    /// <summary>
    /// Appends events to the stream for the given id. Returns Result.
    /// </summary>
    Task<Robotico.Result.Result> AppendAsync(TId streamId, IEnumerable<TEvent> events, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads all events for the stream in order. Returns Result of event sequence.
    /// </summary>
    Task<Robotico.Result.Result<IReadOnlyList<TEvent>>> ReadAsync(TId streamId, CancellationToken cancellationToken = default);
}
