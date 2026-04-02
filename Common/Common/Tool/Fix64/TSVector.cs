/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
 *
 *  本软件按"原样"提供，不附带任何明示或暗示的担保。
 *  作者不对因使用本软件而产生的任何损害承担责任。
 *
 *  允许任何人将本软件用于任何目的，包括商业应用，并可自由修改和重新分发，
 *  但需遵守以下限制：
 *
 *  1. 不得歪曲本软件的来源；不得声称您编写了原始软件。
 *     如果您在产品中使用本软件，在产品文档中致谢会受到赞赏，但非必需。
 *  2. 修改后的源版本必须明确标记为修改版，不得歪曲为原始软件。
 *  3. 本声明不得从任何源分发中删除或修改。
 */

using System;

namespace Start
{
    /// <summary>
    /// 三维向量结构，用于表示三维空间中的点或方向
    /// 提供了向量的各种数学运算和转换方法
    /// </summary>
    [Serializable] // 允许该结构被序列化
    public struct TSVector
    {
        // 内部静态变量，用于零向量判断的阈值平方值
        private static FP ZeroEpsilonSq = TSMath.Epsilon;

        // 内部零向量实例
        internal static TSVector InternalZero;

        // 内部任意向量实例（初始化为(1,1,1)）
        internal static TSVector Arbitrary;

        /// <summary>向量的X分量</summary>
        public FP X;

        /// <summary>向量的Y分量</summary>
        public FP Y;

        /// <summary>向量的Z分量</summary>
        public FP Z;

        #region 静态只读向量常量

        /// <summary>
        /// 零向量，分量为(0,0,0)
        /// </summary>
        public static readonly TSVector Zero;

        /// <summary>
        /// 左方向向量，分量为(-1,0,0)
        /// </summary>
        public static readonly TSVector Left;

        /// <summary>
        /// 右方向向量，分量为(1,0,0)
        /// </summary>
        public static readonly TSVector Right;

        /// <summary>
        /// 上方向向量，分量为(0,1,0)
        /// </summary>
        public static readonly TSVector Up;

        /// <summary>
        /// 下方向向量，分量为(0,-1,0)
        /// </summary>
        public static readonly TSVector Down;

        /// <summary>
        /// 后方向向量，分量为(0,0,-1)
        /// </summary>
        public static readonly TSVector Back;

        /// <summary>
        /// 前方向向量，分量为(0,0,1)
        /// </summary>
        public static readonly TSVector Forward;

        /// <summary>
        /// 全1向量，分量为(1,1,1)
        /// </summary>
        public static readonly TSVector One;

        /// <summary>
        /// 最小值向量，各分量为FP类型的最小值
        /// </summary>
        public static readonly TSVector MinValue;

        /// <summary>
        /// 最大值向量，各分量为FP类型的最大值
        /// </summary>
        public static readonly TSVector MaxValue;

        #endregion

        #region 静态构造函数

        /// <summary>
        /// 静态构造函数，初始化所有静态只读向量常量
        /// </summary>
        static TSVector()
        {
            One = new TSVector(1, 1, 1);
            Zero = new TSVector(0, 0, 0);
            Left = new TSVector(-1, 0, 0);
            Right = new TSVector(1, 0, 0);
            Up = new TSVector(0, 1, 0);
            Down = new TSVector(0, -1, 0);
            Back = new TSVector(0, 0, -1);
            Forward = new TSVector(0, 0, 1);
            MinValue = new TSVector(FP.MinValue);
            MaxValue = new TSVector(FP.MaxValue);
            Arbitrary = new TSVector(1, 1, 1);
            InternalZero = Zero;
        }

        #endregion

        /// <summary>
        /// 计算向量各分量的绝对值
        /// </summary>
        /// <param name="other">输入向量</param>
        /// <returns>各分量取绝对值后的新向量</returns>
        public static TSVector Abs(TSVector other)
        {
            return new TSVector(FP.Abs(other.X), FP.Abs(other.Y), FP.Abs(other.Z));
        }

