using Age.Core.Collections;

namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    private struct RenderPassKey : IEquatable<RenderPassKey>
    {
        public InlineList4<MultiPassCreateInfo.AttachmentInfo>    Attachments;
        public InlineList4<MultiPassCreateInfo.SubPassInfo>       Passes;
        public InlineList4<MultiPassCreateInfo.SubPassDependency> Dependencies;

        public RenderPassKey(in MultiPassCreateInfo createInfo)
        {
            createInfo.Attachments.CopyTo(ref this.Attachments);
            createInfo.Passes.CopyTo(ref this.Passes);
            createInfo.Dependencies.CopyTo(ref this.Dependencies);
        }

        public readonly bool Equals(RenderPassKey other) =>
            this.Attachments.Equals(other.Attachments)
            && this.Passes.Equals(other.Passes)
            && this.Dependencies.Equals(other.Dependencies);

        public override readonly bool Equals(object? obj) =>
            obj is RenderPassKey renderPassKey && this.Equals(renderPassKey);

        public override readonly int GetHashCode() =>
            HashCode.Combine(
                this.Attachments.GetHashCode(),
                this.Passes.GetHashCode(),
                this.Dependencies.GetHashCode()
            );
    }
}
