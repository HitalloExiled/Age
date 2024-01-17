using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Age.Core.Unsafe;
using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;
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
    /// The length of an array of VkMemoryHeap structures describing memory heaps, as returned in <see cref="VkPhysicalDeviceMemoryProperties.memoryHeaps"/>.
    /// </summary>
    public const uint VK_MAX_MEMORY_HEAPS = 16;

    /// <summary>
    /// The length of an array of VkMemoryType structures describing memory types, as returned in <see cref="VkPhysicalDeviceMemoryProperties.memoryTypes"/>.
    /// </summary>
    public const uint VK_MAX_MEMORY_TYPES = 32;

    /// <summary>
    /// The length in char values of an array containing a physical device name string, as returned in <see cref="VkPhysicalDeviceProperties.deviceName"/>.
    /// </summary>
    public const uint VK_MAX_PHYSICAL_DEVICE_NAME_SIZE = 256;

    /// <summary>
    /// The special queue family index VK_QUEUE_FAMILY_IGNORED indicates that a queue family parameter or member is ignored.
    /// </summary>
    public const uint VK_QUEUE_FAMILY_IGNORED = uint.MaxValue;

    /// <summary>
    /// Subpass index sentinel expanding synchronization scope outside a subpass
    /// </summary>
    public const uint VK_SUBPASS_EXTERNAL = uint.MaxValue;

    /// <summary>
    /// The length in byte values of an array containing a universally unique device or driver build identifier, as returned in <see cref="VkPhysicalDeviceIDProperties.deviceUUID"/> and <see cref="VkPhysicalDeviceIDProperties.driverUUID"/>.
    /// </summary>
    public const uint VK_UUID_SIZE = 16;

    public const string VK_LAYER_KHRONOS_VALIDATION = "VK_LAYER_KHRONOS_validation";

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkAllocateCommandBuffers(VkDevice device, VkCommandBufferAllocateInfo* pAllocateInfo, VkCommandBuffer* pCommandBuffers);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkAllocateDescriptorSets(VkDevice device, VkDescriptorSetAllocateInfo* pAllocateInfo, VkDescriptorSet* pDescriptorSets);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkAllocateMemory(VkDevice device, VkMemoryAllocateInfo* pAllocateInfo, VkAllocationCallbacks* pAllocator, VkDeviceMemory* pMemory);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkBeginCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferBeginInfo* pBeginInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkBindBufferMemory(VkDevice device, VkBuffer buffer, VkDeviceMemory memory, VkDeviceSize memoryOffset);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkBindImageMemory(VkDevice device, VkImage image, VkDeviceMemory memory, VkDeviceSize memoryOffset);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdBeginRenderPass(VkCommandBuffer commandBuffer, VkRenderPassBeginInfo* pRenderPassBegin, VkSubpassContents contents);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, VkDescriptorSet* pDescriptorSets, uint dynamicOffsetCount, uint* pDynamicOffsets);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdBindPipeline(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipeline pipeline);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdBindIndexBuffer(VkCommandBuffer commandBuffer, VkBuffer buffer, VkDeviceSize offset, VkIndexType indexType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, uint bindingCount, VkBuffer* pBuffers, VkDeviceSize* pOffsets);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdBlitImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, VkImageBlit* pRegions, VkFilter filter);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, VkClearColorValue* pColor, uint rangeCount, VkImageSubresourceRange* pRanges);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdCopyBuffer(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkBuffer dstBuffer, uint regionCount, VkBufferCopy* pRegions);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdCopyBufferToImage(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, VkBufferImageCopy* pRegions);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdDraw(VkCommandBuffer commandBuffer, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdDrawIndexed(VkCommandBuffer commandBuffer, uint indexCount, uint instanceCount, uint firstIndex, int vertexOffset, uint firstInstance);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdEndRenderPass(VkCommandBuffer commandBuffer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdSetScissor(VkCommandBuffer commandBuffer, uint firstScissor, uint scissorCount, VkRect2D* pScissors);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdSetViewport(VkCommandBuffer commandBuffer, uint firstViewport, uint viewportCount, VkViewport* pViewports);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateBuffer(VkDevice device, VkBufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkBuffer* pBuffer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateCommandPool(VkDevice device, VkCommandPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkCommandPool* pCommandPool);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateDescriptorPool(VkDevice device, VkDescriptorPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorPool* pDescriptorPool);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateDescriptorSetLayout(VkDevice device, VkDescriptorSetLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorSetLayout* pSetLayout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateDevice(VkPhysicalDevice physicalDevice, VkDeviceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDevice* pDevice);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateFence(VkDevice device, VkFenceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateFramebuffer(VkDevice device, VkFramebufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkFramebuffer* pFramebuffer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo* pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateImage(VkDevice device, VkImageCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkImage* pImage);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateImageView(VkDevice device, VkImageViewCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkImageView* pView);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreatePipelineLayout(VkDevice device, VkPipelineLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkPipelineLayout* pPipelineLayout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateRenderPass(VkDevice device, VkRenderPassCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkRenderPass* pRenderPass);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateSampler(VkDevice device, VkSamplerCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSampler* pSampler);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateSemaphore(VkDevice device, VkSemaphoreCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSemaphore* pSemaphore);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateShaderModule(VkDevice device, VkShaderModuleCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkShaderModule* pShaderModule);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyBuffer(VkDevice device, VkBuffer buffer, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyCommandPool(VkDevice device, VkCommandPool commandPool, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyDescriptorPool(VkDevice device, VkDescriptorPool descriptorPool, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyDescriptorSetLayout(VkDevice device, VkDescriptorSetLayout descriptorSetLayout, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyDevice(VkDevice device, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyFence(VkDevice device, VkFence fence, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyFramebuffer(VkDevice device, VkFramebuffer framebuffer, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyImage(VkDevice device, VkImage image, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyImageView(VkDevice device, VkImageView imageView, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyInstance(VkInstance instance, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyPipeline(VkDevice device, VkPipeline pipeline, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyPipelineLayout(VkDevice device, VkPipelineLayout pipelineLayout, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyRenderPass(VkDevice device, VkRenderPass renderPass, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroySampler(VkDevice device, VkSampler sampler, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroySemaphore(VkDevice device, VkSemaphore semaphore, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyShaderModule(VkDevice device, VkShaderModule shaderModule, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkDeviceWaitIdle(VkDevice device);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkEndCommandBuffer(VkCommandBuffer commandBuffer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkEnumerateInstanceExtensionProperties(byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkEnumerateInstanceLayerProperties(uint* pPropertyCount, VkLayerProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkEnumeratePhysicalDevices(VkInstance instance, uint* pPhysicalDeviceCount, VkPhysicalDevice* pPhysicalDevices);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkFreeCommandBuffers(VkDevice device, VkCommandPool commandPool, uint commandBufferCount, VkCommandBuffer* pCommandBuffers);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkFreeDescriptorSets(VkDevice device, VkDescriptorPool descriptorPool, uint descriptorSetCount, VkDescriptorSet* pDescriptorSets);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkFreeMemory(VkDevice device, VkDeviceMemory memory, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void* VkGetDeviceProcAddr(VkDevice device, byte* pName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkGetDeviceQueue(VkDevice device, uint queueFamilyIndex, uint queueIndex, VkQueue* pQueue);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void* VkGetInstanceProcAddr(VkInstance instance, byte* pName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkGetBufferMemoryRequirements(VkDevice device, VkBuffer buffer, VkMemoryRequirements* pMemoryRequirements);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkGetImageMemoryRequirements(VkDevice device, VkImage image, VkMemoryRequirements* pMemoryRequirements);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkGetPhysicalDeviceFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkFormatProperties* pFormatProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkGetPhysicalDeviceFeatures(VkPhysicalDevice physicalDevice, VkPhysicalDeviceFeatures* pFeatures);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkGetPhysicalDeviceMemoryProperties(VkPhysicalDevice physicalDevice, VkPhysicalDeviceMemoryProperties* pMemoryProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkGetPhysicalDeviceProperties(VkPhysicalDevice physicalDevice, VkPhysicalDeviceProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkGetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice physicalDevice, uint* pQueueFamilyPropertyCount, VkQueueFamilyProperties* pQueueFamilyProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkMapMemory(VkDevice device, VkDeviceMemory memory, VkDeviceSize offset, VkDeviceSize size, VkMemoryMapFlags flags, void** ppData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkQueueWaitIdle(VkQueue queue);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkQueueSubmit(VkQueue queue, uint submitCount, VkSubmitInfo* pSubmits, VkFence fence);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkResetCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferResetFlags flags);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkResetFences(VkDevice device, uint fenceCount, VkFence* pFences);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkUnmapMemory(VkDevice device, VkDeviceMemory memory);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkUpdateDescriptorSets(VkDevice device, uint descriptorWriteCount, VkWriteDescriptorSet* pDescriptorWrites, uint descriptorCopyCount, VkCopyDescriptorSet* pDescriptorCopies);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkWaitForFences(VkDevice device, uint fenceCount, VkFence* pFences, VkBool32 waitAll, ulong timeout);

    private readonly Dictionary<string, HashSet<string>> deviceExtensionsMap   = [];
    private readonly Dictionary<string, HashSet<string>> instanceExtensionsMap = [];

    private readonly VkAllocateCommandBuffers                 vkAllocateCommandBuffers                 = loader.Load<VkAllocateCommandBuffers>(nameof(vkAllocateCommandBuffers));
    private readonly VkAllocateDescriptorSets                 vkAllocateDescriptorSets                 = loader.Load<VkAllocateDescriptorSets>(nameof(vkAllocateDescriptorSets));
    private readonly VkAllocateMemory                         vkAllocateMemory                         = loader.Load<VkAllocateMemory>(nameof(vkAllocateMemory));
    private readonly VkBeginCommandBuffer                     vkBeginCommandBuffer                     = loader.Load<VkBeginCommandBuffer>(nameof(vkBeginCommandBuffer));
    private readonly VkBindBufferMemory                       vkBindBufferMemory                       = loader.Load<VkBindBufferMemory>(nameof(vkBindBufferMemory));
    private readonly VkBindImageMemory                        vkBindImageMemory                        = loader.Load<VkBindImageMemory>(nameof(vkBindImageMemory));
    private readonly VkCmdBeginRenderPass                     vkCmdBeginRenderPass                     = loader.Load<VkCmdBeginRenderPass>(nameof(vkCmdBeginRenderPass));
    private readonly VkCmdBindIndexBuffer                     vkCmdBindIndexBuffer                     = loader.Load<VkCmdBindIndexBuffer>(nameof(vkCmdBindIndexBuffer));
    private readonly VkCmdBindDescriptorSets                  vkCmdBindDescriptorSets                  = loader.Load<VkCmdBindDescriptorSets>(nameof(vkCmdBindDescriptorSets));
    private readonly VkCmdBindPipeline                        vkCmdBindPipeline                        = loader.Load<VkCmdBindPipeline>(nameof(vkCmdBindPipeline));
    private readonly VkCmdBindVertexBuffers                   vkCmdBindVertexBuffers                   = loader.Load<VkCmdBindVertexBuffers>(nameof(vkCmdBindVertexBuffers));
    private readonly VkCmdBlitImage                           vkCmdBlitImage                           = loader.Load<VkCmdBlitImage>(nameof(vkCmdBlitImage));
    private readonly VkCmdClearColorImage                     vkCmdClearColorImage                     = loader.Load<VkCmdClearColorImage>(nameof(vkCmdClearColorImage));
    private readonly VkCmdCopyBuffer                          vkCmdCopyBuffer                          = loader.Load<VkCmdCopyBuffer>(nameof(vkCmdCopyBuffer));
    private readonly VkCmdCopyBufferToImage                   vkCmdCopyBufferToImage                   = loader.Load<VkCmdCopyBufferToImage>(nameof(vkCmdCopyBufferToImage));
    private readonly VkCmdDraw                                vkCmdDraw                                = loader.Load<VkCmdDraw>(nameof(vkCmdDraw));
    private readonly VkCmdDrawIndexed                         vkCmdDrawIndexed                         = loader.Load<VkCmdDrawIndexed>(nameof(vkCmdDrawIndexed));
    private readonly VkCmdEndRenderPass                       vkCmdEndRenderPass                       = loader.Load<VkCmdEndRenderPass>(nameof(vkCmdEndRenderPass));
    private readonly VkCmdPipelineBarrier                     vkCmdPipelineBarrier                     = loader.Load<VkCmdPipelineBarrier>(nameof(vkCmdPipelineBarrier));
    private readonly VkCmdSetScissor                          vkCmdSetScissor                          = loader.Load<VkCmdSetScissor>(nameof(vkCmdSetScissor));
    private readonly VkCmdSetViewport                         vkCmdSetViewport                         = loader.Load<VkCmdSetViewport>(nameof(vkCmdSetViewport));
    private readonly VkCreateBuffer                           vkCreateBuffer                           = loader.Load<VkCreateBuffer>(nameof(vkCreateBuffer));
    private readonly VkCreateCommandPool                      vkCreateCommandPool                      = loader.Load<VkCreateCommandPool>(nameof(vkCreateCommandPool));
    private readonly VkCreateDescriptorPool                   vkCreateDescriptorPool                   = loader.Load<VkCreateDescriptorPool>(nameof(vkCreateDescriptorPool));
    private readonly VkCreateDescriptorSetLayout              vkCreateDescriptorSetLayout              = loader.Load<VkCreateDescriptorSetLayout>(nameof(vkCreateDescriptorSetLayout));
    private readonly VkCreateDevice                           vkCreateDevice                           = loader.Load<VkCreateDevice>(nameof(vkCreateDevice));
    private readonly VkCreateFence                            vkCreateFence                            = loader.Load<VkCreateFence>(nameof(vkCreateFence));
    private readonly VkCreateFramebuffer                      vkCreateFramebuffer                      = loader.Load<VkCreateFramebuffer>(nameof(vkCreateFramebuffer));
    private readonly VkCreateGraphicsPipelines                vkCreateGraphicsPipelines                = loader.Load<VkCreateGraphicsPipelines>(nameof(vkCreateGraphicsPipelines));
    private readonly VkCreateImage                            vkCreateImage                            = loader.Load<VkCreateImage>(nameof(vkCreateImage));
    private readonly VkCreateImageView                        vkCreateImageView                        = loader.Load<VkCreateImageView>(nameof(vkCreateImageView));
    private readonly VkCreateInstance                         vkCreateInstance                         = loader.Load<VkCreateInstance>(nameof(vkCreateInstance));
    private readonly VkCreatePipelineLayout                   vkCreatePipelineLayout                   = loader.Load<VkCreatePipelineLayout>(nameof(vkCreatePipelineLayout));
    private readonly VkCreateRenderPass                       vkCreateRenderPass                       = loader.Load<VkCreateRenderPass>(nameof(vkCreateRenderPass));
    private readonly VkCreateSampler                          vkCreateSampler                          = loader.Load<VkCreateSampler>(nameof(vkCreateSampler));
    private readonly VkCreateSemaphore                        vkCreateSemaphore                        = loader.Load<VkCreateSemaphore>(nameof(vkCreateSemaphore));
    private readonly VkCreateShaderModule                     vkCreateShaderModule                     = loader.Load<VkCreateShaderModule>(nameof(vkCreateShaderModule));
    private readonly VkDestroyBuffer                          vkDestroyBuffer                          = loader.Load<VkDestroyBuffer>(nameof(vkDestroyBuffer));
    private readonly VkDestroyCommandPool                     vkDestroyCommandPool                     = loader.Load<VkDestroyCommandPool>(nameof(vkDestroyCommandPool));
    private readonly VkDestroyDescriptorPool                  vkDestroyDescriptorPool                  = loader.Load<VkDestroyDescriptorPool>(nameof(vkDestroyDescriptorPool));
    private readonly VkDestroyDescriptorSetLayout             vkDestroyDescriptorSetLayout             = loader.Load<VkDestroyDescriptorSetLayout>(nameof(vkDestroyDescriptorSetLayout));
    private readonly VkDestroyDevice                          vkDestroyDevice                          = loader.Load<VkDestroyDevice>(nameof(vkDestroyDevice));
    private readonly VkDestroyFence                           vkDestroyFence                           = loader.Load<VkDestroyFence>(nameof(vkDestroyFence));
    private readonly VkDestroyFramebuffer                     vkDestroyFramebuffer                     = loader.Load<VkDestroyFramebuffer>(nameof(vkDestroyFramebuffer));
    private readonly VkDestroyImage                           vkDestroyImage                           = loader.Load<VkDestroyImage>(nameof(vkDestroyImage));
    private readonly VkDestroyImageView                       vkDestroyImageView                       = loader.Load<VkDestroyImageView>(nameof(vkDestroyImageView));
    private readonly VkDestroyInstance                        vkDestroyInstance                        = loader.Load<VkDestroyInstance>(nameof(vkDestroyInstance));
    private readonly VkDestroyPipeline                        vkDestroyPipeline                        = loader.Load<VkDestroyPipeline>(nameof(vkDestroyPipeline));
    private readonly VkDestroyPipelineLayout                  vkDestroyPipelineLayout                  = loader.Load<VkDestroyPipelineLayout>(nameof(vkDestroyPipelineLayout));
    private readonly VkDestroyRenderPass                      vkDestroyRenderPass                      = loader.Load<VkDestroyRenderPass>(nameof(vkDestroyRenderPass));
    private readonly VkDestroySampler                         vkDestroySampler                         = loader.Load<VkDestroySampler>(nameof(vkDestroySampler));
    private readonly VkDestroySemaphore                       vkDestroySemaphore                       = loader.Load<VkDestroySemaphore>(nameof(vkDestroySemaphore));
    private readonly VkDestroyShaderModule                    vkDestroyShaderModule                    = loader.Load<VkDestroyShaderModule>(nameof(vkDestroyShaderModule));
    private readonly VkDeviceWaitIdle                         vkDeviceWaitIdle                         = loader.Load<VkDeviceWaitIdle>(nameof(vkDeviceWaitIdle));
    private readonly VkEndCommandBuffer                       vkEndCommandBuffer                       = loader.Load<VkEndCommandBuffer>(nameof(vkEndCommandBuffer));
    private readonly VkEnumerateDeviceExtensionProperties     vkEnumerateDeviceExtensionProperties     = loader.Load<VkEnumerateDeviceExtensionProperties>(nameof(vkEnumerateDeviceExtensionProperties));
    private readonly VkEnumerateInstanceExtensionProperties   vkEnumerateInstanceExtensionProperties   = loader.Load<VkEnumerateInstanceExtensionProperties>(nameof(vkEnumerateInstanceExtensionProperties));
    private readonly VkEnumerateInstanceLayerProperties       vkEnumerateInstanceLayerProperties       = loader.Load<VkEnumerateInstanceLayerProperties>(nameof(vkEnumerateInstanceLayerProperties));
    private readonly VkEnumeratePhysicalDevices               vkEnumeratePhysicalDevices               = loader.Load<VkEnumeratePhysicalDevices>(nameof(vkEnumeratePhysicalDevices));
    private readonly VkFreeCommandBuffers                     vkFreeCommandBuffers                     = loader.Load<VkFreeCommandBuffers>(nameof(vkFreeCommandBuffers));
    private readonly VkFreeDescriptorSets                     vkFreeDescriptorSets                     = loader.Load<VkFreeDescriptorSets>(nameof(vkFreeDescriptorSets));
    private readonly VkFreeMemory                             vkFreeMemory                             = loader.Load<VkFreeMemory>(nameof(vkFreeMemory));
    private readonly VkGetBufferMemoryRequirements            vkGetBufferMemoryRequirements            = loader.Load<VkGetBufferMemoryRequirements>(nameof(vkGetBufferMemoryRequirements));
    private readonly VkGetDeviceProcAddr                      vkGetDeviceProcAddr                      = loader.Load<VkGetDeviceProcAddr>(nameof(vkGetDeviceProcAddr));
    private readonly VkGetDeviceQueue                         vkGetDeviceQueue                         = loader.Load<VkGetDeviceQueue>(nameof(vkGetDeviceQueue));
    private readonly VkGetImageMemoryRequirements             vkGetImageMemoryRequirements             = loader.Load<VkGetImageMemoryRequirements>(nameof(vkGetImageMemoryRequirements));
    private readonly VkGetInstanceProcAddr                    vkGetInstanceProcAddr                    = loader.Load<VkGetInstanceProcAddr>(nameof(vkGetInstanceProcAddr));
    private readonly VkGetPhysicalDeviceFeatures              vkGetPhysicalDeviceFeatures              = loader.Load<VkGetPhysicalDeviceFeatures>(nameof(vkGetPhysicalDeviceFeatures));
    private readonly VkGetPhysicalDeviceFormatProperties      vkGetPhysicalDeviceFormatProperties      = loader.Load<VkGetPhysicalDeviceFormatProperties>(nameof(vkGetPhysicalDeviceFormatProperties));
    private readonly VkGetPhysicalDeviceMemoryProperties      vkGetPhysicalDeviceMemoryProperties      = loader.Load<VkGetPhysicalDeviceMemoryProperties>(nameof(vkGetPhysicalDeviceMemoryProperties));
    private readonly VkGetPhysicalDeviceProperties            vkGetPhysicalDeviceProperties            = loader.Load<VkGetPhysicalDeviceProperties>(nameof(vkGetPhysicalDeviceProperties));
    private readonly VkGetPhysicalDeviceQueueFamilyProperties vkGetPhysicalDeviceQueueFamilyProperties = loader.Load<VkGetPhysicalDeviceQueueFamilyProperties>(nameof(vkGetPhysicalDeviceQueueFamilyProperties));
    private readonly VkMapMemory                              vkMapMemory                              = loader.Load<VkMapMemory>(nameof(vkMapMemory));
    private readonly VkQueueSubmit                            vkQueueSubmit                            = loader.Load<VkQueueSubmit>(nameof(vkQueueSubmit));
    private readonly VkQueueWaitIdle                          vkQueueWaitIdle                          = loader.Load<VkQueueWaitIdle>(nameof(vkQueueWaitIdle));
    private readonly VkResetCommandBuffer                     vkResetCommandBuffer                     = loader.Load<VkResetCommandBuffer>(nameof(vkResetCommandBuffer));
    private readonly VkResetFences                            vkResetFences                            = loader.Load<VkResetFences>(nameof(vkResetFences));
    private readonly VkUnmapMemory                            vkUnmapMemory                            = loader.Load<VkUnmapMemory>(nameof(vkUnmapMemory));
    private readonly VkUpdateDescriptorSets                   vkUpdateDescriptorSets                   = loader.Load<VkUpdateDescriptorSets>(nameof(vkUpdateDescriptorSets));
    private readonly VkWaitForFences                          vkWaitForFences                          = loader.Load<VkWaitForFences>(nameof(vkWaitForFences));

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
    /// <para>Allocate command buffers from an existing command pool</para>
    /// <para><see cref="Vk.AllocateCommandBuffers"/> can be used to allocate multiple command buffers. If the allocation of any of those command buffers fails, the implementation must free all successfully allocated command buffer objects from this command, set all entries of the pCommandBuffers array to NULL and return the error.</para>
    /// <para>Note: Filling pCommandBuffers with NULL values on failure is an exception to the default error behavior that output parameters will have undefined contents.</para>
    /// <para>When command buffers are first allocated, they are in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">initial state</see>.</para>
    /// </summary>
    /// <param name="device">The logical device that owns the command pool.</param>
    /// <param name="pAllocateInfo">A pointer to a <see cref="VkCommandBufferAllocateInfo"/> structure describing parameters of the allocation.</param>
    /// <param name="pCommandBuffers">A pointer to an array of <see cref="VkCommandBuffer"/> handles in which the resulting command buffer objects are returned. The array must be at least the length specified by the commandBufferCount member of pAllocateInfo. Each allocated command buffer begins in the initial state.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult AllocateCommandBuffers(VkDevice device, VkCommandBufferAllocateInfo* pAllocateInfo, VkCommandBuffer* pCommandBuffers) =>
        this.vkAllocateCommandBuffers.Invoke(device, pAllocateInfo, pCommandBuffers);

    public VkResult AllocateCommandBuffers(VkDevice device, in VkCommandBufferAllocateInfo allocateInfo, out VkCommandBuffer[] commandBuffers)
    {
        commandBuffers = new VkCommandBuffer[allocateInfo.commandBufferCount];

        fixed (VkCommandBufferAllocateInfo* pAllocateInfo = &allocateInfo)
        fixed (VkCommandBuffer* pCommandBuffers           = commandBuffers)
        {
            return this.vkAllocateCommandBuffers.Invoke(device, pAllocateInfo, pCommandBuffers);
        }
    }

    public VkResult AllocateCommandBuffers(VkDevice device, in VkCommandBufferAllocateInfo allocateInfo, out VkCommandBuffer commandBuffer)
    {
        fixed (VkCommandBufferAllocateInfo* pAllocateInfo   = &allocateInfo)
        fixed (VkCommandBuffer*             pCommandBuffers = &commandBuffer)
        {
            return this.vkAllocateCommandBuffers.Invoke(device, pAllocateInfo, pCommandBuffers);
        }
    }

    /// <summary>
    /// <para>Allocate one or more descriptor sets.</para>
    /// <para>The allocated descriptor sets are returned in pDescriptorSets.</para>
    /// <para>When a descriptor set is allocated, the initial state is largely uninitialized and all descriptors are undefined, with the exception that samplers with a non-null pImmutableSamplers are initialized on allocation. Descriptors also become undefined if the underlying resource or view object is destroyed. Descriptor sets containing undefined descriptors can still be bound and used, subject to the following conditions:</para>
    /// <list type="bullet">
    /// <item>For descriptor set bindings created with the <see cref="VkDescriptorBindingFlagBits.VK_DESCRIPTOR_BINDING_PARTIALLY_BOUND_BIT"/> bit set, all descriptors in that binding that are dynamically used must have been populated before the descriptor set is <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-binding">consumed</see>.</item>
    /// <item>For descriptor set bindings created without the <see cref="VkDescriptorBindingFlagBits.VK_DESCRIPTOR_BINDING_PARTIALLY_BOUND_BIT"/> bit set, all descriptors in that binding that are statically used must have been populated before the descriptor set is <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-binding">consumed</see>.</item>
    /// <item>Descriptor bindings with descriptor type of <see cref="VkDescriptorBindingFlagBits.VK_DESCRIPTOR_TYPE_INLINE_UNIFORM_BLOCK"/> can be undefined when the descriptor set is <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-binding">consumed</see>; though values in that block will be undefined.</item>
    /// <item>Entries that are not used by a pipeline can have undefined descriptors.</item>
    /// </list>
    /// <para>If a call to <see cref="AllocateDescriptorSets"/> would cause the total number of descriptor sets allocated from the pool to exceed the value of <see cref="VkDescriptorPoolCreateInfo.maxSets"/> used to create pAllocateInfo->descriptorPool, then the allocation may fail due to lack of space in the descriptor pool. Similarly, the allocation may fail due to lack of space if the call to <see cref="AllocateDescriptorSets"/> would cause the number of any given descriptor type to exceed the sum of all the descriptorCount members of each element of <see cref="VkDescriptorPoolCreateInfo.pPoolSizes"/> with a type equal to that type.</para>
    /// <para>Additionally, the allocation may also fail if a call to <see cref="AllocateDescriptorSets"/> would cause the total number of inline uniform block bindings allocated from the pool to exceed the value of <see cref="VkDescriptorPoolInlineUniformBlockCreateInfo.maxInlineUniformBlockBindings"/> used to create the descriptor pool.</para>
    /// <para>If the allocation fails due to no more space in the descriptor pool, and not because of system or device memory exhaustion, then <see cref="VkResult.VK_ERROR_OUT_OF_POOL_MEMORY"/> must be returned.</para>
    /// <para><see cref="AllocateDescriptorSets"/> can be used to create multiple descriptor sets. If the creation of any of those descriptor sets fails, then the implementation must destroy all successfully created descriptor set objects from this command, set all entries of the pDescriptorSets array to VK_NULL_HANDLE and return the error.</para>
    /// </summary>
    /// <param name="device">The logical device that owns the descriptor pool.</param>
    /// <param name="pAllocateInfo">A pointer to a <see cref="VkDescriptorSetAllocateInfo"/> structure describing parameters of the allocation.</param>
    /// <param name="pDescriptorSets">A pointer to an array of <see cref="VkDescriptorSet"/> handles in which the resulting descriptor set objects are returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult AllocateDescriptorSets(VkDevice device, VkDescriptorSetAllocateInfo* pAllocateInfo, VkDescriptorSet* pDescriptorSets) =>
        this.vkAllocateDescriptorSets.Invoke(device, pAllocateInfo, pDescriptorSets);

    public VkResult AllocateDescriptorSets(VkDevice device, in VkDescriptorSetAllocateInfo allocateInfo, out VkDescriptorSet[] descriptorSets)
    {
        descriptorSets = new VkDescriptorSet[allocateInfo.descriptorSetCount];

        fixed (VkDescriptorSetAllocateInfo* pAllocateInfo   = &allocateInfo)
        fixed (VkDescriptorSet*             pDescriptorSets = descriptorSets)
        {
            return this.vkAllocateDescriptorSets.Invoke(device, pAllocateInfo, pDescriptorSets);
        }
    }

    /// <summary>
    /// <para>Allocate device memory.</para>
    /// <para>Allocations returned by <see cref="AllocateMemory"/> are guaranteed to meet any alignment requirement of the implementation. For example, if an implementation requires 128 byte alignment for images and 64 byte alignment for buffers, the device memory returned through this mechanism would be 128-byte aligned. This ensures that applications can correctly suballocate objects of different types (with potentially different alignment requirements) in the same memory object.</para>
    /// <para>When memory is allocated, its contents are undefined with the following constraint:</para>
    /// <para>The contents of unprotected memory must not be a function of the contents of data protected memory objects, even if those memory objects were previously freed.</para>
    /// <remarks>Note: The contents of memory allocated by one application should not be a function of data from protected memory objects of another application, even if those memory objects were previously freed.</remarks>
    /// <para>The maximum number of valid memory allocations that can exist simultaneously within a <see cref="VkDevice"/> may be restricted by implementation- or platform-dependent limits. The maxMemoryAllocationCount feature describes the number of allocations that can exist simultaneously before encountering these internal limits.</para>
    /// <remarks>Note: For historical reasons, if maxMemoryAllocationCount is exceeded, some implementations may return <see cref="VkResult.VK_ERROR_TOO_MANY_OBJECTS"/>. Exceeding this limit will result in undefined behavior, and an application should not rely on the use of the returned error code in order to identify when the limit is reached.</remarks>
    /// <remarks>Note: Many protected memory implementations involve complex hardware and system software support, and often have additional and much lower limits on the number of simultaneous protected memory allocations (from memory types with the <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_PROTECTED_BIT"/> property) than for non-protected memory allocations. These limits can be system-wide, and depend on a variety of factors outside of the Vulkan implementation, so they cannot be queried in Vulkan. Applications should use as few allocations as possible from such memory types by suballocating aggressively, and be prepared for allocation failure even when there is apparently plenty of capacity remaining in the memory heap. As a guideline, the Vulkan conformance test suite requires that at least 80 minimum-size allocations can exist concurrently when no other uses of protected memory are active in the system.</remarks>
    /// <para>Some platforms may have a limit on the maximum size of a single allocation. For example, certain systems may fail to create allocations with a size greater than or equal to 4GB. Such a limit is implementation-dependent, and if such a failure occurs then the error <see cref="VkResult.VK_ERROR_OUT_OF_DEVICE_MEMORY"/> must be returned. This limit is advertised in <see cref="VkPhysicalDeviceMaintenance3Properties.maxMemoryAllocationSize"/>.</para>
    /// <para>The cumulative memory size allocated to a heap can be limited by the size of the specified heap. In such cases, allocated memory is tracked on a per-device and per-heap basis. Some platforms allow overallocation into other heaps. The overallocation behavior can be specified through the <see cref="VkAmdMemoryOverallocationBehavior"/> extension.</para>
    /// <para>If the <see cref="VkPhysicalDevicePageableDeviceLocalMemoryFeaturesEXT.pageableDeviceLocalMemory"/> feature is enabled, memory allocations made from a heap that includes <see cref="VkMemoryHeapFlagBits.VK_MEMORY_HEAP_DEVICE_LOCAL_BIT"/> in <see cref="VkMemoryHeap.flags"/> may be transparently moved to host-local memory allowing multiple applications to share device-local memory. If there is no space left in device-local memory when this new allocation is made, other allocations may be moved out transparently to make room. The operating system will determine which allocations to move to device-local memory or host-local memory based on platform-specific criteria. To help the operating system make good choices, the application should set the appropriate memory priority with <see cref="VkMemoryPriorityAllocateInfoEXT"/> and adjust it as necessary with <see cref="VkExtPageableDeviceLocalMemory.SetDeviceMemoryPriority"/>. Higher priority allocations will moved to device-local memory first.</para>
    /// <para>Memory allocations made on heaps without the <see cref="VkMemoryHeapFlagBits.VK_MEMORY_HEAP_DEVICE_LOCAL_BIT"/> property will not be transparently promoted to device-local memory by the operating system.</para>
    /// </summary>
    /// <param name="device">The logical device that owns the memory.</param>
    /// <param name="pAllocateInfo">A pointer to a <see cref="VkMemoryAllocateInfo"/> structure describing parameters of the allocation. A successfully returned allocation must use the requested parameters — no substitution is permitted by the implementation.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pMemory">A pointer to a VkDeviceMemory handle in which information about the allocated memory is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult AllocateMemory(VkDevice device, VkMemoryAllocateInfo* pAllocateInfo, VkAllocationCallbacks* pAllocator, VkDeviceMemory* pMemory) =>
        this.vkAllocateMemory.Invoke(device, pAllocateInfo, pAllocator, pMemory);

    public VkResult AllocateMemory(VkDevice device, in VkMemoryAllocateInfo allocateInfo, in VkAllocationCallbacks allocator, out VkDeviceMemory memory)
    {
        fixed (VkMemoryAllocateInfo*  pAllocateInfo = &allocateInfo)
        fixed (VkAllocationCallbacks* pAllocator    = &allocator)
        fixed (VkDeviceMemory*        pMemory       = &memory)
        {
            return this.vkAllocateMemory.Invoke(device, pAllocateInfo, NullIfDefault(allocator, pAllocator), pMemory);
        }
    }

    /// <summary>
    /// Start recording a command buffer.
    /// </summary>
    /// <param name="commandBuffer">The handle of the command buffer which is to be put in the recording state.</param>
    /// <param name="pBeginInfo">A pointer to a <see cref="VkCommandBufferBeginInfo"/> structure defining additional information about how the command buffer begins recording.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult BeginCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferBeginInfo* pBeginInfo) =>
        this.vkBeginCommandBuffer.Invoke(commandBuffer, pBeginInfo);

    public VkResult BeginCommandBuffer(VkCommandBuffer commandBuffer)
    {
        var commandBufferBeginInfo = new VkCommandBufferBeginInfo();

        return this.vkBeginCommandBuffer.Invoke(commandBuffer, &commandBufferBeginInfo);
    }

    public VkResult BeginCommandBuffer(VkCommandBuffer commandBuffer, in VkCommandBufferBeginInfo beginInfo)
    {
        fixed (VkCommandBufferBeginInfo* pBeginInfo = &beginInfo)
        {
            return this.vkBeginCommandBuffer.Invoke(commandBuffer, pBeginInfo);
        }
    }

    /// <summary>
    /// <para>Bind device memory to a buffer object.</para>
    /// <para><see cref="BindBufferMemory"/> is equivalent to passing the same parameters through <see cref="BindBufferMemoryInfo"/> to <see cref="BindBufferMemory2"/>.</para>
    /// </summary>
    /// <param name="device">The logical device that owns the buffer and memory.</param>
    /// <param name="buffer">The buffer to be attached to memory.</param>
    /// <param name="memory">A <see cref="VkDeviceMemory"/> object describing the device memory to attach.</param>
    /// <param name="memoryOffset">The start offset of the region of memory which is to be bound to the buffer. The number of bytes returned in the <see cref="VkMemoryRequirements.size"/> member in memory, starting from memoryOffset bytes, will be bound to the specified buffer.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult BindBufferMemory(VkDevice device, VkBuffer buffer, VkDeviceMemory memory, VkDeviceSize memoryOffset) =>
        this.vkBindBufferMemory.Invoke(device, buffer, memory, memoryOffset);

    /// <summary>
    /// <para>Bind device memory to an image object.</para>
    /// <para><see cref="BindImageMemory"/> is equivalent to passing the same parameters through <see cref="VkBindImageMemoryInfo"/> to <see cref="BindImageMemory2"/>.</para>
    /// </summary>
    /// <param name="device">The logical device that owns the image and memory.</param>
    /// <param name="image">The image.</param>
    /// <param name="memory">The <see cref="VkDeviceMemory"/> object describing the device memory to attach.</param>
    /// <param name="memoryOffset">The start offset of the region of memory which is to be bound to the image. The number of bytes returned in the <see cref="VkMemoryRequirements.size"/> member in memory, starting from memoryOffset bytes, will be bound to the specified image.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult BindImageMemory(VkDevice device, VkImage image, VkDeviceMemory memory, VkDeviceSize memoryOffset) =>
        this.vkBindImageMemory.Invoke(device, image, memory, memoryOffset);

    /// <summary>
    /// Begin a new render pass.
    /// </summary>
    /// <param name="commandBuffer">The command buffer in which to record the command.</param>
    /// <param name="pRenderPassBegin">A pointer to a <see cref="VkRenderPassBeginInfo"/> structure specifying the render pass to begin an instance of, and the framebuffer the instance uses.</param>
    /// <param name="contents">A <see cref="VkSubpassContents"/> value specifying how the commands in the first subpass will be provided.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdBeginRenderPass(VkCommandBuffer commandBuffer, VkRenderPassBeginInfo* pRenderPassBegin, VkSubpassContents contents) =>
        this.vkCmdBeginRenderPass.Invoke(commandBuffer, pRenderPassBegin, contents);

    public void CmdBeginRenderPass(VkCommandBuffer commandBuffer, in VkRenderPassBeginInfo renderPassBegin, VkSubpassContents contents)
    {
        fixed (VkRenderPassBeginInfo* pRenderPassBegin = &renderPassBegin)
        {
            this.vkCmdBeginRenderPass.Invoke(commandBuffer, pRenderPassBegin, contents);
        }
    }

    /// <summary>
    /// Binds descriptor sets to a command buffer.
    /// <see cref="CmdBindDescriptorSets"/> binds descriptor sets pDescriptorSets[0..descriptorSetCount-1] to set numbers [firstSet..firstSet+descriptorSetCount-1] for subsequent <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#pipelines-bindpoint-commands">bound pipeline commands</see> set by pipelineBindPoint. Any bindings that were previously applied via these sets , or calls to <see cref="VkExtDescriptorBuffer.CmdSetDescriptorBufferOffsets"/> or <see cref="VkExtDescriptorBuffer.CmdBindDescriptorBufferEmbeddedSamplers"/>, are no longer valid.
    /// Once bound, a descriptor set affects rendering of subsequent commands that interact with the given pipeline type in the command buffer until either a different set is bound to the same set number, or the set is disturbed as described in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-compatibility">Pipeline Layout Compatibility</see>.
    /// A compatible descriptor set must be bound for all set numbers that any shaders in a pipeline access, at the time that a drawing or dispatching command is recorded to execute using that pipeline. However, if none of the shaders in a pipeline statically use any bindings with a particular set number, then no descriptor set need be bound for that set number, even if the pipeline layout includes a non-trivial descriptor set layout for that set number.
    /// When consuming a descriptor, a descriptor is considered valid if the descriptor is not undefined as described by <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptor-set-initial-state">descriptor set allocation</see>. If the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-nullDescriptor">nullDescriptor</see> feature is enabled, a null descriptor is also considered valid. A descriptor that was disturbed by <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-compatibility">Pipeline Layout Compatibility</see>, or was never bound by <see cref="CmdBindDescriptorSets"/> is not considered valid. If a pipeline accesses a descriptor either statically or dynamically depending on the <see cref="VkDescriptorBindingFlagBits"/>, the consuming descriptor type in the pipeline must match the <see cref="VkDescriptorType"/> in <see cref="VkDescriptorSetLayoutCreateInfo"/> for the descriptor to be considered valid. If a descriptor is a mutable descriptor, the consuming descriptor type in the pipeline must match the active descriptor type for the descriptor to be considered valid.
    /// Note: Further validation may be carried out beyond validation for descriptor types, e.g. <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#textures-input-validation">Texel Input Validation</see>.
    /// If any of the sets being bound include dynamic uniform or storage buffers, then pDynamicOffsets includes one element for each array element in each dynamic descriptor type binding in each set. Values are taken from pDynamicOffsets in an order such that all entries for set N come before set N+1; within a set, entries are ordered by the binding numbers in the descriptor set layouts; and within a binding array, elements are in order. dynamicOffsetCount must equal the total number of dynamic descriptors in the sets being bound.
    /// The effective offset used for dynamic uniform and storage buffer bindings is the sum of the relative offset taken from pDynamicOffsets, and the base address of the buffer plus base offset in the descriptor set. The range of the dynamic uniform and storage buffer bindings is the buffer range as specified in the descriptor set.
    /// Each of the pDescriptorSets must be compatible with the pipeline layout specified by layout. The layout used to program the bindings must also be compatible with the pipeline used in subsequent bound pipeline commands with that pipeline type, as defined in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#descriptorsets-compatibility">Pipeline Layout Compatibility</see> section.
    /// The descriptor set contents bound by a call to <see cref="CmdBindDescriptorSets"/> may be consumed at the following times:
    /// For descriptor bindings created with the <see cref="VkDescriptorBindingFlagBits.VK_DESCRIPTOR_BINDING_UPDATE_AFTER_BIND_BIT"/> bit set, the contents may be consumed when the command buffer is submitted to a queue, or during shader execution of the resulting draws and dispatches, or any time in between. Otherwise,
    /// during host execution of the command, or during shader execution of the resulting draws and dispatches, or any time in between.
    /// Thus, the contents of a descriptor set binding must not be altered (overwritten by an update command, or freed) between the first point in time that it may be consumed, and when the command completes executing on the queue.
    /// The contents of pDynamicOffsets are consumed immediately during execution of <see cref="CmdBindDescriptorSets"/>. Once all pending uses have completed, it is legal to update and reuse a descriptor set.
    /// </summary>
    /// <param name="commandBuffer">The command buffer that the descriptor sets will be bound to.</param>
    /// <param name="pipelineBindPoint">A VkPipelineBindPoint indicating the type of the pipeline that will use the descriptors. There is a separate set of bind points for each pipeline type, so binding one does not disturb the others.</param>
    /// <param name="layout">A VkPipelineLayout object used to program the bindings.</param>
    /// <param name="firstSet">The set number of the first descriptor set to be bound.</param>
    /// <param name="descriptorSetCount">The number of elements in the pDescriptorSets array.</param>
    /// <param name="pDescriptorSets">A pointer to an array of handles to VkDescriptorSet objects describing the descriptor sets to bind to.</param>
    /// <param name="dynamicOffsetCount">The number of dynamic offsets in the pDynamicOffsets array.</param>
    /// <param name="pDynamicOffsets">A pointer to an array of uint32_t values specifying dynamic offsets.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, VkDescriptorSet* pDescriptorSets, uint dynamicOffsetCount, uint* pDynamicOffsets) =>
        this.vkCmdBindDescriptorSets.Invoke(commandBuffer, pipelineBindPoint, layout, firstSet, descriptorSetCount, pDescriptorSets, dynamicOffsetCount, pDynamicOffsets);

    public void CmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, VkDescriptorSet[]? descriptorSets, uint[]? dynamicOffsets)
    {
        fixed (VkDescriptorSet* pDescriptorSets = descriptorSets)
        fixed (uint*            pDynamicOffsets = dynamicOffsets)
        {
            this.vkCmdBindDescriptorSets.Invoke(commandBuffer, pipelineBindPoint, layout, firstSet, (uint)(descriptorSets?.Length ?? 0), pDescriptorSets, (uint)(dynamicOffsets?.Length ?? 0), pDynamicOffsets);
        }
    }

    /// <summary>
    /// <para>Bind a pipeline object to a command buffer.</para>
    /// <para>Once bound, a pipeline binding affects subsequent commands that interact with the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#shaders-binding">given pipeline</see> type in the command buffer until a different pipeline of the same type is bound to the bind point, or until the pipeline bind point is disturbed by binding a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#shaders-objects">shader object</see> as described in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#shaders-objects-pipeline-interaction">Interaction with Pipelines</see>. Commands that do not interact with the given pipeline type must not be affected by the pipeline state.</para>
    /// </summary>
    /// <param name="commandBuffer">The command buffer that the pipeline will be bound to.</param>
    /// <param name="pipelineBindPoint">A VkPipelineBindPoint value specifying to which bind point the pipeline is bound. Binding one does not disturb the others.</param>
    /// <param name="pipeline">The pipeline to be bound.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdBindPipeline(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipeline pipeline) =>
        this.vkCmdBindPipeline.Invoke(commandBuffer, pipelineBindPoint, pipeline);

    /// <summary>
    /// Bind an index buffer to a command buffer.
    /// </summary>
    /// <param name="commandBuffer">The command buffer into which the command is recorded.</param>
    /// <param name="buffer">The buffer being bound.</param>
    /// <param name="offset">The starting offset in bytes within buffer used in index buffer address calculations.</param>
    /// <param name="indexType">A <see cref="VkIndexType"/> value specifying the size of the indices.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdBindIndexBuffer(VkCommandBuffer commandBuffer, VkBuffer buffer, VkDeviceSize offset, VkIndexType indexType) =>
        this.vkCmdBindIndexBuffer.Invoke(commandBuffer, buffer, offset, indexType);

    /// <summary>
    /// Bind vertex buffers to a command buffer.
    /// The values taken from elements i of pBuffers and pOffsets replace the current state for the vertex input binding firstBinding + i, for i in [0, bindingCount). The vertex input binding is updated to start at the offset indicated by pOffsets[i] from the start of the buffer pBuffers[i]. All vertex input attributes that use each of these bindings will use these updated addresses in their address calculations for subsequent drawing commands. If the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#features-nullDescriptor">nullDescriptor</see> feature is enabled, elements of pBuffers can be <see cref="VK_NULL_HANDLE"/>, and can be used by the vertex shader. If a vertex input attribute is bound to a vertex input binding that is <see cref="VK_NULL_HANDLE"/>, the values taken from memory are considered to be zero, and missing G, B, or A components are <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdBindVertexBuffers.html#fxvertex-input-extraction">filled with (0,0,1)</see>.
    /// </summary>
    /// <param name="commandBuffer">The command buffer into which the command is recorded.</param>
    /// <param name="firstBinding">The index of the first vertex input binding whose state is updated by the command.</param>
    /// <param name="bindingCount">The number of vertex input bindings whose state is updated by the command.</param>
    /// <param name="pBuffers">A pointer to an array of buffer handles.</param>
    /// <param name="pOffsets">A pointer to an array of buffer offsets.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, uint bindingCount, VkBuffer* pBuffers, VkDeviceSize* pOffsets) =>
        this.vkCmdBindVertexBuffers.Invoke(commandBuffer, firstBinding, bindingCount, pBuffers, pOffsets);

    public void CmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, VkBuffer[] buffers, VkDeviceSize[] offsets)
    {
        fixed (VkBuffer*     pBuffers = buffers)
        fixed (VkDeviceSize* pOffsets = offsets)
        {
            this.vkCmdBindVertexBuffers.Invoke(commandBuffer, firstBinding, (uint)buffers.Length, pBuffers, pOffsets);
        }
    }

    /// <summary>
    /// <para><see cref="CmdBlitImage"/> must not be used for multisampled source or destination images. Use <see cref="CmdResolveImage"/> for this purpose.</para>
    /// <para>As the sizes of the source and destination extents can differ in any dimension, texels in the source extent are scaled and filtered to the destination extent. Scaling occurs via the following operations:</para>
    /// <list type="bullet">
    /// <item>
    /// <para>For each destination texel, the integer coordinate of that texel is converted to an unnormalized texture coordinate, using the effective inverse of the equations described in unnormalized to integer conversion:</para>
    /// <para>ubase = i + ½</para>
    /// <para>vbase = j + ½</para>
    /// <para>wbase = k + ½</para>
    /// </item>
    /// <item>
    /// <para>These base coordinates are then offset by the first destination offset:</para>
    /// <para>uoffset = ubase - xdst0</para>
    /// <para>voffset = vbase - ydst0</para>
    /// <para>woffset = wbase - zdst0</para>
    /// <para>aoffset = a - baseArrayCountdst</para>
    /// </item>
    /// <item>
    /// <para>The scale is determined from the source and destination regions, and applied to the offset coordinates:</para>
    /// <para>scaleu = (xsrc1 - xsrc0) / (xdst1 - xdst0)</para>
    /// <para>scalev = (ysrc1 - ysrc0) / (ydst1 - ydst0)</para>
    /// <para>scalew = (zsrc1 - zsrc0) / (zdst1 - zdst0)</para>
    /// <para>uscaled = uoffset × scaleu</para>
    /// <para>vscaled = voffset × scalev</para>
    /// <para>wscaled = woffset × scalew</para>
    /// </item>
    /// <item>
    /// <para>Finally the source offset is added to the scaled coordinates, to determine the final unnormalized coordinates used to sample from srcImage:</para>
    /// <para>u = uscaled + xsrc0</para>
    /// <para>v = vscaled + ysrc0</para>
    /// <para>w = wscaled + zsrc0</para>
    /// <para>q = mipLevel</para>
    /// <para>a = aoffset + baseArrayCountsrc</para>
    /// </item>
    /// </list>
    /// <para>These coordinates are used to sample from the source image, as described in Image Operations chapter, with the filter mode equal to that of filter, a mipmap mode of <see cref="VkSamplerMipmapMode.VK_SAMPLER_MIPMAP_MODE_NEAREST"/> and an address mode of <see cref="VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_EDGE"/>. Implementations must clamp at the edge of the source image, and may additionally clamp to the edge of the source region.</para>
    /// <remarks>Note: Due to allowable rounding errors in the generation of the source texture coordinates, it is not always possible to guarantee exactly which source texels will be sampled for a given blit. As rounding errors are implementation-dependent, the exact results of a blitting operation are also implementation-dependent.</remarks>
    /// <para>Blits are done layer by layer starting with the baseArrayLayer member of srcSubresource for the source and dstSubresource for the destination. layerCount layers are blitted to the destination image.</para>
    /// <para>When blitting 3D textures, slices in the destination region bounded by dstOffsets[0].z and dstOffsets[1].z are sampled from slices in the source region bounded by srcOffsets[0].z and srcOffsets[1].z. If the filter parameter is <see cref="VkFilter.VK_FILTER_LINEAR"/> then the value sampled from the source image is taken by doing linear filtering using the interpolated z coordinate represented by w in the previous equations. If the filter parameter is <see cref="VK_FILTER_NEAREST"/> then the value sampled from the source image is taken from the single nearest slice, with an implementation-dependent arithmetic rounding mode.</para>
    /// <para>The following filtering and conversion rules apply:</para>
    /// <list type="bullet">
    /// <item>Integer formats can only be converted to other integer formats with the same signedness.</item>
    /// <item>No format conversion is supported between depth/stencil images. The formats must match.</item>
    /// <item>Format conversions on unorm, snorm, scaled and packed float formats of the copied aspect of the image are performed by first converting the pixels to float values.</item>
    /// <item>For sRGB source formats, nonlinear RGB values are converted to linear representation prior to filtering.</item>
    /// <item>After filtering, the float values are first clamped and then cast to the destination image format. In case of sRGB destination format, linear RGB values are converted to nonlinear representation before writing the pixel to the image.</item>
    /// </list>
    /// <para>Signed and unsigned integers are converted by first clamping to the representable range of the destination format, then casting the value.</para>
    /// </summary>
    /// <param name="commandBuffer">The command buffer into which the command will be recorded.</param>
    /// <param name="srcImage">The source image.</param>
    /// <param name="srcImageLayout">The layout of the source image subresources for the blit.</param>
    /// <param name="dstImage">The destination image.</param>
    /// <param name="dstImageLayout">The layout of the destination image subresources for the blit.</param>
    /// <param name="regionCount">The number of regions to blit.</param>
    /// <param name="pRegions">A pointer to an array of <see cref="VkImageBlit"/> structures specifying the regions to blit.</param>
    /// <param name="filter">A <see cref="VkFilter"/> specifying the filter to apply if the blits require scaling.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdBlitImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, VkImageBlit* pRegions, VkFilter filter) =>
        this.vkCmdBlitImage.Invoke(commandBuffer, srcImage, srcImageLayout, dstImage, dstImageLayout, regionCount, pRegions, filter);

    public void CmdBlitImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, VkImageBlit[] regions, VkFilter filter)
    {
        fixed (VkImageBlit* pRegions = regions)
        {
            this.vkCmdBlitImage.Invoke(commandBuffer, srcImage, srcImageLayout, dstImage, dstImageLayout, (uint)regions.Length, pRegions, filter);
        }
    }

    /// <summary>
    /// <para>Copy data between buffer regions.</para>
    /// <para>Each source region specified by pRegions is copied from the source buffer to the destination region of the destination buffer. If any of the specified regions in srcBuffer overlaps in memory with any of the specified regions in dstBuffer, values read from those overlapping regions are undefined.</para>
    /// </summary>
    /// <param name="commandBuffer">The command buffer into which the command will be recorded.</param>
    /// <param name="srcBuffer">The source buffer.</param>
    /// <param name="dstBuffer">The destination buffer.</param>
    /// <param name="regionCount">The number of regions to copy.</param>
    /// <param name="pRegions">A pointer to an array of <see cref="VkBufferCopy"/> structures specifying the regions to copy.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdCopyBuffer(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkBuffer dstBuffer, uint regionCount, VkBufferCopy* pRegions) =>
        this.vkCmdCopyBuffer.Invoke(commandBuffer, srcBuffer, dstBuffer, regionCount, pRegions);

    public void CmdCopyBuffer(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkBuffer dstBuffer, in VkBufferCopy region)
    {
        fixed (VkBufferCopy* pRegions = &region)
        {
            this.vkCmdCopyBuffer.Invoke(commandBuffer, srcBuffer, dstBuffer, 1, pRegions);
        }
    }

    public void CmdCopyBuffer(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkBuffer dstBuffer, VkBufferCopy[] regions)
    {
        fixed (VkBufferCopy* pRegions = regions)
        {
            this.vkCmdCopyBuffer.Invoke(commandBuffer, srcBuffer, dstBuffer, (uint)regions.Length, pRegions);
        }
    }

    /// <summary>
    /// Each specified range in pRanges is cleared to the value specified by pColor.
    /// </summary>
    /// <param name="commandBuffer">The command buffer into which the command will be recorded.</param>
    /// <param name="image">The image to be cleared.</param>
    /// <param name="imageLayout">Specifies the current layout of the image subresource ranges to be cleared, and must be VK_IMAGE_LAYOUT_SHARED_PRESENT_KHR, VK_IMAGE_LAYOUT_GENERAL or VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL.</param>
    /// <param name="pColor">A pointer to a VkClearColorValue structure containing the values that the image subresource ranges will be cleared to (see https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#clears-values below).</param>
    /// <param name="rangeCount">The number of image subresource range structures in pRanges.</param>
    /// <param name="pRanges">A pointer to an array of <see cref="VkImageSubresourceRange"/> structures describing a range of mipmap levels, array layers, and aspects to be cleared, as described in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#resources-image-views">Image Views</see>.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, VkClearColorValue* pColor, uint rangeCount, VkImageSubresourceRange* pRanges) =>
        this.vkCmdClearColorImage.Invoke(commandBuffer, image, imageLayout, pColor, rangeCount, pRanges);

    public void CmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, in VkClearColorValue color, in VkImageSubresourceRange range)
    {
        fixed (VkClearColorValue* pColor = &color)
        fixed (VkImageSubresourceRange* pRanges = &range)
        {
            this.vkCmdClearColorImage.Invoke(commandBuffer, image, imageLayout, pColor, 1, pRanges);
        }
    }

    public void CmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, in VkClearColorValue color, VkImageSubresourceRange[] ranges)
    {
        fixed (VkClearColorValue* pColor = &color)
        fixed (VkImageSubresourceRange* pRanges = ranges)
        {
            this.vkCmdClearColorImage.Invoke(commandBuffer, image, imageLayout, pColor, (uint)ranges.Length, pRanges);
        }
    }

    /// <summary>
    /// <para>Copy data from a buffer into an image.</para>
    /// <para>Each source region specified by pRegions is copied from the source buffer to the destination region of the destination image according to the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#copies-buffers-images-addressing">addressing calculations</see> for each resource. If any of the specified regions in srcBuffer overlaps in memory with any of the specified regions in dstImage, values read from those overlapping regions are undefined. If any region accesses a depth aspect in dstImage and the <see cref="VkExtDepthRangeUnrestricted"/> extension is not enabled, values copied from srcBuffer outside of the range [0,1] will be be written as undefined values to the destination image.</para>
    /// <para>Copy regions for the image must be aligned to a multiple of the texel block extent in each dimension, except at the edges of the image, where region extents must match the edge of the image.</para>
    /// </summary>
    /// <param name="commandBuffer">The command buffer into which the command will be recorded.</param>
    /// <param name="srcBuffer">The source buffer.</param>
    /// <param name="dstImage">The destination image.</param>
    /// <param name="dstImageLayout">The layout of the destination image subresources for the copy.</param>
    /// <param name="regionCount">The number of regions to copy.</param>
    /// <param name="pRegions">A pointer to an array of <see cref="VkBufferImageCopy"/> structures specifying the regions to copy.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdCopyBufferToImage(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, VkBufferImageCopy* pRegions) =>
        this.vkCmdCopyBufferToImage.Invoke(commandBuffer, srcBuffer, dstImage, dstImageLayout, regionCount, pRegions);

    public void CmdCopyBufferToImage(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkImage dstImage, VkImageLayout dstImageLayout, in VkBufferImageCopy region)
    {
        fixed (VkBufferImageCopy* pRegions = &region)
        {
            this.vkCmdCopyBufferToImage.Invoke(commandBuffer, srcBuffer, dstImage, dstImageLayout, 1, pRegions);
        }
    }

    public void CmdCopyBufferToImage(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkImage dstImage, VkImageLayout dstImageLayout, VkBufferImageCopy[] regions)
    {
        fixed (VkBufferImageCopy* pRegions = regions)
        {
            this.vkCmdCopyBufferToImage.Invoke(commandBuffer, srcBuffer, dstImage, dstImageLayout, (uint)regions.Length, pRegions);
        }
    }

    /// <summary>
    /// <para>Draw primitives.</para>
    /// <para>When the command is executed, primitives are assembled using the current primitive topology and vertexCount consecutive vertex indices with the first vertexIndex value equal to firstVertex. The primitives are drawn instanceCount times with instanceIndex starting with firstInstance and increasing sequentially for each instance. The assembled primitives execute the bound graphics pipeline.</para>
    /// </summary>
    /// <param name="commandBuffer">The command buffer into which the command is recorded.</param>
    /// <param name="vertexCount">The number of vertices to draw.</param>
    /// <param name="instanceCount">The number of instances to draw.</param>
    /// <param name="firstVertex">The index of the first vertex to draw.</param>
    /// <param name="firstInstance">The instance ID of the first instance to draw.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdDraw(VkCommandBuffer commandBuffer, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance) =>
        this.vkCmdDraw.Invoke(commandBuffer, vertexCount, instanceCount, firstVertex, firstInstance);

    public void CmdDrawIndexed(VkCommandBuffer commandBuffer, uint indexCount, uint instanceCount, uint firstIndex, int vertexOffset, uint firstInstance) =>
        this.vkCmdDrawIndexed.Invoke(commandBuffer, indexCount, instanceCount, firstIndex, vertexOffset, firstInstance);

    /// <summary>
    /// <para>End the current render pass.</para>
    /// <para>Ending a render pass instance performs any multisample resolve operations on the final subpass.</para>
    /// </summary>
    /// <param name="commandBuffer">The command buffer in which to end the current render pass instance.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdEndRenderPass(VkCommandBuffer commandBuffer) =>
        this.vkCmdEndRenderPass.Invoke(commandBuffer);

    /// <summary>
    /// <para>Insert a memory dependency.</para>
    /// <para><see cref="CmdPipelineBarrier"/> operates almost identically to <see cref="CmdPipelineBarrier2"/>, except that the scopes and barriers are defined as direct parameters rather than being defined by an <see cref="VkDependencyInfo"/>.</para>
    /// <para>When <see cref="CmdPipelineBarrier"/> is submitted to a queue, it defines a memory dependency between commands that were submitted to the same queue before it, and those submitted to the same queue after it.</para>
    /// <para>If <see cref="CmdPipelineBarrier"/> was recorded outside a render pass instance, the first <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-scopes">synchronization scope</see> includes all commands that occur earlier in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-submission-order">submission order</see>. If <see cref="CmdPipelineBarrier"/> was recorded inside a render pass instance, the first <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-scopes">synchronization scope</see> includes only commands that occur earlier in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-submission-order">submission order</see> within the same subpass. In either case, the first <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-scopes">synchronization scope</see> is limited to operations on the pipeline stages determined by the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-pipeline-stages-masks">source stage mask</see> specified by srcStageMask.</para>
    /// <para>If <see cref="CmdPipelineBarrier"/> was recorded outside a render pass instance, the second <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-scopes">synchronization scope</see> includes all commands that occur later in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-submission-order">submission order</see>. If <see cref="CmdPipelineBarrier"/> was recorded inside a render pass instance, the second <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-scopes">synchronization scope</see> includes only commands that occur later in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-submission-order">submission order</see> within the same subpass. In either case, the second <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-dependencies-scopes">synchronization scope</see> is limited to operations on the pipeline stages determined by the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-pipeline-stages-masks">destination stage mask</see> specified by dstStageMask.</para>
    /// <para>The first access scope is limited to accesses in the pipeline stages determined by the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-pipeline-stages-masks">source stage mask</see> specified by srcStageMask. Within that, the first access scope only includes the first access scopes defined by elements of the pMemoryBarriers, pBufferMemoryBarriers and pImageMemoryBarriers arrays, which each define a set of <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-memory-barriers">memory barriers</see>. If no <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-memory-barriers">memory barriers</see> are specified, then the first access scope includes no accesses.</para>
    /// <para>The second access scope is limited to accesses in the pipeline stages determined by the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-pipeline-stages-masks">destination stage mask</see> specified by dstStageMask. Within that, the second access scope only includes the second access scopes defined by elements of the pMemoryBarriers, pBufferMemoryBarriers and pImageMemoryBarriers arrays, which each define a set of <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-memory-barriers">memory barriers</see>. If no <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-memory-barriers">memory barriers</see> are specified, then the second access scope includes no accesses.</para>
    /// <para>If dependencyFlags includes <see cref="VkDependencyFlagBits.VK_DEPENDENCY_BY_REGION_BIT"/>, then any dependency between <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-framebuffer-regions">framebuffer-space</see> pipeline stages is <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-framebuffer-regions">framebuffer-local</see> - otherwise it is <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-framebuffer-regions">framebuffer-global</see>.</para>
    /// </summary>
    /// <param name="commandBuffer">The command buffer into which the command is recorded.</param>
    /// <param name="srcStageMask">A bitmask of <see cref="VkPipelineStageFlagBits"/> specifying the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-pipeline-stages-masks">source stages</see>.</param>
    /// <param name="dstStageMask">A bitmask of <see cref="VkPipelineStageFlagBits"/> specifying the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-pipeline-stages-masks">destination stages</see>.</param>
    /// <param name="dependencyFlags">A bitmask of <see cref="VkDependencyFlagBits"/> specifying how execution and memory dependencies are formed.</param>
    /// <param name="memoryBarrierCount">The length of the pMemoryBarriers array.</param>
    /// <param name="pMemoryBarriers">A pointer to an array of <see cref="VkMemoryBarrier"/> structures.</param>
    /// <param name="bufferMemoryBarrierCount">The length of the pBufferMemoryBarriers array.</param>
    /// <param name="pBufferMemoryBarriers">A pointer to an array of <see cref="VkBufferMemoryBarrier"/> structures.</param>
    /// <param name="imageMemoryBarrierCount">The length of the pImageMemoryBarriers array.</param>
    /// <param name="pImageMemoryBarriers">A pointer to an array of <see cref="VkImageMemoryBarrier"/> structures.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers) =>
        this.vkCmdPipelineBarrier.Invoke(commandBuffer, srcStageMask, dstStageMask, dependencyFlags, memoryBarrierCount, pMemoryBarriers, bufferMemoryBarrierCount, pBufferMemoryBarriers, imageMemoryBarrierCount, pImageMemoryBarriers);

    public void CmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, VkMemoryBarrier[]? memoryBarriers, VkBufferMemoryBarrier[]? bufferMemoryBarriers, VkImageMemoryBarrier[]? imageMemoryBarriers)
    {
        fixed (VkMemoryBarrier*       pMemoryBarriers       = memoryBarriers)
        fixed (VkBufferMemoryBarrier* pBufferMemoryBarriers = bufferMemoryBarriers)
        fixed (VkImageMemoryBarrier*  pImageMemoryBarriers  = imageMemoryBarriers)
        {
            this.vkCmdPipelineBarrier.Invoke(commandBuffer, srcStageMask, dstStageMask, dependencyFlags, (uint)(memoryBarriers?.Length ?? 0), pMemoryBarriers, (uint)(bufferMemoryBarriers?.Length ?? 0), pBufferMemoryBarriers, (uint)(imageMemoryBarriers?.Length ?? 0), pImageMemoryBarriers);
        }
    }

    /// <summary>
    /// <para>Set scissor rectangles dynamically for a command buffer.</para>
    /// <para>The scissor rectangles taken from element i of pScissors replace the current state for the scissor index firstScissor + i, for i in [0, scissorCount).</para>
    /// <para>This command sets the scissor rectangles for subsequent drawing commands when drawing using <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#shaders-objects">shader objects</see>, or when the graphics pipeline is created with <see cref="VkDynamicState.VK_DYNAMIC_STATE_SCISSOR"/> set in <see cref="VkPipelineDynamicStateCreateInfo.pDynamicStates"/>. Otherwise, this state is specified by the <see cref="VkPipelineViewportStateCreateInfo.pScissors"/> values used to create the currently active pipeline.</para>
    /// </summary>
    /// <param name="commandBuffer">The command buffer into which the command will be recorded.</param>
    /// <param name="firstScissor">The index of the first scissor whose state is updated by the command.</param>
    /// <param name="scissorCount">The number of scissors whose rectangles are updated by the command.</param>
    /// <param name="pScissors">A pointer to an array of <see cref="VkRect2D"/> structures defining scissor rectangles.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdSetScissor(VkCommandBuffer commandBuffer, uint firstScissor, uint scissorCount, VkRect2D* pScissors) =>
        this.vkCmdSetScissor.Invoke(commandBuffer, firstScissor, scissorCount, pScissors);

    public void CmdSetScissor(VkCommandBuffer commandBuffer, uint firstScissor, in VkRect2D scissors)
    {
        fixed (VkRect2D* pScissors = &scissors)
        {
            this.vkCmdSetScissor.Invoke(commandBuffer, firstScissor, 1, pScissors);
        }
    }

    public void CmdSetScissor(VkCommandBuffer commandBuffer, uint firstScissor, VkRect2D[] scissors)
    {
        fixed (VkRect2D* pScissors = scissors)
        {
            this.vkCmdSetScissor.Invoke(commandBuffer, firstScissor, (uint)scissors.Length, pScissors);
        }
    }

    /// <summary>
    /// Set the viewport dynamically for a command buffer.
    /// This command sets the viewport transformation parameters state for subsequent drawing commands when drawing using <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#shaders-objects">shader objects</see>, or when the graphics pipeline is created with <see cref="VkDynamicState.VK_DYNAMIC_STATE_VIEWPORT"/> set in <see cref="VkPipelineDynamicStateCreateInfo.pDynamicStates"/>. Otherwise, this state is specified by the <see cref="VkPipelineViewportStateCreateInfo.pViewports"/> values used to create the currently active pipeline.
    /// The viewport parameters taken from element i of pViewports replace the current state for the viewport index firstViewport + i, for i in [0, viewportCount).
    /// </summary>
    /// <param name="commandBuffer">The command buffer into which the command will be recorded.</param>
    /// <param name="firstViewport">The index of the first viewport whose parameters are updated by the command.</param>
    /// <param name="viewportCount">The number of viewports whose parameters are updated by the command.</param>
    /// <param name="pViewports">A pointer to an array of <see cref="VkViewport"/> structures specifying viewport parameters.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdSetViewport(VkCommandBuffer commandBuffer, uint firstViewport, uint viewportCount, VkViewport* pViewports) =>
        this.vkCmdSetViewport.Invoke(commandBuffer, firstViewport, viewportCount, pViewports);

    public void CmdSetViewport(VkCommandBuffer commandBuffer, uint firstViewport, in VkViewport viewports)
    {
        fixed (VkViewport* pViewports = &viewports)
        {
            this.vkCmdSetViewport.Invoke(commandBuffer, firstViewport, 1, pViewports);
        }
    }

    public void CmdSetViewport(VkCommandBuffer commandBuffer, uint firstViewport, VkViewport[] viewports)
    {
        fixed (VkViewport* pViewports = viewports)
        {
            this.vkCmdSetViewport.Invoke(commandBuffer, firstViewport, (uint)viewports.Length, pViewports);
        }
    }

    /// <summary>
    /// Create a new buffer object.
    /// </summary>
    /// <param name="device">The logical device that creates the buffer object.</param>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkBufferCreateInfo"/> structure containing parameters affecting creation of the buffer.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pBuffer">A pointer to a <see cref="VkBuffer"/> handle in which the resulting buffer object is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult CreateBuffer(VkDevice device, VkBufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkBuffer* pBuffer) =>
        this.vkCreateBuffer.Invoke(device, pCreateInfo, pAllocator, pBuffer);

    public VkResult CreateBuffer(VkDevice device, in VkBufferCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkBuffer buffer)
    {
        fixed (VkBufferCreateInfo*    pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &allocator)
        fixed (VkBuffer*              pBuffer     = &buffer)
        {
            return this.vkCreateBuffer.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pBuffer);
        }
    }

    /// <summary>
    /// Create a new command pool object.
    /// </summary>
    /// <param name="device">The logical device that creates the command pool.</param>
    /// <param name="pCreateInfo">A pointer to a VkCommandPoolCreateInfo structure specifying the state of the command pool object.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pCommandPool">A pointer to a <see cref="VkCommandPool"/> handle in which the created pool is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult CreateCommandPool(VkDevice device, VkCommandPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkCommandPool* pCommandPool) =>
        this.vkCreateCommandPool.Invoke(device, pCreateInfo, pAllocator, pCommandPool);

    public VkResult CreateCommandPool(VkDevice device, in VkCommandPoolCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkCommandPool commandPool)
    {
        fixed (VkCommandPoolCreateInfo* pCreateInfo  = &createInfo)
        fixed (VkAllocationCallbacks*   pAllocator   = &allocator)
        fixed (VkCommandPool*           pCommandPool = &commandPool)
        {
            return this.vkCreateCommandPool.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pCommandPool);
        }
    }

    /// <summary>
    /// <para>Creates a descriptor pool object.</para>
    /// <para>The created descriptor pool is returned in pDescriptorPool.</para>
    /// </summary>
    /// <param name="device">The logical device that creates the descriptor pool.</param>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkDescriptorPoolCreateInfo"/> structure specifying the state of the descriptor pool object.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pDescriptorPool">A pointer to a VkDescriptorPool handle in which the resulting descriptor pool object is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult CreateDescriptorPool(VkDevice device, VkDescriptorPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorPool* pDescriptorPool) =>
        this.vkCreateDescriptorPool.Invoke(device, pCreateInfo, pAllocator, pDescriptorPool);

    public VkResult CreateDescriptorPool(VkDevice device, in VkDescriptorPoolCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkDescriptorPool descriptorPool)
    {
        fixed (VkDescriptorPoolCreateInfo* pCreateInfo     = &createInfo)
        fixed (VkAllocationCallbacks*      pAllocator      = &allocator)
        fixed (VkDescriptorPool*           pDescriptorPool = &descriptorPool)
        {
            return this.vkCreateDescriptorPool.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pDescriptorPool);
        }
    }

    /// <summary>
    /// Create a new descriptor set layout.
    /// </summary>
    /// <param name="device">The logical device that creates the descriptor set layout.</param>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkDescriptorSetLayoutCreateInfo"/> structure specifying the state of the descriptor set layout object.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pSetLayout">A pointer to a <see cref="VkDescriptorSetLayout"/> handle in which the resulting descriptor set layout object is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult CreateDescriptorSetLayout(VkDevice device, VkDescriptorSetLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorSetLayout* pSetLayout) =>
        this.vkCreateDescriptorSetLayout.Invoke(device, pCreateInfo, pAllocator, pSetLayout);

    public VkResult CreateDescriptorSetLayout(VkDevice device, in VkDescriptorSetLayoutCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkDescriptorSetLayout setLayout)
    {
        fixed (VkDescriptorSetLayoutCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*           pAllocator  = &allocator)
        fixed (VkDescriptorSetLayout*           pSetLayout  = &setLayout)
        {
            return this.vkCreateDescriptorSetLayout.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pSetLayout);
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
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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
    /// Create a new fence object.
    /// </summary>
    /// <param name="device">The logical device that creates the fence.</param>
    /// <param name="pCreateInfo">A pointer to a VkFenceCreateInfo structure containing information about how the fence is to be created.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pFence">A pointer to a handle in which the resulting fence object is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult CreateFence(VkDevice device, VkFenceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence) =>
        this.vkCreateFence.Invoke(device, pCreateInfo, pAllocator, pFence);

    public VkResult CreateFence(VkDevice device, in VkFenceCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkFence fence)
    {
        fixed (VkFenceCreateInfo*     pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &allocator)
        fixed (VkFence*               pFence      = &fence)
        {
            return this.vkCreateFence.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pFence);
        }
    }

    /// <summary>
    /// Create a new framebuffer object.
    /// </summary>
    /// <param name="device">The logical device that creates the framebuffer.</param>
    /// <param name="pCreateInfo">A pointer to a VkFramebufferCreateInfo structure describing additional information about framebuffer creation.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pFramebuffer">A pointer to a VkFramebuffer handle in which the resulting framebuffer object is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult CreateFramebuffer(VkDevice device, VkFramebufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkFramebuffer* pFramebuffer) =>
        this.vkCreateFramebuffer.Invoke(device, pCreateInfo, pAllocator, pFramebuffer);

    public VkResult CreateFramebuffer(VkDevice device, in VkFramebufferCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkFramebuffer framebuffer)
    {
        fixed (VkFramebufferCreateInfo* pCreateInfo  = &createInfo)
        fixed (VkAllocationCallbacks*   pAllocator   = &allocator)
        fixed (VkFramebuffer*           pFramebuffer = &framebuffer)
        {
            return this.vkCreateFramebuffer.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pFramebuffer);
        }
    }

    /// <summary>
    /// <para>Create graphics pipelines.</para>
    /// <para>The <see cref="VkGraphicsPipelineCreateInfo"/> structure includes an array of <see cref="VkPipelineShaderStageCreateInfo"/> structures for each of the desired active shader stages, as well as creation information for all relevant fixed-function stages, and a pipeline layout.</para>
    /// </summary>
    /// <param name="device">The logical device that creates the graphics pipelines.</param>
    /// <param name="pipelineCache">Either VK_NULL_HANDLE, indicating that pipeline caching is disabled; or the handle of a valid pipeline cache object, in which case use of that cache is enabled for the duration of the command.</param>
    /// <param name="createInfoCount">The length of the pCreateInfos and pPipelines arrays.</param>
    /// <param name="pCreateInfos">A pointer to an array of <see cref="VkGraphicsPipelineCreateInfo"/> structures.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pPipelines">A pointer to an array of <see cref="VkPipeline"/> handles in which the resulting graphics pipeline objects are returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult CreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo* pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines) =>
        this.vkCreateGraphicsPipelines.Invoke(device, pipelineCache, createInfoCount, pCreateInfos, pAllocator, pPipelines);

    public VkResult CreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, in VkGraphicsPipelineCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkPipeline pipelines)
    {
        fixed (VkGraphicsPipelineCreateInfo* pCreateInfos = &createInfo)
        fixed (VkAllocationCallbacks*        pAllocator   = &allocator)
        fixed (VkPipeline*                   pPipelines   = &pipelines)
        {
            return this.vkCreateGraphicsPipelines.Invoke(device, pipelineCache, 1, pCreateInfos, NullIfDefault(allocator, pAllocator), pPipelines);
        }
    }

    public VkResult CreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, VkGraphicsPipelineCreateInfo[] createInfos, in VkAllocationCallbacks allocator, out VkPipeline[] pipelines)
    {
        fixed (VkGraphicsPipelineCreateInfo* pCreateInfos = createInfos)
        fixed (VkAllocationCallbacks*        pAllocator   = &allocator)
        fixed (VkPipeline*                   pPipelines   = pipelines)
        {
            return this.vkCreateGraphicsPipelines.Invoke(device, pipelineCache, (uint)createInfos.Length, pCreateInfos, NullIfDefault(allocator, pAllocator), pPipelines);
        }
    }

    /// <summary>
    /// Create a new image object.
    /// </summary>
    /// <param name="device">The logical device that creates the image.</param>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkImageCreateInfo"/> structure containing parameters to be used to create the image.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pImage">A pointer to a <see cref="VkImage"/> handle in which the resulting image object is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult CreateImage(VkDevice device, VkImageCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkImage* pImage) =>
        this.vkCreateImage.Invoke(device, pCreateInfo, pAllocator, pImage);

    public VkResult CreateImage(VkDevice device, in VkImageCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkImage image)
    {
        fixed (VkImageCreateInfo*     pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &allocator)
        fixed (VkImage*               pImage      = &image)
        {
            return this.vkCreateImage.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pImage);
        }
    }

    /// <summary>
    /// Create an image view from an existing image.
    /// </summary>
    /// <param name="device">The logical device that creates the image view.</param>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkImageViewCreateInfo"/> structure containing parameters to be used to create the image view.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pView">A pointer to a VkImageView handle in which the resulting image view object is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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
    /// <para>Create a new Vulkan instance.</para>
    /// <see cref="CreateInstance"/> verifies that the requested layers exist. If not, <see cref="CreateInstance"/> will return <see cref="VkResult.VK_ERROR_LAYER_NOT_PRESENT"/>. Next <see cref="CreateInstance"/> verifies that the requested extensions are supported (e.g. in the implementation or in any enabled instance layer) and if any requested extension is not supported, <see cref="CreateInstance"/> must return VK_ERROR_EXTENSION_NOT_PRESENT. After verifying and enabling the instance layers and extensions the VkInstance object is created and returned to the application. If a requested extension is only supported by a layer, both the layer and the extension need to be specified at <see cref="CreateInstance"/> time for the creation to succeed.
    /// </summary>
    /// <param name="pCreateInfo">A pointer to a <see cref="VkInstanceCreateInfo"/> structure controlling creation of the instance.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pInstance">Points a VkInstance handle in which the resulting instance is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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
    /// Create a new sampler object.
    /// </summary>
    /// <param name="device">The logical device that creates the sampler.</param>
    /// <param name="pCreateInfo">A pointer to a VkSamplerCreateInfo structure specifying the state of the sampler object.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pSampler">A pointer to a <see cref="VkSampler"/> handle in which the resulting sampler object is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult CreateSampler(VkDevice device, VkSamplerCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSampler* pSampler) =>
        this.vkCreateSampler.Invoke(device, pCreateInfo, pAllocator, pSampler);

    public VkResult CreateSampler(VkDevice device, in VkSamplerCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkSampler sampler)
    {
        fixed (VkSamplerCreateInfo*   pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &allocator)
        fixed (VkSampler*             pSampler    = &sampler)
        {
            return this.vkCreateSampler.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pSampler);
        }
    }

    /// <summary>
    /// Create a new queue semaphore object.
    /// </summary>
    /// <param name="device">The logical device that creates the semaphore.</param>
    /// <param name="pCreateInfo">A pointer to a VkSemaphoreCreateInfo structure containing information about how the semaphore is to be created.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pSemaphore">A pointer to a handle in which the resulting semaphore object is returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult CreateSemaphore(VkDevice device, VkSemaphoreCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSemaphore* pSemaphore) =>
        this.vkCreateSemaphore.Invoke(device, pCreateInfo, pAllocator, pSemaphore);

    public VkResult CreateSemaphore(VkDevice device, in VkSemaphoreCreateInfo createInfo, in VkAllocationCallbacks allocator, out VkSemaphore semaphore)
    {
        fixed (VkSemaphoreCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &allocator)
        fixed (VkSemaphore*           pSemaphore  = &semaphore)
        {
            return this.vkCreateSemaphore.Invoke(device, pCreateInfo, NullIfDefault(allocator, pAllocator), pSemaphore);
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
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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
    /// Destroy a buffer object.
    /// </summary>
    /// <param name="device">The logical device that destroys the buffer.</param>
    /// <param name="buffer">The buffer to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroyBuffer(VkDevice device, VkBuffer buffer, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyBuffer.Invoke(device, buffer, pAllocator);

    public void DestroyBuffer(VkDevice device, VkBuffer buffer, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyBuffer.Invoke(device, buffer, NullIfDefault(allocator, pAllocator));
        }
    }

    /// <summary>
    /// <para>Destroy a command pool object</para>
    /// <para>When a pool is destroyed, all command buffers allocated from the pool are freed.</para>
    /// <para>Any primary command buffer allocated from another <see cref="VkCommandPool"/> that is in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">recording or executable state</see> and has a secondary command buffer allocated from commandPool recorded into it, becomes <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">invalid</see>.</para>
    /// </summary>
    /// <param name="device">The logical device that destroys the command pool.</param>
    /// <param name="commandPool">The handle of the command pool to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroyCommandPool(VkDevice device, VkCommandPool commandPool, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyCommandPool.Invoke(device, commandPool, pAllocator);

    public void DestroyCommandPool(VkDevice device, VkCommandPool commandPool, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyCommandPool.Invoke(device, commandPool, NullIfDefault(allocator, pAllocator));
        }
    }

    /// <summary>
    /// <para>Destroy a descriptor pool object.</para>
    /// <para>When a pool is destroyed, all descriptor sets allocated from the pool are implicitly freed and become invalid. Descriptor sets allocated from a given pool do not need to be freed before destroying that descriptor pool.</para>
    /// </summary>
    /// <param name="device">The logical device that destroys the descriptor pool.</param>
    /// <param name="descriptorPool">The descriptor pool to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    public void DestroyDescriptorPool(VkDevice device, VkDescriptorPool descriptorPool, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyDescriptorPool.Invoke(device, descriptorPool, pAllocator);

    /// <summary>
    /// Destroy a descriptor set layout object.
    /// </summary>
    /// <param name="device">The logical device that destroys the descriptor set layout.</param>
    /// <param name="descriptorSetLayout">The descriptor set layout to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroyDescriptorSetLayout(VkDevice device, VkDescriptorSetLayout descriptorSetLayout, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyDescriptorSetLayout.Invoke(device, descriptorSetLayout, pAllocator);

    public void DestroyDescriptorSetLayout(VkDevice device, VkDescriptorSetLayout descriptorSetLayout, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyDescriptorSetLayout.Invoke(device, descriptorSetLayout, NullIfDefault(allocator, pAllocator));
        }
    }

    /// <summary>
    /// <para>Destroy a logical device.</para>
    /// <para>To ensure that no work is active on the device, <see cref="DeviceWaitIdle"/> can be used to gate the destruction of the device. Prior to destroying a device, an application is responsible for destroying/freeing any Vulkan objects that were created using that device as the first parameter of the corresponding vkCreate* or vkAllocate* command.</para>
    /// </summary>
    /// <param name="device">The logical device to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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
    /// Destroy a fence object.
    /// </summary>
    /// <param name="device">The logical device that destroys the fence.</param>
    /// <param name="fence">The handle of the fence to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroyFence(VkDevice device, VkFence fence, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyFence.Invoke(device, fence, pAllocator);

    public void DestroyFence(VkDevice device, VkFence fence, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyFence.Invoke(device, fence, NullIfDefault(allocator, pAllocator));
        }
    }

    /// <summary>
    /// Destroy a framebuffer object.
    /// </summary>
    /// <param name="device">The logical device that destroys the framebuffer.</param>
    /// <param name="framebuffer">The handle of the framebuffer to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroyFramebuffer(VkDevice device, VkFramebuffer framebuffer, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyFramebuffer(device, framebuffer, pAllocator);

    public void DestroyFramebuffer(VkDevice device, VkFramebuffer framebuffer, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyFramebuffer(device, framebuffer, NullIfDefault(allocator, pAllocator));
        }
    }

    /// <summary>
    /// Destroy an image object.
    /// </summary>
    /// <param name="device">The logical device that destroys the image.</param>
    /// <param name="image">The image to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroyImage(VkDevice device, VkImage image, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyImage.Invoke(device, image, pAllocator);

    public void DestroyImage(VkDevice device, VkImage image, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyImage.Invoke(device, image, pAllocator);
        }
    }

    /// <summary>
    /// Destroy an image view object.
    /// </summary>
    /// <param name="device">The logical device that destroys the image view.</param>
    /// <param name="imageView">The image view to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroyImageView(VkDevice device, VkImageView imageView, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyImageView.Invoke(device, imageView, pAllocator);

    public void DestroyImageView(VkDevice device, VkImageView imageView, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyImageView.Invoke(device, imageView, NullIfDefault(allocator, pAllocator));
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
    /// Destroy a pipeline object.
    /// </summary>
    /// <param name="device">The logical device that destroys the pipeline.</param>
    /// <param name="pipeline">The handle of the pipeline to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroyPipeline(VkDevice device, VkPipeline pipeline, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroyPipeline.Invoke(device, pipeline, pAllocator);

    public void DestroyPipeline(VkDevice device, VkPipeline pipeline, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroyPipeline.Invoke(device, pipeline, NullIfDefault(allocator, pAllocator));
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
    /// Destroy a sampler object.
    /// </summary>
    /// <param name="device">The logical device that destroys the sampler.</param>
    /// <param name="sampler">The sampler to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroySampler(VkDevice device, VkSampler sampler, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroySampler.Invoke(device, sampler, pAllocator);

    public void DestroySampler(VkDevice device, VkSampler sampler, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroySampler.Invoke(device, sampler, NullIfDefault(allocator, pAllocator));
        }
    }

    /// <summary>
    /// Destroy a semaphore object.
    /// </summary>
    /// <param name="device">The logical device that destroys the semaphore.</param>
    /// <param name="semaphore">The handle of the semaphore to destroy.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void DestroySemaphore(VkDevice device, VkSemaphore semaphore, VkAllocationCallbacks* pAllocator) =>
        this.vkDestroySemaphore.Invoke(device, semaphore, pAllocator);

    public void DestroySemaphore(VkDevice device, VkSemaphore semaphore, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkDestroySemaphore.Invoke(device, semaphore, NullIfDefault(allocator, pAllocator));
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
    /// <para>Wait for a device to become idle.</para>
    /// </summary>
    /// <param name="device">The logical device to idle.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult DeviceWaitIdle(VkDevice device) =>
        this.vkDeviceWaitIdle.Invoke(device);

    /// <summary>
    /// <para>Finish recording a command buffer.</para>
    /// <para>The command buffer must have been in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">recording state</see>, and, if successful, is moved to the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">executable state</see>.</para>
    /// <para>If there was an error during recording, the application will be notified by an unsuccessful return code returned by <see cref="EndCommandBuffer"/>, and the command buffer will be moved to the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">invalid state</see>.</para>
    /// <para>In case the application recorded one or more <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#video-encode-operations">video encode operations</see> into the command buffer, implementations may return the <see cref="VkResult.VK_ERROR_INVALID_VIDEO_STD_PARAMETERS_KHR"/> error if any of the specified Video Std parameters do not adhere to the syntactic or semantic requirements of the used video compression standard, or if values derived from parameters according to the rules defined by the used video compression standard do not adhere to the capabilities of the video compression standard or the implementation.</para>
    /// <remarks>Note: Applications should not rely on the <see cref="VkResult.VK_ERROR_INVALID_VIDEO_STD_PARAMETERS_KHR"/> error being returned by any command as a means to verify Video Std parameters, as implementations are not required to report the error in any specific set of cases.</remarks>
    /// </summary>
    /// <param name="commandBuffer">The command buffer to complete recording.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult EndCommandBuffer(VkCommandBuffer commandBuffer) =>
        this.vkEndCommandBuffer.Invoke(commandBuffer);

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
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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
            properties = [];

            return result;
        }

        properties = new VkExtensionProperties[propertyCount];

        fixed (byte*                  pLayerName  = Encoding.UTF8.GetBytes(layerName ?? ""))
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
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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
            properties = [];

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
            properties = [];

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
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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
            physicalDevices = [];

            return result;
        }

        physicalDevices = new VkPhysicalDevice[physicalDeviceCount];

        fixed (VkPhysicalDevice* pPhysicalDevices = physicalDevices)
        {
            return this.vkEnumeratePhysicalDevices.Invoke(instance, &physicalDeviceCount, pPhysicalDevices);
        }
    }

    /// <summary>
    /// <para>Free command buffers.</para>
    /// <para>Any primary command buffer that is in the recording or executable state and has any element of pCommandBuffers recorded into it, becomes <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">invalid</see>.</para>
    /// </summary>
    /// <param name="device">The logical device that owns the command pool.</param>
    /// <param name="commandPool">The command pool from which the command buffers were allocated.</param>
    /// <param name="commandBufferCount">The length of the pCommandBuffers array.</param>
    /// <param name="pCommandBuffers">A pointer to an array of handles of command buffers to free.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void FreeCommandBuffers(VkDevice device, VkCommandPool commandPool, uint commandBufferCount, VkCommandBuffer* pCommandBuffers) =>
        this.vkFreeCommandBuffers.Invoke(device, commandPool, commandBufferCount, pCommandBuffers);

    public void FreeCommandBuffers(VkDevice device, VkCommandPool commandPool, in VkCommandBuffer commandBuffer)
    {
        fixed (VkCommandBuffer* pCommandBuffers = &commandBuffer)
        {
            this.vkFreeCommandBuffers.Invoke(device, commandPool, 1, pCommandBuffers);
        }
    }

    public void FreeCommandBuffers(VkDevice device, VkCommandPool commandPool, VkCommandBuffer[] commandBuffers)
    {
        fixed (VkCommandBuffer* pCommandBuffers = commandBuffers)
        {
            this.vkFreeCommandBuffers.Invoke(device, commandPool, (uint)commandBuffers.Length, pCommandBuffers);
        }
    }

    /// <summary>
    /// Free one or more descriptor sets
    /// </summary>
    /// <param name="device">The logical device that owns the descriptor pool.</param>
    /// <param name="descriptorPool">The descriptor pool from which the descriptor sets were allocated.</param>
    /// <param name="descriptorSetCount">The number of elements in the pDescriptorSets array.</param>
    /// <param name="pDescriptorSets">A pointer to an array of handles to <see cref="VkDescriptorSet"/> objects.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult FreeDescriptorSets(VkDevice device, VkDescriptorPool descriptorPool, uint descriptorSetCount, VkDescriptorSet* pDescriptorSets) =>
        this.vkFreeDescriptorSets.Invoke(device, descriptorPool, descriptorSetCount, pDescriptorSets);

    public VkResult FreeDescriptorSets(VkDevice device, VkDescriptorPool descriptorPool, in VkDescriptorSet descriptorSet)
    {
        fixed (VkDescriptorSet* pDescriptorSets = &descriptorSet)
        {
            return this.vkFreeDescriptorSets.Invoke(device, descriptorPool, 1, pDescriptorSets);
        }
    }

    public VkResult FreeDescriptorSets(VkDevice device, VkDescriptorPool descriptorPool, VkDescriptorSet[] descriptorSets)
    {
        fixed (VkDescriptorSet* pDescriptorSets = descriptorSets)
        {
            return this.vkFreeDescriptorSets.Invoke(device, descriptorPool, (uint)descriptorSets.Length, pDescriptorSets);
        }
    }

    /// <summary>
    /// <para>Free device memory.</para>
    /// <para>Before freeing a memory object, an application must ensure the memory object is no longer in use by the device — for example by command buffers in the pending state. Memory can be freed whilst still bound to resources, but those resources must not be used afterwards. Freeing a memory object releases the reference it held, if any, to its payload. If there are still any bound images or buffers, the memory object’s payload may not be immediately released by the implementation, but must be released by the time all bound images and buffers have been destroyed. Once all references to a payload are released, it is returned to the heap from which it was allocated.</para>
    /// <para>How memory objects are bound to Images and Buffers is described in detail in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#resources-association">Resource Memory Association</see> section.</para>
    /// <para>If a memory object is mapped at the time it is freed, it is implicitly unmapped.</para>
    /// <remarks>Note: As described above, host writes are not implicitly flushed when the memory object is unmapped, but the implementation must guarantee that writes that have not been flushed do not affect any other memory.</remarks>
    /// </summary>
    /// <param name="device">The logical device that owns the memory.</param>
    /// <param name="memory">The <see cref="VkDeviceMemory"/> object to be freed.</param>
    /// <param name="pAllocator">Controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void FreeMemory(VkDevice device, VkDeviceMemory memory, VkAllocationCallbacks* pAllocator) =>
        this.vkFreeMemory.Invoke(device, memory, pAllocator);

    public void FreeMemory(VkDevice device, VkDeviceMemory memory, in VkAllocationCallbacks allocator)
    {
        fixed (VkAllocationCallbacks* pAllocator = &allocator)
        {
            this.vkFreeMemory.Invoke(device, memory, NullIfDefault(allocator, pAllocator));
        }
    }

    /// <summary>
    /// In order to support systems with multiple Vulkan implementations, the function pointers returned by <see cref="GetInstanceProcAddr"/> may point to dispatch code that calls a different real implementation for different <see cref="VkDevice"/> objects or their child objects. The overhead of the internal dispatch for <see cref="VkDevice"/> objects can be avoided by obtaining device-specific function pointers for any commands that use a device or device-child object as their dispatchable object.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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
            : throw new Exception($"Cannot found extension {T.Name}");

    /// <summary>
    /// Return a function pointer for a command.
    /// </summary>
    /// <param name="instance">The instance that the function pointer will be compatible with, or NULL for commands not dependent on any instance.</param>
    /// <param name="pName">The name of the command to obtain.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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
    /// Returns the memory requirements for specified Vulkan object.
    /// </summary>
    /// <param name="device">The logical device that owns the buffer.</param>
    /// <param name="buffer">The buffer to query.</param>
    /// <param name="pMemoryRequirements">A pointer to a <see cref="VkMemoryRequirements"/> structure in which the memory requirements of the buffer object are returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void GetBufferMemoryRequirements(VkDevice device, VkBuffer buffer, VkMemoryRequirements* pMemoryRequirements) =>
        this.vkGetBufferMemoryRequirements.Invoke(device, buffer, pMemoryRequirements);

    public void GetBufferMemoryRequirements(VkDevice device, VkBuffer buffer, out VkMemoryRequirements memoryRequirements)
    {
        fixed (VkMemoryRequirements* pMemoryRequirements = &memoryRequirements)
        {
            this.vkGetBufferMemoryRequirements.Invoke(device, buffer, pMemoryRequirements);
        }
    }

    /// <summary>
    /// Returns the memory requirements for specified Vulkan object.
    /// </summary>
    /// <param name="device">The logical device that owns the image.</param>
    /// <param name="image">The image to query.</param>
    /// <param name="pMemoryRequirements">A pointer to a <see cref="VkMemoryRequirements"/> structure in which the memory requirements of the image object are returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void GetImageMemoryRequirements(VkDevice device, VkImage image, VkMemoryRequirements* pMemoryRequirements) =>
        this.vkGetImageMemoryRequirements.Invoke(device, image, pMemoryRequirements);

    public void GetImageMemoryRequirements(VkDevice device, VkImage image, out VkMemoryRequirements memoryRequirements)
    {
        fixed (VkMemoryRequirements* pMemoryRequirements = &memoryRequirements)
        {
            this.vkGetImageMemoryRequirements.Invoke(device, image, pMemoryRequirements);
        }
    }

    /// <summary>
    /// Lists physical device's format capabilities;
    /// </summary>
    /// <param name="physicalDevice">The physical device from which to query the format properties.</param>
    /// <param name="format">The format whose properties are queried.</param>
    /// <param name="pFormatProperties">A pointer to a VkFormatProperties structure in which physical device properties for format are returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void GetPhysicalDeviceFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkFormatProperties* pFormatProperties) =>
        this.vkGetPhysicalDeviceFormatProperties.Invoke(physicalDevice, format, pFormatProperties);

    public void GetPhysicalDeviceFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, out VkFormatProperties formatProperties)
    {
        fixed (VkFormatProperties* pFormatProperties = &formatProperties)
        {
            this.vkGetPhysicalDeviceFormatProperties.Invoke(physicalDevice, format, pFormatProperties);
        }
    }

    /// <summary>
    /// <para>Reports capabilities of a physical device.</para>
    /// <para><see cref="GetInstanceProcAddr"/> itself is obtained in a platform- and loader- specific manner. Typically, the loader library will export this command as a function symbol, so applications can link against the loader library, or load it dynamically and look up the symbol using platform-specific APIs.</para>
    /// </summary>
    /// <param name="physicalDevice">The physical device from which to query the supported features.</param>
    /// <param name="pFeatures">A pointer to a <see cref="VkPhysicalDeviceFeatures"/> structure in which the physical device features are returned. For each feature, a value of true specifies that the feature is supported on this physical device, and VK_FALSE specifies that the feature is not supported.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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
    /// Reports memory information for the specified physical device.
    /// </summary>
    /// <param name="physicalDevice">The handle to the device to query.</param>
    /// <param name="pMemoryProperties">A pointer to a <see cref="VkPhysicalDeviceMemoryProperties"/> structure in which the properties are returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void GetPhysicalDeviceMemoryProperties(VkPhysicalDevice physicalDevice, VkPhysicalDeviceMemoryProperties* pMemoryProperties) =>
        this.vkGetPhysicalDeviceMemoryProperties.Invoke(physicalDevice, pMemoryProperties);

    public void GetPhysicalDeviceMemoryProperties(VkPhysicalDevice physicalDevice, out VkPhysicalDeviceMemoryProperties memoryProperties)
    {
        fixed (VkPhysicalDeviceMemoryProperties* pMemoryProperties = &memoryProperties)
        {
            this.vkGetPhysicalDeviceMemoryProperties.Invoke(physicalDevice, pMemoryProperties);
        }
    }

    /// <summary>
    /// Returns properties of a physical device.
    /// </summary>
    /// <param name="physicalDevice">The handle to the physical device whose properties will be queried.</param>
    /// <param name="pProperties">A pointer to a <see cref="VkPhysicalDeviceProperties"/> structure in which properties are returned.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
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

            this.deviceExtensionsMap[layer ?? ""] = extensions = properties.Select(x => Marshal.PtrToStringAnsi((nint)x.extensionName)!).ToHashSet();
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

    /// <summary>
    /// Map a memory object into application address space.
    /// After a successful call to <see cref="MapMemory"/> the memory object memory is considered to be currently host mapped.
    /// Note: It is an application error to call <see cref="MapMemory"/> on a memory object that is already host mapped.
    /// Note: <see cref="MapMemory"/> will fail if the implementation is unable to allocate an appropriately sized contiguous virtual address range, e.g. due to virtual address space fragmentation or platform limits. In such cases, vkMapMemory must return <see cref="VkResult.VK_ERROR_MEMORY_MAP_FAILED"/>. The application can improve the likelihood of success by reducing the size of the mapped range and/or removing unneeded mappings using <see cref="UnmapMemory"/>.
    /// <see cref="MapMemory"/> does not check whether the device memory is currently in use before returning the host-accessible pointer. The application must guarantee that any previously submitted command that writes to this range has completed before the host reads from or writes to that range, and that any previously submitted command that reads from that range has completed before the host writes to that region (see here for details on fulfilling such a guarantee). If the device memory was allocated without the <see cref="VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT"/> set, these guarantees must be made for an extended range: the application must round down the start of the range to the nearest multiple of <see cref="VkPhysicalDeviceLimits.nonCoherentAtomSize"/>, and round the end of the range up to the nearest multiple of <see cref="VkPhysicalDeviceLimits.nonCoherentAtomSize"/>.
    /// While a range of device memory is host mapped, the application is responsible for synchronizing both device and host access to that memory range.
    /// Note: It is important for the application developer to become meticulously familiar with all of the mechanisms described in the chapter on <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization">Synchronization and Cache Control</see> as they are crucial to maintaining memory access ordering.
    /// Calling <see cref="MapMemory"/> is equivalent to calling <see cref="VkKhrMapMemory2.MapMemory2"/> with an empty pNext chain.
    /// </summary>
    /// <param name="device">The logical device that owns the memory.</param>
    /// <param name="memory">The <see cref="VkDeviceMemory"/> object to be mapped.</param>
    /// <param name="offset">A zero-based byte offset from the beginning of the memory object.</param>
    /// <param name="size">The size of the memory range to map, or VK_WHOLE_SIZE to map from offset to the end of the allocation.</param>
    /// <param name="flags">Reserved for future use.</param>
    /// <param name="ppData">A pointer to a void* variable in which a host-accessible pointer to the beginning of the mapped range is returned. This pointer minus offset must be aligned to at least <see cref="VkPhysicalDeviceLimits.minMemoryMapAlignment"/>.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult MapMemory(VkDevice device, VkDeviceMemory memory, VkDeviceSize offset, VkDeviceSize size, VkMemoryMapFlags flags, void** ppData) =>
        this.vkMapMemory.Invoke(device, memory, offset, size, flags, ppData);

    public VkResult MapMemory<T>(VkDevice device, VkDeviceMemory memory, VkDeviceSize offset, VkMemoryMapFlags flags, T** ppData) where T : unmanaged =>
        this.vkMapMemory.Invoke(device, memory, offset, (ulong)Marshal.SizeOf<T>(), flags, (void**)ppData);

    public VkResult MapMemory<T>(VkDevice device, VkDeviceMemory memory, VkDeviceSize offset, VkMemoryMapFlags flags, T[] data) where T : unmanaged
    {
        using var pointer = new PointerArray(sizeof(T*) * data.Length);

        var ppData = (T**)(nint)pointer;

        if (this.vkMapMemory.Invoke(device, memory, offset, (ulong)(Marshal.SizeOf<T>() * data.Length), flags, (void**)ppData) is var result && result == VkResult.VK_SUCCESS)
        {
            Copy(data, *ppData, data.Length);
        }

        return result;
    }

    /// <summary>
    /// <para>Wait for a queue to become idle.</para>
    /// <para><see cref="QueueWaitIdle"/> is equivalent to having submitted a valid fence to every previously executed queue submission command that accepts a fence, then waiting for all of those fences to signal using <see cref="WaitForFences"/> with an infinite timeout and waitAll set to true.</para>
    /// </summary>
    /// <param name="queue">The queue on which to wait.</param>
    /// <returns></returns>
    public VkResult QueueWaitIdle(VkQueue queue) =>
        this.vkQueueWaitIdle.Invoke(queue);

    /// <summary>
    /// <para>Submits a sequence of semaphores or command buffers to a queue.</para>
    /// <para><see cref="QueueSubmit"/> is a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#devsandqueues-submission">queue submission command</see>, with each batch defined by an element of pSubmits. Batches begin execution in the order they appear in pSubmits, but may complete out of order.</para>
    /// <para>Fence and semaphore operations submitted with <see cref="QueueSubmit"/> have additional ordering constraints compared to other submission commands, with dependencies involving previous and subsequent queue operations. Information about these additional constraints can be found in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-semaphores">semaphore</see> and <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-fences">fence</see> sections of the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization">synchronization chapter</see>.</para>
    /// <para>Details on the interaction of pWaitDstStageMask with synchronization are described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-semaphores-waiting">semaphore wait operation</see> section of <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization">the synchronization chapter</see>.</para>
    /// <para>The order that batches appear in pSubmits is used to determine <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-submission-order">submission order</see>, and thus all the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-implicit">implicit ordering guarantees</see> that respect it. Other than these implicit ordering guarantees and any <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization">explicit synchronization primitives</see>, these batches may overlap or otherwise execute out of order.</para>
    /// <para>If any command buffer submitted to this queue is in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">executable state</see>, it is moved to the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">pending state</see>. Once execution of all submissions of a command buffer complete, it moves from the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">pending state</see>, back to the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">executable state</see>. If a command buffer was recorded with the <see cref="VkCommandBufferUsageFlagBits.VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT"/> flag, it instead moves to the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">invalid state</see>.</para>
    /// <para>If <see cref="QueueSubmit"/> fails, it may return <see cref="VkResult.VK_ERROR_OUT_OF_HOST_MEMORY"/> or <see cref="VkResult.VK_ERROR_OUT_OF_DEVICE_MEMORY"/>. If it does, the implementation must ensure that the state and contents of any resources or synchronization primitives referenced by the submitted command buffers and any semaphores referenced by pSubmits is unaffected by the call or its failure. If vkQueueSubmit fails in such a way that the implementation is unable to make that guarantee, the implementation must return <see cref="VkResult.VK_ERROR_DEVICE_LOST"/>. See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#devsandqueues-lost-device">Lost Device</see>.</para>
    /// </summary>
    /// <param name="queue">The queue that the command buffers will be submitted to.</param>
    /// <param name="submitCount">The number of elements in the pSubmits array.</param>
    /// <param name="pSubmits">A pointer to an array of VkSubmitInfo structures, each specifying a command buffer submission batch.</param>
    /// <param name="fence">An optional handle to a fence to be signaled once all submitted command buffers have completed execution. If fence is not VK_NULL_HANDLE, it defines a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-fences-signaling">fence signal operation</see>.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult QueueSubmit(VkQueue queue, uint submitCount, VkSubmitInfo* pSubmits, VkFence fence) =>
        this.vkQueueSubmit.Invoke(queue, submitCount, pSubmits, fence);

    public VkResult QueueSubmit(VkQueue queue, in VkSubmitInfo submit, VkFence fence)
    {
        fixed (VkSubmitInfo* pSubmits = &submit)
        {
            return this.vkQueueSubmit.Invoke(queue, 1, pSubmits, fence);
        }
    }

    public VkResult QueueSubmit(VkQueue queue, VkSubmitInfo[] submits, VkFence fence)
    {
        fixed (VkSubmitInfo* pSubmits = submits)
        {
            return this.vkQueueSubmit.Invoke(queue, (uint)submits.Length, pSubmits, fence);
        }
    }

    /// <summary>
    /// Reset a command buffer to the initial state.
    /// </summary>
    /// <param name="commandBuffer">The command buffer to reset. The command buffer can be in any state other than <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">pending</see>, and is moved into the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">initial state</see>.</param>
    /// <param name="flags">A bitmask of <see cref="VkCommandBufferResetFlagBits"/> controlling the reset operation.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult ResetCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferResetFlags flags) =>
        this.vkResetCommandBuffer.Invoke(commandBuffer, flags);

    /// <summary>
    /// <para>Resets one or more fence objects.</para>
    /// <para>If any member of pFences currently has its <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-fences-importing">payload imported</see> with temporary permanence, that fence’s prior permanent payload is first restored. The remaining operations described therefore operate on the restored payload.</para>
    /// <para>When <see cref="ResetFences"/> is executed on the host, it defines a fence unsignal operation for each fence, which resets the fence to the unsignaled state.</para>
    /// <para>If any member of pFences is already in the unsignaled state when <see cref="ResetFences"/> is executed, then <see cref="ResetFences"/> has no effect on that fence.</para>
    /// </summary>
    /// <param name="device">The logical device that owns the fences.</param>
    /// <param name="fenceCount">The number of fences to reset.</param>
    /// <param name="pFences">A pointer to an array of fence handles to reset.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult ResetFences(VkDevice device, uint fenceCount, VkFence* pFences) =>
        this.vkResetFences.Invoke(device, fenceCount, pFences);

    public VkResult ResetFences(VkDevice device, in VkFence fence)
    {
        fixed (VkFence* pFences = &fence)
        {
            return this.vkResetFences.Invoke(device, 1, pFences);
        }
    }

    public VkResult ResetFences(VkDevice device, VkFence[] fences)
    {
        fixed (VkFence* pFences = fences)
        {
            return this.vkResetFences.Invoke(device, (uint)fences.Length, pFences);
        }
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

    /// <summary>
    /// <para>Unmap a previously mapped memory object.</para>
    /// <para>Calling <see cref="UnmapMemory"/> is equivalent to calling <see cref="VkKhrMapMemory2.UnmapMemory2"/> with an empty pNext chain and the flags parameter set to zero.</para>
    /// </summary>
    /// <param name="device">The logical device that owns the memory.</param>
    /// <param name="memory">A pointer to a <see cref="VkMemoryUnmapInfoKHR"/> structure describing parameters of the unmap.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void UnmapMemory(VkDevice device, VkDeviceMemory memory) =>
        this.vkUnmapMemory.Invoke(device, memory);

    /// <summary>
    /// <para>Update the contents of a descriptor set object.</para>
    /// <para>The operations described by pDescriptorWrites are performed first, followed by the operations described by pDescriptorCopies. Within each array, the operations are performed in the order they appear in the array.</para>
    /// <para>Each element in the pDescriptorWrites array describes an operation updating the descriptor set using descriptors for resources specified in the structure.</para>
    /// <para>Each element in the pDescriptorCopies array is a <see cref="VkCopyDescriptorSet"/> structure describing an operation copying descriptors between sets.</para>
    /// <para>If the dstSet member of any element of pDescriptorWrites or pDescriptorCopies is bound, accessed, or modified by any command that was recorded to a command buffer which is currently in the recording or executable state, and any of the descriptor bindings that are updated were not created with the <see cref="VkDescriptorBindingFlagBits.VK_DESCRIPTOR_BINDING_UPDATE_AFTER_BIND_BIT"/> or <see cref="VkDescriptorBindingFlagBits.VK_DESCRIPTOR_BINDING_UPDATE_UNUSED_WHILE_PENDING_BIT"/> bits set, that command buffer becomes <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">invalid</see>.</para>
    /// </summary>
    /// <param name="device">the logical device that updates the descriptor sets.</param>
    /// <param name="descriptorWriteCount">The number of elements in the pDescriptorWrites array.</param>
    /// <param name="pDescriptorWrites">A pointer to an array of <see cref="VkWriteDescriptorSet"/> structures describing the descriptor sets to write to.</param>
    /// <param name="descriptorCopyCount">The number of elements in the pDescriptorCopies array.</param>
    /// <param name="pDescriptorCopies">A pointer to an array of <see cref="VkCopyDescriptorSet"/> structures describing the descriptor sets to copy between.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void UpdateDescriptorSets(VkDevice device, uint descriptorWriteCount, VkWriteDescriptorSet* pDescriptorWrites, uint descriptorCopyCount, VkCopyDescriptorSet* pDescriptorCopies) =>
        this.vkUpdateDescriptorSets.Invoke(device, descriptorWriteCount, pDescriptorWrites, descriptorCopyCount, pDescriptorCopies);

    public void UpdateDescriptorSets(VkDevice device, VkWriteDescriptorSet[]? descriptorWrites, VkCopyDescriptorSet[]? descriptorCopies)
    {
        fixed (VkWriteDescriptorSet* pDescriptorWrites = descriptorWrites)
        fixed (VkCopyDescriptorSet*  pDescriptorCopies = descriptorCopies)
        {
            this.vkUpdateDescriptorSets.Invoke(device, (uint)(descriptorWrites?.Length ?? 0), pDescriptorWrites, (uint)(descriptorCopies?.Length ?? 0), pDescriptorCopies);
        }
    }

    /// <summary>
    /// <para>Wait for one or more fences to become signaled.</para>
    /// <para>If the condition is satisfied when <see cref="WaitForFences"/> is called, then <see cref="WaitForFences"/> returns immediately. If the condition is not satisfied at the time <see cref="WaitForFences"/> is called, then <see cref="WaitForFences"/> will block and wait until the condition is satisfied or the timeout has expired, whichever is sooner.</para>
    /// <para>If timeout is zero, then <see cref="WaitForFences"/> does not wait, but simply returns the current state of the fences. VK_TIMEOUT will be returned in this case if the condition is not satisfied, even though no actual wait was performed.</para>
    /// <para>If the condition is satisfied before the timeout has expired, vkWaitForFences returns <see cref="VkResult.VK_SUCCESS"/>. Otherwise, vkWaitForFences returns <see cref="VkResult.VK_TIMEOUT"/> after the timeout has expired.</para>
    /// <para>If device loss occurs (see Lost Device) before the timeout has expired, vkWaitForFences must return in finite time with either <see cref="VkResult.VK_SUCCESS"/> or <see cref="VkResult.VK_ERROR_DEVICE_LOST"/>.</para>
    /// <remarks>Note: While we guarantee that <see cref="WaitForFences"/> must return in finite time, no guarantees are made that it returns immediately upon device loss. However, the client can reasonably expect that the delay will be on the order of seconds and that calling <see cref="WaitForFences"/> will not result in a permanently (or seemingly permanently) dead process.</remarks>
    /// </summary>
    /// <param name="device">The logical device that owns the fences.</param>
    /// <param name="fenceCount">The number of fences to wait on.</param>
    /// <param name="pFences">A pointer to an array of fenceCount fence handles.</param>
    /// <param name="waitAll">The condition that must be satisfied to successfully unblock the wait. If waitAll is true, then the condition is that all fences in pFences are signaled. Otherwise, the condition is that at least one fence in pFences is signaled.</param>
    /// <param name="timeout">The timeout period in units of nanoseconds. timeout is adjusted to the closest value allowed by the implementation-dependent timeout accuracy, which may be substantially longer than one nanosecond, and may be longer than the requested period.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult WaitForFences(VkDevice device, uint fenceCount, VkFence* pFences, VkBool32 waitAll, ulong timeout) =>
        this.vkWaitForFences.Invoke(device, fenceCount, pFences, waitAll, timeout);

    public VkResult WaitForFences(VkDevice device, in VkFence fence, VkBool32 waitAll, ulong timeout)
    {
        fixed (VkFence* pFences = &fence)
        {
            return this.vkWaitForFences.Invoke(device, 1, pFences, waitAll, timeout);
        }
    }

    public VkResult WaitForFences(VkDevice device, VkFence[] fences, VkBool32 waitAll, ulong timeout)
    {
        fixed (VkFence* pFences = fences)
        {
            return this.vkWaitForFences.Invoke(device, (uint)fences.Length, pFences, waitAll, timeout);
        }
    }
}
