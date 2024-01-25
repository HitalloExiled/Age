using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// Structure specifying vertex input binding description.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe record VertexInputBindingDescription : NativeReference<VkVertexInputBindingDescription>
{
    /// <summary>
    /// The binding number that this structure describes.
    /// </summary>
    public uint Binding
    {
        get => this.PNative->binding;
        init => this.PNative->binding = value;
    }

    /// <summary>
    /// The byte stride between consecutive elements within the buffer.
    /// </summary>
    public uint Stride
    {
        get => this.PNative->stride;
        init => this.PNative->stride = value;
    }

    /// <summary>
    /// A VkVertexInputRate value specifying whether vertex attribute addressing is a function of the vertex index or of the instance index.
    /// </summary>
    public VertexInputRate InputRate
    {
        get => this.PNative->inputRate;
        init => this.PNative->inputRate = value;
    }
}
