using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering;

public ref struct ShaderProperties
{
    public required string       Filepath;
    public required VkRenderPass RenderPass;
    public required Action       OnChanged;

    public required ReadOnlySpan<VkVertexInputAttributeDescription> Attributes;
    public required VkVertexInputBindingDescription                 Bindings;
    public required ReadOnlySpan<string>                            Dependencies;
    public required VkFrontFace                                     FrontFace;
    public required VkPrimitiveTopology                             PrimitiveTopology;
    public required VkSampleCountFlags                              RasterizationSamples;
    public required ReadOnlySpan<byte>                              Source;
    public required StencilOp                                       StencilOp;
    public required uint                                            Subpass;
}
