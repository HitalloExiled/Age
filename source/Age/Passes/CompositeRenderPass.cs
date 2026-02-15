using Age.Graphs;
using Age.Rendering.Resources;

namespace Age.Passes;

public abstract class CompositeRenderPass : RenderGraphNode<Texture2D>
{
    private readonly RenderPass[] passes = [];

    protected virtual ClearValues ClearValues => ClearValues.Default;

    public abstract CommandBuffer CommandBuffer { get; }
    public abstract RenderTarget RenderTarget   { get; }

    public override Texture2D? Output => this.RenderTarget?.ColorAttachments[0].Texture;

    protected CompositeRenderPass(ReadOnlySpan<RenderPass> passes)
    {
        for (var i = 0; i < passes.Length; i++)
        {
            var pass = passes[i];

            if (pass.Composite != null)
            {
                throw new InvalidOperationException($"Cannot attach subpass at index {i}: it is already attached to another RenderPass");
            }

            pass.Composite = this;
            pass.Index     = (uint)i;

            if (this.IsConnected)
            {
                pass.RenderGraph = this.RenderGraph;
            }
        }

        this.passes = passes.ToArray();
    }

    protected override void AfterExecute()
    {
        base.AfterExecute();

        foreach (var pass in this.passes)
        {
            pass.InternalAfterExecute();
        }
    }

    protected override void BeforeExecute()
    {
        base.BeforeExecute();

        foreach (var pass in this.passes)
        {
            pass.InternalBeforeExecute();
        }
    }

    protected sealed override void Execute()
    {
        if (this.Viewport!.IsDirty || this.passes.Length == 0)
        {
            return;
        }

        this.CommandBuffer.SetViewport(this.RenderTarget.Size);
        this.CommandBuffer.BeginRenderPass(this.RenderTarget, this.ClearValues.AsReadOnlySpan());

        var renderContext = this.Viewport!.RenderContext;

        this.passes[0].InternalRecord(renderContext);

        foreach (var pass in this.passes.AsSpan(1))
        {
            this.CommandBuffer.NextSubPass();

            pass.InternalRecord(renderContext);
        }

        this.CommandBuffer.EndRenderPass();
    }

    protected override void OnConnected()
    {
        foreach (var pass in this.passes)
        {
            pass.RenderGraph = this.RenderGraph;
        }
    }

    protected override void OnDisconnecting()
    {
        foreach (var pass in this.passes)
        {
            pass.RenderGraph = null;
        }
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            foreach (var pass in this.passes)
            {
                pass.Dispose();
            }
        }
    }
}
