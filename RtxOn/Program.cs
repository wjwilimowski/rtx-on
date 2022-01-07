// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Drawing;
using RtxOn;
using RtxOn.Algebra;
using RtxOn.Arrangement;
using RtxOn.Materials;
using RtxOn.Rendering;
using RtxOn.Viewport;


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

var width = 1280;
var height = 800;
var view = new View(camera, width, height);

var scene = new Scene(view, spheres, light);

const int nReflections = 6;

var renderer = new Renderer(nReflections, true);

using (var gif = AnimatedGif.AnimatedGif.Create("result.gif", 33))
{
    const int maxFrame = 220;
    string pad = new string('0', maxFrame.ToString().Length);
    var totalTime = TimeSpan.Zero;
    var stw = new Stopwatch();
    stw.Start();
    for (int i = 0; i < maxFrame; i++)
    {
        var offset = initialCameraDirection.Negative() * i * 0.06f + new Vector3(0, i * 0.02f, 0);
        view.Camera.SetPosition(initialScreenMid + offset.RotateAround(new Vector3(0, 1, 0), (float)i / 220));
        view.Camera.LookAt(center);
        var rendered = renderer.RenderParallel(scene).ToList();
        
        var bitmap = new Bitmap(width, height);
        foreach (var renderedPixel in rendered)
        {
            bitmap.SetPixel(renderedPixel.X, renderedPixel.Y, Color.FromArgb((int)(255 * renderedPixel.Color.R), (int)(255 * renderedPixel.Color.G), (int)(255 * renderedPixel.Color.B)));
        }
        
        gif.AddFrame(bitmap);
        var totalElapsed = stw.Elapsed;
        var frameElapsed = totalElapsed - totalTime;
        totalTime = totalElapsed;

        var ip = i + 1;
        var eta = (totalTime / ip) * (220 - i);
        Console.WriteLine($"Frame {ip.ToString(pad)}/{maxFrame}\tin {frameElapsed}\t{totalTime} total\t{eta} ETA");
    }
}