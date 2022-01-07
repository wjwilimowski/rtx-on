// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Drawing;
using RtxOn;



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

var center = (sphere1.Center + sphere2.Center + sphere3.Center) / 3f;

var initialScreenMid = center + new Vector3(0, 0, 2f);
var focalLength = 1f;
var initialCameraDirection = (center - initialScreenMid).Normalize();
var camera = new Camera(initialScreenMid, initialCameraDirection, focalLength);

var width = 640;
var height = 400;
var view = new View(camera, width, height);

var scene = new Scene(view, spheres, light);

const int nReflections = 6;

var renderer = new Renderer(nReflections, true);

using (var gif = AnimatedGif.AnimatedGif.Create("result.gif", 33))
{
    for (int i = 0; i < 30; i++)
    {
        view.Camera.SetPosition(initialScreenMid + initialCameraDirection.Negative() * i * 0.1f + new Vector3(0, i * 0.1f, 0));
        view.Camera.LookAt(center);
        
        var stw = new Stopwatch();
        stw.Start();
        var rendered = renderer.RenderParallel(scene).ToList();
        stw.Stop();
        
        var bitmap = new Bitmap(width, height);
        foreach (var renderedPixel in rendered)
        {
            bitmap.SetPixel(renderedPixel.X, renderedPixel.Y, Color.FromArgb((int)(255 * renderedPixel.Color.R), (int)(255 * renderedPixel.Color.G), (int)(255 * renderedPixel.Color.B)));
        }
        
        gif.AddFrame(bitmap);

        Console.WriteLine($"Done in {stw.Elapsed}");
    }
}