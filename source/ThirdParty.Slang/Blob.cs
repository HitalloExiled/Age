namespace ThirdParty.Slang;

public readonly unsafe ref struct Blob
{
    internal readonly IBlob* Handle;

    public readonly byte* Buffer => this.Handle == null ? (byte*)null : (byte*)this.Handle->Vtbl->GetBufferPointer(this.Handle);
    public readonly int   Length => this.Handle == null ? 0 : (int)this.Handle->Vtbl->GetBufferSize(this.Handle);

    public readonly Span<byte> AsSpan() =>
        new(this.Buffer, this.Length);

    public readonly void Dispose()
    {
        if (this.Handle != null)
        {
            this.Handle->Vtbl->SlangUnknown.Release(this.Handle);
        }
    }

    public static implicit operator Span<byte>(Blob blob) => blob.AsSpan();
}
