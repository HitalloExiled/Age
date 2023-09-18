using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
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
    /// The length in char values of an array containing a physical device name string, as returned in <see cref="VkPhysicalDeviceProperties.deviceName"/>.
    /// </summary>
    public const uint VK_MAX_PHYSICAL_DEVICE_NAME_SIZE = 256;

    /// <summary>
    /// The length in byte values of an array containing a universally unique device or driver build identifier, as returned in <see cref="VkPhysicalDeviceIDProperties.deviceUUID"/> and <see cref="VkPhysicalDeviceIDProperties.driverUUID"/>.
    /// </summary>
    public const uint VK_UUID_SIZE = 16;

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkAllocateCommandBuffers(VkDevice device, VkCommandBufferAllocateInfo* pAllocateInfo, VkCommandBuffer* pCommandBuffers);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkBeginCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferBeginInfo* pBeginInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdBeginRenderPass(VkCommandBuffer commandBuffer, VkRenderPassBeginInfo* pRenderPassBegin, VkSubpassContents contents);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdBindPipeline(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipeline pipeline);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdDraw(VkCommandBuffer commandBuffer, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdEndRenderPass(VkCommandBuffer commandBuffer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdSetScissor(VkCommandBuffer commandBuffer, uint firstScissor, uint scissorCount, VkRect2D* pScissors);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkCmdSetViewport(VkCommandBuffer commandBuffer, uint firstViewport, uint viewportCount, VkViewport* pViewports);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateCommandPool(VkDevice device, VkCommandPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkCommandPool* pCommandPool);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateDevice(VkPhysicalDevice physicalDevice, VkDeviceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDevice* pDevice);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateFence(VkDevice device, VkFenceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateFramebuffer(VkDevice device, VkFramebufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkFramebuffer* pFramebuffer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, VkGraphicsPipelineCreateInfo* pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateImageView(VkDevice device, VkImageViewCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkImageView* pView);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreatePipelineLayout(VkDevice device, VkPipelineLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkPipelineLayout* pPipelineLayout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateRenderPass(VkDevice device, VkRenderPassCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkRenderPass* pRenderPass);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateSemaphore(VkDevice device, VkSemaphoreCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSemaphore* pSemaphore);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateShaderModule(VkDevice device, VkShaderModuleCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkShaderModule* pShaderModule);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyCommandPool(VkDevice device, VkCommandPool commandPool, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyDevice(VkDevice device, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyFence(VkDevice device, VkFence fence, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyFramebuffer(VkDevice device, VkFramebuffer framebuffer, VkAllocationCallbacks* pAllocator);

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
    private delegate void VkDestroySemaphore(VkDevice device, VkSemaphore semaphore, VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void VkDestroyShaderModule(VkDevice device, VkShaderModule shaderModule, VkAllocationCallbacks* pAllocator);

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

    private readonly VkAllocateCommandBuffers                 vkAllocateCommandBuffers                 = loader.Load<VkAllocateCommandBuffers>("vkAllocateCommandBuffers");
    private readonly VkBeginCommandBuffer                     vkBeginCommandBuffer                     = loader.Load<VkBeginCommandBuffer>("vkBeginCommandBuffer");
    private readonly VkCmdBeginRenderPass                     vkCmdBeginRenderPass                     = loader.Load<VkCmdBeginRenderPass>("vkCmdBeginRenderPass");
    private readonly VkCmdBindPipeline                        vkCmdBindPipeline                        = loader.Load<VkCmdBindPipeline>("vkCmdBindPipeline");
    private readonly VkCmdDraw                                vkCmdDraw                                = loader.Load<VkCmdDraw>("vkCmdDraw");
    private readonly VkCmdEndRenderPass                       vkCmdEndRenderPass                       = loader.Load<VkCmdEndRenderPass>("vkCmdEndRenderPass");
    private readonly VkCmdSetScissor                          vkCmdSetScissor                          = loader.Load<VkCmdSetScissor>("vkCmdSetScissor");
    private readonly VkCmdSetViewport                         vkCmdSetViewport                         = loader.Load<VkCmdSetViewport>("vkCmdSetViewport");
    private readonly VkCreateCommandPool                      vkCreateCommandPool                      = loader.Load<VkCreateCommandPool>("vkCreateCommandPool");
    private readonly VkCreateDevice                           vkCreateDevice                           = loader.Load<VkCreateDevice>("vkCreateDevice");
    private readonly VkCreateFence                            vkCreateFence                            = loader.Load<VkCreateFence>("vkCreateFence");
    private readonly VkCreateFramebuffer                      vkCreateFramebuffer                      = loader.Load<VkCreateFramebuffer>("vkCreateFramebuffer");
    private readonly VkCreateGraphicsPipelines                vkCreateGraphicsPipelines                = loader.Load<VkCreateGraphicsPipelines>("vkCreateGraphicsPipelines");
    private readonly VkCreateImageView                        vkCreateImageView                        = loader.Load<VkCreateImageView>("vkCreateImageView");
    private readonly VkCreateInstance                         vkCreateInstance                         = loader.Load<VkCreateInstance>("vkCreateInstance");
    private readonly VkCreatePipelineLayout                   vkCreatePipelineLayout                   = loader.Load<VkCreatePipelineLayout>("vkCreatePipelineLayout");
    private readonly VkCreateRenderPass                       vkCreateRenderPass                       = loader.Load<VkCreateRenderPass>("vkCreateRenderPass");
    private readonly VkCreateSemaphore                        vkCreateSemaphore                        = loader.Load<VkCreateSemaphore>("vkCreateSemaphore");
    private readonly VkCreateShaderModule                     vkCreateShaderModule                     = loader.Load<VkCreateShaderModule>("vkCreateShaderModule");
    private readonly VkDestroyCommandPool                     vkDestroyCommandPool                     = loader.Load<VkDestroyCommandPool>("vkDestroyCommandPool");
    private readonly VkDestroyDevice                          vkDestroyDevice                          = loader.Load<VkDestroyDevice>("vkDestroyDevice");
    private readonly VkDestroyFence                           vkDestroyFence                           = loader.Load<VkDestroyFence>("vkDestroyFence");
    private readonly VkDestroyFramebuffer                     vkDestroyFramebuffer                     = loader.Load<VkDestroyFramebuffer>("vkDestroyFramebuffer");
    private readonly VkDestroyImageView                       vkDestroyImageView                       = loader.Load<VkDestroyImageView>("vkDestroyImageView");
    private readonly VkDestroyInstance                        vkDestroyInstance                        = loader.Load<VkDestroyInstance>("vkDestroyInstance");
    private readonly VkDestroyPipeline                        vkDestroyPipeline                        = loader.Load<VkDestroyPipeline>("vkDestroyPipeline");
    private readonly VkDestroyPipelineLayout                  vkDestroyPipelineLayout                  = loader.Load<VkDestroyPipelineLayout>("vkDestroyPipelineLayout");
    private readonly VkDestroyRenderPass                      vkDestroyRenderPass                      = loader.Load<VkDestroyRenderPass>("vkDestroyRenderPass");
    private readonly VkDestroySemaphore                       vkDestroySemaphore                       = loader.Load<VkDestroySemaphore>("vkDestroySemaphore");
    private readonly VkDestroyShaderModule                    vkDestroyShaderModule                    = loader.Load<VkDestroyShaderModule>("vkDestroyShaderModule");
    private readonly VkEndCommandBuffer                       vkEndCommandBuffer                       = loader.Load<VkEndCommandBuffer>("vkEndCommandBuffer");
    private readonly VkEnumerateDeviceExtensionProperties     vkEnumerateDeviceExtensionProperties     = loader.Load<VkEnumerateDeviceExtensionProperties>("vkEnumerateDeviceExtensionProperties");
    private readonly VkEnumerateInstanceExtensionProperties   vkEnumerateInstanceExtensionProperties   = loader.Load<VkEnumerateInstanceExtensionProperties>("vkEnumerateInstanceExtensionProperties");
    private readonly VkEnumerateInstanceLayerProperties       vkEnumerateInstanceLayerProperties       = loader.Load<VkEnumerateInstanceLayerProperties>("vkEnumerateInstanceLayerProperties");
    private readonly VkEnumeratePhysicalDevices               vkEnumeratePhysicalDevices               = loader.Load<VkEnumeratePhysicalDevices>("vkEnumeratePhysicalDevices");
    private readonly VkGetDeviceProcAddr                      vkGetDeviceProcAddr                      = loader.Load<VkGetDeviceProcAddr>("vkGetDeviceProcAddr");
    private readonly VkGetDeviceQueue                         vkGetDeviceQueue                         = loader.Load<VkGetDeviceQueue>("vkGetDeviceQueue");
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

    public VkResult AllocateCommandBuffers(VkDevice device, in VkCommandBufferAllocateInfo allocateInfo, out VkCommandBuffer commandBuffers)
    {
        fixed (VkCommandBufferAllocateInfo* pAllocateInfo   = &allocateInfo)
        fixed (VkCommandBuffer*             pCommandBuffers = &commandBuffers)
        {
            return this.vkAllocateCommandBuffers.Invoke(device, pAllocateInfo, pCommandBuffers);
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

    public VkResult BeginCommandBuffer(VkCommandBuffer commandBuffer, ref VkCommandBufferBeginInfo beginInfo)
    {
        fixed (VkCommandBufferBeginInfo* pBeginInfo = &beginInfo)
        {
            return this.vkBeginCommandBuffer.Invoke(commandBuffer, pBeginInfo);
        }
    }

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
    /// <para>Bind a pipeline object to a command buffer.</para>
    /// <para>Once bound, a pipeline binding affects subsequent commands that interact with the given pipeline type in the command buffer until a different pipeline of the same type is bound to the bind point, or until the pipeline bind point is disturbed by binding a <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#shaders-objects">shader object</see> as described in <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#shaders-objects-pipeline-interaction">Interaction with Pipelines</see>. Commands that do not interact with the <see href="<see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#shaders-binding">given pipeline</see>">given pipeline</see> type must not be affected by the pipeline state.</para>
    /// </summary>
    /// <param name="commandBuffer">The command buffer that the pipeline will be bound to.</param>
    /// <param name="pipelineBindPoint">A VkPipelineBindPoint value specifying to which bind point the pipeline is bound. Binding one does not disturb the others.</param>
    /// <param name="pipeline">The pipeline to be bound.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdBindPipeline(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipeline pipeline) =>
        this.vkCmdBindPipeline.Invoke(commandBuffer, pipelineBindPoint, pipeline);

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

    /// <summary>
    /// <para>End the current render pass.</para>
    /// <para>Ending a render pass instance performs any multisample resolve operations on the final subpass.</para>
    /// </summary>
    /// <param name="commandBuffer">The command buffer in which to end the current render pass instance.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public void CmdEndRenderPass(VkCommandBuffer commandBuffer) =>
        this.vkCmdEndRenderPass.Invoke(commandBuffer);

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

    public void CmdSetScissor(VkCommandBuffer commandBuffer, uint firstScissor, uint scissorCount, in VkRect2D scissors)
    {
        fixed (VkRect2D* pScissors = &scissors)
        {
            this.vkCmdSetScissor.Invoke(commandBuffer, firstScissor, scissorCount, pScissors);
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

    public void CmdSetViewport(VkCommandBuffer commandBuffer, uint firstViewport, uint viewportCount, in VkViewport viewports)
    {
        fixed (VkViewport* pViewports = &viewports)
        {
            this.vkCmdSetViewport.Invoke(commandBuffer, firstViewport, viewportCount, pViewports);
        }
    }

    /// <summary>
    /// Create a new command pool object.
    /// </summary>
    /// <param name="device">the logical device that creates the command pool.</param>
    /// <param name="pCreateInfo">a pointer to a VkCommandPoolCreateInfo structure specifying the state of the command pool object.</param>
    /// <param name="pAllocator">controls host memory allocation as described in the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see> chapter.</param>
    /// <param name="pCommandPool">a pointer to a <see cref="VkCommandPool"/> handle in which the created pool is returned.</param>
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

    public VkResult CreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, uint createInfoCount, in VkGraphicsPipelineCreateInfo createInfos, in VkAllocationCallbacks allocator, out VkPipeline pipelines)
    {
        fixed (VkGraphicsPipelineCreateInfo* pCreateInfos = &createInfos)
        fixed (VkAllocationCallbacks*        pAllocator   = &allocator)
        fixed (VkPipeline*                   pPipelines   = &pipelines)
        {
            return this.vkCreateGraphicsPipelines.Invoke(device, pipelineCache, createInfoCount, pCreateInfos, NullIfDefault(allocator, pAllocator), pPipelines);
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
            this.vkDestroyFence.Invoke(device, fence, pAllocator);
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
            properties = Array.Empty<VkExtensionProperties>();

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
            : throw new Exception($"Cant find extension {T.Name}");

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
    public VkResult QueueSubmit(VkQueue queue, uint submitCount, VkSubmitInfo* pSubmits, VkFence fence) => throw new NotImplementedException();

    /// <summary>
    /// Reset a command buffer to the initial state.
    /// </summary>
    /// <param name="commandBuffer">The command buffer to reset. The command buffer can be in any state other than <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">pending</see>, and is moved into the <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#commandbuffers-lifecycle">initial state</see>.</param>
    /// <param name="flags">A bitmask of <see cref="VkCommandBufferResetFlagBits"/> controlling the reset operation.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult ResetCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferResetFlags flags) => throw new NotImplementedException();

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
    public VkResult ResetFences(VkDevice device, uint fenceCount, VkFence* pFences) => throw new NotImplementedException();

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
    /// <param name="waitAll">The condition that must be satisfied to successfully unblock the wait. If waitAll is VK_TRUE, then the condition is that all fences in pFences are signaled. Otherwise, the condition is that at least one fence in pFences is signaled.</param>
    /// <param name="timeout">The timeout period in units of nanoseconds. timeout is adjusted to the closest value allowed by the implementation-dependent timeout accuracy, which may be substantially longer than one nanosecond, and may be longer than the requested period.</param>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public VkResult WaitForFences(VkDevice device, uint fenceCount, VkFence* pFences, VkBool32 waitAll, ulong timeout) => throw new NotImplementedException();
}
