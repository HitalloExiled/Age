using Age.Rendering.Resources;

namespace Age.Graphs;

public abstract class RenderPass<TOutput> : RenderGraphNode<TOutput>
where TOutput : new()
{
    protected abstract RenderTarget RenderTarget { get; }

    protected sealed override void Execute(RenderContext context) => throw new NotImplementedException();
}

public abstract class RenderPass<TInput, TOutput> : RenderGraphNode<TInput, TOutput>
where TInput  : new()
where TOutput : new()
{
    protected sealed override void Execute(RenderContext context) => throw new NotImplementedException();
}
