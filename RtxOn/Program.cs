// See https://aka.ms/new-console-template for more information

using System.Drawing;
using RtxOn;

var width = 640;
var height = 400;

var screenMid = new Vector3(0, 0, 0);
var focalLength = 1f;
var cameraDirection = new Vector3(0, 0, -1).Normalize();
var viewCamera = new ViewCamera(screenMid, cameraDirection, focalLength, width, height);
var camera = viewCamera.Focal;

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
    public readonly Vector3 Focal;
    public readonly Vector3 ScreenMid;
    public readonly Vector3 TowardsScreenSide;
    public readonly Vector3 TowardsScreenEdge;
    public readonly int WidthPx;
    public readonly int HeightPx;
    public readonly float Ratio;

    public ViewCamera(Vector3 screenMid, Vector3 direction, float focalLength, int widthPx, int heightPx)
    {
        Ratio = (float)widthPx / heightPx;
        
        var dirUp = new Vector3(0, 1, 0);

        TowardsScreenSide = Vector3.NormalToSurfaceFromPoint(direction, dirUp).Normalize();
        TowardsScreenEdge = Vector3.NormalToSurfaceFromPoint(TowardsScreenSide, direction).Normalize();

        Focal = screenMid + direction * -focalLength;
        ScreenMid = screenMid;
        
        WidthPx = widthPx;
        HeightPx = heightPx;
    }

    public IEnumerable<(Vector3 direction, int ix, int iy)> Pixels()
    {
        foreach (var (y, iy) in Utils.FloatRange(-1 / Ratio, 1 / Ratio, HeightPx).Enumerate())
        {
            foreach (var (x, ix) in Utils.FloatRange(-1, 1, WidthPx).Enumerate())
            {
                var pixel = ScreenMid + TowardsScreenEdge * y + TowardsScreenSide * x;
                var direction = pixel.Minus(Focal).Normalize();

                yield return (direction, ix, iy);
            }
        }
    }
}