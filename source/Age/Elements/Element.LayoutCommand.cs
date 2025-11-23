using Age.Core.Extensions;
using Age.Commands;
using Age.Core.Collections;
using System.Numerics;
using System.Diagnostics.CodeAnalysis;

using static Age.Shaders.CanvasShader;

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

    private LayoutCommand layoutCommands;

    private RectCommand AllocateLayoutCommand(LayoutCommand layoutCommand)
    {
        var index = this.GetIndex(layoutCommand);

        if (this.layoutCommands.HasFlags(layoutCommand))
        {
            return (RectCommand)this.Commands[index];
        }

        var command = CommandPool.RectCommand.Get(this, CommandFilter.Color | CommandFilter.Encode);

        if (layoutCommand == LayoutCommand.Box)
        {
            command.Flags = Flags.ColorAsBackground;
        }

        command.StencilLayer = this.StencilLayer;

        this.InsertCommand(index, command);

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

            this.RemoveCommandAt(index);

            this.layoutCommands &= ~layoutCommand;

            this.UpdateCommandsSeparator();
        }
    }

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

    private void UpdateBackgroundImageSize()
    {
        if (this.TryGetLayoutCommandImage(out var layoutCommandImage))
        {
            this.ResolveImageSize(this.ComputedStyle.BackgroundImage!, layoutCommandImage.TextureMap.Texture.Size, out var size, out var matrix, out var uv);

            layoutCommandImage.Size        = size;
            layoutCommandImage.LocalMatrix = matrix;
            layoutCommandImage.TextureMap  = layoutCommandImage.TextureMap with { UV = uv };
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

        this.SetCommandsSeparator(layoutCommand.HasValue ? this.GetIndex(layoutCommand.Value) + 1 : 0);
    }
}
