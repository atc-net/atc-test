[![NuGet Version](https://img.shields.io/nuget/v/atc-test.svg?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/atc.test)

# ATC Test

Common tools for writing tests using XUnit, AutoFixture, NSubstitute and FluentAssertions.

## Test Attributes

| Name | Description |
|-|-|
| `AutoNSubstituteData` | Provides auto-generated data specimens generated by AutoFixture and NSubstitute as an extension to XUnit's [Theory] attribute.|
| `InlineAutoNSubstituteData` | Provides a data source for a data theory, with the data coming from inline values combined with auto-generated data specimens generated by AutoFixture and NSubstitute.|
| `MemberAutoNSubstituteData` | Provides a data source for a data theory, with the data coming from one of the following sources and combined with auto-generated data specimens generated by AutoFixture and NSubstitute.|

Note: NSubstitute is used when the type being created is abstract, or when the `[Substitute]` is applied.

## Test Helpers

| Name | Description |
|-|-|
| `EquivalencyAssertionOptionsExtensions` | Extensions for FluentAssertions to compare dates with a precision when using `.BeEquivalentTo()`.|
| `FixtureFactory` | Static factory for creating AutoFixture `Fixture` instances.|
| `ObjectExtensions` | Extensions calling protected members on an object.|
| `SubstituteExtensions` | Extensions for NSubstitutes to wait for calls and get arguments of a received call.|
| `TaskExtensions` | Extensions for Tasks to add timeouts when awaiting. |

## Extensibility

The default `Fixture` returned by the `FixtrueFactory.Create()` method is used for all the `Attributes` mentioned above.

To add customizations to this, you can add the `AutoRegisterAttribute` to any custom `ICustomization` or `ISpecimenBuilder` to have it automatically added to the Fixture.

See [`CancellationTokenGenerator`](src/Atc.Test/Customizations/Generators/CancellationTokenGenerator.cs) for an example on how to do this.

## How to contribute

[Contribution Guidelines](https://atc-net.github.io/introduction/about-atc#how-to-contribute)

[Coding Guidelines](https://atc-net.github.io/introduction/about-atc#coding-guidelines)