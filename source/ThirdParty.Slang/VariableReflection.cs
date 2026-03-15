using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Age.Core;

namespace ThirdParty.Slang;

public unsafe class VariableReflection : SessionResource<VariableReflection>
{
    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionVariable_GetName(this.Handle))!;

    // TODO: Report crash
    // [field: AllowNull]
    // public SlangReflectionGeneric? GenericContainer => field ??= PInvoke.spReflectionVariable_GetGenericContainer(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public SlangReflectionType Type => field ??= new(this.Session, PInvoke.spReflectionVariable_GetType(this.Handle));

    [field: AllowNull]
    public SlangReflectionUserAttribute[] UserAttributes
    {
        get
        {
            if (field == null)
            {
                field = new SlangReflectionUserAttribute[this.UserAttributeCount];

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

    public VariableReflection ApplySpecializations(SlangReflectionGeneric generic) =>
        new(this.Session, PInvoke.spReflectionVariable_applySpecializations(this.Handle, generic.Handle));

    public SlangReflectionModifier? FindModifier(SlangModifierID modifierID)
    {
        var handle = PInvoke.spReflectionVariable_FindModifier(this.Handle, modifierID);

        return handle == default ? null : new(handle);
    }

    public SlangReflectionUserAttribute? FindUserAttributeByName(string name)
    {
        using var pName = new UnmanagedString(name);

        var handle = PInvoke.spReflectionVariable_FindUserAttributeByName(this.Handle, this.Session.GlobalSession.Handle, pName);

        return handle == default ? null : new(handle);
    }

    public SlangReflectionUserAttribute GetUserAttribute(uint index) =>
        new(PInvoke.spReflectionVariable_GetUserAttribute(this.Handle, index));
}
