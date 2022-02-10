namespace Atc.Test.Tests.SampleTypes;

public class SampleDependantClass
{
    public SampleDependantClass(ISampleInterface dependency)
    {
        Dependency = dependency;
    }

    public ISampleInterface Dependency { get; }
}