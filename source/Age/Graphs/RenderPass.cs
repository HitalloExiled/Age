using System.Drawing;
using Age.Rendering.Extensions;
using Age.Rendering.Resources;
using ThirdParty.Vulkan;

namespace Age.Graphs;

public abstract class RenderPass<TOutput> : RenderGraphNode<TOutput>
{
    protected abstract Color         ClearColor    { get; }
    protected abstract RenderTarget  RenderTarget  { get; }
    protected abstract CommandBuffer CommandBuffer { get; }

    protected unsafe sealed override void Execute(RenderContext context)
    {
        Span<VkClearValue> clearValues = stackalloc VkClearValue[2];

        clearValues[0].Color.Float32[0] = this.ClearColor.R;
        clearValues[0].Color.Float32[1] = this.ClearColor.G;
        clearValues[0].Color.Float32[2] = this.ClearColor.B;
        clearValues[0].Color.Float32[3] = this.ClearColor.A;
        clearValues[1].DepthStencil.Depth = 1;

        var extent = this.RenderTarget.Size.ToExtent2D();

        this.CommandBuffer.SetViewport(extent);
        this.CommandBuffer.BeginRenderPass(extent, this.RenderTarget.RenderPass, this.RenderTarget.Framebuffer, clearValues);

        this.Record(context);

        this.CommandBuffer.EndRenderPass();
    }

    protected abstract void Record(RenderContext context);
}

public abstract class RenderPass<TInput, TOutput> : RenderGraphNode<TInput, TOutput>
{
    protected sealed override void Execute(RenderContext context) => throw new NotImplementedException();
}
