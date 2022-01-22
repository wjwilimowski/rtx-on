using RtxOn.Algebra;
using RtxOn.Materials;

namespace RtxOn.Arrangement;

public class Scene
{
    public Scene(IVisible[] visibles, Light[] lights)
    {
        Visibles = visibles;
        Lights = lights;
    }

    public IVisible[] Visibles { get; }
    public Light[] Lights { get; }
}