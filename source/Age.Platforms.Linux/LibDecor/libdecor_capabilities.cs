namespace Age.Platforms.Linux.LibDecor;

internal enum libdecor_capabilities
{
	LIBDECOR_ACTION_MOVE       = 1 << 0,
	LIBDECOR_ACTION_RESIZE     = 1 << 1,
	LIBDECOR_ACTION_MINIMIZE   = 1 << 2,
	LIBDECOR_ACTION_FULLSCREEN = 1 << 3,
	LIBDECOR_ACTION_CLOSE      = 1 << 4,
};
