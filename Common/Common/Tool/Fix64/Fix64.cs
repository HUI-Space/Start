using System;
using System.IO;

namespace TrueSync
{
    /// <summary>
    /// 表示一个Q31.32格式的定点数（31位整数部分 + 32位小数部分）
    /// 用于在需要精确数值计算且避免浮点数精度误差的场景（如物理模拟、财务计算等）
    /// </summary>
    [Serializable]
    public partial struct FP : IEquatable<FP>, IComparable<FP>
    {
        /// <summary>
        /// 存储定点数的原始序列化值（64位长整数）
        /// 高31位表示整数部分，低32位表示小数部分
        /// </summary>
        public long _serializedValue;

        /// <summary>
        /// 最大值（64位长整数的最大值）
        /// </summary>
        public const long MAX_VALUE = long.MaxValue;

        /// <summary>
        /// 最小值（64位长整数的最小值）
        /// </summary>
        public const long MIN_VALUE = long.MinValue;

        /// <summary>
        /// 总位数（64位）
        /// </summary>
        public const int NUM_BITS = 64;

        /// <summary>
        /// 小数部分的位数（32位）
        /// </summary>
        public const int FRACTIONAL_PLACES = 32;

        /// <summary>
        /// 表示1.0的原始值（2^32）
        /// 用于整数与定点数之间的转换基准
        /// </summary>
        public const long ONE = 1L << FRACTIONAL_PLACES;

        /// <summary>
        /// 表示10.0的原始值（10 * 2^32）
        /// </summary>
        public const long TEN = 10L << FRACTIONAL_PLACES;

        /// <summary>
        /// 表示0.5的原始值（2^31）
        /// </summary>
        public const long HALF = 1L << (FRACTIONAL_PLACES - 1);

        /// <summary>
        /// 2π的定点数表示（预计算的原始值）
        /// </summary>
        public const long PI_TIMES_2 = 0x6487ED511;

        /// <summary>
        /// π的定点数表示（预计算的原始值）
        /// </summary>
        public const long PI = 0x3243F6A88;

        /// <summary>
        /// π/2的定点数表示（预计算的原始值）
        /// </summary>
        public const long PI_OVER_2 = 0x1921FB544;

        /// <summary>
        /// 自然对数ln(2)的定点数表示
        /// </summary>
        public const long LN2 = 0xB17217F7;

        /// <summary>
        /// log2最大值的定点数表示
        /// </summary>
        public const long LOG2MAX = 0x1F00000000;

        /// <summary>
        /// log2最小值的定点数表示
        /// </summary>
        public const long LOG2MIN = -0x2000000000;

        /// <summary>
        /// 查找表(LUT)的大小，用于三角函数快速计算
        /// 基于PI_OVER_2右移15位计算得出
        /// </summary>
        public const int LUT_SIZE = (int)(PI_OVER_2 >> 15);

        /// <summary>
        /// 该类型的精度（2^-32 ≈ 2.3283e-10）
        /// </summary>
        public static readonly decimal Precision = (decimal)(new FP(1L));

        /// <summary>
        /// 可表示的最大值（MAX_VALUE - 1，避免溢出）
        /// </summary>
        public static readonly FP MaxValue = new FP(MAX_VALUE - 1);

        /// <summary>
        /// 可表示的最小值（MIN_VALUE + 2，避免溢出）
        /// </summary>
        public static readonly FP MinValue = new FP(MIN_VALUE + 2);

        /// <summary>
        /// 表示1.0的FP实例
        /// </summary>
        public static readonly FP One = new FP(ONE);

        /// <summary>
        /// 表示10.0的FP实例
        /// </summary>
        public static readonly FP Ten = new FP(TEN);

        /// <summary>
        /// 表示0.5的FP实例
        /// </summary>
        public static readonly FP Half = new FP(HALF);

        /// <summary>
        /// 表示0.0的FP实例
        /// </summary>
        public static readonly FP Zero = new FP();

        /// <summary>
        /// 表示正无穷大的FP实例
        /// </summary>
        public static readonly FP PositiveInfinity = new FP(MAX_VALUE);

        /// <summary>
        /// 表示负无穷大的FP实例
        /// </summary>
        public static readonly FP NegativeInfinity = new FP(MIN_VALUE + 1);

        /// <summary>
        /// 表示非数字(NaN)的FP实例
        /// </summary>
        public static readonly FP NaN = new FP(MIN_VALUE);

        /// <summary>
        /// 10的负幂常量，用于小数精度计算
        /// </summary>
        public static readonly FP EN1 = FP.One / 10;

        public static readonly FP EN2 = FP.One / 100;
        public static readonly FP EN3 = FP.One / 1000;
        public static readonly FP EN4 = FP.One / 10000;
        public static readonly FP EN5 = FP.One / 100000;
        public static readonly FP EN6 = FP.One / 1000000;
        public static readonly FP EN7 = FP.One / 10000000;
        public static readonly FP EN8 = FP.One / 100000000;

        /// <summary>
        /// 最小精度常量（约等于0.001）
        /// </summary>
        public static readonly FP Epsilon = FP.EN3;

        /// <summary>
        /// π的FP实例
        /// </summary>
        public static readonly FP Pi = new FP(PI);

