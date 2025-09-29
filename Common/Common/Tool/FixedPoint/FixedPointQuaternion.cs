using System;

namespace Start
{


    /// <summary>
    /// 定点数四元数
    /// </summary>
    [Serializable]
    public struct FixedPointQuaternion : IEquatable<FixedPointQuaternion>
    {
        public FixedPointNumber X;
        public FixedPointNumber Y;
        public FixedPointNumber Z;
        public FixedPointNumber W;

        public FixedPointQuaternion(FixedPointNumber x, FixedPointNumber y, FixedPointNumber z, FixedPointNumber w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static FixedPointQuaternion Identity => new FixedPointQuaternion(
            FixedPointNumber.Zero, FixedPointNumber.Zero, FixedPointNumber.Zero, FixedPointNumber.One);

        public FixedPointNumber Magnitude => FixedPointNumber.CreateFromDouble(
            Math.Sqrt(
                (X.ToDouble() * X.ToDouble()) + 
                (Y.ToDouble() * Y.ToDouble()) + 
                (Z.ToDouble() * Z.ToDouble()) + 
                (W.ToDouble() * W.ToDouble())
            )
        );

        public FixedPointQuaternion Normalized
        {
            get
            {
                var mag = Magnitude;
                if (mag == FixedPointNumber.Zero)
                    return Identity;
                return new FixedPointQuaternion(X / mag, Y / mag, Z / mag, W / mag);
            }
        }

        public static FixedPointQuaternion operator *(FixedPointQuaternion a, FixedPointQuaternion b)
        {
            return new FixedPointQuaternion(
                a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
                a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
                a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W,
                a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z
            );
        }

        public static FixedPointVector3 operator *(FixedPointQuaternion rotation, FixedPointVector3 point)
        {
            var x = point.X;
            var y = point.Y;
            var z = point.Z;
            var qx = rotation.X;
            var qy = rotation.Y;
            var qz = rotation.Z;
            var qw = rotation.W;

            var ix = qw * x + qy * z - qz * y;
            var iy = qw * y + qz * x - qx * z;
            var iz = qw * z + qx * y - qy * x;
            var iw = -qx * x - qy * y - qz * z;

            return new FixedPointVector3(
                ix * qw + iw * -qx + iy * -qz - iz * -qy,
                iy * qw + iw * -qy + iz * -qx - ix * -qz,
                iz * qw + iw * -qz + ix * -qy - iy * -qx
            );
        }

        public static bool operator ==(FixedPointQuaternion a, FixedPointQuaternion b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        }

        public static bool operator !=(FixedPointQuaternion a, FixedPointQuaternion b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is FixedPointQuaternion quaternion && Equals(quaternion);
        }

        public bool Equals(FixedPointQuaternion other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && 
                   Z.Equals(other.Z) && W.Equals(other.W);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }
    }
}