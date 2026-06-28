using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Age.Core;

namespace ThirdParty.Slang;

public unsafe class VariableReflection : Managed<VariableReflection>
{
    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionVariable_GetName(this.Handle))!;

    [field: AllowNull]
    public GenericReflection? GenericContainer => field ??= PInvoke.spReflectionVariable_GetGenericContainer(this.Handle) is var x && x != default ? new(this.Session, x) : null;

    [field: AllowNull]
    public TypeReflection Type => field ??= new(this.Session, PInvoke.spReflectionVariable_GetType(this.Handle));

    [field: AllowNull]
    public Attribute[] UserAttributes
    {
        get
        {
            if (field == null)
            {
                field = new Attribute[this.UserAttributeCount];

                for (var i = 0; i < field.Length; i++)
                {
                    field[i] = this.GetUserAttribute((uint)i);
                }
            }

            return field;
        }
    }

    public bool HasDefaultValue    => PInvoke.spReflectionVariable_HasDefaultValue(this.Handle);
    public uint UserAttributeCount => PInvoke.spReflectionVariable_GetUserAttributeCount(this.Handle);

    internal VariableReflection(Session session, Handle<VariableReflection> handle) : base(session, handle) { }

    public VariableReflection ApplySpecializations(GenericReflection generic) =>
        new(this.Session, PInvoke.spReflectionVariable_applySpecializations(this.Handle, generic.Handle));

    public SlangReflectionModifier FindModifier(SlangModifierID modifierID) =>
        PInvoke.spReflectionVariable_FindModifier(this.Handle, modifierID);

    public Attribute? FindUserAttributeByName(string name)
    {
        using var pName = new NativeString(name);

        var handle = PInvoke.spReflectionVariable_FindUserAttributeByName(this.Handle, this.Session.GlobalSession.Handle, pName);

        return handle == default ? null : new(this.Session, handle);
    }

    public Attribute GetUserAttribute(uint index) =>
        new(this.Session, PInvoke.spReflectionVariable_GetUserAttribute(this.Handle, index));
}
