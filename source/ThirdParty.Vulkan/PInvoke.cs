using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

internal unsafe static partial class PInvoke
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkAllocateCommandBuffers.html">vkAllocateCommandBuffers</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkAllocateCommandBuffers(VkHandle<VkDevice> device, VkCommandBufferAllocateInfo* pAllocateInfo, VkHandle<VkCommandBuffer>* pCommandBuffers);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkAllocateDescriptorSets.html">vkAllocateDescriptorSets</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkAllocateDescriptorSets(VkHandle<VkDevice> device, VkDescriptorSetAllocateInfo* pAllocateInfo, VkHandle<VkDescriptorSet>* pDescriptorSets);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkAllocateMemory.html">vkAllocateMemory</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkAllocateMemory(VkHandle<VkDevice> device, VkMemoryAllocateInfo* pAllocateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkDeviceMemory>* pMemory);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkBeginCommandBuffer.html">vkBeginCommandBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkBeginCommandBuffer(VkHandle<VkCommandBuffer> commandBuffer, VkCommandBufferBeginInfo* pBeginInfo);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkBindBufferMemory.html">vkBindBufferMemory</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkBindBufferMemory(VkHandle<VkDevice> device, VkHandle<VkBuffer> buffer, VkHandle<VkDeviceMemory> memory, VkDeviceSize memoryOffset);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkBindImageMemory.html">vkBindImageMemory</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkBindImageMemory(VkHandle<VkDevice> device, VkHandle<VkImage> image, VkHandle<VkDeviceMemory> memory, VkDeviceSize memoryOffset);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdBeginRenderPass.html">vkCmdBeginRenderPass</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdBeginRenderPass(VkHandle<VkCommandBuffer> commandBuffer, VkRenderPassBeginInfo* pRenderPassBegin, VkSubpassContents contents);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdBindDescriptorSets.html">vkCmdBindDescriptorSets</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdBindDescriptorSets(VkHandle<VkCommandBuffer> commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkHandle<VkPipelineLayout> layout, uint firstSet, uint descriptorSetCount, VkHandle<VkDescriptorSet>* pDescriptorSets, uint dynamicOffsetCount, uint* pDynamicOffsets);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdBindPipeline.html">vkCmdBindPipeline</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdBindPipeline(VkHandle<VkCommandBuffer> commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkHandle<VkPipeline> pipeline);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdBindIndexBuffer.html">vkCmdBindIndexBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdBindIndexBuffer(VkHandle<VkCommandBuffer> commandBuffer, VkHandle<VkBuffer> buffer, VkDeviceSize offset, VkIndexType indexType);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdBindVertexBuffers.html">vkCmdBindVertexBuffers</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdBindVertexBuffers(VkHandle<VkCommandBuffer> commandBuffer, uint firstBinding, uint bindingCount, VkHandle<VkBuffer>* pBuffers, VkDeviceSize* pOffsets);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdBlitImage.html">vkCmdBlitImage</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdBlitImage(VkHandle<VkCommandBuffer> commandBuffer, VkHandle<VkImage> srcImage, VkImageLayout srcImageLayout, VkHandle<VkImage> dstImage, VkImageLayout dstImageLayout, uint regionCount, VkImageBlit* pRegions, VkFilter filter);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdClearColorImage.html">vkCmdClearColorImage</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdClearColorImage(VkHandle<VkCommandBuffer> commandBuffer, VkHandle<VkImage> image, VkImageLayout imageLayout, VkClearColorValue* pColor, uint rangeCount, VkImageSubresourceRange* pRanges);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdCopyBuffer.html">vkCmdCopyBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdCopyBuffer(VkHandle<VkCommandBuffer> commandBuffer, VkHandle<VkBuffer> srcBuffer, VkHandle<VkBuffer> dstBuffer, uint regionCount, VkBufferCopy* pRegions);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdCopyBufferToImage.html">vkCmdCopyBufferToImage</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdCopyBufferToImage(VkHandle<VkCommandBuffer> commandBuffer, VkHandle<VkBuffer> srcBuffer, VkHandle<VkImage> dstImage, VkImageLayout dstImageLayout, uint regionCount, VkBufferImageCopy* pRegions);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdDraw.html">vkCmdDraw</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdDraw(VkHandle<VkCommandBuffer> commandBuffer, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdDrawIndexed.html">vkCmdDrawIndexed</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdDrawIndexed(VkHandle<VkCommandBuffer> commandBuffer, uint indexCount, uint instanceCount, uint firstIndex, int vertexOffset, uint firstInstance);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdEndRenderPass.html">vkCmdEndRenderPass</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdEndRenderPass(VkHandle<VkCommandBuffer> commandBuffer);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdPipelineBarrier.html">vkCmdPipelineBarrier</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdPipelineBarrier(VkHandle<VkCommandBuffer> commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, uint imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdSetScissor.html">vkCmdSetScissor</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdSetScissor(VkHandle<VkCommandBuffer> commandBuffer, uint firstScissor, uint scissorCount, VkRect2D* pScissors);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCmdSetViewport.html">vkCmdSetViewport</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkCmdSetViewport(VkHandle<VkCommandBuffer> commandBuffer, uint firstViewport, uint viewportCount, VkViewport* pViewports);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateBuffer.html">vkCreateBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateBuffer(VkHandle<VkDevice> device, VkBufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkBuffer>* pBuffer);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateCommandPool.html">vkCreateCommandPool</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateCommandPool(VkHandle<VkDevice> device, VkCommandPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkCommandPool>* pCommandPool);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateDescriptorPool.html">vkCreateDescriptorPool</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateDescriptorPool(VkHandle<VkDevice> device, VkDescriptorPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkDescriptorPool>* pDescriptorPool);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateDescriptorSetLayout.html">vkCreateDescriptorSetLayout</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateDescriptorSetLayout(VkHandle<VkDevice> device, VkDescriptorSetLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkDescriptorSetLayout>* pSetLayout);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateDevice.html">vkCreateDevice</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateDevice(VkHandle<VkPhysicalDevice> physicalDevice, VkDeviceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkDevice>* pDevice);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateFence.html">vkCreateFence</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateFence(VkHandle<VkDevice> device, VkFenceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkFence>* pFence);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateFramebuffer.html">vkCreateFramebuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateFramebuffer(VkHandle<VkDevice> device, VkFramebufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkFramebuffer>* pFramebuffer);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateGraphicsPipelines.html">vkCreateGraphicsPipelines</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateGraphicsPipelines(VkHandle<VkDevice> device, VkHandle<VkPipelineCache> pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo* pCreateInfos, VkAllocationCallbacks* pAllocator, VkHandle<VkPipeline>* pPipelines);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateImage.html">vkCreateImage</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateImage(VkHandle<VkDevice> device, VkImageCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkImage>* pImage);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateImageView.html">vkCreateImageView</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateImageView(VkHandle<VkDevice> device, VkImageViewCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkImageView>* pView);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateInstance.html">vkCreateInstance</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkInstance>* pInstance);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreatePipelineLayout.html">vkCreatePipelineLayout</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreatePipelineLayout(VkHandle<VkDevice> device, VkPipelineLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkPipelineLayout>* pPipelineLayout);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateRenderPass.html">vkCreateRenderPass</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateRenderPass(VkHandle<VkDevice> device, VkRenderPassCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkRenderPass>* pRenderPass);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateSampler.html">vkCreateSampler</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateSampler(VkHandle<VkDevice> device, VkSamplerCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkSampler>* pSampler);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateSemaphore.html">vkCreateSemaphore</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateSemaphore(VkHandle<VkDevice> device, VkSemaphoreCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkSemaphore>* pSemaphore);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkCreateShaderModule.html">vkCreateShaderModule</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkCreateShaderModule(VkHandle<VkDevice> device, VkShaderModuleCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkHandle<VkShaderModule>* pShaderModule);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyBuffer.html">vkDestroyBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyBuffer(VkHandle<VkDevice> device, VkHandle<VkBuffer> buffer, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyCommandPool.html">vkDestroyCommandPool</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyCommandPool(VkHandle<VkDevice> device, VkHandle<VkCommandPool> commandPool, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyDescriptorPool.html">vkDestroyDescriptorPool</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyDescriptorPool(VkHandle<VkDevice> device, VkHandle<VkDescriptorPool> descriptorPool, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyDescriptorSetLayout.html">vkDestroyDescriptorSetLayout</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyDescriptorSetLayout(VkHandle<VkDevice> device, VkHandle<VkDescriptorSetLayout> descriptorSetLayout, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyDevice.html">vkDestroyDevice</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyDevice(VkHandle<VkDevice> device, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyFence.html">vkDestroyFence</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyFence(VkHandle<VkDevice> device, VkHandle<VkFence> fence, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyFramebuffer.html">vkDestroyFramebuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyFramebuffer(VkHandle<VkDevice> device, VkHandle<VkFramebuffer> framebuffer, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyImage.html">vkDestroyImage</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyImage(VkHandle<VkDevice> device, VkHandle<VkImage> image, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyImageView.html">vkDestroyImageView</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyImageView(VkHandle<VkDevice> device, VkHandle<VkImageView> imageView, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyInstance.html">vkDestroyInstance</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyInstance(VkHandle<VkInstance> instance, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyPipeline.html">vkDestroyPipeline</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyPipeline(VkHandle<VkDevice> device, VkHandle<VkPipeline> pipeline, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyPipelineLayout.html">vkDestroyPipelineLayout</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyPipelineLayout(VkHandle<VkDevice> device, VkHandle<VkPipelineLayout> pipelineLayout, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyRenderPass.html">vkDestroyRenderPass</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyRenderPass(VkHandle<VkDevice> device, VkHandle<VkRenderPass> renderPass, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroySampler.html">vkDestroySampler</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroySampler(VkHandle<VkDevice> device, VkHandle<VkSampler> sampler, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroySemaphore.html">vkDestroySemaphore</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroySemaphore(VkHandle<VkDevice> device, VkHandle<VkSemaphore> semaphore, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDestroyShaderModule.html">vkDestroyShaderModule</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkDestroyShaderModule(VkHandle<VkDevice> device, VkHandle<VkShaderModule> shaderModule, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkDeviceWaitIdle.html">vkDeviceWaitIdle</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkDeviceWaitIdle(VkHandle<VkDevice> device);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkEndCommandBuffer.html">vkEndCommandBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkEndCommandBuffer(VkHandle<VkCommandBuffer> commandBuffer);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkEnumerateDeviceExtensionProperties.html">vkEnumerateDeviceExtensionProperties</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkEnumerateDeviceExtensionProperties(VkHandle<VkPhysicalDevice> physicalDevice, byte* pLayerName, uint* pPropertyCount, VkExtensionProperties* pProperties);

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
    public static partial VkResult vkEnumeratePhysicalDevices(nint instance, uint* pPhysicalDeviceCount, VkHandle<VkPhysicalDevice>* pPhysicalDevices);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkFreeCommandBuffers.html">vkFreeCommandBuffers</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkFreeCommandBuffers(VkHandle<VkDevice> device, VkHandle<VkCommandPool> commandPool, uint commandBufferCount, VkHandle<VkCommandBuffer>* pCommandBuffers);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkFreeDescriptorSets.html">vkFreeDescriptorSets</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkFreeDescriptorSets(VkHandle<VkDevice> device, VkHandle<VkDescriptorPool> descriptorPool, uint descriptorSetCount, VkHandle<VkDescriptorSet>* pDescriptorSets);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkFreeMemory.html">vkFreeMemory</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkFreeMemory(VkHandle<VkDevice> device, VkHandle<VkDeviceMemory> memory, VkAllocationCallbacks* pAllocator);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetDeviceProcAddr.html">vkGetDeviceProcAddr</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void* vkGetDeviceProcAddr(VkHandle<VkDevice> device, byte* pName);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetDeviceQueue.html">vkGetDeviceQueue</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetDeviceQueue(VkHandle<VkDevice> device, uint queueFamilyIndex, uint queueIndex, VkHandle<VkQueue>* pQueue);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetInstanceProcAddr.html">vkGetInstanceProcAddr</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void* vkGetInstanceProcAddr(VkHandle<VkInstance> instance, byte* pName);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetBufferMemoryRequirements.html">vkGetBufferMemoryRequirements</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetBufferMemoryRequirements(VkHandle<VkDevice> device, VkHandle<VkBuffer> buffer, VkMemoryRequirements* pMemoryRequirements);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetImageMemoryRequirements.html">vkGetImageMemoryRequirements</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetImageMemoryRequirements(VkHandle<VkDevice> device, VkHandle<VkImage> image, VkMemoryRequirements* pMemoryRequirements);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceFormatProperties.html">vkGetPhysicalDeviceFormatProperties</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetPhysicalDeviceFormatProperties(VkHandle<VkPhysicalDevice> physicalDevice, VkFormat format, VkFormatProperties* pFormatProperties);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceFeatures.html">vkGetPhysicalDeviceFeatures</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetPhysicalDeviceFeatures(VkHandle<VkPhysicalDevice> physicalDevice, VkPhysicalDeviceFeatures* pFeatures);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceMemoryProperties.html">vkGetPhysicalDeviceMemoryProperties</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetPhysicalDeviceMemoryProperties(VkHandle<VkPhysicalDevice> physicalDevice, VkPhysicalDeviceMemoryProperties* pMemoryProperties);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceProperties.html">vkGetPhysicalDeviceProperties</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetPhysicalDeviceProperties(VkHandle<VkPhysicalDevice> physicalDevice, VkPhysicalDeviceProperties* pProperties);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkGetPhysicalDeviceQueueFamilyProperties.html">vkGetPhysicalDeviceQueueFamilyProperties</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkGetPhysicalDeviceQueueFamilyProperties(VkHandle<VkPhysicalDevice> physicalDevice, uint* pQueueFamilyPropertyCount, VkQueueFamilyProperties* pQueueFamilyProperties);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkMapMemory.html">vkMapMemory</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkMapMemory(VkHandle<VkDevice> device, VkHandle<VkDeviceMemory> memory, VkDeviceSize offset, VkDeviceSize size, VkMemoryMapFlags flags, void** ppData);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkQueueWaitIdle.html">vkQueueWaitIdle</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkQueueWaitIdle(VkHandle<VkQueue> queue);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkQueueSubmit.html">vkQueueSubmit</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkQueueSubmit(VkHandle<VkQueue> queue, uint submitCount, VkSubmitInfo* pSubmits, VkHandle<VkFence> fence);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkResetCommandBuffer.html">vkResetCommandBuffer</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkResetCommandBuffer(VkHandle<VkCommandBuffer> commandBuffer, VkCommandBufferResetFlags flags);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkResetFences.html">vkResetFences</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkResetFences(VkHandle<VkDevice> device, uint fenceCount, VkHandle<VkFence>* pFences);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkUnmapMemory.html">vkUnmapMemory</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkUnmapMemory(VkHandle<VkDevice> device, VkHandle<VkDeviceMemory> memory);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkUpdateDescriptorSets.html">vkUpdateDescriptorSets</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void vkUpdateDescriptorSets(VkHandle<VkDevice> device, uint descriptorWriteCount, VkWriteDescriptorSet* pDescriptorWrites, uint descriptorCopyCount, VkCopyDescriptorSet* pDescriptorCopies);

    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/vkWaitForFences.html">vkWaitForFences</see>
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial VkResult vkWaitForFences(VkHandle<VkDevice> device, uint fenceCount, VkHandle<VkFence>* pFences, VkBool32 waitAll, ulong timeout);
}
