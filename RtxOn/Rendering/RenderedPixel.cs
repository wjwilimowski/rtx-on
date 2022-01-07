using RtxOn.Materials;

namespace RtxOn.Rendering;

public readonly struct RenderedPixel
{
    public readonly int X;
    public readonly int Y;
    public readonly ObjColor Color;

    public RenderedPixel(int x, int y, ObjColor color)
    {
        X = x;
        Y = y;
        Color = color;
    }
}