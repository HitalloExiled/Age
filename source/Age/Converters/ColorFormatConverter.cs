namespace Age.Converters;

public static class ColorFormatConverter
{
    public static uint RGBAtoBGRA(uint color)
    {
        var r = color & 255;
        var g = (color >> 8) & 255;
        var b = (color >> 16) & 255;
        var a = (color >> 24) & 255;

        return b | (g << 8) | (r << 16) | (a << 24);
    }
}
