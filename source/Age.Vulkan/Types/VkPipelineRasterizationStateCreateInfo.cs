using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure specifying parameters of a newly created pipeline rasterization state.</para>
/// <para>The application can also add a <see cref="VkPipelineRasterizationStateRasterizationOrderAMD"/> structure to the pNext chain of a <see cref="VkPipelineRasterizationStateCreateInfo"/> structure. This structure enables selecting the rasterization order to use when rendering with the corresponding graphics pipeline as described in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#primsrast-order">Rasterization Order</see>.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkPipelineRasterizationStateCreateInfo
{
    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// Reserved for future use.
    /// </summary>
    public VkPipelineRasterizationStateCreateFlags flags;

    /// <summary>
    /// Controls whether to clamp the fragment’s depth values as described in Depth Test. If the pipeline is not created with <see cref="VkPipelineRasterizationDepthClipStateCreateInfoEXT"/> present then enabling depth clamp will also disable clipping primitives to the z planes of the frustrum as described in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#vertexpostproc-clipping">Primitive Clipping</see>. Otherwise depth clipping is controlled by the state set in <see cref="VkPipelineRasterizationDepthClipStateCreateInfoEXT"/>.
    /// </summary>
    public VkBool32 depthClampEnable;

    /// <summary>
    /// Controls whether primitives are discarded immediately before the rasterization stage.
    /// </summary>
    public VkBool32 rasterizerDiscardEnable;

    /// <summary>
    /// The triangle rendering mode. See <see cref="VkPolygonMode"/>.
    /// </summary>
    public VkPolygonMode polygonMode;

    /// <summary>
    /// The triangle facing direction used for primitive culling. See <see cref="VkCullModeFlagBits"/>.
    /// </summary>
    public VkCullModeFlags cullMode;

    /// <summary>
    /// A VkFrontFace value specifying the front-facing triangle orientation to be used for culling.
    /// </summary>
    public VkFrontFace frontFace;

    /// <summary>
    /// Controls whether to bias fragment depth values.
    /// </summary>
    public VkBool32 depthBiasEnable;

    /// <summary>
    /// A scalar factor controlling the constant depth value added to each fragment.
    /// </summary>
    public float depthBiasConstantFactor;

    /// <summary>
    /// The maximum (or minimum) depth bias of a fragment.
    /// </summary>
    public float depthBiasClamp;

    /// <summary>
    /// A scalar factor applied to a fragment’s slope in depth bias calculations.
    /// </summary>
    public float depthBiasSlopeFactor;

    /// <summary>
    /// The width of rasterized line segments.
    /// </summary>
    public float lineWidth;

    public VkPipelineRasterizationStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_CREATE_INFO;
}
