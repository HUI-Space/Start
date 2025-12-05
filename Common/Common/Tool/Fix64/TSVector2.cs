#region License

/*
MIT License
Copyright © 2006 The Mono.Xna Team

All rights reserved.

Authors
 * Alan McGovern

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion License

using System;

namespace Start
{
    /// <summary>
    /// 表示二维空间中的向量，使用定点数(FP)进行高精度计算，适用于物理引擎、游戏开发等场景
    /// 实现了向量的各种数学运算及常用向量操作
    /// </summary>
    [Serializable]
    public struct TSVector2 : IEquatable<TSVector2>
    {
        #region Private Fields

        /// <summary>
        /// 私有静态零向量实例 (0, 0)
        /// </summary>
        private static TSVector2 zeroVector = new TSVector2(0, 0);

        /// <summary>
        /// 私有静态单位向量实例 (1, 1)
        /// </summary>
        private static TSVector2 oneVector = new TSVector2(1, 1);

        /// <summary>
        /// 私有静态右方向向量实例 (1, 0)
        /// </summary>
        private static TSVector2 rightVector = new TSVector2(1, 0);

        /// <summary>
        /// 私有静态左方向向量实例 (-1, 0)
        /// </summary>
        private static TSVector2 leftVector = new TSVector2(-1, 0);

        /// <summary>
        /// 私有静态上方向向量实例 (0, 1)
        /// </summary>
        private static TSVector2 upVector = new TSVector2(0, 1);

        /// <summary>
        /// 私有静态下方向向量实例 (0, -1)
        /// </summary>
        private static TSVector2 downVector = new TSVector2(0, -1);

        #endregion Private Fields

        #region Public Fields

        /// <summary>
        /// 向量的X分量（水平分量）
        /// </summary>
        public FP x;

        /// <summary>
        /// 向量的Y分量（垂直分量）
        /// </summary>
        public FP y;

        #endregion Public Fields

        #region Properties

        /// <summary>
        /// 获取零向量 (0, 0)
        /// </summary>
        public static TSVector2 zero => zeroVector;

        /// <summary>
        /// 获取单位向量 (1, 1)
        /// </summary>
        public static TSVector2 one => oneVector;

        /// <summary>
        /// 获取右方向向量 (1, 0)
        /// </summary>
        public static TSVector2 right => rightVector;

        /// <summary>
        /// 获取左方向向量 (-1, 0)
        /// </summary>
        public static TSVector2 left => leftVector;

        /// <summary>
        /// 获取上方向向量 (0, 1)
        /// </summary>
        public static TSVector2 up => upVector;

        /// <summary>
        /// 获取下方向向量 (0, -1)
        /// </summary>
        public static TSVector2 down => downVector;

        /// <summary>
        /// 获取向量的模长（长度）
        /// 计算方式：sqrt(x² + y²)
        /// </summary>
        public FP magnitude
        {
            get
            {
                DistanceSquared(ref this, ref zeroVector, out var result);
                return FP.Sqrt(result);
            }
        }

        /// <summary>
        /// 获取当前向量的单位向量（方向相同，模长为1）
        /// </summary>
        public TSVector2 normalized
        {
            get
            {
                Normalize(ref this, out var result);
                return result;
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// 使用指定的X和Y分量初始化二维向量
        /// </summary>
        /// <param name="x">X轴分量值</param>
        /// <param name="y">Y轴分量值</param>
        public TSVector2(FP x, FP y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 使用相同的值初始化向量的X和Y分量（创建"正方形"向量）
        /// </summary>
        /// <param name="value">用于初始化X和Y分量的值</param>
        public TSVector2(FP value)
        {
            x = value;
            y = value;
        }

        /// <summary>
        /// 设置向量的X和Y分量为指定值
        /// </summary>
        /// <param name="x">新的X轴分量值</param>
        /// <param name="y">新的Y轴分量值</param>
        public void Set(FP x, FP y)
        {
            this.x = x;
            this.y = y;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// 计算向量在给定法向量上的反射向量（通过引用传递参数以提高性能）
        /// 反射公式：result = vector - 2 * dot(vector, normal) * normal
        /// </summary>
        /// <param name="vector">要反射的原始向量</param>
        /// <param name="normal">用于反射计算的法向量（需单位化）</param>
        /// <param name="result">输出的反射向量</param>
        public static void Reflect(ref TSVector2 vector, ref TSVector2 normal, out TSVector2 result)
        {
            FP dot = Dot(vector, normal);
            result.x = vector.x - ((2 * dot) * normal.x);
            result.y = vector.y - ((2 * dot) * normal.y);
        }

        /// <summary>
        /// 计算向量在给定法向量上的反射向量
        /// </summary>
        /// <param name="vector">要反射的原始向量</param>
        /// <param name="normal">用于反射计算的法向量（需单位化）</param>
        /// <returns>反射后的向量</returns>
        public static TSVector2 Reflect(TSVector2 vector, TSVector2 normal)
        {
            TSVector2 result;
            Reflect(ref vector, ref normal, out result);
            return result;
        }

        /// <summary>
        /// 计算两个向量的和（value1 + value2）
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>两个向量相加的结果</returns>
        public static TSVector2 Add(TSVector2 value1, TSVector2 value2)
        {
            value1.x += value2.x;
            value1.y += value2.y;
            return value1;
        }

        /// <summary>
        /// 计算两个向量的和（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <param name="result">输出的和向量</param>
        public static void Add(ref TSVector2 value1, ref TSVector2 value2, out TSVector2 result)
        {
            result.x = value1.x + value2.x;
            result.y = value1.y + value2.y;
        }

        /// <summary>
        /// 计算三个点的重心坐标插值
        /// 重心坐标插值公式：value1 + amount1*(value2 - value1) + amount2*(value3 - value1)
        /// </summary>
        /// <param name="value1">第一个点向量</param>
        /// <param name="value2">第二个点向量</param>
        /// <param name="value3">第三个点向量</param>
        /// <param name="amount1">第一个插值系数</param>
        /// <param name="amount2">第二个插值系数</param>
        /// <returns>插值结果向量</returns>
        public static TSVector2 Barycentric(TSVector2 value1, TSVector2 value2, TSVector2 value3, FP amount1,
            FP amount2)
        {
            return new TSVector2(
                TSMath.Barycentric(value1.x, value2.x, value3.x, amount1, amount2),
                TSMath.Barycentric(value1.y, value2.y, value3.y, amount1, amount2));
        }

        /// <summary>
        /// 计算三个点的重心坐标插值（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">第一个点向量</param>
        /// <param name="value2">第二个点向量</param>
        /// <param name="value3">第三个点向量</param>
        /// <param name="amount1">第一个插值系数</param>
        /// <param name="amount2">第二个插值系数</param>
        /// <param name="result">输出的插值结果向量</param>
        public static void Barycentric(ref TSVector2 value1, ref TSVector2 value2, ref TSVector2 value3, FP amount1,
            FP amount2, out TSVector2 result)
        {
            result = new TSVector2(
                TSMath.Barycentric(value1.x, value2.x, value3.x, amount1, amount2),
                TSMath.Barycentric(value1.y, value2.y, value3.y, amount1, amount2));
        }

        /// <summary>
        /// 使用Catmull-Rom样条曲线计算四个点之间的插值
        /// Catmull-Rom插值用于创建平滑曲线，通过四个控制点生成中间点
        /// </summary>
        /// <param name="value1">第一个控制点</param>
        /// <param name="value2">第二个控制点（起始点）</param>
        /// <param name="value3">第三个控制点（结束点）</param>
        /// <param name="value4">第四个控制点</param>
        /// <param name="amount">插值参数（0-1之间，0表示value2，1表示value3）</param>
        /// <returns>插值结果向量</returns>
        public static TSVector2 CatmullRom(TSVector2 value1, TSVector2 value2, TSVector2 value3, TSVector2 value4,
            FP amount)
        {
            return new TSVector2(
                TSMath.CatmullRom(value1.x, value2.x, value3.x, value4.x, amount),
                TSMath.CatmullRom(value1.y, value2.y, value3.y, value4.y, amount));
        }

        /// <summary>
        /// 使用Catmull-Rom样条曲线计算四个点之间的插值（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">第一个控制点</param>
        /// <param name="value2">第二个控制点（起始点）</param>
        /// <param name="value3">第三个控制点（结束点）</param>
        /// <param name="value4">第四个控制点</param>
        /// <param name="amount">插值参数（0-1之间，0表示value2，1表示value3）</param>
        /// <param name="result">输出的插值结果向量</param>
        public static void CatmullRom(ref TSVector2 value1, ref TSVector2 value2, ref TSVector2 value3,
            ref TSVector2 value4,
            FP amount, out TSVector2 result)
        {
            result = new TSVector2(
                TSMath.CatmullRom(value1.x, value2.x, value3.x, value4.x, amount),
                TSMath.CatmullRom(value1.y, value2.y, value3.y, value4.y, amount));
        }

        /// <summary>
        /// 将向量的分量限制在指定的最小值和最大值之间
        /// </summary>
        /// <param name="value1">要限制的向量</param>
        /// <param name="min">包含最小值的向量</param>
        /// <param name="max">包含最大值的向量</param>
        /// <returns>限制后的向量</returns>
        public static TSVector2 Clamp(TSVector2 value1, TSVector2 min, TSVector2 max)
        {
            return new TSVector2(
                TSMath.Clamp(value1.x, min.x, max.x),
                TSMath.Clamp(value1.y, min.y, max.y));
        }

        /// <summary>
        /// 将向量的分量限制在指定的最小值和最大值之间（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">要限制的向量</param>
        /// <param name="min">包含最小值的向量</param>
        /// <param name="max">包含最大值的向量</param>
        /// <param name="result">输出的限制后向量</param>
        public static void Clamp(ref TSVector2 value1, ref TSVector2 min, ref TSVector2 max, out TSVector2 result)
        {
            result = new TSVector2(
                TSMath.Clamp(value1.x, min.x, max.x),
                TSMath.Clamp(value1.y, min.y, max.y));
        }

        /// <summary>
        /// 计算两个向量之间的欧氏距离
        /// 距离公式：sqrt((x1-x2)² + (y1-y2)²)
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>两个向量之间的距离</returns>
        public static FP Distance(TSVector2 value1, TSVector2 value2)
        {
            FP result;
            DistanceSquared(ref value1, ref value2, out result);
            return (FP)FP.Sqrt(result);
        }

        /// <summary>
        /// 计算两个向量之间的欧氏距离（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <param name="result">输出的距离值</param>
        public static void Distance(ref TSVector2 value1, ref TSVector2 value2, out FP result)
        {
            DistanceSquared(ref value1, ref value2, out result);
            result = (FP)FP.Sqrt(result);
        }

        /// <summary>
        /// 计算两个向量之间距离的平方（避免开方运算，用于性能敏感场景的比较）
        /// 公式：(x1-x2)² + (y1-y2)²
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>距离的平方值</returns>
        public static FP DistanceSquared(TSVector2 value1, TSVector2 value2)
        {
            FP result;
            DistanceSquared(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 计算两个向量之间距离的平方（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <param name="result">输出的距离平方值</param>
        public static void DistanceSquared(ref TSVector2 value1, ref TSVector2 value2, out FP result)
        {
            result = (value1.x - value2.x) * (value1.x - value2.x) + (value1.y - value2.y) * (value1.y - value2.y);
        }

        /// <summary>
        /// 计算两个向量的分量除法（value1 / value2）
        /// </summary>
        /// <param name="value1">被除数向量</param>
        /// <param name="value2">除数向量</param>
        /// <returns>除法结果向量</returns>
        public static TSVector2 Divide(TSVector2 value1, TSVector2 value2)
        {
            value1.x /= value2.x;
            value1.y /= value2.y;
            return value1;
        }

        /// <summary>
        /// 计算两个向量的分量除法（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">被除数向量</param>
        /// <param name="value2">除数向量</param>
        /// <param name="result">输出的除法结果向量</param>
        public static void Divide(ref TSVector2 value1, ref TSVector2 value2, out TSVector2 result)
        {
            result.x = value1.x / value2.x;
            result.y = value1.y / value2.y;
        }

        /// <summary>
        /// 将向量的每个分量除以一个标量值
        /// </summary>
        /// <param name="value1">要缩放的向量</param>
        /// <param name="divider">除数标量</param>
        /// <returns>缩放后的向量</returns>
        public static TSVector2 Divide(TSVector2 value1, FP divider)
        {
            FP factor = 1 / divider;
            value1.x *= factor;
            value1.y *= factor;
            return value1;
        }

        /// <summary>
        /// 将向量的每个分量除以一个标量值（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">要缩放的向量</param>
        /// <param name="divider">除数标量</param>
        /// <param name="result">输出的缩放后向量</param>
        public static void Divide(ref TSVector2 value1, FP divider, out TSVector2 result)
        {
            FP factor = 1 / divider;
            result.x = value1.x * factor;
            result.y = value1.y * factor;
        }

        /// <summary>
        /// 计算两个向量的点积（内积）
        /// 点积公式：x1*x2 + y1*y2
        /// 点积结果可用于判断向量方向关系：正为同向，负为反向，零为垂直
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>点积结果</returns>
        public static FP Dot(TSVector2 value1, TSVector2 value2)
        {
            return value1.x * value2.x + value1.y * value2.y;
        }

        /// <summary>
        /// 计算两个向量的点积（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <param name="result">输出的点积结果</param>
        public static void Dot(ref TSVector2 value1, ref TSVector2 value2, out FP result)
        {
            result = value1.x * value2.x + value1.y * value2.y;
        }

        /// <summary>
        /// 确定指定对象是否等于当前向量
        /// </summary>
        /// <param name="obj">要与当前向量比较的对象</param>
        /// <returns>如果对象是相等的向量则返回true，否则返回false</returns>
        public override bool Equals(object obj)
        {
            return (obj is TSVector2) ? this == ((TSVector2)obj) : false;
        }

        /// <summary>
        /// 确定当前向量是否等于另一个向量
        /// </summary>
        /// <param name="other">要比较的另一个向量</param>
        /// <returns>如果两个向量相等则返回true，否则返回false</returns>
        public bool Equals(TSVector2 other)
        {
            return this == other;
        }

        /// <summary>
        /// 计算当前向量的哈希码
        /// </summary>
        /// <returns>哈希码值</returns>
        public override int GetHashCode()
        {
            return (int)(x + y);
        }

        /// <summary>
        /// 使用Hermite插值计算两个向量之间的插值，考虑切线方向
        /// Hermite插值可用于实现带方向的平滑过渡
        /// </summary>
        /// <param name="value1">起始点向量</param>
        /// <param name="tangent1">起始点的切线向量</param>
        /// <param name="value2">结束点向量</param>
        /// <param name="tangent2">结束点的切线向量</param>
        /// <param name="amount">插值参数（0-1之间）</param>
        /// <returns>插值结果向量</returns>
        public static TSVector2 Hermite(TSVector2 value1, TSVector2 tangent1, TSVector2 value2, TSVector2 tangent2,
            FP amount)
        {
            TSVector2 result = new TSVector2();
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
            return result;
        }

        /// <summary>
        /// 使用Hermite插值计算两个向量之间的插值（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">起始点向量</param>
        /// <param name="tangent1">起始点的切线向量</param>
        /// <param name="value2">结束点向量</param>
        /// <param name="tangent2">结束点的切线向量</param>
        /// <param name="amount">插值参数（0-1之间）</param>
        /// <param name="result">输出的插值结果向量</param>
        public static void Hermite(ref TSVector2 value1, ref TSVector2 tangent1, ref TSVector2 value2,
            ref TSVector2 tangent2,
            FP amount, out TSVector2 result)
        {
            result.x = TSMath.Hermite(value1.x, tangent1.x, value2.x, tangent2.x, amount);
            result.y = TSMath.Hermite(value1.y, tangent1.y, value2.y, tangent2.y, amount);
        }

        /// <summary>
        /// 将向量的模长限制在指定的最大长度内
        /// 如果向量长度超过maxLength，则按比例缩放至maxLength
        /// </summary>
        /// <param name="vector">要限制长度的向量</param>
        /// <param name="maxLength">最大允许长度</param>
        /// <returns>限制长度后的向量</returns>
        public static TSVector2 ClampMagnitude(TSVector2 vector, FP maxLength)
        {
            return Normalize(vector) * maxLength;
        }

        /// <summary>
        /// 计算向量模长的平方（避免开方运算，用于性能敏感场景）
        /// 公式：x² + y²
        /// </summary>
        /// <returns>模长的平方值</returns>
        public FP LengthSquared()
        {
            FP result;
            DistanceSquared(ref this, ref zeroVector, out result);
            return result;
        }

        /// <summary>
        /// 在两个向量之间进行线性插值（Lerp），并将插值参数限制在0-1范围内
        /// 插值公式：value1 + (value2 - value1) * amount
        /// </summary>
        /// <param name="value1">起始向量（amount=0时的结果）</param>
        /// <param name="value2">结束向量（amount=1时的结果）</param>
        /// <param name="amount">插值参数（0-1之间，超出范围会被钳位）</param>
        /// <returns>插值结果向量</returns>
        public static TSVector2 Lerp(TSVector2 value1, TSVector2 value2, FP amount)
        {
            amount = TSMath.Clamp(amount, 0, 1);

            return new TSVector2(
                TSMath.Lerp(value1.x, value2.x, amount),
                TSMath.Lerp(value1.y, value2.y, amount));
        }

        /// <summary>
        /// 在两个向量之间进行线性插值（Lerp），不限制插值参数范围
        /// 用于需要外插（超出value1和value2范围）的场景
        /// </summary>
        /// <param name="value1">起始向量</param>
        /// <param name="value2">结束向量</param>
        /// <param name="amount">插值参数（可以是任意值）</param>
        /// <returns>插值结果向量</returns>
        public static TSVector2 LerpUnclamped(TSVector2 value1, TSVector2 value2, FP amount)
        {
            return new TSVector2(
                TSMath.Lerp(value1.x, value2.x, amount),
                TSMath.Lerp(value1.y, value2.y, amount));
        }

        /// <summary>
        /// 在两个向量之间进行线性插值（Lerp），不限制插值参数范围（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">起始向量</param>
        /// <param name="value2">结束向量</param>
        /// <param name="amount">插值参数（可以是任意值）</param>
        /// <param name="result">输出的插值结果向量</param>
        public static void LerpUnclamped(ref TSVector2 value1, ref TSVector2 value2, FP amount, out TSVector2 result)
        {
            result = new TSVector2(
                TSMath.Lerp(value1.x, value2.x, amount),
                TSMath.Lerp(value1.y, value2.y, amount));
        }

        /// <summary>
        /// 计算两个向量的分量最大值（逐分量取最大值）
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>每个分量都是两个向量对应分量最大值的向量</returns>
        public static TSVector2 Max(TSVector2 value1, TSVector2 value2)
        {
            return new TSVector2(
                TSMath.Max(value1.x, value2.x),
                TSMath.Max(value1.y, value2.y));
        }

        /// <summary>
        /// 计算两个向量的分量最大值（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <param name="result">输出的最大值向量</param>
        public static void Max(ref TSVector2 value1, ref TSVector2 value2, out TSVector2 result)
        {
            result.x = TSMath.Max(value1.x, value2.x);
            result.y = TSMath.Max(value1.y, value2.y);
        }

        /// <summary>
        /// 计算两个向量的分量最小值（逐分量取最小值）
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>每个分量都是两个向量对应分量最小值的向量</returns>
        public static TSVector2 Min(TSVector2 value1, TSVector2 value2)
        {
            return new TSVector2(
                TSMath.Min(value1.x, value2.x),
                TSMath.Min(value1.y, value2.y));
        }

        /// <summary>
        /// 计算两个向量的分量最小值（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <param name="result">输出的最小值向量</param>
        public static void Min(ref TSVector2 value1, ref TSVector2 value2, out TSVector2 result)
        {
            result.x = TSMath.Min(value1.x, value2.x);
            result.y = TSMath.Min(value1.y, value2.y);
        }

        /// <summary>
        /// 将当前向量与另一个向量进行分量乘法（缩放）
        /// </summary>
        /// <param name="other">用于缩放的向量</param>
        public void Scale(TSVector2 other)
        {
            this.x = x * other.x;
            this.y = y * other.y;
        }

        /// <summary>
        /// 计算两个向量的分量乘法（缩放）
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">用于缩放的第二个向量</param>
        /// <returns>分量相乘的结果向量</returns>
        public static TSVector2 Scale(TSVector2 value1, TSVector2 value2)
        {
            TSVector2 result;
            result.x = value1.x * value2.x;
            result.y = value1.y * value2.y;

            return result;
        }

        /// <summary>
        /// 计算两个向量的分量乘法
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>分量相乘的结果向量</returns>
        public static TSVector2 Multiply(TSVector2 value1, TSVector2 value2)
        {
            value1.x *= value2.x;
            value1.y *= value2.y;
            return value1;
        }

        /// <summary>
        /// 将向量的每个分量乘以一个标量值（缩放向量）
        /// </summary>
        /// <param name="value1">要缩放的向量</param>
        /// <param name="scaleFactor">缩放因子</param>
        /// <returns>缩放后的向量</returns>
        public static TSVector2 Multiply(TSVector2 value1, FP scaleFactor)
        {
            value1.x *= scaleFactor;
            value1.y *= scaleFactor;
            return value1;
        }

        /// <summary>
        /// 将向量的每个分量乘以一个标量值（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">要缩放的向量</param>
        /// <param name="scaleFactor">缩放因子</param>
        /// <param name="result">输出的缩放后向量</param>
        public static void Multiply(ref TSVector2 value1, FP scaleFactor, out TSVector2 result)
        {
            result.x = value1.x * scaleFactor;
            result.y = value1.y * scaleFactor;
        }

        /// <summary>
        /// 计算两个向量的分量乘法（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <param name="result">输出的分量相乘结果向量</param>
        public static void Multiply(ref TSVector2 value1, ref TSVector2 value2, out TSVector2 result)
        {
            result.x = value1.x * value2.x;
            result.y = value1.y * value2.y;
        }

        /// <summary>
        /// 计算向量的负向量（每个分量取相反数）
        /// </summary>
        /// <param name="value">要取负的向量</param>
        /// <returns>负向量</returns>
        public static TSVector2 Negate(TSVector2 value)
        {
            value.x = -value.x;
            value.y = -value.y;
            return value;
        }

        /// <summary>
        /// 计算向量的负向量（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value">要取负的向量</param>
        /// <param name="result">输出的负向量</param>
        public static void Negate(ref TSVector2 value, out TSVector2 result)
        {
            result.x = -value.x;
            result.y = -value.y;
        }

        /// <summary>
        /// 将当前向量归一化（转换为单位向量，保持方向不变，模长为1）
        /// </summary>
        public void Normalize()
        {
            Normalize(ref this, out this);
        }

        /// <summary>
        /// 将指定向量归一化并返回结果
        /// </summary>
        /// <param name="value">要归一化的向量</param>
        /// <returns>归一化后的单位向量</returns>
        public static TSVector2 Normalize(TSVector2 value)
        {
            Normalize(ref value, out value);
            return value;
        }

        /// <summary>
        /// 将向量归一化（通过引用传递参数以提高性能）
        /// 归一化公式：v / |v| （向量除以其模长）
        /// </summary>
        /// <param name="value">要归一化的向量</param>
        /// <param name="result">输出的单位向量</param>
        public static void Normalize(ref TSVector2 value, out TSVector2 result)
        {
            FP factor;
            DistanceSquared(ref value, ref zeroVector, out factor);
            factor = 1f / (FP)FP.Sqrt(factor);
            result.x = value.x * factor;
            result.y = value.y * factor;
        }

        /// <summary>
        /// 使用平滑step插值在两个向量之间进行插值
        /// 平滑step插值在边界处有更平滑的过渡（导数连续）
        /// </summary>
        /// <param name="value1">起始向量</param>
        /// <param name="value2">结束向量</param>
        /// <param name="amount">插值参数（0-1之间）</param>
        /// <returns>插值结果向量</returns>
        public static TSVector2 SmoothStep(TSVector2 value1, TSVector2 value2, FP amount)
        {
            return new TSVector2(
                TSMath.SmoothStep(value1.x, value2.x, amount),
                TSMath.SmoothStep(value1.y, value2.y, amount));
        }

        /// <summary>
        /// 使用平滑step插值在两个向量之间进行插值（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">起始向量</param>
        /// <param name="value2">结束向量</param>
        /// <param name="amount">插值参数（0-1之间）</param>
        /// <param name="result">输出的插值结果向量</param>
        public static void SmoothStep(ref TSVector2 value1, ref TSVector2 value2, FP amount, out TSVector2 result)
        {
            result = new TSVector2(
                TSMath.SmoothStep(value1.x, value2.x, amount),
                TSMath.SmoothStep(value1.y, value2.y, amount));
        }

        /// <summary>
        /// 计算两个向量的差（value1 - value2）
        /// </summary>
        /// <param name="value1">被减向量</param>
        /// <param name="value2">减向量</param>
        /// <returns>差向量</returns>
        public static TSVector2 Subtract(TSVector2 value1, TSVector2 value2)
        {
            value1.x -= value2.x;
            value1.y -= value2.y;
            return value1;
        }

        /// <summary>
        /// 计算两个向量的差（通过引用传递参数以提高性能）
        /// </summary>
        /// <param name="value1">被减向量</param>
        /// <param name="value2">减向量</param>
        /// <param name="result">输出的差向量</param>
        public static void Subtract(ref TSVector2 value1, ref TSVector2 value2, out TSVector2 result)
        {
            result.x = value1.x - value2.x;
            result.y = value1.y - value2.y;
        }

        /// <summary>
        /// 计算两个向量之间的夹角（单位：度）
        /// 角度公式：acos( (a·b) / (|a|·|b|) ) * (180/π)
        /// </summary>
        /// <param name="a">第一个向量</param>
        /// <param name="b">第二个向量</param>
        /// <returns>两个向量之间的夹角（度）</returns>
        public static FP Angle(TSVector2 a, TSVector2 b)
        {
            return FP.Acos(a.normalized * b.normalized) * FP.Rad2Deg;
        }

        /// <summary>
        /// 将当前二维向量转换为三维向量（Z分量为0）
        /// </summary>
        /// <returns>转换后的三维向量</returns>
        public TSVector ToTSVector()
        {
            return new TSVector(this.x, this.y, 0);
        }

        /// <summary>
        /// 将向量转换为字符串表示形式
        /// </summary>
        /// <returns>格式为"(x, y)"的字符串</returns>
        public override string ToString()
        {
            return string.Format("({0:f1}, {1:f1})", x.AsFloat(), y.AsFloat());
        }

        #endregion Public Methods

        #region Operators

        /// <summary>
        /// 重载一元减运算符，返回向量的负向量
        /// </summary>
        /// <param name="value">要取负的向量</param>
        /// <returns>负向量</returns>
        public static TSVector2 operator -(TSVector2 value)
        {
            value.x = -value.x;
            value.y = -value.y;
            return value;
        }

        /// <summary>
        /// 重载相等运算符，判断两个向量是否相等
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>如果两个向量的对应分量都相等则返回true，否则返回false</returns>
        public static bool operator ==(TSVector2 value1, TSVector2 value2)
        {
            return value1.x == value2.x && value1.y == value2.y;
        }

        /// <summary>
        /// 重载不等运算符，判断两个向量是否不相等
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>如果两个向量的任何对应分量不相等则返回true，否则返回false</returns>
        public static bool operator !=(TSVector2 value1, TSVector2 value2)
        {
            return value1.x != value2.x || value1.y != value2.y;
        }

        /// <summary>
        /// 重载加法运算符，计算两个向量的和
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>和向量</returns>
        public static TSVector2 operator +(TSVector2 value1, TSVector2 value2)
        {
            value1.x += value2.x;
            value1.y += value2.y;
            return value1;
        }

        /// <summary>
        /// 重载减法运算符，计算两个向量的差
        /// </summary>
        /// <param name="value1">被减向量</param>
        /// <param name="value2">减向量</param>
        /// <returns>差向量</returns>
        public static TSVector2 operator -(TSVector2 value1, TSVector2 value2)
        {
            value1.x -= value2.x;
            value1.y -= value2.y;
            return value1;
        }

        /// <summary>
        /// 重载乘法运算符，计算两个向量的点积
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>点积结果</returns>
        public static FP operator *(TSVector2 value1, TSVector2 value2)
        {
            return TSVector2.Dot(value1, value2);
        }

        /// <summary>
        /// 重载乘法运算符，将向量乘以标量（缩放向量）
        /// </summary>
        /// <param name="value">要缩放的向量</param>
        /// <param name="scaleFactor">缩放因子</param>
        /// <returns>缩放后的向量</returns>
        public static TSVector2 operator *(TSVector2 value, FP scaleFactor)
        {
            value.x *= scaleFactor;
            value.y *= scaleFactor;
            return value;
        }

        /// <summary>
        /// 重载乘法运算符，将标量乘以向量（缩放向量）
        /// </summary>
        /// <param name="scaleFactor">缩放因子</param>
        /// <param name="value">要缩放的向量</param>
        /// <returns>缩放后的向量</returns>
        public static TSVector2 operator *(FP scaleFactor, TSVector2 value)
        {
            value.x *= scaleFactor;
            value.y *= scaleFactor;
            return value;
        }

        /// <summary>
        /// 重载除法运算符，计算两个向量的分量除法
        /// </summary>
        /// <param name="value1">被除数向量</param>
        /// <param name="value2">除数向量</param>
        /// <returns>除法结果向量</returns>
        public static TSVector2 operator /(TSVector2 value1, TSVector2 value2)
        {
            value1.x /= value2.x;
            value1.y /= value2.y;
            return value1;
        }

        /// <summary>
        /// 重载除法运算符，将向量除以标量（缩放向量）
        /// </summary>
        /// <param name="value1">被除数向量</param>
        /// <param name="divider">除数标量</param>
        /// <returns>除法结果向量</returns>
        public static TSVector2 operator /(TSVector2 value1, FP divider)
        {
            FP factor = 1 / divider;
            value1.x *= factor;
            value1.y *= factor;
            return value1;
        }

        #endregion Operators
    }
}