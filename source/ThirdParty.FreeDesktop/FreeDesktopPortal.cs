using System.Runtime.InteropServices;
using Age.Core;
using Age.Numerics;

namespace ThirdParty.FreeDesktop;

public sealed unsafe class FreeDesktopPortal : Disposable
{
    private const string BUS_NAME       = "org.freedesktop.portal.Desktop";
    private const string BUS_PATH       = "/org/freedesktop/portal/desktop";
    private const string MATCH_RULE     = "type='signal',sender='org.freedesktop.portal.Desktop',interface='org.freedesktop.portal.Settings',member='SettingChanged'";
    private const string SETTINGS_IFACE = "org.freedesktop.portal.Settings";
    private const int    TIMEOUT_MS     = 5000;

    private readonly Lock cacheLock = new();

    private Color?                    accentColor;
    private ColorScheme               colorScheme;
    private DBusConnection*           connection;
    private bool?                     highContrast;
    private bool                      isAvailable;
    private Thread?                   monitorThread;
    private readonly ManualResetEvent stopRequested = new(false);

    public Color? AccentColor
    {
        get
        {
            lock (this.cacheLock)
            {
                return this.accentColor;
            }
        }
    }

    public ColorScheme ColorScheme
    {
        get
        {
            lock (this.cacheLock)
            {
                return this.colorScheme;
            }
        }
    }

    public bool? HighContrast
    {
        get
        {
            lock (this.cacheLock)
            {
                return this.highContrast;
            }
        }
    }

    public bool IsAvailable => this.isAvailable;

    public event Action? SettingsChanged;

    public FreeDesktopPortal()
    {
        DBusError error;
        lib_dbus.dbus_error_init(&error);

        this.connection = lib_dbus.dbus_bus_get(DBusBusType.DBUS_BUS_SESSION, &error);
        if (lib_dbus.dbus_error_is_set(&error) != 0)
        {
            lib_dbus.dbus_error_free(&error);
            return;
        }

        this.isAvailable = true;
        this.AddMatchRule();
        this.Refresh();

        this.monitorThread = new Thread(this.MonitorLoop) { IsBackground = true, Name = "PortalMonitor" };
        this.monitorThread.Start();
    }

    private static T GetBasic<T>(DBusMessageIter* iter) where T : unmanaged
    {
        T val;
        lib_dbus.dbus_message_iter_get_basic(iter, &val);
        return val;
    }

    private static object?[]? GetContainer(DBusMessageIter* iter)
    {
        DBusMessageIter sub;
        lib_dbus.dbus_message_iter_recurse(iter, &sub);

        var list = new List<object?>();
        while (lib_dbus.dbus_message_iter_get_arg_type(&sub) != lib_dbus.DBUS_TYPE_INVALID)
        {
            list.Add(ReadIterValue(&sub));
            if (lib_dbus.dbus_message_iter_next(&sub) == 0)
            {
                break;
            }
        }
        return [.. list];
    }

    private static string? GetString(DBusMessageIter* iter)
    {
        byte* str;
        lib_dbus.dbus_message_iter_get_basic(iter, &str);
        return Marshal.PtrToStringUTF8((IntPtr)str);
    }

    private static object? ParseVariantReply(DBusMessage* reply)
    {
        DBusMessageIter iter0, iter1, iter2;

        if (lib_dbus.dbus_message_iter_init(reply, &iter0) == 0)
        {
            return null;
        }

        if (lib_dbus.dbus_message_iter_get_arg_type(&iter0) != lib_dbus.DBUS_TYPE_VARIANT)
        {
            return null;
        }

        lib_dbus.dbus_message_iter_recurse(&iter0, &iter1);

        if (lib_dbus.dbus_message_iter_get_arg_type(&iter1) != lib_dbus.DBUS_TYPE_VARIANT)
        {
            return null;
        }

        lib_dbus.dbus_message_iter_recurse(&iter1, &iter2);
        return ReadIterValue(&iter2);
    }

    private static object? ReadIterValue(DBusMessageIter* iter)
    {
        var type = lib_dbus.dbus_message_iter_get_arg_type(iter);

        if (type == lib_dbus.DBUS_TYPE_VARIANT)
        {
            DBusMessageIter sub;
            lib_dbus.dbus_message_iter_recurse(iter, &sub);
            return ReadIterValue(&sub);
        }

        return type is lib_dbus.DBUS_TYPE_STRUCT or lib_dbus.DBUS_TYPE_ARRAY
            ? GetContainer(iter)
            : type switch
            {
                int t when t == lib_dbus.DBUS_TYPE_UINT32                                                                   => GetBasic<uint>(iter),
                int t when t == lib_dbus.DBUS_TYPE_BOOLEAN                                                                  => GetBasic<uint>(iter) != 0,
                int t when t == lib_dbus.DBUS_TYPE_DOUBLE                                                                   => GetBasic<double>(iter),
                int t when t == lib_dbus.DBUS_TYPE_BYTE                                                                     => GetBasic<byte>(iter),
                int t when t == lib_dbus.DBUS_TYPE_INT16                                                                    => GetBasic<short>(iter),
                int t when t == lib_dbus.DBUS_TYPE_UINT16                                                                   => GetBasic<ushort>(iter),
                int t when t == lib_dbus.DBUS_TYPE_INT32                                                                    => GetBasic<int>(iter),
                int t when t == lib_dbus.DBUS_TYPE_INT64                                                                    => GetBasic<long>(iter),
                int t when t == lib_dbus.DBUS_TYPE_UINT64                                                                   => GetBasic<ulong>(iter),
                int t when t is lib_dbus.DBUS_TYPE_STRING or lib_dbus.DBUS_TYPE_OBJECT_PATH or lib_dbus.DBUS_TYPE_SIGNATURE => GetString(iter),
                _ => null
            };
    }

