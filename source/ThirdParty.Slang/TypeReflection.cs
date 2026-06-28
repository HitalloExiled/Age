using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Age.Core;

namespace ThirdParty.Slang;

public unsafe class TypeReflection : Managed<TypeReflection>
{
    [field: AllowNull]
    public TypeReflection? ElementType => field ??= PInvoke.spReflectionType_GetElementType(this.Handle) is var x && x != default ? new(this.Session, x) : null;

    [field: AllowNull]
    public GenericReflection? GenericContainer => field ??= PInvoke.spReflectionType_GetGenericContainer(this.Handle) is var x && x != default ? new(this.Session, x) : null;

    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionType_GetName(this.Handle))!;

    [field: AllowNull]
    public TypeReflection? ResourceResultType => field ??= PInvoke.spReflectionType_GetResourceResultType(this.Handle) is var x && x != default ? new(this.Session, x) : null;

    [field: AllowNull]
    public VariableReflection[] Fields
    {
        get
        {
            if (field == null)
            {
                field = new VariableReflection[this.FieldCount];

                for (var i = 0; i < field.Length; i++)
                {
                    field[i] = this.GetFieldByIndex((uint)i);
                }
            }

            return field;
        }
    }

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

    internal TypeReflection(Session session, Handle<TypeReflection> handle) : base(session, handle)
    { }

    public TypeReflection ApplySpecializations(GenericReflection generic) =>
        new(this.Session, PInvoke.spReflectionType_applySpecializations(this.Handle, generic.Handle));

    public Attribute FindUserAttributeByName(string name)
    {
        var pName = new NativeString(name);

        return new(this.Session, PInvoke.spReflectionType_FindUserAttributeByName(this.Handle, pName));
    }

    public VariableReflection GetFieldByIndex(uint index) =>
        new(this.Session, PInvoke.spReflectionType_GetFieldByIndex(this.Handle, index));

    public TypeReflection GetSpecializedTypeArgType(long index) =>
        new(this.Session, PInvoke.spReflectionType_getSpecializedTypeArgType(this.Handle, index));

    public Attribute GetUserAttribute(uint index) =>
        new(this.Session, PInvoke.spReflectionType_GetUserAttribute(this.Handle, index));
}
