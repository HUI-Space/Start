using System;

namespace Start
{
    public static class MathUtility
    {
        /// <summary>
        /// 计算两个整数的最大公约数 (GCD)。
        /// </summary>
        /// <param name="a">第一个整数。</param>
        /// <param name="b">第二个整数。</param>
        /// <returns>两个整数的最大公约数。</returns>
        public static int MaximalCommonDivisor(int a, int b)
        {
            // 确保 a 和 b 都是非负数
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
        /// 最小公约数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int MinimumCommonDivisor(int a, int b)
        {
            int temp, r;
            if (a < b)
            {
                temp = a;
                a = b;
                b = temp;
            }

            while (b != 0)
            {
                r = a % b;
                a = b;
                b = r;
            }

            return a;
        }
    }
}