#ifndef BEZIER_H
#define BEZIER_H

vec2 cubic_bezier(vec2 p0, vec2 p1, vec2 p2, vec2 p3, float t)
{
    mat4 M = mat4(
         1.0,  0.0,  0.0, 0.0,
        -3.0,  3.0,  0.0, 0.0,
         3.0, -6.0,  3.0, 0.0,
        -1.0,  3.0, -3.0, 1.0
    );

    vec4 T = vec4(1.0, t, t * t, t * t * t);

    vec4 result = M * T;

    float x = dot(result, vec4(p0.x, p1.x, p2.x, p3.x));
    float y = dot(result, vec4(p0.y, p1.y, p2.y, p3.y));

    return vec2(x, y);
}

vec2 quadratic_bezier(vec2 p0, vec2 p1, vec2 p2, float t)
{
    vec2 q0 = mix(p0, p1, t);
    vec2 q1 = mix(p1, p2, t);

    return mix(q0, q1, t);
}

#endif
