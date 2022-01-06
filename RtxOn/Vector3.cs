readonly struct Vector3
{
    public readonly float X;
    public readonly float Y;
    public readonly float Z;

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3 Normalize()
    {
        return Div(Norm());
    }

    public float Norm()
    {
        return (float) Math.Sqrt(X * X + Y * Y + Z * Z);
    }

    public Vector3 Div(float div)
    {
        return new Vector3(X / div, Y / div, Z / div);
    }

    public Vector3 Mul(float mul)
    {
        return new Vector3(X * mul, Y * mul, Z * mul);
    }

    public Vector3 Minus(Vector3 other)
    {
        return new Vector3(X - other.X, Y - other.Y, Z - other.Z);
    }

    public Vector3 Plus(Vector3 other)
    {
        return new Vector3(X + other.X, Y + other.Y, Z + other.Z);
    }

    public float Dot(Vector3 other)
    {
        return X * other.X + Y * other.Y + Z * other.Z;
    }
}