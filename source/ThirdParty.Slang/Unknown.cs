using Age.Core;

namespace ThirdParty.Slang;

public unsafe abstract class SlangUnknown(bool ownsHandler) : Disposable, IEquatable<SlangUnknown>
{
    private readonly bool ownsHandler = ownsHandler;

    internal ISlangUnknown* Handle;

    protected SlangUnknown(ISlangUnknown* handle, bool ownsHandler) : this(ownsHandler) =>
        this.Handle = handle;

    protected override void OnDisposed(bool disposing)
    {
        if (!this.ownsHandler)
        {
            return;
        }

        this.Handle->Vtbl->Release(this.Handle);
    }

    public bool Equals(SlangUnknown? other) =>
        other != null && this.Handle == other.Handle;

    public override bool Equals(object? obj) =>
        this.Equals(obj as SlangUnknown);

    public override int GetHashCode() =>
        ((nint)this.Handle).GetHashCode();

    public static implicit operator void*(SlangUnknown slangUnknown) => slangUnknown.Handle;
    public static implicit operator ISlangUnknown*(SlangUnknown slangUnknown) => slangUnknown.Handle;
}
