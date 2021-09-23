namespace Atc.Test.Tests
{
    public class SampleDependantClass
    {
        public SampleDependantClass(ISampleInterface dependency)
        {
            Dependency = dependency;
        }

        public ISampleInterface Dependency { get; }
    }
}
