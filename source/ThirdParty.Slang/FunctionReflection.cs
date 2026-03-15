using System.Text;
using Age.Core;
using Age.Core.Extensions;

namespace ThirdParty.Slang;

public unsafe class FunctionReflection : Managed<FunctionReflection>
{
    public GenericReflection GenericContainer   => field ??= new(this.Session, PInvoke.spReflectionFunction_GetGenericContainer(this.Handle));
    public bool              IsOverloaded       => PInvoke.spReflectionFunction_isOverloaded(this.Handle);
    public string            Name               => field ??= Encoding.GetStringFromNullTerminated(PInvoke.spReflectionFunction_GetName(this.Handle))!;
    public uint              OverloadCount      => PInvoke.spReflectionFunction_getOverloadCount(this.Handle);
    public TypeReflection    ReturnType         => field ??= new(this.Session, PInvoke.spReflectionFunction_GetResultType(this.Handle));
    public uint              UserAttributeCount => PInvoke.spReflectionFunction_GetUserAttributeCount(this.Handle);

    internal FunctionReflection(Session session, Handle<FunctionReflection> handle) : base(session, handle)
    { }

    public FunctionReflection ApplySpecializations(GenericReflection generic) =>
        new(this.Session, PInvoke.spReflectionFunction_applySpecializations(this.Handle, generic.Handle));

    public VariableReflection GetParameterByIndex(uint index) =>
        new(this.Session, PInvoke.spReflectionFunction_GetParameter(this.Handle, index));

    public FunctionReflection GetOverload(uint index) =>
        new(this.Session, PInvoke.spReflectionFunction_getOverload(this.Handle, index));

    public Attribute GetUserAttributeByIndex(uint index) =>
        new(this.Session, PInvoke.spReflectionFunction_GetUserAttribute(this.Handle, index));

    public Attribute? FindAttributeByName(string name)
    {
        var uName = new UnmanagedString(name);

        var handle = PInvoke.spReflectionFunction_FindUserAttributeByName(this.Handle, this.Session.GlobalSession.Handle, uName);

        return handle == default ? null : new(this.Session, handle);
    }

    public Attribute? FindUserAttributeByName(string name) =>
        this.FindAttributeByName(name);

    public SlangReflectionModifier FindModifier(SlangModifierID id) =>
        PInvoke.spReflectionFunction_FindModifier(this.Handle, id);

    public FunctionReflection SpecializeWithArgTypes(ReadOnlySpan<TypeReflection> types)
    {
        var pTypes = stackalloc Handle<TypeReflection>[types.Length];

        for (var i = 0; i < types.Length; i++)
        {
            pTypes[i] = types[i].Handle;
        }

        return new(this.Session, PInvoke.spReflectionFunction_specializeWithArgTypes(this.Handle, types.Length, pTypes));
    }
}
