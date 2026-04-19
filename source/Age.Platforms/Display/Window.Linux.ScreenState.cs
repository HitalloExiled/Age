#if LINUX
namespace Age.Platforms.Display;

public partial class Window
{
    private struct ScreenState
    {
        public ScreenData Data;
        public ScreenData PendingData;
    }
}
#endif
