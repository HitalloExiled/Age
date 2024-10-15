#ifndef TRANSFORM_H
#define TRANSFORM_H

struct Transform
{
    mat2 rotation;
    vec2 position;
};

mat3 transform_to_mat3(in Transform transform)
{
    return mat3(
        vec3(transform.rotation[0], 0),
        vec3(transform.rotation[1], 0),
        vec3(transform.position, 1)
    );
}

#endif
