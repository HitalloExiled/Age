#define DUMP_IMAGES

using Age.Rendering.Vulkan;
using SkiaSharp;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public class Image : Resource<VkImage>
{
    public Allocation Allocation { get; }
    public VkExtent3D Extent     { get; }

    internal Image(VulkanRenderer renderer, VkImage image, VkExtent3D extent, Allocation allocation) : base(renderer, image)
    {
        this.Extent     = extent;
        this.Allocation = allocation;
    }

    protected override void OnDispose()
    {
        this.Allocation.Dispose();
        this.Value.Dispose();
    }

#if DUMP_IMAGES
    private static void SaveToFile(uint[] data, VkExtent3D extent)
    {
        static SKColor convert(uint value) => new(value);

        var pixels = data.Select(convert).ToArray();

        var bitmap = new SKBitmap((int)extent.Width, (int)extent.Height)
        {
            Pixels = pixels
        };

        var skimage = SKImage.FromBitmap(bitmap);

        try
        {
            using var stream = File.OpenWrite(Path.Join(Directory.GetCurrentDirectory(), $"ObjectId.png"));

            skimage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
        }
        catch
        {

        }
    }
#endif

    public unsafe uint[] GetBuffer()
    {
        var size = this.Extent.Width * this.Extent.Height * sizeof(uint);

        using var buffer = this.Renderer.CreateBuffer(size, VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

        this.Renderer.CopyImageToBuffer(this, buffer, this.Extent);

        buffer.Allocation.Memory.Map(0, size, 0, out var data);

        var pixels = new Span<uint>((uint*)data, (int)(size / 4)).ToArray();

#if DUMP_IMAGES
        SaveToFile(pixels, this.Extent);
#endif

        return pixels;
    }
}
