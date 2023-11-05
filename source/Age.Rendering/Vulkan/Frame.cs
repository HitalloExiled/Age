using Age.Numerics;
using Age.Vulkan.Native.Types;

namespace Age.Rendering.Vulkan;

public record Frame
{
    public List<UniformSet> UniformsSets { get; } = [];

    public required VkCommandBuffer CommandBuffer { get; init; }
    public required VkFence         Fence         { get; init; }
    public required ulong           Index         { get; init; }
    public required Size<uint>      Viewport      { get; init; }

    public bool Skipped { get; set; }
 }
