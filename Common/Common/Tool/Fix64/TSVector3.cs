using System;

namespace Start
{
    /// <summary>
    /// 三维向量结构，用于表示三维空间中的点或方向
    /// 提供向量的各种数学运算和转换方法
    /// </summary>
    [Serializable]
    public struct TSVector3 : IEquatable<TSVector3>
    {
        #region 静态只读向量常量

        /// <summary>零向量，分量为(0,0,0)</summary>
        public static readonly TSVector3 zero = new TSVector3(0, 0, 0);

        /// <summary>全1向量，分量为(1,1,1)</summary>
        public static readonly TSVector3 one = new TSVector3(1, 1, 1);

        /// <summary>左方向向量，分量为(-1,0,0)</summary>
        public static readonly TSVector3 left = new TSVector3(-1, 0, 0);

        /// <summary>右方向向量，分量为(1,0,0)</summary>
        public static readonly TSVector3 right = new TSVector3(1, 0, 0);

        /// <summary>上方向向量，分量为(0,1,0)</summary>
        public static readonly TSVector3 up = new TSVector3(0, 1, 0);

        /// <summary>下方向向量，分量为(0,-1,0)</summary>
        public static readonly TSVector3 down = new TSVector3(0, -1, 0);

        /// <summary>后方向向量，分量为(0,0,-1)</summary>
        public static readonly TSVector3 back = new TSVector3(0, 0, -1);

        /// <summary>前方向向量，分量为(0,0,1)</summary>
        public static readonly TSVector3 forward = new TSVector3(0, 0, 1);

        #endregion

        /// <summary>向量的X分量</summary>
        public FP x;

        /// <summary>向量的Y分量</summary>
        public FP y;

        /// <summary>向量的Z分量</summary>
        public FP z;

        #region 构造函数

        /// <summary>
        /// 使用int类型初始化向量的三个分量
        /// </summary>
        /// <param name="x">X分量值</param>
        /// <param name="y">Y分量值</param>
        /// <param name="z">Z分量值</param>
        public TSVector3(int x, int y, int z)
        {
            this.x = (FP)x;
            this.y = (FP)y;
            this.z = (FP)z;
        }

