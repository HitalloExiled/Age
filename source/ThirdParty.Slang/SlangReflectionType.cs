using System.Runtime.InteropServices;
using System.Text;

namespace ThirdParty.Slang;

public unsafe class SlangReflectionType : ManagedSlang
{
    internal SlangReflectionType(nint handle) : base(handle)
    { }

    public SlangReflectionUserAttributeHandle FindUserAttributeByName(string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            return PInvoke.spReflectionType_FindUserAttributeByName(this.Handle, pName);
        }
    }

    public uint GetColumnCount() =>
        PInvoke.spReflectionType_GetColumnCount(this.Handle);

    public ulong GetElementCount() =>
        PInvoke.spReflectionType_GetElementCount(this.Handle);

    public SlangReflectionTypeHandle GetElementType() =>
        PInvoke.spReflectionType_GetElementType(this.Handle);

    public SlangReflectionVariableLayoutHandle GetFieldByIndex(uint index) =>
        PInvoke.spReflectionType_GetFieldByIndex(this.Handle, index);

    public uint GetFieldCount() =>
        PInvoke.spReflectionType_GetFieldCount(this.Handle);

    public SlangReflectionGenericHandle GetGenericContainer() =>
        PInvoke.spReflectionType_GetGenericContainer(this.Handle);

    public SlangTypeKind GetKind() =>
        PInvoke.spReflectionType_GetKind(this.Handle);

    public string GetName() =>
        Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionType_GetName(this.Handle))!;

    public SlangResourceAccess GetResourceAccess() =>
        PInvoke.spReflectionType_GetResourceAccess(this.Handle);

    public SlangReflectionTypeHandle GetResourceResultType() =>
        PInvoke.spReflectionType_GetResourceResultType(this.Handle);

    public SlangResourceShape GetResourceShape() =>
        PInvoke.spReflectionType_GetResourceShape(this.Handle);

    public uint GetRowCount() =>
        PInvoke.spReflectionType_GetRowCount(this.Handle);

    public SlangScalarType GetScalarType() =>
        PInvoke.spReflectionType_GetScalarType(this.Handle);

    public SlangInt getSpecializedTypeArgCount() =>
        PInvoke.spReflectionType_getSpecializedTypeArgCount(this.Handle);

    public SlangReflectionType getSpecializedTypeArgType(long index) =>
        new(PInvoke.spReflectionType_getSpecializedTypeArgType(this.Handle, index));

    public SlangReflectionUserAttribute GetUserAttribute(uint index) =>
        new(PInvoke.spReflectionType_GetUserAttribute(this.Handle, index));

    public uint GetUserAttributeCount() =>
        PInvoke.spReflectionType_GetUserAttributeCount(this.Handle);
}
