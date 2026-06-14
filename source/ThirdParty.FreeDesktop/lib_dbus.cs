using System.Runtime.InteropServices;

namespace ThirdParty.FreeDesktop;

internal struct DBusConnection;
internal struct DBusMessage;

internal unsafe static partial class lib_dbus
{
    private const string LIBRARY = "libdbus-1.so";

    #region dbus-protocol
    public const int DBUS_TYPE_INVALID     = 0;
    public const int DBUS_TYPE_BYTE        = 'y';
    public const int DBUS_TYPE_BOOLEAN     = 'b';
    public const int DBUS_TYPE_INT16       = 'n';
    public const int DBUS_TYPE_UINT16      = 'q';
    public const int DBUS_TYPE_INT32       = 'i';
    public const int DBUS_TYPE_UINT32      = 'u';
    public const int DBUS_TYPE_INT64       = 'x';
    public const int DBUS_TYPE_UINT64      = 't';
    public const int DBUS_TYPE_DOUBLE      = 'd';
    public const int DBUS_TYPE_STRING      = 's';
    public const int DBUS_TYPE_OBJECT_PATH = 'o';
    public const int DBUS_TYPE_SIGNATURE   = 'g';
    public const int DBUS_TYPE_UNIX_FD     = 'h';
    public const int DBUS_TYPE_ARRAY       = 'a';
    public const int DBUS_TYPE_VARIANT     = 'v';
    public const int DBUS_TYPE_STRUCT      = 'r';
    public const int DBUS_TYPE_DICT_ENTRY  = 'e';

    public const int DBUS_TIMEOUT_INFINITE    = 0x7fffffff;
    public const int DBUS_TIMEOUT_USE_DEFAULT = -1;
    #endregion

    #region dbus-bus
    [LibraryImport(LIBRARY)]
    public static partial DBusConnection* dbus_bus_get(DBusBusType type, DBusError* error);

    [LibraryImport(LIBRARY)]
    public static partial byte* dbus_bus_get_unique_name(DBusConnection* connection);

    [LibraryImport(LIBRARY)]
    public static partial void dbus_bus_add_match(DBusConnection* connection, byte* rule, DBusError* error);

    [LibraryImport(LIBRARY)]
    public static partial void dbus_bus_remove_match(DBusConnection* connection, byte* rule, DBusError* error);
    #endregion

    #region dbus-connection
    [LibraryImport(LIBRARY)]
    public static partial void dbus_connection_unref(DBusConnection* connection);

    [LibraryImport(LIBRARY)]
    public static partial DBusMessage* dbus_connection_send_with_reply_and_block(DBusConnection* connection, DBusMessage* message, int timeout_milliseconds, DBusError* error);

    [LibraryImport(LIBRARY)]
    public static partial dbus_bool_t dbus_connection_read_write(DBusConnection* connection, int timeout_milliseconds);

    [LibraryImport(LIBRARY)]
    public static partial DBusMessage* dbus_connection_pop_message(DBusConnection* connection);
    #endregion

    #region dbus-error
    [LibraryImport(LIBRARY)]
    public static partial void dbus_error_init(DBusError* error);

    [LibraryImport(LIBRARY)]
    public static partial void dbus_error_free(DBusError* error);

    [LibraryImport(LIBRARY)]
    public static partial dbus_bool_t dbus_error_is_set(DBusError* error);
    #endregion

    #region dbus-message
    [LibraryImport(LIBRARY)]
    public static partial DBusMessage* dbus_message_new_method_call(byte* bus_name, byte* path, byte* iface, byte* method);

    [LibraryImport(LIBRARY)]
    public static partial void dbus_message_unref(DBusMessage* message);

    [LibraryImport(LIBRARY)]
    public static partial dbus_bool_t dbus_message_is_signal(DBusMessage* message, byte* iface, byte* signal_name);

    [LibraryImport(LIBRARY)]
    public static partial byte* dbus_message_get_path(DBusMessage* message);
    #endregion

    #region dbus-message-iter
    [LibraryImport(LIBRARY)]
    public static partial dbus_bool_t dbus_message_iter_init(DBusMessage* message, DBusMessageIter* iter);

    [LibraryImport(LIBRARY)]
    public static partial int dbus_message_iter_get_arg_type(DBusMessageIter* iter);

    [LibraryImport(LIBRARY)]
    public static partial dbus_bool_t dbus_message_iter_next(DBusMessageIter* iter);

    [LibraryImport(LIBRARY)]
    public static partial void dbus_message_iter_get_basic(DBusMessageIter* iter, void* value);

    [LibraryImport(LIBRARY)]
    public static partial void dbus_message_iter_recurse(DBusMessageIter* iter, DBusMessageIter* sub);

    [LibraryImport(LIBRARY)]
    public static partial void dbus_message_iter_init_append(DBusMessage* message, DBusMessageIter* iter);

    [LibraryImport(LIBRARY)]
    public static partial dbus_bool_t dbus_message_iter_append_basic(DBusMessageIter* iter, int type, void* value);

    [LibraryImport(LIBRARY)]
    public static partial dbus_bool_t dbus_message_iter_open_container(DBusMessageIter* iter, int type, byte* contained_signature, DBusMessageIter* sub);

    [LibraryImport(LIBRARY)]
    public static partial dbus_bool_t dbus_message_iter_close_container(DBusMessageIter* iter, DBusMessageIter* sub);
    #endregion
}
