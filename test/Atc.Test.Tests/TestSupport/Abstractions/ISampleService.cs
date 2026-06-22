namespace Atc.Test.Tests.TestSupport.Abstractions;

public interface ISampleService
{
    void Notify(string value);

    Task SendAsync(string value);

    ValueTask<int> CalculateAsync(int value);
}
