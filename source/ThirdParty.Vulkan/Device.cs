using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.Native;
using ThirdParty.Vulkan.TypeExtensions;

namespace ThirdParty.Vulkan;

public unsafe partial class Device : DisposableNativeHandle
{
    private readonly AllocationCallbacks? allocator;

    internal Device(PhysicalDevice physicalDevice, CreateInfo createInfo, AllocationCallbacks? allocator)
    {
        fixed (VkDevice* pValue = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateDevice(physicalDevice, createInfo, allocator, pValue));
        }

        this.allocator = allocator;
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyDevice(this.Handle, this.allocator);

    public DeviceMemory AllocateMemory(DeviceMemory.AllocateInfo allocateInfo) =>
        new(this, allocateInfo, this.allocator);

    public Buffer CreateBuffer(Buffer.CreateInfo createInfo) =>
        new(this, createInfo, this.allocator);

    public CommandPool CreateCommandPool(CommandPool.CreateInfo createInfo) =>
        new(this, createInfo, this.allocator);

    public CommandPool CreateCommandPool(uint queueFamilyIndex, CommandPoolCreateFlags flags) =>
        this.CreateCommandPool(new() { Flags = flags, QueueFamilyIndex = queueFamilyIndex });

    public DescriptorPool CreateDescriptorPool(DescriptorPool.CreateInfo createInfo) =>
        new(this, createInfo, this.allocator);

    public Framebuffer CreateFramebuffer(Framebuffer.CreateInfo createInfo) => throw new NotImplementedException();

    public Image CreateImage(Image.CreateInfo createInfo) =>
        new(this, createInfo, this.allocator);

    public ImageView CreateImageView(ImageView.CreateInfo createInfo) =>
        new(this, createInfo, this.allocator);

    public DescriptorSetLayout CreateDescriptorSetLayout(DescriptorSetLayout.CreateInfo createInfo) =>
        new(this, createInfo, this.allocator);

    public Fence CreateFence(Fence.CreateInfo createInfo) => throw new NotImplementedException();

    public Pipeline CreateGraphicsPipelines(GraphicsPipeline.CreateInfo createInfo) => throw new NotImplementedException();

    public PipelineLayout CreatePipelineLayout(PipelineLayout.CreateInfo createInfo) => throw new NotImplementedException();

    public RenderPass CreateRenderPass(RenderPass.CreateInfo createInfo) => throw new NotImplementedException();

    public Sampler CreateSampler(Sampler.CreateInfo createInfo) => throw new NotImplementedException();

    public Semaphore CreateSemaphore(Semaphore.CreateInfo createInfo) => throw new NotImplementedException();

    public ShaderModule CreateShaderModule(ShaderModule.CreateInfo createInfo) => throw new NotImplementedException();

    public Queue GetQueue(uint familyIndex, uint index) =>
        new(this, familyIndex, index);

    public bool TryGetExtension<T>(out T extension) where T : IDeviceExtension<T> => throw new NotImplementedException();

    public void UpdateDescriptorSets(WriteDescriptorSet[] descriptorWrites, CopyDescriptorSet[] descriptorCopies)
    {
        fixed (VkWriteDescriptorSet* pDescriptorWrites = descriptorWrites.CastToNative())
        fixed (VkCopyDescriptorSet*  pDescriptorCopies = descriptorCopies.CastToNative())
        {
            PInvoke.vkUpdateDescriptorSets(this, (uint)descriptorWrites.Length, pDescriptorWrites, (uint)descriptorCopies.Length, pDescriptorCopies);
        }
    }

    public void WaitIdle() => throw new NotImplementedException();
}
