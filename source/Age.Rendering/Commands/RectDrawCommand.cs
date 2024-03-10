using Age.Numerics;
using Age.Rendering.Enums;
using Age.Rendering.Resources;

namespace Age.Rendering.Commands;

public record RectDrawCommand(Rect<float> Rect, Texture Texture, Point<float>[] UV, Color Color, Sampler Sampler) : DrawCommand(DrawCommandType.Rect);
