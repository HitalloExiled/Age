using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

public unsafe class SlangReflectionVariableLayout : ManagedSlang<SlangReflectionVariableLayout>
{
    [field: AllowNull]
    public string SemanticName => field ??= Marshal.PtrToStringAnsi((nint)PInvoke.spReflectionVariableLayout_GetSemanticName(this.Handle))!;

    [field: AllowNull]
    public SlangReflectionVariableLayout? PendingDataLayout => field ??= PInvoke.spReflectionVariableLayout_getPendingDataLayout(this.Handle) is var x && x != default ? new(x) : null;

    [field: AllowNull]
    public SlangReflectionTypeLayout TypeLayout => field ??= new(PInvoke.spReflectionVariableLayout_GetTypeLayout(this.Handle));

    [field: AllowNull]
    public SlangReflectionVariable? Variable => field ??= PInvoke.spReflectionVariableLayout_GetVariable(this.Handle) is var x && x != default ? new(x) : null;

    public SlangStage Stage => PInvoke.spReflectionVariableLayout_getStage(this.Handle);

    internal SlangReflectionVariableLayout(Handle<SlangReflectionVariableLayout> handle) : base(handle)
    { }

    public ulong GetOffset(SlangParameterCategory category) =>
        PInvoke.spReflectionVariableLayout_GetOffset(this.Handle, category);

    public ulong GetSpace(SlangParameterCategory category) =>
        PInvoke.spReflectionVariableLayout_GetSpace(this.Handle, category);
}