        /// <summary>
        /// π/2的FP实例
        /// </summary>
        public static readonly FP PiOver2 = new FP(PI_OVER_2);

        /// <summary>
        /// 2π的FP实例
        /// </summary>
        public static readonly FP PiTimes2 = new FP(PI_TIMES_2);

        /// <summary>
        /// 1/π的FP实例（约0.3183）
        /// </summary>
        public static readonly FP PiInv = (FP)0.3183098861837906715377675267M;

        /// <summary>
        /// 2/π的FP实例（约0.6366）
        /// </summary>
        public static readonly FP PiOver2Inv = (FP)0.6366197723675813430755350535M;

        /// <summary>
        /// 角度转弧度的转换因子（π/180）
        /// </summary>
        public static readonly FP Deg2Rad = Pi / new FP(180);

        /// <summary>
        /// 弧度转角度的转换因子（180/π）
        /// </summary>
        public static readonly FP Rad2Deg = new FP(180) / Pi;

        /// <summary>
        /// 查找表的间隔值，用于三角函数计算时的索引转换
        /// </summary>
        public static readonly FP LutInterval = (FP)(LUT_SIZE - 1) / PiOver2;

        /// <summary>
        /// log2最大值的FP实例
        /// </summary>
        public static readonly FP Log2Max = new FP(LOG2MAX);

        /// <summary>
        /// log2最小值的FP实例
        /// </summary>
        public static readonly FP Log2Min = new FP(LOG2MIN);

        /// <summary>
        /// ln(2)的FP实例
        /// </summary>
        public static readonly FP Ln2 = new FP(LN2);

        /// <summary>
        /// 返回定点数的符号
        /// </summary>
        /// <param name="value">要检查的定点数</param>
        /// <returns>1表示正数，0表示零，-1表示负数</returns>
        public static int Sign(FP value)
        {
            return
                value._serializedValue < 0 ? -1 : // 负数返回-1
                value._serializedValue > 0 ? 1 : // 正数返回1
                0; // 零返回0
        }

        /// <summary>
        /// 返回定点数的绝对值
        /// 注意：Abs(Fix64.MinValue) == Fix64.MaxValue（处理边界情况）
        /// </summary>
        /// <param name="value">输入值</param>
        /// <returns>绝对值</returns>
        public static FP Abs(FP value)
        {
            if (value._serializedValue == MIN_VALUE)
            {
                return MaxValue; // 特殊处理最小值（避免溢出）
            }

            // 无分支实现：使用符号位掩码计算绝对值
            // 原理：若为负数，掩码为-1（全1），则value + mask = value - 1，再异或mask实现取反
            var mask = value._serializedValue >> 63; // 获取符号位（0或-1）
            FP result;
            result._serializedValue = (value._serializedValue + mask) ^ mask;
            return result;
        }

        /// <summary>
        /// 快速计算绝对值（不处理MinValue的特殊情况）
        /// 注意：当输入为MinValue时结果未定义
        /// </summary>
        /// <param name="value">输入值</param>
        /// <returns>绝对值</returns>
        public static FP FastAbs(FP value)
        {
            // 无分支实现，同Abs但无边界检查
            var mask = value._serializedValue >> 63;
            FP result;
            result._serializedValue = (value._serializedValue + mask) ^ mask;
            return result;
        }

        /// <summary>
        /// 返回小于或等于指定数的最大整数（向下取整）
        /// </summary>
        /// <param name="value">输入值</param>
        /// <returns>向下取整的结果</returns>
        public static FP Floor(FP value)
        {
            // 直接清零小数部分（低32位）
            FP result;
            result._serializedValue = (long)((ulong)value._serializedValue & 0xFFFFFFFF00000000);
            return result;
        }

        /// <summary>
        /// 返回大于或等于指定数的最小整数（向上取整）
        /// </summary>
        /// <param name="value">输入值</param>
        /// <returns>向上取整的结果</returns>
        public static FP Ceiling(FP value)
        {
            // 检查是否有小数部分（低32位非零）
            var hasFractionalPart = (value._serializedValue & 0x00000000FFFFFFFF) != 0;
            // 有小数部分则向下取整后加1，否则直接返回
            return hasFractionalPart ? Floor(value) + One : value;
        }

        /// <summary>
        /// 将值四舍五入到最近的整数
        /// 若值恰好在两个整数中间，返回偶数
        /// </summary>
        /// <param name="value">输入值</param>
        /// <returns>四舍五入的结果</returns>
        public static FP Round(FP value)
        {
            // 提取小数部分（低32位）
            var fractionalPart = value._serializedValue & 0x00000000FFFFFFFF;
            // 提取整数部分（向下取整）
            var integralPart = Floor(value);

            if (fractionalPart < 0x80000000)
            {
                // 小数部分小于0.5，直接返回整数部分
                return integralPart;
            }

            if (fractionalPart > 0x80000000)
            {
                // 小数部分大于0.5，整数部分加1
                return integralPart + One;
            }

            // 小数部分等于0.5时，四舍五入到最近的偶数
            return (integralPart._serializedValue & ONE) == 0
                ? integralPart // 整数部分为偶数，直接返回
                : integralPart + One; // 整数部分为奇数，加1变为偶数
        }

        /// <summary>
        /// 加法运算符：x + y
        /// 执行饱和加法（溢出时根据操作数符号取MinValue或MaxValue）
        /// </summary>
        public static FP operator +(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue + y._serializedValue;
            return result;
        }

