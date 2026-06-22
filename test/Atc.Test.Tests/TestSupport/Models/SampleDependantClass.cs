namespace Atc.Test.Tests.TestSupport.Models;

public class SampleDependantClass
{
    public SampleDependantClass(ISampleInterface dependency)
    {
        Dependency = dependency;
    }

    public ISampleInterface Dependency { get; }
}