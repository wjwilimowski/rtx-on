using RtxOn.Materials;

namespace RtxOn.Algebra;

public class Cylinder : IVisible
{
    private readonly float _radius;
    private readonly Vector3 _bottom;
    private readonly Vector3 _top;
    private readonly Vector3 _bottomToTop;
    private readonly float _height;
    
    public Cylinder(BlinnPhong color, Vector3 bottom, Vector3 top, float radius)
    {
        Color = color;
        _bottom = bottom;
        _top = top;
        _radius = radius;
        _bottomToTop = (_top - _bottom).Normalize(out _height);
    }

    public bool TryIntersect(Vector3 rayDirection, Vector3 rayOrigin, out float distance)
    {
        distance = default;
        
        var rc = rayOrigin - _bottom;
        var n = rayDirection.CrossProduct(_bottomToTop).Normalize(out float ln);

        if (ln is < Const.Epsilon and > Const.MinusEpsilon)
        {
            // TODO epsilon
            return false;
        }

        var d = Math.Abs(rc.Dot(n));

        if (d > _radius) 
            return false;
        
        var o1 = rc.CrossProduct(_bottomToTop);
        var t = o1.Dot(n) / -ln;

        var o2 = n.CrossProduct(_bottomToTop);
        var s = (float)Math.Abs(Math.Sqrt(_radius * _radius - d * d) / rayDirection.Dot(o2));

        var @in = t - s;
        var @out = t + s;

        if (@in < Const.MinusEpsilon)
        {
            if (@out < Const.MinusEpsilon)
            {
                return false;
            }
            
            distance = @out;
        } else if (@out < Const.MinusEpsilon || @in < @out)
        {
            distance = @in;
        }
        else
        {
            distance = @out;
        }

        return true;
    }

    public BlinnPhong Color { get; }
    public Vector3 GetCollisionNormal(Vector3 intersectionPoint)
    {
        var bottomToIntersection = intersectionPoint - _bottom;
        var distanceFromBottomToCast = bottomToIntersection.Dot(_bottomToTop);
        var cast = _bottom + _bottomToTop * distanceFromBottomToCast;
        return (intersectionPoint - cast).Normalize();
    }
}