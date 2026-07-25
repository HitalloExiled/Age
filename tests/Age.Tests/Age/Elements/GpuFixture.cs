using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.Storage;

namespace Age.Tests.Age.Elements;

public class GpuFixture : IDisposable
{
    private static readonly string? lavapipePath;

    private readonly Surface     tempSurface;
    private readonly TextStorage textStorage;

    public VulkanRenderer  Renderer       { get; }
    public TextureStorage  TextureStorage { get; }

    static GpuFixture()
    {
        var candidates = new List<string>();

        if (OperatingSystem.IsLinux())
        {
            candidates.AddRange([
                "/usr/share/vulkan/icd.d/lvp_icd.x86_64.json",
                "/usr/share/vulkan/icd.d/lvp_icd.i686.json",
            ]);
        }

        if (OperatingSystem.IsWindows())
        {
            var sdkRoot = Environment.GetEnvironmentVariable("VULKAN_SDK");

            if (sdkRoot != null)
            {
                candidates.Add(Path.Combine(sdkRoot, "Source", "layers", "generated", "mock_icd", "VkICD_mock_icd.json"));
            }
        }

        lavapipePath = candidates.FirstOrDefault(File.Exists);
    }

    public GpuFixture()
    {
        if (lavapipePath == null)
        {
            Assert.Skip("No software Vulkan ICD found. Install mesa-vulkan-drivers (Linux) or Vulkan SDK (Windows).");
        }

        Environment.SetEnvironmentVariable("VK_ICD_FILENAMES", lavapipePath);

        try
        {
            this.Renderer       = new VulkanRenderer(headless: true);
            this.tempSurface    = this.Renderer.CreateSurface(new(1, 1));
            this.TextureStorage = new TextureStorage(this.Renderer);
            this.textStorage    = new TextStorage(this.Renderer);
        }
        catch
        {
            this.textStorage?.Dispose();
            this.TextureStorage?.Dispose();
            this.tempSurface?.Dispose();
            this.Renderer?.Dispose();
            Assert.Skip("Vulkan initialization failed with software ICD");
        }
    }

    public void Dispose()
    {
        this.textStorage.Dispose();
        this.TextureStorage.Dispose();
        this.tempSurface.Dispose();
        this.Renderer.Dispose();

        GC.SuppressFinalize(this);
    }
}

[CollectionDefinition("GPU")]
public class GpuCollection : ICollectionFixture<GpuFixture> { }
