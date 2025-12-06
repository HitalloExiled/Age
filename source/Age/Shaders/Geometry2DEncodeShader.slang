import "./Geometry2DShader.Helpers";

#include "./Geometry2DShader.slang"

[shader("fragment")]
FragmentOutput main(in FragmentInput input)
{
    var color = data.color;

    if (hasBorder(data))
    {
        var position = data.size * input.uv;

        CornerKind corner;

        if (isCorner(data, position, corner))
        {
            float4 cornerColor;
            bool   isOutside;

            if (!isInsideRadius(data, position, corner, cornerColor, isOutside) && isOutside)
            {
                discard;
            }
        }
    }

    return { color };
}
