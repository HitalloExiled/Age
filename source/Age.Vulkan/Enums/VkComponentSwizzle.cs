namespace Age.Vulkan.Enums;

/// <summary>
/// <para>Specify how a component is swizzled.</para>
/// <para>Component Mappings Equivalent To <see cref="VK_COMPONENT_SWIZZLE_IDENTITY"/></para>
/// <list type="table">
/// <listheader>
/// <term>Component</term>
/// <description>Identity Mapping</description>
/// </listheader>
/// <item>
/// <description>components.r</description>
/// <description>VK_COMPONENT_SWIZZLE_R</description>
/// </item>
/// <item>
/// <term>components.g</term>
/// <description>VK_COMPONENT_SWIZZLE_G</description>
/// </item>
/// <item>
/// <term>components.b</term>
/// <description>VK_COMPONENT_SWIZZLE_B</description>
/// </item>
/// <item>
/// <term>components.a</term>
/// <description>VK_COMPONENT_SWIZZLE_A</description>
/// </item>
/// </list>
/// </summary>
public enum VkComponentSwizzle
{
    /// <summary>
    /// specifies that the component is set to the identity swizzle.
    /// </summary>
    VK_COMPONENT_SWIZZLE_IDENTITY = 0,

    /// <summary>
    /// specifies that the component is set to zero.
    /// </summary>
    VK_COMPONENT_SWIZZLE_ZERO = 1,

    /// <summary>
    /// specifies that the component is set to either 1 or 1.0, depending on whether the type of the image view format is integer or floating-point respectively, as determined by the Format Definition section for each VkFormat.
    /// </summary>
    VK_COMPONENT_SWIZZLE_ONE = 2,

    /// <summary>
    /// specifies that the component is set to the value of the R component of the image.
    /// </summary>
    VK_COMPONENT_SWIZZLE_R = 3,

    /// <summary>
    /// specifies that the component is set to the value of the G component of the image.
    /// </summary>
    VK_COMPONENT_SWIZZLE_G = 4,

    /// <summary>
    /// specifies that the component is set to the value of the B component of the image.
    /// </summary>
    VK_COMPONENT_SWIZZLE_B = 5,

    /// <summary>
    /// specifies that the component is set to the value of the A component of the image.
    /// </summary>
    VK_COMPONENT_SWIZZLE_A = 6,
}
