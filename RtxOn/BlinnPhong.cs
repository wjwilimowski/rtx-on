namespace RtxOn;

readonly struct BlinnPhong
{
    public readonly ObjColor Ambient;
    public readonly ObjColor Diffuse;
    public readonly ObjColor Specular;
    public readonly float Shininess;
    public readonly float Reflection;
    
    public static ObjColor Illuminate(
        BlinnPhong obj,
        BlinnPhong light,
        Vector3 intersectionToLight,
        Vector3 objSurfaceNormal,
        Vector3 intersectionToCamera)
    {
        var lv = intersectionToLight + intersectionToCamera;
        var dotLvNormal = objSurfaceNormal.Dot(lv.Normalize());
        var rawSpecularCoefficient = (float)Math.Pow(dotLvNormal, obj.Shininess / 4f);
        var specularCoefficient = float.IsNaN(rawSpecularCoefficient) ? 0f : rawSpecularCoefficient;
        
        var color = obj.Ambient * light.Ambient +
                    obj.Diffuse * light.Diffuse * intersectionToLight.Dot(objSurfaceNormal) +
                    obj.Specular * light.Specular * specularCoefficient;

        return color;
    }

    public BlinnPhong(ObjColor ambient, ObjColor diffuse, ObjColor specular, float shininess, float reflection)
    {
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
        Shininess = shininess;
        Reflection = reflection;
    }

    public static BlinnPhong Bright => new(
        ObjColor.White, 
        ObjColor.White, 
        ObjColor.White, 
        100f,
        .5f);
    
    public static BlinnPhong Red => new(
        new ObjColor(.1f, 0, 0), 
        ObjColor.Red, 
        ObjColor.White, 
        100f,
        .5f);
    
    public static BlinnPhong Green => new(
        new ObjColor(0f, .1f, 0), 
        ObjColor.Green, 
        ObjColor.White, 
        100f,
        .3f);
    
    public static BlinnPhong Blue => new(
        new ObjColor(0f, 0f, .1f), 
        ObjColor.Blue, 
        ObjColor.White, 
        100f,
        .5f);
    
    public static BlinnPhong Gray => new(
        new ObjColor(.1f, .1f, .1f), 
        new ObjColor(.6f, .6f, .6f), 
        ObjColor.White, 
        100f,
        .5f);
    
    public static BlinnPhong Purple => new(
        new ObjColor(.1f, 0f, .1f), 
        ObjColor.Purple, 
        ObjColor.White, 
        100f,
        .3f);
}