using System.Diagnostics.CodeAnalysis;
using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class Instance : DisposableNativeHandle
{
    private readonly AllocationCallbacks? allocator;

    public Instance(CreateInfo createInfo, AllocationCallbacks? allocator = null)
    {
        this.allocator = allocator;

        fixed (nint* pHandler = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateInstance(createInfo, this.allocator, pHandler));
        }
    }

    public static LayerProperties[] EnumerateLayerProperties()
    {
        uint propertyCount;

        VulkanException.Check(PInvoke.vkEnumerateInstanceLayerProperties(&propertyCount, null));

        var buffer = new VkLayerProperties[propertyCount];

        fixed (VkLayerProperties* pProperties = buffer)
        {
            VulkanException.Check(PInvoke.vkEnumerateInstanceLayerProperties(&propertyCount, null));
        }

        var layerProperties = new LayerProperties[propertyCount];

        for (var i = 0; i < propertyCount; i++)
        {
            layerProperties[i] = new(buffer[i]);
        }

        return layerProperties;
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyInstance(this.Handle, this.allocator);

    public PhysicalDevice[] EnumeratePhysicalDevices()
    {
        uint physicalDeviceCount;

        VulkanException.Check(PInvoke.vkEnumeratePhysicalDevices(this.Handle, &physicalDeviceCount, null));

        var vkPhysicalDevices = new VkPhysicalDevice[physicalDeviceCount];
        var physicalDevices   = new PhysicalDevice[physicalDeviceCount];

        fixed (VkPhysicalDevice* pPhysicalDevices = vkPhysicalDevices)
        {
            VulkanException.Check(PInvoke.vkEnumeratePhysicalDevices(this.Handle, &physicalDeviceCount, pPhysicalDevices));
        }

        for (var i = 0; i < physicalDeviceCount; i++)
        {
            physicalDevices[i] = new(vkPhysicalDevices[i], this.allocator);
        }

        return physicalDevices;
    }

    public bool TryGetExtension<T>([NotNullWhen(true)] out T? vkExtDebugUtils) where T : IInstanceExtension<T> => throw new NotImplementedException();
}
