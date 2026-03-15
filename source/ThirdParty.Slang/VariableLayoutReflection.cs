using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class VariableLayoutReflection : Managed<VariableLayoutReflection>
{
    [field: AllowNull]
    public string SemanticName => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionVariableLayout_GetSemanticName(this.Handle))!;

    [field: AllowNull]
    public VariableLayoutReflection? PendingDataLayout => field ??= PInvoke.spReflectionVariableLayout_getPendingDataLayout(this.Handle) is var x && x != default ? new(this.Session, x) : null;

    [field: AllowNull]
    public TypeLayoutReflection TypeLayout => field ??= new(this.Session, PInvoke.spReflectionVariableLayout_GetTypeLayout(this.Handle));

    [field: AllowNull]
    public VariableReflection? Variable => field ??= PInvoke.spReflectionVariableLayout_GetVariable(this.Handle) is var x && x != default ? new(this.Session, x) : null;

    public uint       BindingIndex    => PInvoke.spReflectionParameter_GetBindingIndex(this.Handle);
    public uint       BindingSpace    => PInvoke.spReflectionParameter_GetBindingSpace(this.Handle);
    public ulong      ParameterOffset => PInvoke.spReflectionVariableLayout_GetOffset(this.Handle, this.TypeLayout.ParameterCategory);
    public ulong      ParameterSpace  =>  PInvoke.spReflectionVariableLayout_GetSpace(this.Handle, this.TypeLayout.ParameterCategory);
    public ulong      SemanticIndex   => PInvoke.spReflectionVariableLayout_GetSemanticIndex(this.Handle);
    public SlangStage Stage           => PInvoke.spReflectionVariableLayout_getStage(this.Handle);

    internal VariableLayoutReflection(Session session, Handle<VariableLayoutReflection> handle) : base(session, handle)
    { }

    public ulong GetOffset(SlangParameterCategory category) =>
        PInvoke.spReflectionVariableLayout_GetOffset(this.Handle, category);

    public ulong GetSpace(SlangParameterCategory category) =>
        PInvoke.spReflectionVariableLayout_GetSpace(this.Handle, category);
}
