using System.Collections.Concurrent;
using RtxOn.Algebra;
using RtxOn.Arrangement;
using RtxOn.Materials;
using RtxOn.Viewport;
using Vector3 = RtxOn.Algebra.Vector3;

namespace RtxOn.Rendering;

public class Renderer
{
    const float Epsilon = 0.00001f;

    private readonly int _nReflections;
    private readonly bool _enableAmbientLight;
    public Renderer(int nReflections, bool enableAmbientLight)
    {
        _nReflections = nReflections;
        _enableAmbientLight = enableAmbientLight;
    }
    
    public IEnumerable<RenderedPixel> Render(Scene scene, View view)
    {
        foreach ((Vector3 pixelDirection, int ix, int iy) in view.Pixels())
        {
            yield return RenderPixel(scene, view.Camera.FocalPoint, pixelDirection, ix, iy);
        }
    }
    
    public IEnumerable<RenderedPixel> RenderParallel(Scene scene, View view)
    {
        var result = new ConcurrentBag<RenderedPixel>();
        Parallel.ForEach(view.Pixels(), cameraPixel =>
        {
            var (pixelDirection, ix, iy) = cameraPixel;
            var pixel = RenderPixel(scene, view.Camera.FocalPoint, pixelDirection, ix, iy);

            result.Add(pixel);
        });

        return result.ToArray();
    }

    private RenderedPixel RenderPixel(Scene scene, Vector3 eye, Vector3 direction, int ix, int iy)
    {
        var origin = eye;
        var reflection = 1f;
        Color illumination = Color.Black;

        for (int i = 0; i < _nReflections; i++)
        {
            if (!TryFindNearestVisible(scene.Visibles, origin, direction, out IVisible obj, out float distance))
            {
                break;
            }

            var rawIntersectionPoint = origin + direction * distance;
            var normal = obj.GetCollisionNormal(rawIntersectionPoint);
            var intersection = rawIntersectionPoint + normal * Epsilon;

            var light = scene.Light;

            var (illuminate, isShadowed) = Illuminate(light, scene, eye, obj, intersection, normal);
            illumination += illuminate * reflection;

            if (isShadowed)
            {
                break;
            }


            reflection *= obj.Color.Reflection;
            origin = intersection;
            direction = direction.Reflected(normal);
        }

        illumination = illumination.Clamp();

        var pixel = new RenderedPixel(ix, iy, illumination);
        return pixel;
    }

    private (Color illuminate, bool isShadowed) Illuminate(
        Light light,
        Scene scene,
        Vector3 focalPoint,
        IVisible obj,
        Vector3 intersectionPoint,
        Vector3 surfaceNormal)
    {
        
        var rayFromIntersectionToLight = light.Position.Minus(intersectionPoint);
        var intersectionToLight = rayFromIntersectionToLight.Normalize(out float distanceFromIntersectionToLight);

        if (TryFindNearestVisible(scene.Visibles, intersectionPoint, intersectionToLight, out _, out float d) &&
            d < distanceFromIntersectionToLight)
        {
            return (_enableAmbientLight ? obj.Color.Ambient : Color.Black, true);
        }

        var illuminate = light.Illuminate(
            obj.Color,
            intersectionToLight,
            surfaceNormal,
            focalPoint.Minus(intersectionPoint).Normalize());
        
        return (illuminate, false);
    }

    private static bool TryFindNearestVisible(IEnumerable<IVisible> visibles, Vector3 origin, Vector3 ray, out IVisible nearestVisible, out float nearestDistance)
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