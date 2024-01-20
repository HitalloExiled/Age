using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying a pipeline color blend attachment state.
/// </summary>
public struct VkPipelineColorBlendAttachmentState
{
    /// <summary>
    /// Controls whether blending is enabled for the corresponding color attachment. If blending is not enabled, the source fragment’s color for that attachment is passed through unmodified.
    /// </summary>
    public VkBool32 blendEnable;

    /// <summary>
    /// Selects which blend factor is used to determine the source factors (Sr,Sg,Sb).
    /// </summary>
    public VkBlendFactor srcColorBlendFactor;

    /// <summary>
    /// Selects which blend factor is used to determine the destination factors (Dr,Dg,Db).
    /// </summary>
    public VkBlendFactor dstColorBlendFactor;

    /// <summary>
    /// Selects which blend operation is used to calculate the RGB values to write to the color attachment.
    /// </summary>
    public VkBlendOp colorBlendOp;

    /// <summary>
    /// Selects which blend factor is used to determine the source factor Sa.
    /// </summary>
    public VkBlendFactor srcAlphaBlendFactor;

    /// <summary>
    /// Selects which blend factor is used to determine the destination factor Da.
    /// </summary>
    public VkBlendFactor dstAlphaBlendFactor;

    /// <summary>
    /// Selects which blend operation is used to calculate the alpha values to write to the color attachment.
    /// </summary>
    public VkBlendOp alphaBlendOp;

    /// <summary>
    /// A bitmask of <see cref="VkColorComponentFlagBits"/> specifying which of the R, G, B, and/or A components are enabled for writing, as described for the Color Write Mask.
    /// </summary>
    public VkColorComponentFlags colorWriteMask;
}
