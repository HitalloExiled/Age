namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPrimitiveTopology.html">VkPrimitiveTopology</see>
/// </summary>
public enum PrimitiveTopology
{
    PointList                  = 0,
    LineList                   = 1,
    LineStrip                  = 2,
    TriangleList               = 3,
    TriangleStrip              = 4,
    TriangleFan                = 5,
    LineListWithAdjacency      = 6,
    LineStripWithAdjacency     = 7,
    TriangleListWithAdjacency  = 8,
    TriangleStripWithAdjacency = 9,
    PatchList                  = 10,
}
