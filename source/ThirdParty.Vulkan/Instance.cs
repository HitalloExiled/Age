using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class Instance : DisposableNativeHandle
{
    private readonly HashSet<string> enabledExtensions = [];

    internal AllocationCallbacks? Allocator { get; }

    public Instance(CreateInfo createInfo, AllocationCallbacks? allocator = null)
    {
        this.enabledExtensions = [.. createInfo.EnabledExtensions];

        this.Allocator = allocator;

        fixed (nint* pHandler = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateInstance(createInfo, this.Allocator, pHandler));
        }
    }

    public static ExtensionProperties[] EnumerateExtensionProperties(string? layer)
    {
        fixed (byte* pLayer = Encoding.UTF8.GetBytes(layer ?? ""))
        {
            uint propertyCount;

            PInvoke.vkEnumerateInstanceExtensionProperties(pLayer, &propertyCount, null);

            var vkExtensionProperties = new VkExtensionProperties[propertyCount];

            fixed (VkExtensionProperties* pExtensionProperties = vkExtensionProperties)
            {
                PInvoke.vkEnumerateInstanceExtensionProperties(pLayer, &propertyCount, pExtensionProperties);

                var extensionProperties = new ExtensionProperties[propertyCount];

                for (var i = 0; i < propertyCount; i++)
                {
                    extensionProperties[i] = new(vkExtensionProperties[i]);
                }

                return extensionProperties;
            }
        }
    }

    public static LayerProperties[] EnumerateLayerProperties()
    {
        uint propertyCount;

        VulkanException.Check(PInvoke.vkEnumerateInstanceLayerProperties(&propertyCount, null));

        var buffer = new VkLayerProperties[propertyCount];

        fixed (VkLayerProperties* pProperties = buffer)
        {
            VulkanException.Check(PInvoke.vkEnumerateInstanceLayerProperties(&propertyCount, pProperties));
        }

        var layerProperties = new LayerProperties[propertyCount];

        for (var i = 0; i < propertyCount; i++)
        {
            layerProperties[i] = new(buffer[i]);
        }

        return layerProperties;
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyInstance(this.Handle, this.Allocator);

    public PhysicalDevice[] EnumeratePhysicalDevices()
    {
        uint physicalDeviceCount;

        VulkanException.Check(PInvoke.vkEnumeratePhysicalDevices(this.Handle, &physicalDeviceCount, null));

        var vkPhysicalDevices = new VkPhysicalDevice[physicalDeviceCount];

        fixed (VkPhysicalDevice* pPhysicalDevices = vkPhysicalDevices)
        {
            VulkanException.Check(PInvoke.vkEnumeratePhysicalDevices(this.Handle, &physicalDeviceCount, pPhysicalDevices));
        }

        var physicalDevices = new PhysicalDevice[physicalDeviceCount];

        for (var i = 0; i < physicalDeviceCount; i++)
        {
            physicalDevices[i] = new(vkPhysicalDevices[i], this);
        }

        return physicalDevices;
    }

    internal T GetProcAddr<T>(string name) where T : Delegate
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            return Marshal.GetDelegateForFunctionPointer<T>((nint)PInvoke.vkGetInstanceProcAddr(this, pName));
        }
    }

    public bool TryGetExtension<T>([NotNullWhen(true)] out T? extension) where T : IInstanceExtension<T>
    {
        extension = this.enabledExtensions.Contains(T.Name) ? T.Create(this) : default;

        return extension != null;
    }
}
