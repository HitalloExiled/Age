using System.Diagnostics;

namespace ThirdParty.Vulkan;

[DebuggerDisplay("{Variant}.{Major}.{Minor}.{Patch}")]
public readonly record struct VkVersion
{
    private readonly uint value;

    public static VkVersion V1_0 => new(0, 1, 0);

    public uint Variant => (this.value >> 29) & 0b111;
    public uint Major   => (this.value >> 22) & 0b1111111;
    public uint Minor   => (this.value >> 12) & 0b1111111111;
    public uint Patch   => this.value & 0b111111111111;

    public VkVersion(uint value) =>
        this.value = value;

    public VkVersion(uint variant, uint major, uint minor, uint patch = default) =>
        this.value = (variant << 29) | (major << 22) | (minor << 12) | patch;

    public static implicit operator uint(VkVersion version) => version.value;
}
