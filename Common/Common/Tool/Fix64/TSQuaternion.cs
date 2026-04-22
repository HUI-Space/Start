/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
 *
 *  本软件按"原样"提供，不附带任何明示或暗示的担保。
 *  作者不对因使用本软件而产生的任何损害承担责任。
 *
 *  允许任何人将本软件用于任何目的，包括商业应用，
 *  并可自由修改和重新分发，但需遵守以下限制：
 *
 *  1. 不得歪曲本软件的来源；不得声称您编写了原始软件。
 *     如果您在产品中使用本软件，在产品文档中致谢将不胜感激，但非必需。
 *  2. 修改后的源版本必须明确标记为修改版，不得歪曲为原始软件。
 *  3. 本声明不得从任何源分发中删除或修改。
 */

using System;

namespace Start
{
    /// <summary>
    /// 表示3D空间中方向的四元数结构
    /// 四元数是一种用于表示旋转的数学工具，相比欧拉角可避免万向锁问题
    /// 内部采用(x, y, z, w)分量表示，其中(x,y,z)为向量部分，w为标量部分
    /// </summary>
    [Serializable] // 支持序列化，可用于保存和传输
    public struct TSQuaternion
    {
        /// <summary>四元数的X分量（向量部分）</summary>
        public FP X;

        /// <summary>四元数的Y分量（向量部分）</summary>
        public FP Y;

        /// <summary>四元数的Z分量（向量部分）</summary>
        public FP Z;

        /// <summary>四元数的W分量（标量部分）</summary>
        public FP W;

        /// <summary>单位四元数（表示无旋转）</summary>
        public static readonly TSQuaternion Identity;

        /// <summary>
        /// 静态构造函数，初始化单位四元数
        /// 单位四元数的分量为(0,0,0,1)，表示没有旋转
        /// </summary>
        static TSQuaternion()
        {
            Identity = new TSQuaternion(0, 0, 0, 1);
        }

