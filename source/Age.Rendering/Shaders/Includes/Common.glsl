#ifndef COMMON_H
#define COMMON_H

#define PI 3.141592653589793

vec4 to_vec4(float elements[4])
{
    return vec4(elements[0], elements[1], elements[2], elements[3]);
}

bool has_flag(uint value, uint flag)
{
    return (value & flag) == flag;
}

bool is_point_inside_ellipse(vec2 point, vec2 center, float a, float b)
{
    float dx = point.x - center.x;
    float dy = point.y - center.y;

    float value = (dx * dx) / (a * a) + (dy * dy) / (b * b);

    return value <= 1;
}

bool is_point_inside_right_triangle(vec2 point, vec2 v1, vec2 v2, vec2 right_angle_vertex)
{
    if (point.x < min(v1.x, right_angle_vertex.x) || point.x > max(v1.x, right_angle_vertex.x) || point.y < min(v2.y, right_angle_vertex.y) || point.y > max(v2.y, right_angle_vertex.y))
    {
        return false;
    }

    float hypotenuseY = v2.y + (right_angle_vertex.y - v2.y) * ((point.x - v2.x) / (right_angle_vertex.x - v2.x));

    return point.y <= hypotenuseY;
}

#endif
