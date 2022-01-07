using RtxOn.Algebra;
using RtxOn.Materials;
using RtxOn.Viewport;

namespace RtxOn.Arrangement;

public class Scene
{
    public Scene(View view, Sphere[] spheres, Light light)
    {
        View = view;
        Spheres = spheres;
        Light = light;
    }

    public View View { get; }
    public Sphere[] Spheres { get; }
    public Light Light { get; }
}