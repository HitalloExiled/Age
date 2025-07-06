using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public struct ShaderOptions
{
    #region 4-bytes
    public required VkSampleCountFlags RasterizationSamples;

    public VkFrontFace FrontFace;
    public StencilKind Stencil;
    public uint        Subpass;
    #endregion

    #region 2-bytes
    public bool Watch;

    #endregion
}
