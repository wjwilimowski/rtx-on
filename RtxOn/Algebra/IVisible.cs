using RtxOn.Materials;

namespace RtxOn.Algebra;

public interface IVisible
{
    bool TryIntersect(Vector3 rayDirection, Vector3 rayOrigin, out float distance);
    BlinnPhong Color { get; }

    Vector3 GetCollisionNormal(Vector3 intersectionPoint);
}