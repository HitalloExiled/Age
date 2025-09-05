using Age.Core.Extensions;
using Age.Commands;
using Age.Core.Collections;
using System.Numerics;
using static Age.Shaders.CanvasShader;
using System.Diagnostics.CodeAnalysis;

namespace Age.Elements;

public abstract partial class Element
{
    [KeyedListKey]
    internal enum LayoutCommand : ushort
    {
        None       = 0,
        Box        = 1 << 0,
        Image      = 1 << 1,
        ScrollBarX = 1 << 2,
        ScrollBarY = 1 << 3,
    }

    private byte          commandsSeparator;
    private LayoutCommand layoutCommands;

    internal ReadOnlySpan<Command> PreCommands  => this.Commands.AsSpan(..this.commandsSeparator);
    internal ReadOnlySpan<Command> PostCommands => this.Commands.AsSpan(this.commandsSeparator..);

    private RectCommand AllocateLayoutCommand(LayoutCommand layoutCommand)
    {
        var index = this.GetIndex(layoutCommand);

        if (this.layoutCommands.HasFlags(layoutCommand))
        {
            return (RectCommand)this.Commands[index];
        }

        var command = CommandPool.RectCommand.Get();

        switch (layoutCommand)
        {
            case LayoutCommand.Box:
                command.Flags           = Flags.ColorAsBackground;
                command.PipelineVariant = PipelineVariant.Color | PipelineVariant.Index;

                break;

            default:
                command.PipelineVariant = PipelineVariant.Color | PipelineVariant.Index;

                break;
        }

        command.StencilLayer = this.StencilLayer;

        this.Commands.Insert(index, command);

        this.layoutCommands |= layoutCommand;

        this.UpdateCommandsSeparator();

        return command;
    }

    private RectCommand AllocateLayoutCommandBox() =>
        this.AllocateLayoutCommand(LayoutCommand.Box);

    private RectCommand AllocateLayoutCommandImage() =>
        this.AllocateLayoutCommand(LayoutCommand.Image);

    private RectCommand AllocateLayoutCommandScrollBarX() =>
        this.AllocateLayoutCommand(LayoutCommand.ScrollBarX);

    private RectCommand AllocateLayoutCommandScrollBarY() =>
        this.AllocateLayoutCommand(LayoutCommand.ScrollBarY);

    private RectCommand GetLayoutCommand(LayoutCommand layoutCommand) =>
        this.TryGetLayoutCommand(layoutCommand, out var rectCommand)
            ? rectCommand
            : throw new InvalidOperationException($"{this} - {layoutCommand} not allocated.");

    private RectCommand GetLayoutCommandBox() =>
        this.GetLayoutCommand(LayoutCommand.Box);

    private RectCommand GetLayoutCommandImage() =>
        this.GetLayoutCommand(LayoutCommand.Image);

    private RectCommand GetLayoutCommandScrollBarX() =>
        this.GetLayoutCommand(LayoutCommand.ScrollBarX);

    private RectCommand GetLayoutCommandScrollBarY() =>
        this.GetLayoutCommand(LayoutCommand.ScrollBarY);

    private int GetIndex(LayoutCommand layoutCommand)
    {
        var mask = layoutCommand - 1;

        return BitOperations.PopCount((uint)(this.layoutCommands & mask));
    }

    private bool HasAnyLayoutCommand(LayoutCommand layoutCommands) =>
        this.layoutCommands.HasAnyFlag(layoutCommands);

    private bool HasLayoutCommand(LayoutCommand layoutCommand) =>
        this.layoutCommands.HasFlags(layoutCommand);

    private void ReleaseLayoutCommand(LayoutCommand layoutCommand)
    {
        if (this.HasLayoutCommand(layoutCommand))
        {
            var mask = layoutCommand - 1;
            var index = BitOperations.PopCount((uint)(this.layoutCommands & mask));

            var command = (RectCommand)this.Commands[index];

            CommandPool.RectCommand.Return(command);

            this.Commands.RemoveAt(index);

            this.layoutCommands &= ~layoutCommand;

            this.UpdateCommandsSeparator();
        }
    }

    private void ReleaseLayoutCommandBox() =>
        this.ReleaseLayoutCommand(LayoutCommand.Box);

    private void ReleaseLayoutCommandImage() =>
        this.ReleaseLayoutCommand(LayoutCommand.Image);

    private void ReleaseLayoutCommandScrollBarX() =>
        this.ReleaseLayoutCommand(LayoutCommand.ScrollBarX);

    private void ReleaseLayoutCommandScrollBarY() =>
        this.ReleaseLayoutCommand(LayoutCommand.ScrollBarY);

    private bool TryGetLayoutCommand(LayoutCommand layoutCommand, [NotNullWhen(true)] out RectCommand? rectCommand)
    {
        var index = this.GetIndex(layoutCommand);

        if (this.layoutCommands.HasFlags(layoutCommand))
        {
            rectCommand = (RectCommand)this.Commands[index];
            return true;
        }

        rectCommand = null;
        return false;
    }

    private bool TryGetLayoutCommandBox([NotNullWhen(true)] out RectCommand? rectCommand) =>
        this.TryGetLayoutCommand(LayoutCommand.Box, out rectCommand);

    private bool TryGetLayoutCommandImage([NotNullWhen(true)] out RectCommand? rectCommand) =>
        this.TryGetLayoutCommand(LayoutCommand.Image, out rectCommand);

    private bool TryGetLayoutCommandScrollBarX([NotNullWhen(true)] out RectCommand? rectCommand) =>
        this.TryGetLayoutCommand(LayoutCommand.ScrollBarX, out rectCommand);

    private bool TryGetLayoutCommandScrollBarY([NotNullWhen(true)] out RectCommand? rectCommand) =>
        this.TryGetLayoutCommand(LayoutCommand.ScrollBarY, out rectCommand);

    private void UpdateBackgroundImage()
    {
        if (this.ComputedStyle.BackgroundImage != null)
        {
            var layoutCommandImage = this.GetLayoutCommandImage();

            this.ResolveImageSize(this.ComputedStyle.BackgroundImage, layoutCommandImage.TextureMap.Texture.Size.Cast<float>(), out var size, out var transform, out var uv);

            layoutCommandImage.Size = size;
            layoutCommandImage.Transform = transform;
            layoutCommandImage.TextureMap = layoutCommandImage.TextureMap with { UV = uv };

            layoutCommandImage.StencilLayer!.MakeDirty();
        }
    }

    private void UpdateCommandsSeparator()
    {
        LayoutCommand? layoutCommand = null;

        if (this.layoutCommands.HasFlags(LayoutCommand.Image))
        {
            layoutCommand = LayoutCommand.Image;
        }
        else if (this.layoutCommands.HasFlags(LayoutCommand.Box))
        {
            layoutCommand = LayoutCommand.Box;
        }

        this.commandsSeparator = (byte)(layoutCommand.HasValue ? this.GetIndex(layoutCommand.Value) + 1 : 0);
    }
}
