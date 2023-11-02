namespace Age.Vulkan.Native.Enums;

/// <summary>
/// <para>Comparison operator for depth, stencil, and sampler operations.</para>
/// <para>Comparison operators are used for:</para>
/// <list type="bullet">
/// <item>The <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#textures-depth-compare-operation">Depth Compare Operation</see> operator for a sampler, specified by <see cref="VkSamplerCreateInfo.compareOp"/>.</item>
/// <item>The stencil comparison operator for the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#fragops-stencil">stencil test</see>, specified by <see cref="Vk.CmdSetStencilOp"/>.compareOp or <see cref="VkStencilOpState.compareOp"/>.</item>
/// <item>The <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#fragops-depth-comparison">Depth Comparison</see> operator for the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#fragops-depth">depth test</see>, specified by <see cref="Vk.CmdSetDepthCompareOp"/>.depthCompareOp or <see cref="VkPipelineDepthStencilStateCreateInfo.depthCompareOp"/>.</item>
/// </list>
/// Each such use describes how the reference and test values for that comparison are determined.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkCompareOp
{
    /// <summary>
    /// Specifies that the comparison always evaluates false.
    /// </summary>
    VK_COMPARE_OP_NEVER = 0,

    /// <summary>
    /// Specifies that the comparison evaluates reference < test.
    /// </summary>
    VK_COMPARE_OP_LESS = 1,

    /// <summary>
    /// Specifies that the comparison evaluates reference = test.
    /// </summary>
    VK_COMPARE_OP_EQUAL = 2,

    /// <summary>
    /// Specifies that the comparison evaluates reference ≤ test.
    /// </summary>
    VK_COMPARE_OP_LESS_OR_EQUAL = 3,

    /// <summary>
    /// Specifies that the comparison evaluates reference > test.
    /// </summary>
    VK_COMPARE_OP_GREATER = 4,

    /// <summary>
    /// Specifies that the comparison evaluates reference ≠ test.
    /// </summary>
    VK_COMPARE_OP_NOT_EQUAL = 5,

    /// <summary>
    /// Specifies that the comparison evaluates reference ≥ test.
    /// </summary>
    VK_COMPARE_OP_GREATER_OR_EQUAL = 6,

    /// <summary>
    /// Specifies that the comparison always evaluates true.
    /// </summary>
    VK_COMPARE_OP_ALWAYS = 7,
}
