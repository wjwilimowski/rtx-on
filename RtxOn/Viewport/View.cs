using RtxOn.Algebra;

namespace RtxOn.Viewport;

public class View
{
    public Camera Camera { get; }
    
    private readonly int _widthPx;
    private readonly int _heightPx;
    private readonly float _ratio;

    public View(Camera camera, int widthPx, int heightPx)
    {
        Camera = camera;
        
        _widthPx = widthPx;
        _heightPx = heightPx;
        _ratio = (float)widthPx / heightPx;
    }

    public IEnumerable<(Vector3 direction, int ix, int iy)> Pixels()
    {
        foreach (var (y, iy) in Utils.FloatRange(-1 / _ratio, 1 / _ratio, _heightPx).Enumerate())
        {
            foreach (var (x, ix) in Utils.FloatRange(-1, 1, _widthPx).Enumerate())
            {
                var pixel = Camera.ScreenMid + Camera.TowardsScreenEdge * y + Camera.TowardsScreenSide * x;
                var direction = pixel.Minus(Camera.FocalPoint).Normalize();

                yield return (direction, ix, iy);
            }
        }
    }
}