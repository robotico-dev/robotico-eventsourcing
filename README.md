# Robotico.EventSourcing

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![GitHub Packages](https://img.shields.io/badge/GitHub%20Packages-Robotico.EventSourcing-blue?logo=github)](https://github.com/robotico-dev/robotico-eventsourcing-csharp/packages)

Event sourcing: event store abstraction and aggregates that fold events. Result-based. Depends on Robotico.Result. Add when multiple bounded contexts share the same event-sourcing model.

## Robotico dependencies

```mermaid
flowchart LR
  A[Robotico.EventSourcing] --> B[Robotico.Result]
```

## Installation

```bash
dotnet add package Robotico.EventSourcing
```

## License

See repository license file.
