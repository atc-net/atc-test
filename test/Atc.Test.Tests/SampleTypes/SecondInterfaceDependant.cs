namespace Atc.Test.Tests.SampleTypes;

public class SecondInterfaceDependant
{
    public SecondInterfaceDependant(ISecondInterface dependency)
    {
        Dependency = dependency;
    }

    public ISecondInterface Dependency { get; }
}
