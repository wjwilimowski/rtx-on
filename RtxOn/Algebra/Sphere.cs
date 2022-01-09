using RtxOn.Materials;

namespace RtxOn.Algebra;

public class Sphere : IVisible
{
    public readonly Vector3 Center;
    public readonly float Radius;
    public BlinnPhong Color { get; }

    public Sphere(float x, float y, float z, float r, BlinnPhong color)
    {
        Center = new Vector3(x, y, z);
        Radius = r;
        Color = color;
    }

    public Sphere(Vector3 center, float r, BlinnPhong color)
    {
        Center = center;
        Radius = r;
        Color = color;
    }

    public bool TryIntersect(Vector3 rayDirection, Vector3 rayOrigin, out float distance)
    {
        var centerToOrigin = rayOrigin.Minus(Center);
        var centerToOriginNorm = centerToOrigin.Norm();
        var b = 2 * rayDirection.Dot(centerToOrigin);
        var c = centerToOriginNorm * centerToOriginNorm - Radius * Radius;
        
        if (Utils.Delta(b, c, out float t1, out float t2) > 0 && t1 > 0 && t2 > 0)
        {
            distance = Math.Min(t1, t2);
            return true;
        }

        distance = default;
        return false;
    }
    
    public Vector3 GetCollisionNormal(Vector3 intersectionPoint)
    {
        return intersectionPoint.Minus(Center).Normalize();
    }
}