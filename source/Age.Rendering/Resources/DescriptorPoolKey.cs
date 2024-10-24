using System.Runtime.CompilerServices;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

[InlineArray(17)]
public struct DescriptorPoolKey : IEquatable<DescriptorPoolKey>
{
    private uint element;

    public uint this[VkDescriptorType type]
    {
        get
        {
            var index = (int)type;

            return index < 11
                ? Unsafe.Add(ref this.element, index)
                : type switch
                {
                    VkDescriptorType.InlineUniformBlock       => Unsafe.Add(ref this.element, 11),
                    VkDescriptorType.AccelerationStructureKHR => Unsafe.Add(ref this.element, 12),
                    VkDescriptorType.AccelerationStructureNV  => Unsafe.Add(ref this.element, 13),
                    VkDescriptorType.SampleWeightImageQCOM    => Unsafe.Add(ref this.element, 14),
                    VkDescriptorType.BlockMatchImageQCOM      => Unsafe.Add(ref this.element, 15),
                    VkDescriptorType.MutableEXT               => Unsafe.Add(ref this.element, 16),
                    _ => 0,
                };
        }
        set
        {
            var index = (int)type;

            if (index < 11)
            {
                Unsafe.Add(ref this.element, index) = value;
            }
            else
            {
                switch (type)
                {
                    case VkDescriptorType.InlineUniformBlock:
                        Unsafe.Add(ref this.element, 11) = value;
                        break;
                    case VkDescriptorType.AccelerationStructureKHR:
                        Unsafe.Add(ref this.element, 12) = value;
                        break;
                    case VkDescriptorType.AccelerationStructureNV:
                        Unsafe.Add(ref this.element, 13) = value;
                        break;
                    case VkDescriptorType.SampleWeightImageQCOM:
                        Unsafe.Add(ref this.element, 14) = value;
                        break;
                    case VkDescriptorType.BlockMatchImageQCOM:
                        Unsafe.Add(ref this.element, 15) = value;
                        break;
                    case VkDescriptorType.MutableEXT:
                        Unsafe.Add(ref this.element, 16) = value;
                        break;
                    default:
                        break;
                };
            }
        }
    }

    public bool Equals(DescriptorPoolKey other) =>
        Unsafe.Add(ref this.element, 0) == Unsafe.Add(ref other.element, 0)
        && Unsafe.Add(ref this.element, 1)  == Unsafe.Add(ref other.element, 1)
        && Unsafe.Add(ref this.element, 2)  == Unsafe.Add(ref other.element, 2)
        && Unsafe.Add(ref this.element, 3)  == Unsafe.Add(ref other.element, 3)
        && Unsafe.Add(ref this.element, 4)  == Unsafe.Add(ref other.element, 4)
        && Unsafe.Add(ref this.element, 5)  == Unsafe.Add(ref other.element, 5)
        && Unsafe.Add(ref this.element, 6)  == Unsafe.Add(ref other.element, 6)
        && Unsafe.Add(ref this.element, 7)  == Unsafe.Add(ref other.element, 7)
        && Unsafe.Add(ref this.element, 8)  == Unsafe.Add(ref other.element, 8)
        && Unsafe.Add(ref this.element, 9)  == Unsafe.Add(ref other.element, 9)
        && Unsafe.Add(ref this.element, 10) == Unsafe.Add(ref other.element, 10)
        && Unsafe.Add(ref this.element, 11) == Unsafe.Add(ref other.element, 11)
        && Unsafe.Add(ref this.element, 12) == Unsafe.Add(ref other.element, 12)
        && Unsafe.Add(ref this.element, 13) == Unsafe.Add(ref other.element, 13)
        && Unsafe.Add(ref this.element, 14) == Unsafe.Add(ref other.element, 14)
        && Unsafe.Add(ref this.element, 15) == Unsafe.Add(ref other.element, 15)
        && Unsafe.Add(ref this.element, 16) == Unsafe.Add(ref other.element, 16);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        hashCode.Add(Unsafe.Add(ref this.element, 0));
        hashCode.Add(Unsafe.Add(ref this.element, 1));
        hashCode.Add(Unsafe.Add(ref this.element, 2));
        hashCode.Add(Unsafe.Add(ref this.element, 3));
        hashCode.Add(Unsafe.Add(ref this.element, 4));
        hashCode.Add(Unsafe.Add(ref this.element, 5));
        hashCode.Add(Unsafe.Add(ref this.element, 6));
        hashCode.Add(Unsafe.Add(ref this.element, 7));
        hashCode.Add(Unsafe.Add(ref this.element, 8));
        hashCode.Add(Unsafe.Add(ref this.element, 9));
        hashCode.Add(Unsafe.Add(ref this.element, 10));
        hashCode.Add(Unsafe.Add(ref this.element, 11));
        hashCode.Add(Unsafe.Add(ref this.element, 12));
        hashCode.Add(Unsafe.Add(ref this.element, 13));
        hashCode.Add(Unsafe.Add(ref this.element, 14));
        hashCode.Add(Unsafe.Add(ref this.element, 15));
        hashCode.Add(Unsafe.Add(ref this.element, 16));

        return hashCode.ToHashCode();
    }

    public override bool Equals(object? obj) =>
        obj is DescriptorPoolKey key && this.Equals(key);

    public static bool operator ==(DescriptorPoolKey left, DescriptorPoolKey right) => left.Equals(right);

    public static bool operator !=(DescriptorPoolKey left, DescriptorPoolKey right) => !(left == right);
}
