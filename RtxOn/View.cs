namespace RtxOn;

public class Camera
{
    public Vector3 ScreenMid { get; private set; }
    public Vector3 Direction { get; private set; }
    public float FocalLength { get; private set; }

    public Vector3 TowardsScreenSide { get; private set; }
    public Vector3 TowardsScreenEdge { get; private set; }
    public Vector3 FocalPoint { get; private set; }
    
    public Camera(Vector3 screenMid, Vector3 direction, float focalLength)
    {
        ScreenMid = screenMid;
        Direction = direction;
        FocalLength = focalLength;
        
        Recalculate();
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

    public void LookAt(Vector3 point)
    {
        SetDirection((point - ScreenMid).Normalize());
    }

    public void SetFocalLength(float focalLength)
    {
        FocalLength = focalLength;
        Recalculate();
    }

    private void Recalculate()
    {
        var dirUp = new Vector3(0, 1, 0);

        TowardsScreenSide = Vector3.NormalToSurfaceFromPoint(Direction, dirUp).Normalize();
        TowardsScreenEdge = Vector3.NormalToSurfaceFromPoint(TowardsScreenSide, Direction).Normalize();
        
        FocalPoint = ScreenMid + Direction * -FocalLength;
    }
}

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