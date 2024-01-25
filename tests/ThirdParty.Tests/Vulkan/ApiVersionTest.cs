namespace ThirdParty.Tests.Vulkan;

public class VersionTest
{
    [Fact]
    public void Instanciate()
    {
        var version = new ThirdParty.Vulkan.Version(6, 5, 3, 7);

        Assert.Equal(6u, version.Variant);
        Assert.Equal(5u, version.Major);
        Assert.Equal(3u, version.Minor);
        Assert.Equal(7u, version.Patch);
    }
}
