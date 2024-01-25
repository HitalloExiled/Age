using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

internal unsafe static partial class PInvoke
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkAllocateCommandBuffers.html">vkAllocateCommandBuffers</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkAllocateCommandBuffers(VkDevice device, VkCommandBufferAllocateInfo* pAllocateInfo, VkCommandBuffer* pCommandBuffers);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkAllocateDescriptorSets.html">vkAllocateDescriptorSets</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkAllocateDescriptorSets(VkDevice device, VkDescriptorSetAllocateInfo* pAllocateInfo, VkDescriptorSet* pDescriptorSets);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkAllocateMemory.html">vkAllocateMemory</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkAllocateMemory(VkDevice device, VkMemoryAllocateInfo* pAllocateInfo, VkAllocationCallbacks* pAllocator, VkDeviceMemory* pMemory);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkBeginCommandBuffer.html">vkBeginCommandBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkBeginCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferBeginInfo* pBeginInfo);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkBindBufferMemory.html">vkBindBufferMemory</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkBindBufferMemory(VkDevice device, VkBuffer buffer, VkDeviceMemory memory, VkDeviceSize memoryOffset);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkBindImageMemory.html">vkBindImageMemory</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkBindImageMemory(VkDevice device, VkImage image, VkDeviceMemory memory, VkDeviceSize memoryOffset);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdBeginRenderPass.html">vkCmdBeginRenderPass</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdBeginRenderPass(VkCommandBuffer commandBuffer, VkRenderPassBeginInfo* pRenderPassBegin, VkSubpassContents contents);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdBindDescriptorSets.html">vkCmdBindDescriptorSets</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, uint descriptorSetCount, VkDescriptorSet* pDescriptorSets, uint dynamicOffsetCount, uint* pDynamicOffsets);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdBindPipeline.html">vkCmdBindPipeline</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdBindPipeline(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipeline pipeline);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdBindIndexBuffer.html">vkCmdBindIndexBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdBindIndexBuffer(VkCommandBuffer commandBuffer, VkBuffer buffer, VkDeviceSize offset, VkIndexType indexType);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdBindVertexBuffers.html">vkCmdBindVertexBuffers</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, uint firstBinding, uint bindingCount, VkBuffer* pBuffers, VkDeviceSize* pOffsets);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdBlitImage.html">vkCmdBlitImage</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdBlitImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, VkImageBlit* pRegions, VkFilter filter);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdClearColorImage.html">vkCmdClearColorImage</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, VkClearColorValue* pColor, uint rangeCount, VkImageSubresourceRange* pRanges);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdCopyBuffer.html">vkCmdCopyBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdCopyBuffer(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkBuffer dstBuffer, uint regionCount, VkBufferCopy* pRegions);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdCopyBufferToImage.html">vkCmdCopyBufferToImage</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdCopyBufferToImage(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkImage dstImage, VkImageLayout dstImageLayout, uint regionCount, VkBufferImageCopy* pRegions);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdDraw.html">vkCmdDraw</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdDraw(VkCommandBuffer commandBuffer, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdDrawIndexed.html">vkCmdDrawIndexed</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdDrawIndexed(VkCommandBuffer commandBuffer, uint indexCount, uint instanceCount, uint firstIndex, int vertexOffset, uint firstInstance);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdEndRenderPass.html">vkCmdEndRenderPass</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdEndRenderPass(VkCommandBuffer commandBuffer);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdPipelineBarrier.html">vkCmdPipelineBarrier</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdSetScissor.html">vkCmdSetScissor</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdSetScissor(VkCommandBuffer commandBuffer, uint firstScissor, uint scissorCount, VkRect2D* pScissors);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdSetViewport.html">vkCmdSetViewport</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdSetViewport(VkCommandBuffer commandBuffer, uint firstViewport, uint viewportCount, VkViewport* pViewports);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateBuffer.html">vkCreateBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateBuffer(VkDevice device, VkBufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkBuffer* pBuffer);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateCommandPool.html">vkCreateCommandPool</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateCommandPool(VkDevice device, VkCommandPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkCommandPool* pCommandPool);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateDescriptorPool.html">vkCreateDescriptorPool</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateDescriptorPool(VkDevice device, VkDescriptorPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorPool* pDescriptorPool);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateDescriptorSetLayout.html">vkCreateDescriptorSetLayout</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateDescriptorSetLayout(VkDevice device, VkDescriptorSetLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorSetLayout* pSetLayout);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateDevice.html">vkCreateDevice</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, VkDeviceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDevice* pDevice);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateFence.html">vkCreateFence</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateFence(VkDevice device, VkFenceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateFramebuffer.html">vkCreateFramebuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateFramebuffer(VkDevice device, VkFramebufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkFramebuffer* pFramebuffer);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateGraphicsPipelines.html">vkCreateGraphicsPipelines</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo* pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateImage.html">vkCreateImage</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateImage(VkDevice device, VkImageCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkImage* pImage);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateImageView.html">vkCreateImageView</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateImageView(VkDevice device, VkImageViewCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkImageView* pView);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateInstance.html">vkCreateInstance</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreatePipelineLayout.html">vkCreatePipelineLayout</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreatePipelineLayout(VkDevice device, VkPipelineLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkPipelineLayout* pPipelineLayout);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateRenderPass.html">vkCreateRenderPass</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateRenderPass(VkDevice device, VkRenderPassCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkRenderPass* pRenderPass);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateSampler.html">vkCreateSampler</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateSampler(VkDevice device, VkSamplerCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSampler* pSampler);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateSemaphore.html">vkCreateSemaphore</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateSemaphore(VkDevice device, VkSemaphoreCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSemaphore* pSemaphore);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateShaderModule.html">vkCreateShaderModule</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateShaderModule(VkDevice device, VkShaderModuleCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkShaderModule* pShaderModule);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyBuffer.html">vkDestroyBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyBuffer(VkDevice device, VkBuffer buffer, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyCommandPool.html">vkDestroyCommandPool</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyCommandPool(VkDevice device, VkCommandPool commandPool, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyDescriptorPool.html">vkDestroyDescriptorPool</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyDescriptorPool(VkDevice device, VkDescriptorPool descriptorPool, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyDescriptorSetLayout.html">vkDestroyDescriptorSetLayout</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyDescriptorSetLayout(VkDevice device, VkDescriptorSetLayout descriptorSetLayout, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyDevice.html">vkDestroyDevice</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyDevice(VkDevice device, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyFence.html">vkDestroyFence</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyFence(VkDevice device, VkFence fence, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyFramebuffer.html">vkDestroyFramebuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyFramebuffer(VkDevice device, VkFramebuffer framebuffer, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyImage.html">vkDestroyImage</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyImage(VkDevice device, VkImage image, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyImageView.html">vkDestroyImageView</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyImageView(VkDevice device, VkImageView imageView, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyInstance.html">vkDestroyInstance</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyInstance(VkInstance instance, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyPipeline.html">vkDestroyPipeline</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyPipeline(VkDevice device, VkPipeline pipeline, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyPipelineLayout.html">vkDestroyPipelineLayout</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyPipelineLayout(VkDevice device, VkPipelineLayout pipelineLayout, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyRenderPass.html">vkDestroyRenderPass</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyRenderPass(VkDevice device, VkRenderPass renderPass, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroySampler.html">vkDestroySampler</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroySampler(VkDevice device, VkSampler sampler, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroySemaphore.html">vkDestroySemaphore</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroySemaphore(VkDevice device, VkSemaphore semaphore, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyShaderModule.html">vkDestroyShaderModule</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyShaderModule(VkDevice device, VkShaderModule shaderModule, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDeviceWaitIdle.html">vkDeviceWaitIdle</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkDeviceWaitIdle(VkDevice device);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkEndCommandBuffer.html">vkEndCommandBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkEndCommandBuffer(VkCommandBuffer commandBuffer);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkEnumerateDeviceExtensionProperties.html">vkEnumerateDeviceExtensionProperties</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkEnumerateInstanceExtensionProperties.html">vkEnumerateInstanceExtensionProperties</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkEnumerateInstanceExtensionProperties(byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkEnumerateInstanceLayerProperties.html">vkEnumerateInstanceLayerProperties</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkEnumerateInstanceLayerProperties(uint* pPropertyCount, VkLayerProperties* pProperties);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkEnumeratePhysicalDevices.html">vkEnumeratePhysicalDevices</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkEnumeratePhysicalDevices(VkInstance instance, uint* pPhysicalDeviceCount, VkPhysicalDevice* pPhysicalDevices);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkFreeCommandBuffers.html">vkFreeCommandBuffers</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkFreeCommandBuffers(VkDevice device, VkCommandPool commandPool, uint commandBufferCount, VkCommandBuffer* pCommandBuffers);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkFreeDescriptorSets.html">vkFreeDescriptorSets</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkFreeDescriptorSets(VkDevice device, VkDescriptorPool descriptorPool, uint descriptorSetCount, VkDescriptorSet* pDescriptorSets);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkFreeMemory.html">vkFreeMemory</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkFreeMemory(VkDevice device, VkDeviceMemory memory, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetDeviceProcAddr.html">vkGetDeviceProcAddr</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void* vkGetDeviceProcAddr(VkDevice device, byte* pName);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetDeviceQueue.html">vkGetDeviceQueue</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetDeviceQueue(VkDevice device, uint queueFamilyIndex, uint queueIndex, VkQueue* pQueue);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetInstanceProcAddr.html">vkGetInstanceProcAddr</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void* vkGetInstanceProcAddr(VkInstance instance, byte* pName);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetBufferMemoryRequirements.html">vkGetBufferMemoryRequirements</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetBufferMemoryRequirements(VkDevice device, VkBuffer buffer, VkMemoryRequirements* pMemoryRequirements);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetImageMemoryRequirements.html">vkGetImageMemoryRequirements</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetImageMemoryRequirements(VkDevice device, VkImage image, VkMemoryRequirements* pMemoryRequirements);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceFormatProperties.html">vkGetPhysicalDeviceFormatProperties</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetPhysicalDeviceFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkFormatProperties* pFormatProperties);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceFeatures.html">vkGetPhysicalDeviceFeatures</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetPhysicalDeviceFeatures(VkPhysicalDevice physicalDevice, VkPhysicalDeviceFeatures* pFeatures);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceMemoryProperties.html">vkGetPhysicalDeviceMemoryProperties</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetPhysicalDeviceMemoryProperties(VkPhysicalDevice physicalDevice, VkPhysicalDeviceMemoryProperties* pMemoryProperties);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceProperties.html">vkGetPhysicalDeviceProperties</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetPhysicalDeviceProperties(VkPhysicalDevice physicalDevice, VkPhysicalDeviceProperties* pProperties);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceQueueFamilyProperties.html">vkGetPhysicalDeviceQueueFamilyProperties</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice physicalDevice, uint* pQueueFamilyPropertyCount, VkQueueFamilyProperties* pQueueFamilyProperties);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkMapMemory.html">vkMapMemory</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkMapMemory(VkDevice device, VkDeviceMemory memory, VkDeviceSize offset, VkDeviceSize size, VkMemoryMapFlags flags, void** ppData);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkQueueWaitIdle.html">vkQueueWaitIdle</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkQueueWaitIdle(VkQueue queue);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkQueueSubmit.html">vkQueueSubmit</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkQueueSubmit(VkQueue queue, uint submitCount, VkSubmitInfo* pSubmits, VkFence fence);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkResetCommandBuffer.html">vkResetCommandBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkResetCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferResetFlags flags);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkResetFences.html">vkResetFences</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkResetFences(VkDevice device, uint fenceCount, VkFence* pFences);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkUnmapMemory.html">vkUnmapMemory</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkUnmapMemory(VkDevice device, VkDeviceMemory memory);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkUpdateDescriptorSets.html">vkUpdateDescriptorSets</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkUpdateDescriptorSets(VkDevice device, uint descriptorWriteCount, VkWriteDescriptorSet* pDescriptorWrites, uint descriptorCopyCount, VkCopyDescriptorSet* pDescriptorCopies);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkWaitForFences.html">vkWaitForFences</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkWaitForFences(VkDevice device, uint fenceCount, VkFence* pFences, VkBool32 waitAll, ulong timeout);
}
