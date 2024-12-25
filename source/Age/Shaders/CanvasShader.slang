import "./CanvasShader.Helpers";

#include "./CanvasShader.Base.slang"

[shader("fragment")]
public FragmentOutput main(in FragmentInput input)
{
    var texel = texture.Sample(input.uv);

    var color = hasFlag(data.flags, Flags.ColorAsBackground)
            ? data.color
            : hasFlag(data.flags, Flags.GrayscaleTexture | Flags.MultiplyColor)
                ? float4(1 - texel.rrr, texel.g) * data.color
                : texel;

    if (hasBorder(data))
    {
        var position = data.rect.size * input.uv;

        BorderSide side;
        CornerKind corner;

        if (isCorner(data, position, corner))
        {
            float4 cornerColor;
            bool   isOutside;

            if (isInsideRadius(data, position, corner, cornerColor, isOutside))
            {
                color = cornerColor;
            }
            else if (isOutside)
            {
                discard;
            }
        }
        else if (isBorder(data, position, side))
        {
            color = toFloat4(side.color);
        }
    }

    return { color };
}
