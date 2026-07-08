namespace Age.Platforms.Linux.LibXKBCommon;

internal enum xkb_compose_status
{
    XKB_COMPOSE_NOTHING,
    XKB_COMPOSE_COMPOSING,
    XKB_COMPOSE_COMPOSED,
    XKB_COMPOSE_CANCELLED
}
