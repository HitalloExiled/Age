using Age.Core.Collections;

namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct MultiPassCreateInfo
    {
        public struct SubPassInfo : IEquatable<SubPassInfo>
        {
            public required InlineList4<int> ColorAttachments;
            public required int?             DepthStencilAttachment;

            public readonly bool Equals(SubPassInfo other) =>
                this.ColorAttachments.Equals(other.ColorAttachments)
                && this.DepthStencilAttachment == other.DepthStencilAttachment;

            public override readonly bool Equals(object? obj) =>
                obj is SubPassInfo subPassInfo && this.Equals(subPassInfo);

            public override readonly int GetHashCode() =>
                HashCode.Combine(this.ColorAttachments, this.DepthStencilAttachment);

            public static bool operator ==(SubPassInfo left, SubPassInfo right) => left.Equals(right);
            public static bool operator !=(SubPassInfo left, SubPassInfo right) => !(left == right);


        }
    }
}
