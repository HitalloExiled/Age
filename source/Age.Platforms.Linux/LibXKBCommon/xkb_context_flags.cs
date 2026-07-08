namespace Age.Platforms.Linux.LibXKBCommon;

public enum xkb_context_flags
{
    XKB_CONTEXT_NO_FLAGS             = 0,
    XKB_CONTEXT_NO_DEFAULT_INCLUDES  = 1 << 0,
    XKB_CONTEXT_NO_ENVIRONMENT_NAMES = 1 << 1
}
