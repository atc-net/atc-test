---
applyTo: '**/*.{cs}'
---

C# Usage

- Always use latest C# features:
  - File-scoped namespaces (`namespace X;` syntax)
  - Leverage primary constructors for simple classes
  - Collection initializers/expressions
  - Pattern matching with `is null` and `is not null` instead of `== null` and `!= null`
  - Init-only properties (`init` accessor) for immutable objects when records aren't appropriate
  - Global using directives for commonly used imports
  - Default interface implementations when appropriate
- Enable and respect nullable reference types (`string?` vs `string`)
- Use records for immutable data structures
- Mark all types as sealed unless designed for inheritance
- Use `var` when the type is obvious from the right side of the assignment
- Use clear names instead of making comments
- Avoid using exceptions for control flow:
  - When exceptions are thrown, always use meaningful exceptions following .NET conventions
  - Use `UnreachableException` to signal unreachable code, that cannot be reached by tests
- Async/await best practices:
  - Avoid `async void` except for event handlers
  - Use `ConfigureAwait(false)` in library code
  - Propagate cancellation tokens
  - Use `Task.WhenAll` for parallel task execution
- Logging guidance:
  - Log only meaningful events at appropriate severity levels
  - Logging messages should not include a period
  - Use structured source generated logging
- Code comments:
  - Don't add comments unless the code is truly not expressing the intent
  - Never remove TODO: comments