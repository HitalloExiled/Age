implementing Numerics;

public float2 cubicBezier(float2 p0, float2 p1, float2 p2, float2 p3, float t)
{
    var M = float4x4(
         1.0,  0.0,  0.0, 0.0,
        -3.0,  3.0,  0.0, 0.0,
         3.0, -6.0,  3.0, 0.0,
        -1.0,  3.0, -3.0, 1.0
    );

    var T = float4(1.0, t, t * t, t * t * t);

    var result = mul(M, T);

    var x = dot(result, float4(p0.x, p1.x, p2.x, p3.x));
    var y = dot(result, float4(p0.y, p1.y, p2.y, p3.y));

    return float2(x, y);
}

public float2 quadraticBezier(float2 p0, float2 p1, float2 p2, float t)
{
    var q0 = lerp(p0, p1, t);
    var q1 = lerp(p1, p2, t);

    return lerp(q0, q1, t);
}
