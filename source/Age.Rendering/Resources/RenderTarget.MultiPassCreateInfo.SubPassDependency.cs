namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct MultiPassCreateInfo
    {
        public struct SubPassDependency : IEquatable<SubPassDependency>
        {
            public int  SrcSubpass;
            public int  DstSubpass;
            public uint SrcStageMask;
            public uint DstStageMask;
            public uint SrcAccessMask;
            public uint DstAccessMask;

            public readonly bool Equals(SubPassDependency other) =>
                this.SrcSubpass       == other.SrcSubpass
                && this.DstSubpass    == other.DstSubpass
                && this.SrcStageMask  == other.SrcStageMask
                && this.DstStageMask  == other.DstStageMask
                && this.SrcAccessMask == other.SrcAccessMask
                && this.DstAccessMask == other.DstAccessMask;

            public override readonly bool Equals(object? obj) =>
                obj is SubPassDependency subPassDependency && this.Equals(subPassDependency);

            public override readonly int GetHashCode() =>
                HashCode.Combine(
                    this.SrcSubpass,
                    this.DstSubpass,
                    this.SrcStageMask,
                    this.DstStageMask,
                    this.SrcAccessMask,
                    this.DstAccessMask
                );

            public static bool operator ==(SubPassDependency left, SubPassDependency right) => left.Equals(right);
            public static bool operator !=(SubPassDependency left, SubPassDependency right) => !(left == right);


        }
    }
}
