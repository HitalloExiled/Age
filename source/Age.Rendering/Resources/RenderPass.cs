using Age.Core;
using Age.Core.Collections;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public sealed partial class RenderPass : Resource<VkRenderPass>
{
    internal override VkRenderPass Instance { get; }

    public unsafe RenderPass(in RenderPassCreateInfo createInfo)
    {
        using var disposables = new Disposables();

        using var subpassDescriptions     = new RefList<VkSubpassDescription>();
        using var attachmentDescriptions  = new RefList<VkAttachmentDescription>();
        using var depthStencilAttachments = new RefList<VkAttachmentDescription>();

        foreach (var subpass in createInfo.SubPasses)
        {
            var colorAttachmentReferences        = new NativeList<VkAttachmentReference>(subpass.ColorAttachments.Length);
            var resolveAttachmentReferences      = new NativeList<VkAttachmentReference>();
            var depthStencilAttachmentReferences = new NativeList<VkAttachmentReference>();

            using var subPassColorAttachments = new RefList<SubPass.ColorAttachment>(subpass.ColorAttachments.Length);

            disposables.Add(colorAttachmentReferences);
            disposables.Add(resolveAttachmentReferences);
            disposables.Add(depthStencilAttachmentReferences);

            foreach (var attachment in subpass.ColorAttachments)
            {
                colorAttachmentReferences.Add(new() { Attachment = (uint)attachmentDescriptions.Count, Layout = VkImageLayout.ColorAttachmentOptimal });
                attachmentDescriptions.Add(attachment.Color);

                if (attachment.Resolve.HasValue)
                {
                    resolveAttachmentReferences.Add(new() { Attachment = (uint)attachmentDescriptions.Count, Layout = VkImageLayout.ColorAttachmentOptimal });
                    attachmentDescriptions.Add(attachment.Resolve.Value);
                }

                var subPassColorAttachment = new SubPass.ColorAttachment
                {
                    Color = new()
                    {
                        Format  = attachment.Color.Format,
                        Samples = attachment.Color.Samples,
                    },
                    Resolve = attachment.Resolve.HasValue
                        ? new()
                        {
                            Format  = attachment.Resolve.Value.Format,
                            Samples = attachment.Resolve.Value.Samples,
                        } : default,
                };

                subPassColorAttachments.Add(subPassColorAttachment);
            }

            if (subpass.DepthStencilAttachment.HasValue)
            {
                depthStencilAttachmentReferences.Add(new() { Attachment = (uint)attachmentDescriptions.Count, Layout = VkImageLayout.DepthStencilAttachmentOptimal });
                attachmentDescriptions.Add(subpass.DepthStencilAttachment.Value);
            }

            var subpassDescription = new VkSubpassDescription
            {
                PipelineBindPoint       = subpass.PipelineBindPoint,
                PColorAttachments       = colorAttachmentReferences.AsPointer(),
                PResolveAttachments     = resolveAttachmentReferences.AsPointer(),
                ColorAttachmentCount    = (uint)colorAttachmentReferences.Count,
                PDepthStencilAttachment = depthStencilAttachmentReferences.AsPointer(),
            };

            subpassDescriptions.Add(subpassDescription);
        }

        fixed (VkSubpassDependency* pDependencies = createInfo.SubpassDependencies)
        {
            var renderPassCreateInfo = new VkRenderPassCreateInfo
            {
                AttachmentCount = (uint)attachmentDescriptions.Count,
                PAttachments    = attachmentDescriptions.AsPointer(),
                DependencyCount = (uint)createInfo.SubpassDependencies.Length,
                PDependencies   = pDependencies,
                SubpassCount    = (uint)subpassDescriptions.Count,
                PSubpasses      = subpassDescriptions.AsPointer(),
            };

            this.Instance = VulkanRenderer.Singleton.Context.Device.CreateRenderPass(renderPassCreateInfo);
        }
    }

    protected override void OnDisposed() =>
        this.Instance.Dispose();
}
