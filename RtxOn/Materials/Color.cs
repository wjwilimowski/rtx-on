namespace RtxOn.Materials;

public readonly struct Color
{
    public readonly float R;
    public readonly float G;
    public readonly float B;

    public Color(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }

    public static Color Red => new Color(.7f, 0, 0);
    public static Color Green => new Color(0, .7f, 0);
    public static Color Blue => new Color(0, 0, .7f);
    public static Color White => new Color(1, 1, 1);
    public static Color Black => new Color(0, 0, 0);
    public static Color Purple => new Color(.7f, 0, .7f);

    public static Color operator *(Color x, Color y)
    {
        return new Color(x.R * y.R, x.G * y.G, x.B * y.B);
    }

    public static Color operator *(Color x, float mul)
    {
        return new Color(x.R * mul, x.G * mul, x.B * mul);
    }

    public static Color operator +(Color x, Color y)
    {
        return new Color(x.R + y.R, x.G + y.G, x.B + y.B);
    }

    public Color Clamp()
    {
        return new Color(Math.Clamp(R, 0, 1), Math.Clamp(G, 0, 1), Math.Clamp(B, 0, 1));
    }
}