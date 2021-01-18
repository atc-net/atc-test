using Atc.Test.Customizations.Generators;
using AutoFixture;
using AutoFixture.Kernel;

namespace Atc.Test.Customizations
{
    /// <summary>
    /// Responsible for registering custom <see cref="ISpecimenBuilder"/>s.
    /// </summary>
    public class CustomGeneratorsCustomization : ICustomization
    {
        /// <inheritdoc/>
        public void Customize(IFixture fixture)
        {
            fixture?.Customizations
                .Add(new CancellationTokenGenerator());
        }
    }
}