namespace Guardian.Tests;

public sealed class EnvironmentTests
{
    [Fact]
    public void RuntimeIsDotNet8OrNewer()
    {
        Assert.True(Environment.Version.Major >= 8);
    }
}
