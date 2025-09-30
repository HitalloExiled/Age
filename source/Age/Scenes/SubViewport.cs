using Age.Commands;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

namespace Age.Scenes;

public class SubViewport : Viewport
{
    public override event Action? Resized;

    private RenderTarget renderTarget;

    public override Size<uint> Size
    {
        get => this.RenderTarget.Size;
        set
        {
            if (this.RenderTarget.Size != value)
            {
                this.renderTarget.Dispose();
                this.renderTarget = CreateRenderTarget(value);

                this.UpdateCommand();

                this.Resized?.Invoke();
            }
        }
    }

    public override string       NodeName     => nameof(SubViewport);
    public override RenderTarget RenderTarget => this.renderTarget;
    public override Texture2D    Texture      => this.RenderTarget.ColorAttachments[0].Texture;

    public SubViewport(in Size<uint> size)
    {
        this.renderTarget = CreateRenderTarget(size);
        this.UpdateCommand();
    }

    private static RenderTarget CreateRenderTarget(Size<uint> size)
    {
        var createInfo = new RenderTarget.CreateInfo
        {
            Size             = size,
            ColorAttachments =
            [
                new()
                {
                    FinalLayout = ThirdParty.Vulkan.Enums.VkImageLayout.ShaderReadOnlyOptimal,
                    Format      = TextureFormat.B8G8R8A8Unorm,
                    SampleCount = SampleCount.N1,
                    Usage       = TextureUsage.ColorAttachment | TextureUsage.Sampled,
                }
            ],
            DepthStencilAttachment = new()
            {
                FinalLayout = ThirdParty.Vulkan.Enums.VkImageLayout.DepthStencilAttachmentOptimal,
                Format      = (TextureFormat)VulkanRenderer.Singleton.DepthBufferFormat,
                Aspect      = TextureAspect.Depth,
            }
        };

        return new(createInfo);
    }

    private void UpdateCommand()
    {
        if (this.SingleCommand is not RectCommand command)
        {
            this.SingleCommand = command = CommandPool.RectCommand.Get();

            command.CommandFilter = CommandFilter.Color;
        }

        command.Size       = this.RenderTarget.Size;
        command.TextureMap = new(this.RenderTarget.ColorAttachments[0].Texture, UVRect.Normalized);
    }

    private protected override void OnDisposedInternal()
    {
        base.OnDisposedInternal();

        this.RenderTarget.Dispose();
    }
}