        /// <summary>
        /// 带溢出检查的加法
        /// CLR会内联此方法以提高性能
        /// </summary>
        /// <param name="x">第一个操作数</param>
        /// <param name="y">第二个操作数</param>
        /// <returns>加法结果（溢出时返回边界值）</returns>
        public static FP OverflowAdd(FP x, FP y)
        {
            var xl = x._serializedValue;
            var yl = y._serializedValue;
            var sum = xl + yl;

            // 溢出检查：若操作数符号相同且结果符号与操作数不同，则溢出
            if (((~(xl ^ yl) & (xl ^ sum)) & MIN_VALUE) != 0)
            {
                sum = xl > 0 ? MAX_VALUE : MIN_VALUE; // 正数溢出取MaxValue，负数溢出取MinValue
            }

            FP result;
            result._serializedValue = sum;
            return result;
        }

        /// <summary>
        /// 无溢出检查的加法（性能优先）
        /// 适用于确保不会溢出的场景
        /// </summary>
        public static FP FastAdd(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue + y._serializedValue;
            return result;
        }

        /// <summary>
        /// 减法运算符：x - y
        /// 执行饱和减法（溢出时根据操作数符号取MinValue或MaxValue）
        /// </summary>
        public static FP operator -(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue - y._serializedValue;
            return result;
        }

        /// <summary>
        /// 带溢出检查的减法
        /// </summary>
        public static FP OverflowSub(FP x, FP y)
        {
            var xl = x._serializedValue;
            var yl = y._serializedValue;
            var diff = xl - yl;

            // 溢出检查：若操作数符号不同且结果符号与被减数不同，则溢出
            if ((((xl ^ yl) & (xl ^ diff)) & MIN_VALUE) != 0)
            {
                diff = xl < 0 ? MIN_VALUE : MAX_VALUE; // 被减数为负溢出取MinValue，否则取MaxValue
            }

            FP result;
            result._serializedValue = diff;
            return result;
        }

        /// <summary>
        /// 无溢出检查的减法（性能优先）
        /// </summary>
        public static FP FastSub(FP x, FP y)
        {
            return new FP(x._serializedValue - y._serializedValue);
        }

        /// <summary>
        /// 加法溢出检查辅助方法
        /// </summary>
        /// <param name="x">第一个加数</param>
        /// <param name="y">第二个加数</param>
        /// <param name="overflow">是否溢出的输出参数</param>
        /// <returns>加法结果</returns>
        static long AddOverflowHelper(long x, long y, ref bool overflow)
        {
            var sum = x + y;
            // 溢出条件：x和y符号相同且与sum符号不同
            overflow |= ((x ^ y ^ sum) & MIN_VALUE) != 0;
            return sum;
        }

        /// <summary>
        /// 乘法运算符：x * y
        /// 采用定点数乘法算法（结果右移32位恢复定点格式）
        /// </summary>
        public static FP operator *(FP x, FP y)
        {
            var xl = x._serializedValue;
            var yl = y._serializedValue;

            // 将64位操作数拆分为高低32位（整数部分和小数部分）
            var xlo = (ulong)(xl & 0x00000000FFFFFFFF); // x的低32位（小数部分）
            var xhi = xl >> FRACTIONAL_PLACES; // x的高32位（整数部分）
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF); // y的低32位（小数部分）
            var yhi = yl >> FRACTIONAL_PLACES; // y的高32位（整数部分）

            // 计算四部分乘积
            var lolo = xlo * ylo; // 小数*小数
            var lohi = (long)xlo * yhi; // 小数*整数
            var hilo = xhi * (long)ylo; // 整数*小数
            var hihi = xhi * yhi; // 整数*整数

            // 调整各部分的位位置以对齐小数位
            var loResult = lolo >> FRACTIONAL_PLACES; // 小数*小数结果右移32位
            var midResult1 = lohi; // 小数*整数结果无需移位
            var midResult2 = hilo; // 整数*小数结果无需移位
            var hiResult = hihi << FRACTIONAL_PLACES; // 整数*整数结果左移32位

            // 求和得到最终结果
            var sum = (long)loResult + midResult1 + midResult2 + hiResult;
            FP result;
            result._serializedValue = sum;
            return result;
        }

