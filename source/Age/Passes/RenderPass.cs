using Age.Graphs;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

namespace Age.Passes;

public abstract class RenderPass : RenderGraphNode<Texture2D>
{
    protected abstract ClearValues ClearValues { get; }

    protected virtual CommandBuffer CommandBuffer => VulkanRenderer.Singleton.CurrentCommandBuffer;
    protected virtual RenderTarget  RenderTarget  => this.Viewport!.RenderTarget;

    public uint                 Index     { get; internal set; }
    public CompositeRenderPass? Composite { get; internal set; }

    protected sealed override void Execute()
    {
        if (this.Viewport!.IsDirty)
        {
            return;
        }

        this.CommandBuffer.SetViewport(this.RenderTarget.Size);
        this.CommandBuffer.BeginRenderPass(this.RenderTarget, this.ClearValues.AsReadOnlySpan());

        var renderContext = this.Viewport!.RenderContext;

        this.Record(renderContext);

        this.CommandBuffer.EndRenderPass(this.RenderTarget);
    }

    protected abstract void Record(RenderContext context);

    internal void InternalAfterExecute() =>
        this.AfterExecute();

    internal void InternalBeforeExecute() =>
        this.BeforeExecute();

    internal void InternalRecord(RenderContext context) =>
        this.Record(context);
}
