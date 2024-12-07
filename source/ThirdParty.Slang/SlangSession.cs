using System.Text;
using Age.Core;

namespace ThirdParty.Slang;

public unsafe class SlangSession : Disposable
{
    internal SlangSessionHandle Handle { get; }

    public SlangSession()
    {
        if ((this.Handle = PInvoke.spCreateSession(null)) == default)
        {
            throw new InvalidOperationException();
        }
    }

    protected override void Disposed(bool disposing) =>
        PInvoke.spDestroyCompileRequest(this.Handle);

    public SlangProfileID FindProfile(string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            return PInvoke.spFindProfile(this.Handle, pName);
        }
    }

    public object LoadModule(string filepath) => throw new NotImplementedException();
}
