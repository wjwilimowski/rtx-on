using RtxOn.Materials;

namespace RtxOn.Algebra;

public class Plane : IVisible
{
    private readonly Vector3 _position;
    private readonly Vector3 _normal;

    public BlinnPhong Color { get; }
    
    public Plane(Vector3 position, Vector3 normal, BlinnPhong color)
    {
        Color = color;
        _normal = normal;
        _position = position;
    }

    public bool TryIntersect(Vector3 rayDirection, Vector3 rayOrigin, out float distance)
    {
        distance = default;
        
        var denominator = rayDirection.Dot(_normal);
        if (denominator == 0f)
            return false; // the ray is parallel to the plane or contained within the plane

        var nominator = (_position - rayOrigin).Dot(_normal);
        distance = nominator / denominator;
        if (distance <= 0)
            return false; // the plane is in the opposite direction

        return true;
    }

    public Vector3 GetCollisionNormal(Vector3 intersectionPoint) => _normal;
}