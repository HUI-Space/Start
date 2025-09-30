using System;

/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
 *
 *  本软件按"原样"提供，不附带任何明示或暗示的担保。
 *  作者不对因使用本软件而产生的任何损害承担责任。
 *
 *  允许任何人将本软件用于任何目的，包括商业应用，
 *  并可自由修改和重新分发，但需遵守以下限制：
 *
 *  1. 不得歪曲软件的来源；不得声称自己编写了原始软件。
 *     如果在产品中使用本软件，在产品文档中致谢会被认可但非强制。
 *  2. 修改后的源版本必须明确标记为修改版，不得歪曲为原始软件。
 *  3. 本声明不得从任何源分发中删除或修改。
 */

namespace TrueSync
{
    /// <summary>
    /// 包含常用的数学运算方法和常量，提供高精度定点数(FP)的数学操作支持
    /// </summary>
    public sealed class TSMath
    {
        /// <summary>
        /// 圆周率常量(π)，值等同于FP类型的Pi常量
        /// </summary>
        public static FP Pi = FP.Pi;

        /**
        *  @brief 二分之一圆周率常量(π/2)
        **/
        public static FP PiOver2 = FP.PiOver2;

        /// <summary>
        /// 极小值常量，常用于判断数值结果是否为零的阈值
        /// </summary>
        public static FP Epsilon = FP.Epsilon;

        /**
        *  @brief 角度转弧度的转换常量
        **/
        public static FP Deg2Rad = FP.Deg2Rad;

        /**
        *  @brief 弧度转角度的转换常量
        **/
        public static FP Rad2Deg = FP.Rad2Deg;


        /**
         * @brief FP类型的无穷大值，使用FP的最大值表示
         * */
        public static FP Infinity = FP.MaxValue;

        /// <summary>
        /// 计算指定数的平方根
        /// </summary>
        /// <param name="number">要计算平方根的数</param>
        /// <returns>返回number的平方根</returns>

        #region public static FP Sqrt(FP number)

        public static FP Sqrt(FP number)
        {
            return FP.Sqrt(number);
        }

        #endregion

        /// <summary>
        /// 获取两个值中的最大值
        /// </summary>
        /// <param name="val1">第一个比较值</param>
        /// <param name="val2">第二个比较值</param>
        /// <returns>返回两个值中较大的那个</returns>

        #region public static FP Max(FP val1, FP val2)

        public static FP Max(FP val1, FP val2)
        {
            return (val1 > val2) ? val1 : val2;
        }

        #endregion

        /// <summary>
        /// 获取两个值中的最小值
        /// </summary>
        /// <param name="val1">第一个比较值</param>
        /// <param name="val2">第二个比较值</param>
        /// <returns>返回两个值中较小的那个</returns>

        #region public static FP Min(FP val1, FP val2)

        public static FP Min(FP val1, FP val2)
        {
            return (val1 < val2) ? val1 : val2;
        }

        #endregion

        /// <summary>
        /// 获取三个值中的最大值
        /// </summary>
        /// <param name="val1">第一个比较值</param>
        /// <param name="val2">第二个比较值</param>
        /// <param name="val3">第三个比较值</param>
        /// <returns>返回三个值中最大的那个</returns>

        #region public static FP Max(FP val1, FP val2,FP val3)

        public static FP Max(FP val1, FP val2, FP val3)
        {
            FP max12 = (val1 > val2) ? val1 : val2; // 先比较前两个值，获取较大值
            return (max12 > val3) ? max12 : val3; // 再与第三个值比较，返回最大值
        }

        #endregion

        /// <summary>
        /// 将值限制在[min, max]范围内
        /// </summary>
        /// <param name="value">要限制的值</param>
        /// <param name="min">最小值边界</param>
        /// <param name="max">最大值边界</param>
        /// <returns>如果value小于min则返回min；如果大于max则返回max；否则返回value本身</returns>

        #region public static FP Clamp(FP value, FP min, FP max)

        public static FP Clamp(FP value, FP min, FP max)
        {
            if (value < min)
            {
                value = min;
                return value;
            }

            if (value > max)
            {
                value = max;
            }

            return value;
        }

        #endregion

        /// <summary>
        /// 将值限制在[0, 1]范围内（FP.Zero到FP.One）
        /// </summary>
        /// <param name="value">要限制的值</param>
        /// <returns>如果value小于0则返回0；如果大于1则返回1；否则返回value本身</returns>
        public static FP Clamp01(FP value)
        {
            if (value < FP.Zero)
                return FP.Zero;

            if (value > FP.One)
                return FP.One;

            return value;
        }

