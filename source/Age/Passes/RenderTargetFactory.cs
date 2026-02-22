using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

namespace Age.Passes;

internal static class RenderTargetFactory
{
    public static RenderTarget ForEncode(in Size<uint> size)
    {
        var createInfo = new RenderTarget.CreateInfo
        {
            Size             = size,
            ColorAttachments =
            [
                new RenderTarget.CreateInfo.ColorAttachmentInfo
                {
                    FinalLayout = ImageLayout.General,
                    SampleCount = SampleCount.N1,
                    Format      = TextureFormat.R16G16B16A16Unorm,
                    Usage       = TextureUsage.TransferDst | TextureUsage.TransferSrc | TextureUsage.Sampled | TextureUsage.ColorAttachment
                }
            ],
            DepthStencilAttachment = new()
            {
                FinalLayout = ImageLayout.DepthStencilAttachmentOptimal,
                Format      = VulkanRenderer.Singleton.StencilBufferFormat,
                Aspect      = TextureAspect.Stencil,
            },
        };

        return new(createInfo);
    }

    public static RenderTarget ForCompositeEncode(in Size<uint> size)
    {
        var createInfo = new RenderTarget.MultiPassCreateInfo
        {
            Size        = size,
            Attachments =
            [
                new RenderTarget.CreateInfo.ColorAttachmentInfo
                {
                    FinalLayout = ImageLayout.ShaderReadOnlyOptimal,
                    SampleCount = SampleCount.N1,
                    Format      = TextureFormat.R16G16B16A16Unorm,
                    Usage       = TextureUsage.TransferDst | TextureUsage.TransferSrc | TextureUsage.Sampled | TextureUsage.ColorAttachment
                },
                new RenderTarget.CreateInfo.DepthStencilAttachmentInfo()
                {
                    FinalLayout = ImageLayout.DepthStencilAttachmentOptimal,
                    Format      = VulkanRenderer.Singleton.StencilBufferFormat,
                    Aspect      = TextureAspect.Stencil,
                },
            ],
            Passes =
            [
                new()
                {
                    ColorAttachments       = [0],
                    DepthStencilAttachment = 1
                },
                new()
                {
                    ColorAttachments       = [0],
                    DepthStencilAttachment = 1
                }
            ],
        };

        return new(createInfo);
    }
}
