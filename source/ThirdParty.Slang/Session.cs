using System.Runtime.CompilerServices;
using Age.Core;

namespace ThirdParty.Slang;

public unsafe class Session : Disposable
{
    private readonly ISession* handle;

    public GlobalSession GlobalSession { get; }

    internal Session(GlobalSession globalSession, in Internal.SessionDesc sessionDesc)
    {
        var pSessionDesc = (Internal.SessionDesc*)Unsafe.AsPointer(in sessionDesc);

        fixed (ISession** pHandle = &this.handle)
        {
            SlangException.ThrowIfInvalid(globalSession.Handle->Vtbl->CreateSession(globalSession.Handle, pSessionDesc, pHandle));
        }

        this.handle->Vtbl->SlangUnknown.AddRef(this.handle);

        this.GlobalSession = globalSession;
    }

    protected override void OnDisposed(bool disposing) => throw new NotImplementedException();
}
