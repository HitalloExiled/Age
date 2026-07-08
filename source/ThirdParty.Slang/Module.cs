using System.Text;
using Age.Core;
using Age.Core.Collections;
using Age.Core.Extensions;

namespace ThirdParty.Slang;

public sealed unsafe class Module : ComponentType
{
    internal new IModule* Handle => (IModule*)base.Handle;

    public int DefinedEntryPointCount => this.Handle->Vtbl->GetDefinedEntryPointCount(this.Handle);
    public int DependencyFileCount    => this.Handle->Vtbl->GetDependencyFileCount(this.Handle);

    internal Module(Session session, IModule* handle) : base(session, (IComponentType*)handle, false) { }

    public EntryPoint? FindEntryPointByName(string name)
    {
        IEntryPoint* pEntryPoint;

        using var pName = new NativeString(name);

        SlangException.Check(this.Handle->Vtbl->FindEntryPointByName(this.Handle, pName, &pEntryPoint), $"Failed to find {nameof(EntryPoint)} with the name ${name}");

        return pEntryPoint == null ? null : new EntryPoint(this, pEntryPoint);
    }

    public EntryPoint GetDefinedEntryPoint(int index)
    {
        IEntryPoint* pEntrypoint;

        SlangException.Check(this.Handle->Vtbl->GetDefinedEntryPoint(this.Handle, index, &pEntrypoint), $"Failed to get entrypoint at {index}");

        return new(this, pEntrypoint);
    }

    public DisposableRentedArray<EntryPoint> GetDefinedEntryPoints()
    {
        var entrypoints = new DisposableRentedArray<EntryPoint>(this.DefinedEntryPointCount);

        for (var i = 0; i < entrypoints.Length; i++)
        {
            entrypoints[i] = this.GetDefinedEntryPoint(i);
        }

        return entrypoints;
    }

    public string? GetDependencyFilePath(int index) =>
        Encoding.GetStringFromNullTerminated(this.Handle->Vtbl->GetDependencyFilePath(this.Handle, index));

    public string[] GetDependencyFiles()
    {
        var dependencies = new string[this.DependencyFileCount];

        for (var i = 0; i < dependencies.Length; i++)
        {
            dependencies[i] = this.GetDependencyFilePath(i)!;
        }

        return dependencies;
    }

    public static implicit operator IModule*(Module module) => module.Handle;
}
