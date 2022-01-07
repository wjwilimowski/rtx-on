namespace RtxOn;

public static class Rendering
{
    public static IEnumerable<RenderedPixel> RenderScene(Scene scene, int nReflections)
    {
        var spheres = scene.Spheres;
        var light = scene.Light;
        var camera = scene.ViewCamera.FocalPoint;
        
        foreach ((Vector3 pixelDirection, int ix, int iy) in scene.ViewCamera.Pixels())
        {
            var direction = pixelDirection;
            var origin = scene.ViewCamera.FocalPoint;
            var reflection = 1f;
            ObjColor illumination = ObjColor.Black;
        
            for (int i = 0; i < nReflections; i++)
            {
                if (!TryFindNearestSphere(spheres, origin, direction, out Sphere obj, out float distance))
                {
                    break;
                }
            
                var rawIntersectionPoint = origin + direction * distance;
                var surfaceNormal = rawIntersectionPoint.Minus(obj.Center).Normalize();
                const float epsilon = 0.000001f;
                var intersectionPoint = rawIntersectionPoint + surfaceNormal * epsilon;
            
                var rayFromIntersectionToLight = light.Position.Minus(intersectionPoint);
                var distanceFromIntersectionToLight = rayFromIntersectionToLight.Norm();
                var intersectionToLight = rayFromIntersectionToLight / distanceFromIntersectionToLight;

                if (TryFindNearestSphere(spheres, intersectionPoint, intersectionToLight, out _, out float d) &&
                    d < distanceFromIntersectionToLight)
                {
                    break;
                }

                illumination += BlinnPhong.Illuminate(
                    obj.Color,
                    light.Color,
                    intersectionToLight,
                    surfaceNormal,
                    camera.Minus(intersectionPoint).Normalize()) * reflection;

                reflection *= obj.Color.Reflection;
                origin = intersectionPoint;
                direction = direction.Reflected(surfaceNormal);
            }

            illumination = illumination.Clamp();

            yield return new RenderedPixel(ix, iy, illumination);
        }
    }

    private static bool TryFindNearestSphere(Sphere[] spheres, Vector3 camera, Vector3 ray, out Sphere nearestSphere, out float nearestDistance)
    {
        nearestDistance = float.MaxValue;
        nearestSphere = default;
    
        foreach (var sphere in spheres)
        {
            if (sphere.TryIntersect(ray, camera, out var sphereDistance) && sphereDistance < nearestDistance)
            {
                nearestDistance = sphereDistance;
                nearestSphere = sphere;
            }
        }

    
        return nearestDistance < float.MaxValue;
    }
}