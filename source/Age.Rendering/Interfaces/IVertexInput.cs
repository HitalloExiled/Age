using ThirdParty.Vulkan;

namespace Age.Rendering.Interfaces;

public interface IVertexInput
{
    static abstract VkVertexInputBindingDescription GetBindings();
    static abstract VkVertexInputAttributeDescription[] GetAttributes();
}
