using ThirdParty.Vulkan;

namespace ThirdParty.Tests.Vulkan;

public class VulkanFixture : IDisposable
{
    private static readonly string? icdPath;

    public VkInstance Instance { get; }

    static VulkanFixture()
    {
        var candidates = new List<string>();
        var sdkRoot    = Environment.GetEnvironmentVariable("VULKAN_SDK");

        if (sdkRoot != null)
        {
            if (OperatingSystem.IsLinux())
            {
                candidates.Add(Path.Combine(sdkRoot, "VkICD_mock_icd.json"));
            }

            if (OperatingSystem.IsWindows())
            {
                candidates.Add(Path.Combine(sdkRoot, "Source", "layers", "generated", "mock_icd", "VkICD_mock_icd.json"));
            }
        }

        if (OperatingSystem.IsLinux())
        {
            candidates.AddRange([
                "/usr/share/vulkan/icd.d/lvp_icd.x86_64.json",
                "/usr/share/vulkan/icd.d/lvp_icd.i686.json",
            ]);
        }

        icdPath = candidates.FirstOrDefault(File.Exists);
    }

    public VulkanFixture()
    {
        if (icdPath == null)
        {
            Assert.Skip("No software Vulkan ICD found. Install mesa-vulkan-drivers (Linux) or Vulkan SDK (Windows).");
        }

        Environment.SetEnvironmentVariable("VK_ICD_FILENAMES", icdPath);

        try
        {
            unsafe
            {
                var applicationInfo = new VkApplicationInfo
                {
                    ApiVersion = VkVersion.V1_0,
                };

                var instanceCreateInfo = new VkInstanceCreateInfo
                {
                    PApplicationInfo = &applicationInfo,
                };

                this.Instance = new VkInstance(instanceCreateInfo);
            }
        }
        catch
        {
            this.Instance?.Dispose();
            Assert.Skip("Vulkan initialization failed with software ICD");
        }
    }

    public void Dispose()
    {
        this.Instance.Dispose();
        GC.SuppressFinalize(this);
    }
}

[CollectionDefinition("Vulkan")]
public class VulkanCollection : ICollectionFixture<VulkanFixture>;
