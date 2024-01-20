namespace Age.Vulkan.Enums;

/// <summary>
/// Control polygon rasterization mode.
/// These modes affect only the final rasterization of polygons: in particular, a polygon’s vertices are shaded and the polygon is clipped and possibly culled before these modes are applied.
/// <para>If <see cref="VkPhysicalDeviceMaintenance5PropertiesKHR.polygonModePointSize"/> is set to VK_TRUE, the point size of the final rasterization of polygons is taken from PointSize when polygon mode is VK_POLYGON_MODE_POINT.</para>
/// <para>Otherwise, if <see cref="VkPhysicalDeviceMaintenance5PropertiesKHR.polygonModePointSize"/> is set to VK_FALSE, the point size of the final rasterization of polygons is 1.0 when polygon mode is VK_POLYGON_MODE_POINT.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkPolygonMode
{
    /// <summary>
    /// Specifies that polygon vertices are drawn as points.
    /// </summary>
    VK_POLYGON_MODE_FILL = 0,

    /// <summary>
    /// Specifies that polygon edges are drawn as line segments.
    /// </summary>
    VK_POLYGON_MODE_LINE = 1,

    /// <summary>
    /// Specifies that polygons are rendered using the polygon rasterization rules in this section.
    /// </summary>
    VK_POLYGON_MODE_POINT = 2,

    /// <summary>
    /// <para>Specifies that polygons are rendered using polygon rasterization rules, modified to consider a sample within the primitive if the sample location is inside the axis-aligned bounding box of the triangle after projection. Note that the barycentric weights used in attribute interpolation can extend outside the range [0,1] when these primitives are shaded. Special treatment is given to a sample position on the boundary edge of the bounding box. In such a case, if two rectangles lie on either side of a common edge (with identical endpoints) on which a sample position lies, then exactly one of the triangles must produce a fragment that covers that sample during rasterization.</para>
    /// <para>Polygons rendered in <see cref="VK_POLYGON_MODE_FILL_RECTANGLE_NV"/> mode may be clipped by the frustum or by user clip planes. If clipping is applied, the triangle is culled rather than clipped.</para>
    /// <para>Area calculation and facingness are determined for <see cref="VK_POLYGON_MODE_FILL_RECTANGLE_NV"/> mode using the triangle’s vertices.</para>
    /// </summary>
    /// <remarks>Provided by VK_NV_fill_rectangle</remarks>
    VK_POLYGON_MODE_FILL_RECTANGLE_NV = 1000153000,
}
