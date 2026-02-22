import "./Geometry2DShader.Helpers";

#include "./Geometry2DShader.slang"

[shader("fragment")]
FragmentOutput main(in FragmentInput input)
{
    if (hasBorder(data))
    {
        var position = data.size * input.uv;

        BorderSide side;
        CornerKind corner;

        if (isCorner(data, position, corner))
        {
            float4 cornerColor;
            bool   isOutside;

            if (isInsideRadius(data, position, corner, cornerColor, isOutside))
            {
                discard;
            }
            else if (isOutside)
            {
                discard;
            }
        }
        else if (isBorder(data, position, side))
        {
            discard;
        }
    }

    return { };
}
