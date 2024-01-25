using ThirdParty.Vulkan.Native.KHR;

namespace ThirdParty.Vulkan.Extensions.KHR;

public struct Win32Surface
{
    /// <inheritdoc cref="VkWin32SurfaceCreateInfoKHR" />
    public unsafe record CreateInfo : NativeReference<VkWin32SurfaceCreateInfoKHR>
    {
        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public uint Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public HINSTANCE Hinstance
        {
            get => this.PNative->hinstance;
            init => this.PNative->hinstance = value;
        }

        public HWND Hwnd
        {
            get => this.PNative->hwnd;
            init => this.PNative->hwnd = value;
        }
    }
}
