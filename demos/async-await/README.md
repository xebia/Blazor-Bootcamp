# Async/Await Demos (.NET 10 + Spectre.Console)

A small CLI app for teaching async concepts:

- `async` / `await` basics and continuation scheduling
- `Task.WhenAll` behavior (exceptions + cancellation)
- `ValueTask` fast-path vs pitfalls
- `System.Threading.Channels` producer/consumer with backpressure

## Prereqs

- .NET SDK 10 (`dotnet --version` should show `10.x`)

## Run

From repo root:

- List demos:
  - `dotnet run --project src/AsyncAwaitDemos -- list`

- await basics:
  - `dotnet run --project src/AsyncAwaitDemos -- await-basics --iterations 5 --delay-ms 150`

- Task.WhenAll with one failure:
  - `dotnet run --project src/AsyncAwaitDemos -- task-whenall --iterations 5 --delay-ms 200 --fail-on 3`

- Task.WhenAll with cancellation:
  - `dotnet run --project src/AsyncAwaitDemos -- task-whenall --iterations 10 --delay-ms 200 --cancel-after-ms 350`

- ValueTask demo:
  - `dotnet run --project src/AsyncAwaitDemos -- valuetask --iterations 50 --hit-rate 80 --delay-ms 30`

- ValueTask pitfall (double await):
  - `dotnet run --project src/AsyncAwaitDemos -- valuetask --iterations 3 --double-await`

- Channel demo:
  - `dotnet run --project src/AsyncAwaitDemos -- channel --capacity 5 --items 20 --producer-delay-ms 50 --consumer-delay-ms 150`

## Tips for teaching

- Use `--delay-ms` to exaggerate timing.
- Start with `await-basics`, then move to `task-whenall`.
- For `valuetask`, emphasize:
  - prefer `Task` unless you *know* you need `ValueTask`
  - don’t await a `ValueTask` more than once (convert to `Task` if needed)
- For `channel`, discuss backpressure (`BoundedChannelFullMode.Wait`).
