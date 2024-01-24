using System.Runtime.InteropServices;
using ThirdParty.SpirvCross.Enums;

namespace ThirdParty.SpirvCross;

public unsafe class Compiler
{
    private readonly Context       context;
    private readonly spvc_compiler handler;

    internal Compiler(spvc_compiler handler, Context context)
    {
        this.handler = handler;
        this.context = context;
    }

    public string Compile()
    {
        byte* source;

        this.context.CheckResult(PInvoke.spvc_compiler_compile(handler, &source));

        return Marshal.PtrToStringAnsi((nint)source)!;
    }

    public Resources CreateShaderResources()
    {
        spvc_resources resources;

        this.context.CheckResult(PInvoke.spvc_compiler_create_shader_resources(this.handler, &resources));

        return new(resources, this.context);
    }

    public uint GetDecoration(uint id, Decoration decoration) =>
        PInvoke.spvc_compiler_get_decoration(handler, id, decoration);
}
