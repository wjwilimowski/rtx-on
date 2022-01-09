namespace RtxOn.Materials;

public readonly struct BlinnPhong
{
    public readonly Color Ambient;
    public readonly Color Diffuse;
    public readonly Color Specular;
    public readonly float Shininess;
    public readonly float Reflection;

    public BlinnPhong(Color ambient, Color diffuse, Color specular, float shininess, float reflection)
    {
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
        Shininess = shininess;
        Reflection = reflection;
    }

    public static BlinnPhong Bright => new(
        Color.White, 
        Color.White, 
        Color.White, 
        100f,
        .5f);
    
    public static BlinnPhong Red => new(
        new Color(.1f, 0, 0), 
        Color.Red, 
        Color.White, 
        100f,
        .5f);
    
    public static BlinnPhong Green => new(
        new Color(0f, .1f, 0), 
        Color.Green, 
        Color.White, 
        100f,
        .3f);
    
    public static BlinnPhong Blue => new(
        new Color(0f, 0f, .1f), 
        Color.Blue, 
        Color.White, 
        100f,
        .5f);
    
    public static BlinnPhong Gray => new(
        new Color(.1f, .1f, .1f), 
        new Color(.6f, .6f, .6f), 
        Color.White, 
        100f,
        .9f);
    
    public static BlinnPhong DarkGrayish => new(
        new Color(.05f, .05f, .05f), 
        new Color(.4f, .3f, .3f), 
        new Color(1f, .8f, .8f), 
        100f,
        .9f);
    
    public static BlinnPhong Purple => new(
        new Color(.1f, 0f, .1f), 
        Color.Purple, 
        Color.White, 
        100f,
        .3f);
}