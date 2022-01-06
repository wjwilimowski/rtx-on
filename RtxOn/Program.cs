// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

var width = 640;
var height = 400;
var ratio = (float)width / height;

float left = 1;
float right = -1;
float top = 1 / ratio;
float bottom = -1 / ratio;

var spheres = new Sphere[]
{
    new Sphere(-.2f, 0, -1, .7f, Color.Purple),
    new Sphere(.1f, -.3f, 0, .1f, Color.Green),
    new Sphere(-.3f, 0, 0, .15f, Color.Blue),
};

var camera = new Vector3(0, 0, 1.0f);
var light = new Light(new Vector3(5, 5, 5), Color.White);

var bitmap = new Bitmap(width, height);

foreach (var (y, iy) in Utils.FloatRange(top, bottom, height).Enumerate())
{
    foreach (var (x, ix) in Utils.FloatRange(left, right, width).Enumerate())
    {
        var pixel = new Vector3(x, y, 0);
        var direction = pixel.Minus(camera).Normalize();

        Color c = Color.Black;
        
        if (TryFindNearestSphere(spheres, camera, direction, out Sphere sphere, out float distance))
        {
            var rawIntersectionPoint = camera.Plus(direction.Mul(distance));
            var surfaceNormal = rawIntersectionPoint.Minus(sphere.Center).Normalize();
            var intersectionPoint = rawIntersectionPoint.Plus(surfaceNormal.Mul(0.00001f));
            
            var rayFromIntersectionToLight = light.Position.Minus(intersectionPoint);
            var distanceToLight = rayFromIntersectionToLight.Norm();

            var directionFromIntersectionToLight = rayFromIntersectionToLight.Normalize();
            
            var isShadowed = TryFindNearestSphere(spheres, intersectionPoint, directionFromIntersectionToLight, out _,
                out float shadowingObjectDistance) && shadowingObjectDistance < distanceToLight;
            if (!isShadowed)
            {
                c = sphere.Color;
            }
        }
        
        bitmap.SetPixel(ix, iy, c);
    }
}

bool TryFindNearestSphere(Sphere[] spheres, Vector3 camera, Vector3 ray, out Sphere nearestSphere, out float nearestDistance)
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

bitmap.Save("output.png");

readonly struct Light
{
    public readonly Vector3 Position;
    public readonly Color Color;

    public Light(Vector3 position, Color color)
    {
        Position = position;
        Color = color;
    }
}

readonly struct Sphere
{
    public readonly Vector3 Center;
    public readonly float Radius;
    public readonly Color Color;

    public Sphere(float x, float y, float z, float r, Color color)
    {
        Center = new Vector3(x, y, z);
        Radius = r;
        Color = color;
    }

    public Sphere(Vector3 center, float r, Color color)
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
        var delta = b * b - 4 * c;
        if (delta > 0)
        {
            var t1 = (-b + (float)Math.Sqrt(delta)) / 2;
            var t2 = (-b - (float)Math.Sqrt(delta)) / 2;

            if (t1 > 0 && t2 > 0)
            {
                distance = Math.Min(t1, t2);
                return true;
            }
        }

        distance = default;
        return false;
    }
}