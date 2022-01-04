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
    new Sphere(-.2f, 0, -1, .7f, Color.Red),
    new Sphere(.1f, -.3f, 0, .1f, Color.Green),
    new Sphere(-.3f, 0, 0, .15f, Color.Blue),
};

var camera = new Vector3(0, 0, 1.0f);

var bitmap = new Bitmap(width, height);

foreach (var (y, iy) in Utils.FloatRange(top, bottom, height).Enumerate())
{
    foreach (var (x, ix) in Utils.FloatRange(left, right, width).Enumerate())
    {
        var pixel = new Vector3(x, y, 0);
        var direction = pixel.Minus(camera).Unit();

        Color c = Color.Black;
        
        float minDistance = float.MaxValue;
        foreach (var sphere in spheres)
        {
            if (sphere.TryIntersect(direction, camera, out var distance) && distance < minDistance)
            {
                minDistance = distance;
                c = sphere.Color;
            }
        }
        bitmap.SetPixel(ix, iy, c);
    }
}

bitmap.Save("output.png");

static class Utils
{
    public static IEnumerable<float> FloatRange(float min, float max, int count)
    {
        var step = (min - max) / count;
        return Enumerable.Range(0, count).Select(i => i * step - min);
    }
    
    public static IEnumerable<(T item, int index)> Enumerate<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }
}

readonly struct Vector3
{
    public readonly float X;
    public readonly float Y;
    public readonly float Z;

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3 Unit()
    {
        return Div(Norm());
    }

    public float Norm()
    {
        return (float) Math.Sqrt(X * X + Y * Y + Z * Z);
    }

    public Vector3 Div(float div)
    {
        return new Vector3(X / div, Y / div, Z / div);
    }

    public Vector3 Minus(Vector3 other)
    {
        return new Vector3(X - other.X, Y - other.Y, Z - other.Z);
    }

    public float Dot(Vector3 other)
    {
        return X * other.X + Y * other.Y + Z * other.Z;
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