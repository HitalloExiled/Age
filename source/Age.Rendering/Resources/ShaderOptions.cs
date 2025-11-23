using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public struct ShaderOptions
{
    #region 4-bytes
    public required SampleCount RasterizationSamples;

    public VkFrontFace FrontFace;
    public StencilOp   StencilOp;
    public uint        Subpass;
    #endregion

    #region 2-bytes
    public bool Watch;

    #endregion
}
