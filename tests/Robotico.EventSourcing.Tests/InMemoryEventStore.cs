using System.Collections.Concurrent;
using Robotico.EventSourcing;
using Robotico.Result.Errors;

namespace Robotico.EventSourcing.Tests;

/// <summary>
/// In-memory implementation of <see cref="IEventStore{TId, TEvent}"/> for tests.
/// </summary>
public sealed class InMemoryEventStore<TId, TEvent> : IEventStore<TId, TEvent>
    where TId : notnull
{
    private readonly ConcurrentDictionary<TId, List<TEvent>> _streams = new();

    /// <inheritdoc />
    public Task<Robotico.Result.Result> AppendAsync(TId streamId, IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(streamId);
        ArgumentNullException.ThrowIfNull(events);

        List<TEvent> list = events.ToList();
        if (list.Count == 0)
        {
            return Task.FromResult(Robotico.Result.Result.Success());
        }

        _streams.AddOrUpdate(streamId, _ => [.. list], (_, existing) =>
        {
            existing.AddRange(list);
            return existing;
        });

        return Task.FromResult(Robotico.Result.Result.Success());
    }

    /// <inheritdoc />
    public Task<Robotico.Result.Result<IReadOnlyList<TEvent>>> ReadAsync(TId streamId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(streamId);

        if (_streams.TryGetValue(streamId, out List<TEvent>? list))
        {
            return Task.FromResult(Robotico.Result.Result.Success<IReadOnlyList<TEvent>>(list));
        }

        return Task.FromResult(Robotico.Result.Result.Error<IReadOnlyList<TEvent>>(new SimpleError("Stream not found.", "STREAM_NOT_FOUND")));
    }
}
