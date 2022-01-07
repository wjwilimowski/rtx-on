namespace RtxOn;

public readonly struct Vector3
{
    public readonly float X;
    public readonly float Y;
    public readonly float Z;

    public override string ToString()
    {
        return $"{X}, {Y}, {Z}";
    }

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    public Vector3 RotateAround(Vector3 axis, float theta)
    {
        var parallel = axis * Dot(axis) / axis.Dot(axis);
        var perpendicular = this - parallel;

        var w = axis.CrossProduct(perpendicular);

        return parallel +
               perpendicular * (float)Math.Cos(theta * 2 * Math.PI) +
               w.Normalize() * perpendicular.Norm() * (float)Math.Sin(theta * 2 * Math.PI);
    }

    public Vector3 Negative()
    {
        return new Vector3(-X, -Y, -Z);
    }

    public static Vector3 NormalToSurfaceFromPoint(Vector3 spanningA, Vector3 spanningB)
    {
        var v31 = spanningA.Negative();
        var v12 = spanningB;

        return v31.CrossProduct(v12);
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

    public static Vector3 operator *(Vector3 a, Vector3 y)
    {
        return new Vector3(a.X * y.X, a.Y * y.Y, a.Z * y.Z);
    }

    public Vector3 CrossProduct(Vector3 other)
    {
        return new Vector3(
            Y * other.Z - Z * other.Y,
            Z * other.X - X * other.Z,
            X * other.Y - Y * other.X
        );
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