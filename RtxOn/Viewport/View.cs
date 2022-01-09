using RtxOn.Algebra;

namespace RtxOn.Viewport;

public class View
{
    public Camera Camera { get; }

    public int WidthPx { get; }
    public int HeightPx { get; }
    private readonly float _ratio;

    public View(Camera camera, int widthPx, int heightPx)
    {
        Camera = camera;
        
        WidthPx = widthPx;
        HeightPx = heightPx;
        _ratio = (float)widthPx / heightPx;
    }

    public IEnumerable<(Vector3 direction, int ix, int iy)> Pixels()
    {
        foreach (var (y, iy) in Utils.FloatRange(-1 / _ratio, 1 / _ratio, HeightPx).Enumerate())
        {
            foreach (var (x, ix) in Utils.FloatRange(-1, 1, WidthPx).Enumerate())
            {
                var pixel = Camera.ScreenMid + Camera.TowardsScreenEdge * y + Camera.TowardsScreenSide * x;
                var direction = pixel.Minus(Camera.FocalPoint).Normalize();

                yield return (direction, ix, iy);
            }
        }
    }

    public override string ToString() => $"{WidthPx}x{HeightPx} - {Camera}";
}