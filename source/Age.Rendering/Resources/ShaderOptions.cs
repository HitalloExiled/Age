using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public struct ShaderOptions
{
    #region 4-bytes
    public SampleCount RasterizationSamples = SampleCount.N1;

    public StencilOp   StencilOp;
    public uint        Subpass;
    #endregion

    public ShaderOptions() { }

    public override readonly int GetHashCode() =>
        HashCode.Combine(
            this.RasterizationSamples,
            this.StencilOp,
            this.Subpass
        );

}
