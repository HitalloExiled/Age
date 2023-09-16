namespace Age.Vulkan.Native.Enums;

/// <summary>
/// Supported primitive topologies.
/// Each primitive topology, and its construction from a list of vertices, is described in detail below with a supporting diagram, according to the following key:
/// <list type="table">
/// <item>
/// <term>Vertex</term>
/// <description>A point in 3-dimensional space. Positions chosen within the diagrams are arbitrary and for illustration only.</description>
/// </item>
/// <item>
/// <term>Vertex Number</term>
/// <description>Sequence position of a vertex within the provided vertex data.</description>
/// </item>
/// <item>
/// <term>Provoking Vertex</term>
/// <description>Provoking vertex within the main primitive. The tail is angled towards the relevant primitive. Used in flat shading.</description>
/// </item>
/// <item>
/// <term>Primitive Edge</term>
/// <description>An edge connecting the points of a main primitive.</description>
/// </item>
/// <item>
/// <term>Adjacency Edge</term>
/// <description>Points connected by these lines do not contribute to a main primitive, and are only accessible in a geometry shader.</description>
/// </item>
/// <item>
/// <term>Winding Order</term>
/// <description>The relative order in which vertices are defined within a primitive, used in the facing determination. This ordering has no specific start or end point.</description>
/// </item>
/// </list>
/// <para>The diagrams are supported with mathematical definitions where the vertices (v) and primitives (p) are numbered starting from 0; v0 is the first vertex in the provided data and p0 is the first primitive in the set of primitives defined by the vertices and topology.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkPrimitiveTopology
{
    /// <summary>
    /// Specifies a series of separate point primitives.
    /// </summary>
    VK_PRIMITIVE_TOPOLOGY_POINT_LIST = 0,

    /// <summary>
    /// Specifies a series of separate line primitives.
    /// </summary>
    VK_PRIMITIVE_TOPOLOGY_LINE_LIST = 1,

    /// <summary>
    /// Specifies a series of connected line primitives with consecutive lines sharing a vertex.
    /// </summary>
    VK_PRIMITIVE_TOPOLOGY_LINE_STRIP = 2,

    /// <summary>
    /// Specifies a series of separate triangle primitives.
    /// </summary>
    VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST = 3,

    /// <summary>
    /// Specifies a series of connected triangle primitives with consecutive triangles sharing an edge.
    /// </summary>
    VK_PRIMITIVE_TOPOLOGY_TRIANGLE_STRIP = 4,

    /// <summary>
    /// Specifies a series of connected triangle primitives with all triangles sharing a common vertex. If the VK_KHR_portability_subset extension is enabled, and <see cref="VkPhysicalDevicePortabilitySubsetFeaturesKHR.triangleFans"/> is VK_FALSE, then triangle fans are not supported by the implementation, and <see cref="VK_PRIMITIVE_TOPOLOGY_TRIANGLE_FAN"/> must not be used.
    /// </summary>
    VK_PRIMITIVE_TOPOLOGY_TRIANGLE_FAN = 5,

    /// <summary>
    /// Specifies a series of separate line primitives with adjacency.
    /// </summary>
    VK_PRIMITIVE_TOPOLOGY_LINE_LIST_WITH_ADJACENCY = 6,

    /// <summary>
    /// Specifies a series of connected line primitives with adjacency, with consecutive primitives sharing three vertices.
    /// </summary>
    VK_PRIMITIVE_TOPOLOGY_LINE_STRIP_WITH_ADJACENCY = 7,

    /// <summary>
    /// Specifies a series of separate triangle primitives with adjacency.
    /// </summary>
    VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST_WITH_ADJACENCY = 8,

    /// <summary>
    /// Specifies connected triangle primitives with adjacency, with consecutive triangles sharing an edge.
    /// </summary>
    VK_PRIMITIVE_TOPOLOGY_TRIANGLE_STRIP_WITH_ADJACENCY = 9,

    /// <summary>
    /// Specifies separate patch primitives.
    /// </summary>
    VK_PRIMITIVE_TOPOLOGY_PATCH_LIST = 10,
}
