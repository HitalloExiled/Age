using Silk.NET.OpenGLES;
using Silk.NET.OpenGLES.Extensions.OVR;

namespace Age.Drivers.GLES3;

public static class Wrapper
{
    public static readonly GL           GL;
    public static readonly OvrMultiview OVR;

    static Wrapper()
    {
        var context = GL.CreateDefaultContext("Age");

        GL  = new(context);
        OVR = new(context);
    }
}
