using RtxOn.Algebra;

namespace RtxOn.Materials;

public readonly struct Light
{
    public readonly Vector3 Position;
    public readonly BlinnPhong Color;

    public Light(Vector3 position, BlinnPhong color)
    {
        Position = position;
        Color = color;
    }
    
}