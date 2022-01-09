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

    public Color Illuminate(
        BlinnPhong obj,
        Vector3 intersectionToLight,
        Vector3 objSurfaceNormal,
        Vector3 intersectionToCamera)
    {
        var lv = intersectionToLight + intersectionToCamera;
        var dotLvNormal = objSurfaceNormal.Dot(lv.Normalize());
        var rawSpecularCoefficient = (float)Math.Pow(dotLvNormal, obj.Shininess / 4f);
        var specularCoefficient = float.IsNaN(rawSpecularCoefficient) ? 0f : rawSpecularCoefficient;
        
        var color = obj.Ambient * Color.Ambient +
                    obj.Diffuse * Color.Diffuse * intersectionToLight.Dot(objSurfaceNormal) +
                    obj.Specular * Color.Specular * specularCoefficient;

        return color;
    }
}