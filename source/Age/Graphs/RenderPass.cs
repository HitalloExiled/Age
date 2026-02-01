using Age.Core.Extensions;
using Age.Passes;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

namespace Age.Graphs;

public abstract class RenderPass<TOutput> : RenderGraphNode<TOutput>
{
    private readonly List<RenderPass<TOutput>> subPasses = [];

    protected abstract ClearValues ClearValues { get; }

    protected virtual CommandBuffer CommandBuffer => VulkanRenderer.Singleton.CurrentCommandBuffer;
    protected virtual RenderTarget  RenderTarget  => this.Viewport!.RenderTarget;

    public ReadOnlySpan<RenderPass<TOutput>> SubPasses
    {
        get => this.subPasses.AsSpan();
        set
        {
            foreach (var subPass in this.subPasses)
            {
                subPass.RenderGraph = default;
                subPass.Parent      = default;
                subPass.Index       = default;
            }

            for (var i = 0; i < value.Length; i++)
            {
                var subPass = value[i];

                if (subPass.Parent != null)
                {
                    throw new InvalidOperationException($"Cannot attach subpass at index {i}: it is already attached to another RenderPass");
                }

                subPass.Parent = this;
                subPass.Index  = (uint)(i + 1);

                if (this.IsConnected)
                {
                    subPass.RenderGraph = this.RenderGraph;
                }
            }

            this.subPasses.ReplaceRange(0.., value);
        }
    }

    public uint                 Index  { get; private set; }
    public RenderPass<TOutput>? Parent { get; private set; }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            foreach (var subPass in this.subPasses)
            {
                subPass.Dispose();
            }
        }
    }

    protected override void OnConnected()
    {
        foreach (var subPass in this.subPasses)
        {
            subPass.RenderGraph = this.RenderGraph;
        }
    }

    protected sealed override void Execute()
    {
        this.CommandBuffer.SetViewport(this.RenderTarget.Size);
        this.CommandBuffer.BeginRenderPass(this.RenderTarget, this.ClearValues.AsReadOnlySpan());

        var renderContext = this.Viewport!.RenderContext;

        this.Record(renderContext);

        foreach (var subPass in this.subPasses)
        {
            this.CommandBuffer.NextSubPass();

            subPass.Record(renderContext);
        }

        this.CommandBuffer.EndRenderPass();
    }

    protected abstract void Record(RenderContext context);
}

public abstract class RenderPass<TInput, TOutput> : RenderGraphNode<TInput, TOutput>;
