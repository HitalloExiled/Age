using System.Runtime.CompilerServices;

namespace Age.Core.Config;

internal enum Variant
{
    STRING,
    INT,
    FLOAT,
}

internal enum PropertyHint
{
    PROPERTY_HINT_NONE,
    PROPERTY_HINT_ENUM,
    PROPERTY_HINT_RANGE,
    PROPERTY_HINT_FILE
}

[Flags]
internal enum PropertyUsageFlags
{
    PROPERTY_USAGE_STORAGE = 2,
    PROPERTY_USAGE_EDITOR  = 4,
    PROPERTY_USAGE_DEFAULT = 6
}

internal record PropertyInfo(
    Variant       Type,
    string             Name,
    PropertyHint       Hint       = PropertyHint.PROPERTY_HINT_NONE,
    string?            HintString = null,
    PropertyUsageFlags Usage      = PropertyUsageFlags.PROPERTY_USAGE_DEFAULT,
    string?            ClassName  = null
);

internal class ProjectSettings
{
    public static ProjectSettings Singleton { get; } = new();

    private readonly Dictionary<string, object> properties = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public object? Get(string key)
    {
        if (this.properties.TryGetValue(key, out var value))
        {
            return value;
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public T? Get<T>(string key) => this.Get(key) is T value ? value : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public T Set<T>(string key, T value) where T : notnull
    {
        if (!this.properties.TryAdd(key, value))
        {
            this.properties[key] = value;
        }

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Has(string key) => this.properties.ContainsKey(key);
}

internal static class Macros
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T GLOBAL_DEF<T>(string key, T value) where T : notnull => ProjectSettings.Singleton.Set(key, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T? GLOBAL_GET<T>(string key) => ProjectSettings.Singleton.Get<T>(key);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void GLOBAL_DEF_BASIC(string key, object value) => ProjectSettings.Singleton.Set(key, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void GLOBAL_DEF_RST(string key, object value) => ProjectSettings.Singleton.Set(key, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void GLOBAL_DEF_RST_BASIC(string key, object value) => ProjectSettings.Singleton.Set(key, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void GLOBAL_DEF_RST_NOVAL(string key, object value) => ProjectSettings.Singleton.Set(key, value);
}
