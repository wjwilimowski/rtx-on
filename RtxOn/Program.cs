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
var viewCamera = new ViewCamera(camera, left, right, top, bottom, width, height);

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

const int nReflections = 6;

var bitmap = new Bitmap(width, height);

foreach ((Vector3 pixelDirection, int ix, int iy) in viewCamera.Pixels())
{
    var direction = pixelDirection;
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
            camera.Minus(intersectionPoint).Normalize()) * reflection;

        reflection *= obj.Color.Reflection;
        origin = intersectionPoint;
        direction = direction.Reflected(surfaceNormal);
    }

    illumination = illumination.Clamp();
                
    bitmap.SetPixel(ix, iy, Color.FromArgb((int)(255 * illumination.R), (int)(255 * illumination.G), (int)(255 * illumination.B)));
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

readonly struct ViewCamera
{
    public readonly Vector3 Focal;/*
    public readonly Vector3 ScreenMid;
    public readonly Vector3 Direction;
    public readonly float FocalLength;*/
    public readonly float Left;
    public readonly float Right;
    public readonly float Top;
    public readonly float Bottom;
    public readonly int WidthPx;
    public readonly int HeightPx;

    public ViewCamera(Vector3 focal, float left, float right, float top, float bottom, int widthPx, int heightPx)
    {
        Focal = focal;
        Left = left;
        Right = right;
        Top = top;
        Bottom = bottom;
        WidthPx = widthPx;
        HeightPx = heightPx;
    }

    public IEnumerable<(Vector3 direction, int ix, int iy)> Pixels()
    {
        foreach (var (y, iy) in Utils.FloatRange(Top, Bottom, HeightPx).Reverse().Enumerate())
        {
            foreach (var (x, ix) in Utils.FloatRange(Left, Right, WidthPx).Enumerate())
            {
                var pixel = new Vector3(x, y, 0f);
                var direction = pixel.Minus(Focal).Normalize();

                yield return (direction, ix, iy);
            }
        }
    }
}