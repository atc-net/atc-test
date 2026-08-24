# Working With Atc.Test

A long-form guide to using `Atc.Test` in a real test suite. The [README](../README.md) covers
installation and the fastest path to a first passing test; this document goes deeper into the
data attributes, `[Frozen]` semantics, customization, and the helper extensions.

## Table of Content

* [The Problem This Solves](#the-problem-this-solves)
* [Data Attributes](#data-attributes)
    * [AutoNSubstituteData](#autonsubstitutedata)
    * [InlineAutoNSubstituteData](#inlineautonsubstitutedata)
    * [MemberAutoNSubstituteData](#memberautonsubstitutedata)
    * [ClassAutoNSubstituteData](#classautonsubstitutedata)
    * [Choosing Between Them](#choosing-between-them)
* [The Frozen Attribute](#the-frozen-attribute)
    * [Basic Frozen Reuse](#basic-frozen-reuse)
    * [Positional Frozen Reuse](#positional-frozen-reuse)
    * [Exact-Type Promotion](#exact-type-promotion)
    * [What Is Not Promoted](#what-is-not-promoted)
* [Auto-Registration of Customizations](#auto-registration-of-customizations)
* [The FixtureFactory](#the-fixturefactory)
* [Helper Extensions](#helper-extensions)
* [Where to Go Next](#where-to-go-next)

## The Problem This Solves

Without a library like this, tests accumulate noise. Every test method wires up its own fixture,
freezes the right dependencies, creates substitutes, and constructs the system under test. With an
evolving constructor graph this becomes a maintenance burden: adding a single constructor parameter
can cascade into touching dozens or hundreds of test files.

`Atc.Test` applies one principle — **you list only the parameters that matter to your test.**
Everything else is generated, substituted, and wired up for you.

The practical consequences:

* Constructor churn becomes **O(1)** instead of **O(N tests)**. Add a dependency; the fixture supplies it.
* Divergent per-developer mock styles are replaced by one factory and one set of reuse semantics.
* Duplicate substitutes for a logically single collaborator are prevented by `[Frozen]`.
* Shared conventions — recursion handling, custom generators — are registered once and inherited everywhere.

This pays off most in mid-to-large suites, in domain services with complex constructor graphs, and on
teams that care about refactor safety. For a very small or short-lived codebase, plain xUnit with
hand-rolled mocks may be enough.

## Data Attributes

Four attributes cover the common scenarios. Picking the right one is most of the learning curve.

### AutoNSubstituteData

The workhorse. Every parameter is auto-generated; interfaces and abstract classes become NSubstitute
substitutes.

```csharp
[Theory]
[AutoNSubstituteData]
public void GetValue_ShouldReturnDataFromService(
    [Frozen] IMyService service,
    MyController sut)
{
    service.GetValue().Returns(42);

    var result = sut.Get();

    result.Should().Be(42);
}
```

`service` is a substitute for `IMyService`. Because it is `[Frozen]`, the *same* instance is injected
into `MyController`, so the `Returns` setup is visible to the system under test. You never write
`Substitute.For<IMyService>()` or `new Fixture()`.

### InlineAutoNSubstituteData

Supply specific values for the leading parameters and let AutoFixture generate the rest — the
auto-mocking equivalent of xUnit's `[InlineData]`.

```csharp
[Theory]
[InlineAutoNSubstituteData(10, 20)]
[InlineAutoNSubstituteData(5, 5)]
[InlineAutoNSubstituteData(0, -1)]
public void Add_ShouldWorkWithSpecificValues(int a, int b, Calculator sut)
    => sut.Add(a, b).Should().Be(a + b);
```

Inline values are assigned to parameters in order. Anything left over is auto-generated. Use this for
boundary conditions and edge cases.

### MemberAutoNSubstituteData

For test data that cannot be expressed as compile-time constants. Works like `[MemberData]`, but
augments the supplied rows with generated specimens.

```csharp
public static IEnumerable<object?[]> TestCases()
{
    yield return new object?[] { 1, 2, 3 };
    yield return new object?[] { 10, 20, 30 };
    yield return new object?[] { -1, 1, 0 };
}

[Theory]
[MemberAutoNSubstituteData(nameof(TestCases))]
public void Add_ShouldReturnExpectedResult(
    int a,
    int b,
    int expected,
    Calculator sut)
    => sut.Add(a, b).Should().Be(expected);
```

The member supplies `a`, `b` and `expected`; `sut` is generated. This attribute is also the only one
that supports [exact-type promotion](#exact-type-promotion).

### ClassAutoNSubstituteData

For test data complex enough to deserve its own type — construction logic, conditional rows, or state
shared between cases.

```csharp
public class CalculatorTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1, 2, 3 };
        yield return new object[] { 10, 20, 30 };
        yield return new object[] { -1, 1, 0 };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

[Theory]
[ClassAutoNSubstituteData(typeof(CalculatorTestCases))]
public void Add_ShouldReturnExpectedResult(int a, int b, int expected, Calculator sut)
    => sut.Add(a, b).Should().Be(expected);
```

### Choosing Between Them

| Situation | Attribute |
|-----------|-----------|
| Nothing needs to be pinned | `AutoNSubstituteData` |
| A few compile-time constant cases | `InlineAutoNSubstituteData` |
| Cases need runtime construction, or you want frozen promotion | `MemberAutoNSubstituteData` |
| Cases warrant a reusable, self-contained type | `ClassAutoNSubstituteData` |

## The Frozen Attribute

Freezing means: *the same instance is reused for every other parameter in this test that needs that
type.* It is what makes "set up a mock, then verify the SUT used it" work.

### Basic Frozen Reuse

```csharp
[Theory]
[AutoNSubstituteData]
public void Handle_ShouldUseFrozenDependency(
    [Frozen] IMyService service,
    MyHandler sut)
{
    service.DoWork().Returns(true);

    var result = sut.Handle();

    result.Should().BeTrue();
    service.Received(1).DoWork();
}
```

Without `[Frozen]`, `service` and the `IMyService` injected into `MyHandler` would be **different
instances**, the setup would be invisible to the SUT, and the `Received` assertion would fail. This is
the single most common source of confusion for newcomers.

### Positional Frozen Reuse

With `InlineAutoNSubstituteData` and `ClassAutoNSubstituteData`, a value supplied at the same index as
a `[Frozen]` parameter is frozen and reused.

```csharp
[Theory]
[InlineAutoNSubstituteData(42)]
public void Positional_Frozen_Reuses_Inline_Value(
    [Frozen] int number,
    SomeConsumer consumer)
{
    consumer.NumberDependency.Should().Be(number);
}
```

The inline `42` sits at index 0, mapping to `[Frozen] int number`, and is then reused when constructing
`SomeConsumer`.

### Exact-Type Promotion

`MemberAutoNSubstituteData` adds a capability the other attributes do not have. If the member row
supplies a value for an earlier parameter, and a *later* parameter is marked `[Frozen]` with the same
exact type, the earlier value is promoted rather than a new specimen being created.

```csharp
public static IEnumerable<object?[]> ServiceRow()
{
    yield return new object?[] { Substitute.For<IMyService>() };
}

[Theory]
[MemberAutoNSubstituteData(nameof(ServiceRow))]
public void Promotion_Reuses_Earlier_Same_Type(
    IMyService supplied,
    [Frozen] IMyService frozenLater,
    NeedsService consumer)
{
    frozenLater.Should().BeSameAs(supplied);
    consumer.Service.Should().BeSameAs(supplied);
}
```

This exists because member rows commonly supply only a leading subset of parameters. Class data is
normally fully positional, where implicit promotion would hide mistakes — so it is deliberately not
applied there.

### What Is Not Promoted

Promotion requires an **exact declared type match**. A concrete instance implementing two interfaces is
not cross-promoted between them.

```csharp
public interface IFoo { }
public interface IBar { }
public class DualImpl : IFoo, IBar { }

public static IEnumerable<object?[]> DualRow()
{
    yield return new object?[] { new DualImpl() };
}

[Theory]
[MemberAutoNSubstituteData(nameof(DualRow))]
public void Different_Interface_Not_Promoted(
    IFoo foo,
    [Frozen] IBar bar,
    UsesBar consumer)
{
    bar.Should().NotBeSameAs(foo);
    consumer.Bar.Should().BeSameAs(bar);
}
```

Even though `DualImpl` implements both interfaces, the frozen `IBar` does not reuse it. This prevents
cross-interface bleed and the subtle bugs that follow. If you *want* reuse, declare the supplying
parameter with the same type as the frozen one.

## Auto-Registration of Customizations

Any `ICustomization` or `ISpecimenBuilder` decorated with `[AutoRegister]` is discovered and applied to
every fixture the library creates.

```csharp
[AutoRegister]
public class GuidCustomization : ICustomization
{
    public void Customize(IFixture fixture)
        => fixture.Register(() => Guid.NewGuid());
}
```

No registration call anywhere. This is the mechanism for establishing project-wide testing conventions:

* Default values for domain types AutoFixture cannot construct
* Recursion guards for awkward object graphs
* Realistic string generation instead of random GUID-like noise
* Custom builders for types with non-trivial construction

Two constraints worth knowing:

* The decorated type **must have a parameterless constructor**.
* Discovery scans assemblies loaded into the current `AppDomain`. Types in your test project are found
  automatically; a customization in a separate assembly that nothing references may not be loaded yet.

A type marked `[AutoRegister]` that implements neither interface throws `NotSupportedException` when a
fixture is created.

## The FixtureFactory

Every data attribute calls `FixtureFactory.Create()`, which applies three customizations:

1. **`RecursionCustomization`** — replaces AutoFixture's throwing behaviour with `OmitOnRecursionBehavior`,
   so a circular reference is omitted rather than failing the test. This is why a property on a generated
   object is sometimes unexpectedly `null`.
2. **`AutoRegisterCustomization`** — discovers and applies everything marked `[AutoRegister]`.
3. **`AutoNSubstituteCustomization`** — configures NSubstitute for interfaces and abstract classes, with
   `ConfigureMembers = false` and `GenerateDelegates = true`.

You can call it directly when you need a fixture outside the attributes:

```csharp
[Fact]
public void Manual_Fixture_Example()
{
    var fixture = FixtureFactory.Create();
    var sut = fixture.Create<MyHandler>();

    sut.Handle().Should().NotBeNull();
}
```

## Helper Extensions

| Helper | Purpose |
|--------|---------|
| `EquivalencyAssertionOptionsExtensions` | Date precision and `JsonElement` handling for `BeEquivalentTo`. |
| `SubstituteExtensions` | Inspect substitutes, retrieve arguments, wait for calls. |
| `TaskExtensions` | Await with timeouts; await many tasks. |
| `ObjectExtensions` | Reach protected members via reflection. |
| `StringExtensions` | Compare text, XML and JSON ignoring formatting. |
| `FixtureFactory` | The central, consistently customized `IFixture`. |

### Equivalency options

```csharp
actual.Should().BeEquivalentTo(expected, o => o.CompareDateTimeUsingCloseTo());
```

`CompareDateTimeUsingCloseTo` defaults to 1000 ms precision and covers both `DateTime` and
`DateTimeOffset`. Override with an `int` (milliseconds) or a `TimeSpan`:

```csharp
o => o.CompareDateTimeUsingCloseTo(precision: 500)
o => o.CompareDateTimeUsingCloseTo(TimeSpan.FromSeconds(2))
```

`CompareJsonElementUsingJson()` compares `JsonElement` values by their underlying JSON representation
rather than attempting to compare the struct directly.

### Substitute inspection

```csharp
sut.Process("hello");

var argument = service.ReceivedCallWithArgument<string>();
argument.Should().Be("hello");
```

`ReceivedCallWithArgument<T>` asserts that **exactly one** argument of type `T` was received across all
calls and returns it. When several are expected, use `ReceivedCallsWithArguments<T>`, which asserts the
collection is non-empty and returns all of them.

Both accept the usual `because` / `becauseArgs` phrasing used throughout AwesomeAssertions.

### Waiting for asynchronous calls

When the call happens on a background thread or after an event:

```csharp
sut.StartBackgroundProcessing();

await service.WaitForCall(x => x.DoWork());
```

The default wait is 5 seconds; pass a `TimeSpan` to change it. Overloads exist for `Action<T>`,
`Func<T, Task>` and `Func<T, ValueTask<TResult>>`. Use `WaitForCallForAnyArgs` to ignore arguments.

### Task helpers

```csharp
var result = await someTask.AddTimeout(TimeSpan.FromSeconds(5));
```

`AddTimeout` throws `TimeoutException` if the task does not complete in time, defaulting to 5 seconds.
**When a debugger is attached the timeout is ignored**, so stepping through a test does not spuriously
fail it.

```csharp
var results = await new[] { task1, task2, task3 }.AwaitTasks();
```

`AwaitTasks` wraps `Task.WhenAll` in a form that reads better in a test.

### Protected members

```csharp
var result = sut.InvokeProtectedMethod<int>("CalculateInternal", input);
```

`HasProperties()` guards `BeEquivalentTo`, which throws for objects with no properties:

```csharp
if (obj.HasProperties())
{
    obj.Should().BeEquivalentTo(expected);
}
```