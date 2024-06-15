using Age.Numerics;
using Age.Rendering.RenderPasses;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Scene;

public class Viewport(Size<uint> size) : Node2D, IDisposable
{
    private bool disposed;

    private RenderTarget renderTarget = CreateRenderTarget(size);

    public override string NodeName => nameof(Viewport);

    public RenderTarget RenderTarget => this.renderTarget;

    public Size<uint> Size
    {
        get => new(this.renderTarget.Size.Width, this.renderTarget.Size.Height);
        set
        {
            if (!value.Equals(this.renderTarget.Size))
            {
                this.renderTarget = CreateRenderTarget(value);
            }
        }
    }

    ~Viewport() =>
        this.Dispose(disposing: false);

    private static RenderTarget CreateRenderTarget(Size<uint> size)
    {
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

        return VulkanRenderer.Singleton.CreateRenderTarget(renderTargetCreateInfo);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            { }

            this.disposed = true;
        }
    }



    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
