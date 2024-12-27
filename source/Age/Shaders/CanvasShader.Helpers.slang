import Common;
import Numerics;

import "./CanvasShader.BorderSide";
import "./CanvasShader.PushConstant";

public enum CornerKind
{
    None,
    LeftTop,
    TopRight,
    RightBottom,
    BottomLeft,
}

public enum BorderKind
{
    None,
    Top,
    Right,
    Bottom,
    Left,
}

void getCorners(in PushConstant data, out Rect leftTop, out Rect topRight, out Rect rightBottom, out Rect bottomLeft)
{
    leftTop     = Rect(float2(data.border.left.thickness,  data.border.top.thickness),    float2(0, 0));
    topRight    = Rect(float2(data.border.right.thickness, data.border.top.thickness),    float2(data.rect.size.x - data.border.right.thickness, 0));
    rightBottom = Rect(float2(data.border.right.thickness, data.border.bottom.thickness), float2(data.rect.size.x - data.border.right.thickness, data.rect.size.y - data.border.bottom.thickness));
    bottomLeft  = Rect(float2(data.border.left.thickness,  data.border.bottom.thickness), float2(0, data.rect.size.y - data.border.bottom.thickness));
}

uint getMinRadius(uint radius, in Rect rect)
{
    return uint(min(radius, min(rect.size.x * 0.5, rect.size.y * 0.5)));
}

float2 getMax(float2 vector)
{
    var value = max(vector.x, vector.y);

    return float2(value, value);
}

bool intersectsBorders(in PushConstant data, in float2 position, BorderKind border)
{
    Rect leftTop;
    Rect topRight;
    Rect rightBottom;
    Rect bottomLeft;

    getCorners(data, leftTop, topRight, rightBottom, bottomLeft);

    switch (border)
    {
        case BorderKind.Top:
        {
            var start = leftTop.position.x  + int(position.y) / leftTop.size.y  * leftTop.size.x;
            var end   = topRight.position.x + int(position.y) / topRight.size.y * topRight.size.x + topRight.size.x;

            return position.x < start || position.x > end;
        }
        case BorderKind.Right:
        {
            var start = topRight.position.y    + int(data.rect.size.x - position.x) / topRight.size.x    * topRight.size.y;
            var end   = rightBottom.position.y + int(data.rect.size.x - position.x) / rightBottom.size.x * rightBottom.size.y + rightBottom.size.y;

            return position.y < start || position.y > end;
        }
        case BorderKind.Bottom:
        {
            var start = (bottomLeft.position.x  + int(data.rect.size.y - position.y) / bottomLeft.size.y  * bottomLeft.size.x);
            var end   = (rightBottom.position.x + int(data.rect.size.y - position.y) / rightBottom.size.y * rightBottom.size.x + rightBottom.size.x);

            return position.x < start || position.x > end;
        }
        case BorderKind.Left:
        {
            var start = leftTop.position.y    + int(position.x) / leftTop.size.x     * leftTop.size.y;
            var end   = bottomLeft.position.y + int(position.x) / bottomLeft.size.x * bottomLeft.size.y + bottomLeft.size.y;

            return position.y < start || position.y > end;
        }
    }

    return false;
}

public bool hasBorder(in PushConstant data)
{
    return data.border.top.thickness
        + data.border.right.thickness
        + data.border.bottom.thickness
        + data.border.left.thickness
        + data.border.radius.leftTop
        + data.border.radius.topRight
        + data.border.radius.rightBottom
        + data.border.radius.bottomLeft > 0;
}

public bool isBorder(in PushConstant data, in float2 position, out BorderSide side)
{
    if (position.y >= 0 && position.y <= data.border.top.thickness && !intersectsBorders(data, position, BorderKind.Top))
    {
        side = data.border.top;
    }
    else if (position.x >= data.rect.size.x - data.border.right.thickness && !intersectsBorders(data, position, BorderKind.Right))
    {
        side = data.border.right;
    }
    else if (position.y >= data.rect.size.y - data.border.bottom.thickness && !intersectsBorders(data, position, BorderKind.Bottom))
    {
        side = data.border.bottom;
    }
    else if (position.x >= 0 && position.x <= data.border.left.thickness && !intersectsBorders(data, position, BorderKind.Left))
    {
        side = data.border.left;
    }
    else
    {
        side = { };

        return false;
    }

    return true;
}