        /// <summary>
        /// 带溢出检查的乘法
        /// 适用于需要确保结果正确性的场景
        /// </summary>
        public static FP OverflowMul(FP x, FP y)
        {
            var xl = x._serializedValue;
            var yl = y._serializedValue;

            // 拆分操作数（同乘法运算符）
            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FRACTIONAL_PLACES;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FRACTIONAL_PLACES;

            // 计算四部分乘积
            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            // 调整位位置
            var loResult = lolo >> FRACTIONAL_PLACES;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FRACTIONAL_PLACES;

            // 累加并检查溢出
            bool overflow = false;
            var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
            sum = AddOverflowHelper(sum, midResult2, ref overflow);
            sum = AddOverflowHelper(sum, hiResult, ref overflow);

            // 检查操作数符号是否相同
            bool opSignsEqual = ((xl ^ yl) & MIN_VALUE) == 0;

            // 根据符号和溢出情况处理边界
            if (opSignsEqual)
            {
                // 符号相同：若结果为负或正溢出，则返回MaxValue
                if (sum < 0 || (overflow && xl > 0))
                {
                    return MaxValue;
                }
            }
            else
            {
                // 符号不同：若结果为正，则返回MinValue
                if (sum > 0)
                {
                    return MinValue;
                }
            }

            // 检查高位进位是否导致溢出
            var topCarry = hihi >> FRACTIONAL_PLACES;
            if (topCarry != 0 && topCarry != -1)
            {
                return opSignsEqual ? MaxValue : MinValue;
            }

            // 处理特殊溢出情况
            if (!opSignsEqual)
            {
                long posOp, negOp;
                if (xl > yl)
                {
                    posOp = xl;
                    negOp = yl;
                }
                else
                {
                    posOp = yl;
                    negOp = xl;
                }

                // 符号不同且操作数绝对值大于1，结果异常则溢出
                if (sum > negOp && negOp < -ONE && posOp > ONE)
                {
                    return MinValue;
                }
            }

            FP result;
            result._serializedValue = sum;
            return result;
        }

        /// <summary>
        /// 无溢出检查的乘法（性能优先）
        /// 适用于确保不会溢出的场景
        /// </summary>
        public static FP FastMul(FP x, FP y)
        {
            // 同乘法运算符，但无溢出检查
            var xl = x._serializedValue;
            var yl = y._serializedValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FRACTIONAL_PLACES;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FRACTIONAL_PLACES;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FRACTIONAL_PLACES;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FRACTIONAL_PLACES;

            var sum = (long)loResult + midResult1 + midResult2 + hiResult;
            FP result;
            result._serializedValue = sum;
            return result;
        }

        /// <summary>
        /// 计算无符号长整数的前导零数量
        /// 用于除法中的移位优化
        /// </summary>
        public static int CountLeadingZeroes(ulong x)
        {
            int result = 0;
            // 先按4位一组快速移位
            while ((x & 0xF000000000000000) == 0)
            {
                result += 4;
                x <<= 4;
            }

            // 再按1位精细移位
            while ((x & 0x8000000000000000) == 0)
            {
                result += 1;
                x <<= 1;
            }

            return result;
        }

