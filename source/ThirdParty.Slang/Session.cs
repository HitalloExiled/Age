using Age.Core;

namespace ThirdParty.Slang;

public unsafe class Session : SlangUnknown
{
    internal new ISession* Handle => (ISession*)base.Handle;

    public GlobalSession GlobalSession { get; }

    internal Session(GlobalSession globalSession, ISession* handle) : base((ISlangUnknown*)handle, true) =>
        this.GlobalSession = globalSession;

    public Module? LoadModule(string moduleName)
    {
        using var pModuleName = new UnmanagedString(moduleName);

        var pModule = this.Handle->Vtbl->LoadModule(this.Handle, pModuleName, null);

        return pModule == null ? null : new Module(this, pModule);
    }

    public Module? LoadModuleFromSource(string moduleName, string path, ReadOnlySpan<byte> source)
    {
        fixed (byte* pSouce = source)
        {
            using var pModuleName = new UnmanagedString(moduleName);
            using var pPath       = new UnmanagedString(path);

            var blob = PInvoke.slang_createBlob(pSouce, (ulong)source.Length);

            var pModule = this.Handle->Vtbl->LoadModuleFromSource(this.Handle, pModuleName, pPath, blob, null);

            return pModule == null ? null : new Module(this, pModule);
        }
    }

    public Module? LoadModuleFromSourceString(string moduleName, string path, string content)
    {
            using var pModuleName = new UnmanagedString(moduleName);
            using var pPath       = new UnmanagedString(path);
            using var pContent    = new UnmanagedString(content);

            var pModule = this.Handle->Vtbl->LoadModuleFromSourceString(this.Handle, pModuleName, pPath, pContent, null);

            return pModule == null ? null : new Module(this, pModule);
    }

    public ComponentType CreateCompositeComponentType(ReadOnlySpan<ComponentType> components)
    {
        var pComponentTypes = stackalloc IComponentType*[components.Length];

        for (var i = 0; i < components.Length; i++)
        {
            pComponentTypes[i] = components[i].Handle;
        }

        IComponentType* compositeComponentType;

        using var diagnosticsBlob = new Blob();

        SlangException.Check(this.Handle->Vtbl->CreateCompositeComponentType(this.Handle, pComponentTypes, components.Length, &compositeComponentType, &diagnosticsBlob.Handle), diagnosticsBlob);

        return new(this, compositeComponentType);
    }

    public static implicit operator ISession*(Session session) => session.Handle;
}
