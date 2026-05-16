#if LINUX
namespace Age.Platforms.Display;

public partial class WindowManager
{
    private struct ScreenState
    {
        public ScreenData Data;
        public ScreenData PendingData;
    }
}
#endif
