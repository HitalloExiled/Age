using Age.Core;

namespace ThirdParty.Slang;

public unsafe class FileSystem : SlangCastable
{
    internal new ISlangFileSystem* Handle => (ISlangFileSystem*)base.Handle;

    internal FileSystem(ISlangCastable* handle, bool ownsHandler) : base(handle, ownsHandler)
    { }

    public Blob LoadFile(string path)
    {
        using var uPath = new NativeString(path);

        var blob = new Blob();

        SlangException.Check(this.Handle->Vtbl->LoadFile(this.Handle, uPath, &blob.Handle), $"Failed to load file {path}");

        return blob;
    }
}
