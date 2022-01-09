using RtxOn.Algebra;
using RtxOn.Materials;

namespace RtxOn.Arrangement;

public class Scene
{
    public Scene(IVisible[] visibles, Light light)
    {
        Visibles = visibles;
        Light = light;
    }

    public IVisible[] Visibles { get; }
    public Light Light { get; }
}