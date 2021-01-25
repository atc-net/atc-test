using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;

namespace Atc.Test.Customizations
{
    /// <summary>
    /// Responsible for registering customizations with the <see cref="AutoRegisterAttribute"/> specified.
    /// </summary>
    public class AutoRegisterCustomization : ICustomization
    {
        /// <inheritdoc/>
        public void Customize(IFixture fixture)
        {
            if (fixture is null)
            {
                throw new ArgumentNullException(nameof(fixture));
            }

            var autoRegisterTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(HasAutoRegisterAttribute)
                .ToArray();

            foreach (var type in autoRegisterTypes)
            {
                var customization = Activator.CreateInstance(type);
                switch (customization)
                {
                    case ICustomization c:
                        fixture.Customize(c);
                        break;

                    case ISpecimenBuilder b:
                        fixture.Customizations.Add(b);
                        break;

                    default:
                        throw new NotSupportedException(
                            $"Invalid type {type.Name}. Only ICustomization and " +
                            $"ISpecimenBuilder is supported for the " +
                            $"AutoRegisterAttribute.");
                }
            }
        }

        private static bool HasAutoRegisterAttribute(Type type)
            => type.GetCustomAttributes(
                typeof(AutoRegisterAttribute),
                inherit: false).Length > 0;
    }
}