using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public record IndexBuffer : Disposable
{
    public required Buffer      Buffer { get; init; }
    public required uint        Size   { get; init; }
    public required VkIndexType Type   { get; init; }

    protected override void OnDispose() =>
        this.Buffer.Dispose();
}
