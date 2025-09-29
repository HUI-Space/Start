/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
* 
*  本软件按"原样"提供，不提供任何明示或暗示的担保。
*  在任何情况下，作者均不对因使用本软件而产生的任何损害承担责任。
*
*  允许任何人将本软件用于任何目的，包括商业应用，并可自由修改和重新分发，
*  但需遵守以下限制：
*
*  1. 不得歪曲本软件的来源；不得声称您编写了原始软件。
*     如果您在产品中使用本软件，在产品文档中致谢会受到赞赏但非必需。
*  2. 修改后的源版本必须明确标记为修改版，不得歪曲为原始软件。
*  3. 本声明不得从任何源分发中删除或修改。 
*/

using System;

namespace TrueSync
{
    /// <summary>
    /// 四维向量结构，用于表示具有X、Y、Z、W四个分量的向量数据
    /// 可用于3D图形学中的齐次坐标、颜色RGBA值等需要四个分量的场景
    /// </summary>
    [Serializable] // 允许该结构被序列化，可用于网络传输或本地存储
    public struct TSVector4
    {
        // 内部静态变量，用于判断向量是否接近零向量的平方阈值
        private static FP ZeroEpsilonSq = TSMath.Epsilon;
        internal static TSVector4 InternalZero;

        /// <summary>向量的X分量</summary>
        public FP x;
        /// <summary>向量的Y分量</summary>
        public FP y;
        /// <summary>向量的Z分量</summary>
        public FP z;
        /// <summary>向量的W分量</summary>
        public FP w;

        #region 静态只读变量
        /// <summary>
        /// 所有分量都为0的向量 (0,0,0,0)
        /// 常用于初始化或表示零向量
        /// </summary>
        public static readonly TSVector4 zero;
        
        /// <summary>
        /// 所有分量都为1的向量 (1,1,1,1)
        /// 常用于缩放操作的初始值
        /// </summary>
        public static readonly TSVector4 one;
        
        /// <summary>
        /// 所有分量都为FP类型最小值的向量
        /// 可用于初始化最小值比较
        /// </summary>
        public static readonly TSVector4 MinValue;
        
        /// <summary>
        /// 所有分量都为FP类型最大值的向量
        /// 可用于初始化最大值比较
        /// </summary>
        public static readonly TSVector4 MaxValue;
        #endregion

        #region 私有静态构造函数
        /// <summary>
        /// 静态构造函数，初始化所有静态只读向量
        /// 在第一次访问TSVector4类型时执行
        /// </summary>
        static TSVector4()
        {
            one = new TSVector4(1, 1, 1, 1);
            zero = new TSVector4(0, 0, 0, 0);
            MinValue = new TSVector4(FP.MinValue);
            MaxValue = new TSVector4(FP.MaxValue);
            InternalZero = zero;
        }
        #endregion

        /// <summary>
        /// 计算向量各分量的绝对值
        /// </summary>
        /// <param name="other">输入向量</param>
        /// <returns>各分量取绝对值后的新向量</returns>
        public static TSVector4 Abs(TSVector4 other)
        {
            // 注意：原代码此处z分量重复使用，应为笔误，正确应为w分量取绝对值
            return new TSVector4(FP.Abs(other.x), FP.Abs(other.y), FP.Abs(other.z), FP.Abs(other.w));
        }

        /// <summary>
        /// 获取向量的平方长度（模长的平方）
        /// 相比magnitude性能更高，常用于比较向量长度大小
        /// </summary>
        /// <returns>向量的平方长度</returns>
        public FP sqrMagnitude
        {
            get
            {
                return (x * x) + (y * y) + (z * z) + (w * w);
            }
        }

        /// <summary>
        /// 获取向量的长度（模长）
        /// 计算方式为平方长度的平方根
        /// </summary>
        /// <returns>向量的长度</returns>
        public FP magnitude
        {
            get
            {
                FP num = sqrMagnitude;
                return FP.Sqrt(num);
            }
        }

        /// <summary>
        /// 将向量的长度限制在指定的最大值内
        /// 如果原向量长度小于等于maxLength，则返回原向量
        /// 否则返回方向相同但长度为maxLength的向量
        /// </summary>
        /// <param name="vector">需要限制长度的向量</param>
        /// <param name="maxLength">最大长度限制</param>
        /// <returns>长度被限制后的新向量</returns>
        public static TSVector4 ClampMagnitude(TSVector4 vector, FP maxLength)
        {
            return Normalize(vector) * maxLength;
        }

