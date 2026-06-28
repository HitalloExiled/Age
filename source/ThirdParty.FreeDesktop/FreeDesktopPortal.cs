using System.Runtime.CompilerServices;
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

    private readonly Lock             cacheLock     = new();
    private readonly ManualResetEvent stopRequested = new(false);

    private Color           accentColor;
    private ColorScheme     colorScheme;
    private DBusConnection* connection;
    private int             doubleClick;
    private bool            highContrast;
    private bool            leftHanded;
    private Thread?         monitorThread;

    public Color AccentColor
    {
        get
        {
            lock (this.cacheLock)
            {
                return this.accentColor;
            }
        }
    }

    public int DoubleClick
    {
        get
        {
            lock (this.cacheLock)
            {
                return this.doubleClick;
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

    public bool HighContrast
    {
        get
        {
            lock (this.cacheLock)
            {
                return this.highContrast;
            }
        }
    }

    public bool LeftHanded
    {
        get
        {
            lock (this.cacheLock)
            {
                return this.leftHanded;
            }
        }
    }

    public event Action? SettingsChanged;

    public FreeDesktopPortal(bool watch = false)
    {
        DBusError error;
        lib_dbus.dbus_error_init(&error);

        this.connection = lib_dbus.dbus_bus_get(DBusBusType.DBUS_BUS_SESSION, &error);
        if (lib_dbus.dbus_error_is_set(&error) != 0)
        {
            lib_dbus.dbus_error_free(&error);
            return;
        }

        this.AddMatchRule();
        this.Refresh();

        if (watch)
        {
            this.monitorThread = new Thread(this.MonitorLoop) { IsBackground = true, Name = "PortalMonitor" };
            this.monitorThread.Start();
        }
    }

    private static T GetBasic<T>(DBusMessageIter* iter) where T : unmanaged
    {
        T val;
        lib_dbus.dbus_message_iter_get_basic(iter, &val);
        return val;
    }

    private DBusMessage* CallReadMethod(string @namespace, string key)
    {
        base.ThrowIfDisposed();

        using var busName = new NativeString(BUS_NAME);
        using var objPath = new NativeString(BUS_PATH);
        using var iface   = new NativeString(SETTINGS_IFACE);
        using var method  = new NativeString("Read");
        using var ns      = new NativeString(@namespace);
        using var k       = new NativeString(key);

        var message = lib_dbus.dbus_message_new_method_call(busName, objPath, iface, method);

        if (message == null)
        {
            return null;
        }

        DBusMessageIter iter;
        lib_dbus.dbus_message_iter_init_append(message, &iter);

        byte* nsPtr = ns;
        byte* kPtr  = k;

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

        return reply;
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
                lib_dbus.DBUS_TYPE_UINT32                                                                   => GetBasic<uint>(iter),
                lib_dbus.DBUS_TYPE_BOOLEAN                                                                  => GetBasic<uint>(iter) != 0,
                lib_dbus.DBUS_TYPE_DOUBLE                                                                   => GetBasic<double>(iter),
                lib_dbus.DBUS_TYPE_BYTE                                                                     => GetBasic<byte>(iter),
                lib_dbus.DBUS_TYPE_INT16                                                                    => GetBasic<short>(iter),
                lib_dbus.DBUS_TYPE_UINT16                                                                   => GetBasic<ushort>(iter),
                lib_dbus.DBUS_TYPE_INT32                                                                    => GetBasic<int>(iter),
                lib_dbus.DBUS_TYPE_INT64                                                                    => GetBasic<long>(iter),
                lib_dbus.DBUS_TYPE_UINT64                                                                   => GetBasic<ulong>(iter),
                lib_dbus.DBUS_TYPE_STRING or lib_dbus.DBUS_TYPE_OBJECT_PATH or lib_dbus.DBUS_TYPE_SIGNATURE => GetString(iter),
                _ => null
            };
    }

    private void AddMatchRule()
    {
        using var rule = new NativeString(MATCH_RULE);

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
        using var signalIface  = new NativeString(SETTINGS_IFACE);
        using var signalMember = new NativeString("SettingChanged");

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

    private static Color ParseArrayColor(DBusMessageIter* iter)
    {
        DBusMessageIter sub;
        lib_dbus.dbus_message_iter_recurse(iter, &sub);

        if (lib_dbus.dbus_message_iter_get_arg_type(&sub) != lib_dbus.DBUS_TYPE_DOUBLE)
        {
            return default;
        }

        var r = GetBasic<double>(&sub);

        if (lib_dbus.dbus_message_iter_next(&sub) == 0)
        {
            return default;
        }

        if (lib_dbus.dbus_message_iter_get_arg_type(&sub) != lib_dbus.DBUS_TYPE_DOUBLE)
        {
            return default;
        }

        var g = GetBasic<double>(&sub);

        if (lib_dbus.dbus_message_iter_next(&sub) == 0)
        {
            return default;
        }

        if (lib_dbus.dbus_message_iter_get_arg_type(&sub) != lib_dbus.DBUS_TYPE_DOUBLE)
        {
            return default;
        }

        var b = GetBasic<double>(&sub);

        return new((float)r, (float)g, (float)b);
    }

    private static T ParseVariantReply<T>(DBusMessage* reply) where T : unmanaged
    {
        DBusMessageIter iter0, iter1, iter2;

        if (lib_dbus.dbus_message_iter_init(reply, &iter0) == 0)
        {
            return default;
        }

        if (lib_dbus.dbus_message_iter_get_arg_type(&iter0) != lib_dbus.DBUS_TYPE_VARIANT)
        {
            return default;
        }

        lib_dbus.dbus_message_iter_recurse(&iter0, &iter1);
        if (lib_dbus.dbus_message_iter_get_arg_type(&iter1) != lib_dbus.DBUS_TYPE_VARIANT)
        {
            return default;
        }

        lib_dbus.dbus_message_iter_recurse(&iter1, &iter2);

        var type = lib_dbus.dbus_message_iter_get_arg_type(&iter2);

        return type switch
        {
            lib_dbus.DBUS_TYPE_UINT32                             when typeof(T) == typeof(uint)   => As(GetBasic<uint>(&iter2)),
            lib_dbus.DBUS_TYPE_BOOLEAN                            when typeof(T) == typeof(bool)   => As(GetBasic<uint>(&iter2) != 0),
            lib_dbus.DBUS_TYPE_DOUBLE                             when typeof(T) == typeof(double) => As(GetBasic<double>(&iter2)),
            lib_dbus.DBUS_TYPE_BYTE                               when typeof(T) == typeof(byte)   => As(GetBasic<byte>(&iter2)),
            lib_dbus.DBUS_TYPE_INT16                              when typeof(T) == typeof(short)  => As(GetBasic<short>(&iter2)),
            lib_dbus.DBUS_TYPE_UINT16                             when typeof(T) == typeof(ushort) => As(GetBasic<ushort>(&iter2)),
            lib_dbus.DBUS_TYPE_INT32                              when typeof(T) == typeof(int)    => As(GetBasic<int>(&iter2)),
            lib_dbus.DBUS_TYPE_INT64                              when typeof(T) == typeof(long)   => As(GetBasic<long>(&iter2)),
            lib_dbus.DBUS_TYPE_UINT64                             when typeof(T) == typeof(ulong)  => As(GetBasic<ulong>(&iter2)),
            lib_dbus.DBUS_TYPE_ARRAY or lib_dbus.DBUS_TYPE_STRUCT when typeof(T) == typeof(Color)  => As(ParseArrayColor(&iter2)),
            _ => default
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T As<TFrom>(TFrom value) => Unsafe.As<TFrom, T>(ref value);
    }

    private bool TryReadSettingCore(string @namespace, string key, out object? value)
    {
        var reply = this.CallReadMethod(@namespace, key);

        if (reply == null)
        {
            value = default;

            return false;
        }

        var result = ParseVariantReply(reply);

        lib_dbus.dbus_message_unref(reply);

        value = result;

        return true;
    }

    private bool TryReadSettingCore<T>(string @namespace, string key, out T value) where T : unmanaged
    {
        var reply = this.CallReadMethod(@namespace, key);

        if (reply == null)
        {
            value = default;

            return false;
        }

        var result = ParseVariantReply<T>(reply);

        lib_dbus.dbus_message_unref(reply);

        value = result;

        return true;
    }

    private void RemoveMatchRule()
    {
        using var rule = new NativeString(MATCH_RULE);
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
    }

    public bool TryReadSetting(string @namespace, string key, out object? value) =>
        this.TryReadSettingCore(@namespace, key, out value);

    public object? ReadSetting(string @namespace, string key) =>
        this.TryReadSetting(@namespace, key, out var value) ? value : throw new InvalidOperationException($"Failed to read setting '{key}' from namespace '{@namespace}'.");

    public T? ReadSetting<T>(string @namespace, string key) =>
        this.TryReadSetting<T>(@namespace, key, out var value) ? value : throw new InvalidOperationException($"Failed to read setting '{key}' of type '{typeof(T).Name}' from namespace '{@namespace}'.");

    public void Refresh()
    {
        this.ThrowIfDisposed();

        Color       previousAccentColor;
        ColorScheme previousColorScheme;
        int         previousDoubleClick;
        bool        previousHighContrast;
        bool        previousLeftHanded;

        lock (this.cacheLock)
        {
            previousAccentColor  = this.accentColor;
            previousColorScheme  = this.colorScheme;
            previousDoubleClick  = this.doubleClick;
            previousHighContrast = this.highContrast;
            previousLeftHanded   = this.leftHanded;
        }

        var changed = false;

        lock (this.cacheLock)
        {
            if (this.TryReadSettingCore<Color>("org.freedesktop.appearance", "accent-color", out var accentColor))
            {
                if (accentColor != previousAccentColor)
                {
                    this.accentColor = accentColor;

                    changed = true;
                }
            }
            else if (previousAccentColor != default)
            {
                this.accentColor = default;

                changed = true;
            }

            if (this.TryReadSettingCore<int>("org.gnome.desktop.peripherals.mouse", "double-click", out var doubleClick))
            {
                if (doubleClick != previousDoubleClick)
                {
                    this.doubleClick = doubleClick;

                    changed = true;
                }
            }
            else if (previousDoubleClick != default)
            {
                this.doubleClick = default;

                changed = true;
            }

            if (this.TryReadSettingCore<uint>("org.freedesktop.appearance", "color-scheme", out var colorScheme) && (ColorScheme)colorScheme != previousColorScheme)
            {
                this.colorScheme = (ColorScheme)colorScheme;

                changed = true;
            }

            if (this.TryReadSettingCore<bool>("org.gnome.desktop.a11y.interface", "high-contrast", out var highContrast))
            {
                if (highContrast != previousHighContrast)
                {
                    this.highContrast = highContrast;

                    changed = true;
                }
            }
            else if (previousHighContrast != default)
            {
                this.highContrast = default;

                changed = true;
            }

            if (this.TryReadSettingCore<bool>("org.gnome.desktop.peripherals.mouse", "left-handed", out var leftHanded))
            {
                if (leftHanded != previousLeftHanded)
                {
                    this.leftHanded = leftHanded;

                    changed = true;
                }
            }
            else if (previousLeftHanded != default)
            {
                this.leftHanded = default;

                changed = true;
            }
        }

        if (changed)
        {
            SettingsChanged?.Invoke();
        }
    }

    public bool TryReadSetting<T>(string @namespace, string key, out T? value)
    {
        var found = false;
        value = default;

        if (typeof(T) == typeof(uint))
        {
            found = this.TryReadSettingCore<uint>(@namespace, key, out var result);
            value = Unsafe.As<uint, T>(ref result);
        }

        if (typeof(T) == typeof(bool))
        {
            found = this.TryReadSettingCore<bool>(@namespace, key, out var result);
            value = Unsafe.As<bool, T>(ref result);
        }

        if (typeof(T) == typeof(double))
        {
            found = this.TryReadSettingCore<double>(@namespace, key, out var result);
            value = Unsafe.As<double, T>(ref result);
        }

        if (typeof(T) == typeof(byte))
        {
            found = this.TryReadSettingCore<byte>(@namespace, key, out var result);
            value = Unsafe.As<byte, T>(ref result);
        }

        if (typeof(T) == typeof(short))
        {
            found = this.TryReadSettingCore<short>(@namespace, key, out var result);
            value = Unsafe.As<short, T>(ref result);
        }

        if (typeof(T) == typeof(ushort))
        {
            found = this.TryReadSettingCore<ushort>(@namespace, key, out var result);
            value = Unsafe.As<ushort, T>(ref result);
        }

        if (typeof(T) == typeof(int))
        {
            found = this.TryReadSettingCore<int>(@namespace, key, out var result);
            value = Unsafe.As<int, T>(ref result);
        }

        if (typeof(T) == typeof(long))
        {
            found = this.TryReadSettingCore<long>(@namespace, key, out var result);
            value = Unsafe.As<long, T>(ref result);
        }

        if (typeof(T) == typeof(ulong))
        {
            found = this.TryReadSettingCore<ulong>(@namespace, key, out var result);
            value = Unsafe.As<ulong, T>(ref result);
        }

        if (typeof(T) == typeof(Color))
        {
            found = this.TryReadSettingCore<Color>(@namespace, key, out var result);
            value = Unsafe.As<Color, T>(ref result);
        }

        if (!found)
        {
            found = this.TryReadSettingCore(@namespace, key, out var result);
            value = result is T v ? v : default;
        }

        return found;
    }
}
