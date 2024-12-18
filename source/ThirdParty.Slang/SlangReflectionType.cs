using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace ThirdParty.Slang;

public unsafe class SlangReflectionType : ManagedSlang<SlangReflectionType>
{
    [field: AllowNull]
    public SlangReflectionType? ElementType => field ??= PInvoke.spReflectionType_GetElementType(this.Handle) is var x && x != default ? new(x) : null;

    // TODO: Report crash
    // [field: AllowNull]
    // public SlangReflectionGeneric? GenericContainer => field ??= PInvoke.spReflectionType_GetGenericContainer(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionType_GetName(this.Handle))!;

    [field: AllowNull]
    public SlangReflectionType? ResourceResultType => field ??= PInvoke.spReflectionType_GetResourceResultType(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public SlangReflectionVariable[] Fields
    {
        get
        {
            if (field == null)
            {
                field = new SlangReflectionVariable[this.FieldCount];

                for (var i = 0; i < field.Length; i++)
                {
                    field[i] = this.GetFieldByIndex((uint)i);
                }
            }

            return field;
        }
    }

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

    public uint                ColumnCount             => PInvoke.spReflectionType_GetColumnCount(this.Handle);
    public ulong               ElementCount            => PInvoke.spReflectionType_GetElementCount(this.Handle);
    public uint                FieldCount              => PInvoke.spReflectionType_GetFieldCount(this.Handle);
    public SlangTypeKind       Kind                    => PInvoke.spReflectionType_GetKind(this.Handle);
    public SlangResourceAccess ResourceAccess          => PInvoke.spReflectionType_GetResourceAccess(this.Handle);
    public SlangResourceShape  ResourceShape           => PInvoke.spReflectionType_GetResourceShape(this.Handle);
    public uint                RowCount                => PInvoke.spReflectionType_GetRowCount(this.Handle);
    public SlangScalarType     ScalarType              => PInvoke.spReflectionType_GetScalarType(this.Handle);
    public long                SpecializedTypeArgCount => PInvoke.spReflectionType_getSpecializedTypeArgCount(this.Handle);
    public uint                UserAttributeCount      => PInvoke.spReflectionType_GetUserAttributeCount(this.Handle);

    internal SlangReflectionType(Handle<SlangReflectionType> handle) : base(handle)
    { }

    public SlangReflectionType ApplySpecializations(SlangReflectionGeneric generic) =>
        new(PInvoke.spReflectionType_applySpecializations(this.Handle, generic.Handle));

    public SlangReflectionUserAttribute FindUserAttributeByName(string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            return new(PInvoke.spReflectionType_FindUserAttributeByName(this.Handle, pName));
        }
    }

    public SlangReflectionVariable GetFieldByIndex(uint index) =>
        new(PInvoke.spReflectionType_GetFieldByIndex(this.Handle, index));

    public SlangReflectionType GetSpecializedTypeArgType(long index) =>
        new(PInvoke.spReflectionType_getSpecializedTypeArgType(this.Handle, index));

    public SlangReflectionUserAttribute GetUserAttribute(uint index) =>
        new(PInvoke.spReflectionType_GetUserAttribute(this.Handle, index));
}
