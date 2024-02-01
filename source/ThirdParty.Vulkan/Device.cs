using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.Native;
using ThirdParty.Vulkan.TypeExtensions;

namespace ThirdParty.Vulkan;

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

    public DeviceMemory AllocateMemory(DeviceMemory.AllocateInfo allocateInfo) =>
        new(this, allocateInfo);

    public Buffer CreateBuffer(Buffer.CreateInfo createInfo) =>
        new(this, createInfo);

    public CommandPool CreateCommandPool(CommandPool.CreateInfo createInfo) =>
        new(this, createInfo);

    public CommandPool CreateCommandPool(uint queueFamilyIndex, CommandPoolCreateFlags flags) =>
        this.CreateCommandPool(new() { Flags = flags, QueueFamilyIndex = queueFamilyIndex });

    public DescriptorPool CreateDescriptorPool(DescriptorPool.CreateInfo createInfo) =>
        new(this, createInfo);

    public Framebuffer CreateFramebuffer(Framebuffer.CreateInfo createInfo) => throw new NotImplementedException();

    public Image CreateImage(Image.CreateInfo createInfo) =>
        new(this, createInfo);

    public ImageView CreateImageView(ImageView.CreateInfo createInfo) =>
        new(this, createInfo);

    public DescriptorSetLayout CreateDescriptorSetLayout(DescriptorSetLayout.CreateInfo createInfo) =>
        new(this, createInfo);

    public Fence CreateFence(Fence.CreateInfo createInfo) => throw new NotImplementedException();

    public Pipeline CreateGraphicsPipelines(GraphicsPipeline.CreateInfo createInfo) => throw new NotImplementedException();

    public PipelineLayout CreatePipelineLayout(PipelineLayout.CreateInfo createInfo) => throw new NotImplementedException();

    public RenderPass CreateRenderPass(RenderPass.CreateInfo createInfo) => throw new NotImplementedException();

    public Sampler CreateSampler(Sampler.CreateInfo createInfo) => throw new NotImplementedException();

    public Semaphore CreateSemaphore(Semaphore.CreateInfo createInfo) => throw new NotImplementedException();

    public ShaderModule CreateShaderModule(ShaderModule.CreateInfo createInfo) => throw new NotImplementedException();

    public T GetProcAddr<T>(string name) where T : Delegate
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            return Marshal.GetDelegateForFunctionPointer<T>((nint)PInvoke.vkGetDeviceProcAddr(this, pName));
        }
    }

    public Queue GetQueue(uint familyIndex, uint index) =>
        new(this, familyIndex, index);

    public bool TryGetExtension<T>([NotNullWhen(true)] out T? extension) where T : IDeviceExtension<T>
    {
        extension = this.enabledExtensions.Contains(T.Name) ? T.Create(this) : default;

        return extension != null;
    }

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
