using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkApplicationInfo.html">VkApplicationInfo</see>
/// </summary>
public unsafe record ApplicationInfo : NativeReference<VkApplicationInfo>
{
    private string? applicationName;
    private string? engineName;

    public nint Next
    {
        get => (nint)this.PNative->pNext;
        init => this.PNative->pNext = value.ToPointer();
    }

    public string? ApplicationName
    {
        get => this.applicationName;
        init => Init(ref this.applicationName, ref this.PNative->pApplicationName, value);
    }

    public Version ApplicationVersion
    {
        get => new(this.PNative->applicationVersion);
        init => this.PNative->applicationVersion = value;
    }

    public string? EngineName
    {
        get => this.engineName;
        init => Init(ref this.engineName, ref this.PNative->pEngineName, value);
    }

    public Version EngineVersion
    {
        get => new(this.PNative->engineVersion);
        init => this.PNative->engineVersion = value;
    }

    public Version ApiVersion
    {
        get => new(this.PNative->apiVersion);
        init => this.PNative->apiVersion = value;
    }

    protected override void OnFinalize()
    {
        Free(ref this.PNative->pApplicationName);
        Free(ref this.PNative->pEngineName);
    }
}
