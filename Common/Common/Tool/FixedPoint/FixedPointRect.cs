using System;

namespace Start
{
    /// <summary>
    /// 定点数矩形
    /// </summary>
    [Serializable]
    public struct FixedPointRect : IEquatable<FixedPointRect>
    {
        public FixedPointNumber X;
        public FixedPointNumber Y;
        public FixedPointNumber Width;
        public FixedPointNumber Height;

        public FixedPointRect(FixedPointNumber x, FixedPointNumber y, FixedPointNumber width, FixedPointNumber height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public FixedPointNumber Left => X;
        public FixedPointNumber Right => X + Width;
        public FixedPointNumber Top => Y;
        public FixedPointNumber Bottom => Y + Height;

        public FixedPointVector2 Position => new FixedPointVector2(X, Y);
        public FixedPointVector2 Size => new FixedPointVector2(Width, Height);
        

        public bool Contains(FixedPointVector2 point)
        {
            return point.X >= X && point.X <= Right && point.Y >= Y && point.Y <= Bottom;
        }

        public override bool Equals(object obj)
        {
            return obj is FixedPointRect rect && Equals(rect);
        }

        public bool Equals(FixedPointRect other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && 
                   Width.Equals(other.Width) && Height.Equals(other.Height);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Width.GetHashCode();
                hashCode = (hashCode * 397) ^ Height.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
        }
    }
}