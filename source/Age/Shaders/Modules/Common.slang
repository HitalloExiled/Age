module Common;

public bool hasFlag(uint value, uint flag)
{
    return (value & flag) == flag;
}

public bool isPointInsideEllipse(in float2 point, in float2 center, float a, float b)
{
    var dx = point.x - center.x;
    var dy = point.y - center.y;

    var value = (dx * dx) / (a * a) + (dy * dy) / (b * b);

    return value <= 1;
}

public bool isPointInsideRightTriangle(in float2 point, in float2 v1, in float2 v2, in float2 rightAngleVertex)
{
    if (point.x < min(v1.x, rightAngleVertex.x) || point.x > max(v1.x, rightAngleVertex.x) || point.y < min(v2.y, rightAngleVertex.y) || point.y > max(v2.y, rightAngleVertex.y))
    {
        return false;
    }

    var hypotenuseY = v2.y + (rightAngleVertex.y - v2.y) * ((point.x - v2.x) / (rightAngleVertex.x - v2.x));

    return point.y <= hypotenuseY;
}

public float4 toFloat4(in float elements[4])
{
    return float4(elements[0], elements[1], elements[2], elements[3]);
}