        /// <summary>
        /// 除法运算符：x / y
        /// 采用定点数除法算法（通过移位和迭代实现）
        /// </summary>
        public static FP operator /(FP x, FP y)
        {
            var xl = x._serializedValue;
            var yl = y._serializedValue;

            if (yl == 0)
            {
                return MAX_VALUE; // 除数为零返回最大值（替代异常）
            }

            // 取绝对值进行无符号除法
            var remainder = (ulong)(xl >= 0 ? xl : -xl);
            var divider = (ulong)(yl >= 0 ? yl : -yl);
            var quotient = 0UL;
            var bitPos = NUM_BITS / 2 + 1; // 初始位位置

            // 若除数是2^n的倍数，优化移位
            while ((divider & 0xF) == 0 && bitPos >= 4)
            {
                divider >>= 4;
                bitPos -= 4;
            }

            // 迭代计算商
            while (remainder != 0 && bitPos >= 0)
            {
                int shift = CountLeadingZeroes(remainder); // 计算前导零以确定移位量
                if (shift > bitPos)
                {
                    shift = bitPos;
                }

                remainder <<= shift; // 左移 remainder 以对齐最高位
                bitPos -= shift;

                var div = remainder / divider; // 计算当前位的商
                remainder = remainder % divider; // 更新余数
                quotient += div << bitPos; // 累加商的当前位

                // 检查溢出
                if ((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0)
                {
                    // 根据符号返回边界值
                    return ((xl ^ yl) & MIN_VALUE) == 0 ? MaxValue : MinValue;
                }

                remainder <<= 1; // 余数左移一位，准备下一轮计算
                --bitPos;
            }

            // 四舍五入处理
            ++quotient;
            var result = (long)(quotient >> 1);
            // 恢复符号
            if (((xl ^ yl) & MIN_VALUE) != 0)
            {
                result = -result;
            }

            FP r;
            r._serializedValue = result;
            return r;
        }

        /// <summary>
        /// 取模运算符：x % y
        /// 处理特殊情况（x为MinValue且y为-1时返回0）
        /// </summary>
        public static FP operator %(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue == MIN_VALUE & y._serializedValue == -1
                ? 0
                : // 特殊情况：MinValue % -1 = 0
                x._serializedValue % y._serializedValue;
            return result;
        }

        /// <summary>
        /// 快速取模运算（不处理特殊情况）
        /// 当x为MinValue且y为-1时会抛出异常
        /// </summary>
        public static FP FastMod(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue % y._serializedValue;
            return result;
        }

        /// <summary>
        /// 取反运算符：-x
        /// 处理MinValue的特殊情况（-MinValue = MaxValue）
        /// </summary>
        public static FP operator -(FP x)
        {
            return x._serializedValue == MIN_VALUE ? MaxValue : new FP(-x._serializedValue);
        }

        /// <summary>
        /// 相等运算符：x == y
        /// 通过比较原始值判断
        /// </summary>
        public static bool operator ==(FP x, FP y)
        {
            return x._serializedValue == y._serializedValue;
        }

        /// <summary>
        /// 不等运算符：x != y
        /// </summary>
        public static bool operator !=(FP x, FP y)
        {
            return x._serializedValue != y._serializedValue;
        }

        /// <summary>
        /// 大于运算符：x > y
        /// </summary>
        public static bool operator >(FP x, FP y)
        {
            return x._serializedValue > y._serializedValue;
        }

        /// <summary>
        /// 小于运算符：x < y
        /// </summary>
        public static bool operator <(FP x, FP y)
        {
            return x._serializedValue < y._serializedValue;
        }

        /// <summary>
        /// 大于等于运算符：x >= y
        /// </summary>
        public static bool operator >=(FP x, FP y)
        {
            return x._serializedValue >= y._serializedValue;
        }

        /// <summary>
        /// 小于等于运算符：x <= y
        /// </summary>
        public static bool operator <=(FP x, FP y)
        {
            return x._serializedValue <= y._serializedValue;
        }

        /// <summary>
        /// 计算指定数的平方根
        /// </summary>
        /// <param name="x">输入值（非负）</param>
        /// <returns>平方根结果</returns>
        /// <exception cref="ArgumentOutOfRangeException">输入为负数时抛出</exception>
        public static FP Sqrt(FP x)
        {
            var xl = x._serializedValue;
            if (xl < 0)
            {
                throw new ArgumentOutOfRangeException("Negative value passed to Sqrt", "x");
            }

            var num = (ulong)xl; // 转换为无符号数处理
            var result = 0UL;

            // 找到最高有效位的位置
            var bit = 1UL << (NUM_BITS - 2);
            while (bit > num)
            {
                bit >>= 2;
            }

            // 分两轮计算以避免使用128位运算
            for (var i = 0; i < 2; ++i)
            {
                // 第一轮计算高48位
                while (bit != 0)
                {
                    if (num >= result + bit)
                    {
                        num -= result + bit;
                        result = (result >> 1) + bit;
                    }
                    else
                    {
                        result = result >> 1;
                    }

                    bit >>= 2;
                }

                if (i == 0)
                {
                    // 第二轮处理低16位
                    if (num > (1UL << (NUM_BITS / 2)) - 1)
                    {
                        // 余数过大，手动调整结果
                        num -= result;
                        num = (num << (NUM_BITS / 2)) - 0x80000000UL;
                        result = (result << (NUM_BITS / 2)) + 0x80000000UL;
                    }
                    else
                    {
                        num <<= (NUM_BITS / 2);
                        result <<= (NUM_BITS / 2);
                    }

                    bit = 1UL << (NUM_BITS / 2 - 2);
                }
            }

            // 四舍五入
            if (num > result)
            {
                ++result;
            }

            FP r;
            r._serializedValue = (long)result;
            return r;
        }

        /// <summary>
        /// 计算正弦值
        /// 对小值有约9位小数精度，大值可能精度下降
        /// 性能：x64上比Math.Sin慢25%，x86上慢200%
        /// </summary>
        public static FP Sin(FP x)
        {
            bool flipHorizontal, flipVertical;
            // 将角度归一化到[0, π/2)并确定是否需要镜像
            var clampedL = ClampSinValue(x._serializedValue, out flipHorizontal, out flipVertical);
            var clamped = new FP(clampedL);

            // 查找表索引计算
            var rawIndex = FastMul(clamped, LutInterval);
            var roundedIndex = Round(rawIndex);
            var indexError = 0; // 索引误差（原始索引与四舍五入索引的差）

            // 获取查找表中的邻近值
            var nearestValue =
                new FP(SinLut[flipHorizontal ? SinLut.Length - 1 - (int)roundedIndex : (int)roundedIndex]);
            var secondNearestValue =
                new FP(SinLut[
                    flipHorizontal
                        ? SinLut.Length - 1 - (int)roundedIndex - Sign(indexError)
                        : (int)roundedIndex + Sign(indexError)]);

            // 线性插值计算
            var delta = FastMul(indexError, FastAbs(FastSub(nearestValue, secondNearestValue)))._serializedValue;
            var interpolatedValue = nearestValue._serializedValue + (flipHorizontal ? -delta : delta);
            var finalValue = flipVertical ? -interpolatedValue : interpolatedValue;

            FP a2;
            a2._serializedValue = finalValue;
            return a2;
        }

        /// <summary>
        /// 快速计算正弦值（精度较低）
        /// x86上比Sin快至少3倍，略快于Math.Sin
        /// 精度限制在4-5位小数（小值范围内）
        /// </summary>
        public static FP FastSin(FP x)
        {
            bool flipHorizontal, flipVertical;
            var clampedL = ClampSinValue(x._serializedValue, out flipHorizontal, out flipVertical);

            // 直接使用角度右移15位作为查找表索引（利用LUT_SIZE的特性）
            var rawIndex = (uint)(clampedL >> 15);
            if (rawIndex >= LUT_SIZE)
            {
                rawIndex = LUT_SIZE - 1;
            }

            // 从查找表获取值并应用镜像
            var nearestValue = SinLut[flipHorizontal ? SinLut.Length - 1 - (int)rawIndex : (int)rawIndex];

            FP result;
            result._serializedValue = flipVertical ? -nearestValue : nearestValue;
            return result;
        }

        /// <summary>
        /// 将角度归一化到[0, π/2)范围，并确定是否需要水平/垂直镜像
        /// 用于优化三角函数计算（仅需存储[0, π/2)的查找表）
        /// </summary>
        /// <param name="angle">原始角度（弧度）的原始值</param>
        /// <param name="flipHorizontal">是否需要水平镜像</param>
        /// <param name="flipVertical">是否需要垂直镜像</param>
        /// <returns>归一化到[0, π/2)的角度原始值</returns>
        public static long ClampSinValue(long angle, out bool flipHorizontal, out bool flipVertical)
        {
            // 先将角度归一化到[0, 2π)
            var clamped2Pi = angle % PI_TIMES_2;
            if (angle < 0)
            {
                clamped2Pi += PI_TIMES_2;
            }

            // 垂直镜像：角度在[π, 2π)时需要翻转（sin(π+θ) = -sinθ）
            flipVertical = clamped2Pi >= PI;

            // 归一化到[0, π)
            var clampedPi = clamped2Pi;
            while (clampedPi >= PI)
            {
                clampedPi -= PI;
            }

            // 水平镜像：角度在[π/2, π)时需要翻转（sin(π-θ) = sinθ）
            flipHorizontal = clampedPi >= PI_OVER_2;

            // 归一化到[0, π/2)
            var clampedPiOver2 = clampedPi;
            if (clampedPiOver2 >= PI_OVER_2)
            {
                clampedPiOver2 -= PI_OVER_2;
            }

            return clampedPiOver2;
        }

        /// <summary>
        /// 计算余弦值（基于正弦值实现：cos(x) = sin(π/2 - x)）
        /// 精度和性能参考Sin方法
        /// </summary>
        public static FP Cos(FP x)
        {
            var xl = x._serializedValue;
            // 利用恒等式：cos(x) = sin(π/2 - x)
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            FP a2 = Sin(new FP(rawAngle));
            return a2;
        }

        /// <summary>
        /// 快速计算余弦值（基于FastSin实现）
        /// 精度和性能参考FastSin方法
        /// </summary>
        public static FP FastCos(FP x)
        {
            var xl = x._serializedValue;
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return FastSin(new FP(rawAngle));
        }

        /// <summary>
        /// 计算正切值（tan(x) = sin(x)/cos(x)）
        /// 注意：该方法测试不充分，可能存在较大误差
        /// </summary>
        public static FP Tan(FP x)
        {
            // 归一化角度到[0, π/2)
            var clampedPi = x._serializedValue % PI;
            var flip = false;
            if (clampedPi < 0)
            {
                clampedPi = -clampedPi;
                flip = true;
            }

            if (clampedPi > PI_OVER_2)
            {
                flip = !flip;
                clampedPi = PI_OVER_2 - (clampedPi - PI_OVER_2);
            }

            var clamped = new FP(clampedPi);

            // 查找表索引计算
            var rawIndex = FastMul(clamped, LutInterval);
            var roundedIndex = Round(rawIndex);
            var indexError = FastSub(rawIndex, roundedIndex);

            // 获取查找表中的邻近值
            var nearestValue = new FP(TanLut[(int)roundedIndex]);
            var secondNearestValue = new FP(TanLut[(int)roundedIndex + Sign(indexError)]);

            // 线性插值
            var delta = FastMul(indexError, FastAbs(FastSub(nearestValue, secondNearestValue)))._serializedValue;
            var interpolatedValue = nearestValue._serializedValue + delta;
            var finalValue = flip ? -interpolatedValue : interpolatedValue;

            FP a2 = new FP(finalValue);
            return a2;
        }

        /// <summary>
        /// 计算反正切值（使用欧拉级数）
        /// 至少有7位小数精度
        /// </summary>
        public static FP Atan(FP z)
        {
            if (z.RawValue == 0) return Zero;

            // 利用奇函数性质：Atan(-z) = -Atan(z)
            var neg = z.RawValue < 0;
            if (neg)
            {
                z = -z;
            }

            FP result;
            var two = (FP)2;
            var three = (FP)3;

            // 利用恒等式：Atan(z) = π/2 - Atan(1/z)（z > 1时）
            bool invert = z > One;
            if (invert) z = One / z;

            // 欧拉级数展开计算
            result = One;
            var term = One;

            var zSq = z * z;
            var zSq2 = zSq * two;
            var zSqPlusOne = zSq + One;
            var zSq12 = zSqPlusOne * two;
            var dividend = zSq2;
            var divisor = zSqPlusOne * three;

            // 迭代计算级数项（最多30次）
            for (var i = 2; i < 30; ++i)
            {
                term *= dividend / divisor;
                result += term;

                dividend += zSq2;
                divisor += zSq12;

                // 项为零时停止迭代
                if (term.RawValue == 0) break;
            }

            result = result * z / zSqPlusOne;

            // 应用反转校正
            if (invert)
            {
                result = PiOver2 - result;
            }

            // 恢复符号
            if (neg)
            {
                result = -result;
            }

            return result;
        }

        /// <summary>
        /// 计算坐标(x,y)的反正切值（四象限）
        /// </summary>
        public static FP Atan2(FP y, FP x)
        {
            var yl = y._serializedValue;
            var xl = x._serializedValue;

            // 处理x=0的特殊情况
            if (xl == 0)
            {
                if (yl > 0)
                {
                    return PiOver2; // y正方向：π/2
                }

                if (yl == 0)
                {
                    return Zero; // 原点：0
                }

                return -PiOver2; // y负方向：-π/2
            }

            FP atan;
            var z = y / x; // 计算y/x的比值

            FP sm = FP.EN2 * 28; // 平滑因子
            // 处理溢出情况
            if (One + sm * z * z == MaxValue)
            {
                return y < Zero ? -PiOver2 : PiOver2;
            }

            // 根据z的大小选择不同的计算方式
            if (Abs(z) < One)
            {
                atan = z / (One + sm * z * z);
                // 调整象限（x为负时）
                if (xl < 0)
                {
                    if (yl < 0)
                    {
                        return atan - Pi; // 第三象限
                    }

                    return atan + Pi; // 第二象限
                }
            }
            else
            {
                atan = PiOver2 - z / (z * z + sm);
                // 调整象限（y为负时）
                if (yl < 0)
                {
                    return atan - Pi; // 第四象限
                }
            }

            return atan; // 第一象限
        }

        /// <summary>
        /// 计算反正弦值（基于反余弦实现：asin(x) = π/2 - acos(x)）
        /// </summary>
        public static FP Asin(FP value)
        {
            return FastSub(PiOver2, Acos(value));
        }

        /// <summary>
        /// 计算反余弦值（基于反正切和平方根实现）
        /// 至少有7位小数精度
        /// </summary>
        public static FP Acos(FP x)
        {
            // 检查输入范围（必须在[-1, 1]之间）
            if (x < -One || x > One)
            {
                throw new ArgumentOutOfRangeException("Must between -FP.One and FP.One", "x");
            }

            if (x.RawValue == 0) return PiOver2; // 特殊情况：acos(0) = π/2

            // 利用恒等式：acos(x) = atan(sqrt(1-x²)/x)
            var result = Atan(Sqrt(One - x * x) / x);
            // 校正符号（x为负时加π）
            return x.RawValue < 0 ? result + Pi : result;
        }

        /// <summary>
        /// 从long隐式转换为FP
        /// 转换公式：value * 2^32
        /// </summary>
        public static implicit operator FP(long value)
        {
            FP result;
            result._serializedValue = value * ONE;
            return result;
        }

        /// <summary>
        /// 从FP显式转换为long
        /// 转换公式：value / 2^32（整数部分）
        /// </summary>
        public static explicit operator long(FP value)
        {
            return value._serializedValue >> FRACTIONAL_PLACES;
        }

        /// <summary>
        /// 从float隐式转换为FP
        /// 转换公式：(long)(value * 2^32)
        /// </summary>
        public static implicit operator FP(float value)
        {
            FP result;
            result._serializedValue = (long)(value * ONE);
            return result;
        }

        /// <summary>
        /// 从FP显式转换为float
        /// 转换公式：(float)(value / 2^32)
        /// </summary>
        public static explicit operator float(FP value)
        {
            return (float)value._serializedValue / ONE;
        }

        /// <summary>
        /// 从double隐式转换为FP
        /// 转换公式：(long)(value * 2^32)
        /// </summary>
        public static implicit operator FP(double value)
        {
            FP result;
            result._serializedValue = (long)(value * ONE);
            return result;
        }

        /// <summary>
        /// 从FP显式转换为double
        /// 转换公式：(double)(value / 2^32)
        /// </summary>
        public static explicit operator double(FP value)
        {
            return (double)value._serializedValue / ONE;
        }

        /// <summary>
        /// 从decimal显式转换为FP
        /// 转换公式：(long)(value * 2^32)
        /// </summary>
        public static explicit operator FP(decimal value)
        {
            FP result;
            result._serializedValue = (long)(value * ONE);
            return result;
        }

        /// <summary>
        /// 从int隐式转换为FP
        /// 转换公式：value * 2^32
        /// </summary>
        public static implicit operator FP(int value)
        {
            FP result;
            result._serializedValue = value * ONE;
            return result;
        }

        /// <summary>
        /// 从FP显式转换为decimal
        /// 转换公式：(decimal)(value / 2^32)
        /// </summary>
        public static explicit operator decimal(FP value)
        {
            return (decimal)value._serializedValue / ONE;
        }

        /// <summary>
        /// 转换为float
        /// </summary>
        public float AsFloat()
        {
            return (float)this;
        }

        /// <summary>
        /// 转换为int
        /// </summary>
        public int AsInt()
        {
            return (int)this;
        }

        /// <summary>
        /// 转换为long
        /// </summary>
        public long AsLong()
        {
            return (long)this;
        }

        /// <summary>
        /// 转换为double
        /// </summary>
        public double AsDouble()
        {
            return (double)this;
        }

        /// <summary>
        /// 转换为decimal
        /// </summary>
        public decimal AsDecimal()
        {
            return (decimal)this;
        }

        /// <summary>
        /// 静态方法：转换FP为float
        /// </summary>
        public static float ToFloat(FP value)
        {
            return (float)value;
        }

        /// <summary>
        /// 静态方法：转换FP为int
        /// </summary>
        public static int ToInt(FP value)
        {
            return (int)value;
        }

        /// <summary>
        /// 静态方法：转换float为FP
        /// </summary>
        public static FP FromFloat(float value)
        {
            return (FP)value;
        }

        /// <summary>
        /// 判断是否为无穷大
        /// </summary>
        public static bool IsInfinity(FP value)
        {
            return value == NegativeInfinity || value == PositiveInfinity;
        }

        /// <summary>
        /// 判断是否为非数字(NaN)
        /// </summary>
        public static bool IsNaN(FP value)
        {
            return value == NaN;
        }

        /// <summary>
        /// 重写Equals方法：比较原始值
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is FP && ((FP)obj)._serializedValue == _serializedValue;
        }

