namespace RtxOn.Algebra;

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
    
    public static float Delta(float b, float c, out float t1, out float t2)
    {
        t1 = default;
        t2 = default;
        
        var delta = b * b - 4 * c;
        if (delta > 0)
        {
            t1 = (-b + (float)Math.Sqrt(delta)) / 2;
            t2 = (-b - (float)Math.Sqrt(delta)) / 2;
        }

        return delta;
    }
}