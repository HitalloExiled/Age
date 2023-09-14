using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Native;

public class Vulkan
{
    public const uint VK_MAX_EXTENSION_NAME_SIZE = 256;

    private unsafe delegate VkResult VkCreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance);
    private unsafe delegate VkResult VkEnumerateInstanceExtensionProperties(char* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties);

    private readonly VkCreateInstance                       vkCreateInstance;
    private readonly VkEnumerateInstanceExtensionProperties vkEnumerateInstanceExtensionProperties;

    private readonly IVulkanLoader loader;

    public static uint ApiVersion1_0 { get; } = MakeApiVersion(0, 1, 0, 0);

    public Vulkan(IVulkanLoader loader)
    {
        this.loader = loader;

        this.vkCreateInstance                       = this.loader.Load<VkCreateInstance>("vkCreateInstance");
        this.vkEnumerateInstanceExtensionProperties = this.loader.Load<VkEnumerateInstanceExtensionProperties>("vkEnumerateInstanceExtensionProperties");
    }

    public static uint MakeApiVersion(uint variant, uint major, uint minor, uint patch) =>
        (variant << 29) | (major << 22) | (minor << 12) | patch;

    /// <summary>
    /// <para>Create a new Vulkan instance.</para>
    /// <see cref="CreateInstance"/> verifies that the requested layers exist. If not, <see cref="CreateInstance"/> will return <see cref="VkResult.VK_ERROR_LAYER_NOT_PRESENT"/>. Next <see cref="CreateInstance"/> verifies that the requested extensions are supported (e.g. in the implementation or in any enabled instance layer) and if any requested extension is not supported, <see cref="CreateInstance"/> must return VK_ERROR_EXTENSION_NOT_PRESENT. After verifying and enabling the instance layers and extensions the VkInstance object is created and returned to the application. If a requested extension is only supported by a layer, both the layer and the extension need to be specified at <see cref="CreateInstance"/> time for the creation to succeed.
    /// </summary>
    /// <param name="createInfo">A pointer to a <see cref="VkInstanceCreateInfo"/> structure controlling creation of the instance.</param>
    /// <param name="allocator">Controls host memory allocation as described in the Memory Allocation chapter.</param>
    /// <param name="instance">Points a VkInstance handle in which the resulting instance is returned.</param>
    public unsafe VkResult CreateInstance(in VkInstanceCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkInstance instance)
    {
        fixed (VkInstanceCreateInfo*  pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &allocator)
        fixed (VkInstance*            pInstance   = &instance)
        {
            var result = this.vkCreateInstance.Invoke(pCreateInfo, pAllocator, pInstance);

            instance = (nint)pInstance;

            return result;
        }
    }

    /// <summary>
    /// <para>When layerName parameter is null, only extensions provided by the Vulkan implementation or by implicitly enabled layers are returned. When layerName is the name of a layer, the instance extensions provided by that layer are returned.</para>
    /// <para>If properties is null, then the number of extensions properties available is returned in propertyCount. Otherwise, propertyCount must point to a variable set by the user to the number of elements in the properties array, and on return the variable is overwritten with the number of structures actually written to properties. If propertyCount is less than the number of extension properties available, at most propertyCount structures will be written, and <see cref="VkResult.Incomplete"/> will be returned instead of <see cref="VkResult.Success"/>, to indicate that not all the available properties were returned.</para>
    /// <para>Because the list of available layers may change externally between calls to <see cref="EnumerateInstanceExtensionProperties"/>, two calls may retrieve different results if a layerName is available in one call but not in another. The extensions supported by a layer may also change between two calls, e.g. if the layer implementation is replaced by a different version between those calls.</para>
    /// <para>Implementations must not advertise any pair of extensions that cannot be enabled together due to behavioral differences, or any extension that cannot be enabled against the advertised version.</para>
    /// </summary>
    /// <param name="layerName">Either null or a UTF-8 string naming the layer to retrieve extensions from.</param>
    /// <param name="propertyCount">Outs an integer related to the number of extension properties available or queried, as described below.</param>
    /// <param name="properties">Either null or an array of VkExtensionProperties structures.</param>
    /// <returns>Up to requested number of global extension properties</returns>
    public unsafe VkResult EnumerateInstanceExtensionProperties(string? layerName, out uint propertyCount, VkExtensionProperties[]? properties)
    {
        fixed (char*                  pLayerName     = layerName)
        fixed (uint*                  pPropertyCount = &propertyCount)
        fixed (VkExtensionProperties* pProperties    = properties.AsSpan())
        {
            return this.vkEnumerateInstanceExtensionProperties.Invoke(pLayerName, pPropertyCount, pProperties);
        }
    }
}
