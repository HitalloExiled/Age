namespace ThirdParty.Slang;

public unsafe class ComponentType : SlangUnknown
{
    internal new IComponentType* Handle => (IComponentType*)base.Handle;

    public Session Session { get; }

    internal ComponentType(Session session, IComponentType* handle, bool ownsHandler = true) : base((ISlangUnknown*)handle, ownsHandler) =>
        this.Session = session;

    public ComponentType Link()
    {
        using var blobDiagnostics = new Blob();

        IComponentType* pComponentType;

        SlangException.Check(this.Handle->Vtbl->Link(this.Handle, &pComponentType, &blobDiagnostics.Handle), blobDiagnostics);

        return new ComponentType(this.Session, pComponentType, true);
    }

    public Blob GetEntryPointCode(long entryPointIndex = 0, long targetIndex = 0)
    {
        var outCode               = new Blob();
        using var diagnosticsBlob = new Blob();

        SlangException.Check(this.Handle->Vtbl->GetEntryPointCode(this.Handle, entryPointIndex, targetIndex, &outCode.Handle, &diagnosticsBlob.Handle), diagnosticsBlob);

        return outCode;
    }

    public SlangReflection? GetLayout(int targetIndex = 0)
    {
        var handle = this.Handle->Vtbl->GetLayout(this.Handle, targetIndex, null);

        return handle == default ? null : new(handle);
    }

    public static implicit operator IComponentType*(ComponentType componentType) => componentType.Handle;
}
