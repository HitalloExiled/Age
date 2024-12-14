implementing Numerics;

public struct Rect
{
    public float2 size;
    public float2 position;

    public __init(float2 size, float2 position)
    {
        this.size     = size;
        this.position = position;
    }

    public bool intersects(float2 position)
    {
        return position.x >= this.position.x && position.x <= this.position.x + this.size.x && position.y >= this.position.y && position.y <= this.position.y + this.size.y;
    }
}
