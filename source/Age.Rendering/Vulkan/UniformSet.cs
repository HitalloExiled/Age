using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class UniformSet : Disposable
{
    public required DescriptorPool    DescriptorPool { get; init; }
    public required VkDescriptorSet[] DescriptorSets { get; init; }
    public required Shader            Shader         { get; init; }

    protected override void OnDispose() =>
        this.DescriptorPool.FreeDescriptorSets(this.DescriptorSets);
}
