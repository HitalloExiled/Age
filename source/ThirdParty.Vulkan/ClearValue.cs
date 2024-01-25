using System.Runtime.CompilerServices;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkClearValue" />
public unsafe record ClearValue : NativeReference<VkClearValue>
{
    private ClearColorValue?        color;
    private ClearDepthStencilValue? depthStencil;

    public ClearColorValue? Color
    {
        get => this.color;
        init => this.PNative->color = this.color = value;
    }

    public ClearDepthStencilValue? DepthStencil
    {
        get => this.depthStencil;
        init => this.PNative->depthStencil = this.depthStencil = value;
    }
}

/// <inheritdoc cref="VkClearDepthStencilValue" />
public unsafe record ClearDepthStencilValue : NativeReference<VkClearDepthStencilValue>
{
    public float Depth
    {
        get => this.PNative->depth;
        init => this.PNative->depth = value;
    }

    public uint Stencil
    {
        get => this.PNative->stencil;
        init => this.PNative->stencil = value;
    }
}

/// <inheritdoc cref="VkClearColorValue" />
public unsafe record ClearColorValue : NativeReference<VkClearColorValue>
{
    private readonly byte[] values = new byte[4 * 4];

    public float[] Float32
    {
        get => Unsafe.As<float[]>(this.values);
        init => Init(this.values, (byte*)this.PNative->float32, 4, Unsafe.As<byte[]>(this.values)!, nameof(this.Float32));
    }

    public int[] Int32
    {
        get => Unsafe.As<int[]>(this.values);
        init => Init(this.values, (byte*)this.PNative->int32, 4, Unsafe.As<byte[]>(this.values)!, nameof(this.Float32));
    }

    public uint[] Uint32
    {
        get => Unsafe.As<uint[]>(this.values);
        init => Init(this.values, (byte*)this.PNative->uint32, 4, Unsafe.As<byte[]>(this.values)!, nameof(this.Float32));
    }
}