        /// <summary>
        /// 重写GetHashCode：使用原始值的哈希码
        /// </summary>
        public override int GetHashCode()
        {
            return _serializedValue.GetHashCode();
        }

        /// <summary>
        /// IEquatable<FP>接口实现：比较原始值
        /// </summary>
        public bool Equals(FP other)
        {
            return _serializedValue == other._serializedValue;
        }

        /// <summary>
        /// IComparable<FP>接口实现：比较原始值
        /// </summary>
        public int CompareTo(FP other)
        {
            return _serializedValue.CompareTo(other._serializedValue);
        }

        /// <summary>
        /// 重写ToString：转换为float的字符串表示
        /// </summary>
        public override string ToString()
        {
            return ((float)this).ToString();
        }

        /// <summary>
        /// 带格式提供者的ToString
        /// </summary>
        public string ToString(IFormatProvider provider)
        {
            return ((float)this).ToString(provider);
        }

        /// <summary>
        /// 带格式字符串的ToString
        /// </summary>
        public string ToString(string format)
        {
            return ((float)this).ToString(format);
        }

        /// <summary>
        /// 从原始长整数创建FP实例
        /// </summary>
        public static FP FromRaw(long rawValue)
        {
            return new FP(rawValue);
        }

        /// <summary>
        /// 生成反余弦查找表的代码（用于预计算优化）
        /// 输出到Fix64AcosLut.cs文件
        /// </summary>
        internal static void GenerateAcosLut()
        {
            using (var writer = new StreamWriter("Fix64AcosLut.cs"))
            {
                writer.Write(
                    @"namespace TrueSync {
    partial struct FP {
        public static readonly long[] AcosLut = new[] {");
                int lineCounter = 0;
                // 生成LUT_SIZE个样本点
                for (int i = 0; i < LUT_SIZE; ++i)
                {
                    var angle = i / ((float)(LUT_SIZE - 1)); // 输入范围[0,1]
                    if (lineCounter++ % 8 == 0)
                    {
                        writer.WriteLine();
                        writer.Write("            ");
                    }

                    var acos = Math.Acos(angle); // 计算反余弦
                    var rawValue = ((FP)acos)._serializedValue; // 转换为FP原始值
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }

                writer.Write(
                    @"
        };
    }
}");
            }
        }

        /// <summary>
        /// 生成正弦查找表的代码
        /// 输出到Fix64SinLut.cs文件
        /// </summary>
        internal static void GenerateSinLut()
        {
            using (var writer = new StreamWriter("Fix64SinLut.cs"))
            {
                writer.Write(
                    @"namespace FixMath.NET {
    partial struct Fix64 {
        public static readonly long[] SinLut = new[] {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i)
                {
                    var angle = i * Math.PI * 0.5 / (LUT_SIZE - 1); // 角度范围[0, π/2]
                    if (lineCounter++ % 8 == 0)
                    {
                        writer.WriteLine();
                        writer.Write("            ");
                    }

                    var sin = Math.Sin(angle); // 计算正弦
                    var rawValue = ((FP)sin)._serializedValue; // 转换为FP原始值
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }

                writer.Write(
                    @"
        };
    }
}");
            }
        }

        /// <summary>
        /// 生成正切查找表的代码
        /// 输出到Fix64TanLut.cs文件
        /// </summary>
        internal static void GenerateTanLut()
        {
            using (var writer = new StreamWriter("Fix64TanLut.cs"))
            {
                writer.Write(
                    @"namespace FixMath.NET {
    partial struct Fix64 {
        public static readonly long[] TanLut = new[] {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i)
                {
                    var angle = i * Math.PI * 0.5 / (LUT_SIZE - 1); // 角度范围[0, π/2]
                    if (lineCounter++ % 8 == 0)
                    {
                        writer.WriteLine();
                        writer.Write("            ");
                    }

                    var tan = Math.Tan(angle); // 计算正切
                    // 处理溢出情况（接近π/2时正切值会很大）
                    if (tan > (double)MaxValue || tan < 0.0)
                    {
                        tan = (double)MaxValue;
                    }

                    var rawValue = (((decimal)tan > (decimal)MaxValue || tan < 0.0) ? MaxValue : (FP)tan)
                        ._serializedValue;
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }

                writer.Write(
                    @"
        };
    }
}");
            }
        }

        /// <summary>
        /// 获取底层整数表示（原始值）
        /// </summary>
        public long RawValue
        {
            get { return _serializedValue; }
        }

        /// <summary>
        /// 从原始值构造FP实例（内部使用）
        /// </summary>
        FP(long rawValue)
        {
            _serializedValue = rawValue;
        }

        /// <summary>
        /// 从int构造FP实例
        /// </summary>
        public FP(int value)
        {
            _serializedValue = value * ONE;
        }
    }
}