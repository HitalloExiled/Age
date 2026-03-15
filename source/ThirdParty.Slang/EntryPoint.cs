namespace ThirdParty.Slang;

public sealed unsafe class EntryPoint : ComponentType
{
    internal new IEntryPoint* Handle => (IEntryPoint*)base.Handle;

    public Module Module { get; }

    internal EntryPoint(Module module, IEntryPoint* handle) : base(module.Session, (IComponentType*)handle, true) =>
        this.Module = module;

    public static implicit operator IEntryPoint*(EntryPoint entryPoint) => entryPoint.Handle;
}
