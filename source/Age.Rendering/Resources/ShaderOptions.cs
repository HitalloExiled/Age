using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public struct ShaderOptions
{
    #region 4-bytes
    public SampleCount RasterizationSamples = SampleCount.N1;

    public VkFrontFace FrontFace;
    public StencilOp   StencilOp;
    public uint        Subpass;
    #endregion

    #region 2-bytes
    public bool Watch;

    public ShaderOptions() { }

    public override readonly int GetHashCode() =>
        HashCode.Combine(
            this.RasterizationSamples,
            this.FrontFace,
            this.StencilOp,
            this.Subpass,
            this.Watch
        );

    #endregion
}
