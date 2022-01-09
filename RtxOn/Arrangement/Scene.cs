using RtxOn.Algebra;
using RtxOn.Materials;

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