    private void AddMatchRule()
    {
        using var rule = new UnmanagedString(MATCH_RULE);

        DBusError error;
        lib_dbus.dbus_error_init(&error);
        lib_dbus.dbus_bus_add_match(this.connection, rule, &error);

        if (lib_dbus.dbus_error_is_set(&error) != 0)
        {
            lib_dbus.dbus_error_free(&error);
        }
    }

    private void MonitorLoop()
    {
        using var signalIface  = new UnmanagedString(SETTINGS_IFACE);
        using var signalMember = new UnmanagedString("SettingChanged");

        while (!this.stopRequested.WaitOne(0))
        {
            DBusMessage* msg;
            while ((msg = lib_dbus.dbus_connection_pop_message(this.connection)) != null)
            {
                if (lib_dbus.dbus_message_is_signal(msg, signalIface, signalMember) != 0)
                {
                    this.Refresh();
                }

                lib_dbus.dbus_message_unref(msg);
            }

            _ = lib_dbus.dbus_connection_read_write(this.connection, 50);
        }
    }

    private object? ReadSettingCore(string @namespace, string key)
    {
        if (!this.isAvailable || this.connection == null)
        {
            return null;
        }

        using var busName = new UnmanagedString(BUS_NAME);
        using var objPath = new UnmanagedString(BUS_PATH);
        using var iface   = new UnmanagedString(SETTINGS_IFACE);
        using var method  = new UnmanagedString("Read");
        using var ns      = new UnmanagedString(@namespace);
        using var k       = new UnmanagedString(key);

        var message = lib_dbus.dbus_message_new_method_call(busName, objPath, iface, method);

        if (message == null)
        {
            return null;
        }

        DBusMessageIter iter;
        lib_dbus.dbus_message_iter_init_append(message, &iter);

        byte* nsPtr = ns;
        byte* kPtr = k;

        _ = lib_dbus.dbus_message_iter_append_basic(&iter, lib_dbus.DBUS_TYPE_STRING, &nsPtr);
        _ = lib_dbus.dbus_message_iter_append_basic(&iter, lib_dbus.DBUS_TYPE_STRING, &kPtr);

        DBusError error;
        lib_dbus.dbus_error_init(&error);

        var reply = lib_dbus.dbus_connection_send_with_reply_and_block(this.connection, message, TIMEOUT_MS, &error);

        lib_dbus.dbus_message_unref(message);

        if (reply == null || lib_dbus.dbus_error_is_set(&error) != 0)
        {
            if (reply != null)
            {
                lib_dbus.dbus_message_unref(reply);
            }

            lib_dbus.dbus_error_free(&error);
            return null;
        }

        var result = ParseVariantReply(reply);
        lib_dbus.dbus_message_unref(reply);
        return result;
    }

    private void RemoveMatchRule()
    {
        using var rule = new UnmanagedString(MATCH_RULE);
        DBusError error;
        lib_dbus.dbus_error_init(&error);
        lib_dbus.dbus_bus_remove_match(this.connection, rule, &error);
        if (lib_dbus.dbus_error_is_set(&error) != 0)
        {
            lib_dbus.dbus_error_free(&error);
        }
    }

    protected override void OnDisposed(bool disposing)
    {
        this.stopRequested.Set();

        if (disposing)
        {
            this.monitorThread?.Join(2000);
            this.monitorThread = null;
            this.stopRequested.Dispose();
        }

        if (this.connection != null)
        {
            this.RemoveMatchRule();
            lib_dbus.dbus_connection_unref(this.connection);
            this.connection = null;
        }

        this.isAvailable = false;
    }

    public object? ReadSetting(string @namespace, string key) =>
        ReadSettingCore(@namespace, key);

    public T? ReadSetting<T>(string @namespace, string key) where T : struct
    {
        var value = ReadSettingCore(@namespace, key);
        return value is T t ? t : null;
    }

    public void Refresh()
    {
        if (!this.isAvailable)
        {
            return;
        }

        ColorScheme prevColorScheme;
        Color? prevAccent;
        bool? prevHighContrast;

        lock (this.cacheLock)
        {
            prevColorScheme = this.colorScheme;
            prevAccent = this.accentColor;
            prevHighContrast = this.highContrast;
        }

        var cs = this.ReadSettingCore("org.freedesktop.appearance", "color-scheme");
        var ac = this.ReadSettingCore("org.freedesktop.appearance", "accent-color");
        var hc = this.ReadSettingCore("org.gnome.desktop.a11y.interface", "high-contrast");

        var changed = false;

        lock (this.cacheLock)
        {
            if (cs is uint csVal && (ColorScheme)csVal != prevColorScheme)
            {
                this.colorScheme = (ColorScheme)csVal;
                changed = true;
            }

            if (ac is object?[] arr && arr.Length == 3 &&
                arr[0] is double r && arr[1] is double g && arr[2] is double b)
            {
                var newColor = new Color((float)r, (float)g, (float)b);
                if (newColor != prevAccent)
                {
                    this.accentColor = newColor;
                    changed = true;
                }
            }
            else if (prevAccent != null)
            {
                this.accentColor = null;
                changed = true;
            }

            if (hc is bool hcVal && hcVal != prevHighContrast)
            {
                this.highContrast = hcVal;
                changed = true;
            }
            else if (hc == null && prevHighContrast != null)
            {
                this.highContrast = null;
                changed = true;
            }
        }

        if (changed)
        {
            SettingsChanged?.Invoke();
        }
    }
}
