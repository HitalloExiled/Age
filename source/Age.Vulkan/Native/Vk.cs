using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Types;

using static Age.Core.Unsafe.UnmanagedUtils;

namespace Age.Vulkan.Native;

public unsafe class Vk(IVulkanLoader loader)
{
    /// <summary>
    /// The length in char values of an array containing a string with additional descriptive information about a query, as returned in <see cref="VkLayerProperties.description"/> and other queries.
    /// </summary>
    public const uint VK_MAX_DESCRIPTION_SIZE = 256;

    /// <summary>
    /// The length in char values of an array containing a layer or extension name string, as returned in <see cref="VkLayerProperties.layerName"/>, <see cref="VkExtensionProperties.extensionName"/>, and other queries.
    /// </summary>
    public const uint VK_MAX_EXTENSION_NAME_SIZE = 256;

    /// <summary>
    /// The length in char values of an array containing a physical device name string, as returned in <see cref="VkPhysicalDeviceProperties.deviceName"/>.
    /// </summary>
    public const uint VK_MAX_PHYSICAL_DEVICE_NAME_SIZE = 256;

    /// <summary>
    /// The length in byte values of an array containing a universally unique device or driver build identifier, as returned in <see cref="VkPhysicalDeviceIDProperties.deviceUUID"/> and <see cref="VkPhysicalDeviceIDProperties.driverUUID"/>.
    /// </summary>
    public const uint VK_UUID_SIZE = 16;

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateDevice(VkPhysicalDevice physicalDevice, VkDeviceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDevice* pDevice);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateImageView(VkDevice device, VkImageViewCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkImageView* pView);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreatePipelineLayout(VkDevice device, VkPipelineLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkPipelineLayout* pPipelineLayout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateRenderPass(VkDevice device, VkRenderPassCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkRenderPass* pRenderPass);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateShaderModule(VkDevice device, VkShaderModuleCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkShaderModule* pShaderModule);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyDevice(VkDevice device, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyInstance(VkInstance instance, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyImageView(VkDevice device, VkImageView imageView, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyPipelineLayout(VkDevice device, VkPipelineLayout pipelineLayout, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyRenderPass(VkDevice device, VkRenderPass renderPass, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyShaderModule(VkDevice device, VkShaderModule shaderModule, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkEnumerateInstanceExtensionProperties(byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkEnumerateInstanceLayerProperties(uint* pPropertyCount, VkLayerProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkEnumeratePhysicalDevices(VkInstance instance, uint* pPhysicalDeviceCount, VkPhysicalDevice* pPhysicalDevices);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void* VkGetDeviceProcAddr(VkDevice device, byte* pName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkGetDeviceQueue(VkDevice device, uint queueFamilyIndex, uint queueIndex, VkQueue* pQueue);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void* VkGetInstanceProcAddr(VkInstance instance, byte* pName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkGetPhysicalDeviceFeatures(VkPhysicalDevice physicalDevice, VkPhysicalDeviceFeatures* pFeatures);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkGetPhysicalDeviceProperties(VkPhysicalDevice physicalDevice, VkPhysicalDeviceProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkGetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice physicalDevice, uint* pQueueFamilyPropertyCount, VkQueueFamilyProperties* pQueueFamilyProperties);


    private readonly Dictionary<string, HashSet<string>> deviceExtensionsMap   = new();
    private readonly Dictionary<string, HashSet<string>> instanceExtensionsMap = new();

    private readonly VkCreateDevice                           vkCreateDevice                           = loader.Load<VkCreateDevice>("vkCreateDevice");
    private readonly VkCreateImageView                        vkCreateImageView                        = loader.Load<VkCreateImageView>("vkCreateImageView");
    private readonly VkCreateInstance                         vkCreateInstance                         = loader.Load<VkCreateInstance>("vkCreateInstance");
    private readonly VkCreatePipelineLayout                   vkCreatePipelineLayout                   = loader.Load<VkCreatePipelineLayout>("vkCreatePipelineLayout");
    private readonly VkCreateRenderPass                       vkCreateRenderPass                       = loader.Load<VkCreateRenderPass>("vkCreateRenderPass");
    private readonly VkCreateShaderModule                     vkCreateShaderModule                     = loader.Load<VkCreateShaderModule>("vkCreateShaderModule");
    private readonly VkDestroyDevice                          vkDestroyDevice                          = loader.Load<VkDestroyDevice>("vkDestroyDevice");
    private readonly VkDestroyInstance                        vkDestroyInstance                        = loader.Load<VkDestroyInstance>("vkDestroyInstance");
    private readonly VkDestroyImageView                       vkDestroyImageView                       = loader.Load<VkDestroyImageView>("vkDestroyImageView");
    private readonly VkDestroyPipelineLayout                  vkDestroyPipelineLayout                  = loader.Load<VkDestroyPipelineLayout>("vkDestroyPipelineLayout");
    private readonly VkDestroyRenderPass                      vkDestroyRenderPass                      = loader.Load<VkDestroyRenderPass>("vkDestroyRenderPass");
    private readonly VkDestroyShaderModule                    vkDestroyShaderModule                    = loader.Load<VkDestroyShaderModule>("vkDestroyShaderModule");
    private readonly VkEnumerateDeviceExtensionProperties     vkEnumerateDeviceExtensionProperties     = loader.Load<VkEnumerateDeviceExtensionProperties>("vkEnumerateDeviceExtensionProperties");
    private readonly VkEnumerateInstanceExtensionProperties   vkEnumerateInstanceExtensionProperties   = loader.Load<VkEnumerateInstanceExtensionProperties>("vkEnumerateInstanceExtensionProperties");
    private readonly VkEnumerateInstanceLayerProperties       vkEnumerateInstanceLayerProperties       = loader.Load<VkEnumerateInstanceLayerProperties>("vkEnumerateInstanceLayerProperties");
    private readonly VkEnumeratePhysicalDevices               vkEnumeratePhysicalDevices               = loader.Load<VkEnumeratePhysicalDevices>("vkEnumeratePhysicalDevices");
    private readonly VkGetDeviceQueue                         vkGetDeviceQueue                         = loader.Load<VkGetDeviceQueue>("vkGetDeviceQueue");
    private readonly VkGetDeviceProcAddr                      vkGetDeviceProcAddr                      = loader.Load<VkGetDeviceProcAddr>("vkGetDeviceProcAddr");
    private readonly VkGetInstanceProcAddr                    vkGetInstanceProcAddr                    = loader.Load<VkGetInstanceProcAddr>("vkGetInstanceProcAddr");
    private readonly VkGetPhysicalDeviceFeatures              vkGetPhysicalDeviceFeatures              = loader.Load<VkGetPhysicalDeviceFeatures>("vkGetPhysicalDeviceFeatures");
    private readonly VkGetPhysicalDeviceProperties            vkGetPhysicalDeviceProperties            = loader.Load<VkGetPhysicalDeviceProperties>("vkGetPhysicalDeviceProperties");
    private readonly VkGetPhysicalDeviceQueueFamilyProperties vkGetPhysicalDeviceQueueFamilyProperties = loader.Load<VkGetPhysicalDeviceQueueFamilyProperties>("vkGetPhysicalDeviceQueueFamilyProperties");

    public static uint ApiVersion_1_0 { get; } = MakeApiVersion(0, 1, 0, 0);

    public static uint MakeApiVersion(uint variant, uint major, uint minor, uint patch = default) =>
        (variant << 29) | (major << 22) | (minor << 12) | patch;

    internal T GetDeviceProcAddr<T>(string extension, VkDevice device, string name) where T : Delegate
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var pointer = this.vkGetDeviceProcAddr.Invoke(device, pName);

            return pointer != null
                ? Marshal.GetDelegateForFunctionPointer<T>((nint)pointer)
                : throw new Exception($"Can't find the proc {name} on the provided instance. Check if the extension {extension} is loaded.");
        }
    }

    internal T GetInstanceProcAddr<T>(string extension, VkInstance instance, string name) where T : Delegate
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var pointer = this.vkGetInstanceProcAddr.Invoke(instance, pName);

            return pointer != null
                ? Marshal.GetDelegateForFunctionPointer<T>((nint)pointer)
                : throw new Exception($"Can't find the proc {name} on the provided instance. Check if the extension {extension} is loaded.");
        }
    }

    /// <summary>
    /// Create an image view from an existing image.
    /// </summary>
    /// <param name="device">The logical device that creates the image view.</param>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkImageViewCreateInfo"/> structure containing parameters to be used to create the image view.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pView">A pointer to a VkImageView handle in which the resulting image view object is returned.</param>
    public VkResult CreateImageView(VkDevice device, VkImageViewCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkImageView* pView) =>
        this.vkCreateImageView.Invoke(device, pCreateInfo, pAllocator, pView);

    public VkResult CreateImageView(VkDevice device, in VkImageViewCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkImageView view)
    {
        fixed (VkImageViewCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &allocator)
        fixed (VkImageView*           pView       = &view)
        {
            return this.vkCreateImageView.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pView);
        }
    }

    /// <summary>
    /// <para><see cref="CreateDevice"/> verifies that extensions and features requested in the ppEnabledExtensionNames and pEnabledFeatures members of pCreateInfo, respectively, are supported by the implementation. If any requested extension is not supported, <see cref="CreateDevice"/> must return <see cref="VkResult.VK_ERROR_EXTENSION_NOT_PRESENT"/>. If any requested feature is not supported, <see cref="CreateDevice"/> must return <see cref="VkResult.VK_ERROR_FEATURE_NOT_PRESENT"/>. Support for extensions can be checked before creating a device by querying vkEnumerateDeviceExtensionProperties. Support for features can similarly be checked by querying <see cref="GetPhysicalDeviceFeatures"/>.</para>
    /// <para>After verifying and enabling the extensions the <see cref="VkDevice"/> object is created and returned to the application.</para>
    /// <para>Multiple logical devices can be created from the same physical device. Logical device creation may fail due to lack of device-specific resources (in addition to other errors). If that occurs, <see cref="CreateDevice"/> will return <see cref="VkResult.VK_ERROR_TOO_MANY_OBJECTS"/>.</para>
    /// </summary>
    /// <param name="physicalDevice">Must be one of the device handles returned from a call to <see cref="EnumeratePhysicalDevices"/> (see <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#devsandqueues-physical-device-enumeration">Physical Device Enumeration</see>).</param>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkDeviceCreateInfo"/> structure containing information about how to create the device.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pDevice">A pointer to a handle in which the created <see cref="VkDevice"/> is returned.</param>
    public VkResult CreateDevice(VkPhysicalDevice physicalDevice, VkDeviceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDevice* pDevice) =>
        this.vkCreateDevice.Invoke(physicalDevice, pCreateInfo, pAllocator, pDevice);

    public VkResult CreateDevice(VkPhysicalDevice physicalDevice, in VkDeviceCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkDevice device)
    {
        fixed (VkDeviceCreateInfo*    pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &allocator)
        fixed (VkDevice*              pDevice     = &device)
        {
            return this.vkCreateDevice.Invoke(physicalDevice, pCreateInfo, NullIfDefault(allocator, pAllocator), pDevice);
        }
    }

    /// <summary>
    /// <para>Create a new Vulkan instance.</para>
    /// <see cref="CreateInstance"/> verifies that the requested layers exist. If not, <see cref="CreateInstance"/> will return <see cref="VkResult.VK_ERROR_LAYER_NOT_PRESENT"/>. Next <see cref="CreateInstance"/> verifies that the requested extensions are supported (e.g. in the implementation or in any enabled instance layer) and if any requested extension is not supported, <see cref="CreateInstance"/> must return VK_ERROR_EXTENSION_NOT_PRESENT. After verifying and enabling the instance layers and extensions the VkInstance object is created and returned to the application. If a requested extension is only supported by a layer, both the layer and the extension need to be specified at <see cref="CreateInstance"/> time for the creation to succeed.
    /// </summary>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkInstanceCreateInfo"/> structure controlling creation of the instance.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pInstance">Points a VkInstance handle in which the resulting instance is returned.</param>
    public VkResult CreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance) =>
        this.vkCreateInstance.Invoke(pCreateInfo, pAllocator, pInstance);

    public VkResult CreateInstance(in VkInstanceCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkInstance instance)
    {
        fixed (VkInstanceCreateInfo*  pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &allocator)
        fixed (VkInstance*            pInstance   = &instance)
        {
            return this.vkCreateInstance.Invoke(pCreateInfo, NullIfDefault(allocator, pAllocator), pInstance);
        }
    }

    /// <summary>
    /// Creates a new pipeline layout object.
    /// </summary>
    /// <param name="device">The logical device that creates the pipeline layout.</param>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkPipelineLayoutCreateInfo"/> structure specifying the state of the pipeline layout object.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pPipelineLayout">A pointer to a <see cref="VkPipelineLayout"/> handle in which the resulting pipeline layout object is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult CreatePipelineLayout(VkDevice device, VkPipelineLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkPipelineLayout* pPipelineLayout) =>
        this.vkCreatePipelineLayout.Invoke(device, pCreateInfo, pAllocator, pPipelineLayout);

    public VkResult CreatePipelineLayout(VkDevice device, in VkPipelineLayoutCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkPipelineLayout pipelineLayout)
    {
        fixed (VkPipelineLayoutCreateInfo* pCreateInfo     = &createInfo)
        fixed (VkAllocationCallbacks*      pAllocator      = &allocator)
        fixed (VkPipelineLayout*           pPipelineLayout = &pipelineLayout)
        {
            return this.vkCreatePipelineLayout.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pPipelineLayout);
        }
    }

    /// <summary>
    /// Create a new render pass object.
    /// </summary>
    /// <param name="device">The logical device that creates the render pass.</param>
    /// <param name="pCreateInfo">A pointer to a VkRenderPassCreateInfo structure describing the parameters of the render pass.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pRenderPass">A pointer to a VkRenderPass handle in which the resulting render pass object is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult CreateRenderPass(VkDevice device, VkRenderPassCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkRenderPass* pRenderPass) =>
        this.vkCreateRenderPass.Invoke(device, pCreateInfo, pAllocator, pRenderPass);

    public VkResult CreateRenderPass(VkDevice device, in VkRenderPassCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkRenderPass renderPass)
    {
        fixed (VkRenderPassCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*  pAllocator  = &allocator)
        fixed (VkRenderPass*           pRenderPass = &renderPass)
        {
            return this.vkCreateRenderPass.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pRenderPass);
        }
    }

    /// <summary>
    /// <para>Creates a new shader module object.</para>
    /// <para>Once a shader module has been created, any entry points it contains can be used in pipeline shader stages as described in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-compute">Compute Pipelines</see> and <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-graphics">Graphics Pipelines</see>.</para>
    /// <remarks>If the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-maintenance5">maintenance5</see> feature is enabled, shader module creation can be omitted entirely. Instead, applications should provide the <see cref="VkShaderModuleCreateInfo"/> structure directly in to pipeline creation by chaining it to <see cref="VkPipelineShaderStageCreateInfo"/>. This avoids the overhead of creating and managing an additional object.</remarks>
    /// </summary>
    /// <param name="device">The logical device that creates the shader module.</param>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkShaderModuleCreateInfo"/> structure.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pShaderModule">A pointer to a <see cref="VkShaderModule"/> handle in which the resulting shader module object is returned.</param>
    public VkResult CreateShaderModule(VkDevice device, VkShaderModuleCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkShaderModule* pShaderModule) =>
        this.vkCreateShaderModule.Invoke(device, pCreateInfo, pAllocator, pShaderModule);

    public VkResult CreateShaderModule(VkDevice device, in VkShaderModuleCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkShaderModule shaderModule)
    {
        fixed (VkShaderModuleCreateInfo* pCreateInfo   = &createInfo)
        fixed (VkAllocationCallbacks*    pAllocator    = &allocator)
        fixed (VkShaderModule*           pShaderModule = &shaderModule)
        {
            return this.vkCreateShaderModule.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pShaderModule);
        }
    }

    /// <summary>
    /// <para>Destroy a logical device.</para>
    /// <para>To ensure that no work is active on the device, <see cref="DeviceWaitIdle"/> can be used to gate the destruction of the device. Prior to destroying a device, an application is responsible for destroying/freeing any Vulkan objects that were created using that device as the first parameter of the corresponding vkCreate* or vkAllocate* command.</para>
    /// </summary>
    /// <param name="device">The logical device to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    public void DestroyDevice(VkDevice device, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyDevice.Invoke(device, pAllocator);

    public void DestroyDevice(VkDevice device, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyDevice.Invoke(device, NullIfDefault(allocator, pAllocator));
        }
    }

    /// <summary>
    /// Destroy an image view object.
    /// </summary>
    /// <param name="device">The logical device that destroys the image view.</param>
    /// <param name="imageView">The image view to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    public void DestroyImageView(VkDevice device, VkImageView imageView, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyImageView.Invoke(device, imageView, pAllocator);

    public void DestroyImageView(VkDevice device, VkImageView imageView, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyImageView.Invoke(device, imageView, pAllocator);
        }
    }

