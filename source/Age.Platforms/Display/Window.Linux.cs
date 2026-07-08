#if LINUX
namespace Age.Platforms.Display;

public unsafe partial class Window
{
    public nint Surface => (nint)this.State->Surface;
}
#endif
