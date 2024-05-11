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

#endif
