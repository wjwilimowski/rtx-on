namespace RtxOn;

public class Scene
{
    public Scene(ViewCamera viewCamera, Sphere[] spheres, Light light)
    {
        ViewCamera = viewCamera;
        Spheres = spheres;
        Light = light;
    }

    public ViewCamera ViewCamera { get; }
    public Sphere[] Spheres { get; }
    public Light Light { get; }
}