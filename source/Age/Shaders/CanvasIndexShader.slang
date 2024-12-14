import "./CanvasShader.Helpers";

#include "./CanvasShader.Base.slang"

[shader("fragment")]
FragmentOutput main(in FragmentInput input)
{
    var color = data.color;

    var hasBorder = data.border.top.thickness
        + data.border.right.thickness
        + data.border.bottom.thickness
        + data.border.left.thickness
        + data.border.radius.leftTop
        + data.border.radius.topRight
        + data.border.radius.rightBottom
        + data.border.radius.bottomLeft > 0;

    if (hasBorder)
    {
        var position = data.rect.size * input.uv;

        BorderSide side;
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
