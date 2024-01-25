using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// Structure specifying layer properties.
/// </summary>
public unsafe record LayerProperties : NativeReference<VkLayerProperties>
{
    private string? description;
    private string? layerName;

    /// <summary>
    /// The name of the layer. Use this name in the ppEnabledLayerNames array passed in the <see cref="VkInstanceCreateInfo"/> structure to enable this layer for an instance.
    /// </summary>
    public string LayerName => Get(ref this.layerName, this.PNative->layerName)!;

    /// <summary>
    /// The Vulkan version the layer was written to, encoded as described in https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#extendingvulkan-coreversions-versionnumbers.
    /// </summary>
    public Version SpecVersion => new(this.PNative->specVersion);

    /// <summary>
    /// The version of this layer. It is an integer, increasing with backward compatible changes.
    /// </summary>
    public Version ImplementationVersion => new(this.PNative->implementationVersion);

    /// <summary>
    /// Provides additional details that can be used by the application to identify the layer.
    /// </summary>
    public string Description => Get(ref this.description, this.PNative->description)!;

    internal LayerProperties(in VkLayerProperties value) : base(value) { }
}
