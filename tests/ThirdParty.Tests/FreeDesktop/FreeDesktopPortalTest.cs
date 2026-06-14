#if LINUX
using Age.Numerics;
using ThirdParty.FreeDesktop;

namespace ThirdParty.Tests.FreeDesktop;

public class FreeDesktopPortalTest : IDisposable
{
    private readonly FreeDesktopPortal portal = new();

    public void Dispose() => this.portal.Dispose();

    [Fact]
    public void ReadSettingReturnsNullForUnknownKey()
    {
        var result = this.portal.ReadSetting("org.freedesktop.appearance", "nonexistent-setting");

        Assert.Null(result);
    }

    [Fact]
    public void GenericReadSettingReturnsNullForUnknownKey()
    {
        var result = this.portal.ReadSetting<uint>("org.freedesktop.appearance", "nonexistent-setting");

        Assert.Null(result);
    }

    [Fact]
    public void NonGenericAndGenericReturnSameForUnknownKey()
    {
        var nonGeneric = this.portal.ReadSetting("org.freedesktop.appearance", "nonexistent-setting");
        var generic = this.portal.ReadSetting<uint>("org.freedesktop.appearance", "nonexistent-setting");

        Assert.Equal(nonGeneric, generic);
        Assert.Null(nonGeneric);
        Assert.Null(generic);
    }

    [Fact]
    public void GenericReadSettingReturnsNullForMismatchedType()
    {
        var result = this.portal.ReadSetting<Color>("org.freedesktop.appearance", "accent-color");

        Assert.Null(result);
    }

    [Fact]
    public void RefreshDoesNotThrow() =>
        this.portal.Refresh();

    [Fact]
    public void IsAvailableDoesNotThrow() =>
        _ = this.portal.IsAvailable;

    [Fact]
    public void PropertyAccessorsDoNotThrow()
    {
        _ = this.portal.ColorScheme;
        _ = this.portal.AccentColor;
        _ = this.portal.HighContrast;
        }
}
#endif
