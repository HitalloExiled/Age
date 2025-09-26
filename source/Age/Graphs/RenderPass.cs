using Age.Numerics;
using Age.Rendering.Resources;

namespace Age.Graphs;

public abstract class RenderPass<TOutput> : RenderGraphNode<TOutput>
{
    protected abstract Color                    ClearColor    { get; }
    protected abstract ReadOnlySpan<ClearValue> ClearValues   { get; }
    protected abstract CommandBuffer            CommandBuffer { get; }
    protected abstract RenderTarget             RenderTarget  { get; }

    protected unsafe sealed override void Execute(RenderContext context)
    {
        this.CommandBuffer.SetViewport(this.RenderTarget.Size);
        this.CommandBuffer.BeginRenderPass(this.RenderTarget, this.ClearValues);

        this.Record(context);

        this.CommandBuffer.EndRenderPass();
    }

    protected abstract void Record(RenderContext context);
}

public abstract class RenderPass<TInput, TOutput> : RenderGraphNode<TInput, TOutput>
{
    protected sealed override void Execute(RenderContext context) => throw new NotImplementedException();
}