        /// <summary>
        /// 获取向量的平方长度（模长的平方）
        /// 相比计算实际长度，平方长度计算更高效，适用于比较向量长度
        /// </summary>
        /// <returns>向量的平方长度</returns>
        public FP sqrMagnitude
        {
            get { return (((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z)); }
        }

        /// <summary>
        /// 获取向量的长度（模长）
        /// 通过计算平方长度的平方根得到
        /// </summary>
        /// <returns>向量的长度</returns>
        public FP magnitude
        {
            get
            {
                FP num = ((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z);
                return FP.Sqrt(num);
            }
        }

        /// <summary>
        /// 将向量的长度限制在指定的最大值内
        /// 若向量长度超过maxLength，则按比例缩小至maxLength
        /// </summary>
        /// <param name="vector">需要限制长度的向量</param>
        /// <param name="maxLength">最大长度限制</param>
        /// <returns>长度被限制后的新向量</returns>
        public static TSVector ClampMagnitude(TSVector vector, FP maxLength)
        {
            return Normalize(vector) * maxLength;
        }

        /// <summary>
        /// 获取当前向量的归一化（单位向量）版本
        /// 单位向量与原向量方向相同，长度为1
        /// </summary>
        /// <returns>归一化后的新向量</returns>
        public TSVector normalized
        {
            get
            {
                TSVector result = new TSVector(this.X, this.Y, this.Z);
                result.Normalize();
                return result;
            }
        }

        #region 构造函数

        /// <summary>
        /// 使用整数分量初始化向量实例
        /// </summary>
        /// <param name="x">X分量</param>
        /// <param name="y">Y分量</param>
        /// <param name="z">Z分量</param>
        public TSVector(int x, int y, int z)
        {
            this.X = (FP)x;
            this.Y = (FP)y;
            this.Z = (FP)z;
        }

        /// <summary>
        /// 使用FP类型分量初始化向量实例
        /// </summary>
        /// <param name="x">X分量</param>
        /// <param name="y">Y分量</param>
        /// <param name="z">Z分量</param>
        public TSVector(FP x, FP y, FP z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// 使用单个FP值初始化向量，所有分量均设为该值
        /// </summary>
        /// <param name="xyz">用于初始化所有分量的值</param>
        public TSVector(FP xyz)
        {
            this.X = xyz;
            this.Y = xyz;
            this.Z = xyz;
        }

        #endregion

        /// <summary>
        /// 按分量缩放当前向量（与另一个向量的分量相乘）
        /// 会修改当前向量实例
        /// </summary>
        /// <param name="other">用于缩放的向量</param>
        public void Scale(TSVector other)
        {
            this.X = X * other.X;
            this.Y = Y * other.Y;
            this.Z = Z * other.Z;
        }

        /// <summary>
        /// 设置当前向量的所有分量值
        /// 会修改当前向量实例
        /// </summary>
        /// <param name="x">新的X分量</param>
        /// <param name="y">新的Y分量</param>
        /// <param name="z">新的Z分量</param>
        public void Set(FP x, FP y, FP z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// 在两个向量之间进行线性插值
        /// </summary>
        /// <param name="from">起始向量</param>
        /// <param name="to">目标向量</param>
        /// <param name="percent">插值比例（0~1之间）</param>
        /// <returns>插值结果向量</returns>
        public static TSVector Lerp(TSVector from, TSVector to, FP percent)
        {
            return from + (to - from) * percent;
        }

        /// <summary>
        /// 将向量转换为字符串表示形式
        /// 格式为：(x值, y值, z值)，保留一位小数
        /// </summary>
        /// <returns>向量的字符串表示</returns>
        public override string ToString()
        {
            return string.Format("({0:f1}, {1:f1}, {2:f1})", X.AsFloat(), Y.AsFloat(), Z.AsFloat());
        }

        /// <summary>
        /// 检查当前向量是否与指定对象相等
        /// 只有当对象是TSVector且所有分量都相等时才返回true
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is TSVector)) return false;
            TSVector other = (TSVector)obj;
            return (((X == other.X) && (Y == other.Y)) && (Z == other.Z));
        }

        /// <summary>
        /// 按分量缩放向量（两个向量的分量相乘）
        /// 返回新的缩放结果向量，不修改原向量
        /// </summary>
        /// <param name="vecA">第一个向量</param>
        /// <param name="vecB">第二个向量</param>
        /// <returns>分量相乘后的新向量</returns>
        public static TSVector Scale(TSVector vecA, TSVector vecB)
        {
            TSVector result;
            result.X = vecA.X * vecB.X;
            result.Y = vecA.Y * vecB.Y;
            result.Z = vecA.Z * vecB.Z;
            return result;
        }

        /// <summary>
        /// 重载相等运算符，判断两个向量是否相等
        /// 所有分量都相等时返回true
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>是否相等</returns>
        public static bool operator ==(TSVector value1, TSVector value2)
        {
            return (((value1.X == value2.X) && (value1.Y == value2.Y)) && (value1.Z == value2.Z));
        }

        /// <summary>
        /// 重载不等运算符，判断两个向量是否不相等
        /// 任何一个分量不相等时返回true
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>是否不相等</returns>
        public static bool operator !=(TSVector value1, TSVector value2)
        {
            if ((value1.X == value2.X) && (value1.Y == value2.Y))
            {
                return (value1.Z != value2.Z);
            }

            return true;
        }

        #region 最小/最大向量计算

        /// <summary>
        /// 获取两个向量的分量级最小值向量
        /// 每个分量取两个向量对应分量中的较小值
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>分量级最小值向量</returns>
        public static TSVector Min(TSVector value1, TSVector value2)
        {
            TSVector result;
            TSVector.Min(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 获取两个向量的分量级最小值向量（引用传递版本）
        /// 每个分量取两个向量对应分量中的较小值
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <param name="result">输出的分量级最小值向量</param>
        public static void Min(ref TSVector value1, ref TSVector value2, out TSVector result)
        {
            result.X = (value1.X < value2.X) ? value1.X : value2.X;
            result.Y = (value1.Y < value2.Y) ? value1.Y : value2.Y;
            result.Z = (value1.Z < value2.Z) ? value1.Z : value2.Z;
        }

        /// <summary>
        /// 获取两个向量的分量级最大值向量
        /// 每个分量取两个向量对应分量中的较大值
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>分量级最大值向量</returns>
        public static TSVector Max(TSVector value1, TSVector value2)
        {
            TSVector result;
            TSVector.Max(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 计算两个点（向量）之间的欧氏距离
        /// </summary>
        /// <param name="v1">第一个点</param>
        /// <param name="v2">第二个点</param>
        /// <returns>两点之间的距离</returns>
        public static FP Distance(TSVector v1, TSVector v2)
        {
            return FP.Sqrt(
                (v1.X - v2.X) * (v1.X - v2.X) + (v1.Y - v2.Y) * (v1.Y - v2.Y) + (v1.Z - v2.Z) * (v1.Z - v2.Z));
        }

        /// <summary>
        /// 获取两个向量的分量级最大值向量（引用传递版本）
        /// 每个分量取两个向量对应分量中的较大值
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <param name="result">输出的分量级最大值向量</param>
        public static void Max(ref TSVector value1, ref TSVector value2, out TSVector result)
        {
            result.X = (value1.X > value2.X) ? value1.X : value2.X;
            result.Y = (value1.Y > value2.Y) ? value1.Y : value2.Y;
            result.Z = (value1.Z > value2.Z) ? value1.Z : value2.Z;
        }

        #endregion

        /// <summary>
        /// 将当前向量设为零向量（所有分量置为0）
        /// 会修改当前向量实例
        /// </summary>
        public void MakeZero()
        {
            X = FP.Zero;
            Y = FP.Zero;
            Z = FP.Zero;
        }

        /// <summary>
        /// 检查当前向量是否为零向量（所有分量严格为0）
        /// </summary>
        /// <returns>是否为零向量</returns>
        public bool IsZero()
        {
            return (this.sqrMagnitude == FP.Zero);
        }

        /// <summary>
        /// 检查当前向量是否接近零向量（平方长度小于阈值）
        /// 用于处理浮点数精度问题
        /// </summary>
        /// <returns>是否接近零向量</returns>
        public bool IsNearlyZero()
        {
            return (this.sqrMagnitude < ZeroEpsilonSq);
        }

        #region 矩阵变换

        /// <summary>
        /// 使用矩阵变换向量
        /// </summary>
        /// <param name="position">要变换的向量</param>
        /// <param name="matrix">变换矩阵</param>
        /// <returns>变换后的向量</returns>
        public static TSVector Transform(TSVector position, TSMatrix matrix)
        {
            TSVector result;
            TSVector.Transform(ref position, ref matrix, out result);
            return result;
        }

        /// <summary>
        /// 使用矩阵变换向量（引用传递版本）
        /// 通过矩阵乘法实现向量变换
        /// </summary>
        /// <param name="position">要变换的向量</param>
        /// <param name="matrix">变换矩阵</param>
        /// <param name="result">变换后的向量</param>
        public static void Transform(ref TSVector position, ref TSMatrix matrix, out TSVector result)
        {
            FP num0 = ((position.X * matrix.M11) + (position.Y * matrix.M21)) + (position.Z * matrix.M31);
            FP num1 = ((position.X * matrix.M12) + (position.Y * matrix.M22)) + (position.Z * matrix.M32);
            FP num2 = ((position.X * matrix.M13) + (position.Y * matrix.M23)) + (position.Z * matrix.M33);

            result.X = num0;
            result.Y = num1;
            result.Z = num2;
        }

        /// <summary>
        /// 使用矩阵的转置矩阵变换向量（引用传递版本）
        /// 用于特定的坐标变换场景
        /// </summary>
        /// <param name="position">要变换的向量</param>
        /// <param name="matrix">变换矩阵</param>
        /// <param name="result">变换后的向量</param>
        public static void TransposedTransform(ref TSVector position, ref TSMatrix matrix, out TSVector result)
        {
            FP num0 = ((position.X * matrix.M11) + (position.Y * matrix.M12)) + (position.Z * matrix.M13);
            FP num1 = ((position.X * matrix.M21) + (position.Y * matrix.M22)) + (position.Z * matrix.M23);
            FP num2 = ((position.X * matrix.M31) + (position.Y * matrix.M32)) + (position.Z * matrix.M33);

            result.X = num0;
            result.Y = num1;
            result.Z = num2;
        }

        #endregion

        #region 点积计算

        /// <summary>
        /// 计算两个向量的点积（内积）
        /// 点积公式：v1·v2 = v1.x*v2.x + v1.y*v2.y + v1.z*v2.z
        /// </summary>
        /// <param name="vector1">第一个向量</param>
        /// <param name="vector2">第二个向量</param>
        /// <returns>点积结果</returns>
        public static FP Dot(TSVector vector1, TSVector vector2)
        {
            return TSVector.Dot(ref vector1, ref vector2);
        }

        /// <summary>
        /// 计算两个向量的点积（引用传递版本）
        /// </summary>
        /// <param name="vector1">第一个向量</param>
        /// <param name="vector2">第二个向量</param>
        /// <returns>点积结果</returns>
        public static FP Dot(ref TSVector vector1, ref TSVector vector2)
        {
            return ((vector1.X * vector2.X) + (vector1.Y * vector2.Y)) + (vector1.Z * vector2.Z);
        }

        #endregion

        /// <summary>
        /// 将一个向量投影到另一个向量上
        /// 投影结果是与被投影向量同方向的向量
        /// </summary>
        /// <param name="vector">要投影的向量</param>
        /// <param name="onNormal">被投影到的向量（法线方向）</param>
        /// <returns>投影后的向量</returns>
        public static TSVector Project(TSVector vector, TSVector onNormal)
        {
            FP sqrtMag = Dot(onNormal, onNormal);
            if (sqrtMag < TSMath.Epsilon)
                return Zero;
            else
                return onNormal * Dot(vector, onNormal) / sqrtMag;
        }

        /// <summary>
        /// 将向量投影到由法向量定义的平面上
        /// 投影结果是向量在平面上的分量
        /// </summary>
        /// <param name="vector">要投影的向量</param>
        /// <param name="planeNormal">平面的法向量</param>
        /// <returns>投影到平面上的向量</returns>
        public static TSVector ProjectOnPlane(TSVector vector, TSVector planeNormal)
        {
            return vector - Project(vector, planeNormal);
        }

        /// <summary>
        /// 计算两个向量之间的夹角（角度制，0~180度）
        /// 通过点积公式计算：cosθ = (v1·v2)/(|v1||v2|)
        /// </summary>
        /// <param name="from">起始向量</param>
        /// <param name="to">目标向量</param>
        /// <returns>夹角角度（度）</returns>
        public static FP Angle(TSVector from, TSVector to)
        {
            return TSMath.Acos(TSMath.Clamp(Dot(from.normalized, to.normalized), -FP.ONE, FP.ONE)) * TSMath.Rad2Deg;
        }

        /// <summary>
        /// 计算两个向量之间的有符号夹角（角度制）
        /// 角度范围为-180~180度，符号由旋转方向决定
        /// </summary>
        /// <param name="from">起始向量</param>
        /// <param name="to">目标向量</param>
        /// <param name="axis">参考轴（用于确定旋转方向）</param>
        /// <returns>有符号夹角角度（度）</returns>
        public static FP SignedAngle(TSVector from, TSVector to, TSVector axis)
        {
            TSVector fromNorm = from.normalized, toNorm = to.normalized;
            FP unsignedAngle = TSMath.Acos(TSMath.Clamp(Dot(fromNorm, toNorm), -FP.ONE, FP.ONE)) * TSMath.Rad2Deg;
            FP sign = TSMath.Sign(Dot(axis, Cross(fromNorm, toNorm)));
            return unsignedAngle * sign;
        }

        #region 向量加法

        /// <summary>
        /// 计算两个向量的和
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>向量和</returns>
        public static TSVector Add(TSVector value1, TSVector value2)
        {
            TSVector result;
            TSVector.Add(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 计算两个向量的和（引用传递版本）
        /// 分量级相加：result.x = v1.x + v2.x，以此类推
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <param name="result">向量和</param>
        public static void Add(ref TSVector value1, ref TSVector value2, out TSVector result)
        {
            FP num0 = value1.X + value2.X;
            FP num1 = value1.Y + value2.Y;
            FP num2 = value1.Z + value2.Z;

            result.X = num0;
            result.Y = num1;
            result.Z = num2;
        }

        #endregion

        #region 向量除法

        /// <summary>
        /// 将向量除以一个标量（缩放）
        /// </summary>
        /// <param name="value1">要除法的向量</param>
        /// <param name="scaleFactor">除数（标量）</param>
        /// <returns>除法结果向量</returns>
        public static TSVector Divide(TSVector value1, FP scaleFactor)
        {
            TSVector result;
            TSVector.Divide(ref value1, scaleFactor, out result);
            return result;
        }

        /// <summary>
        /// 将向量除以一个标量（引用传递版本）
        /// 分量级相除：result.x = v.x / scaleFactor，以此类推
        /// </summary>
        /// <param name="value1">要除法的向量</param>
        /// <param name="scaleFactor">除数（标量）</param>
        /// <param name="result">除法结果向量</param>
        public static void Divide(ref TSVector value1, FP scaleFactor, out TSVector result)
        {
            result.X = value1.X / scaleFactor;
            result.Y = value1.Y / scaleFactor;
            result.Z = value1.Z / scaleFactor;
        }

        #endregion

        #region 向量减法

        /// <summary>
        /// 计算两个向量的差（v1 - v2）
        /// </summary>
        /// <param name="value1">被减向量</param>
        /// <param name="value2">减向量</param>
        /// <returns>向量差</returns>
        public static TSVector Subtract(TSVector value1, TSVector value2)
        {
            TSVector result;
            TSVector.Subtract(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 计算两个向量的差（引用传递版本）
        /// 分量级相减：result.x = v1.x - v2.x，以此类推
        /// </summary>
        /// <param name="value1">被减向量</param>
        /// <param name="value2">减向量</param>
        /// <param name="result">向量差</param>
        public static void Subtract(ref TSVector value1, ref TSVector value2, out TSVector result)
        {
            FP num0 = value1.X - value2.X;
            FP num1 = value1.Y - value2.Y;
            FP num2 = value1.Z - value2.Z;

            result.X = num0;
            result.Y = num1;
            result.Z = num2;
        }

        #endregion

        #region 叉积计算

        /// <summary>
        /// 计算两个向量的叉积（外积）
        /// 叉积结果是与两个输入向量都垂直的向量
        /// 公式：v1×v2 = (v1.y*v2.z - v1.z*v2.y, v1.z*v2.x - v1.x*v2.z, v1.x*v2.y - v1.y*v2.x)
        /// </summary>
        /// <param name="vector1">第一个向量</param>
        /// <param name="vector2">第二个向量</param>
        /// <returns>叉积结果向量</returns>
        public static TSVector Cross(TSVector vector1, TSVector vector2)
        {
            TSVector result;
            TSVector.Cross(ref vector1, ref vector2, out result);
            return result;
        }

        /// <summary>
        /// 计算两个向量的叉积（引用传递版本）
        /// </summary>
        /// <param name="vector1">第一个向量</param>
        /// <param name="vector2">第二个向量</param>
        /// <param name="result">叉积结果向量</param>
        public static void Cross(ref TSVector vector1, ref TSVector vector2, out TSVector result)
        {
            FP num3 = (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y);
            FP num2 = (vector1.Z * vector2.X) - (vector1.X * vector2.Z);
            FP num = (vector1.X * vector2.Y) - (vector1.Y * vector2.X);
            result.X = num3;
            result.Y = num2;
            result.Z = num;
        }

        #endregion

        /// <summary>
        /// 获取当前向量的哈希码
        /// 由各分量的哈希码组合而成
        /// </summary>
        /// <returns>哈希码</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        #region 向量取反

        /// <summary>
        /// 反转当前向量的方向（所有分量取相反数）
        /// 会修改当前向量实例
        /// </summary>
        public void Negate()
        {
            this.X = -this.X;
            this.Y = -this.Y;
            this.Z = -this.Z;
        }

        /// <summary>
        /// 反转向量的方向，返回新的向量
        /// </summary>
        /// <param name="value">要反转的向量</param>
        /// <returns>反转后的新向量</returns>
        public static TSVector Negate(TSVector value)
        {
            TSVector result;
            TSVector.Negate(ref value, out result);
            return result;
        }

        /// <summary>
        /// 反转向量的方向（引用传递版本）
        /// </summary>
        /// <param name="value">要反转的向量</param>
        /// <param name="result">反转后的向量</param>
        public static void Negate(ref TSVector value, out TSVector result)
        {
            FP num0 = -value.X;
            FP num1 = -value.Y;
            FP num2 = -value.Z;

            result.X = num0;
            result.Y = num1;
            result.Z = num2;
        }

        #endregion

        #region 向量归一化

        /// <summary>
        /// 将向量归一化（单位化），返回新的单位向量
        /// 单位向量长度为1，方向与原向量相同
        /// </summary>
        /// <param name="value">要归一化的向量</param>
        /// <returns>归一化后的单位向量</returns>
        public static TSVector Normalize(TSVector value)
        {
            TSVector result;
            TSVector.Normalize(ref value, out result);
            return result;
        }

        /// <summary>
        /// 归一化当前向量（单位化）
        /// 会修改当前向量实例，使其长度为1
        /// </summary>
        public void Normalize()
        {
            FP num2 = ((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z);
            FP num = FP.One / FP.Sqrt(num2);
            this.X *= num;
            this.Y *= num;
            this.Z *= num;
        }

        /// <summary>
        /// 将向量归一化（引用传递版本）
        /// </summary>
        /// <param name="value">要归一化的向量</param>
        /// <param name="result">归一化后的单位向量</param>
        public static void Normalize(ref TSVector value, out TSVector result)
        {
            FP num2 = ((value.X * value.X) + (value.Y * value.Y)) + (value.Z * value.Z);
            FP num = FP.One / FP.Sqrt(num2);
            result.X = value.X * num;
            result.Y = value.Y * num;
            result.Z = value.Z * num;
        }

        #endregion

        /// <summary>
        /// 交换两个向量的分量值
        /// 会修改输入的两个向量
        /// </summary>
        /// <param name="vector1">第一个向量</param>
        /// <param name="vector2">第二个向量</param>
        public static void Swap(ref TSVector vector1, ref TSVector vector2)
        {
            FP temp;

            temp = vector1.X;
            vector1.X = vector2.X;
            vector2.X = temp;

            temp = vector1.Y;
            vector1.Y = vector2.Y;
            vector2.Y = temp;

            temp = vector1.Z;
            vector1.Z = vector2.Z;
            vector2.Z = temp;
        }

        #region 向量乘法（与标量）

        /// <summary>
        /// 将向量与标量相乘（缩放向量）
        /// </summary>
        /// <param name="value1">要缩放的向量</param>
        /// <param name="scaleFactor">缩放因子</param>
        /// <returns>缩放后的新向量</returns>
        public static TSVector Multiply(TSVector value1, FP scaleFactor)
        {
            TSVector result;
            TSVector.Multiply(ref value1, scaleFactor, out result);
            return result;
        }

        /// <summary>
        /// 将向量与标量相乘（引用传递版本）
        /// 分量级相乘：result.x = v.x * scaleFactor，以此类推
        /// </summary>
        /// <param name="value1">要缩放的向量</param>
        /// <param name="scaleFactor">缩放因子</param>
        /// <param name="result">缩放后的向量</param>
        public static void Multiply(ref TSVector value1, FP scaleFactor, out TSVector result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
        }

        #endregion

        #region 运算符重载

        /// <summary>
        /// 重载取模运算符，用于计算两个向量的叉积
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>叉积结果向量</returns>
        public static TSVector operator %(TSVector value1, TSVector value2)
        {
            TSVector result;
            TSVector.Cross(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 重载乘法运算符，用于计算两个向量的点积
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>点积结果</returns>
        public static FP operator *(TSVector value1, TSVector value2)
        {
            return TSVector.Dot(ref value1, ref value2);
        }

        /// <summary>
        /// 重载乘法运算符，用于向量与标量相乘（向量在后）
        /// </summary>
        /// <param name="value1">向量</param>
        /// <param name="value2">标量</param>
        /// <returns>相乘后的向量</returns>
        public static TSVector operator *(TSVector value1, FP value2)
        {
            TSVector result;
            TSVector.Multiply(ref value1, value2, out result);
            return result;
        }

        /// <summary>
        /// 重载乘法运算符，用于向量与标量相乘（标量在前）
        /// </summary>
        /// <param name="value1">标量</param>
        /// <param name="value2">向量</param>
        /// <returns>相乘后的向量</returns>
        public static TSVector operator *(FP value1, TSVector value2)
        {
            TSVector result;
            TSVector.Multiply(ref value2, value1, out result);
            return result;
        }

        /// <summary>
        /// 重载减法运算符，用于计算两个向量的差
        /// </summary>
        /// <param name="value1">被减向量</param>
        /// <param name="value2">减向量</param>
        /// <returns>向量差</returns>
        public static TSVector operator -(TSVector value1, TSVector value2)
        {
            TSVector result;
            TSVector.Subtract(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 重载加法运算符，用于计算两个向量的和
        /// </summary>
        /// <param name="value1">第一个向量</param>
        /// <param name="value2">第二个向量</param>
        /// <returns>向量和</returns>
        public static TSVector operator +(TSVector value1, TSVector value2)
        {
            TSVector result;
            TSVector.Add(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 重载除法运算符，用于向量除以标量
        /// </summary>
        /// <param name="value1">向量</param>
        /// <param name="value2">标量</param>
        /// <returns>除法后的向量</returns>
        public static TSVector operator /(TSVector value1, FP value2)
        {
            TSVector result;
            TSVector.Divide(ref value1, value2, out result);
            return result;
        }

        #endregion

        /// <summary>
        /// 将当前三维向量转换为二维向量TSVector2
        /// 取X和Y分量，忽略Z分量
        /// </summary>
        /// <returns>转换后的二维向量</returns>
        public TSVector2 ToTSVector2()
        {
            return new TSVector2(this.X, this.Y);
        }

        /// <summary>
        /// 将当前三维向量转换为四维向量TSVector4
        /// 取X、Y、Z分量，W分量设为1
        /// </summary>
        /// <returns>转换后的四维向量</returns>
        public TSVector4 ToTSVector4()
        {
            return new TSVector4(this.X, this.Y, this.Z, FP.One);
        }
    }
}