global using __off_t            = long;
global using int32_t            = int;
global using mode_t             = uint;
global using nfds_t             = ulong;
global using size_t             = ulong;
global using uint32_t           = uint;
global using wl_fixed_t         = int;
global using xkb_keycode_t      = uint;
global using xkb_keysym_t       = uint;
global using xkb_layout_index_t = uint;
global using xkb_level_index_t  = uint;
global using xkb_mod_mask_t     = uint;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Age.Platforms")]
[assembly: InternalsVisibleTo("Age.Rendering")]
[assembly: InternalsVisibleTo("Age.Tests")]
