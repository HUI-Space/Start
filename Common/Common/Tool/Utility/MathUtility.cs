using System;

namespace Start
{
    /// <summary>
    /// 数学计算工具类
    /// </summary>
    public static class MathUtility
    {
        /// <summary>
        /// 计算两个整数的最大公约数 (GCD)
        /// 使用欧几里得算法
        /// </summary>
        /// <param name="a">第一个整数</param>
        /// <param name="b">第二个整数</param>
        /// <returns>最大公约数</returns>
        public static int GreatestCommonDivisor(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);

            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }

            return a;
        }

        /// <summary>
        /// 计算两个整数的最小公倍数 (LCM)
        /// </summary>
        /// <param name="a">第一个整数</param>
        /// <param name="b">第二个整数</param>
        /// <returns>最小公倍数</returns>
        public static int LeastCommonMultiple(int a, int b)
        {
            if (a == 0 || b == 0)
            {
                return 0;
            }

            int gcd = GreatestCommonDivisor(a, b);
            return Math.Abs(a / gcd * b);
        }
    }
}