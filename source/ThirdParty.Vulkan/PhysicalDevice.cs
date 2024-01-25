using System.Text;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class PhysicalDevice : NativeHandle
{
    private readonly AllocationCallbacks? allocator;

    internal PhysicalDevice(VkPhysicalDevice handle, AllocationCallbacks? allocator) : base(handle) =>
        this.allocator = allocator;

    public Device CreateDevice(Device.CreateInfo createInfo) =>
        new(this, createInfo, this.allocator);

    public ExtensionProperties[] EnumerateDeviceExtensionProperties(string? layerName = null)
    {
        fixed (byte* pLayerName = Encoding.UTF8.GetBytes(layerName ?? ""))
        {
            uint propertyCount;

            PInvoke.vkEnumerateDeviceExtensionProperties(this.Handle, pLayerName, &propertyCount, null);

            var buffer = new VkExtensionProperties[propertyCount];

            fixed (VkExtensionProperties* pExtensionProperties = buffer)
            {
                PInvoke.vkEnumerateDeviceExtensionProperties(this.Handle, pLayerName, &propertyCount, pExtensionProperties);
            }

            var extensionProperties = new ExtensionProperties[propertyCount];

            for (var i = 0; i < propertyCount; i++)
            {
                extensionProperties[i] = new(buffer[i]);
            }

            return extensionProperties;
        }
    }

    public Features GetDeviceFeatures()
    {
        VkPhysicalDeviceFeatures physicalDeviceFeatures;

        PInvoke.vkGetPhysicalDeviceFeatures(this.Handle, &physicalDeviceFeatures);

        return new(physicalDeviceFeatures);
    }

    public FormatProperties GetFormatProperties(Format format)
    {
        VkFormatProperties formatProperties;

        PInvoke.vkGetPhysicalDeviceFormatProperties(this.Handle, format, &formatProperties);

        return new(formatProperties);
    }

    public MemoryProperties GetMemoryProperties()
    {
        VkPhysicalDeviceMemoryProperties physicalDeviceMemoryProperties;

        PInvoke.vkGetPhysicalDeviceMemoryProperties(this.Handle, &physicalDeviceMemoryProperties);

        return new(physicalDeviceMemoryProperties);
    }

    public Properties GetProperties()
    {
        VkPhysicalDeviceProperties physicalDeviceProperties;

        PInvoke.vkGetPhysicalDeviceProperties(this.Handle, &physicalDeviceProperties);

        return new(physicalDeviceProperties);
    }

    public QueueFamilyProperties[] GetQueueFamilyProperties()
    {
        uint qeueFamilyPropertyCount;

        PInvoke.vkGetPhysicalDeviceQueueFamilyProperties(this.Handle, &qeueFamilyPropertyCount, null);

        var vkQueueFamilyProperties = new VkQueueFamilyProperties[qeueFamilyPropertyCount];

        fixed (VkQueueFamilyProperties* pQueueFamilyProperties = vkQueueFamilyProperties)
        {
            PInvoke.vkGetPhysicalDeviceQueueFamilyProperties(this.Handle, &qeueFamilyPropertyCount, pQueueFamilyProperties);
        }

        var queueFamilyProperties = new QueueFamilyProperties[qeueFamilyPropertyCount];

        for (var i = 0; i < qeueFamilyPropertyCount; i++)
        {
            queueFamilyProperties[i] = new(vkQueueFamilyProperties[i]);
        }

        return queueFamilyProperties;
    }
}
