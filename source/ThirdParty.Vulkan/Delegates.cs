using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkAllocationFunction.html">PFN_vkAllocationFunction</see>
/// </summary>
public unsafe delegate void* PFN_vkAllocationFunction(void* pUserData, size_t size, size_t alignment, VkSystemAllocationScope allocationScope);

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkDebugUtilsMessengerCallbackEXT.html">PFN_vkDebugUtilsMessengerCallbackEXT</see>
/// </summary>
public unsafe delegate VkBool32 PFN_vkDebugUtilsMessengerCallbackEXT(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData);

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkFreeFunction.html">PFN_vkFreeFunction</see>
/// </summary>
public unsafe delegate void* PFN_vkFreeFunction(void* pUserData, void* pMemory);

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkInternalAllocationNotification.html">PFN_vkInternalAllocationNotification</see>
/// </summary>
public unsafe delegate void* PFN_vkInternalAllocationNotification(void* pUserData, size_t size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope);

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkInternalFreeNotification.html">PFN_vkInternalFreeNotification</see>
/// </summary>
public unsafe delegate void* PFN_vkInternalFreeNotification(void* pUserData, ulong size, VkInternalAllocationType allocationType, VkSystemAllocationScope allocationScope);

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkReallocationFunction.html">PFN_vkReallocationFunction</see>
/// </summary>
public unsafe delegate void* PFN_vkReallocationFunction(void* pUserData, void* pOriginal,size_t size, size_t alignment, VkSystemAllocationScope allocationScope);
