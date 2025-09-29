using System;

namespace Start
{
    /// <summary>
    /// 三维定点数向量
    /// </summary>
    [Serializable]
    public struct FixedPointVector3 : IEquatable<FixedPointVector3>
    {
        public FixedPointNumber X;
        public FixedPointNumber Y;
        public FixedPointNumber Z;

        public FixedPointVector3(FixedPointNumber x, FixedPointNumber y, FixedPointNumber z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public FixedPointVector3(int x, int y, int z)
        {
            X = new FixedPointNumber(x);
            Y = new FixedPointNumber(y);
            Z = new FixedPointNumber(z);
        }

        public static FixedPointVector3 Zero => new FixedPointVector3(FixedPointNumber.Zero, FixedPointNumber.Zero, FixedPointNumber.Zero);
        public static FixedPointVector3 One => new FixedPointVector3(FixedPointNumber.One, FixedPointNumber.One, FixedPointNumber.One);
        public static FixedPointVector3 Up => new FixedPointVector3(FixedPointNumber.Zero, FixedPointNumber.One, FixedPointNumber.Zero);
        public static FixedPointVector3 Right => new FixedPointVector3(FixedPointNumber.One, FixedPointNumber.Zero, FixedPointNumber.Zero);
        public static FixedPointVector3 Forward => new FixedPointVector3(FixedPointNumber.Zero, FixedPointNumber.Zero, FixedPointNumber.One);

        public static FixedPointVector3 operator +(FixedPointVector3 a, FixedPointVector3 b)
        {
            return new FixedPointVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static FixedPointVector3 operator -(FixedPointVector3 a, FixedPointVector3 b)
        {
            return new FixedPointVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static FixedPointVector3 operator *(FixedPointVector3 a, FixedPointNumber b)
        {
            return new FixedPointVector3(a.X * b, a.Y * b, a.Z * b);
        }

        public static FixedPointVector3 operator /(FixedPointVector3 a, FixedPointNumber b)
        {
            return new FixedPointVector3(a.X / b, a.Y / b, a.Z / b);
        }

        public static bool operator ==(FixedPointVector3 a, FixedPointVector3 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(FixedPointVector3 a, FixedPointVector3 b)
        {
            return !(a == b);
        }

        public FixedPointNumber Magnitude => FixedPointNumber.CreateFromDouble(
            Math.Sqrt((X.ToDouble() * X.ToDouble()) + (Y.ToDouble() * Y.ToDouble()) + (Z.ToDouble() * Z.ToDouble())));

        public FixedPointVector3 Normalized
        {
            get
            {
                var mag = Magnitude;
                if (mag == FixedPointNumber.Zero)
                    return Zero;
                return this / mag;
            }
        }

        public static FixedPointNumber Dot(FixedPointVector3 a, FixedPointVector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static FixedPointVector3 Cross(FixedPointVector3 a, FixedPointVector3 b)
        {
            return new FixedPointVector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );
        }

        public override bool Equals(object obj)
        {
            return obj is FixedPointVector3 vector && Equals(vector);
        }

        public bool Equals(FixedPointVector3 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}