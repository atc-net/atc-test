namespace Atc.Test.Tests.SampleTypes;

// Implements both interfaces so underlying instance satisfies each, allowing us to
// detect overly-broad promotion logic that reuses an earlier IFirstInterface value
// for a later [Frozen] ISecondInterface parameter.
public class DualInterfaceImpl : IFirstInterface, ISecondInterface
{
    public string? Name { get; set; }

    public string? Description { get; set; }
}
