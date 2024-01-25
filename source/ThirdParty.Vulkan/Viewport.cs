using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkViewport.html">VkViewport</see>
/// </summary>
public unsafe record Viewport : NativeReference<VkViewport>
{
    public float X
    {
        get => this.PNative->x;
        init => this.PNative->x = value;
    }

    public float Y
    {
        get => this.PNative->y;
        init => this.PNative->y = value;
    }

    public float Width
    {
        get => this.PNative->width;
        init => this.PNative->width = value;
    }

    public float Height
    {
        get => this.PNative->height;
        init => this.PNative->height = value;
    }

    public float MinDepth
    {
        get => this.PNative->minDepth;
        init => this.PNative->minDepth = value;
    }

    public float MaxDepth
    {
        get => this.PNative->maxDepth;
        init => this.PNative->maxDepth = value;
    }
}

