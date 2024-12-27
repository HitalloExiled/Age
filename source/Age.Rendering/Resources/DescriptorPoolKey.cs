using Age.Core.Extensions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

[InlineArray(SIZE)]
public struct DescriptorPoolKey : IEquatable<DescriptorPoolKey>
{
    private const int SIZE = 17;

    private uint element;

    public uint this[VkDescriptorType type]
    {
        get
        {
            var index = (int)type;

            var span = this.AsSpan();

            return index < 11
                ? span[index]
                : type switch
                {
                    VkDescriptorType.InlineUniformBlock       => span[11],
                    VkDescriptorType.AccelerationStructureKHR => span[12],
                    VkDescriptorType.AccelerationStructureNV  => span[13],
                    VkDescriptorType.SampleWeightImageQCOM    => span[14],
                    VkDescriptorType.BlockMatchImageQCOM      => span[15],
                    VkDescriptorType.MutableEXT               => span[16],
                    _ => 0,
                };
        }
        set
        {
            var index = (int)type;

            var span = this.AsSpan();

            if (index < 11)
            {
                span[index] = value;
            }
            else
            {
                switch (type)
                {
                    case VkDescriptorType.InlineUniformBlock:
                        span[11] = value;
                        break;
                    case VkDescriptorType.AccelerationStructureKHR:
                        span[12] = value;
                        break;
                    case VkDescriptorType.AccelerationStructureNV:
                        span[13] = value;
                        break;
                    case VkDescriptorType.SampleWeightImageQCOM:
                        span[14] = value;
                        break;
                    case VkDescriptorType.BlockMatchImageQCOM:
                        span[15] = value;
                        break;
                    case VkDescriptorType.MutableEXT:
                        span[16] = value;
                        break;
                    default:
                        break;
                };
            }
        }
    }

    public unsafe Span<uint> AsSpan() =>
        MemoryMarshal.CreateSpan(ref this.element, SIZE);

    public bool Equals(DescriptorPoolKey other) =>
        this.AsSpan().SequenceEqual(other);

    public override bool Equals(object? obj) =>
        obj is DescriptorPoolKey key && this.Equals(key);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        var buffer = this.AsSpan();

        hashCode.AddBytes(buffer.Cast<uint, byte>());

        return hashCode.ToHashCode();
    }

    public static bool operator ==(DescriptorPoolKey left, DescriptorPoolKey right) => left.Equals(right);

    public static bool operator !=(DescriptorPoolKey left, DescriptorPoolKey right) => !(left == right);
}
