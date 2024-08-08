using Age.Core;

namespace Age.Rendering.Resources;

public class VertexBuffer : Disposable
{
    public required Buffer Buffer { get; init; }
    public required uint   Size   { get; init; }

    protected override void Disposed() =>
        this.Buffer.Dispose();

    public void Update<T>(T data) where T : unmanaged =>
        this.Buffer.Update(data);

    public void Update<T>(Span<T> data) where T : unmanaged =>
        this.Buffer.Update(data);
}
