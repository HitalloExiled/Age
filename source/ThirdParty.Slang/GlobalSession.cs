using Age.Core;

namespace ThirdParty.Slang;

public unsafe class GlobalSession : SlangUnknown
{
    internal new IGlobalSession* Handle => (IGlobalSession*)base.Handle;

    public GlobalSession(uint version) : base(true)
    {
        IGlobalSession* handle;

        SlangException.Check(PInvoke.slang_createGlobalSession(version, &handle), $"Failed to create {nameof(GlobalSession)}");

        base.Handle = (ISlangUnknown*)handle;
    }

    public Session CreateSession(in SessionDesc sessionDesc)
    {
        fixed (SessionDesc* pSessionDesc = &sessionDesc)
        {
            ISession* pSession;

            SlangException.Check(this.Handle->Vtbl->CreateSession(this.Handle, pSessionDesc, &pSession), $"Failed to create {nameof(Session)}");

            return new(this, pSession);
        }
    }

    public SlangProfileID FindProfile(string name)
    {
        using var pName = new NativeString(name);

        return this.Handle->Vtbl->FindProfile(this.Handle, pName);
    }
}
