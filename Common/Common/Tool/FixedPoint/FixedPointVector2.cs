using System;

namespace Start
{
    /// <summary>
    /// 二维定点数向量
    /// </summary>
    [Serializable]
    public struct FixedPointVector2 : IEquatable<FixedPointVector2>
    {
        public FixedPointNumber X;
        public FixedPointNumber Y;

        public FixedPointVector2(FixedPointNumber x, FixedPointNumber y)
        {
            X = x;
            Y = y;
        }
        
        public FixedPointNumber SqrMagnitude => this.X * this.X + this.Y * this.Y;

        public FixedPointNumber Magnitude => FixedPointMath.Sqrt(SqrMagnitude);

        public FixedPointVector2 Normalized
        {
            get
            {
                var mag = Magnitude;
                if (mag == FixedPointNumber.Zero)
                    return Zero;
                return this / mag;
            }
        }

        #region Public Static Methods 
        
        public static FixedPointVector2 Zero => new FixedPointVector2(FixedPointNumber.Zero, FixedPointNumber.Zero);
        public static FixedPointVector2 One => new FixedPointVector2(FixedPointNumber.One, FixedPointNumber.One);
        public static FixedPointVector2 Up => new FixedPointVector2(FixedPointNumber.Zero, FixedPointNumber.One);
        public static FixedPointVector2 Right => new FixedPointVector2(FixedPointNumber.One, FixedPointNumber.Zero);

        /// <summary>
        /// 反射
        /// </summary>
        /// <param name="inDirection"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static FixedPointVector2 Reflect(FixedPointVector2 inDirection, FixedPointVector2 normal)
        {
            FixedPointNumber dot = Dot(inDirection, normal);
            FixedPointNumber x = inDirection.X - 2*dot*normal.X;
            FixedPointNumber y = inDirection.Y - 2*dot*normal.Y;
            return new FixedPointVector2(x, y);
        }
        
        /// <summary>
        /// 垂直向量
        /// </summary>
        /// <param name="inDirection"></param>
        /// <returns></returns>
        public static FixedPointVector2 Perpendicular(FixedPointVector2 inDirection)
        {
            return new FixedPointVector2(-inDirection.Y, inDirection.X);
        }
        
        /// <summary>
        /// 点积
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FixedPointNumber Dot(FixedPointVector2 a, FixedPointVector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
        
        #endregion
        
        #region override operator
        public static FixedPointVector2 operator +(FixedPointVector2 a, FixedPointVector2 b)
        {
            return new FixedPointVector2(a.X + b.X, a.Y + b.Y);
        }

        public static FixedPointVector2 operator -(FixedPointVector2 a, FixedPointVector2 b)
        {
            return new FixedPointVector2(a.X - b.X, a.Y - b.Y);
        }

        public static FixedPointVector2 operator *(FixedPointVector2 a, FixedPointVector2 b)
        {
            return new FixedPointVector2(a.X * b.X, a.Y * b.Y);
        }

        public static FixedPointVector2 operator *(FixedPointVector2 a, FixedPointNumber b)
        {
            return new FixedPointVector2(a.X * b, a.Y * b);
        }

        public static FixedPointVector2 operator /(FixedPointVector2 a, FixedPointNumber b)
        {
            return new FixedPointVector2(a.X / b, a.Y / b);
        }

        public static bool operator ==(FixedPointVector2 a, FixedPointVector2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(FixedPointVector2 a, FixedPointVector2 b)
        {
            return !(a == b);
        }
        
        public override bool Equals(object obj)
        {
            return obj is FixedPointVector2 vector && Equals(vector);
        }

        public bool Equals(FixedPointVector2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() << 2;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
        
        #endregion
    }
}