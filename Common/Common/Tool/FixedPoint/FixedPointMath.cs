using System;

namespace Start
{
    public static class FixedPointMath
    {

        /// <summary>
        /// 开平方根
        /// </summary>
        /// <returns></returns>
        public static FixedPointNumber Sqrt(FixedPointNumber number)
        {
            if (number.Raw < 0)
                throw new ArgumentOutOfRangeException("number", "Value must be non-negative.");
            if (number.Raw == 0)
                return FixedPointNumber.Zero;
            return new FixedPointNumber((int)(SqrtULong((ulong)number.Raw << (FixedPointNumber.FRACTIONAL_BITS + 2)) + 1) >> 1);
        }
        
        internal static uint SqrtULong(ulong N)
        {
            ulong x = 1L << ((31 + (FixedPointNumber.FRACTIONAL_BITS + 2) + 1) / 2);
            while (true)
            {
                ulong y = (x + N / x) >> 1;
                if (y >= x)
                    return (uint)x;
                x = y;
            }
        }
        
        /// <summary>
        /// 限制
        /// </summary>
        /// <param name="a"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static FixedPointNumber Clamp(FixedPointNumber a, FixedPointNumber min, FixedPointNumber max)
        {
            if (a < min)
            {
                return min;
            }
            if (a > max)
            {
                return max;
            }
            return a;
        }
        
        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static FixedPointNumber Lerp(FixedPointNumber a, FixedPointNumber b, FixedPointNumber c)
        {
            return (b - a) * c + a;
        }
        
        /// <summary>
        /// 反向线性插值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static FixedPointNumber InverseLerp(FixedPointNumber a, FixedPointNumber b, FixedPointNumber c)
        {
            if (a == b)
                return FixedPointNumber.Zero;
            return Clamp((c - a) / (b - a), FixedPointNumber.Zero, FixedPointNumber.One);
        }

        /// <summary>
        /// 最小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FixedPointNumber Min(FixedPointNumber a, FixedPointNumber b)
        {
            return a < b ? a : b;
        }

        /// <summary>
        /// 最大
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FixedPointNumber Max(FixedPointNumber a, FixedPointNumber b)
        {
            return a < b ? b : a;
        }
    }
}