    /// <summary>
    /// Destroy an instance of Vulkan
    /// </summary>
    /// <param name="instance">The handle of the instance to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroyInstance(VkInstance instance, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyInstance.Invoke(instance, pAllocator);

    public void DestroyInstance(VkInstance instance, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyInstance.Invoke(instance, NullIfDefault(allocator, pAllocator));
        }
    }

    /// <summary>
    /// Destroy a pipeline layout object.
    /// </summary>
    /// <param name="device">The logical device that destroys the pipeline layout.</param>
    /// <param name="pipelineLayout">The pipeline layout to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroyPipelineLayout(VkDevice device, VkPipelineLayout pipelineLayout, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyPipelineLayout.Invoke(device, pipelineLayout, pAllocator);

    public void DestroyPipelineLayout(VkDevice device, VkPipelineLayout pipelineLayout, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyPipelineLayout.Invoke(device, pipelineLayout, NullIfDefault(allocator, pAllocator));
        }
    }

    /// <summary>
    /// Destroy a render pass object.
    /// </summary>
    /// <param name="device">The logical device that destroys the render pass.</param>
    /// <param name="renderPass">The handle of the render pass to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroyRenderPass(VkDevice device, VkRenderPass renderPass, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyRenderPass.Invoke(device, renderPass, pAllocator);

    public void DestroyRenderPass(VkDevice device, VkRenderPass renderPass, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyRenderPass.Invoke(device, renderPass, NullIfDefault(allocator, pAllocator));
        }
    }

    /// <summary>
    /// <para>Destroy a shader module.</para>
    /// <para>A shader module can be destroyed while pipelines created using its shaders are still in use.</para>
    /// </summary>
    /// <param name="device">The logical device that destroys the shader module.</param>
    /// <param name="shaderModule">The handle of the shader module to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroyShaderModule(VkDevice device, VkShaderModule shaderModule, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyShaderModule.Invoke(device, shaderModule, pAllocator);

    public void DestroyShaderModule(VkDevice device, VkShaderModule shaderModule, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyShaderModule.Invoke(device, shaderModule, NullIfDefault(allocator, pAllocator));
        }
    }

    /// <summary>
    /// <para>Returns properties of available physical device extensions.</para>
    /// <para>When pLayerName parameter is NULL, only extensions provided by the Vulkan implementation or by implicitly enabled layers are returned. When pLayerName is the name of a layer, the device extensions provided by that layer are returned.</para>
    /// <para>Implementations must not advertise any pair of extensions that cannot be enabled together due to behavioral differences, or any extension that cannot be enabled against the advertised version.</para>
    /// <para>Implementations claiming support for the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#roadmap-2022">Roadmap 2022</see> profile must advertise the VK_KHR_global_priority extension in pProperties.</para>
    /// <remarks>Due to platform details on Android, <see cref="EnumerateDeviceExtensionProperties"/> may be called with physicalDevice equal to NULL during layer discovery. This behaviour will only be observed by layer implementations, and not the underlying Vulkan driver.</remarks>
    /// </summary>
    /// <param name="physicalDevice">The physical device that will be queried.</param>
    /// <param name="pLayerName">Either NULL or a pointer to a null-terminated UTF-8 string naming the layer to retrieve extensions from.</param>
    /// <param name="pPropertyCount">A pointer to an integer related to the number of extension properties available or queried, and is treated in the same fashion as the <see cref="EnumerateInstanceExtensionProperties"/> pPropertyCount parameter.</param>
    /// <param name="pProperties">Either NULL or a pointer to an array of <see cref="VkExtensionProperties"/> structures.</param>
    public VkResult EnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties) =>
        this.vkEnumerateDeviceExtensionProperties.Invoke(physicalDevice, pLayerName, pPropertyCount, pProperties);

    public VkResult EnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string? layerName, out uint propertyCount)
    {
        fixed (byte* pLayerName     = Encoding.UTF8.GetBytes(layerName ?? ""))
        fixed (uint* pPropertyCount = &propertyCount)
        {
            return this.vkEnumerateDeviceExtensionProperties.Invoke(physicalDevice, pLayerName, pPropertyCount, null);
        }
    }

    public VkResult EnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, string? layerName, out VkExtensionProperties[] properties)
    {
        if (this.EnumerateDeviceExtensionProperties(physicalDevice, layerName, out uint propertyCount) is var result and not VkResult.VK_SUCCESS)
        {
            properties = Array.Empty<VkExtensionProperties>();

            return result;
        }

        properties = new VkExtensionProperties[propertyCount];

        fixed (byte* pLayerName     = Encoding.UTF8.GetBytes(layerName ?? ""))
        fixed (VkExtensionProperties* pProperties = properties)
        {
            return this.vkEnumerateDeviceExtensionProperties.Invoke(physicalDevice, pLayerName, &propertyCount, pProperties);
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
    public VkResult EnumerateInstanceExtensionProperties(byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties) =>
        this.vkEnumerateInstanceExtensionProperties.Invoke(pLayerName, pPropertyCount, pProperties);

    public VkResult EnumerateInstanceExtensionProperties(string? layerName, out uint propertyCount)
    {
        fixed (byte* pLayerName     = Encoding.UTF8.GetBytes(layerName ?? ""))
        fixed (uint* pPropertyCount = &propertyCount)
        {
            return this.vkEnumerateInstanceExtensionProperties.Invoke(pLayerName, pPropertyCount, null);
        }
    }

    public VkResult EnumerateInstanceExtensionProperties(string? layerName, out VkExtensionProperties[] properties)
    {
        if (this.EnumerateInstanceExtensionProperties(layerName, out uint propertyCount) is var result and not VkResult.VK_SUCCESS)
        {
            properties = Array.Empty<VkExtensionProperties>();

            return result;
        }

        properties = new VkExtensionProperties[propertyCount];

        fixed (byte*                  pLayerName  = Encoding.UTF8.GetBytes(layerName ?? ""))
        fixed (VkExtensionProperties* pProperties = properties)
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
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult EnumerateInstanceLayerProperties(uint* pPropertyCount, VkLayerProperties* pProperties) =>
        this.vkEnumerateInstanceLayerProperties.Invoke(pPropertyCount, pProperties);

    public VkResult EnumerateInstanceLayerProperties(out uint propertyCount)
    {
        fixed (uint* pPropertyCount = &propertyCount)
        {
            return this.vkEnumerateInstanceLayerProperties.Invoke(pPropertyCount, null);
        }
    }

    public VkResult EnumerateInstanceLayerProperties(out VkLayerProperties[] properties)
    {
        if (this.EnumerateInstanceLayerProperties(out uint propertyCount) is var result and not VkResult.VK_SUCCESS)
        {
            properties = Array.Empty<VkLayerProperties>();

            return result;
        }

        properties = new VkLayerProperties[propertyCount];

        fixed (VkLayerProperties* pProperties = properties)
        {
            return this.vkEnumerateInstanceLayerProperties.Invoke(&propertyCount, pProperties);
        }
    }

    /// <summary>
    /// If pPhysicalDevices is NULL, then the number of physical devices available is returned in <see cref="pPhysicalDeviceCount"/>. Otherwise, <see cref="pPhysicalDeviceCount"/> must point to a variable set by the user to the number of elements in the <see cref="pPhysicalDevices"/> array, and on return the variable is overwritten with the number of handles actually written to <see cref="pPhysicalDevices"/>. If <see cref="pPhysicalDeviceCount"/> is less than the number of physical devices available, at most <see cref="pPhysicalDeviceCount"/> structures will be written, and <see cref="VkResult.VK_INCOMPLETE"/> will be returned instead of <see cref="VkResult.VK_SUCCESS"/>, to indicate that not all the available physical devices were returned.
    /// </summary>
    /// <param name="instance">A handle to a Vulkan instance previously created with <see cref="CreateInstance"/>.</param>
    /// <param name="pPhysicalDeviceCount">A pointer to an integer related to the number of physical devices available or queried, as described above.</param>
    /// <param name="pPhysicalDevices">Either NULL or a pointer to an array of VkPhysicalDevice handles.</param>
    public VkResult EnumeratePhysicalDevices(VkInstance instance, uint* pPhysicalDeviceCount, VkPhysicalDevice* pPhysicalDevices) =>
        this.vkEnumeratePhysicalDevices.Invoke(instance, pPhysicalDeviceCount, pPhysicalDevices);

    public VkResult EnumeratePhysicalDevices(VkInstance instance, out uint physicalDeviceCount)
    {
        fixed (uint* pPhysicalDeviceCount = &physicalDeviceCount)
        {
            return this.vkEnumeratePhysicalDevices.Invoke(instance, pPhysicalDeviceCount, null);
        }
    }

    public VkResult EnumeratePhysicalDevices(VkInstance instance, out VkPhysicalDevice[] physicalDevices)
    {
        if (this.EnumeratePhysicalDevices(instance, out uint physicalDeviceCount) is var result and not VkResult.VK_SUCCESS)
        {
            physicalDevices = Array.Empty<VkPhysicalDevice>();

            return result;
        }

        physicalDevices = new VkPhysicalDevice[physicalDeviceCount];

        fixed (VkPhysicalDevice* pPhysicalDevices = physicalDevices)
        {
            return this.vkEnumeratePhysicalDevices.Invoke(instance, &physicalDeviceCount, pPhysicalDevices);
        }
    }

    /// <summary>
    /// In order to support systems with multiple Vulkan implementations, the function pointers returned by <see cref="GetInstanceProcAddr"/> may point to dispatch code that calls a different real implementation for different <see cref="VkDevice"/> objects or their child objects. The overhead of the internal dispatch for <see cref="VkDevice"/> objects can be avoided by obtaining device-specific function pointers for any commands that use a device or device-child object as their dispatchable object.
    /// </summary>
    public void* GetDeviceProcAddr(VkDevice device, byte* pName) =>
        this.vkGetDeviceProcAddr.Invoke(device, pName);

    public T? GetDeviceProcAddr<T>(VkDevice device, string name) where T : Delegate
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var pointer = this.vkGetDeviceProcAddr.Invoke(device, pName);

            return pointer != null ? Marshal.GetDelegateForFunctionPointer<T>((nint)pointer) : null;
        }
    }

    /// <summary>
    /// <para>Get a queue handle from a device.</para>
    /// <para><see cref="GetDeviceQueue"/> must only be used to get queues that were created with the flags parameter of <see cref="VkDeviceQueueCreateInfo"/> set to zero. To get queues that were created with a non-zero flags parameter use <see cref="GetDeviceQueue2"/>.</para>
    /// </summary>
    /// <param name="device">The logical device that owns the queue.</param>
    /// <param name="queueFamilyIndex">The index of the queue family to which the queue belongs.</param>
    /// <param name="queueIndex">The index within this queue family of the queue to retrieve.</param>
    /// <param name="pQueue"A pointer to a <see cref="VkQueue"/> object that will be filled with the handle for the requested queue.></param>
    public void GetDeviceQueue(VkDevice device, uint queueFamilyIndex, uint queueIndex, VkQueue* pQueue) =>
        this.vkGetDeviceQueue.Invoke(device, queueFamilyIndex, queueIndex, pQueue);

    public void GetDeviceQueue(VkDevice device, uint queueFamilyIndex, uint queueIndex, out VkQueue queue)
    {
        fixed (VkQueue* pQueue = &queue)
        {
            this.vkGetDeviceQueue.Invoke(device, queueFamilyIndex, queueIndex, pQueue);
        }
    }

    public T GetInstanceExtension<T>(VkInstance instance, string? layer = default) where T : class, IVkInstanceExtension =>
        this.TryGetInstanceExtension<T>(instance, layer, out var extension)
            ? extension
            : throw new Exception($"Cant find extension {T.Name}");

    /// <summary>
    /// Return a function pointer for a command.
    /// </summary>
    /// <param name="instance">The instance that the function pointer will be compatible with, or NULL for commands not dependent on any instance.</param>
    /// <param name="pName">The name of the command to obtain.</param>
    public void* GetInstanceProcAddr(VkInstance instance, byte* pName) =>
        this.vkGetInstanceProcAddr.Invoke(instance, pName);

    public T? GetInstanceProcAddr<T>(VkInstance instance, string name) where T : Delegate
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var pointer = this.vkGetInstanceProcAddr.Invoke(instance, pName);

            return pointer != null ? Marshal.GetDelegateForFunctionPointer<T>((nint)pointer) : null;
        }
    }

    /// <summary>
    /// <para>Reports capabilities of a physical device.</para>
    /// <para><see cref="GetInstanceProcAddr"/> itself is obtained in a platform- and loader- specific manner. Typically, the loader library will export this command as a function symbol, so applications can link against the loader library, or load it dynamically and look up the symbol using platform-specific APIs.</para>
    /// </summary>
    /// <param name="physicalDevice">The physical device from which to query the supported features.</param>
    /// <param name="pFeatures">A pointer to a <see cref="VkPhysicalDeviceFeatures"/> structure in which the physical device features are returned. For each feature, a value of true specifies that the feature is supported on this physical device, and VK_FALSE specifies that the feature is not supported.</param>
    public void GetPhysicalDeviceFeatures(VkPhysicalDevice physicalDevice, VkPhysicalDeviceFeatures* pFeatures) =>
        this.vkGetPhysicalDeviceFeatures.Invoke(physicalDevice, pFeatures);

    public void GetPhysicalDeviceFeatures(VkPhysicalDevice physicalDevice, out VkPhysicalDeviceFeatures features)
    {
        fixed (VkPhysicalDeviceFeatures* pFeatures = &features)
        {
            this.vkGetPhysicalDeviceFeatures.Invoke(physicalDevice, pFeatures);
        }
    }

    /// <summary>
    /// Returns properties of a physical device.
    /// </summary>
    /// <param name="physicalDevice">The handle to the physical device whose properties will be queried.</param>
    /// <param name="pProperties">A pointer to a <see cref="VkPhysicalDeviceProperties"/> structure in which properties are returned.</param>
    public void GetPhysicalDeviceProperties(VkPhysicalDevice physicalDevice, VkPhysicalDeviceProperties* pProperties) =>
        this.vkGetPhysicalDeviceProperties.Invoke(physicalDevice, pProperties);

    public void GetPhysicalDeviceProperties(VkPhysicalDevice physicalDevice, out VkPhysicalDeviceProperties properties)
    {
        fixed (VkPhysicalDeviceProperties* pProperties = &properties)
        {
            this.vkGetPhysicalDeviceProperties.Invoke(physicalDevice, pProperties);
        }
    }

    /// <summary>
    /// If pQueueFamilyProperties is NULL, then the number of queue families available is returned in pQueueFamilyPropertyCount. Implementations must support at least one queue family. Otherwise, pQueueFamilyPropertyCount must point to a variable set by the user to the number of elements in the pQueueFamilyProperties array, and on return the variable is overwritten with the number of structures actually written to pQueueFamilyProperties. If pQueueFamilyPropertyCount is less than the number of queue families available, at most pQueueFamilyPropertyCount structures will be written.
    /// </summary>
    /// <param name="physicalDevice">The handle to the physical device whose properties will be queried.</param>
    /// <param name="pQueueFamilyPropertyCount">A pointer to an integer related to the number of queue families available or queried, as described above.</param>
    /// <param name="pQueueFamilyProperties">Either NULL or a pointer to an array of <see cref="VkQueueFamilyProperties"/> structures.</param>
    public void GetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice physicalDevice, uint* pQueueFamilyPropertyCount, VkQueueFamilyProperties* pQueueFamilyProperties) =>
        this.vkGetPhysicalDeviceQueueFamilyProperties.Invoke(physicalDevice, pQueueFamilyPropertyCount, pQueueFamilyProperties);

    public void GetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice physicalDevice, out uint queueFamilyPropertyCount)
    {
        fixed (uint* pQueueFamilyPropertyCount = &queueFamilyPropertyCount)
        {
            this.vkGetPhysicalDeviceQueueFamilyProperties.Invoke(physicalDevice, pQueueFamilyPropertyCount, null);
        }
    }

    public void GetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice physicalDevice, out VkQueueFamilyProperties[] queueFamilyProperties)
    {
        this.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, out uint queueFamilyPropertyCount);

        queueFamilyProperties = new VkQueueFamilyProperties[queueFamilyPropertyCount];

        fixed (VkQueueFamilyProperties* pQueueFamilyProperties = queueFamilyProperties)
        {
            this.vkGetPhysicalDeviceQueueFamilyProperties.Invoke(physicalDevice, &queueFamilyPropertyCount, pQueueFamilyProperties);
        }
    }

    public bool HasDeviceExtension(VkPhysicalDevice device, string name, string? layer = default)
    {
        if (!this.deviceExtensionsMap.TryGetValue(layer ?? "", out var extensions))
        {
            this.EnumerateDeviceExtensionProperties(device, layer, out VkExtensionProperties[] properties);

            this.instanceExtensionsMap[layer ?? ""] = extensions = properties.Select(x => Marshal.PtrToStringAnsi((nint)x.extensionName)!).ToHashSet();
        }

        return extensions.Contains(name);
    }

    public bool HasInstanceExtension(string name, string? layer = default)
    {
        if (!this.instanceExtensionsMap.TryGetValue(layer ?? "", out var extensions))
        {
            this.EnumerateInstanceExtensionProperties(layer, out VkExtensionProperties[] properties);

            this.instanceExtensionsMap[layer ?? ""] = extensions = properties.Select(x => Marshal.PtrToStringAnsi((nint)x.extensionName)!).ToHashSet();
        }

        return extensions.Contains(name);
    }

    public bool TryGetDeviceExtension<T>(VkPhysicalDevice physicalDevice, VkDevice device, string? layer, [NotNullWhen(true)] out T? extension) where T : class, IVkDeviceExtension
    {
        extension = null;

        if (this.HasDeviceExtension(physicalDevice, T.Name, layer))
        {
            extension = (T)T.Create(this, device);
        }

        return extension != null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGetDeviceExtension<T>(VkPhysicalDevice physicalDevice, VkDevice device, [NotNullWhen(true)] out T? extension) where T : class, IVkDeviceExtension =>
        this.TryGetDeviceExtension(physicalDevice, device, null, out extension);

    public bool TryGetInstanceExtension<T>(VkInstance instance, string? layer, [NotNullWhen(true)] out T? extension) where T : class, IVkInstanceExtension
    {
        extension = null;

        if (this.HasInstanceExtension(T.Name, layer))
        {
            extension = (T)T.Create(this, instance);
        }

        return extension != null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGetInstanceExtension<T>(VkInstance instance, [NotNullWhen(true)] out T? extension) where T : class, IVkInstanceExtension =>
        this.TryGetInstanceExtension(instance, null, out extension);
}
