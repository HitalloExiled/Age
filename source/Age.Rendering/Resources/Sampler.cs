using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public record Sampler : Disposable
{
    public required VkSampler Value { get; init; }

    protected override void OnDispose() =>
        this.Value.Dispose();
}
