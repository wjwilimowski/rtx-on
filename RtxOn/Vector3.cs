namespace RtxOn;

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

    public Vector3 Negative()
    {
        return new Vector3(-X, -Y, -Z);
    }

    public Vector3 Reflected(Vector3 axis)
    {
        return this - (axis * Dot(axis) * 2f);
    }

    public Vector3 Normalize()
    {
        return this / Norm();
    }

    public float Norm()
    {
        return (float) Math.Sqrt(X * X + Y * Y + Z * Z);
    }

    public static Vector3 operator /(Vector3 v, float div)
    {
        return new Vector3(v.X / div, v.Y / div, v.Z / div);
    }

    public static Vector3 operator *(Vector3 v, float mul)
    {
        return new Vector3(v.X * mul, v.Y * mul, v.Z * mul);
    }

    public Vector3 Minus(Vector3 other)
    {
        return new Vector3(X - other.X, Y - other.Y, Z - other.Z);
    }

    public static Vector3 operator -(Vector3 a, Vector3 b)
    {
        return a.Minus(b);
    }

    public static Vector3 operator +(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public float Dot(Vector3 other)
    {
        return X * other.X + Y * other.Y + Z * other.Z;
    }
}