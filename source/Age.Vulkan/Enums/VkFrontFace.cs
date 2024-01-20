namespace Age.Vulkan.Enums;

/// <summary>
/// <para>Interpret polygon front-facing orientation.</para>
/// <para>The first step of polygon rasterization is to determine whether the triangle is back-facing or front-facing. This determination is made based on the sign of the (clipped or unclipped) polygon’s area computed in framebuffer coordinates.</para>
/// <para>The interpretation of the sign of a is determined by the <see cref="VkPipelineRasterizationStateCreateInfo.frontFace"/> property of the currently active pipeline.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkFrontFace
{
    /// <summary>
    /// Specifies that a triangle with positive area is considered front-facing.
    /// </summary>
    VK_FRONT_FACE_COUNTER_CLOCKWISE = 0,

    /// <summary>
    /// Specifies that a triangle with negative area is considered front-facing.
    /// </summary>
    VK_FRONT_FACE_CLOCKWISE = 1,
}
