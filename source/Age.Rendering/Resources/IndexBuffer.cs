using Age.Core;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public class IndexBuffer : Disposable
{
    public required Buffer      Buffer { get; init; }
    public required uint        Size   { get; init; }
    public required VkIndexType Type   { get; init; }

    protected override void OnDispose() =>
        this.Buffer.Dispose();

    public void Update<T>(T data) where T : unmanaged =>
        this.Buffer.Update(data);

    public void Update<T>(Span<T> data) where T : unmanaged =>
        this.Buffer.Update(data);
}
