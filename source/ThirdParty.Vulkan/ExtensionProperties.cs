using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkExtensionProperties.html">VkExtensionProperties</see>
/// </summary>
public unsafe class ExtensionProperties
{
    public string  ExtensionName { get; }
    public Version SpecVersion   { get; }

    internal ExtensionProperties(in VkExtensionProperties extensionProperties)
    {
        fixed (void* pExtensionName = extensionProperties.extensionName)
        {
            this.ExtensionName = Marshal.PtrToStringAnsi((nint)pExtensionName)!;
        }

        this.SpecVersion = new(extensionProperties.specVersion);
    }
}
