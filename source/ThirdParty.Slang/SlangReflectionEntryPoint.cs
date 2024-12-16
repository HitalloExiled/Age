using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class SlangReflectionEntryPoint : ManagedSlang
{
    public nint Function => PInvoke.spReflectionEntryPoint_getFunction(this.Handle);

    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionEntryPoint_getName(this.Handle))!;

    [field: AllowNull]
    public string NameOverride => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionEntryPoint_getNameOverride(this.Handle))!;

    [field: AllowNull]
    public SlangReflectionVariableLayout[] Parameters
    {
        get
        {
            if (field == null)
            {
                field = new SlangReflectionVariableLayout[this.ParameterCount];

                for (var i = 0; i < field.Length; i++)
                {
                    field[i] = new(PInvoke.spReflectionEntryPoint_getParameterByIndex(this.Handle, (uint)i));
                }
            }

            return field;
        }
    }

    public uint       ParameterCount => PInvoke.spReflectionEntryPoint_getParameterCount(this.Handle);
    public SlangStage Stage          => PInvoke.spReflectionEntryPoint_getStage(this.Handle);

    internal SlangReflectionEntryPoint(nint handle) : base(handle)
    { }

    public SlangReflectionVariableLayout GetParameterByIndex(uint index) =>
        new(PInvoke.spReflectionEntryPoint_getParameterByIndex(this.Handle, index));
}
