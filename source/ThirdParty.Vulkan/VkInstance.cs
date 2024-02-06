using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Age.Core.Interop;
using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkInstance.html">VkInstance</see>
/// </summary>
public unsafe partial class VkInstance : DisposableManagedHandle<VkInstance>
{
    private readonly HashSet<string> enabledExtensions = [];

    internal readonly VkAllocationCallbacks Allocator;

    public VkInstance(in VkInstanceCreateInfo createInfo, in VkAllocationCallbacks allocator = default)
    {
        this.Allocator         = allocator;
        this.enabledExtensions = [.. PointerHelper.ToArray(createInfo.PpEnabledExtensionNames, createInfo.EnabledExtensionCount)];

        fixed (VkHandle<VkInstance>*  pHandler    = &this.Handle)
        fixed (VkInstanceCreateInfo*  pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &allocator)
        {
            VkException.Check(PInvoke.vkCreateInstance(pCreateInfo, PointerHelper.NullIfDefault(allocator, pAllocator), pHandler));
        }
    }

    /// <inheritdoc cref="PInvoke.vkEnumerateInstanceExtensionProperties" />
    public static VkExtensionProperties[] EnumerateExtensionProperties(string? layer)
    {
        fixed (byte* pLayer = Encoding.UTF8.GetBytes(layer ?? ""))
        {
            uint propertyCount;

            VkException.Check(PInvoke.vkEnumerateInstanceExtensionProperties(pLayer, &propertyCount, null));

            var properties = new VkExtensionProperties[propertyCount];

            fixed (VkExtensionProperties* pExtensionProperties = properties)
            {
                VkException.Check(PInvoke.vkEnumerateInstanceExtensionProperties(pLayer, &propertyCount, pExtensionProperties));

                return properties;
            }
        }
    }

    /// <inheritdoc cref="PInvoke.vkEnumerateInstanceLayerProperties" />
    public static VkLayerProperties[] EnumerateLayerProperties()
    {
        uint propertyCount;

        VkException.Check(PInvoke.vkEnumerateInstanceLayerProperties(&propertyCount, null));

        var properties = new VkLayerProperties[propertyCount];

        fixed (VkLayerProperties* pProperties = properties)
        {
            VkException.Check(PInvoke.vkEnumerateInstanceLayerProperties(&propertyCount, pProperties));
        }

        return properties;
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Allocator)
        {
            PInvoke.vkDestroyInstance(this.Handle, PointerHelper.NullIfDefault(this.Allocator, pAllocator));
        }
    }

    /// <inheritdoc cref="PInvoke.vkEnumeratePhysicalDevices" />
    public VkPhysicalDevice[] EnumeratePhysicalDevices()
    {
        uint physicalDeviceCount;

        VkException.Check(PInvoke.vkEnumeratePhysicalDevices(this.Handle, &physicalDeviceCount, null));

        var vkPhysicalDevices = new VkHandle<VkPhysicalDevice>[physicalDeviceCount];

        fixed (VkHandle<VkPhysicalDevice>* pPhysicalDevices = vkPhysicalDevices)
        {
            VkException.Check(PInvoke.vkEnumeratePhysicalDevices(this.Handle, &physicalDeviceCount, pPhysicalDevices));
        }

        var physicalDevices = new VkPhysicalDevice[physicalDeviceCount];

        for (var i = 0; i < physicalDeviceCount; i++)
        {
            physicalDevices[i] = new(vkPhysicalDevices[i], this);
        }

        return physicalDevices;
    }

    /// <inheritdoc cref="PInvoke.vkGetInstanceProcAddr" />
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
