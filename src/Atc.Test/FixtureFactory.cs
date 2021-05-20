using Atc.Test.Customizations;
using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace Atc.Test
{
    /// <summary>
    /// Factory for creating <see cref="IFixture"/>s with default customizations.
    /// </summary>
    public static class FixtureFactory
    {
        /// <summary>
        /// Creates an <see cref="IFixture"/> instance with
        /// default customizations.
        /// </summary>
        /// <returns>Configured <see cref="IFixture"/> instance.</returns>
        public static IFixture Create()
        {
            var fixture = new Fixture()
                .Customize(new RecursionCustomization())
                .Customize(new AutoRegisterCustomization())
                .Customize(new AutoNSubstituteCustomization
                {
                    ConfigureMembers = false,
                    GenerateDelegates = true,
                });

            fixture.Customizations.Add(new RegularExpressionGenerator());

            return fixture;
        }
    }
}