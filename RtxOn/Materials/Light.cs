using RtxOn.Algebra;
using RtxOn.Arrangement;

namespace RtxOn.Materials;

public class PointLight : Light
{
    public PointLight(Vector3 position, BlinnPhong color, float intensity) : base(position, color, intensity)
    {
    }
    
    public override float GetIntensityAt(Vector3 intersectionPoint, Scene scene)
    {
        return IsShadowed(Position, intersectionPoint, scene) ? 0f : Intensity;
    }

    public override Color Illuminate(BlinnPhong obj, Vector3 point, Vector3 objSurfaceNormal, Vector3 intersectionToCamera, Scene _)
    {
        var pointToLight = Position - point;
        var lv = pointToLight + intersectionToCamera;
        var dotLvNormal = objSurfaceNormal.Dot(lv.Normalize());
        var rawSpecularCoefficient = (float)Math.Pow(dotLvNormal, obj.Shininess / 4f);
        var specularCoefficient = float.IsNaN(rawSpecularCoefficient) ? 0f : rawSpecularCoefficient;
        
        var color = obj.Ambient * Color.Ambient +
                    obj.Diffuse * Color.Diffuse * pointToLight.Dot(objSurfaceNormal) * Intensity +
                    obj.Specular * Color.Specular * specularCoefficient * Intensity;

        return color;
    }
}

public class AreaLight : Light
{
    public readonly Vector3 U;
    public readonly Vector3 V;
    public readonly int USteps;
    public readonly int VSteps;
    public readonly int NCells;
    
    public AreaLight(Vector3 position, BlinnPhong color, float intensity, Vector3 u, Vector3 v, int uSteps, int vSteps) : base(position - u - v, color, intensity)
    {
        U = u * 2 / uSteps;
        V = v * 2 / vSteps;
        USteps = uSteps;
        VSteps = vSteps;
        NCells = USteps * VSteps;
    }

    public override Color Illuminate(BlinnPhong obj, Vector3 point, Vector3 objSurfaceNormal, Vector3 intersectionToCamera, Scene scene)
    {
        var totalDiffuse = 0f;
        var totalSpecular = 0f;

        foreach (var lightPoint in EnumeratePoints())
        {
            var intersectionToLight = lightPoint - point;
            var lv = intersectionToLight + intersectionToCamera;
            var dotLvNormal = objSurfaceNormal.Dot(lv.Normalize());
            var rawSpecularCoefficient = (float)Math.Pow(dotLvNormal, obj.Shininess / 4f);

            var intensity = GetIntensityAt(point, scene);
            totalDiffuse += intensity * intersectionToLight.Dot(objSurfaceNormal);
            totalSpecular += intensity * (float.IsNaN(rawSpecularCoefficient) ? 0f : rawSpecularCoefficient);
        }

        var color = obj.Ambient * Color.Ambient +
                    obj.Diffuse * Color.Diffuse * (totalDiffuse / NCells) +
                    obj.Specular * Color.Specular * (totalSpecular / NCells);

        return color;
    }

    public override float GetIntensityAt(Vector3 intersectionPoint, Scene scene)
    {
        return EnumeratePoints().Select(p => IsShadowed(p, intersectionPoint, scene) ? 0f : Intensity).Average();
    }

    private Vector3 MiddleOfCell(int u, int v)
    {
        return Position + U * (u + 0.5f) + V * (v + 0.5f);
    }

    private IEnumerable<Vector3> EnumeratePoints()
    {
        for (int u = 0; u < USteps; u++)
        for (int v = 0; v < VSteps; v++)
        {
            yield return MiddleOfCell(u, v);
        }
    }
}

public abstract class Light
{
    public readonly Vector3 Position;
    public readonly BlinnPhong Color;
    public readonly float Intensity;

    protected Light(Vector3 position, BlinnPhong color, float intensity)
    {
        Position = position;
        Color = color;
        Intensity = intensity;
    }

    public abstract Color Illuminate(BlinnPhong obj, Vector3 point, Vector3 objSurfaceNormal, Vector3 intersectionToCamera, Scene scene);

    public abstract float GetIntensityAt(Vector3 intersectionPoint, Scene scene);

    public static bool IsShadowed(Vector3 lightPoint, Vector3 intersectionPoint, Scene scene)
    {
        var rayFromIntersectionToLight = lightPoint.Minus(intersectionPoint);
        var intersectionToLight = rayFromIntersectionToLight.Normalize(out float distanceFromIntersectionToLight);
        
        return scene.Visibles.TryFindNearest(intersectionPoint, intersectionToLight, out _, out float d) &&
               d < distanceFromIntersectionToLight;
    }
}