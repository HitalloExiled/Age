using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.Services;
using Age.Storage;

namespace Age.Tests.Age.Fixtures;

public class GpuFixture : IDisposable
{
    private static readonly string? lavapipePath;

    private readonly VulkanRenderer   renderer;
    private readonly RenderingService renderingService;
    private readonly ShaderStorage    shaderStorage;
    private readonly Surface          surface;
    private readonly TextStorage      textStorage;
    private readonly TextureStorage   textureStorage;

    static GpuFixture()
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
            this.renderer         = new(headless: true);
            this.textureStorage   = new(this.renderer);
            this.textStorage      = new(this.renderer);
            this.shaderStorage    = new(this.renderer);
            this.renderingService = new(this.renderer);

            this.surface = this.renderer.CreateSurface(new(1, 1));
        }
        catch
        {
            this.surface?.Dispose();

            this.textStorage?.Dispose();
            this.textureStorage?.Dispose();
            this.renderer?.Dispose();
            this.renderingService?.Dispose();
            this.shaderStorage?.Dispose();

            Assert.Skip("Vulkan initialization failed with software ICD");
        }
    }

    public void Dispose()
    {
        this.surface.Dispose();

        this.renderer.Dispose();
        this.renderingService.Dispose();
        this.shaderStorage.Dispose();
        this.textStorage.Dispose();
        this.textureStorage.Dispose();

        GC.SuppressFinalize(this);
    }
}

[CollectionDefinition("GPU")]
public class GpuCollection : ICollectionFixture<GpuFixture>;
