using RtxOn.Materials;

namespace RtxOn.Rendering;

public readonly struct RenderedPixel
{
    public readonly int X;
    public readonly int Y;
    public readonly Color Color;

    public RenderedPixel(int x, int y, Color color)
    {
        X = x;
        Y = y;
        Color = color;
    }
}