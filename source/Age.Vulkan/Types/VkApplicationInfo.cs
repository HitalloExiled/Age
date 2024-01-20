using Age.Vulkan.Enums;

namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying application information
/// </summary>
public unsafe struct VkApplicationInfo
{
    /// <summary>
    /// Value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A pointer to a null-terminated UTF-8 string containing the name of the application.
    /// </summary>
    public byte* pApplicationName;

    /// <summary>
    /// An unsigned integer variable containing the developer-supplied version number of the application.
    /// </summary>
    public uint applicationVersion;

    /// <summary>
    /// A pointer to a null-terminated UTF-8 string containing the name of the engine (if any) used to create the application.
    /// </summary>
    public byte* pEngineName;

    /// <summary>
    /// An unsigned integer variable containing the developer-supplied version number of the engine used to create the application.
    /// </summary>
    public uint engineVersion;

    /// <summary>
    /// Must be the highest version of Vulkan that the application is designed to use, encoded as described in https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#extendingvulkan-coreversions-versionnumbers. The patch version number specified in apiVersion is ignored when creating an instance object. The variant version of the instance must match that requested in apiVersion.
    /// </summary>
    public uint apiVersion;

    public VkApplicationInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO;
}
