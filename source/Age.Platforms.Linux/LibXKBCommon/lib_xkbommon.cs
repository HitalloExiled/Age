using System.Runtime.InteropServices;

namespace Age.Platforms.Linux.LibXKBCommon;

internal struct xkb_context;
internal struct xkb_keymap;
internal struct xkb_state;

internal unsafe static partial class lib_xkbommon
{
    public const uint XKB_KEYCODE_INVALID = 0xffffffff;
    public const uint XKB_LAYOUT_INVALID  = 0xffffffff;
    public const uint XKB_LED_INVALID     = 0xffffffff;
    public const uint XKB_LEVEL_INVALID   = 0xffffffff;
    public const uint XKB_MOD_INVALID     = 0xffffffff;

    public const uint XKB_KEYCODE_MAX = 0xffffffff - 1;

    public const string XKB_MOD_NAME_SHIFT = "Shift";
    public const string XKB_MOD_NAME_CAPS  = "Lock";
    public const string XKB_MOD_NAME_CTRL  = "Control";
    public const string XKB_MOD_NAME_ALT   = "Mod1";
    public const string XKB_MOD_NAME_NUM   = "Mod2";
    public const string XKB_MOD_NAME_LOGO  = "Mod4";

    public const string XKB_LED_NAME_CAPS   = "Caps Lock";
    public const string XKB_LED_NAME_NUM    = "Num Lock";
    public const string XKB_LED_NAME_SCROLL = "Scroll Lock";

    private const string LIBRARY = "libxkbcommon.so.0";

    [LibraryImport(LIBRARY)]
    public static partial xkb_context* xkb_context_new(xkb_context_flags flags);

    [LibraryImport(LIBRARY)]
    public static partial void xkb_context_unref(xkb_context* context);

    [LibraryImport(LIBRARY)]
    public static partial int xkb_keymap_key_get_syms_by_level(
        xkb_keymap*        keymap,
        xkb_keycode_t      key,
        xkb_layout_index_t layout,
        xkb_level_index_t  level,
        xkb_keysym_t**     syms_out
    );

    [LibraryImport(LIBRARY)]
    public static partial int xkb_keymap_key_repeats(xkb_keymap* keymap, xkb_keycode_t key);

    [LibraryImport(LIBRARY)]
    public static partial xkb_keymap* xkb_keymap_new_from_string(
        xkb_context*             context,
        byte*                    @string,
        xkb_keymap_format        format,
        xkb_keymap_compile_flags flags
    );

    [LibraryImport(LIBRARY)]
    public static partial void xkb_keymap_unref(xkb_keymap *keymap);

    [LibraryImport(LIBRARY)]
    public static partial xkb_keysym_t xkb_state_key_get_one_sym(xkb_state* state, xkb_keycode_t key);

    [LibraryImport(LIBRARY)]
    public static partial uint32_t xkb_state_key_get_utf32(xkb_state* state, xkb_keycode_t key);

    [LibraryImport(LIBRARY)]
    public static partial xkb_state* xkb_state_new(xkb_keymap* keymap);

    [LibraryImport(LIBRARY)]
    public static partial void xkb_state_unref(xkb_state* state);

    [LibraryImport(LIBRARY)]
    public static partial int xkb_state_mod_name_is_active(xkb_state* state, byte* name, xkb_state_component type);

    [LibraryImport(LIBRARY)]
    public static partial xkb_state_component xkb_state_update_mask(
        xkb_state*         state,
        xkb_mod_mask_t     depressed_mods,
        xkb_mod_mask_t     latched_mods,
        xkb_mod_mask_t     locked_mods,
        xkb_layout_index_t depressed_layout,
        xkb_layout_index_t latched_layout,
        xkb_layout_index_t locked_layout
    );
}
