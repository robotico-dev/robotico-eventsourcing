namespace Robotico.EventSourcing;

/// <summary>
/// Abstraction for appending and reading domain events (event store). Used by event-sourced aggregates.
/// </summary>
/// <typeparam name="TId">Stream or aggregate id type.</typeparam>
/// <typeparam name="TEvent">Event type (e.g. interface or base).</typeparam>
/// <remarks>
/// Implementations must reject null streamId or null events (throw or return failed Result). See docs/design.adoc for stream semantics and Robotico.Domain, Robotico.Events integration.
/// </remarks>
public interface IEventStore<TId, TEvent>
{
    /// <summary>
    /// Appends events to the stream for the given id. Returns Result.
    /// </summary>
    /// <param name="streamId">Stream or aggregate id.</param>
    /// <param name="events">Events to append in order.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success when appended; failure on concurrency or storage error.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="streamId"/> or <paramref name="events"/> is null (implementations should throw or return a failed Result).</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<Robotico.Result.Result> AppendAsync(TId streamId, IEnumerable<TEvent> events, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads all events for the stream in order. Returns Result of event sequence.
    /// </summary>
    /// <param name="streamId">Stream or aggregate id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success with event list, or failure (e.g. stream not found — document implementation behavior).</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="streamId"/> is null (implementations should throw or return a failed Result).</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<Robotico.Result.Result<IReadOnlyList<TEvent>>> ReadAsync(TId streamId, CancellationToken cancellationToken = default);
}
