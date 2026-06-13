#if LINUX
using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace Age.Platforms.Display;

public partial class WindowManager
{
    private unsafe struct ScreenState
    {
        public ScreenData Data;
        public ScreenData PendingData;

        public static ScreenState* Allocate() =>
            NativeMemory.Alloc<ScreenState>();

        public static void Free(ScreenState* screenState) =>
            NativeMemory.Free(screenState);
    }
}
#endif
