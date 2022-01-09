using RtxOn.Algebra;
using RtxOn.Arrangement;
using RtxOn.Materials;
using RtxOn.Viewport;

namespace ExampleScenes;

public static class Scenes
{
    public static (Scene scene, Camera camera) Example1()
    {
        var light = new Light(new Vector3(5, 5, 5), BlinnPhong.Bright);

        var floorPlane = new Plane(new Vector3(0, -.7f, 0), new Vector3(0, 1, 0), BlinnPhong.Gray);

        var sphere1 = new Sphere(-.2f, 0, -1, .7f, BlinnPhong.Red);
        var sphere2 = new Sphere(.1f, -.3f, 0, .1f, BlinnPhong.Purple);
        var sphere3 = new Sphere(-.3f, 0, 0, .15f, BlinnPhong.Green);

        var visibles = new IVisible[]
        {
            floorPlane,
            sphere1,
            sphere2,
            sphere3
        };

        var center = (sphere1.Center + sphere2.Center + sphere3.Center) / 3f;

        var initialScreenMid = center + new Vector3(0, 0, 2f);
        var focalLength = 1f;
        var initialCameraDirection = (center - initialScreenMid).Normalize();
        var camera = new Camera(initialScreenMid, initialCameraDirection, focalLength);

        var scene = new Scene(visibles, light);

        return (scene, camera);
    }
}