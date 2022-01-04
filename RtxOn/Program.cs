// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

var camera = new Point3 { X = 1.0f };
var screenTopLeft = new Point3 { Y = -1.0f, Z = -1.0f };
var screenBottomRight = new Point3 { Y = 1.0f, Z = 1.0f };

var screenDimensionW = 320;
var screenDimensionH = 200;

var image = Enumerable.Range(0, screenDimensionH).Select(_ => new Color[screenDimensionW]).ToArray();

foreach (var (w, i) in Utils.FloatRange(screenTopLeft.Y, screenBottomRight.Y, screenDimensionW).Enumerate())
{
    foreach (var (h, j) in Utils.FloatRange(screenTopLeft.Z, screenBottomRight.Z, screenDimensionH).Enumerate())
    {
        image[j][i] = Color.FromArgb((byte) (255 * ((float)i / screenDimensionW)), 0, (byte) (255 * ((float)j / screenDimensionH)));
    }
}

var bitmap = new Bitmap(screenDimensionW, screenDimensionH);
foreach (var (row, rowIndex) in image.Enumerate())
{
    foreach (var (pixel, colIndex) in row.Enumerate())
    {
        bitmap.SetPixel(colIndex, rowIndex, pixel);
    }
}
bitmap.Save("output.png");

static class Utils
{
    public static IEnumerable<float> FloatRange(float min, float max, int count)
    {
        var step = (min - max) / count;
        return Enumerable.Range(0, count).Select(i => i * step);
    }
    
    public static IEnumerable<(T item, int index)> Enumerate<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }
    
}

struct Point3
{
    public float X;
    public float Y;
    public float Z;
}

struct Pixel
{
    public int R;
    public int G;
    public int B;
}