using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace ThirdParty.Slang;

public unsafe class SlangReflectionVariable : ManagedSlang
{
    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionVariable_GetName(this.Handle))!;

    // TODO: Investigate crash
    // [field: AllowNull]
    // public SlangReflectionGeneric? GenericContainer => field ??= PInvoke.spReflectionVariable_GetGenericContainer(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public SlangReflectionType Type => field ??= new(PInvoke.spReflectionVariable_GetType(this.Handle));

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

    internal SlangReflectionVariable(nint handle) : base(handle)
    { }

    public SlangReflectionVariable ApplySpecializations(SlangReflectionGenericHandle generic) =>
        new(PInvoke.spReflectionVariable_applySpecializations(this.Handle, generic));

    public SlangReflectionModifier? FindModifier(SlangModifierID modifierID)
    {
        var handle = PInvoke.spReflectionVariable_FindModifier(this.Handle, modifierID);

        return handle == default ? null : new(handle);
    }

    public SlangReflectionUserAttribute? FindUserAttributeByName(SlangSession session, string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var handle = PInvoke.spReflectionVariable_FindUserAttributeByName(this.Handle, session.Handle, pName);

            return handle == default ? null : new(handle);
        }
    }

    public SlangReflectionUserAttribute GetUserAttribute(uint index) =>
        new(PInvoke.spReflectionVariable_GetUserAttribute(this.Handle, index));
}
