using Age.Core.Extensions;
using Age.Core;
using Age.Numerics;
using SkiaSharp;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;

using ImageResource = Age.Rendering.Resources.Image;

namespace Age.Resources;

public class Image : Disposable
{
    private readonly ImageResource resource;

    public Size<uint> Size { get; }

    public Image(Span<byte> buffer, Size<uint> size)
    {
        this.Size = size;

        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers = 1,
            Extent      = new()
            {
                Width  = size.Width,
                Height = size.Height,
                Depth  = 1,
            },
            Format        = VkFormat.B8G8R8A8Unorm,
            ImageType     = VkImageType.N2D,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = VkSampleCountFlags.N1,
            Tiling        = VkImageTiling.Optimal,
            Usage         = VkImageUsageFlags.TransferSrc | VkImageUsageFlags.TransferDst | VkImageUsageFlags.Sampled,
        };

        this.resource = new ImageResource(imageCreateInfo);

        this.resource.Update(buffer);
    }

    public static Image Load(string path)
    {
        using var stream = File.OpenRead(path);

        var bitmap = SKBitmap.Decode(stream);

        var buffer = bitmap.Pixels.AsSpan().Cast<SKColor, byte>();

        return new(buffer.ToArray(), new((uint)bitmap.Width, (uint)bitmap.Height));
    }

    protected override void Disposed(bool disposing)
    {
        if (disposing)
        {
            this.resource.Dispose();
        }
    }

    public static implicit operator ImageResource(Image value) => value.resource;

}
