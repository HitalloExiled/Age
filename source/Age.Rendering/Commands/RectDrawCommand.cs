using Age.Numerics;
using Age.Rendering.Enums;

namespace Age.Rendering.Commands;

public record RectDrawCommand(Rect<float> Rect, Texture Texture) : DrawCommand(DrawCommandType.Rect);