        /// <summary>
        /// 获取当前向量的归一化版本（单位向量）
        /// 单位向量与原向量方向相同，但长度为1
        /// </summary>
        /// <returns>归一化后的新向量</returns>
        public TSVector4 normalized
        {
            get
            {
                TSVector4 result = new TSVector4(this.x, this.y, this.z, this.w);
                result.Normalize();
                return result;
            }
        }

        /// <summary>
        /// 构造函数，使用int类型初始化向量的四个分量
        /// </summary>
        /// <param name="x">X分量值</param>
        /// <param name="y">Y分量值</param>
        /// <param name="z">Z分量值</param>
        /// <param name="w">W分量值</param>
        public TSVector4(int x, int y, int z, int w)
        {
            this.x = (FP)x;
            this.y = (FP)y;
            this.z = (FP)z;
            this.w = (FP)w;
        }

        /// <summary>
        /// 构造函数，使用FP类型初始化向量的四个分量
        /// </summary>
        /// <param name="x">X分量值</param>
        /// <param name="y">Y分量值</param>
        /// <param name="z">Z分量值</param>
        /// <param name="w">W分量值</param>
        public TSVector4(FP x, FP y, FP z, FP w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// 将当前向量与另一个向量进行分量相乘（逐元素乘法）
        /// 结果存储在当前向量中
        /// </summary>
        /// <param name="other">用于相乘的另一个向量</param>
        public void Scale(TSVector4 other)
        {
            this.x = x * other.x;
            this.y = y * other.y;
            this.z = z * other.z;
            this.w = w * other.w;
        }

        /// <summary>
        /// 设置当前向量的四个分量值
        /// </summary>
        /// <param name="x">新的X分量值</param>
        /// <param name="y">新的Y分量值</param>
        /// <param name="z">新的Z分量值</param>
        /// <param name="w">新的W分量值</param>
        public void Set(FP x, FP y, FP z, FP w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// 构造函数，使用同一个FP值初始化所有四个分量
        /// </summary>
        /// <param name="xyzw">用于初始化所有分量的值</param>
        public TSVector4(FP xyzw)
        {
            this.x = xyzw;
            this.y = xyzw;
            this.z = xyzw;
            this.w = xyzw;
        }

        /// <summary>
        /// 在两个向量之间进行线性插值
        /// 插值公式：from + (to - from) * percent
        /// </summary>
        /// <param name="from">起始向量</param>
        /// <param name="to">目标向量</param>
        /// <param name="percent">插值百分比（0-1之间）</param>
        /// <returns>插值结果向量</returns>
        public static TSVector4 Lerp(TSVector4 from, TSVector4 to, FP percent)
        {
            return from + (to - from) * percent;
        }

        /// <summary>
        /// 将向量转换为字符串表示形式
        /// 格式为：(x, y, z, w)，每个分量保留1位小数
        /// </summary>
        /// <returns>向量的字符串表示</returns>
        #region public override string ToString()
        public override string ToString()
        {
            return string.Format("({0:f1}, {1:f1}, {2:f1}, {3:f1})", 
                                 x.AsFloat(), y.AsFloat(), z.AsFloat(), w.AsFloat());
        }
        #endregion

        /// <summary>
        /// 检查当前向量是否与指定对象相等
        /// 只有当对象是TSVector4类型且所有分量都相等时才返回true
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>如果相等则返回true，否则返回false</returns>
        #region public override bool Equals(object obj)
        public override bool Equals(object obj)
        {
            if (!(obj is TSVector4)) return false;
            TSVector4 other = (TSVector4)obj;

            return (x == other.x) && (y == other.y) && (z == other.z) && (w == other.w);
        }
        #endregion

        /// <summary>
        /// 对两个向量进行分量相乘（逐元素乘法）
        /// 返回一个新的向量，其各分量为输入向量对应分量的乘积
        /// </summary>
        /// <param name="vecA">第一个向量</param>
        /// <param name="vecB">第二个向量</param>
        /// <returns>分量相乘后的新向量</returns>
        public static TSVector4 Scale(TSVector4 vecA, TSVector4 vecB)
        {
            TSVector4 result;
            result.x = vecA.x * vecB.x;
            result.y = vecA.y * vecB.y;
            result.z = vecA.z * vecB.z;
            result.w = vecA.w * vecB.w;

            return result;
        }

        /// <summary>
        /// 重载相等运算符，检查两个向量是否相等
        /// 当且仅当所有对应分量都相等时返回true
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>如果相等则返回true，否则返回false</returns>
        #region public static bool operator ==(TSVector4 value1, TSVector4 value2)
        public static bool operator ==(TSVector4 value1, TSVector4 value2)
        {
            return (value1.x == value2.x) && (value1.y == value2.y) && 
                   (value1.z == value2.z) && (value1.w == value2.w);
        }
        #endregion

        /// <summary>
        /// 重载不等运算符，检查两个向量是否不相等
        /// 当任何一个对应分量不相等时返回true
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>如果不相等则返回true，否则返回false</returns>
        #region public static bool operator !=(TSVector4 value1, TSVector4 value2)
        public static bool operator !=(TSVector4 value1, TSVector4 value2)
        {
            // 原代码逻辑优化：只要有一个分量不等则返回true
            return !(value1 == value2);
        }
        #endregion

        /// <summary>
        /// 计算两个向量的分量最小值向量
        /// 结果向量的每个分量是两个输入向量对应分量中的较小值
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>包含各分量最小值的新向量</returns>
        #region public static TSVector4 Min(TSVector4 value1, TSVector4 value2)
        public static TSVector4 Min(TSVector4 value1, TSVector4 value2)
        {
            TSVector4 result;
            TSVector4.Min(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 计算两个向量的分量最小值向量（引用传递版本，性能更优）
        /// 结果向量的每个分量是两个输入向量对应分量中的较小值
        /// </summary>
        /// <param name="value1">第一个向量（引用传递）</param>
        /// <param name="value2">第二个向量（引用传递）</param>
        /// <param name="result">输出参数，存储结果向量</param>
        public static void Min(ref TSVector4 value1, ref TSVector4 value2, out TSVector4 result)
        {
            result.x = (value1.x < value2.x) ? value1.x : value2.x;
            result.y = (value1.y < value2.y) ? value1.y : value2.y;
            result.z = (value1.z < value2.z) ? value1.z : value2.z;
            result.w = (value1.w < value2.w) ? value1.w : value2.w;
        }
        #endregion

        /// <summary>
        /// 计算两个向量的分量最大值向量
        /// 结果向量的每个分量是两个输入向量对应分量中的较大值
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>包含各分量最大值的新向量</returns>
        #region public static TSVector4 Max(TSVector4 value1, TSVector4 value2)
        public static TSVector4 Max(TSVector4 value1, TSVector4 value2)
        {
            TSVector4 result;
            TSVector4.Max(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 计算两个向量之间的欧氏距离
        /// 距离公式：sqrt((x1-x2)² + (y1-y2)² + (z1-z2)² + (w1-w2)²)
        /// </summary>
        /// <param name="v1">第一个向量</param>
        /// <param name="v2">第二个向量</param>
        /// <returns>两个向量之间的距离</returns>
        public static FP Distance(TSVector4 v1, TSVector4 v2)
        {
            return FP.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + 
                          (v1.y - v2.y) * (v1.y - v2.y) + 
                          (v1.z - v2.z) * (v1.z - v2.z) + 
                          (v1.w - v2.w) * (v1.w - v2.w));
        }

        /// <summary>
        /// 计算两个向量的分量最大值向量（引用传递版本，性能更优）
        /// 结果向量的每个分量是两个输入向量对应分量中的较大值
        /// </summary>
        /// <param name="value1">第一个向量（引用传递）</param>
        /// <param name="value2">第二个向量（引用传递）</param>
        /// <param name="result">输出参数，存储结果向量</param>
        public static void Max(ref TSVector4 value1, ref TSVector4 value2, out TSVector4 result)
        {
            result.x = (value1.x > value2.x) ? value1.x : value2.x;
            result.y = (value1.y > value2.y) ? value1.y : value2.y;
            result.z = (value1.z > value2.z) ? value1.z : value2.z;
            result.w = (value1.w > value2.w) ? value1.w : value2.w;
        }
        #endregion

        /// <summary>
        /// 将当前向量的所有分量设置为零
        /// </summary>
        #region public void MakeZero()
        public void MakeZero()
        {
            x = FP.Zero;
            y = FP.Zero;
            z = FP.Zero;
            w = FP.Zero;
        }
        #endregion

        /// <summary>
        /// 检查当前向量是否为零向量（所有分量都为零）
        /// </summary>
        /// <returns>如果是零向量则返回true，否则返回false</returns>
        #region public bool IsZero()
        public bool IsZero()
        {
            return (this.sqrMagnitude == FP.Zero);
        }

        /// <summary>
        /// 检查当前向量是否接近零向量（平方长度小于阈值）
        /// 用于处理浮点数精度问题导致的近似零向量判断
        /// </summary>
        /// <returns>如果接近零向量则返回true，否则返回false</returns>
        public bool IsNearlyZero()
        {
            return (this.sqrMagnitude < ZeroEpsilonSq);
        }
        #endregion

        /// <summary>
        /// 使用4x4矩阵变换向量（齐次坐标变换）
        /// </summary>
        /// <param name="position">要变换的向量</param>
        /// <param name="matrix">变换矩阵</param>
        /// <returns>变换后的新向量</returns>
        #region public static TSVector4 Transform(TSVector4 position, TSMatrix4x4 matrix)
        public static TSVector4 Transform(TSVector4 position, TSMatrix4x4 matrix)
        {
            TSVector4 result;
            TSVector4.Transform(ref position, ref matrix, out result);
            return result;
        }

        /// <summary>
        /// 使用4x4矩阵变换三维向量（自动扩展为齐次坐标）
        /// </summary>
        /// <param name="position">要变换的三维向量</param>
        /// <param name="matrix">变换矩阵</param>
        /// <returns>变换后的四维向量</returns>
        public static TSVector4 Transform(TSVector position, TSMatrix4x4 matrix)
        {
            TSVector4 result;
            TSVector4.Transform(ref position, ref matrix, out result);
            return result;
        }

        /// <summary>
        /// 使用4x4矩阵变换三维向量（引用传递版本）
        /// 内部将三维向量视为齐次坐标(x,y,z,1)进行变换
        /// </summary>
        /// <param name="vector">要变换的三维向量（引用传递）</param>
        /// <param name="matrix">变换矩阵（引用传递）</param>
        /// <param name="result">输出参数，存储变换后的四维向量</param>
        public static void Transform(ref TSVector vector, ref TSMatrix4x4 matrix, out TSVector4 result)
        {
            result.x = vector.x * matrix.M11 + vector.y * matrix.M12 + vector.z * matrix.M13 + matrix.M14;
            result.y = vector.x * matrix.M21 + vector.y * matrix.M22 + vector.z * matrix.M23 + matrix.M24;
            result.z = vector.x * matrix.M31 + vector.y * matrix.M32 + vector.z * matrix.M33 + matrix.M34;
            result.w = vector.x * matrix.M41 + vector.y * matrix.M42 + vector.z * matrix.M43 + matrix.M44;
        }

        /// <summary>
        /// 使用4x4矩阵变换四维向量（引用传递版本）
        /// 完全按照矩阵乘法规则进行计算
        /// </summary>
        /// <param name="vector">要变换的四维向量（引用传递）</param>
        /// <param name="matrix">变换矩阵（引用传递）</param>
        /// <param name="result">输出参数，存储变换后的四维向量</param>
        public static void Transform(ref TSVector4 vector, ref TSMatrix4x4 matrix, out TSVector4 result)
        {
            result.x = vector.x * matrix.M11 + vector.y * matrix.M12 + vector.z * matrix.M13 + vector.w * matrix.M14;
            result.y = vector.x * matrix.M21 + vector.y * matrix.M22 + vector.z * matrix.M23 + vector.w * matrix.M24;
            result.z = vector.x * matrix.M31 + vector.y * matrix.M32 + vector.z * matrix.M33 + vector.w * matrix.M34;
            result.w = vector.x * matrix.M41 + vector.y * matrix.M42 + vector.z * matrix.M43 + vector.w * matrix.M44;
        }
        #endregion

        /// <summary>
        /// 计算两个四维向量的点积（内积）
        /// 点积公式：x1*x2 + y1*y2 + z1*z2 + w1*w2
        /// </summary>
        /// <param name="vector1">第一个向量</param>
        /// <param name="vector2">第二个向量</param>
        /// <returns>两个向量的点积</returns>
        #region public static FP Dot(TSVector4 vector1, TSVector4 vector2)
        public static FP Dot(TSVector4 vector1, TSVector4 vector2)
        {
            return TSVector4.Dot(ref vector1, ref vector2);
        }

        /// <summary>
        /// 计算两个四维向量的点积（引用传递版本，性能更优）
        /// </summary>
        /// <param name="vector1">第一个向量（引用传递）</param>
        /// <param name="vector2">第二个向量（引用传递）</param>
        /// <returns>两个向量的点积</returns>
        public static FP Dot(ref TSVector4 vector1, ref TSVector4 vector2)
        {
            return (vector1.x * vector2.x) + (vector1.y * vector2.y) + 
                   (vector1.z * vector2.z) + (vector1.w * vector2.w);
        }
        #endregion

        /// <summary>
        /// 两个向量相加
        /// 结果向量的各分量为两个输入向量对应分量之和
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>相加后的新向量</returns>
        #region public static TSVector4 Add(TSVector4 value1, TSVector4 value2)
        public static TSVector4 Add(TSVector4 value1, TSVector4 value2)
        {
            TSVector4 result;
            TSVector4.Add(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 两个向量相加（引用传递版本，性能更优）
        /// </summary>
        /// <param name="value1">第一个向量（引用传递）</param>
        /// <param name="value2">第二个向量（引用传递）</param>
        /// <param name="result">输出参数，存储相加后的向量</param>
        public static void Add(ref TSVector4 value1, ref TSVector4 value2, out TSVector4 result)
        {
            result.x = value1.x + value2.x;
            result.y = value1.y + value2.y;
            result.z = value1.z + value2.z;
            result.w = value1.w + value2.w;
        }
        #endregion

        /// <summary>
        /// 向量除以一个标量
        /// 结果向量的各分量为原向量对应分量除以标量
        /// </summary>
        /// <param name="value1">要除法的向量</param>
        /// <param name="scaleFactor">除数（标量）</param>
        /// <returns>除法后的新向量</returns>
        public static TSVector4 Divide(TSVector4 value1, FP scaleFactor)
        {
            TSVector4 result;
            TSVector4.Divide(ref value1, scaleFactor, out result);
            return result;
        }

        /// <summary>
        /// 向量除以一个标量（引用传递版本，性能更优）
        /// </summary>
        /// <param name="value1">要除法的向量（引用传递）</param>
        /// <param name="scaleFactor">除数（标量）</param>
        /// <param name="result">输出参数，存储除法后的向量</param>
        public static void Divide(ref TSVector4 value1, FP scaleFactor, out TSVector4 result)
        {
            result.x = value1.x / scaleFactor;
            result.y = value1.y / scaleFactor;
            result.z = value1.z / scaleFactor;
            result.w = value1.w / scaleFactor;
        }

        /// <summary>
        /// 两个向量相减
        /// 结果向量的各分量为第一个向量减去第二个向量的对应分量
        /// </summary>
        /// <param name="value1">被减向量</param>
        /// <param name="value2">减向量</param>
        /// <returns>相减后的新向量</returns>
        #region public static TSVector4 Subtract(TSVector4 value1, TSVector4 value2)
        public static TSVector4 Subtract(TSVector4 value1, TSVector4 value2)
        {
            TSVector4 result;
            TSVector4.Subtract(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 两个向量相减（引用传递版本，性能更优）
        /// </summary>
        /// <param name="value1">被减向量（引用传递）</param>
        /// <param name="value2">减向量（引用传递）</param>
        /// <param name="result">输出参数，存储相减后的向量</param>
        public static void Subtract(ref TSVector4 value1, ref TSVector4 value2, out TSVector4 result)
        {
            result.x = value1.x - value2.x;
            result.y = value1.y - value2.y;
            result.z = value1.z - value2.z;
            result.w = value1.w - value2.w;
        }
        #endregion

        /// <summary>
        /// 获取当前向量的哈希码
        /// 用于哈希表等数据结构中快速查找
        /// </summary>
        /// <returns>向量的哈希码</returns>
        #region public override int GetHashCode()
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }
        #endregion

        /// <summary>
        /// 反转当前向量的方向（取反）
        /// 所有分量变为其相反数
        /// </summary>
        #region public void Negate()
        public void Negate()
        {
            this.x = -this.x;
            this.y = -this.y;
            this.z = -this.z;
            this.w = -this.w;
        }

        /// <summary>
        /// 反转指定向量的方向（取反），返回新向量
        /// </summary>
        /// <param name="value">要反转方向的向量</param>
        /// <returns>方向反转后的新向量</returns>
        public static TSVector4 Negate(TSVector4 value)
        {
            TSVector4 result;
            TSVector4.Negate(ref value, out result);
            return result;
        }

        /// <summary>
        /// 反转指定向量的方向（引用传递版本，性能更优）
        /// </summary>
        /// <param name="value">要反转方向的向量（引用传递）</param>
        /// <param name="result">输出参数，存储方向反转后的向量</param>
        public static void Negate(ref TSVector4 value, out TSVector4 result)
        {
            result.x = -value.x;
            result.y = -value.y;
            result.z = -value.z;
            result.w = -value.w;
        }
        #endregion

        /// <summary>
        /// 归一化指定向量（单位化）
        /// 单位向量与原向量方向相同，但长度为1
        /// </summary>
        /// <param name="value">要归一化的向量</param>
        /// <returns>归一化后的新向量</returns>
        #region public static TSVector4 Normalize(TSVector4 value)
        public static TSVector4 Normalize(TSVector4 value)
        {
            TSVector4 result;
            TSVector4.Normalize(ref value, out result);
            return result;
        }

        /// <summary>
        /// 归一化当前向量（单位化）
        /// 直接修改当前向量，使其长度为1
        /// </summary>
        public void Normalize()
        {
            FP sqrMag = (x * x) + (y * y) + (z * z) + (w * w);
            FP invSqrt = FP.One / FP.Sqrt(sqrMag); // 计算平方根的倒数，比除法更高效
            x *= invSqrt;
            y *= invSqrt;
            z *= invSqrt;
            w *= invSqrt;
        }

        /// <summary>
        /// 归一化指定向量（引用传递版本，性能更优）
        /// </summary>
        /// <param name="value">要归一化的向量（引用传递）</param>
        /// <param name="result">输出参数，存储归一化后的向量</param>
        public static void Normalize(ref TSVector4 value, out TSVector4 result)
        {
            FP sqrMag = (value.x * value.x) + (value.y * value.y) + 
                       (value.z * value.z) + (value.w * value.w);
            FP invSqrt = FP.One / FP.Sqrt(sqrMag);
            result.x = value.x * invSqrt;
            result.y = value.y * invSqrt;
            result.z = value.z * invSqrt;
            result.w = value.w * invSqrt;
        }
        #endregion

        #region public static void Swap(ref TSVector4 vector1, ref TSVector4 vector2)
        /// <summary>
        /// 交换两个向量的所有分量
        /// 交换后vector1包含vector2的原始值，vector2包含vector1的原始值
        /// </summary>
        /// <param name="vector1">第一个要交换的向量（引用传递）</param>
        /// <param name="vector2">第二个要交换的向量（引用传递）</param>
        public static void Swap(ref TSVector4 vector1, ref TSVector4 vector2)
        {
            FP temp; // 临时变量用于交换

            temp = vector1.x;
            vector1.x = vector2.x;
            vector2.x = temp;

            temp = vector1.y;
            vector1.y = vector2.y;
            vector2.y = temp;

            temp = vector1.z;
            vector1.z = vector2.z;
            vector2.z = temp;

            temp = vector1.w;
            vector1.w = vector2.w;
            vector2.w = temp;
        }
        #endregion

        /// <summary>
        /// 向量与标量相乘
        /// 结果向量的各分量为原向量对应分量乘以标量
        /// </summary>
        /// <param name="value1">要相乘的向量</param>
        /// <param name="scaleFactor">乘数（标量）</param>
        /// <returns>相乘后的新向量</returns>
        #region public static TSVector4 Multiply(TSVector4 value1, FP scaleFactor)
        public static TSVector4 Multiply(TSVector4 value1, FP scaleFactor)
        {
            TSVector4 result;
            TSVector4.Multiply(ref value1, scaleFactor, out result);
            return result;
        }

        /// <summary>
        /// 向量与标量相乘（引用传递版本，性能更优）
        /// </summary>
        /// <param name="value1">要相乘的向量（引用传递）</param>
        /// <param name="scaleFactor">乘数（标量）</param>
        /// <param name="result">输出参数，存储相乘后的向量</param>
        public static void Multiply(ref TSVector4 value1, FP scaleFactor, out TSVector4 result)
        {
            result.x = value1.x * scaleFactor;
            result.y = value1.y * scaleFactor;
            result.z = value1.z * scaleFactor;
            result.w = value1.w * scaleFactor;
        }
        #endregion

        /// <summary>
        /// 重载乘法运算符，实现两个向量的点积计算
        /// 等价于Dot方法
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>两个向量的点积</returns>
        #region public static FP operator *(TSVector4 value1, TSVector4 value2)
        public static FP operator *(TSVector4 value1, TSVector4 value2)
        {
            return TSVector4.Dot(ref value1, ref value2);
        }
        #endregion

        /// <summary>
        /// 重载乘法运算符，实现向量与标量相乘
        /// 等价于Multiply方法
        /// </summary>
        /// <param name="value1">要相乘的向量</param>
        /// <param name="value2">乘数（标量）</param>
        /// <returns>相乘后的新向量</returns>
        #region public static TSVector4 operator *(TSVector4 value1, FP value2)
        public static TSVector4 operator *(TSVector4 value1, FP value2)
        {
            TSVector4 result;
            TSVector4.Multiply(ref value1, value2, out result);
            return result;
        }
        #endregion

        /// <summary>
        /// 重载乘法运算符，实现标量与向量相乘（顺序颠倒）
        /// 结果与向量*标量相同
        /// </summary>
        /// <param name="value1">乘数（标量）</param>
        /// <param name="value2">要相乘的向量</param>
        /// <returns>相乘后的新向量</returns>
        #region public static TSVector4 operator *(FP value1, TSVector4 value2)
        public static TSVector4 operator *(FP value1, TSVector4 value2)
        {
            TSVector4 result;
            TSVector4.Multiply(ref value2, value1, out result);
            return result;
        }
        #endregion

        /// <summary>
        /// 重载减法运算符，实现两个向量相减
        /// 等价于Subtract方法
        /// </summary>
        /// <param name="value1">被减向量</param>
        /// <param name="value2">减向量</param>
        /// <returns>相减后的新向量</returns>
        #region public static TSVector4 operator -(TSVector4 value1, TSVector4 value2)
        public static TSVector4 operator -(TSVector4 value1, TSVector4 value2)
        {
            TSVector4 result; 
            TSVector4.Subtract(ref value1, ref value2, out result);
            return result;
        }
        #endregion

        /// <summary>
        /// 重载加法运算符，实现两个向量相加
        /// 等价于Add方法
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>相加后的新向量</returns>
        #region public static TSVector4 operator +(TSVector4 value1, TSVector4 value2)
        public static TSVector4 operator +(TSVector4 value1, TSVector4 value2)
        {
            TSVector4 result; 
            TSVector4.Add(ref value1, ref value2, out result);
            return result;
        }
        #endregion

        /// <summary>
        /// 重载除法运算符，实现向量除以标量
        /// 等价于Divide方法
        /// </summary>
        /// <param name="value1">要除法的向量</param>
        /// <param name="value2">除数（标量）</param>
        /// <returns>除法后的新向量</returns>
        public static TSVector4 operator /(TSVector4 value1, FP value2)
        {
            TSVector4 result;
            TSVector4.Divide(ref value1, value2, out result);
            return result;
        }

        /// <summary>
        /// 将当前四维向量转换为二维向量TSVector2
        /// 仅保留X和Y分量
        /// </summary>
        /// <returns>包含X和Y分量的二维向量</returns>
        public TSVector2 ToTSVector2()
        {
            return new TSVector2(this.x, this.y);
        }

        /// <summary>
        /// 将当前四维向量转换为三维向量TSVector
        /// 仅保留X、Y和Z分量
        /// </summary>
        /// <returns>包含X、Y和Z分量的三维向量</returns>
        public TSVector ToTSVector()
        {
            return new TSVector(this.x, this.y, this.z);
        }
    }
}