        /// <summary>
        /// 初始化四元数的新实例
        /// </summary>
        /// <param name="x">X分量值</param>
        /// <param name="y">Y分量值</param>
        /// <param name="z">Z分量值</param>
        /// <param name="w">W分量值</param>
        public TSQuaternion(FP x, FP y, FP z, FP w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// 重置四元数的分量值
        /// </summary>
        /// <param name="new_x">新的X分量</param>
        /// <param name="new_y">新的Y分量</param>
        /// <param name="new_z">新的Z分量</param>
        /// <param name="new_w">新的W分量</param>
        public void Set(FP new_x, FP new_y, FP new_z, FP new_w)
        {
            this.X = new_x;
            this.Y = new_y;
            this.Z = new_z;
            this.W = new_w;
        }

        /// <summary>
        /// 根据从一个方向向量到另一个方向向量的旋转来设置四元数
        /// </summary>
        /// <param name="fromDirection">起始方向向量</param>
        /// <param name="toDirection">目标方向向量</param>
        public void SetFromToRotation(TSVector fromDirection, TSVector toDirection)
        {
            // 计算从起始方向到目标方向的旋转四元数
            TSQuaternion targetRotation = TSQuaternion.FromToRotation(fromDirection, toDirection);
            // 应用计算出的旋转分量
            this.Set(targetRotation.X, targetRotation.Y, targetRotation.Z, targetRotation.W);
        }

        /// <summary>
        /// 获取当前四元数对应的欧拉角（单位：度）
        /// 欧拉角表示绕X轴（俯仰）、Y轴（偏航）、Z轴（翻滚）的旋转
        /// </summary>
        public TSVector eulerAngles
        {
            get
            {
                TSVector result = new TSVector();

                // 计算中间变量，用于推导欧拉角
                FP ysqr = Y * Y;
                FP t0 = -2.0f * (ysqr + Z * Z) + 1.0f;
                FP t1 = +2.0f * (X * Y - W * Z);
                FP t2 = -2.0f * (X * Z + W * Y);
                FP t3 = +2.0f * (Y * Z - W * X);
                FP t4 = -2.0f * (X * X + ysqr) + 1.0f;

                // 确保值在有效范围内（避免反三角函数计算错误）
                t2 = t2 > 1.0f ? 1.0f : t2;
                t2 = t2 < -1.0f ? -1.0f : t2;

                // 计算各轴旋转角度（弧度转角度）
                result.X = FP.Atan2(t3, t4) * FP.Rad2Deg; // 绕X轴旋转（俯仰角）
                result.Y = FP.Asin(t2) * FP.Rad2Deg; // 绕Y轴旋转（偏航角）
                result.Z = FP.Atan2(t1, t0) * FP.Rad2Deg; // 绕Z轴旋转（翻滚角）

                // 反转结果以匹配标准欧拉角定义
                return result * -1;
            }
        }

        /// <summary>
        /// 计算两个四元数之间的旋转角度（单位：度）
        /// </summary>
        /// <param name="a">第一个四元数</param>
        /// <param name="b">第二个四元数</param>
        /// <returns>两个方向之间的最小旋转角度（0-180度）</returns>
        public static FP Angle(TSQuaternion a, TSQuaternion b)
        {
            // 计算a的逆四元数
            TSQuaternion aInv = TSQuaternion.Inverse(a);
            // 计算从a到b的旋转四元数
            TSQuaternion f = b * aInv;

            // 计算角度（四元数的w分量与旋转角度相关）
            FP angle = FP.Acos(f.W) * 2 * FP.Rad2Deg;

            // 确保返回最小角度（不超过180度）
            if (angle > 180)
            {
                angle = 360 - angle;
            }

            return angle;
        }

        /// <summary>
        /// 两个四元数相加
        /// 注意：四元数加法不表示旋转叠加，仅为分量级别的数学运算
        /// </summary>
        /// <param name="quaternion1">第一个四元数</param>
        /// <param name="quaternion2">第二个四元数</param>
        /// <returns>两个四元数的和</returns>

        #region public static JQuaternion Add(JQuaternion quaternion1, JQuaternion quaternion2)

        public static TSQuaternion Add(TSQuaternion quaternion1, TSQuaternion quaternion2)
        {
            TSQuaternion result;
            TSQuaternion.Add(ref quaternion1, ref quaternion2, out result);
            return result;
        }

        #endregion

        /// <summary>
        /// 创建一个指向指定前方方向的旋转四元数（上方向默认为Y轴正方向）
        /// </summary>
        /// <param name="forward">前方方向向量</param>
        /// <returns>对应的旋转四元数</returns>
        public static TSQuaternion LookRotation(TSVector forward)
        {
            // 使用默认上方向（Y轴）创建观察矩阵，再转换为四元数
            return CreateFromMatrix(TSMatrix.LookAt(forward, TSVector.Up));
        }

        /// <summary>
        /// 创建一个指向指定前方方向和上方向的旋转四元数
        /// </summary>
        /// <param name="forward">前方方向向量</param>
        /// <param name="upwards">上方向向量</param>
        /// <returns>对应的旋转四元数</returns>
        public static TSQuaternion LookRotation(TSVector forward, TSVector upwards)
        {
            // 使用指定的上方向创建观察矩阵，再转换为四元数
            return CreateFromMatrix(TSMatrix.LookAt(forward, upwards));
        }

        /// <summary>
        /// 球面线性插值（SLERP）两个四元数
        /// 相比线性插值，SLERP在旋转插值时能保持均匀的角速度
        /// </summary>
        /// <param name="from">起始四元数</param>
        /// <param name="to">目标四元数</param>
        /// <param name="t">插值参数（0-1）</param>
        /// <returns>插值结果四元数</returns>
        public static TSQuaternion Slerp(TSQuaternion from, TSQuaternion to, FP t)
        {
            // 限制t在[0,1]范围内
            t = TSMath.Clamp(t, 0, 1);

            // 计算两个四元数的点积
            FP dot = Dot(from, to);

            // 如果点积为负，反转目标四元数以选择最短路径
            if (dot < 0.0f)
            {
                to = Multiply(to, -1);
                dot = -dot;
            }

            // 计算半角
            FP halfTheta = FP.Acos(dot);

            // 应用SLERP公式计算插值结果
            return Multiply(
                Multiply(from, FP.Sin((1 - t) * halfTheta)) + Multiply(to, FP.Sin(t * halfTheta)),
                1 / FP.Sin(halfTheta)
            );
        }

        /// <summary>
        /// 向目标四元数旋转，每次旋转不超过指定的最大角度
        /// 用于平滑旋转控制，避免旋转过快
        /// </summary>
        /// <param name="from">起始四元数</param>
        /// <param name="to">目标四元数</param>
        /// <param name="maxDegreesDelta">最大允许旋转角度（度）</param>
        /// <returns>旋转后的四元数</returns>
        public static TSQuaternion RotateTowards(TSQuaternion from, TSQuaternion to, FP maxDegreesDelta)
        {
            // 计算两个四元数的点积
            FP dot = Dot(from, to);

            // 如果点积为负，反转目标四元数以选择最短路径
            if (dot < 0.0f)
            {
                to = Multiply(to, -1);
                dot = -dot;
            }

            // 计算当前角度和目标角度
            FP halfTheta = FP.Acos(dot);
            FP theta = halfTheta * 2;

            // 将最大角度转换为弧度
            maxDegreesDelta *= FP.Deg2Rad;

            // 如果当前角度小于最大允许角度，直接返回目标四元数
            if (maxDegreesDelta >= theta)
            {
                return to;
            }

            // 计算插值比例
            maxDegreesDelta /= theta;

            // 应用插值计算
            return Multiply(
                Multiply(from, FP.Sin((1 - maxDegreesDelta) * halfTheta)) +
                Multiply(to, FP.Sin(maxDegreesDelta * halfTheta)),
                1 / FP.Sin(halfTheta)
            );
        }

        /// <summary>
        /// 从欧拉角（度）创建四元数
        /// </summary>
        /// <param name="x">绕X轴旋转角度（俯仰角）</param>
        /// <param name="y">绕Y轴旋转角度（偏航角）</param>
        /// <param name="z">绕Z轴旋转角度（翻滚角）</param>
        /// <returns>对应的四元数</returns>
        public static TSQuaternion Euler(FP x, FP y, FP z)
        {
            // 将角度转换为弧度
            x *= FP.Deg2Rad;
            y *= FP.Deg2Rad;
            z *= FP.Deg2Rad;

            TSQuaternion rotation;
            // 使用Yaw-Pitch-Roll顺序创建四元数
            TSQuaternion.CreateFromYawPitchRoll(y, x, z, out rotation);

            return rotation;
        }

        /// <summary>
        /// 从欧拉角向量（度）创建四元数
        /// </summary>
        /// <param name="eulerAngles">欧拉角向量（x:俯仰, y:偏航, z:翻滚）</param>
        /// <returns>对应的四元数</returns>
        public static TSQuaternion Euler(TSVector eulerAngles)
        {
            return Euler(eulerAngles.X, eulerAngles.Y, eulerAngles.Z);
        }

        /// <summary>
        /// 从轴角表示创建四元数
        /// 轴角表示：绕指定轴旋转指定角度
        /// </summary>
        /// <param name="angle">旋转角度（度）</param>
        /// <param name="axis">旋转轴向量</param>
        /// <returns>对应的四元数</returns>
        public static TSQuaternion AngleAxis(FP angle, TSVector axis)
        {
            // 标准化旋转轴（确保是单位向量）
            axis.Normalize();

            // 计算半角（四元数使用半角计算）
            FP halfAngle = angle * FP.Deg2Rad * FP.Half;

            TSQuaternion rotation;
            FP sin = FP.Sin(halfAngle);

            // 四元数轴角公式：(axis.x*sin(θ/2), axis.y*sin(θ/2), axis.z*sin(θ/2), cos(θ/2))
            rotation.X = axis.X * sin;
            rotation.Y = axis.Y * sin;
            rotation.Z = axis.Z * sin;
            rotation.W = FP.Cos(halfAngle);

            return rotation;
        }

        /// <summary>
        /// 从偏航角（Yaw）、俯仰角（Pitch）、翻滚角（Roll）创建四元数
        /// 旋转顺序为Z-X-Y（Yaw-Pitch-Roll）
        /// </summary>
        /// <param name="yaw">偏航角（绕Y轴旋转，弧度）</param>
        /// <param name="pitch">俯仰角（绕X轴旋转，弧度）</param>
        /// <param name="roll">翻滚角（绕Z轴旋转，弧度）</param>
        /// <param name="result">输出的四元数</param>
        public static void CreateFromYawPitchRoll(FP yaw, FP pitch, FP roll, out TSQuaternion result)
        {
            // 计算各角度的半角正弦和余弦
            FP num9 = roll * FP.Half;
            FP num6 = FP.Sin(num9);
            FP num5 = FP.Cos(num9);
            FP num8 = pitch * FP.Half;
            FP num4 = FP.Sin(num8);
            FP num3 = FP.Cos(num8);
            FP num7 = yaw * FP.Half;
            FP num2 = FP.Sin(num7);
            FP num = FP.Cos(num7);

            // 应用Yaw-Pitch-Roll转四元数公式
            result.X = ((num * num4) * num5) + ((num2 * num3) * num6);
            result.Y = ((num2 * num3) * num5) - ((num * num4) * num6);
            result.Z = ((num * num3) * num6) - ((num2 * num4) * num5);
            result.W = ((num * num3) * num5) + ((num2 * num4) * num6);
        }

        /// <summary>
        /// 两个四元数相加（引用传递版本，性能更优）
        /// </summary>
        /// <param name="quaternion1">第一个四元数</param>
        /// <param name="quaternion2">第二个四元数</param>
        /// <param name="result">输出的和四元数</param>
        public static void Add(ref TSQuaternion quaternion1, ref TSQuaternion quaternion2, out TSQuaternion result)
        {
            result.X = quaternion1.X + quaternion2.X;
            result.Y = quaternion1.Y + quaternion2.Y;
            result.Z = quaternion1.Z + quaternion2.Z;
            result.W = quaternion1.W + quaternion2.W;
        }

        /// <summary>
        /// 计算四元数的共轭
        /// 共轭四元数的向量部分取反，标量部分不变：(-x, -y, -z, w)
        /// 对于单位四元数，共轭等于其逆
        /// </summary>
        /// <param name="value">输入四元数</param>
        /// <returns>共轭四元数</returns>
        public static TSQuaternion Conjugate(TSQuaternion value)
        {
            TSQuaternion quaternion;
            quaternion.X = -value.X;
            quaternion.Y = -value.Y;
            quaternion.Z = -value.Z;
            quaternion.W = value.W;
            return quaternion;
        }

        /// <summary>
        /// 计算两个四元数的点积
        /// 点积公式：q1·q2 = w1w2 + x1x2 + y1y2 + z1z2
        /// 点积的绝对值越大，两个四元数表示的方向越接近
        /// </summary>
        /// <param name="a">第一个四元数</param>
        /// <param name="b">第二个四元数</param>
        /// <returns>点积结果</returns>
        public static FP Dot(TSQuaternion a, TSQuaternion b)
        {
            return a.W * b.W + a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        /// 计算四元数的逆
        /// 逆四元数与原四元数相乘结果为单位四元数
        /// 公式：q⁻¹ = conjugate(q) / |q|²，其中|q|为四元数的模
        /// </summary>
        /// <param name="rotation">输入四元数</param>
        /// <returns>逆四元数</returns>
        public static TSQuaternion Inverse(TSQuaternion rotation)
        {
            // 计算模的平方的倒数
            FP invNorm = FP.One / ((rotation.X * rotation.X) + (rotation.Y * rotation.Y) + (rotation.Z * rotation.Z) +
                                   (rotation.W * rotation.W));
            // 共轭乘以模平方的倒数得到逆
            return TSQuaternion.Multiply(TSQuaternion.Conjugate(rotation), invNorm);
        }

        /// <summary>
        /// 创建从一个方向向量旋转到另一个方向向量的四元数
        /// </summary>
        /// <param name="fromVector">起始方向向量</param>
        /// <param name="toVector">目标方向向量</param>
        /// <returns>对应的旋转四元数</returns>
        public static TSQuaternion FromToRotation(TSVector fromVector, TSVector toVector)
        {
            // 计算两个向量的叉积（得到旋转轴）
            TSVector w = TSVector.Cross(fromVector, toVector);
            // 初始四元数：(叉积分量, 点积)
            TSQuaternion q = new TSQuaternion(w.X, w.Y, w.Z, TSVector.Dot(fromVector, toVector));
            // 调整标量部分，确保四元数有效
            q.W += FP.Sqrt(fromVector.sqrMagnitude * toVector.sqrMagnitude);
            // 标准化四元数
            q.Normalize();

            return q;
        }

        /// <summary>
        /// 线性插值（LERP）两个四元数，并标准化结果
        /// 注意：LERP在旋转插值时可能导致旋转速度不均匀
        /// </summary>
        /// <param name="a">起始四元数</param>
        /// <param name="b">目标四元数</param>
        /// <param name="t">插值参数（0-1，会被限制在该范围）</param>
        /// <returns>插值结果四元数</returns>
        public static TSQuaternion Lerp(TSQuaternion a, TSQuaternion b, FP t)
        {
            // 限制t在[0,1]范围内
            t = TSMath.Clamp(t, FP.Zero, FP.One);

            return LerpUnclamped(a, b, t);
        }

        /// <summary>
        /// 未限制范围的线性插值（LERP）
        /// t可以是任意值，用于外插计算
        /// </summary>
        /// <param name="a">起始四元数</param>
        /// <param name="b">目标四元数</param>
        /// <param name="t">插值参数</param>
        /// <returns>插值结果四元数</returns>
        public static TSQuaternion LerpUnclamped(TSQuaternion a, TSQuaternion b, FP t)
        {
            // 线性插值公式：a*(1-t) + b*t
            TSQuaternion result = TSQuaternion.Multiply(a, (1 - t)) + TSQuaternion.Multiply(b, t);
            // 标准化结果以确保是单位四元数
            result.Normalize();

            return result;
        }

        /// <summary>
        /// 两个四元数相减
        /// 分量级别的数学运算，不代表旋转操作
        /// </summary>
        /// <param name="quaternion1">被减四元数</param>
        /// <param name="quaternion2">减四元数</param>
        /// <returns>两个四元数的差</returns>

        #region public static JQuaternion Subtract(JQuaternion quaternion1, JQuaternion quaternion2)

        public static TSQuaternion Subtract(TSQuaternion quaternion1, TSQuaternion quaternion2)
        {
            TSQuaternion result;
            TSQuaternion.Subtract(ref quaternion1, ref quaternion2, out result);
            return result;
        }

        #endregion

        /// <summary>
        /// 两个四元数相减（引用传递版本，性能更优）
        /// </summary>
        /// <param name="quaternion1">被减四元数</param>
        /// <param name="quaternion2">减四元数</param>
        /// <param name="result">输出的差四元数</param>
        public static void Subtract(ref TSQuaternion quaternion1, ref TSQuaternion quaternion2, out TSQuaternion result)
        {
            result.X = quaternion1.X - quaternion2.X;
            result.Y = quaternion1.Y - quaternion2.Y;
            result.Z = quaternion1.Z - quaternion2.Z;
            result.W = quaternion1.W - quaternion2.W;
        }

        /// <summary>
        /// 两个四元数相乘（表示旋转的复合）
        /// 注意：四元数乘法不满足交换律，q1*q2表示先应用q2旋转，再应用q1旋转
        /// </summary>
        /// <param name="quaternion1">第一个四元数</param>
        /// <param name="quaternion2">第二个四元数</param>
        /// <returns>乘积四元数</returns>

        #region public static JQuaternion Multiply(JQuaternion quaternion1, JQuaternion quaternion2)

        public static TSQuaternion Multiply(TSQuaternion quaternion1, TSQuaternion quaternion2)
        {
            TSQuaternion result;
            TSQuaternion.Multiply(ref quaternion1, ref quaternion2, out result);
            return result;
        }

        #endregion

        /// <summary>
        /// 两个四元数相乘（引用传递版本，性能更优）
        /// </summary>
        /// <param name="quaternion1">第一个四元数</param>
        /// <param name="quaternion2">第二个四元数</param>
        /// <param name="result">输出的乘积四元数</param>
        public static void Multiply(ref TSQuaternion quaternion1, ref TSQuaternion quaternion2, out TSQuaternion result)
        {
            // 提取四元数分量
            FP x = quaternion1.X;
            FP y = quaternion1.Y;
            FP z = quaternion1.Z;
            FP w = quaternion1.W;
            FP num4 = quaternion2.X;
            FP num3 = quaternion2.Y;
            FP num2 = quaternion2.Z;
            FP num = quaternion2.W;

            // 计算中间变量（四元数乘法公式展开）
            FP num12 = (y * num2) - (z * num3);
            FP num11 = (z * num4) - (x * num2);
            FP num10 = (x * num3) - (y * num4);
            FP num9 = ((x * num4) + (y * num3)) + (z * num2);

            // 计算结果分量
            result.X = ((x * num) + (num4 * w)) + num12;
            result.Y = ((y * num) + (num3 * w)) + num11;
            result.Z = ((z * num) + (num2 * w)) + num10;
            result.W = (w * num) - num9;
        }


        /// <summary>
        /// 四元数与标量相乘（缩放四元数）
        /// </summary>
        /// <param name="quaternion1">四元数</param>
        /// <param name="scaleFactor">缩放因子</param>
        /// <returns>缩放后的四元数</returns>

        #region public static JQuaternion Multiply(JQuaternion quaternion1, FP scaleFactor)

        public static TSQuaternion Multiply(TSQuaternion quaternion1, FP scaleFactor)
        {
            TSQuaternion result;
            TSQuaternion.Multiply(ref quaternion1, scaleFactor, out result);
            return result;
        }

        #endregion

        /// <summary>
        /// 四元数与标量相乘（引用传递版本，性能更优）
        /// </summary>
        /// <param name="quaternion1">四元数</param>
        /// <param name="scaleFactor">缩放因子</param>
        /// <param name="result">输出的缩放后四元数</param>
        public static void Multiply(ref TSQuaternion quaternion1, FP scaleFactor, out TSQuaternion result)
        {
            result.X = quaternion1.X * scaleFactor;
            result.Y = quaternion1.Y * scaleFactor;
            result.Z = quaternion1.Z * scaleFactor;
            result.W = quaternion1.W * scaleFactor;
        }


        /// <summary>
        /// 将四元数标准化（转换为单位四元数）
        /// 单位四元数的模为1，是表示旋转的有效四元数
        /// </summary>

        #region public void Normalize()

        public void Normalize()
        {
            // 计算模的平方
            FP num2 = (((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z)) + (this.W * this.W);
            // 计算模的倒数（避免开方后再倒数，提高性能）
            FP num = 1 / (FP.Sqrt(num2));
            // 各分量乘以模的倒数，得到单位四元数
            this.X *= num;
            this.Y *= num;
            this.Z *= num;
            this.W *= num;
        }

        #endregion

        /// <summary>
        /// 从旋转矩阵创建四元数
        /// 旋转矩阵是3x3矩阵，用于表示3D空间中的旋转
        /// </summary>
        /// <param name="matrix">旋转矩阵</param>
        /// <returns>对应的四元数</returns>

        #region public static JQuaternion CreateFromMatrix(JMatrix matrix)

        public static TSQuaternion CreateFromMatrix(TSMatrix matrix)
        {
            TSQuaternion result;
            TSQuaternion.CreateFromMatrix(ref matrix, out result);
            return result;
        }

        #endregion

        /// <summary>
        /// 从旋转矩阵创建四元数（引用传递版本，性能更优）
        /// 使用矩阵的迹和对角线元素计算，避免精度问题
        /// </summary>
        /// <param name="matrix">旋转矩阵</param>
        /// <param name="result">输出的四元数</param>
        public static void CreateFromMatrix(ref TSMatrix matrix, out TSQuaternion result)
        {
            // 计算矩阵的迹（对角线元素之和）
            FP num8 = (matrix.M11 + matrix.M22) + matrix.M33;
            if (num8 > FP.Zero)
            {
                // 迹为正的情况，使用迹计算四元数
                FP num = FP.Sqrt((num8 + FP.One));
                result.W = num * FP.Half;
                num = FP.Half / num;
                result.X = (matrix.M23 - matrix.M32) * num;
                result.Y = (matrix.M31 - matrix.M13) * num;
                result.Z = (matrix.M12 - matrix.M21) * num;
            }
            else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                // 最大对角线元素为M11的情况
                FP num7 = FP.Sqrt((((FP.One + matrix.M11) - matrix.M22) - matrix.M33));
                FP num4 = FP.Half / num7;
                result.X = FP.Half * num7;
                result.Y = (matrix.M12 + matrix.M21) * num4;
                result.Z = (matrix.M13 + matrix.M31) * num4;
                result.W = (matrix.M23 - matrix.M32) * num4;
            }
            else if (matrix.M22 > matrix.M33)
            {
                // 最大对角线元素为M22的情况
                FP num6 = FP.Sqrt((((FP.One + matrix.M22) - matrix.M11) - matrix.M33));
                FP num3 = FP.Half / num6;
                result.X = (matrix.M21 + matrix.M12) * num3;
                result.Y = FP.Half * num6;
                result.Z = (matrix.M32 + matrix.M23) * num3;
                result.W = (matrix.M31 - matrix.M13) * num3;
            }
            else
            {
                // 最大对角线元素为M33的情况
                FP num5 = FP.Sqrt((((FP.One + matrix.M33) - matrix.M11) - matrix.M22));
                FP num2 = FP.Half / num5;
                result.X = (matrix.M31 + matrix.M13) * num2;
                result.Y = (matrix.M32 + matrix.M23) * num2;
                result.Z = FP.Half * num5;
                result.W = (matrix.M12 - matrix.M21) * num2;
            }
        }


        /// <summary>
        /// 重载乘法运算符，实现四元数相乘
        /// </summary>
        /// <param name="value1">第一个四元数</param>
        /// <param name="value2">第二个四元数</param>
        /// <returns>乘积四元数</returns>

        #region public static FP operator *(JQuaternion value1, JQuaternion value2)

        public static TSQuaternion operator *(TSQuaternion value1, TSQuaternion value2)
        {
            TSQuaternion result;
            TSQuaternion.Multiply(ref value1, ref value2, out result);
            return result;
        }

        #endregion

        /// <summary>
        /// 重载加法运算符，实现四元数相加
        /// </summary>
        /// <param name="value1">第一个四元数</param>
        /// <param name="value2">第二个四元数</param>
        /// <returns>和四元数</returns>

        #region public static FP operator +(JQuaternion value1, JQuaternion value2)

        public static TSQuaternion operator +(TSQuaternion value1, TSQuaternion value2)
        {
            TSQuaternion result;
            TSQuaternion.Add(ref value1, ref value2, out result);
            return result;
        }

        #endregion

        /// <summary>
        /// 重载减法运算符，实现四元数相减
        /// </summary>
        /// <param name="value1">被减四元数</param>
        /// <param name="value2">减四元数</param>
        /// <returns>差四元数</returns>

        #region public static FP operator -(JQuaternion value1, JQuaternion value2)

        public static TSQuaternion operator -(TSQuaternion value1, TSQuaternion value2)
        {
            TSQuaternion result;
            TSQuaternion.Subtract(ref value1, ref value2, out result);
            return result;
        }

        #endregion

        /**
         *  @brief 使用四元数旋转向量
         *  旋转公式：v' = q * v * q⁻¹（这里通过优化公式直接计算）
         **/
        public static TSVector operator *(TSQuaternion quat, TSVector vec)
        {
            // 预计算中间变量，减少重复计算
            FP num = quat.X * 2f;
            FP num2 = quat.Y * 2f;
            FP num3 = quat.Z * 2f;
            FP num4 = quat.X * num;
            FP num5 = quat.Y * num2;
            FP num6 = quat.Z * num3;
            FP num7 = quat.X * num2;
            FP num8 = quat.X * num3;
            FP num9 = quat.Y * num3;
            FP num10 = quat.W * num;
            FP num11 = quat.W * num2;
            FP num12 = quat.W * num3;

            // 应用旋转公式计算结果向量
            TSVector result;
            result.X = (1f - (num5 + num6)) * vec.X + (num7 - num12) * vec.Y + (num8 + num11) * vec.Z;
            result.Y = (num7 + num12) * vec.X + (1f - (num4 + num6)) * vec.Y + (num9 - num10) * vec.Z;
            result.Z = (num8 - num11) * vec.X + (num9 + num10) * vec.Y + (1f - (num4 + num5)) * vec.Z;

            return result;
        }

        /// <summary>
        /// 转换为字符串表示
        /// </summary>
        /// <returns>包含四元数分量的字符串</returns>
        public override string ToString()
        {
            return string.Format("({0:f1}, {1:f1}, {2:f1}, {3:f1})", X.AsFloat(), Y.AsFloat(), Z.AsFloat(),
                W.AsFloat());
        }
    }
}