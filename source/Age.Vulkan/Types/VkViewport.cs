namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying a viewport.
/// </summary>
public struct VkViewport
{
    /// <summary>
    /// <see cref="x"/> and <see cref="y"/> are the viewport’s upper left corner (x,y).
    /// </summary>
    public float x;

    /// <summary>
    /// <see cref="x"/> and <see cref="y"/> are the viewport’s upper left corner (x,y).
    /// </summary>
    public float y;

    /// <summary>
    /// <see cref="width"/> and <see cref="height"/> are the viewport’s width and height, respectively.
    /// </summary>
    public float width;

    /// <summary>
    /// <see cref="width"/> and <see cref="height"/> are the viewport’s width and height, respectively.
    /// </summary>
    public float height;

    /// <summary>
    /// <see cref="minDepth"/> and <see cref="maxDepth"/> are the depth range for the viewport.
    /// </summary>
    public float minDepth;

    /// <summary>
    /// <see cref="minDepth"/> and <see cref="maxDepth"/> are the depth range for the viewport.
    /// </summary>
    public float maxDepth;
}
