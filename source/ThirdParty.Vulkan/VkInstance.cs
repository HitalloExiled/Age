using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Age.Core.Extensions;
using ThirdParty.Vulkan.Interfaces;

using static Age.Core.PointerHelper;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkInstance.html">VkInstance</see>
/// </summary>
public sealed unsafe class VkInstance : DisposableManagedHandle<VkInstance>
{
    private readonly HashSet<string> enabledExtensions = [];

    internal readonly VkAllocationCallbacks Allocator;

    public VkInstance(in VkInstanceCreateInfo createInfo, in VkAllocationCallbacks allocator = default)
    {
        this.Allocator         = allocator;
        this.enabledExtensions = [.. Array.ToUTF8StringArray(createInfo.PpEnabledExtensionNames, createInfo.EnabledExtensionCount)];

        fixed (VkHandle<VkInstance>*  pHandler    = &this.handle)
        fixed (VkInstanceCreateInfo*  pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &allocator)
        {
            VkException.Check(PInvoke.vkCreateInstance(pCreateInfo, NullIfDefault(pAllocator), pHandler));
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

    protected override void Disposed()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Allocator)
        {
            PInvoke.vkDestroyInstance(this.handle, NullIfDefault(pAllocator));
        }
    }

    /// <inheritdoc cref="PInvoke.vkEnumeratePhysicalDevices" />
    public VkPhysicalDevice[] EnumeratePhysicalDevices()
    {
        uint physicalDeviceCount;

        VkException.Check(PInvoke.vkEnumeratePhysicalDevices(this.handle, &physicalDeviceCount, null));

        var vkPhysicalDevices = new VkHandle<VkPhysicalDevice>[physicalDeviceCount];

        fixed (VkHandle<VkPhysicalDevice>* pPhysicalDevices = vkPhysicalDevices)
        {
            VkException.Check(PInvoke.vkEnumeratePhysicalDevices(this.handle, &physicalDeviceCount, pPhysicalDevices));
        }

        var physicalDevices = new VkPhysicalDevice[physicalDeviceCount];

        for (var i = 0; i < physicalDeviceCount; i++)
        {
            physicalDevices[i] = new(vkPhysicalDevices[i], this);
        }

        return physicalDevices;
    }

    public T GetExtension<T>() where T : IInstanceExtension<T> =>
        this.TryGetExtension<T>(out var extension) ? extension : throw new InvalidOperationException($"Can't load extension {T.Name}");

    /// <inheritdoc cref="PInvoke.vkGetInstanceProcAddr" />
    public T GetProcAddr<T>(string name) where T : Delegate
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            return Marshal.GetDelegateForFunctionPointer<T>((nint)PInvoke.vkGetInstanceProcAddr(this.handle, pName));
        }
    }

    public bool TryGetExtension<T>([NotNullWhen(true)] out T? extension) where T : IInstanceExtension<T>
    {
        extension = this.enabledExtensions.Contains(T.Name) ? T.Create(this) : default;

        return extension != null;
    }
}