public bool isCorner(in PushConstant data, in float2 position, out CornerKind corner)
{
    uint leftTopRadius     = getMinRadius(data.border.radius.leftTop, data.rect);
    uint topRightRadius    = getMinRadius(data.border.radius.topRight, data.rect);
    uint rightBottomRadius = getMinRadius(data.border.radius.rightBottom, data.rect);
    uint bottomLeftRadius  = getMinRadius(data.border.radius.bottomLeft, data.rect);

    if (position.x >= 0 && position.x < leftTopRadius && position.y >= 0 && position.y <= leftTopRadius)
    {
        corner = CornerKind.LeftTop;
    }
    else if (position.y >= 0 && position.y <= topRightRadius && position.x >= data.rect.size.x - topRightRadius)
    {
        corner = CornerKind.TopRight;
    }
    else if (position.x >= data.rect.size.x - rightBottomRadius && position.y >= data.rect.size.y - rightBottomRadius)
    {
        corner = CornerKind.RightBottom;
    }
    else if (position.y >= data.rect.size.y - bottomLeftRadius && position.x >= 0 && position.x < bottomLeftRadius)
    {
        corner = CornerKind.BottomLeft;
    }
    else
    {
        corner = CornerKind.None;

        return false;
    }

    return true;
}

public bool isInsideRadius(in PushConstant data, in float2 position, CornerKind corner, out float4 color, out bool isOutside)
{
    Rect leftTop;
    Rect topRight;
    Rect rightBottom;
    Rect bottomLeft;

    getCorners(data, leftTop, topRight, rightBottom, bottomLeft);

    float2 origin;
    float  radius;
    float  thicknessX;
    float  thicknessY;

    switch (corner)
    {
        case CornerKind.LeftTop:
            {
                radius     = getMinRadius(data.border.radius.leftTop, data.rect);
                origin     = float2(radius, radius);
                thicknessX = data.border.left.thickness;
                thicknessY = data.border.top.thickness;

                var range = leftTop.position.x + position.y / leftTop.size.y * leftTop.size.x;

                color = position.x > range ? toFloat4(data.border.top.color) : toFloat4(data.border.left.color);
            }
            break;
        case CornerKind.TopRight:
            {
                radius     = getMinRadius(data.border.radius.topRight, data.rect);
                origin     = float2(data.rect.size.x - radius, radius);
                thicknessX = data.border.right.thickness;
                thicknessY = data.border.top.thickness;

                var range = topRight.position.x + position.y / topRight.size.y * topRight.size.x + topRight.size.x;

                color = position.x < range ? toFloat4(data.border.top.color) : toFloat4(data.border.right.color);
            }
            break;
        case CornerKind.RightBottom:
            {
                radius     = getMinRadius(data.border.radius.rightBottom, data.rect);
                origin     = float2(data.rect.size.x - radius, data.rect.size.y - radius);
                thicknessX = data.border.right.thickness;
                thicknessY = data.border.bottom.thickness;

                var range = rightBottom.position.x + (data.rect.size.y - position.y) / rightBottom.size.y * rightBottom.size.x + rightBottom.size.x;

                color = position.x < range ? toFloat4(data.border.bottom.color) : toFloat4(data.border.right.color);
            }
            break;
        case CornerKind.BottomLeft:
            {
                radius     = getMinRadius(data.border.radius.bottomLeft, data.rect);
                origin     = float2(radius, data.rect.size.y - radius);
                thicknessX = data.border.left.thickness;
                thicknessY = data.border.bottom.thickness;

                var range = bottomLeft.position.x + (data.rect.size.y - position.y) / bottomLeft.size.y * bottomLeft.size.x;

                color = position.x > range ? toFloat4(data.border.bottom.color) : toFloat4(data.border.left.color);
            }
            break;
    }

    var directionLength = length(position - origin);

    if (!(isOutside = directionLength > radius))
    {
        if (thicknessX == thicknessY)
        {
            return directionLength > radius - thicknessX;
        }

        var radiusX = radius > thicknessX ? radius - thicknessX : 0;
        var radiusY = radius > thicknessY ? radius - thicknessY : 0;

        if (radiusX > 0 && radiusY > 0)
        {
            return !isPointInsideEllipse(position, origin, radiusX, radiusY);
        }

        return true;
    }

    return false;
}
