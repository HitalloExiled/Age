using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public partial class RenderPass : Disposable
{
    public required VkRenderPass  Value     { get; init; }
    public required int           SubPasses { get; init; }

    protected override void OnDispose() =>
        this.Value.Dispose();
}
