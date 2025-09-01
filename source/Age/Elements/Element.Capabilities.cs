using Age.Core.Extensions;

namespace Age.Elements;

public abstract partial class Element
{
    [Flags]
    private enum Capability : byte
    {
        None   = 0,
        Scroll = 1 << 0,
        Focus  = 1 << 1,
    }

    private Capability activeCapabilities;

    protected bool IsFocusable
    {
        get => this.activeCapabilities.HasFlags(Capability.Focus);
        set => this.SetCapability(Capability.Focus, value);
    }

    protected bool IsScrollable
    {
        get => this.activeCapabilities.HasFlags(Capability.Scroll);
        set => this.SetCapability(Capability.Scroll, value);
    }

    private void SetCapability(Capability capability, bool enable)
    {
        if (enable)
        {
            this.activeCapabilities |= capability;
        }
        else
        {
            this.activeCapabilities &= ~capability;
        }
    }
}
