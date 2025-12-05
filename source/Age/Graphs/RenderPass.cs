using Age.Rendering.Resources;

namespace Age.Graphs;

public abstract class RenderPass<TOutput> : RenderGraphNode<TOutput>
{
    protected abstract ReadOnlySpan<ClearValue> ClearValues   { get; }
    protected abstract CommandBuffer            CommandBuffer { get; }
    protected abstract RenderTarget             RenderTarget  { get; }

    protected sealed override void Execute()
    {
        this.CommandBuffer.SetViewport(this.RenderTarget.Size);
        this.CommandBuffer.BeginRenderPass(this.RenderTarget, this.ClearValues);

        this.Record(this.Viewport!.RenderContext);

        this.CommandBuffer.EndRenderPass();
    }

    protected abstract void Record(RenderContext context);
}

public abstract class RenderPass<TInput, TOutput> : RenderGraphNode<TInput, TOutput>;
