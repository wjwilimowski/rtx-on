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
}