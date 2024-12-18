using System.Text;

namespace ThirdParty.Slang;

public unsafe class SlangSession : DisposableManagedSlang<SlangSession>
{
    public SlangSession() : base(PInvoke.spCreateSession(null))
    { }

    protected override void Disposed(bool disposing) =>
        PInvoke.spDestroySession(this.Handle);

    public SlangProfileID FindProfile(string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            return PInvoke.spFindProfile(this.Handle, pName);
        }
    }
}
