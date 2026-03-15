using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class EntryPointReflection : Managed<EntryPointReflection>
{
    public FunctionReflection Function => field ??= new(this.Session, PInvoke.spReflectionEntryPoint_getFunction(this.Handle));

    [field: AllowNull]
    public string Name => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionEntryPoint_getName(this.Handle))!;

    [field: AllowNull]
    public string NameOverride => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionEntryPoint_getNameOverride(this.Handle))!;

    [field: AllowNull]
    public VariableLayoutReflection[] Parameters
    {
        get
        {
            if (field == null)
            {
                field = new VariableLayoutReflection[this.ParameterCount];

                for (var i = 0; i < field.Length; i++)
                {
                    field[i] = this.GetParameterByIndex((uint)i);
                }
            }

            return field;
        }
    }

    public int                           HasDefaultConstantBuffer => PInvoke.spReflectionEntryPoint_hasDefaultConstantBuffer(this.Handle);
    public uint                          ParameterCount           => PInvoke.spReflectionEntryPoint_getParameterCount(this.Handle);
    public VariableLayoutReflection      ResultVarLayout          => field ??= new(this.Session, PInvoke.spReflectionEntryPoint_getResultVarLayout(this.Handle));
    public SlangStage                    Stage                    => PInvoke.spReflectionEntryPoint_getStage(this.Handle);
    public int                           UsesAnySampleRateInput   => PInvoke.spReflectionEntryPoint_usesAnySampleRateInput(this.Handle);
    public VariableLayoutReflection      VarLayout                => field ??= new(this.Session, PInvoke.spReflectionEntryPoint_getVarLayout(this.Handle));
    public TypeLayoutReflection          TypeLayout               => this.VarLayout.TypeLayout;

    internal EntryPointReflection(Session session, Handle<EntryPointReflection> handle) : base(session, handle)
    { }

    public void GetComputeThreadGroupSize(ulong axisCount, ReadOnlySpan<ulong> outSizeAlongAxis)
    {
        fixed (ulong* pOutSizeAlongAxis = outSizeAlongAxis)
        {
            PInvoke.spReflectionEntryPoint_getComputeThreadGroupSize(this.Handle, axisCount, pOutSizeAlongAxis);
        }
    }

    public void GetComputeWaveSize(ReadOnlySpan<ulong> outWaveSize)
    {
        fixed (ulong* pOutWaveSize = outWaveSize)
        {
            PInvoke.spReflectionEntryPoint_getComputeWaveSize(this.Handle, pOutWaveSize);
        }
    }

    public VariableLayoutReflection GetParameterByIndex(uint index) =>
        new(this.Session, PInvoke.spReflectionEntryPoint_getParameterByIndex(this.Handle, index));
}
