# Introduction

![NuGet Version](https://img.shields.io/nuget/v/Atc.Test.svg?logo=nuget&style=for-the-badge)

`Atc.Test` is a .NET helper library that streamlines authoring tests with xUnit v3, AutoFixture, NSubstitute, and FluentAssertions. It provides rich data attributes, automatic specimen customization, and ergonomic frozen value reuse to reduce ceremony and improve test readability.

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

## Table of Content

* [Introduction](#introduction)
    * [Why Atc.Test](#why-atctest)
* [Table of Content](#table-of-content)
    * [Features](#features)
    * [Getting Started](#getting-started)
        * [Install Package](#install-package)
        * [Why xUnit Must Be Referenced Directly](#why-xunit-must-be-referenced-directly)
        * [First Test Examples](#first-test-examples)
    * [Advanced Usage](#advanced-usage)
        * [Frozen Reuse Scenarios](#frozen-reuse-scenarios)
        * [Auto Registration of Customizations](#auto-registration-of-customizations)
        * [Helper Extensions](#helper-extensions)
    * [Requirements](#requirements)
    * [How to Contribute](#how-to-contribute)

## Features

* Data attributes integrating AutoFixture + NSubstitute: `AutoNSubstituteData`, `InlineAutoNSubstituteData`, `MemberAutoNSubstituteData`, `ClassAutoNSubstituteData`.
* Automatic interface/abstract substitution via NSubstitute.
* Exact-type frozen promotion for member data (reuse supplied instance across later `[Frozen]` parameters).
* Deterministic fixture configuration with opt‑in auto-registration of custom `ICustomization` / `ISpecimenBuilder` via `[AutoRegister]`.
* Convenience extensions: equivalency options, substitute inspection helpers, task timeout helpers, object protected member access.
* Multi-targeted (netstandard2.1, net8.0, net9.0) for broad compatibility.
* Clear separation of concerns: you own the xUnit runner/version.

## Getting Started

### Install Package

Add `Atc.Test` to your test project along with explicit references to xUnit and the test SDK:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="xunit.v3" Version="3.0.1" />
    <PackageReference Include="Atc.Test" Version="$(LatestOrPinned)" />
  </ItemGroup>
</Project>
```

### Why xUnit Must Be Referenced Directly

`Atc.Test` depends on `xunit.v3.extensibility.core` (the extensibility surface) but intentionally does **not** bring in the `xunit.v3` meta-package:

* Avoid NU1701 warnings from runner assets not targeting `netstandard2.1`.
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

### Helper Extensions

| Helper | Purpose |
|--------|---------|
| `EquivalencyAssertionOptionsExtensions` | Adds convenience config (e.g., date precision) to FluentAssertions equivalency. |
| `SubstituteExtensions` | Inspect substitutes, wait for calls, retrieve arguments. |
| `TaskExtensions` | Await with timeouts. |
| `ObjectExtensions` | Access protected members via reflection helpers. |
| `FixtureFactory` | Central factory returning a consistently customized `IFixture`. |

## Requirements

| Aspect | Value |
|--------|-------|
| Target Frameworks | netstandard2.1, net8.0, net9.0 |
| Test Framework | xUnit v3 (must be referenced directly) |
| Mocking | NSubstitute (transitively used for interfaces/abstract classes) |
| Assertions | FluentAssertions (recommended) |

## How to Contribute

[Contribution Guidelines](https://atc-net.github.io/introduction/about-atc#how-to-contribute)  
[Coding Guidelines](https://atc-net.github.io/introduction/about-atc#coding-guidelines)

