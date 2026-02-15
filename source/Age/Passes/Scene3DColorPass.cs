using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Rendering.Extensions;
using Age.Resources;
using Age.Scenes;

namespace Age.Passes;

public class Scene3DColorPass : Scene3DPass
{
    public override string Name => nameof(Scene3DColorPass);

    protected override CommandFilter CommandFilter => CommandFilter.Color;

    protected override void Record(Camera3D camera, MeshCommand command)
    {
        var commandBuffer = this.CommandBuffer;

        var mesh       = Unsafe.As<Mesh>(command.Owner);
        var ubo        = this.UpdateUbo(camera, mesh, this.Viewport!.Size.ToExtent2D());
        var shader     = mesh.Material.GetShader(this.RenderTarget, new() { Subpass = this.Index });
        var uniformSet = this.GetUniformSet(shader, mesh.Material, camera, ubo);

        commandBuffer.BindShader(shader);
        commandBuffer.BindVertexBuffer([command.VertexBuffer]);
        commandBuffer.BindIndexBuffer(command.IndexBuffer);
        commandBuffer.BindUniformSet(uniformSet);
        commandBuffer.DrawIndexed(command.IndexBuffer);
    }
}
