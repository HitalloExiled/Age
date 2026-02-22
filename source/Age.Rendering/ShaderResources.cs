using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering;

public struct ShaderResources
{
    public required VkDescriptorSetLayout DescriptorSetLayout;
    public required VkPipeline            Pipeline;
    public required VkPipelineLayout      PipelineLayout;

    public VkShaderStageFlags Stages;
}
