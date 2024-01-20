using Age.Vulkan.Enums;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Structure containing parameters of a memory allocation.</para>
/// <para>The internal data of an allocated device memory object must include a reference to implementation-specific resources, referred to as the memory object’s payload. Applications can also import and export that internal data to and from device memory objects to share data between Vulkan instances and other compatible APIs. A VkMemoryAllocateInfo structure defines a memory import operation if its pNext chain includes one of the following structures:</para>
/// <list type="bullet">
/// <item><see cref="VkImportMemoryWin32HandleInfoKHR"/> with a non-zero handleType value</item>
/// <item><see cref="VkImportMemoryFdInfoKHR"/> with a non-zero handleType value</item>
/// <item><see cref="VkImportMemoryHostPointerInfoEXT"/> with a non-zero handleType value</item>
/// <item><see cref="VkImportAndroidHardwareBufferInfoANDROID"/> with a non-NULL buffer value</item>
/// <item><see cref="VkImportMemoryZirconHandleInfoFUCHSIA"/> with a non-zero handleType value</item>
/// <item><see cref="VkImportMemoryBufferCollectionFUCHSIA"/></item>
/// <item><see cref="VkImportScreenBufferInfoQNX"/> with a non-NULL buffer value</item>
/// </list>
/// <para>If the parameters define an import operation and the external handle type is <see cref="VK_EXTERNAL_MEMORY_HANDLE_TYPE_D3D11_TEXTURE_BIT"/>, VK_EXTERNAL_MEMORY_HANDLE_TYPE_D3D11_TEXTURE_KMT_BIT, or <see cref="VkExternalMemoryFeatureFlagBits.VK_EXTERNAL_MEMORY_HANDLE_TYPE_D3D12_RESOURCE_BIT"/>, allocationSize is ignored. The implementation must query the size of these allocations from the OS.</para>
/// <para>Whether device memory objects constructed via a memory import operation hold a reference to their payload depends on the properties of the handle type used to perform the import, as defined below for each valid handle type. Importing memory must not modify the content of the memory. Implementations must ensure that importing memory does not enable the importing Vulkan instance to access any memory or resources in other Vulkan instances other than that corresponding to the memory object imported. Implementations must also ensure accessing imported memory which has not been initialized does not allow the importing Vulkan instance to obtain data from the exporting Vulkan instance or vice-versa.</para>
/// <remarks>Note: How exported and imported memory is isolated is left to the implementation, but applications should be aware that such isolation may prevent implementations from placing multiple exportable memory objects in the same physical or virtual page. Hence, applications should avoid creating many small external memory objects whenever possible.</remarks>
/// <para>Importing memory must not increase overall heap usage within a system. However, it must affect the following per-process values:</para>
/// <list type="bullet">
/// <item><see cref="VkPhysicalDeviceMaintenance3Properties.maxMemoryAllocationCount"/></item>
/// <item><see cref="VkPhysicalDeviceMemoryBudgetPropertiesEXT.heapUsage"/></item>
/// </list>
/// <para>When performing a memory import operation, it is the responsibility of the application to ensure the external handles and their associated payloads meet all valid usage requirements. However, implementations must perform sufficient validation of external handles and payloads to ensure that the operation results in a valid memory object which will not cause program termination, device loss, queue stalls, or corruption of other resources when used as allowed according to its allocation parameters. If the external handle provided does not meet these requirements, the implementation must fail the memory import operation with the error code <see cref="VkResult.VK_ERROR_INVALID_EXTERNAL_HANDLE"/>. If the parameters define an export operation and the external handle type is <see cref="VkExternalMemoryFeatureFlagBits.VK_EXTERNAL_MEMORY_HANDLE_TYPE_ANDROID_HARDWARE_BUFFER_BIT_ANDROID"/>, implementations should not strictly follow memoryTypeIndex. Instead, they should modify the allocation internally to use the required memory type for the application’s given usage. This is because for an export operation, there is currently no way for the client to know the memory type index before allocating.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkMemoryAllocateInfo
{
    /// <summary>
    /// A <see cref="VkStructureType"/> value identifying this structure.
    /// </summary>
    public readonly VkStructureType sType;

    /// <summary>
    /// Null or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// The size of the allocation in bytes.
    /// </summary>
    public VkDeviceSize allocationSize;

    /// <summary>
    /// An index identifying a memory type from the memoryTypes array of the <see cref="VkPhysicalDeviceMemoryProperties"/> structure.
    /// </summary>
    public uint memoryTypeIndex;

    public VkMemoryAllocateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO;
}
