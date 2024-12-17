using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ThirdParty.Slang;

public unsafe class SlangReflectionTypeLayout : ManagedSlang
{
    [field: AllowNull]
    public SlangReflectionVariableLayout? ContainerVarLayout => field ??= PInvoke.spReflectionTypeLayout_getContainerVarLayout(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public SlangReflectionTypeLayout? ElementTypeLayout => field ??= PInvoke.spReflectionTypeLayout_GetElementTypeLayout(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public SlangReflectionVariableLayout? ElementVarLayout => field ??= PInvoke.spReflectionTypeLayout_GetElementVarLayout(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public SlangReflectionVariableLayout? ExplicitCounter => field ??= PInvoke.spReflectionTypeLayout_GetExplicitCounter(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public SlangReflectionVariableLayout[] Fields
    {
        get
        {
            if (field == null)
            {
                field = new SlangReflectionVariableLayout[this.FieldCount];

                for (var i = 0; i < field.Length; i++)
                {
                    field[i] = this.GetFieldByIndex((uint)i);
                }
            }

            return field;
        }
    }

    [field: AllowNull]
    public SlangReflectionTypeLayout? PendingDataTypeLayout => field ??= PInvoke.spReflectionTypeLayout_getPendingDataTypeLayout(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public SlangReflectionVariableLayout? SpecializedTypePendingDataVarLayout => field ??= PInvoke.spReflectionTypeLayout_getSpecializedTypePendingDataVarLayout(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public SlangReflectionType? Type => field ??= PInvoke.spReflectionTypeLayout_GetType(this.Handle) is var x && x != default ? new(x) : null;

    public uint          CategoryCount       => PInvoke.spReflectionTypeLayout_GetCategoryCount(this.Handle);
    public uint          FieldCount          => PInvoke.spReflectionTypeLayout_GetFieldCount(this.Handle);
    public int           GenericParamIndex   => PInvoke.spReflectionTypeLayout_getGenericParamIndex(this.Handle);
    public SlangTypeKind Kind                => PInvoke.spReflectionTypeLayout_getKind(this.Handle);
    public long          SubObjectRangeCount => PInvoke.spReflectionTypeLayout_getSubObjectRangeCount(this.Handle);

    internal SlangReflectionTypeLayout(SlangReflectionTypeLayoutHandle handle) : base(handle)
    { }

    public long FindFieldIndexByName(string nameBegin, string nameEnd)
    {
        fixed (byte* pNameBegin = Encoding.UTF8.GetBytes(nameBegin))
        fixed (byte* pNameEnd   = Encoding.UTF8.GetBytes(nameEnd))
        {
            return PInvoke.spReflectionTypeLayout_findFieldIndexByName(this.Handle, pNameBegin, pNameEnd);
        }
    }

    public int GetAlignment(SlangParameterCategory category) =>
        PInvoke.spReflectionTypeLayout_getAlignment(this.Handle, category);

    public long GetBindingRangeBindingCount(long index) =>
        PInvoke.spReflectionTypeLayout_getBindingRangeBindingCount(this.Handle, index);

    public long GetBindingRangeDescriptorRangeCount(long index) =>
        PInvoke.spReflectionTypeLayout_getBindingRangeDescriptorRangeCount(this.Handle, index);

    public long GetBindingRangeDescriptorSetIndex(long index) =>
        PInvoke.spReflectionTypeLayout_getBindingRangeDescriptorSetIndex(this.Handle, index);

    public long GetBindingRangeFirstDescriptorRangeIndex(long index) =>
        PInvoke.spReflectionTypeLayout_getBindingRangeFirstDescriptorRangeIndex(this.Handle, index);

    public SlangImageFormat GetBindingRangeImageFormat(long index) =>
        PInvoke.spReflectionTypeLayout_getBindingRangeImageFormat(this.Handle, index);

    public SlangReflectionTypeLayout GetBindingRangeLeafTypeLayout(long index) =>
        new(PInvoke.spReflectionTypeLayout_getBindingRangeLeafTypeLayout(this.Handle, index));

    public SlangReflectionVariable GetBindingRangeLeafVariable(long index) =>
        new(PInvoke.spReflectionTypeLayout_getBindingRangeLeafVariable(this.Handle, index));

    public SlangParameterCategory GetDescriptorSetDescriptorRangeCategory(long setIndex, long rangeIndex) =>
        PInvoke.spReflectionTypeLayout_getDescriptorSetDescriptorRangeCategory(this.Handle, setIndex, rangeIndex);

    public long GetDescriptorSetDescriptorRangeCount(long setIndex) =>
        PInvoke.spReflectionTypeLayout_getDescriptorSetDescriptorRangeCount(this.Handle, setIndex);

    public long GetDescriptorSetDescriptorRangeDescriptorCount(long setIndex, long rangeIndex) =>
        PInvoke.spReflectionTypeLayout_getDescriptorSetDescriptorRangeDescriptorCount(this.Handle, setIndex, rangeIndex);

    public long GetDescriptorSetDescriptorRangeIndexOffset(long setIndex, long rangeIndex) =>
        PInvoke.spReflectionTypeLayout_getDescriptorSetDescriptorRangeIndexOffset(this.Handle, setIndex, rangeIndex);

    public SlangBindingType GetDescriptorSetDescriptorRangeType(long setIndex, long rangeIndex) =>
        PInvoke.spReflectionTypeLayout_getDescriptorSetDescriptorRangeType(this.Handle, setIndex, rangeIndex);

    public long GetDescriptorSetSpaceOffset(long setIndex) =>
        PInvoke.spReflectionTypeLayout_getDescriptorSetSpaceOffset(this.Handle, setIndex);

    public ulong GetElementStride(SlangParameterCategory category) =>
        PInvoke.spReflectionTypeLayout_GetElementStride(this.Handle, category);

    public long GetFieldBindingRangeOffset(long fieldIndex) =>
        PInvoke.spReflectionTypeLayout_getFieldBindingRangeOffset(this.Handle, fieldIndex);

    public SlangReflectionVariableLayout GetFieldByIndex(uint index) =>
        new(PInvoke.spReflectionTypeLayout_GetFieldByIndex(this.Handle, index));

    public ulong GetSize(SlangParameterCategory category) =>
        PInvoke.spReflectionTypeLayout_GetSize(this.Handle, category);

    public ulong GetStride(SlangParameterCategory category) =>
        PInvoke.spReflectionTypeLayout_GetStride(this.Handle, category);

    public long GetSubObjectRangeBindingRangeIndex(long subObjectRangeIndex) =>
        PInvoke.spReflectionTypeLayout_getSubObjectRangeBindingRangeIndex(this.Handle, subObjectRangeIndex);

    public long GetSubObjectRangeDescriptorRangeBindingCount(long subObjectRangeIndex, long bindingRangeIndexInSubObject) =>
        PInvoke.spReflectionTypeLayout_getSubObjectRangeDescriptorRangeBindingCount(this.Handle, subObjectRangeIndex, bindingRangeIndexInSubObject);

    public SlangBindingType GetSubObjectRangeDescriptorRangeBindingType(long subObjectRangeIndex, long bindingRangeIndexInSubObject) =>
        PInvoke.spReflectionTypeLayout_getSubObjectRangeDescriptorRangeBindingType(this.Handle, subObjectRangeIndex, bindingRangeIndexInSubObject);

    public long GetSubObjectRangeDescriptorRangeCount(long subObjectRangeIndex) =>
        PInvoke.spReflectionTypeLayout_getSubObjectRangeDescriptorRangeCount(this.Handle, subObjectRangeIndex);

    public long GetSubObjectRangeDescriptorRangeIndexOffset(long subObjectRangeIndex, long bindingRangeIndexInSubObject) =>
        PInvoke.spReflectionTypeLayout_getSubObjectRangeDescriptorRangeIndexOffset(this.Handle, subObjectRangeIndex, bindingRangeIndexInSubObject);

    public long GetSubObjectRangeDescriptorRangeSpaceOffset(long subObjectRangeIndex, long bindingRangeIndexInSubObject) =>
        PInvoke.spReflectionTypeLayout_getSubObjectRangeDescriptorRangeSpaceOffset(this.Handle, subObjectRangeIndex, bindingRangeIndexInSubObject);

    public long GetSubObjectRangeObjectCount(long index) =>
        PInvoke.spReflectionTypeLayout_getSubObjectRangeObjectCount(this.Handle, index);

    public SlangReflectionVariableLayout GetSubObjectRangeOffset(long subObjectRangeIndex) =>
        new(PInvoke.spReflectionTypeLayout_getSubObjectRangeOffset(this.Handle, subObjectRangeIndex));

    public long GetSubObjectRangeSpaceOffset(long subObjectRangeIndex) =>
        PInvoke.spReflectionTypeLayout_getSubObjectRangeSpaceOffset(this.Handle, subObjectRangeIndex);

    public SlangReflectionTypeLayout GetSubObjectRangeTypeLayout(long index) =>
        new(PInvoke.spReflectionTypeLayout_getSubObjectRangeTypeLayout(this.Handle, index));

    public long IsBindingRangeSpecializable(long index) =>
        PInvoke.spReflectionTypeLayout_isBindingRangeSpecializable(this.Handle, index);
}
