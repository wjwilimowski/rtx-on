// See https://aka.ms/new-console-template for more information

using System.Drawing;
using RtxOn;

var width = 1280;
var height = 800;
var ratio = (float)width / height;

float left = 1;
float right = -1;
float top = 1 / ratio;
float bottom = -1 / ratio;


var camera = new Vector3(0, 0, 1f);
var light = new Light(new Vector3(5, 5, 5), BlinnPhong.Bright);

var floorSphere = new Sphere(0, -9000f, 0, 9000f - .7f, BlinnPhong.Gray);

var sphere1 = new Sphere(-.2f, 0, -1, .7f, BlinnPhong.Red);
var sphere2 = new Sphere(.1f, -.3f, 0, .1f, BlinnPhong.Purple);
var sphere3 = new Sphere(-.3f, 0, 0, .15f, BlinnPhong.Green);

var spheres = new Sphere[]
{
    floorSphere,
    sphere1,
    sphere2,
    sphere3
};

const int nReflections = 3;

var bitmap = new Bitmap(width, height);

foreach (var (y, iy) in Utils.FloatRange(top, bottom, height).Reverse().Enumerate())
{
    foreach (var (x, ix) in Utils.FloatRange(left, right, width).Enumerate())
    {
        var pixel = new Vector3(x, y, 0f);
        var direction = pixel.Minus(camera).Normalize();

        var origin = camera;
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
                camera.Minus(intersectionPoint).Normalize());

            reflection *= obj.Color.Reflection;
            origin = intersectionPoint;
            direction = direction.Reflected(surfaceNormal);
        }

        illumination = illumination.Clamp();
                
        bitmap.SetPixel(ix, iy, Color.FromArgb((int)(255 * illumination.R), (int)(255 * illumination.G), (int)(255 * illumination.B)));
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
    public readonly BlinnPhong Color;

    public Light(Vector3 position, BlinnPhong color)
    {
        Position = position;
        Color = color;
    }
    
}

readonly struct BlinnPhong
{
    public readonly ObjColor Ambient;
    public readonly ObjColor Diffuse;
    public readonly ObjColor Specular;
    public readonly float Shininess;
    public readonly float Reflection;
    
    public static ObjColor Illuminate(
        BlinnPhong obj,
        BlinnPhong light,
        Vector3 intersectionToLight,
        Vector3 objSurfaceNormal,
        Vector3 intersectionToCamera)
    {
        var lv = intersectionToLight + intersectionToCamera;
        var dotLvNormal = objSurfaceNormal.Dot(lv.Normalize());
        var rawSpecularCoefficient = (float)Math.Pow(dotLvNormal, obj.Shininess / 4f);
        var specularCoefficient = float.IsNaN(rawSpecularCoefficient) ? 0f : rawSpecularCoefficient;
        
        var color = obj.Ambient * light.Ambient +
               obj.Diffuse * light.Diffuse * intersectionToLight.Dot(objSurfaceNormal) +
               obj.Specular * light.Specular * specularCoefficient;

        return color;
    }

    public BlinnPhong(ObjColor ambient, ObjColor diffuse, ObjColor specular, float shininess, float reflection)
    {
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
        Shininess = shininess;
        Reflection = reflection;
    }

    public static BlinnPhong Bright => new(
        ObjColor.White, 
        ObjColor.White, 
        ObjColor.White, 
        100f,
        .5f);
    
    public static BlinnPhong Red => new(
        new ObjColor(.1f, 0, 0), 
        ObjColor.Red, 
        ObjColor.White, 
        100f,
        .5f);
    
    public static BlinnPhong Green => new(
        new ObjColor(0f, .1f, 0), 
        ObjColor.Green, 
        ObjColor.White, 
        100f,
        .5f);
    
    public static BlinnPhong Blue => new(
        new ObjColor(0f, 0f, .1f), 
        ObjColor.Blue, 
        ObjColor.White, 
        100f,
        .5f);
    
    public static BlinnPhong Gray => new(
        new ObjColor(.1f, .1f, .1f), 
        new ObjColor(.6f, .6f, .6f), 
        ObjColor.White, 
        100f,
        .5f);
    
    public static BlinnPhong Purple => new(
        new ObjColor(.1f, 0f, .1f), 
        ObjColor.Purple, 
        ObjColor.White, 
        100f,
        .5f);
}

readonly struct Sphere
{
    public readonly Vector3 Center;
    public readonly float Radius;
    public readonly BlinnPhong Color;

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