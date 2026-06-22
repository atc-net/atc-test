namespace Atc.Test.Tests.TestSupport.Models;

public class SecondInterfaceDependant
{
    public SecondInterfaceDependant(ISecondInterface dependency)
    {
        Dependency = dependency;
    }

    public ISecondInterface Dependency { get; }
}