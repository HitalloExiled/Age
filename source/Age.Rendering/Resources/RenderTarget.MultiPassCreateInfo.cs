using Age.Numerics;

namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct MultiPassCreateInfo
    {
        public required Size<uint>                   Size;
        public required ReadOnlySpan<AttachmentInfo> Attachments;
        public required ReadOnlySpan<SubPassInfo>    Passes;

        public ReadOnlySpan<SubPassDependency> Dependencies;

        public override readonly int GetHashCode()
        {
            var hashcode = new HashCode();

            foreach (var pass in this.Passes)
            {
                foreach (var colorAttachment in pass.ColorAttachments)
                {
                    hashcode.Add(colorAttachment);
                    hashcode.Add(this.Attachments[colorAttachment]);
                }

                if (pass.DepthStencilAttachment is int depthStencilAttachment)
                {
                    hashcode.Add(depthStencilAttachment);

                    hashcode.Add(this.Attachments[depthStencilAttachment]);
                }
            }

            return hashcode.ToHashCode();
        }
    }
}
