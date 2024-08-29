using System.Text;
using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VKPhysicalDevice.html">VKPhysicalDevice</see>
/// </summary>
public unsafe partial class VkPhysicalDevice : ManagedHandle<VkPhysicalDevice>
{
    internal VkInstance Instance { get; }

    internal VkPhysicalDevice(VkHandle<VkPhysicalDevice> handle, VkInstance instance) : base(handle) =>
        this.Instance = instance;

    /// <inheritdoc cref="PInvoke.vkCreateDevice" />
    public VkDevice CreateDevice(in VkDeviceCreateInfo createInfo) =>
        new(this, createInfo);

    /// <inheritdoc cref="PInvoke.vkEnumerateDeviceExtensionProperties" />
    public VkExtensionProperties[] EnumerateDeviceExtensionProperties(string? layerName = null)
    {
        fixed (byte* pLayerName = Encoding.UTF8.GetBytes(layerName ?? ""))
        {
            uint propertyCount;

            VkException.Check(PInvoke.vkEnumerateDeviceExtensionProperties(this.handle, pLayerName, &propertyCount, null));

            var properties = new VkExtensionProperties[propertyCount];

            fixed (VkExtensionProperties* pProperties = properties)
            {
                VkException.Check(PInvoke.vkEnumerateDeviceExtensionProperties(this.handle, pLayerName, &propertyCount, pProperties));
            }

            return properties;
        }
    }

    /// <inheritdoc cref="PInvoke.vkGetPhysicalDeviceFeatures" />
    public void GetDeviceFeatures(out VkPhysicalDeviceFeatures features)
    {
        fixed (VkPhysicalDeviceFeatures* pFeatures = &features)
        {
            PInvoke.vkGetPhysicalDeviceFeatures(this.handle, pFeatures);
        }
    }

    /// <inheritdoc cref="PInvoke.vkGetPhysicalDeviceFormatProperties" />
    public void GetFormatProperties(VkFormat format, out VkFormatProperties formatProperties)
    {
        fixed (VkFormatProperties* pFormatProperties = &formatProperties)
        {
            PInvoke.vkGetPhysicalDeviceFormatProperties(this.handle, format, pFormatProperties);
        }
    }

    /// <inheritdoc cref="PInvoke.vkGetPhysicalDeviceMemoryProperties" />
    public void GetMemoryProperties(out VkPhysicalDeviceMemoryProperties memoryProperties)
    {
        fixed (VkPhysicalDeviceMemoryProperties* pMemoryProperties = &memoryProperties)
        {
            PInvoke.vkGetPhysicalDeviceMemoryProperties(this.handle, pMemoryProperties);
        }
    }

    /// <inheritdoc cref="PInvoke.vkGetPhysicalDeviceProperties" />
    public void GetProperties(out VkPhysicalDeviceProperties properties)
    {
        fixed (VkPhysicalDeviceProperties* pProperties = &properties)
        {
            PInvoke.vkGetPhysicalDeviceProperties(this.handle, pProperties);
        }
    }

    /// <inheritdoc cref="PInvoke.vkGetPhysicalDeviceQueueFamilyProperties" />
    public VkQueueFamilyProperties[] GetQueueFamilyProperties()
    {
        uint qeueFamilyPropertyCount;

        PInvoke.vkGetPhysicalDeviceQueueFamilyProperties(this.handle, &qeueFamilyPropertyCount, null);

        var queueFamilyProperties = new VkQueueFamilyProperties[qeueFamilyPropertyCount];

        fixed (VkQueueFamilyProperties* pQueueFamilyProperties = queueFamilyProperties)
        {
            PInvoke.vkGetPhysicalDeviceQueueFamilyProperties(this.handle, &qeueFamilyPropertyCount, pQueueFamilyProperties);
        }

        return queueFamilyProperties;
    }
}
