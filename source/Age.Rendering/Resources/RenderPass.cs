using Age.Core;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public sealed class RenderPass(VkRenderPass instance) : SharedDisposable<RenderPass>
{
    internal VkRenderPass Instance => instance;

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            VulkanRenderer.Singleton.DeferredDispose(this.Instance);
        }
    }

    public static implicit operator VkRenderPass(RenderPass renderPass) => renderPass.Instance;
}
