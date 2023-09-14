using System.Text;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Native;

public abstract class VK
{
    /// <summary>
    /// the length in char values of an array containing a string with additional descriptive information about a query, as returned in <see cref="VkLayerProperties.description"/> and other queries.
    /// </summary>
    public const uint VK_MAX_DESCRIPTION_SIZE = 256;

    /// <summary>
    /// the length in char values of an array containing a layer or extension name string, as returned in <see cref="VkLayerProperties.layerName"/>, <see cref="VkExtensionProperties.extensionName"/>, and other queries.
    /// </summary>
    public const uint VK_MAX_EXTENSION_NAME_SIZE = 256;

    private unsafe delegate VkResult VkCreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance);
    private unsafe delegate void VkDestroyInstance(VkInstance instance, VkAllocationCallbacks* pAllocator);
    private unsafe delegate VkResult VkEnumerateInstanceExtensionProperties(byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties);
    private unsafe delegate VkResult VkEnumerateInstanceLayerProperties(uint* pPropertyCount, VkLayerProperties* pProperties);

    private readonly VkCreateInstance                       vkCreateInstance;
    private readonly VkDestroyInstance                      vkDestroyInstance;
    private readonly VkEnumerateInstanceExtensionProperties vkEnumerateInstanceExtensionProperties;
    private readonly VkEnumerateInstanceLayerProperties     vkEnumerateInstanceLayerProperties;

    public static uint ApiVersion_1_0 { get; } = MakeApiVersion(0, 1, 0, 0);

    protected abstract IVulkanLoader Loader { get; }

    public VK()
    {
        this.vkCreateInstance                       = this.Loader.Load<VkCreateInstance>("vkCreateInstance");
        this.vkDestroyInstance                      = this.Loader.Load<VkDestroyInstance>("vkDestroyInstance");
        this.vkEnumerateInstanceExtensionProperties = this.Loader.Load<VkEnumerateInstanceExtensionProperties>("vkEnumerateInstanceExtensionProperties");
        this.vkEnumerateInstanceLayerProperties     = this.Loader.Load<VkEnumerateInstanceLayerProperties>("vkEnumerateInstanceLayerProperties");
    }

    public static uint MakeApiVersion(uint variant, uint major, uint minor, uint patch = default) =>
        (variant << 29) | (major << 22) | (minor << 12) | patch;

    /// <summary>
    /// <para>Create a new Vulkan instance.</para>
    /// <see cref="CreateInstance"/> verifies that the requested layers exist. If not, <see cref="CreateInstance"/> will return <see cref="VkResult.VK_ERROR_LAYER_NOT_PRESENT"/>. Next <see cref="CreateInstance"/> verifies that the requested extensions are supported (e.g. in the implementation or in any enabled instance layer) and if any requested extension is not supported, <see cref="CreateInstance"/> must return VK_ERROR_EXTENSION_NOT_PRESENT. After verifying and enabling the instance layers and extensions the VkInstance object is created and returned to the application. If a requested extension is only supported by a layer, both the layer and the extension need to be specified at <see cref="CreateInstance"/> time for the creation to succeed.
    /// </summary>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkInstanceCreateInfo"/> structure controlling creation of the instance.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the Memory Allocation chapter.</param>
    /// <param name="pInstance">Points a VkInstance handle in which the resulting instance is returned.</param>
    public unsafe VkResult CreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance) =>
        this.vkCreateInstance.Invoke(pCreateInfo, pAllocator, pInstance);

    public unsafe VkResult CreateInstance(in VkInstanceCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkInstance instance)
    {
        fixed (VkInstanceCreateInfo*  pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &allocator)
        fixed (VkInstance*            pInstance   = &instance)
        {
            return this.vkCreateInstance.Invoke(pCreateInfo, allocator.Equals(default(VkAllocationCallbacks)) ? null : pAllocator, pInstance);
        }
    }

    /// <summary>
    /// Destroy an instance of Vulkan
    /// </summary>
    /// <param name="instance">The handle of the instance to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public unsafe void DestroyInstance(VkInstance instance, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyInstance.Invoke(instance, pAllocator);

    public unsafe void DestroyInstance(VkInstance instance, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyInstance.Invoke(instance, allocator.Equals(default(VkAllocationCallbacks)) ? null : pAllocator);
        }
    }

    /// <summary>
    /// <para>When layerName parameter is null, only extensions provided by the Vulkan implementation or by implicitly enabled layers are returned. When layerName is the name of a layer, the instance extensions provided by that layer are returned.</para>
    /// <para>If properties is null, then the number of extensions properties available is returned in propertyCount. Otherwise, propertyCount must point to a variable set by the user to the number of elements in the properties array, and on return the variable is overwritten with the number of structures actually written to properties. If propertyCount is less than the number of extension properties available, at most propertyCount structures will be written, and <see cref="VkResult.Incomplete"/> will be returned instead of <see cref="VkResult.Success"/>, to indicate that not all the available properties were returned.</para>
    /// <para>Because the list of available layers may change externally between calls to <see cref="EnumerateInstanceExtensionProperties"/>, two calls may retrieve different results if a layerName is available in one call but not in another. The extensions supported by a layer may also change between two calls, e.g. if the layer implementation is replaced by a different version between those calls.</para>
    /// <para>Implementations must not advertise any pair of extensions that cannot be enabled together due to behavioral differences, or any extension that cannot be enabled against the advertised version.</para>
    /// </summary>
    /// <param name="pLayerName">Either NULL or a pointer to a null-terminated UTF-8 string naming the layer to retrieve extensions from.</param>
    /// <param name="pPropertyCount">A pointer to an integer related to the number of extension properties available or queried, as described above.</param>
    /// <param name="pProperties">Either NULL or a pointer to an array of VkExtensionProperties structures.</param>
    /// <returns>Up to requested number of global extension properties</returns>
    public unsafe VkResult EnumerateInstanceExtensionProperties(byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties) =>
        this.vkEnumerateInstanceExtensionProperties.Invoke(pLayerName, pPropertyCount, pProperties);

    public unsafe VkResult EnumerateInstanceExtensionProperties(string? layerName, out uint propertyCount)
    {
        fixed (byte*                  pLayerName     = Encoding.UTF8.GetBytes(layerName ?? ""))
        fixed (uint*                  pPropertyCount = &propertyCount)
        {
            return this.vkEnumerateInstanceExtensionProperties.Invoke(pLayerName, pPropertyCount, null);
        }
    }

    public unsafe VkResult EnumerateInstanceExtensionProperties(string? layerName, out VkExtensionProperties[] properties)
    {
        this.EnumerateInstanceExtensionProperties(layerName, out uint propertyCount);

        properties = new VkExtensionProperties[(int)propertyCount];

        fixed (byte*                  pLayerName  = Encoding.UTF8.GetBytes(layerName ?? ""))
        fixed (VkExtensionProperties* pProperties = properties.AsSpan())
        {
            return this.vkEnumerateInstanceExtensionProperties.Invoke(pLayerName, &propertyCount, pProperties);
        }
    }

    /// <summary>
    /// <para>If pProperties is NULL, then the number of layer properties available is returned in pPropertyCount. Otherwise, pPropertyCount must point to a variable set by the user to the number of elements in the pProperties array, and on return the variable is overwritten with the number of structures actually written to pProperties. If pPropertyCount is less than the number of layer properties available, at most pPropertyCount structures will be written, and <see cref="VkResult.VK_INCOMPLETE"/> will be returned instead of <see cref="VkResult.VK_SUCCESS"/>, to indicate that not all the available properties were returned.</para>
    /// <para>The list of available layers may change at any time due to actions outside of the Vulkan implementation, so two calls to <see cref="EnumerateInstanceLayerProperties"/> with the same parameters may return different results, or retrieve different pPropertyCount values or pProperties contents. Once an instance has been created, the layers enabled for that instance will continue to be enabled and valid for the lifetime of that instance, even if some of them become unavailable for future instances.</para>
    /// </summary>
    /// <param name="pPropertyCount">A pointer to an integer related to the number of layer properties available or queried, as described above.</param>
    /// <param name="pProperties">Either NULL or a pointer to an array of <see cref="VkLayerProperties"/> structures.</param>
    /// <returns></returns>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public unsafe VkResult EnumerateInstanceLayerProperties(uint* pPropertyCount, VkLayerProperties* pProperties) =>
        this.vkEnumerateInstanceLayerProperties.Invoke(pPropertyCount, pProperties);

    public unsafe VkResult EnumerateInstanceLayerProperties(out uint propertyCount)
    {
        fixed (uint* pPropertyCount = &propertyCount)
        {
            return this.vkEnumerateInstanceLayerProperties.Invoke(pPropertyCount, null);
        }
    }

    public unsafe VkResult EnumerateInstanceLayerProperties(out VkLayerProperties[] properties)
    {
        this.EnumerateInstanceLayerProperties(out uint propertyCount);

        properties = new VkLayerProperties[(int)propertyCount];

        fixed (VkLayerProperties* pProperties = properties.AsSpan())
        {
            return this.vkEnumerateInstanceLayerProperties.Invoke(&propertyCount, pProperties);
        }
    }
}
