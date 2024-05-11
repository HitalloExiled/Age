#ifndef RECT_H
#define RECT_H

struct Rect
{
    vec2 size;
    vec2 position;
};

bool intersects(vec2 position, Rect rect)
{
    return position.x >= rect.position.x && position.x <= rect.position.x + rect.size.x && position.y >= rect.position.y && position.y <= rect.position.y + rect.size.y;
}

#endif