        /// <summary>
        /// 计算矩阵的绝对值矩阵（将矩阵中每个元素的符号改为正）
        /// </summary>
        /// <param name="matrix">输入矩阵</param>
        /// <param name="result">输出的绝对值矩阵</param>

        #region public static void Absolute(ref TSMatrix matrix,out TSMatrix result)

        public static void Absolute(ref TSMatrix matrix, out TSMatrix result)
        {
            result.M11 = FP.Abs(matrix.M11);
            result.M12 = FP.Abs(matrix.M12);
            result.M13 = FP.Abs(matrix.M13);
            result.M21 = FP.Abs(matrix.M21);
            result.M22 = FP.Abs(matrix.M22);
            result.M23 = FP.Abs(matrix.M23);
            result.M31 = FP.Abs(matrix.M31);
            result.M32 = FP.Abs(matrix.M32);
            result.M33 = FP.Abs(matrix.M33);
        }

        #endregion

        /// <summary>
        /// 计算指定角度的正弦值（弧度制）
        /// </summary>
        /// <param name="value">角度（弧度）</param>
        /// <returns>返回value的正弦值</returns>
        public static FP Sin(FP value)
        {
            return FP.Sin(value);
        }

        /// <summary>
        /// 计算指定角度的余弦值（弧度制）
        /// </summary>
        /// <param name="value">角度（弧度）</param>
        /// <returns>返回value的余弦值</returns>
        public static FP Cos(FP value)
        {
            return FP.Cos(value);
        }

        /// <summary>
        /// 计算指定角度的正切值（弧度制）
        /// </summary>
        /// <param name="value">角度（弧度）</param>
        /// <returns>返回value的正切值</returns>
        public static FP Tan(FP value)
        {
            return FP.Tan(value);
        }

        /// <summary>
        /// 计算指定值的反正弦值（返回弧度制角度）
        /// </summary>
        /// <param name="value">正弦值（范围[-1,1]）</param>
        /// <returns>返回对应的角度（弧度）</returns>
        public static FP Asin(FP value)
        {
            return FP.Asin(value);
        }

        /// <summary>
        /// 计算指定值的反余弦值（返回弧度制角度）
        /// </summary>
        /// <param name="value">余弦值（范围[-1,1]）</param>
        /// <returns>返回对应的角度（弧度）</returns>
        public static FP Acos(FP value)
        {
            return FP.Acos(value);
        }

        /// <summary>
        /// 计算指定值的反正切值（返回弧度制角度）
        /// </summary>
        /// <param name="value">正切值</param>
        /// <returns>返回对应的角度（弧度）</returns>
        public static FP Atan(FP value)
        {
            return FP.Atan(value);
        }

        /// <summary>
        /// 计算坐标(x,y)的反正切值（返回弧度制角度），可确定所在象限
        /// </summary>
        /// <param name="y">y坐标值</param>
        /// <param name="x">x坐标值</param>
        /// <returns>返回对应的角度（弧度）</returns>
        public static FP Atan2(FP y, FP x)
        {
            return FP.Atan2(y, x);
        }

        /// <summary>
        /// 计算小于或等于指定数的最大整数
        /// </summary>
        /// <param name="value">输入值</param>
        /// <returns>返回不大于value的最大整数</returns>
        public static FP Floor(FP value)
        {
            return FP.Floor(value);
        }

        /// <summary>
        /// 计算大于或等于指定数的最小整数
        /// </summary>
        /// <param name="value">输入值</param>
        /// <returns>返回不小于value的最小整数</returns>
        public static FP Ceiling(FP value)
        {
            return value; // 注意：此处原实现可能存在问题，通常需要更复杂的计算
        }

        /// <summary>
        /// 将值四舍五入到最接近的整数，如果恰好在两个整数中间，返回偶数
        /// </summary>
        /// <param name="value">输入值</param>
        /// <returns>四舍五入后的整数</returns>
        public static FP Round(FP value)
        {
            return FP.Round(value);
        }

        /// <summary>
        /// 返回指定Fix64数的符号
        /// 正数返回1，零返回0，负数返回-1
        /// </summary>
        /// <param name="value">输入的Fix64数</param>
        /// <returns>表示符号的整数</returns>
        public static int Sign(FP value)
        {
            return FP.Sign(value);
        }

        /// <summary>
        /// 返回指定Fix64数的绝对值
        /// 注意：Abs(Fix64.MinValue)等于Fix64.MaxValue
        /// </summary>
        /// <param name="value">输入的Fix64数</param>
        /// <returns>value的绝对值</returns>
        public static FP Abs(FP value)
        {
            return FP.Abs(value);
        }