        /// <summary>
        /// 使用FP类型初始化向量的三个分量
        /// </summary>
        /// <param name="x">X分量值</param>
        /// <param name="y">Y分量值</param>
        /// <param name="z">Z分量值</param>
        public TSVector3(FP x, FP y, FP z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// 使用单个FP值初始化向量，所有分量均设为该值
        /// </summary>
        /// <param name="xyz">用于初始化所有分量的值</param>
        public TSVector3(FP xyz)
        {
            x = xyz;
            y = xyz;
            z = xyz;
        }

        #endregion

        #region 基本属性

        /// <summary>
        /// 获取向量的平方长度（模长的平方）
        /// 相比计算实际长度更高效，适用于比较向量长度
        /// </summary>
        public FP sqrMagnitude => x * x + y * y + z * z;

        /// <summary>
        /// 获取向量的长度（模长）
        /// 通过计算平方长度的平方根得到
        /// </summary>
        public FP magnitude => FP.Sqrt(sqrMagnitude);

        /// <summary>
        /// 获取当前向量的归一化（单位向量）版本
        /// 单位向量与原向量方向相同，长度为1
        /// </summary>
        public TSVector3 normalized
        {
            get
            {
                TSVector3 result = this;
                result.Normalize();
                return result;
            }
        }

        #endregion

        #region 向量操作

        /// <summary>
        /// 将当前向量归一化（单位化）
        /// 会修改当前向量实例，使其长度为1
        /// </summary>
        public void Normalize()
        {
            FP length = magnitude;
            if (length > FP.Zero)
            {
                FP invLength = FP.One / length;
                x *= invLength;
                y *= invLength;
                z *= invLength;
            }
        }

        /// <summary>
        /// 反转当前向量的方向（所有分量取相反数）
        /// 会修改当前向量实例
        /// </summary>
        public void Negate()
        {
            x = -x;
            y = -y;
            z = -z;
        }

        /// <summary>
        /// 按分量缩放当前向量（与另一个向量的分量相乘）
        /// 会修改当前向量实例
        /// </summary>
        /// <param name="other">用于缩放的向量</param>
        public void Scale(TSVector3 other)
        {
            x *= other.x;
            y *= other.y;
            z *= other.z;
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
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// 将当前向量设为零向量（所有分量置为0）
        /// </summary>
        public void MakeZero()
        {
            x = FP.Zero;
            y = FP.Zero;
            z = FP.Zero;
        }

        #endregion

        #region 静态方法

        /// <summary>
        /// 计算两个向量的点积
        /// </summary>
        /// <param name="a">第一个向量</param>
        /// <param name="b">第二个向量</param>
        /// <returns>点积结果</returns>
        public static FP Dot(TSVector3 a, TSVector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        /// <summary>
        /// 计算两个向量的叉积
        /// </summary>
        /// <param name="a">第一个向量</param>
        /// <param name="b">第二个向量</param>
        /// <returns>叉积结果向量</returns>
        public static TSVector3 Cross(TSVector3 a, TSVector3 b)
        {
            return new TSVector3(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );
        }

        /// <summary>
        /// 计算两个向量的和
        /// </summary>
        /// <param name="a">第一个向量</param>
        /// <param name="b">第二个向量</param>
        /// <returns>和向量</returns>
        public static TSVector3 Add(TSVector3 a, TSVector3 b)
        {
            return new TSVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        /// <summary>
        /// 计算两个向量的差
        /// </summary>
        /// <param name="a">被减向量</param>
        /// <param name="b">减向量</param>
        /// <returns>差向量</returns>
        public static TSVector3 Subtract(TSVector3 a, TSVector3 b)
        {
            return new TSVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        /// <summary>
        /// 将向量与标量相乘（缩放向量）
        /// </summary>
        /// <param name="vector">要缩放的向量</param>
        /// <param name="scale">缩放因子</param>
        /// <returns>缩放后的向量</returns>
        public static TSVector3 Multiply(TSVector3 vector, FP scale)
        {
            return new TSVector3(vector.x * scale, vector.y * scale, vector.z * scale);
        }

        /// <summary>
        /// 在两个向量之间进行线性插值
        /// </summary>
        /// <param name="from">起始向量</param>
        /// <param name="to">目标向量</param>
        /// <param name="t">插值比例（0~1之间）</param>
        /// <returns>插值结果向量</returns>
        public static TSVector3 Lerp(TSVector3 from, TSVector3 to, FP t)
        {
            return new TSVector3(
                from.x + (to.x - from.x) * t,
                from.y + (to.y - from.y) * t,
                from.z + (to.z - from.z) * t
            );
        }

        /// <summary>
        /// 计算两个点之间的欧氏距离
        /// </summary>
        /// <param name="a">第一个点</param>
        /// <param name="b">第二个点</param>
        /// <returns>两点之间的距离</returns>
        public static FP Distance(TSVector3 a, TSVector3 b)
        {
            return Subtract(a, b).magnitude;
        }

        #endregion

        #region 运算符重载

        /// <summary>
        /// 向量加法运算符
        /// </summary>
        public static TSVector3 operator +(TSVector3 a, TSVector3 b)
        {
            return Add(a, b);
        }

        /// <summary>
        /// 向量减法运算符
        /// </summary>
        public static TSVector3 operator -(TSVector3 a, TSVector3 b)
        {
            return Subtract(a, b);
        }

        /// <summary>
        /// 向量取反运算符
        /// </summary>
        public static TSVector3 operator -(TSVector3 v)
        {
            return new TSVector3(-v.x, -v.y, -v.z);
        }

        /// <summary>
        /// 向量与标量乘法运算符
        /// </summary>
        public static TSVector3 operator *(TSVector3 v, FP s)
        {
            return Multiply(v, s);
        }

        /// <summary>
        /// 标量与向量乘法运算符
        /// </summary>
        public static TSVector3 operator *(FP s, TSVector3 v)
        {
            return Multiply(v, s);
        }

        /// <summary>
        /// 向量相等运算符
        /// </summary>
        public static bool operator ==(TSVector3 a, TSVector3 b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// 向量不等运算符
        /// </summary>
        public static bool operator !=(TSVector3 a, TSVector3 b)
        {
            return !a.Equals(b);
        }

        #endregion

        #region 转换方法

        /// <summary>
        /// 转换为二维向量（忽略Z分量）
        /// </summary>
        public TSVector2 ToTSVector2()
        {
            return new TSVector2(x, y);
        }

        /// <summary>
        /// 转换为四维向量（W分量设为1）
        /// </summary>
        public TSVector4 ToTSVector4()
        {
            return new TSVector4(x, y, z, FP.One);
        }

        #endregion

        #region 重写方法

        /// <summary>
        /// 检查当前向量是否与指定对象相等
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is TSVector3 other && Equals(other);
        }

        /// <summary>
        /// 检查当前向量是否与另一个向量相等
        /// </summary>
        public bool Equals(TSVector3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        /// <summary>
        /// 获取当前向量的哈希码
        /// </summary>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        /// <summary>
        /// 将向量转换为字符串表示形式
        /// </summary>
        public override string ToString()
        {
            return $"({x.AsFloat():f1}, {y.AsFloat():f1}, {z.AsFloat():f1})";
        }

        #endregion
    }
}