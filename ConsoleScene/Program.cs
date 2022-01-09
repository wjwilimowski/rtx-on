// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Drawing;
using RtxOn.Algebra;
using RtxOn.Rendering;
using RtxOn.Viewport;

var (scene, camera) = ExampleScenes.Scenes.Example1();
var initialCameraDirection = camera.Direction.Clone();
var initialScreenMid = camera.ScreenMid.Clone();
var center = (scene.Spheres[0].Center + scene.Spheres[1].Center + scene.Spheres[2].Center) / 3f;
        
const int width = 320;
const int height = 200;
var view = new View(camera, width, height);

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
        camera.SetPosition(initialScreenMid + offset.RotateAround(new Vector3(0, 1, 0), (float)i / 220));
        camera.LookAt(center);
        var rendered = renderer.RenderParallel(scene, view).ToList();
        
        var bitmap = new Bitmap(view.WidthPx, view.HeightPx);
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