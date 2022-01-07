using RtxOn.Algebra;

namespace RtxOn.Viewport;

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