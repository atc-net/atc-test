# Atc.Test

![NuGet Version](https://img.shields.io/nuget/v/Atc.Test.svg?logo=nuget&style=for-the-badge)

`Atc.Test` is a .NET helper library that streamlines authoring tests with xUnit v3, AutoFixture, NSubstitute, and AwesomeAssertions. It provides rich data attributes, automatic specimen customization, and ergonomic frozen value reuse to reduce ceremony and improve test readability.

## Atc.Test in 30 Seconds

Every dependency your test does not care about is noise. `Atc.Test` lets you declare only the parameters the test is actually about — everything else is generated, substituted and wired for you.

**Without `Atc.Test`**

```csharp
[Fact]
public void Should_Return_Order_When_Found()
{
    // Arrange
    var repository = Substitute.For<IOrderRepository>();
    var pricing = Substitute.For<IPricingService>();
    var audit = Substitute.For<IAuditLog>();
    var clock = Substitute.For<TimeProvider>();
    var sut = new OrderService(repository, pricing, audit, clock);

    var orderId = Guid.NewGuid();
    var order = new Order { Id = orderId, Name = "Test order" };
    repository.Get(orderId).Returns(order);

    // Act
    var result = sut.GetOrder(orderId);

    // Assert
    result.Should().BeSameAs(order);
}
```

**With `Atc.Test`**

```csharp
[Theory]
[AutoNSubstituteData]
public void Should_Return_Order_When_Found(
    [Frozen] IOrderRepository repository,
    Guid orderId,
    Order order,
    OrderService sut)
{
    // Arrange
    repository.Get(orderId).Returns(order);

    // Act
    var result = sut.GetOrder(orderId);

    // Assert
    result.Should().BeSameAs(order);
}
```

`IPricingService`, `IAuditLog` and `TimeProvider` are still supplied to the constructor — they are simply no longer in your way. Add a fifth constructor parameter tomorrow and this test does not change.

## Table of Content

* [Atc.Test in 30 Seconds](#atctest-in-30-seconds)
* [Cheat Sheet](#cheat-sheet)
* [Features](#features)
* [Getting Started](#getting-started)
    * [Install Package](#install-package)
    * [Version Compatibility](#version-compatibility)
    * [Why xUnit Must Be Referenced Directly](#why-xunit-must-be-referenced-directly)
    * [First Test Examples](#first-test-examples)
* [Common Recipes](#common-recipes)
* [Built-in Specimen Support](#built-in-specimen-support)
* [Working With Atc.Test (deep dive)](docs/working-with.md)
* [Advanced Usage](#advanced-usage)
    * [Frozen Reuse Scenarios](#frozen-reuse-scenarios)
    * [Auto Registration of Customizations](#auto-registration-of-customizations)
    * [Helper Extensions](#helper-extensions)
* [Troubleshooting / FAQ](#troubleshooting--faq)
* [Why Atc.Test](#why-atctest)
* [Requirements](#requirements)
* [Migrating from FluentAssertions](#migrating-from-fluentassertions)
* [How to Contribute](#how-to-contribute)

## Cheat Sheet

Most users only ever need this table.

| What you want | Use | Notes |
|---------------|-----|-------|
| Generate every parameter automatically | `[Theory] [AutoNSubstituteData]` | Interfaces/abstract types become NSubstitute substitutes. |
| Mix fixed values with generated ones | `[Theory] [InlineAutoNSubstituteData(2, 3)]` | Inline values fill the leading parameters. |
| Drive a theory from a member | `[Theory] [MemberAutoNSubstituteData(nameof(Source))]` | Supports **exact-type promotion** for `[Frozen]`. |
| Drive a theory from a class | `[Theory] [ClassAutoNSubstituteData(typeof(Source))]` | Positional frozen injection only, no promotion. |
| Reuse one instance across the graph | `[Frozen] IMyService service` | The same instance is injected into everything needing exactly `IMyService`. |
| Teach the fixture about your own type | `[AutoRegister]` on an `ICustomization` / `ISpecimenBuilder` | Discovered automatically, no registration call needed. |
| Build a fixture by hand | `FixtureFactory.Create()` | Same configuration the attributes use. |
| Fail a hanging async test fast | `await task.AddTimeout()` | Defaults to 5s, and is **bypassed when a debugger is attached**. |

## Features

* Data attributes integrating AutoFixture + NSubstitute: `AutoNSubstituteData`, `InlineAutoNSubstituteData`, `MemberAutoNSubstituteData`, `ClassAutoNSubstituteData`.
* Automatic interface/abstract substitution via NSubstitute.
* Exact-type frozen promotion for member data (reuse supplied instance across later `[Frozen]` parameters).
* Deterministic fixture configuration with opt‑in auto-registration of custom `ICustomization` / `ISpecimenBuilder` via `[AutoRegister]`.
* Built-in specimen support for types AutoFixture handles poorly or not at all — `CancellationToken`, `DateOnly`, `TimeOnly`, `Uri`, `TimeProvider`, immutable collections and recursive graphs.
* Convenience extensions: equivalency options, substitute inspection helpers, task timeout helpers, object protected member access.
* Clear separation of concerns: you own the xUnit runner/version.

## Getting Started

### Install Package

Add `Atc.Test` to your test project along with explicit references to xUnit and the test SDK:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.6.0" />
    <PackageReference Include="xunit.v3" Version="4.0.0" />
    <PackageReference Include="Atc.Test" Version="$(LatestOrPinned)" />
  </ItemGroup>
</Project>
```

### Version Compatibility

| `Atc.Test` | xUnit | Target framework | Assertions |
|------------|-------|------------------|------------|
| 3.x | xUnit v3 (`xunit.v3` 4.x) | `net10.0` | AwesomeAssertions |

For releases prior to 3.0.0, see the [CHANGELOG](CHANGELOG.md).

### Why xUnit Must Be Referenced Directly

`Atc.Test` depends on `xunit.v3.extensibility.core` (the extensibility surface) but intentionally does **not** bring in the `xunit.v3` meta-package:

* Avoid forcing a specific xUnit runner/meta-package on your project.
* Let you pin or float the xUnit version independently.
* Keep framework + runner decisions in your test project for predictable upgrades.
* Preserve the library’s focus: providing attributes/utilities instead of prescribing test infrastructure.

If you want a different xUnit patch/minor version, change the `<PackageReference Include="xunit.v3" ... />` line—no changes to `Atc.Test` required.

#### xUnit v3 Only (Incompatible With v2)

`Atc.Test` relies on xUnit v3 extensibility APIs:

* Async data attribute signature: `ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(...)`.
* `ITheoryDataRow` & metadata (Label, Explicit, Timeout) preservation.
* `DisposalTracker` parameter passed to data attributes.

These do not exist in xUnit v2. Attempting to use a v2 framework or runner will result in discovery failures or compile errors.

| Scenario | Outcome |
|----------|---------|
| Replace `xunit.v3` with `xunit` (v2) | Build errors: missing v3 types & method signatures |
| Run with legacy v2 runner | Test discovery fails (no v3 discovery support) |
| Mix projects: some v2, some using `Atc.Test` | Allowed, but they must not share v3-based base test classes |
| Remove explicit `xunit.v3` reference | Build error / missing types (transitive reference intentionally absent) |

Optional guard rails (not included by default):

```xml
<!-- Example MSBuild check you can copy into a test project -->
<Target Name="ValidateXunitV3" BeforeTargets="Build">
    <Error Condition="!Exists('$(NuGetPackageRoot)xunit.v3/')"
                 Text="Atc.Test requires an explicit PackageReference to xunit.v3 in the test project." />
</Target>
```

“Why no v2 support?” the answer is simply that the library embraces the cleaner v3 data extensibility model; back-porting would require a parallel code path and reduce clarity.

### First Test Examples

```csharp
public class CalculatorTests
{
    [Theory]
    [AutoNSubstituteData]
    public void AutoData_Generates_Specimens(int a, int b, Calculator sut)
        => sut.Add(a, b).Should().Be(a + b);

    [Theory]
    [InlineAutoNSubstituteData(2, 3)]
    public void InlineAutoData_Mixes_Inline_And_Auto(int a, int b, Calculator sut)
        => sut.Add(a, b).Should().Be(5);

    public static IEnumerable<object?[]> MemberSource()
    {
        yield return new object?[] { 1, 2 };
        yield return new object?[] { 10, 20 };
    }

    [Theory]
    [MemberAutoNSubstituteData(nameof(MemberSource))]
    public void MemberAutoData_Augments_Member_Data(int a, int b, Calculator sut)
        => sut.Add(a, b).Should().Be(a + b);
}
```

All remaining parameters (after inline/member supplied ones) are created via an AutoFixture `IFixture` that substitutes interfaces/abstract classes using NSubstitute.

> **Note**
> NSubstitute is used automatically when the requested type is an interface or abstract class.

## Common Recipes

### Mock one dependency, auto-generate the rest

Decorate the one you care about with `[Frozen]`. The same instance is injected into the system under test.

```csharp
[Theory]
[AutoNSubstituteData]
public void Should_Persist_Order(
    [Frozen] IOrderRepository repository,
    Order order,
    OrderService sut)
{
    sut.Save(order);

    repository.Received(1).Save(order);
}
```

### Assert a substitute was called with a specific argument

`ReceivedCallWithArgument<T>` returns the single argument of type `T` across all received calls, so you can assert on it directly instead of writing an `Arg.Is` predicate. It fails if there is not exactly one.

```csharp
sut.Save(order);

var saved = repository.ReceivedCallWithArgument<Order>();

saved.Status.Should().Be(OrderStatus.Pending);
```

Use `ReceivedCallsWithArguments<T>` when several arguments of that type are expected; it returns them all.

### Wait for an asynchronous call

For code that dispatches work in the background, `WaitForCall` waits until the call arrives or the timeout elapses.

```csharp
await handler.WaitForCall(x => x.Handle(Arg.Any<Message>()));
```

`WaitForCallForAnyArgs` is the argument-agnostic variant. Both default to a 5 second timeout, accept an explicit `TimeSpan`, and throw on timeout.

### Stop a hanging test from blocking the suite

`AddTimeout` fails a task that never completes, instead of letting the run hang until the framework kills it.

```csharp
var result = await sut.ProcessAsync().AddTimeout();

await sut.StartAsync().AddTimeout(TimeSpan.FromSeconds(30));
```

The default is 5 seconds. When a debugger is attached the timeout is **ignored**, so stepping through a test does not trip a `TimeoutException`.

To await several tasks and collect their results, `AwaitTasks` reads more naturally than `Task.WhenAll`:

```csharp
var results = await new[] { first, second, third }.AwaitTasks();
```

### Freeze the clock

`TimeProvider` resolves to a provider reporting a fixed instant, so time-dependent code is deterministic and the test can assert against the same value.

```csharp
[Theory]
[AutoNSubstituteData]
public void Should_Stamp_Creation_Time(
    [Frozen] TimeProvider timeProvider,
    Order order,
    OrderService sut)
{
    sut.Create(order);

    order.CreatedUtc.Should().Be(timeProvider.GetUtcNow());
}
```

### Control generation for your own type

Any `ICustomization` or `ISpecimenBuilder` marked with `[AutoRegister]` is picked up automatically — no registration call anywhere.

```csharp
[AutoRegister]
public class PositiveAmountCustomization : ICustomization
{
    public void Customize(IFixture fixture)
        => fixture.Customize<Amount>(c => c.FromFactory(() => new Amount(100)));
}
```

### Compare objects containing timestamps

Round-tripped timestamps rarely match to the tick. `CompareDateTimeUsingCloseTo` relaxes the comparison.

```csharp
actual.Should().BeEquivalentTo(
    expected,
    o => o.CompareDateTimeUsingCloseTo());
```

The default precision is 1000 ms. Pass an `int` for a different millisecond precision, or a `TimeSpan` when that reads better:

```csharp
o => o.CompareDateTimeUsingCloseTo(precision: 500)
o => o.CompareDateTimeUsingCloseTo(TimeSpan.FromSeconds(2))
```

### Compare strings ignoring formatting

```csharp
actualJson.Should().HaveSimilarJsonAs(expectedJson);
actualXml.Should().HaveSimilarXmlAs(expectedXml);
actualText.Should().HaveSimilarContentAs(expectedText);
```

## Built-in Specimen Support

`FixtureFactory.Create()` — and therefore every data attribute — handles the following out of the box. Everything else falls back to standard AutoFixture behaviour.

| Type | Behaviour |
|------|-----------|
| `CancellationToken` | A token that has **not** been canceled. |
| `DateOnly` | Derived from a generated `DateTime`. |
| `TimeOnly` | Derived from a generated `DateTime`. |
| `Uri` | A readable absolute URI on the reserved `example.org` domain. |
| `TimeProvider` | A provider reporting a fixed, generated UTC instant (`LocalTimeZone` is UTC). Combine with `[Frozen]` to share the instant. |
| `ImmutableArray<T>`, `ImmutableList<T>`, `ImmutableHashSet<T>`, `ImmutableSortedSet<T>`, `ImmutableDictionary<TKey,TValue>`, `ImmutableSortedDictionary<TKey,TValue>` | Populated by generating a mutable counterpart and converting it. |
| Interfaces / abstract classes | Substituted with NSubstitute. |
| Recursive types | Recursion is omitted rather than throwing — the recursive member is left `null`. |

## Advanced Usage

### Frozen Reuse Scenarios

When you decorate a parameter with `[Frozen]`, its resolved instance is reused for other specimens requiring that exact type. `MemberAutoNSubstituteData` adds **exact-type promotion**: reusing an earlier supplied value for a later `[Frozen]` parameter when that later slot was not part of the member row.

| Scenario | Attribute | Behavior |
|----------|-----------|----------|
| Positional frozen reuse | `ClassAutoNSubstituteData` & `MemberAutoNSubstituteData` | If a value is supplied at the same index as a `[Frozen]` parameter, it is frozen and reused. |
| Exact-type promotion (member data only) | `MemberAutoNSubstituteData` | Later `[Frozen] T` without a supplied value reuses an earlier supplied parameter whose declared type is exactly `T`. |
| No interface/base promotion | Both | Only exact parameter type matches are reused (no interface or base class widening). |

#### Example: Positional Reuse

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

#### Example: Exact-Type Promotion (Member Data)

```csharp
public static IEnumerable<object?[]> ServiceRow()
{
    yield return new object?[] { Substitute.For<IMyService>() }; // supplies parameter 0 only
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

#### Example: Non-Promotion Across Different Interfaces

```csharp
public interface IFoo {}
public interface IBar {}
public class DualImpl : IFoo, IBar {}

public static IEnumerable<object?[]> DualRow()
{
    yield return new object?[] { new DualImpl() }; // supplies IFoo parameter only
}

[Theory]
[MemberAutoNSubstituteData(nameof(DualRow))]
public void Different_Interface_Not_Promoted(
    IFoo foo,
    [Frozen] IBar bar,
    UsesBar consumer)
{
    bar.Should().NotBeSameAs(foo);          // separate instance
    consumer.Bar.Should().BeSameAs(bar);    // consumer wired to frozen IBar
}
```

Design Rationale:

* Class data is usually fully positional—implicit promotion might hide mistakes.
* Member data often supplies only a prefix—promotion reduces duplication while staying explicit.
* Exact-type restriction avoids cross-interface bleed (e.g., dual implementations hijacking unrelated abstractions).

### Auto Registration of Customizations

Any `ICustomization` or `ISpecimenBuilder` decorated with `[AutoRegister]` is added automatically to the fixture created by `FixtureFactory.Create()`.

Example:

```csharp
[AutoRegister]
public class GuidCustomization : ICustomization
{
    public void Customize(IFixture fixture) => fixture.Register(() => Guid.NewGuid());
}
```

The decorated type must expose a parameterless constructor. Discovery scans the assemblies loaded into the current `AppDomain`, so customizations declared in your test project are found without any configuration.

### Helper Extensions

| Helper | Purpose |
|--------|---------|
| `EquivalencyAssertionOptionsExtensions` | Adds convenience config (e.g., date precision) to AwesomeAssertions equivalency. |
| `SubstituteExtensions` | Inspect substitutes, wait for calls, retrieve arguments. |
| `TaskExtensions` | Await with timeouts. |
| `ObjectExtensions` | Access protected members via reflection helpers. |
| `StringExtensions` | Compare text, XML and JSON disregarding formatting. |
| `FixtureFactory` | Central factory returning a consistently customized `IFixture`. |

## Troubleshooting / FAQ

**`NotSupportedException` when a fixture is created**
A type marked `[AutoRegister]` implements neither `ICustomization` nor `ISpecimenBuilder`. Implement one of them, or remove the attribute.

**My `[AutoRegister]` type is never applied**
It must expose a parameterless constructor, and its assembly must be loaded. Types in your test project are loaded automatically; types in a separate assembly that nothing references may not be.

**`[Frozen]` did not reuse the instance from my member data**
Promotion matches the **exact** declared parameter type. A value supplied as `DualImpl` will not be promoted into a `[Frozen] IFoo` parameter. Declare the supplying parameter with the same type as the frozen one.

**Class data does not promote frozen values, but member data does**
This is intentional. Class data is normally fully positional, where implicit promotion would hide mistakes; member data commonly supplies only a leading subset. Use `MemberAutoNSubstituteData` when you want promotion.

**A property on my generated object is unexpectedly `null`**
The type is recursive. `Atc.Test` replaces AutoFixture's throwing recursion behaviour with `OmitOnRecursionBehavior`, so the recursive member is omitted instead of failing the test.

**Build errors about missing xUnit types**
`Atc.Test` does not bring in the `xunit.v3` meta-package by design. Add an explicit `<PackageReference Include="xunit.v3" ... />` to your test project.

**My timestamps differ by a few ticks in `BeEquivalentTo`**
Use `o => o.CompareDateTimeUsingCloseTo()`, or freeze `TimeProvider` so both sides derive from the same instant.

## Why Atc.Test

> You can “just wire everything manually” with plain xUnit and hand‑rolled mocks—so why use this instead?

| Problem Without | What You Gain With `Atc.Test` | Why It Matters Over Time |
|-----------------|-------------------------------|--------------------------|
| Repeating constructor/mocker boilerplate in every test | Parameter-only intent: you list just what the test cares about | Lower cognitive load; faster review – noise removed |
| Fragile refactors (add a ctor param ⇒ touch many files) | Fixture-driven auto‑supply of new dependencies | Constructor churn becomes O(1) instead of O(N tests) |
| Divergent ad‑hoc mock styles (naming, setup order) | Central factory + consistent frozen reuse semantics | Suite stays uniform; easier large-scale edits / audits |
| Accidental duplicate substitutes for logically single collaborator | `[Frozen]` exact-type reuse + early supplied promotion (member data) | Prevents subtle mismatch bugs & expectation gaps |
| Manual re-creation of “shared conventions” (recursion handling, generators) | One-time customization via `[AutoRegister]` | New test inherits standards automatically |
| AI-generated setup drifts over time | Declarative attributes act as a stable policy layer | Reduces maintenance & future prompt dependency |

### When It Delivers the Most Value

* Mid/large test suites (hundreds+ of theory cases).
* Domain services with evolving constructor graphs / dependencies.
* Teams that value refactor safety and consistent test style.
* Situations where only a few parameters per test truly matter.

### When Bare xUnit (+ manual mocks) May Be Enough

* Very small or short‑lived codebases.
* Highly bespoke object graphs where you override almost every generated value anyway.
* Educational contexts emphasizing explicit wiring for learning.

### Summary

`Atc.Test` trades a tiny amount of initial abstraction for compounding savings in refactors, readability, and consistency. AI can quickly generate boilerplate; this library’s value is eliminating the need for that boilerplate in the first place—and giving you a single, policy‑driven locus for customization and reuse.

## Requirements

| Aspect | Value |
|--------|-------|
| Test Framework | xUnit v3 (must be referenced directly) |
| Mocking | NSubstitute (transitively used for interfaces/abstract classes) |
| Assertions | AwesomeAssertions (recommended) |

## Migrating from FluentAssertions

As of v3.0.0, `Atc.Test` depends on [AwesomeAssertions](https://github.com/AwesomeAssertions/AwesomeAssertions)
(the Apache-2.0 community fork of FluentAssertions 7.x) instead of FluentAssertions, whose v8+
releases require a paid commercial license. To upgrade a consuming project:

1. Replace `using FluentAssertions;` with `using AwesomeAssertions;` (and likewise for the
   `.Equivalency`, `.Primitives`, `.Execution`, and `.Extensions` sub-namespaces, plus any `<Using Include="FluentAssertions" />` global usings in your `.csproj`).
2. Remove any direct `<PackageReference Include="FluentAssertions" ... />` from your test projects —
   `Atc.Test` brings in `AwesomeAssertions` transitively.
3. Your `.Should()...` assertions need no changes; the API is identical to FluentAssertions 7.x.

## How to Contribute

[Contribution Guidelines](https://atc-net.github.io/introduction/about-atc#how-to-contribute)  
[Coding Guidelines](https://atc-net.github.io/introduction/about-atc#coding-guidelines)

