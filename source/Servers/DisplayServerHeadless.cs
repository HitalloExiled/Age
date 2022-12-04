using Age.Core.Error;
using Age.Core.Math;

namespace Age.Servers;

internal class DisplayServerHeadless : DisplayServer
{
    public override bool CanAnyWindowDraw => throw new NotImplementedException();

    public static DisplayServer CreateFunc(string renderingDriver, WindowMode windowMode, VSyncMode windowVsyncMode, WindowFlagsBit windowFlags, out Vector2<int> windowPosition, Vector2<int> windowSize, out ErrorType err) => throw new NotImplementedException();

    public static List<string> GetRenderingDriversFunc() => throw new NotImplementedException();

    public override bool HasFeature(Feature feature) => throw new NotImplementedException();
    public override void ScreenSetOrientation(ScreenOrientation orientation) => throw new NotImplementedException();
    public override bool WindowCanDraw() => throw new NotImplementedException();
    public override void WindowSetCurrentScreen(int initScreen) => throw new NotImplementedException();
    public override void WindowSetFlag(WindowFlags flag, bool enabled) => throw new NotImplementedException();
    public override void WindowSetMode(WindowMode wINDOW_MODE_MAXIMIZED) => throw new NotImplementedException();
    public override void WindowSetPosition(Vector2<int> position) => throw new NotImplementedException();
}
