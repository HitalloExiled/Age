using System.Runtime.InteropServices;
using ThirdParty.SpirvCross.Enums;

namespace ThirdParty.SpirvCross;

public sealed unsafe class Compiler
{
    private readonly Context       context;
    private readonly spvc_compiler handle;

    internal Compiler(spvc_compiler handle, Context context)
    {
        this.handle  = handle;
        this.context = context;
    }

    public string Compile()
    {
        byte* source;

        this.context.CheckResult(PInvoke.spvc_compiler_compile(this.handle, &source));

        return Marshal.PtrToStringAnsi((nint)source)!;
    }

    public Resources CreateShaderResources()
    {
        spvc_resources resources;

        this.context.CheckResult(PInvoke.spvc_compiler_create_shader_resources(this.handle, &resources));

        return new(resources, this.context);
    }

    public uint GetDecoration(uint id, Decoration decoration) =>
        PInvoke.spvc_compiler_get_decoration(this.handle, id, decoration);
}
