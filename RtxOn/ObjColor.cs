namespace RtxOn;

public readonly struct ObjColor
{
    public readonly float R;
    public readonly float G;
    public readonly float B;

    public ObjColor(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }

    public static ObjColor Red => new ObjColor(.7f, 0, 0);
    public static ObjColor Green => new ObjColor(0, .7f, 0);
    public static ObjColor Blue => new ObjColor(0, 0, .7f);
    public static ObjColor White => new ObjColor(1, 1, 1);
    public static ObjColor Black => new ObjColor(0, 0, 0);
    public static ObjColor Purple => new ObjColor(.7f, 0, .7f);

    public static ObjColor operator *(ObjColor x, ObjColor y)
    {
        return new ObjColor(x.R * y.R, x.G * y.G, x.B * y.B);
    }

    public static ObjColor operator *(ObjColor x, float mul)
    {
        return new ObjColor(x.R * mul, x.G * mul, x.B * mul);
    }

    public static ObjColor operator +(ObjColor x, ObjColor y)
    {
        return new ObjColor(x.R + y.R, x.G + y.G, x.B + y.B);
    }

    public ObjColor Clamp()
    {
        return new ObjColor(Math.Clamp(R, 0, 1), Math.Clamp(G, 0, 1), Math.Clamp(B, 0, 1));
    }
}