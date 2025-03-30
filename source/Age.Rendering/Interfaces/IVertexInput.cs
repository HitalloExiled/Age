using ThirdParty.Vulkan;

namespace Age.Rendering.Interfaces;

public interface IVertexInput
{
    public abstract static VkVertexInputBindingDescription GetBindings();
    public abstract static VkVertexInputAttributeDescription[] GetAttributes();
}
