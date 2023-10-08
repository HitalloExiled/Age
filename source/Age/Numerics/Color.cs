namespace Age.Numerics;

public record struct Color
{
    public float R;
    public float G;
    public float B;
    public float A = 1;

    public Color(float r, float g, float b, float a = 1)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
    }
}
