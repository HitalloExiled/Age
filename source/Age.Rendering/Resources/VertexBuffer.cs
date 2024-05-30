namespace Age.Rendering.Resources;

public class VertexBuffer : Disposable
{
    public required Buffer Buffer { get; init; }
    public required uint   Size   { get; init; }

    protected override void OnDispose() =>
        this.Buffer.Dispose();
}
