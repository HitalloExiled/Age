namespace Age.Rendering.Resources;

public record VertexBuffer : Disposable
{
    public required Buffer Buffer { get; init; }
    public required ulong  Size   { get; init; }

    protected override void OnDispose() =>
        this.Buffer.Dispose();
}
