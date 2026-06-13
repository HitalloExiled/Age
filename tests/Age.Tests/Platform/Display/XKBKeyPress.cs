#if LINUX
using Age.Core.Extensions;
using Age.Platforms.Linux.LibXKBCommon;
using Age.Platforms.Linux;
using System.Text;

using static Age.Platforms.Linux.LibXKBCommon.lib_xkbommon;

namespace Age.Tests.Platform.Display;

#pragma warning disable IDE1006

public class XKBKeyPress
{
    [Fact]
    public unsafe void KeyPress()
    {
        const uint KEY_a     = (uint)input_event_codes.KEY_A + 8;
        const uint TILDE_KEY = (uint)input_event_codes.KEY_APOSTROPHE + 8;

        var context = xkb_context_new(xkb_context_flags.XKB_CONTEXT_NO_FLAGS);

        Assert.NotNull(context);

        using var rules   = "evdev".ToUnmanaged();
        using var model   = "pc105".ToUnmanaged();
        using var layout  = "br".ToUnmanaged();
        using var variant = "abnt2".ToUnmanaged();

        var names = new xkb_rule_names
        {
            rules   = rules,
            model   = model,
            layout  = layout,
            variant = variant,
            options = null
        };

        var keymap = xkb_keymap_new_from_names(context, &names, xkb_keymap_compile_flags.XKB_KEYMAP_COMPILE_NO_FLAGS);

        Assert.NotNull(keymap);

        var state = xkb_state_new(keymap);

        Assert.NotNull(state);

        using var locale = "C".ToUnmanaged();

        var composeTable = xkb_compose_table_new_from_locale(context, locale, xkb_compose_compile_flags.XKB_COMPOSE_COMPILE_NO_FLAGS);

        Assert.NotNull(composeTable);

        var composeState = xkb_compose_state_new(composeTable, xkb_compose_state_flags.XKB_COMPOSE_STATE_NO_FLAGS);

        Assert.NotNull(composeState);

        var sym_tilde = xkb_state_key_get_one_sym(state, TILDE_KEY);

        Assert.Equal((uint)xkbcommon_keysyms.XKB_KEY_dead_tilde, sym_tilde);

        xkb_compose_state_feed(composeState, sym_tilde);

        Assert.Equal(xkb_compose_status.XKB_COMPOSE_COMPOSING, xkb_compose_state_get_status(composeState));

        var sym_a = xkb_state_key_get_one_sym(state, KEY_a);

        Assert.Equal((uint)xkbcommon_keysyms.XKB_KEY_a, sym_a);

        xkb_compose_state_feed(composeState, sym_a);

        Assert.Equal(xkb_compose_status.XKB_COMPOSE_COMPOSED, xkb_compose_state_get_status(composeState));

        var composed = stackalloc byte[16];

        _ = xkb_compose_state_get_utf8(composeState, composed, 16 - 1);

        Assert.Equal("ã", Encoding.GetStringFromNullTerminated(composed));

        xkb_compose_state_unref(composeState);
        xkb_compose_table_unref(composeTable);
        xkb_keymap_unref(keymap);
        xkb_state_unref(state);
        xkb_context_unref(context);
    }
}
#endif
