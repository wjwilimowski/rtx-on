// See https://aka.ms/new-console-template for more information

using RtxOn.Rendering;

Console.WriteLine("Hello, World!");

public class RenderSceneBenchmark
{
    private const int NReflections = 6;
    private readonly Renderer _renderer = new Renderer(NReflections, true);
    
    
}