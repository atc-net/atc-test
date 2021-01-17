using Atc.TestTools.Customizations.Generators;
using AutoFixture;
using AutoFixture.Kernel;

namespace Atc.TestTools.Customizations
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