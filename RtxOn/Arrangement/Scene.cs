using RtxOn.Algebra;
using RtxOn.Materials;
using RtxOn.Viewport;

namespace RtxOn.Arrangement;

public class Scene
{
    public Scene(Sphere[] spheres, Light light)
    {
        Spheres = spheres;
        Light = light;
    }

    public Sphere[] Spheres { get; }
    public Light Light { get; }
}