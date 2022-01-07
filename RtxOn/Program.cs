﻿// See https://aka.ms/new-console-template for more information

using System.Drawing;
using RtxOn;

var width = 640;
var height = 400;

var screenMid = new Vector3(0, 0, 0);
var focalLength = 1f;
var cameraDirection = new Vector3(0, 0, -1).Normalize();
var viewCamera = new ViewCamera(screenMid, cameraDirection, focalLength, width, height);

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

var scene = new Scene(viewCamera, spheres, light);

const int nReflections = 6;

var bitmap = new Bitmap(width, height);

foreach (var renderedPixel in Rendering.RenderScene(scene, nReflections))
{
    bitmap.SetPixel(renderedPixel.X, renderedPixel.Y, Color.FromArgb((int)(255 * renderedPixel.Color.R), (int)(255 * renderedPixel.Color.G), (int)(255 * renderedPixel.Color.B)));
}

bitmap.Save("output.png");