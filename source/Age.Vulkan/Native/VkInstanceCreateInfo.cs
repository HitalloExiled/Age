using VkInstanceCreateFlags = Age.Vulkan.Native.VkInstanceCreateFlagBits;

#pragma warning disable IDE0001

namespace Age.Vulkan.Native;

/// <summary>
/// Structure specifying parameters of a newly created instance
/// </summary>
public unsafe struct VkInstanceCreateInfo
{
    /// <summary>
    /// Value identifying this structure.
    /// </summary>
    public VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A bitmask of <see cref="VkInstanceCreateFlagBits"/> indicating the behavior of the instance.
    /// </summary>
    public VkInstanceCreateFlags flags;

    /// <summary>
    /// Null or a pointer to a <see cref="VkApplicationInfo"/> structure. If not NULL, this information helps implementations recognize behavior inherent to classes of applications. <see cref="VkApplicationInfo"/> is defined in detail below.
    /// </summary>
    public VkApplicationInfo* pApplicationInfo;

    /// <summary>
    /// Is the number of global layers to enable.
    /// </summary>
    public uint enabledLayerCount;

    /// <summary>
    /// Is a pointer to an array of enabledLayerCount null-terminated UTF-8 strings containing the names of layers to enable for the created instance. The layers are loaded in the order they are listed in this array, with the first array element being the closest to the application, and the last array element being the closest to the driver. See the https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#extendingvulkan-layers section for further details.
    /// </summary>
    public char** ppEnabledLayerNames;

    /// <summary>
    /// Is the number of global extensions to enable.
    /// </summary>
    public uint enabledExtensionCount;

    /// <summary>
    /// Is a pointer to an array of enabledExtensionCount null-terminated UTF-8 strings containing the names of extensions to enable.
    /// </summary>
    public char** ppEnabledExtensionNames;
}
