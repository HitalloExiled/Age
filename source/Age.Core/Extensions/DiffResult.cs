using System.Buffers;

namespace Age.Core.Extensions;

public readonly ref struct DiffResult<T>(T[] addedBuffer, int addedCount, T[] removedBuffer, int removedCount)
{
    private readonly T[] addedBuffer   = addedBuffer;
    private readonly T[] removedBuffer = removedBuffer;

    public readonly Span<T> Added   => this.addedBuffer.AsSpan(0, addedCount);
    public readonly Span<T> Removed => this.removedBuffer.AsSpan(0, removedCount);

    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(this.addedBuffer);
        ArrayPool<T>.Shared.Return(this.removedBuffer);
    }
}
