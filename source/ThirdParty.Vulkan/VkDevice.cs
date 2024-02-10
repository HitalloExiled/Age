using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Age.Core.Interop;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Interfaces;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDevice.html">VkDevice</see>
/// </summary>
public unsafe partial class VkDevice : DisposableManagedHandle<VkDevice>
{
    private readonly HashSet<string> enabledExtensions = [];

    internal VkPhysicalDevice PhysicalDevice { get; }
    internal VkInstance       Instance       => this.PhysicalDevice.Instance;

    internal VkDevice(VkPhysicalDevice physicalDevice, in VkDeviceCreateInfo createInfo)
    {
        this.PhysicalDevice    = physicalDevice;
        this.enabledExtensions = [.. PointerHelper.ToArray(createInfo.PpEnabledExtensionNames, createInfo.EnabledExtensionCount)];

        fixed (VkHandle<VkDevice>*    pHandle     = &this.handle)
        fixed (VkDeviceCreateInfo*    pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateDevice(physicalDevice.Handle, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyDevice(this.handle, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }

    /// <inheritdoc cref="PInvoke.vkAllocateMemory" />
    public VkDeviceMemory AllocateMemory(in VkMemoryAllocateInfo allocateInfo) =>
        new(this, allocateInfo);

    /// <inheritdoc cref="PInvoke.vkCreateBuffer" />
    public VkBuffer CreateBuffer(in VkBufferCreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateCommandPool" />
    public VkCommandPool CreateCommandPool(in VkCommandPoolCreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateCommandPool" />
    public VkCommandPool CreateCommandPool(uint queueFamilyIndex, VkCommandPoolCreateFlags flags) =>
        this.CreateCommandPool(new() { Flags = flags, QueueFamilyIndex = queueFamilyIndex });

    /// <inheritdoc cref="PInvoke.vkCreateDescriptorPool" />
    public VkDescriptorPool CreateDescriptorPool(VkDescriptorPoolCreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateFramebuffer" />
    public VkFramebuffer CreateFramebuffer(VkFramebufferCreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateImage" />
    public VkImage CreateImage(VkImageCreateInfo createInfo) =>
        new(this, createInfo, false);

    /// <inheritdoc cref="PInvoke.vkCreateImageView" />
    public VkImageView CreateImageView(VkImageViewCreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateDescriptorSetLayout" />
    public VkDescriptorSetLayout CreateDescriptorSetLayout(VkDescriptorSetLayoutCreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateFence" />
    public VkFence CreateFence(VkFenceCreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateGraphicsPipelines" />
    public VkPipeline CreateGraphicsPipelines(VkGraphicsPipelineCreateInfo createInfo) =>
        new VkGraphicsPipeline(this, createInfo, null);

    /// <inheritdoc cref="PInvoke.vkCreateGraphicsPipelines" />
    public VkPipeline CreateGraphicsPipelines(VkPipelineCache pipelineCache, VkGraphicsPipelineCreateInfo createInfo) =>
        new VkGraphicsPipeline(this, createInfo, pipelineCache);

    /// <inheritdoc cref="PInvoke.vkCreateGraphicsPipelines" />
    public VkPipeline[] CreateGraphicsPipelines(VkPipelineCache pipelineCache, VkGraphicsPipelineCreateInfo[] createInfos)
    {
        var vkPipelines = new VkHandle<VkPipeline>[createInfos.Length];

        fixed (VkGraphicsPipelineCreateInfo* pCreateInfo = createInfos)
        fixed (VkHandle<VkPipeline>*         pPipelines  = vkPipelines)
        fixed (VkAllocationCallbacks*        pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateGraphicsPipelines(this.handle, pipelineCache.Handle, (uint)createInfos.Length, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pPipelines));
        }

        var pipelines = new VkPipeline[vkPipelines.Length];

        for (var i = 0; i < vkPipelines.Length; i++)
        {
            pipelines[i] = new VkGraphicsPipeline(vkPipelines[i], this);
        }

        return pipelines;
    }

    /// <inheritdoc cref="PInvoke.vkCreatePipelineLayout" />
    public VkPipelineLayout CreatePipelineLayout(in VkPipelineLayoutCreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateRenderPass" />
    public VkRenderPass CreateRenderPass(in VkRenderPassCreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateSampler" />
    public VkSampler CreateSampler(in VkSamplerCreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateSemaphore" />
    public VkSemaphore CreateSemaphore() =>
        new(this, new());

    /// <inheritdoc cref="PInvoke.vkCreateSemaphore" />
    public VkSemaphore CreateSemaphore(in VkSemaphoreCreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateShaderModule" />
    public VkShaderModule CreateShaderModule(in VkShaderModuleCreateInfo createInfo) =>
        new(this, createInfo);

    public T GetExtension<T>() where T : IDeviceExtension<T> =>
        this.TryGetExtension<T>(out var extension) ? extension : throw new InvalidOperationException($"Can't load required extension {T.Name}");


    /// <inheritdoc cref="PInvoke.vkGetDeviceProcAddr" />
    public T GetProcAddr<T>(string name) where T : Delegate
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            return Marshal.GetDelegateForFunctionPointer<T>((nint)PInvoke.vkGetDeviceProcAddr(this.handle, pName));
        }
    }

    /// <inheritdoc cref="PInvoke.vkGetDeviceQueue" />
    public VkQueue GetQueue(uint familyIndex, uint index) =>
        new(this, familyIndex, index);

    public bool TryGetExtension<T>([NotNullWhen(true)] out T? extension) where T : IDeviceExtension<T>
    {
        extension = this.enabledExtensions.Contains(T.Name) ? T.Create(this) : default;

        return extension != null;
    }

    /// <inheritdoc cref="PInvoke.vkUpdateDescriptorSets" />
    public void UpdateDescriptorSets(VkWriteDescriptorSet[] descriptorWrites, VkCopyDescriptorSet[] descriptorCopies)
    {
        fixed (VkWriteDescriptorSet* pDescriptorWrites = descriptorWrites)
        fixed (VkCopyDescriptorSet*  pDescriptorCopies = descriptorCopies)
        {
            PInvoke.vkUpdateDescriptorSets(this.handle, (uint)descriptorWrites.Length, pDescriptorWrites, (uint)descriptorCopies.Length, pDescriptorCopies);
        }
    }

    /// <inheritdoc cref="PInvoke.vkDeviceWaitIdle" />
    public void WaitIdle() =>
        VkException.Check(PInvoke.vkDeviceWaitIdle(this.handle));
}
