using System.Diagnostics.CodeAnalysis;
using Age.Graphs;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

namespace Age.Passes;

public abstract partial class Scene3DPass : RenderPass<Texture2D>
{
    protected FrameResource[] FrameResources { get; } = new FrameResource[VulkanContext.MAX_FRAMES_IN_FLIGHT];

    [AllowNull]
    public override Texture2D Output => this.RenderGraph?.Viewport.Texture ?? Texture2D.Default;

    protected Scene3DPass() =>
        this.FrameResources.AsSpan().Fill(new());
}
