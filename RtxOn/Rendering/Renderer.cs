﻿using System.Collections.Concurrent;
using System.Numerics;
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

    private RenderedPixel RenderPixel(Scene scene, Vector3 focalPoint, Vector3 direction, int ix, int iy)
    {
        var origin = focalPoint;
        var reflection = 1f;
        ObjColor illumination = ObjColor.Black;

        for (int i = 0; i < _nReflections; i++)
        {
            if (!TryFindNearestSphere(scene.Spheres, origin, direction, out Sphere obj, out float distance))
            {
                break;
            }

            var rawIntersectionPoint = origin + direction * distance;
            var surfaceNormal = rawIntersectionPoint.Minus(obj.Center).Normalize();
            var intersectionPoint = rawIntersectionPoint + surfaceNormal * Epsilon;

            var rayFromIntersectionToLight = scene.Light.Position.Minus(intersectionPoint);
            var distanceFromIntersectionToLight = rayFromIntersectionToLight.Norm();
            var intersectionToLight = rayFromIntersectionToLight / distanceFromIntersectionToLight;

            if (TryFindNearestSphere(scene.Spheres, intersectionPoint, intersectionToLight, out _, out float d) &&
                d < distanceFromIntersectionToLight)
            {
                if (_enableAmbientLight)
                {
                    illumination += obj.Color.Ambient;
                }

                break;
            }

            illumination += BlinnPhong.Illuminate(
                obj.Color,
                scene.Light.Color,
                intersectionToLight,
                surfaceNormal,
                focalPoint.Minus(intersectionPoint).Normalize()) * reflection;

            reflection *= obj.Color.Reflection;
            origin = intersectionPoint;
            direction = direction.Reflected(surfaceNormal);
        }

        illumination = illumination.Clamp();

        var pixel = new RenderedPixel(ix, iy, illumination);
        return pixel;
    }

    private static bool TryFindNearestSphere(Sphere[] spheres, Vector3 origin, Vector3 ray, out Sphere nearestSphere, out float nearestDistance)
    {
        nearestDistance = float.MaxValue;
        nearestSphere = default;
    
        foreach (var sphere in spheres)
        {
            if (sphere.TryIntersect(ray, origin, out var sphereDistance) && sphereDistance < nearestDistance)
            {
                nearestDistance = sphereDistance;
                nearestSphere = sphere;
            }
        }

    
        return nearestDistance < float.MaxValue;
    }
}