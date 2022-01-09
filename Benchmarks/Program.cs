// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using ExampleScenes;
using RtxOn.Arrangement;
using RtxOn.Rendering;
using RtxOn.Viewport;

var summary = BenchmarkRunner.Run<RenderVsRenderParallel>();

public class RenderVsRenderParallel
{
    private const int NReflections = 6;
    private readonly Renderer _renderer;

    private readonly Scene _scene;
    private readonly Camera _camera;

    private readonly Consumer _consumer = new Consumer();
    
    [ParamsSource(nameof(ViewParamsSource))]
    public View View { get; set; }

    public IEnumerable<View> ViewParamsSource()
    {
        yield return new View(_camera, 320, 200);
        yield return new View(_camera, 640, 400);
        yield return new View(_camera, 1280, 800);
    }

    public RenderVsRenderParallel()
    {
        (_scene, _camera) = Scenes.Example1();

        _renderer = new Renderer(NReflections, true);
    }

    [Benchmark(Baseline = true)]
    public void Render() => _renderer.Render(_scene, View).Consume(_consumer);

    [Benchmark]
    public void RenderParallel() => _renderer.RenderParallel(_scene, View).Consume(_consumer);

}