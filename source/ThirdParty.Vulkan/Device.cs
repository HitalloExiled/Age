using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.Native;
using ThirdParty.Vulkan.TypeExtensions;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDevice.html">VkDevice</see>
/// </summary>
public unsafe partial class Device : DisposableNativeHandle
{
    private readonly HashSet<string> enabledExtensions = [];

    internal PhysicalDevice PhysicalDevice { get; }

    internal Device(PhysicalDevice physicalDevice, CreateInfo createInfo)
    {
        this.PhysicalDevice    = physicalDevice;
        this.enabledExtensions = [.. createInfo.EnabledExtensions];

        fixed (VkDevice* pValue = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateDevice(physicalDevice, createInfo, physicalDevice.Instance.Allocator, pValue));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyDevice(this.Handle, this.PhysicalDevice.Instance.Allocator);

    /// <inheritdoc cref="PInvoke.vkAllocateMemory" />
    public DeviceMemory AllocateMemory(DeviceMemory.AllocateInfo allocateInfo) =>
        new(this, allocateInfo);

    /// <inheritdoc cref="PInvoke.vkCreateBuffer" />
    public Buffer CreateBuffer(Buffer.CreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateCommandPool" />
    public CommandPool CreateCommandPool(CommandPool.CreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateCommandPool" />
    public CommandPool CreateCommandPool(uint queueFamilyIndex, CommandPoolCreateFlags flags) =>
        this.CreateCommandPool(new() { Flags = flags, QueueFamilyIndex = queueFamilyIndex });

    /// <inheritdoc cref="PInvoke.vkCreateDescriptorPool" />
    public DescriptorPool CreateDescriptorPool(DescriptorPool.CreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateFramebuffer" />
    public Framebuffer CreateFramebuffer(Framebuffer.CreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateImage" />
    public Image CreateImage(Image.CreateInfo createInfo) =>
        new(this, createInfo, false);

    /// <inheritdoc cref="PInvoke.vkCreateImageView" />
    public ImageView CreateImageView(ImageView.CreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateDescriptorSetLayout" />
    public DescriptorSetLayout CreateDescriptorSetLayout(DescriptorSetLayout.CreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateFence" />
    public Fence CreateFence(Fence.CreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateGraphicsPipelines" />
    public Pipeline CreateGraphicsPipelines(GraphicsPipeline.CreateInfo createInfo) =>
        new GraphicsPipeline(this, createInfo, null);

    /// <inheritdoc cref="PInvoke.vkCreateGraphicsPipelines" />
    public Pipeline CreateGraphicsPipelines(PipelineCache pipelineCache, GraphicsPipeline.CreateInfo createInfo) =>
        new GraphicsPipeline(this, createInfo, pipelineCache);

    /// <inheritdoc cref="PInvoke.vkCreateGraphicsPipelines" />
    public Pipeline[] CreateGraphicsPipelines(PipelineCache pipelineCache, GraphicsPipeline.CreateInfo[] createInfos)
    {
        var vkPipelines = new VkPipeline[createInfos.Length];

        fixed (VkGraphicsPipelineCreateInfo* pCreateInfo = createInfos.ToNatives())
        fixed (VkPipeline* pPipelines = vkPipelines)
        {
            VulkanException.Check(PInvoke.vkCreateGraphicsPipelines(this, pipelineCache, (uint)createInfos.Length, pCreateInfo, this.PhysicalDevice.Instance.Allocator, pPipelines));
        }

        var pipelines = new Pipeline[vkPipelines.Length];

        for (var i = 0; i < vkPipelines.Length; i++)
        {
            pipelines[i] = new GraphicsPipeline(vkPipelines[i], this);
        }

        return pipelines;
    }

    /// <inheritdoc cref="PInvoke.vkCreatePipelineLayout" />
    public PipelineLayout CreatePipelineLayout(PipelineLayout.CreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateRenderPass" />
    public RenderPass CreateRenderPass(RenderPass.CreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateSampler" />
    public Sampler CreateSampler(Sampler.CreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateSemaphore" />
    public Semaphore CreateSemaphore(Semaphore.CreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkCreateShaderModule" />
    public ShaderModule CreateShaderModule(ShaderModule.CreateInfo createInfo) =>
        new(this, createInfo);


    /// <inheritdoc cref="PInvoke.vkGetDeviceProcAddr" />
    public T GetProcAddr<T>(string name) where T : Delegate
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            return Marshal.GetDelegateForFunctionPointer<T>((nint)PInvoke.vkGetDeviceProcAddr(this, pName));
        }
    }

    /// <inheritdoc cref="PInvoke.vkGetDeviceQueue" />
    public Queue GetQueue(uint familyIndex, uint index) =>
        new(this, familyIndex, index);

    public bool TryGetExtension<T>([NotNullWhen(true)] out T? extension) where T : IDeviceExtension<T>
    {
        extension = this.enabledExtensions.Contains(T.Name) ? T.Create(this) : default;

        return extension != null;
    }

    /// <inheritdoc cref="PInvoke.vkUpdateDescriptorSets" />
    public void UpdateDescriptorSets(WriteDescriptorSet[] descriptorWrites, CopyDescriptorSet[] descriptorCopies)
    {
        fixed (VkWriteDescriptorSet* pDescriptorWrites = descriptorWrites.ToNatives())
        fixed (VkCopyDescriptorSet*  pDescriptorCopies = descriptorCopies.ToNatives())
        {
            PInvoke.vkUpdateDescriptorSets(this, (uint)descriptorWrites.Length, pDescriptorWrites, (uint)descriptorCopies.Length, pDescriptorCopies);
        }
    }


    /// <inheritdoc cref="PInvoke.vkDeviceWaitIdle" />
    public void WaitIdle() =>
        VulkanException.Check(PInvoke.vkDeviceWaitIdle(this));
}
