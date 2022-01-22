namespace RtxOn.Algebra;

public static class VisibleExtensions
{
    public static bool TryFindNearest(this IEnumerable<IVisible> visibles, Vector3 origin, Vector3 ray, out IVisible nearestVisible, out float nearestDistance)
    {
        nearestDistance = float.MaxValue;
        nearestVisible = null;
    
        foreach (var visible in visibles)
        {
            if (visible.TryIntersect(ray, origin, out var sphereDistance) && sphereDistance < nearestDistance)
            {
                nearestDistance = sphereDistance;
                nearestVisible = visible;
            }
        }

    
        return nearestDistance < float.MaxValue;
    }
}