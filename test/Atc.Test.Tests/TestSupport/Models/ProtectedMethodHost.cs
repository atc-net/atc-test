// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace Atc.Test.Tests.TestSupport.Models;

public class ProtectedMethodHost
{
    public int Offset { get; set; }

    protected int Add(
        int a,
        int b)
        => Offset + a + b;

    protected string Echo(string value)
        => Offset > 0
            ? $"{Offset}:{value}"
            : value;
}