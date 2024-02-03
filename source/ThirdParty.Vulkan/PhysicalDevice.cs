using System.Text;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VKPhysicalDevice.html">VKPhysicalDevice</see>
/// </summary>
public unsafe partial class PhysicalDevice : NativeHandle
{
    internal Instance Instance { get; }

    internal PhysicalDevice(VkPhysicalDevice handle, Instance instance) : base(handle) =>
        this.Instance = instance;

    /// <inheritdoc cref="PInvoke.vkCreateDevice" />
    public Device CreateDevice(Device.CreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkEnumerateDeviceExtensionProperties" />
    public ExtensionProperties[] EnumerateDeviceExtensionProperties(string? layerName = null)
    {
        fixed (byte* pLayerName = Encoding.UTF8.GetBytes(layerName ?? ""))
        {
            uint propertyCount;

            VulkanException.Check(PInvoke.vkEnumerateDeviceExtensionProperties(this.Handle, pLayerName, &propertyCount, null));

            var buffer = new VkExtensionProperties[propertyCount];

            fixed (VkExtensionProperties* pExtensionProperties = buffer)
            {
                VulkanException.Check(PInvoke.vkEnumerateDeviceExtensionProperties(this.Handle, pLayerName, &propertyCount, pExtensionProperties));
            }

            var extensionProperties = new ExtensionProperties[propertyCount];

            for (var i = 0; i < propertyCount; i++)
            {
                extensionProperties[i] = new(buffer[i]);
            }

            return extensionProperties;
        }
    }

    /// <inheritdoc cref="PInvoke.vkGetPhysicalDeviceFeatures" />
    public Features GetDeviceFeatures()
    {
        VkPhysicalDeviceFeatures physicalDeviceFeatures;

        PInvoke.vkGetPhysicalDeviceFeatures(this.Handle, &physicalDeviceFeatures);

        return new(physicalDeviceFeatures);
    }

    /// <inheritdoc cref="PInvoke.vkGetPhysicalDeviceFormatProperties" />
    public FormatProperties GetFormatProperties(Format format)
    {
        VkFormatProperties formatProperties;

        PInvoke.vkGetPhysicalDeviceFormatProperties(this.Handle, format, &formatProperties);

        return new(formatProperties);
    }

    /// <inheritdoc cref="PInvoke.vkGetPhysicalDeviceMemoryProperties" />
    public MemoryProperties GetMemoryProperties()
    {
        VkPhysicalDeviceMemoryProperties physicalDeviceMemoryProperties;

        PInvoke.vkGetPhysicalDeviceMemoryProperties(this.Handle, &physicalDeviceMemoryProperties);

        return new(physicalDeviceMemoryProperties);
    }

    /// <inheritdoc cref="PInvoke.vkGetPhysicalDeviceProperties" />
    public Properties GetProperties()
    {
        VkPhysicalDeviceProperties physicalDeviceProperties;

        PInvoke.vkGetPhysicalDeviceProperties(this.Handle, &physicalDeviceProperties);

        return new(physicalDeviceProperties);
    }

    /// <inheritdoc cref="PInvoke.vkGetPhysicalDeviceQueueFamilyProperties" />
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
