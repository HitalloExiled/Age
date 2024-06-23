using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing;
using Age.Rendering.RenderPasses;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Scene;

public class Viewport : Element, IDisposable
{
    private bool disposed;

    private RenderTarget renderTarget;

    public Viewport(Size<uint> size) =>
        this.renderTarget = this.CreateRenderTarget(size);

    public override string NodeName => nameof(Viewport);

    public RenderTarget RenderTarget => this.renderTarget;

    public Size<uint> ViewSize
    {
        get => new(this.renderTarget.Size.Width, this.renderTarget.Size.Height);
        set
        {
            if (!value.Equals(this.renderTarget.Size))
            {
                this.renderTarget = this.CreateRenderTarget(value);
            }
        }
    }

    ~Viewport() =>
        this.Dispose(false);

    private RenderTarget CreateRenderTarget(Size<uint> size)
    {
        this.Style.MinSize = size;

        if (RenderGraph.Active == null)
        {
            throw new InvalidOperationException("There no active RenderGraph");
        }

        var renderPass = RenderGraph.Active.GetRenderPass<SceneRenderGraphPass>()
            ?? throw new InvalidOperationException($"Can't find any {nameof(SceneRenderGraphPass)} on {RenderGraph.Active.Name} RenderGraph");

        var renderTargetCreateInfo = new RenderTargetCreateInfo
        {
            Extent     = new() { Width = size.Width, Height = size.Height },
            Format     = VkFormat.B8G8R8A8Unorm,
            RenderPass = renderPass,
        };

        var renderTarget = VulkanRenderer.Singleton.CreateRenderTarget(renderTargetCreateInfo);

        if (this.SingleCommand is not RectCommand command)
        {
            this.SingleCommand = command = new();
        }

        command.Rect           = new Rect<float>(renderTarget.Texture.Image.Extent.Width, renderTarget.Texture.Image.Extent.Height, 0, 0);
        command.SampledTexture = new(renderTarget.Texture, Container.Singleton.TextureStorage.DefaultSampler, UVRect.Normalized);

        return renderTarget;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            { }

            this.RenderTarget.Dispose();

            this.disposed = true;
        }
    }

    protected override void OnDestroy() =>
        this.Dispose();

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
