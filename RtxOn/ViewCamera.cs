namespace RtxOn;

public class ViewCamera
{
    public Vector3 ScreenMid { get; private set; }
    public Vector3 Direction { get; private set; }
    public float FocalLength { get; private set; }
    
    private Vector3 _towardsScreenSide;
    private Vector3 _towardsScreenEdge;
    public Vector3 FocalPoint { get; private set; }
    
    private readonly int _widthPx;
    private readonly int _heightPx;
    private readonly float _ratio;

    public ViewCamera(Vector3 screenMid, Vector3 direction, float focalLength, int widthPx, int heightPx)
    {
        ScreenMid = screenMid;
        Direction = direction;
        FocalLength = focalLength;
        
        _widthPx = widthPx;
        _heightPx = heightPx;
        _ratio = (float)widthPx / heightPx;
        
        Recalculate();
    }

    public IEnumerable<(Vector3 direction, int ix, int iy)> Pixels()
    {
        foreach (var (y, iy) in Utils.FloatRange(-1 / _ratio, 1 / _ratio, _heightPx).Enumerate())
        {
            foreach (var (x, ix) in Utils.FloatRange(-1, 1, _widthPx).Enumerate())
            {
                var pixel = ScreenMid + _towardsScreenEdge * y + _towardsScreenSide * x;
                var direction = pixel.Minus(FocalPoint).Normalize();

                yield return (direction, ix, iy);
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        ScreenMid = position;
        Recalculate();
    }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction;
        Recalculate();
    }

    public void SetFocalLength(float focalLength)
    {
        FocalLength = focalLength;
        Recalculate();
    }

    private void Recalculate()
    {
        var dirUp = new Vector3(0, 1, 0);

        _towardsScreenSide = Vector3.NormalToSurfaceFromPoint(Direction, dirUp).Normalize();
        _towardsScreenEdge = Vector3.NormalToSurfaceFromPoint(_towardsScreenSide, Direction).Normalize();
        
        FocalPoint = ScreenMid + Direction * -FocalLength;
    }
}