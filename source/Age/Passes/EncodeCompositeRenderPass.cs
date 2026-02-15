using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Age.Rendering.Resources;
using ThirdParty.Vulkan.Enums;

namespace Age.Passes;

public class EncodeCompositeRenderPass(ReadOnlySpan<RenderPass> passes) : CompositeRenderPass(passes)
{
    [AllowNull]
    private RenderTarget renderTarget;

    [AllowNull]
    private CommandBuffer commandBuffer;

    public override string Name => nameof(EncodeCompositeRenderPass);

    public override CommandBuffer CommandBuffer => this.commandBuffer;
    public override RenderTarget  RenderTarget  => this.renderTarget;

    private void RecreateRenderTarget()
    {
        Debug.Assert(this.Viewport != null);

        this.renderTarget?.Dispose();
        this.renderTarget = RenderTargetFactory.ForCompositeEncode(this.Viewport.Size);
    }

    protected override void OnConnected()
    {
        Debug.Assert(this.Viewport != null);

        this.Viewport.Resized += this.RecreateRenderTarget;

        this.renderTarget  = RenderTargetFactory.ForCompositeEncode(this.Viewport.Size);
        this.commandBuffer = new(VkCommandBufferLevel.Primary);

        base.OnConnected();
    }

    protected override void OnDisconnecting()
    {
        this.Viewport?.Resized -= this.RecreateRenderTarget;

        this.renderTarget?.Dispose();
        this.commandBuffer?.Dispose();

        base.OnDisconnecting();
    }

    protected override void OnDisposed(bool disposing)
    {
        base.OnDisposed(disposing);
        this.OnDisconnecting();
    }
}
