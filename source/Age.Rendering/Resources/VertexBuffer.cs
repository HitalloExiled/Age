using Age.Core;

namespace Age.Rendering.Resources;

public sealed class VertexBuffer : Disposable
{
    public required Buffer Buffer { get; init; }
    public required uint   Size   { get; init; }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.Buffer.Dispose();
        }
    }

    public void Update<T>(T data) where T : unmanaged =>
        this.Buffer.Update(data);

    public void Update<T>(scoped ReadOnlySpan<T> data) where T : unmanaged =>
        this.Buffer.Update(data);
}