        /// <summary>
        /// 计算重心坐标插值
        /// </summary>
        /// <param name="value1">第一个值</param>
        /// <param name="value2">第二个值</param>
        /// <param name="value3">第三个值</param>
        /// <param name="amount1">第一个插值系数</param>
        /// <param name="amount2">第二个插值系数</param>
        /// <returns>插值结果</returns>
        public static FP Barycentric(FP value1, FP value2, FP value3, FP amount1, FP amount2)
        {
            return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
        }

        /// <summary>
        /// 使用Catmull-Rom算法进行插值计算（用于平滑曲线生成）
        /// </summary>
        /// <param name="value1">第一个控制点</param>
        /// <param name="value2">第二个控制点</param>
        /// <param name="value3">第三个控制点</param>
        /// <param name="value4">第四个控制点</param>
        /// <param name="amount">插值参数（0到1之间）</param>
        /// <returns>插值结果</returns>
        public static FP CatmullRom(FP value1, FP value2, FP value3, FP value4, FP amount)
        {
            // 使用来自http://www.mvps.org/directx/articles/catmull/的公式
            // 内部使用FP类型以避免精度损失
            FP amountSquared = amount * amount;
            FP amountCubed = amountSquared * amount;
            return (FP)(0.5 * (2.0 * value2 +
                               (value3 - value1) * amount +
                               (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * amountSquared +
                               (3.0 * value2 - value1 - 3.0 * value3 + value4) * amountCubed));
        }

        /// <summary>
        /// 计算两个数之间的距离（绝对值差）
        /// </summary>
        /// <param name="value1">第一个数</param>
        /// <param name="value2">第二个数</param>
        /// <returns>两个数的距离</returns>
        public static FP Distance(FP value1, FP value2)
        {
            return FP.Abs(value1 - value2);
        }

        /// <summary>
        /// 使用Hermite插值算法进行插值（考虑切线方向）
        /// </summary>
        /// <param name="value1">起始值</param>
        /// <param name="tangent1">起始点切线</param>
        /// <param name="value2">结束值</param>
        /// <param name="tangent2">结束点切线</param>
        /// <param name="amount">插值参数（0到1之间）</param>
        /// <returns>插值结果</returns>
        public static FP Hermite(FP value1, FP tangent1, FP value2, FP tangent2, FP amount)
        {
            // 全部转换为FP类型以避免精度损失
            // 否则，当amount参数值较大时，结果可能为NaN而非无穷大
            FP v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
            FP sCubed = s * s * s;
            FP sSquared = s * s;

            if (amount == 0f)
                result = value1;
            else if (amount == 1f)
                result = value2;
            else
                result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                         (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                         t1 * s +
                         v1;
            return (FP)result;
        }

        /// <summary>
        /// 线性插值（Lerp）
        /// </summary>
        /// <param name="value1">起始值</param>
        /// <param name="value2">结束值</param>
        /// <param name="amount">插值系数（0到1之间，超出范围会被钳制）</param>
        /// <returns>插值结果：value1 + (value2 - value1) * amount</returns>
        public static FP Lerp(FP value1, FP value2, FP amount)
        {
            return value1 + (value2 - value1) * Clamp01(amount);
        }

        /// <summary>
        /// 反线性插值，计算值在两个值之间的比例
        /// </summary>
        /// <param name="value1">起始值</param>
        /// <param name="value2">结束值</param>
        /// <param name="amount">要计算比例的值</param>
        /// <returns>返回amount在value1到value2之间的比例（0到1之间）</returns>
        public static FP InverseLerp(FP value1, FP value2, FP amount)
        {
            if (value1 != value2)
                return Clamp01((amount - value1) / (value2 - value1));
            return FP.Zero;
        }

        /// <summary>
        /// 平滑插值，在起始值和结束值之间创建平滑的过渡（缓入缓出效果）
        /// </summary>
        /// <param name="value1">起始值</param>
        /// <param name="value2">结束值</param>
        /// <param name="amount">插值参数（0到1之间）</param>
        /// <returns>平滑插值结果</returns>
        public static FP SmoothStep(FP value1, FP value2, FP amount)
        {
            // 预期amount在0到1之间
            // 如果amount < 0，返回value1
            // 如果amount > 1，返回value2
            FP result = Clamp(amount, 0f, 1f);
            result = Hermite(value1, 0f, value2, 0f, result);
            return result;
        }


        /// <summary>
        /// 计算2的指定次幂
        /// 提供至少6位小数的精度
        /// </summary>
        internal static FP Pow2(FP x)
        {
            if (x.RawValue == 0)
            {
                return FP.One; // 2^0 = 1
            }

            // 通过利用exp(-x) = 1/exp(x)避免处理负参数
            bool neg = x.RawValue < 0;
            if (neg)
            {
                x = -x;
            }

            if (x == FP.One)
            {
                return neg ? FP.One / (FP)2 : (FP)2; // 2^1=2，2^-1=0.5
            }

            if (x >= FP.Log2Max)
            {
                return neg ? FP.One / FP.MaxValue : FP.MaxValue; // 超出范围返回极值
            }

            if (x <= FP.Log2Min)
            {
                return neg ? FP.MaxValue : FP.Zero; // 超出范围返回极值
            }

            /* 算法基于指数函数的幂级数展开：
             * http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
             *
             * 从第n项计算第n+1项时，使用x/n乘以第n项
             * 当项值降为零时，停止求和
             */

            int integerPart = (int)Floor(x); // 提取指数的整数部分
            // 提取指数的小数部分
            x = FP.FromRaw(x.RawValue & 0x00000000FFFFFFFF);

            var result = FP.One;
            var term = FP.One;
            int i = 1;
            while (term.RawValue != 0)
            {
                term = FP.FastMul(FP.FastMul(x, term), FP.Ln2) / (FP)i;
                result += term;
                i++;
            }

            result = FP.FromRaw(result.RawValue << integerPart); // 处理整数部分的幂次
            if (neg)
            {
                result = FP.One / result; // 处理负指数
            }

            return result;
        }

        /// <summary>
        /// 计算指定数的以2为底的对数
        /// 提供至少9位小数的精度
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">参数为非正值时抛出</exception>
        internal static FP Log2(FP x)
        {
            if (x.RawValue <= 0)
            {
                throw new ArgumentOutOfRangeException("Non-positive value passed to Ln", "x");
            }

            // 此实现基于Clay. S. Turner的快速二进制对数算法
            // (C. S. Turner,  "A Fast Binary Logarithm Algorithm", IEEE Signal
            //     Processing Mag., pp. 124,140, Sep. 2010.)

            long b = 1U << (FP.FRACTIONAL_PLACES - 1);
            long y = 0;

            long rawX = x.RawValue;
            while (rawX < FP.ONE)
            {
                rawX <<= 1;
                y -= FP.ONE;
            }

            while (rawX >= (FP.ONE << 1))
            {
                rawX >>= 1;
                y += FP.ONE;
            }

            var z = FP.FromRaw(rawX);

            for (int i = 0; i < FP.FRACTIONAL_PLACES; i++)
            {
                z = FP.FastMul(z, z);
                if (z.RawValue >= (FP.ONE << 1))
                {
                    z = FP.FromRaw(z.RawValue >> 1);
                    y += b;
                }

                b >>= 1;
            }

            return FP.FromRaw(y);
        }

        /// <summary>
        /// 计算指定数的自然对数（以e为底）
        /// 提供至少7位小数的精度
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">参数为非正值时抛出</exception>
        public static FP Ln(FP x)
        {
            return FP.FastMul(Log2(x), FP.Ln2); // 利用换底公式：ln(x) = log2(x) * ln(2)
        }

        /// <summary>
        /// 计算指定数的指定次幂（b^exp）
        /// 结果提供约5位数字的精度
        /// </summary>
        /// <exception cref="DivideByZeroException">底数为零且指数为负时抛出</exception>
        /// <exception cref="ArgumentOutOfRangeException">底数为负且指数非零时抛出</exception>
        public static FP Pow(FP b, FP exp)
        {
            if (b == FP.One)
            {
                return FP.One; // 1的任何次幂都是1
            }

            if (exp.RawValue == 0)
            {
                return FP.One; // 任何数的0次幂都是1
            }

            if (b.RawValue == 0)
            {
                if (exp.RawValue < 0)
                {
                    //throw new DivideByZeroException();
                    return FP.MaxValue; // 0的负次幂视为无穷大
                }

                return FP.Zero; // 0的正次幂是0
            }

            FP log2 = Log2(b); // 计算底数的log2
            return Pow2(exp * log2); // 利用公式：b^exp = 2^(exp * log2(b))
        }

        /// <summary>
        /// 使当前值向目标值移动，每次移动不超过最大步长
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="target">目标值</param>
        /// <param name="maxDelta">最大移动步长</param>
        /// <returns>移动后的新值</returns>
        public static FP MoveTowards(FP current, FP target, FP maxDelta)
        {
            if (Abs(target - current) <= maxDelta)
                return target; // 如果距离小于等于最大步长，直接到达目标
            return (current + (Sign(target - current)) * maxDelta); // 否则移动最大步长
        }

        /// <summary>
        /// 计算值在指定长度范围内的重复值（类似取模运算，但结果始终为正）
        /// </summary>
        /// <param name="t">输入值</param>
        /// <param name="length">范围长度</param>
        /// <returns>在[0, length)范围内的重复值</returns>
        public static FP Repeat(FP t, FP length)
        {
            return (t - (Floor(t / length) * length));
        }

        /// <summary>
        /// 计算两个角度之间的最小差值（考虑角度的周期性）
        /// </summary>
        /// <param name="current">当前角度（度）</param>
        /// <param name="target">目标角度（度）</param>
        /// <returns>最小角度差（范围[-180, 180]）</returns>
        public static FP DeltaAngle(FP current, FP target)
        {
            FP num = Repeat(target - current, (FP)360f); // 计算差值的周期值
            if (num > (FP)180f)
            {
                num -= (FP)360f; // 确保差值在[-180, 180]范围内
            }

            return num;
        }

        /// <summary>
        /// 使当前角度向目标角度移动，每次移动不超过最大步长（考虑角度周期性）
        /// </summary>
        /// <param name="current">当前角度（度）</param>
        /// <param name="target">目标角度（度）</param>
        /// <param name="maxDelta">最大移动步长（度）</param>
        /// <returns>移动后的新角度</returns>
        public static FP MoveTowardsAngle(FP current, FP target, float maxDelta)
        {
            target = current + DeltaAngle(current, target); // 计算目标角度的等效值
            return MoveTowards(current, target, maxDelta); // 使用线性移动方法
        }

        /// <summary>
        /// 平滑阻尼函数，使当前值逐渐接近目标值（带速度的平滑过渡）
        /// 使用默认的deltaTime（FP.EN2）
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="target">目标值</param>
        /// <param name="currentVelocity">当前速度（引用参数，会被修改）</param>
        /// <param name="smoothTime">平滑时间（值越小过渡越快）</param>
        /// <param name="maxSpeed">最大速度限制</param>
        /// <returns>平滑过渡后的值</returns>
        public static FP SmoothDamp(FP current, FP target, ref FP currentVelocity, FP smoothTime, FP maxSpeed)
        {
            FP deltaTime = FP.EN2;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        /// <summary>
        /// 平滑阻尼函数，使当前值逐渐接近目标值（带速度的平滑过渡）
        /// 使用默认的deltaTime（FP.EN2）和maxSpeed（-FP.MaxValue表示无限制）
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="target">目标值</param>
        /// <param name="currentVelocity">当前速度（引用参数，会被修改）</param>
        /// <param name="smoothTime">平滑时间（值越小过渡越快）</param>
        /// <returns>平滑过渡后的值</returns>
        public static FP SmoothDamp(FP current, FP target, ref FP currentVelocity, FP smoothTime)
        {
            FP deltaTime = FP.EN2;
            FP positiveInfinity = -FP.MaxValue;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
        }

        /// <summary>
        /// 平滑阻尼函数的核心实现，使当前值逐渐接近目标值（带速度的平滑过渡）
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="target">目标值</param>
        /// <param name="currentVelocity">当前速度（引用参数，会被修改）</param>
        /// <param name="smoothTime">平滑时间（值越小过渡越快）</param>
        /// <param name="maxSpeed">最大速度限制</param>
        /// <param name="deltaTime">时间增量（帧间隔时间）</param>
        /// <returns>平滑过渡后的值</returns>
        public static FP SmoothDamp(FP current, FP target, ref FP currentVelocity, FP smoothTime, FP maxSpeed,
            FP deltaTime)
        {
            smoothTime = Max(FP.EN4, smoothTime); // 确保平滑时间不为零
            FP num = (FP)2f / smoothTime; // 计算响应系数
            FP num2 = num * deltaTime;
            FP num3 = FP.One /
                      (((FP.One + num2) + (((FP)0.48f * num2) * num2)) + ((((FP)0.235f * num2) * num2) * num2));
            FP num4 = current - target; // 计算当前值与目标值的差值
            FP num5 = target;
            FP max = maxSpeed * smoothTime; // 计算最大允许差值
            num4 = Clamp(num4, -max, max); // 限制差值范围
            target = current - num4; // 调整目标值
            FP num7 = (currentVelocity + (num * num4)) * deltaTime; // 计算速度增量
            currentVelocity = (currentVelocity - (num * num7)) * num3; // 更新速度
            FP num8 = target + ((num4 + num7) * num3); // 计算新的当前值

            // 如果已经超过目标值，则直接设置为目标值
            if (((num5 - current) > FP.Zero) == (num8 > num5))
            {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime; // 重置速度
            }

            return num8;
        }
    }
}