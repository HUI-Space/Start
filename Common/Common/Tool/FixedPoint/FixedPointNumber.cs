using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Start
{
    /// <summary>
    /// "&" 是位运算符 运算规则是：当两个对应的二进制位都为 1 时，结果位才为 1；否则，结果位为 0。 1010 & 1100 = 1000
    /// "~" 是按位取反（bit - wise NOT）运算符。它对一个操作数的二进制位逐位进行取反操作。  ~00000100得到11111011
    /// ">>" 是右移运算符。它用于将一个整数的二进制位向右移动指定的位数
    /// "<<" 是左移运算符。它用于将一个整数的二进制位向左移动指定的位数
    /// </summary>
    [Serializable]
    public partial struct FixedPointNumber : IEquatable<FixedPointNumber>, IComparable<FixedPointNumber>
    {
        /// <summary>
        /// 是否使用BigInteger提高运算精度。
        /// 为true时会牺牲一定的性能，但是不会造成数学运算时数据溢出的情况。
        /// 为false时，运算性能会提高，但是数值过大时可能造成数学运算时数据溢出。
        /// 建议没有性能瓶颈时，设置为true，提高精度。
        /// </summary>
        public const bool IS_USE_BIG_INTEGER = true;
        
        /// <summary>
        /// 总的位数
        /// </summary>
        public const int TOTAL_BITS = sizeof(long) * 8;
        
        /// <summary>
        /// 小数位计数 （用于保存小数的位数）
        /// Fractional bit count
        /// </summary>
        public const int FRACTIONAL_BITS = 32;

        /// <summary>
        /// 整数位计数 （用于保存整数的位数）
        /// </summary>
        public const int INTEGER_BITS = 32;
        
        /// <summary>
        /// 二进制表示：0000000000000000000000000000000011111111111111111111111111111111
        /// FRACTION_MASK 这相当于 4,294,967,295（即 0xFFFFFFFF 或 2^32 - 1）
        /// 这个掩码可以用于提取一个 64 位定点数中的低 32 位（小数部分）
        /// </summary>
        public const long FRACTION_MASK = (long)(ulong.MaxValue >> INTEGER_BITS);
        
        /// <summary>
        /// 二进制表示：1111111111111111111111111111111100000000000000000000000000000000
        /// INTEGER_MASK 的值是 0xFFFFFFFF00000000，即 9,223,372,036,854,775,808。
        /// 这个掩码可以用于提取一个 64 位定点数中的高 32 位（整数部分）
        /// </summary>
        public const long INTEGER_MASK = -1L & ~FRACTION_MASK;
        
        /// <summary>
        /// 二进制表示：11111111111111111111111111111111 + 1 = 100000000000000000000000000000000
        /// 十进制表示：4,294,967,295 + 1 = 4,294,967,296。
        /// FRACTION_RANGE 的值是 4,294,967,296，即 0x100000000
        /// 这个值代表了小数部分可以表示的最大范围（即 2^32），因为它包括从 0 到 4,294,967,295 共 4,294,967,296 个可能的值。
        /// </summary>
        public const long FRACTION_RANGE = FRACTION_MASK + 1;
        
        /// <summary>
        /// 二进制表示： 1111111111111111111111111111111100000000000000000000000000000000
        /// 十进制表示: -2,147,483,648（即 int.MinValue）
        /// 最小值
        /// </summary>
        public const long MIN_VALUE = long.MinValue >> FRACTIONAL_BITS;

        /// <summary>
        /// 二进制表示： 0000000000000000000000000000000011111111111111111111111111111111
        /// 十进制表示： 2,147,483,647（即 int.MaxValue）
        /// 最大值
        /// </summary>
        public const long MAX_VALUE = long.MaxValue >> FRACTIONAL_BITS;
        
        /// <summary>
        /// 最大值
        /// </summary>
        public static readonly FixedPointNumber Max = new FixedPointNumber(MAX_VALUE);

        /// <summary>
        /// 最小值
        /// </summary>
        public static readonly FixedPointNumber Min = new FixedPointNumber(MIN_VALUE);

        /// <summary>
        /// -1
        /// </summary>
        public static readonly FixedPointNumber Negative_One = new FixedPointNumber(-1);

        /// <summary>
        /// 0
        /// </summary>
        public static readonly FixedPointNumber Zero = new FixedPointNumber(0);
        
        /// <summary>
        /// 1
        /// </summary>
        public static readonly FixedPointNumber One = new FixedPointNumber(1);
        
        long _raw;

        /// <summary>
        /// 原始数据:前面的表示整数，后8位为小数
        /// </summary>
        public long Raw
        {
            set => _raw = value;

            get => _raw;
        }
        
        /// <summary>
        /// 是否是整数
        /// </summary>
        /// <returns></returns>
        public bool IsInteger => (Raw & FRACTION_MASK) == 0;
        
        /// <summary>
        /// 获取整数部分
        /// </summary>
        public FixedPointNumber IntegerPart => new FixedPointNumber(Raw & INTEGER_MASK);

        /// <summary>
        /// 获取小数部分
        /// </summary>
        public FixedPointNumber FractionalPart => new FixedPointNumber(Raw & FRACTION_MASK);
        
        #region 静态方法
        
        /// <summary>
        /// 通过原始数据直接创建定点数
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static FixedPointNumber CreateFromRaw(long raw)
        {
            return new FixedPointNumber(raw);
        }

        /// <summary>
        /// 通过浮点数来创建定点数(精度自动四舍五入到4位)。
        /// 相比Number(int numerator, int denominator)，开销更大
        /// </summary>
        /// <param name="floatingPointNumber"></param>
        public static FixedPointNumber CreateFromDouble(double floatingPointNumber)
        {
            var raw = DoubleToRaw(floatingPointNumber);
            return CreateFromRaw(raw);
        }

        /// <summary>
        /// 通过浮点数来创建定点数(精度自动四舍五入到4位)
        /// 相比Number(int numerator, int denominator)，开销更大
        /// </summary>
        /// <param name="floatingPointNumber"></param>
        public static FixedPointNumber CreateFromFloat(float floatingPointNumber)
        {
            var raw = FloatToRaw(floatingPointNumber);
            return CreateFromRaw(raw);
        }

        /// <summary>
        /// 整数转换成Raw
        /// </summary>
        /// <param name="integer"></param>
        /// <returns></returns>
        public static long IntegerToRaw(int integer)
        {
            //先转long这样左移的时候，左边的空间才够
            return (long)integer << FRACTIONAL_BITS;
        }

        /// <summary>
        /// 通过分子分母生成Raw
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static long NumeratorAndDenominatorToRaw(int numerator, int denominator)
        {
            return IntegerToRaw(numerator) / denominator;
        }

        /// <summary>
        /// 通过浮点数生成Raw
        /// </summary>
        /// <param name="floatingPointNumber"></param>
        /// <returns></returns>
        public static long FloatToRaw(float floatingPointNumber)
        {
            return DoubleToRaw(floatingPointNumber);
        }
        
        /// <summary>
        /// 通过浮点数生成Raw
        /// </summary>
        /// <param name="floatingPointNumber"></param>
        /// <returns></returns>
        public static long DoubleToRaw(double floatingPointNumber)
        {
            if (0 == floatingPointNumber)
            {
                return IntegerToRaw(0);
            }

            //保留4位精度
            var roundFloatingPointNumber = Math.Round(floatingPointNumber, 4);
            //拿到绝对值
            var absolute = Math.Abs(roundFloatingPointNumber);
            int denominator = 1;

            while (denominator < 10000 && (absolute * denominator) % 1 > 0)
            {
                denominator *= 10;
            }

            //分子
            int numerator = Convert.ToInt32(roundFloatingPointNumber * denominator);

            return NumeratorAndDenominatorToRaw(numerator, denominator);
        }
        
        #endregion
        
        private FixedPointNumber(long raw)
        {
            _raw = raw;
        }

        /// <summary>
        /// 通过整数创建定点数
        /// </summary>
        /// <param name="integer"></param>
        /// <returns></returns>
        public FixedPointNumber(int integer)
        {
            _raw = IntegerToRaw(integer);
        }

        /// <summary>
        /// 通过指定的分子和分母来创建定点数
        /// </summary>
        /// <param name="numerator">分子</param>
        /// <param name="denominator">分母</param>
        /// <returns></returns>
        public FixedPointNumber(int numerator, int denominator)
        {
            _raw = NumeratorAndDenominatorToRaw(numerator, denominator);
        }
        
        #region 数据输出

        static string ToString(double floatingPointNumber)
        {
            var str = floatingPointNumber.ToString("F4").TrimEnd('0').TrimEnd('.');
            return str;
        }

        public override string ToString()
        {
            double d = ToDouble();
            return ToString(d);
        }

        public int ToInt()
        {
            return (int)(Raw >> FRACTIONAL_BITS);
        }

        public short ToShort()
        {
            return (short)ToInt();
        }

        public float ToFloat()
        {
            return (float)ToDouble();
        }

        public double ToDouble()
        {
            return (Raw >> FRACTIONAL_BITS) + (Raw & FRACTION_MASK) / (double)FRACTION_RANGE;
        }

        /// <summary>
        /// 转换为二进制
        /// </summary>
        /// <returns></returns>
        public string ToBinary(bool isPadLeft = true)
        {
            string binary = Convert.ToString(Raw, 2);
            if (isPadLeft)
            {
                binary = binary.PadLeft(TOTAL_BITS, '0');
            }
            return binary;
        }

        #endregion

        #region 重写运算符

        #region override operator <

        public static bool operator <(FixedPointNumber a, FixedPointNumber b)
        {
            return a.Raw < b.Raw;
        }

        public static bool operator <(int a, FixedPointNumber b)
        {
            return new FixedPointNumber(a) < b;
        }

        public static bool operator <(FixedPointNumber a, int b)
        {
            return a < new FixedPointNumber(b);
        }

        #endregion

        #region override operator >

        public static bool operator >(FixedPointNumber a, FixedPointNumber b)
        {
            return a.Raw > b.Raw;
        }

        public static bool operator >(int a, FixedPointNumber b)
        {
            return new FixedPointNumber(a) > b;
        }

        public static bool operator >(FixedPointNumber a, int b)
        {
            return a > new FixedPointNumber(b);
        }

        #endregion

        #region override operator <=

        public static bool operator <=(FixedPointNumber a, FixedPointNumber b)
        {
            return a.Raw <= b.Raw;
        }

        public static bool operator <=(int a, FixedPointNumber b)
        {
            return new FixedPointNumber(a) <= b;
        }

        public static bool operator <=(FixedPointNumber a, int b)
        {
            return a <= new FixedPointNumber(b);
        }

        #endregion

        #region override operator >=

        public static bool operator >=(FixedPointNumber a, FixedPointNumber b)
        {
            return a.Raw >= b.Raw;
        }

        public static bool operator >=(int a, FixedPointNumber b)
        {
            return new FixedPointNumber(a) >= b;
        }

        public static bool operator >=(FixedPointNumber a, int b)
        {
            return a >= new FixedPointNumber(b);
        }

        #endregion

        #region override operator ==

        public static bool operator ==(FixedPointNumber a, FixedPointNumber b)
        {
            return a.Raw == b.Raw;
        }

        public static bool operator ==(int a, FixedPointNumber b)
        {
            return new FixedPointNumber(a) == b;
        }

        public static bool operator ==(FixedPointNumber a, int b)
        {
            return a == new FixedPointNumber(b);
        }

        #endregion

        #region override operator !=

        public static bool operator !=(FixedPointNumber a, FixedPointNumber b)
        {
            return a.Raw != b.Raw;
        }

        public static bool operator !=(FixedPointNumber a, int b)
        {
            return a != new FixedPointNumber(b);
        }

        public static bool operator !=(int a, FixedPointNumber b)
        {
            return new FixedPointNumber(a) != b;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType() && this == (FixedPointNumber)obj;
        }

        #region override operator +

        public static FixedPointNumber operator +(FixedPointNumber a, FixedPointNumber b)
        {
            return new FixedPointNumber(a.Raw + b.Raw);
        }

        public static FixedPointNumber operator +(FixedPointNumber a, int b)
        {
            return a + new FixedPointNumber(b);
        }

        public static FixedPointNumber operator +(int a, FixedPointNumber b)
        {
            return new FixedPointNumber(a) + b;
        }

        #endregion

        #region override operator -

        public static FixedPointNumber operator -(FixedPointNumber a, FixedPointNumber b)
        {
            return new FixedPointNumber(a.Raw - b.Raw);
        }

        public static FixedPointNumber operator -(FixedPointNumber a, int b)
        {
            return a - new FixedPointNumber(b);
        }

        public static FixedPointNumber operator -(int a, FixedPointNumber b)
        {
            return new FixedPointNumber(a) - b;
        }

        #endregion

        #region override operator *

        public static FixedPointNumber operator *(FixedPointNumber a, FixedPointNumber b)
        {
            if (IS_USE_BIG_INTEGER)
            {
                var bigNumber = (BigInteger)a.Raw * b.Raw;
                bigNumber += FRACTION_RANGE >> 1;
                bigNumber >>= FRACTIONAL_BITS;
                return new FixedPointNumber(ConvertBigIntegerToLong(ref bigNumber));
            }

            return new FixedPointNumber((a.Raw * b.Raw + (FRACTION_RANGE >> 1)) >> FRACTIONAL_BITS);
        }

        public static FixedPointNumber operator *(FixedPointNumber a, int b)
        {
            return a * new FixedPointNumber(b);
        }

        public static FixedPointNumber operator *(int a, FixedPointNumber b)
        {
            return new FixedPointNumber(a) * b;
        }
        
        

        #endregion

        #region override operator /

        public static FixedPointNumber operator /(FixedPointNumber a, FixedPointNumber b)
        {
            if (IS_USE_BIG_INTEGER)
            {
                var bigNumber = ((BigInteger)a.Raw) << FRACTIONAL_BITS;
                bigNumber /= b.Raw;                
                return new FixedPointNumber(ConvertBigIntegerToLong(ref bigNumber));
            }

            return new FixedPointNumber((a.Raw << FRACTIONAL_BITS) / b.Raw);
        }

        public static FixedPointNumber operator /(FixedPointNumber a, int b)
        {
            return a / new FixedPointNumber(b);
        }

        public static FixedPointNumber operator /(int a, FixedPointNumber b)
        {
            return new FixedPointNumber(a) / b;
        }

        #endregion

        #region override operator %

        public static FixedPointNumber operator %(FixedPointNumber a, FixedPointNumber b)
        {
            return new FixedPointNumber(
                a.Raw == MIN_VALUE & b.Raw == -1 ? 0 : a.Raw % b.Raw);
        }

        public static FixedPointNumber operator %(FixedPointNumber a, int b)
        {
            return a % new FixedPointNumber(b);
        }

        public static FixedPointNumber operator %(int a, FixedPointNumber b)
        {
            return new FixedPointNumber(a) % b;
        }

        #endregion

        #region override operator <<

        public static FixedPointNumber operator <<(FixedPointNumber a, int b)
        {
            return new FixedPointNumber(a.Raw << b);
        }

        #endregion

        #region override operator >>

        public static FixedPointNumber operator >>(FixedPointNumber a, int b)
        {
            return new FixedPointNumber(a.Raw >> b);
        }

        #endregion
        
        public static FixedPointNumber operator -(FixedPointNumber a)
        {
            a.Raw = -a.Raw;
            return a;
        }

        #region override operator explicit 显示转换规则

        public static explicit operator int(FixedPointNumber a)
        {
            return a.ToInt();
        }

        public static explicit operator FixedPointNumber(int a)
        {
            return new FixedPointNumber(a);
        }

        #endregion

        public override int GetHashCode()
        {
            return _raw.GetHashCode();
        }

        #endregion
        
        /// <summary>
        /// 将BigInteger转换为long
        /// </summary>
        /// <param name="bigInteger"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ConvertBigIntegerToLong(ref BigInteger bigInteger)
        {            
            if (bigInteger > long.MaxValue)
            {
                return long.MaxValue;
            }

            if (bigInteger < long.MinValue)
            {
                return long.MinValue;
            }

            return (long)bigInteger;
        }

        public bool Equals(FixedPointNumber other)
        {
            return _raw == other._raw;
        }

        public int CompareTo(FixedPointNumber other)
        {
            return _raw.CompareTo(other._raw);
        }
    }
}