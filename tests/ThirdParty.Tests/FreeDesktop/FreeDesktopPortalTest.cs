#if LINUX
using Age.Numerics;
using ThirdParty.FreeDesktop;

namespace ThirdParty.Tests.FreeDesktop;

public class FreeDesktopPortalTest : IDisposable
{
    private readonly FreeDesktopPortal portal = new();

    private bool disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.portal.Dispose();
            }

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void ReadSettingReturnsNullForUnknownKey()
    {
        Assert.False(this.portal.TryReadSetting("org.freedesktop.appearance", "nonexistent-setting", out var value));
        Assert.Null(value);
    }

    [Fact]
    public void GenericReadSettingReturnsNullForUnknownKey()
    {
        Assert.False(this.portal.TryReadSetting<uint>("org.freedesktop.appearance", "nonexistent-setting", out var value));
        Assert.True(value == default);
    }

    [Fact]
    public void NonGenericAndGenericReturnSameForUnknownKey()
    {
        Assert.False(this.portal.TryReadSetting("org.freedesktop.appearance", "nonexistent-setting", out var nonGeneric));
        Assert.False(this.portal.TryReadSetting<uint>("org.freedesktop.appearance", "nonexistent-setting", out var generic));
        Assert.Equal((uint?)nonGeneric ?? 0, generic);
    }

    [Fact]
    public void GenericReadSettingReturnsNullForMismatchedType()
    {
        Assert.True(this.portal.TryReadSetting<Color>("org.freedesktop.appearance", "accent-color", out var value));
        Assert.True(value != default);
    }

    [Fact]
    public void RefreshDoesNotThrow() =>
        this.portal.Refresh();
}
#endif
