/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
 *
 *  本软件按"原样"提供，不提供任何明示或暗示的担保。
 *  作者不对因使用本软件而产生的任何损害承担责任。
 *
 *  允许任何人将本软件用于任何目的，包括商业应用，并可自由修改和重新分发，
 *  但需遵守以下限制：
 *
 *  1. 不得歪曲本软件的来源；不得声称您编写了原始软件。如果您在产品中使用本软件，
 *     在产品文档中致谢是值得赞赏的，但不是必需的。
 *  2. 修改后的源代码版本必须明确标记为修改版，不得歪曲为原始软件。
 *  3. 本声明不得从任何源代码分发中删除或修改。
 */

namespace Start
{
    /// <summary>
    /// 表示一个3x3的矩阵结构，用于数学计算，特别是三维空间中的旋转和变换操作
    /// </summary>
    public struct TSMatrix
    {
        /// <summary>
        /// 矩阵第一行第一列的元素
        /// </summary>
        public FP M11; // 第一行向量的第一个元素

        /// <summary>
        /// 矩阵第一行第二列的元素
        /// </summary>
        public FP M12; // 第一行向量的第二个元素

        /// <summary>
        /// 矩阵第一行第三列的元素
        /// </summary>
        public FP M13; // 第一行向量的第三个元素

        /// <summary>
        /// 矩阵第二行第一列的元素
        /// </summary>
        public FP M21; // 第二行向量的第一个元素

        /// <summary>
        /// 矩阵第二行第二列的元素
        /// </summary>
        public FP M22; // 第二行向量的第二个元素

        /// <summary>
        /// 矩阵第二行第三列的元素
        /// </summary>
        public FP M23;
        // 第二行向量的第三个元素

        /// <summary>
        /// 矩阵第三行第一列的元素
        /// </summary>
        public FP M31; // 第三行向量的第一个元素

        /// <summary>
        /// 矩阵第三行第二列的元素
        /// </summary>
        public FP M32; // 第三行向量的第二个元素

        /// <summary>
        /// 矩阵第三行第三列的元素
        /// </summary>
        public FP M33; // 第三行向量的第三个元素

        /// <summary>
        /// 内部使用的单位矩阵实例
        /// </summary>
        internal static TSMatrix InternalIdentity;

        /// <summary>
        /// 单位矩阵（对角线元素为1，其余为0）
        /// </summary>
        public static readonly TSMatrix Identity;

        /// <summary>
        /// 零矩阵（所有元素均为0）
        /// </summary>
        public static readonly TSMatrix Zero;

        /// <summary>
        /// 静态构造函数，初始化单位矩阵和零矩阵
        /// </summary>
        static TSMatrix()
        {
            // 初始化零矩阵（默认构造的结构体成员均为0）
            Zero = new TSMatrix();

            // 初始化单位矩阵，对角线元素设为1
            Identity = new TSMatrix();
            Identity.M11 = FP.One;
            Identity.M22 = FP.One;
            Identity.M33 = FP.One;

            // 为内部单位矩阵赋值
            InternalIdentity = Identity;
        }

        /// <summary>
        /// 获取当前矩阵对应的欧拉角（以度为单位）
        /// </summary>
        public TSVector eulerAngles
        {
            get
            {
                TSVector result = new TSVector();

                // 通过矩阵元素计算欧拉角（X、Y、Z轴）
                // 使用反正切函数从矩阵元素中提取旋转角度，并转换为度
                result.x = TSMath.Atan2(M32, M33) * FP.Rad2Deg;
                result.y = TSMath.Atan2(-M31, TSMath.Sqrt(M32 * M32 + M33 * M33)) * FP.Rad2Deg;
                result.z = TSMath.Atan2(M21, M11) * FP.Rad2Deg;

                // 返回取反后的欧拉角
                return result * -1;
            }
        }

        /// <summary>
        /// 根据偏航角（Yaw）、俯仰角（Pitch）和翻滚角（Roll）创建旋转矩阵
        /// </summary>
        /// <param name="yaw">偏航角（绕Y轴旋转），单位为度</param>
        /// <param name="pitch">俯仰角（绕X轴旋转），单位为度</param>
        /// <param name="roll">翻滚角（绕Z轴旋转），单位为度</param>
        /// <returns>创建的旋转矩阵</returns>
        public static TSMatrix CreateFromYawPitchRoll(FP yaw, FP pitch, FP roll)
        {
            TSMatrix matrix;
            TSQuaternion quaternion;
            // 先通过欧拉角创建四元数
            TSQuaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
            // 再通过四元数创建旋转矩阵
            CreateFromQuaternion(ref quaternion, out matrix);
            return matrix;
        }

        /// <summary>
        /// 创建绕X轴旋转的旋转矩阵
        /// </summary>
        /// <param name="radians">旋转角度（弧度）</param>
        /// <returns>绕X轴旋转的矩阵</returns>
        public static TSMatrix CreateRotationX(FP radians)
        {
            TSMatrix matrix;
            FP cosVal = FP.Cos(radians); // 计算角度的余弦值
            FP sinVal = FP.Sin(radians); // 计算角度的正弦值

            // 填充绕X轴旋转的矩阵元素
            matrix.M11 = FP.One;
            matrix.M12 = FP.Zero;
            matrix.M13 = FP.Zero;
            matrix.M21 = FP.Zero;
            matrix.M22 = cosVal;
            matrix.M23 = sinVal;
            matrix.M31 = FP.Zero;
            matrix.M32 = -sinVal;
            matrix.M33 = cosVal;
            return matrix;
        }

        /// <summary>
        /// 创建绕X轴旋转的旋转矩阵，并通过输出参数返回
        /// </summary>
        /// <param name="radians">旋转角度（弧度）</param>
        /// <param name="result">输出参数，用于存储创建的绕X轴旋转的矩阵</param>
        public static void CreateRotationX(FP radians, out TSMatrix result)
        {
            FP cosVal = FP.Cos(radians);
            FP sinVal = FP.Sin(radians);

            // 填充矩阵元素
            result.M11 = FP.One;
            result.M12 = FP.Zero;
            result.M13 = FP.Zero;
            result.M21 = FP.Zero;
            result.M22 = cosVal;
            result.M23 = sinVal;
            result.M31 = FP.Zero;
            result.M32 = -sinVal;
            result.M33 = cosVal;
        }

        /// <summary>
        /// 创建绕Y轴旋转的旋转矩阵
        /// </summary>
        /// <param name="radians">旋转角度（弧度）</param>
        /// <returns>绕Y轴旋转的矩阵</returns>
        public static TSMatrix CreateRotationY(FP radians)
        {
            TSMatrix matrix;
            FP cosVal = FP.Cos(radians);
            FP sinVal = FP.Sin(radians);

            // 填充绕Y轴旋转的矩阵元素
            matrix.M11 = cosVal;
            matrix.M12 = FP.Zero;
            matrix.M13 = -sinVal;
            matrix.M21 = FP.Zero;
            matrix.M22 = FP.One;
            matrix.M23 = FP.Zero;
            matrix.M31 = sinVal;
            matrix.M32 = FP.Zero;
            matrix.M33 = cosVal;
            return matrix;
        }

        /// <summary>
        /// 创建绕Y轴旋转的旋转矩阵，并通过输出参数返回
        /// </summary>
        /// <param name="radians">旋转角度（弧度）</param>
        /// <param name="result">输出参数，用于存储创建的绕Y轴旋转的矩阵</param>
        public static void CreateRotationY(FP radians, out TSMatrix result)
        {
            FP cosVal = FP.Cos(radians);
            FP sinVal = FP.Sin(radians);

            // 填充矩阵元素
            result.M11 = cosVal;
            result.M12 = FP.Zero;
            result.M13 = -sinVal;
            result.M21 = FP.Zero;
            result.M22 = FP.One;
            result.M23 = FP.Zero;
            result.M31 = sinVal;
            result.M32 = FP.Zero;
            result.M33 = cosVal;
        }

        /// <summary>
        /// 创建绕Z轴旋转的旋转矩阵
        /// </summary>
        /// <param name="radians">旋转角度（弧度）</param>
        /// <returns>绕Z轴旋转的矩阵</returns>
        public static TSMatrix CreateRotationZ(FP radians)
        {
            TSMatrix matrix;
            FP cosVal = FP.Cos(radians);
            FP sinVal = FP.Sin(radians);

            // 填充绕Z轴旋转的矩阵元素
            matrix.M11 = cosVal;
            matrix.M12 = sinVal;
            matrix.M13 = FP.Zero;
            matrix.M21 = -sinVal;
            matrix.M22 = cosVal;
            matrix.M23 = FP.Zero;
            matrix.M31 = FP.Zero;
            matrix.M32 = FP.Zero;
            matrix.M33 = FP.One;
            return matrix;
        }

        /// <summary>
        /// 创建绕Z轴旋转的旋转矩阵，并通过输出参数返回
        /// </summary>
        /// <param name="radians">旋转角度（弧度）</param>
        /// <param name="result">输出参数，用于存储创建的绕Z轴旋转的矩阵</param>
        public static void CreateRotationZ(FP radians, out TSMatrix result)
        {
            FP cosVal = FP.Cos(radians);
            FP sinVal = FP.Sin(radians);

            // 填充矩阵元素
            result.M11 = cosVal;
            result.M12 = sinVal;
            result.M13 = FP.Zero;
            result.M21 = -sinVal;
            result.M22 = cosVal;
            result.M23 = FP.Zero;
            result.M31 = FP.Zero;
            result.M32 = FP.Zero;
            result.M33 = FP.One;
        }

        /// <summary>
        /// 初始化矩阵结构的新实例
        /// </summary>
        /// <param name="m11">第一行第一列元素</param>
        /// <param name="m12">第一行第二列元素</param>
        /// <param name="m13">第一行第三列元素</param>
        /// <param name="m21">第二行第一列元素</param>
        /// <param name="m22">第二行第二列元素</param>
        /// <param name="m23">第二行第三列元素</param>
        /// <param name="m31">第三行第一列元素</param>
        /// <param name="m32">第三行第二列元素</param>
        /// <param name="m33">第三行第三列元素</param>

        #region public JMatrix(FP m11, FP m12, FP m13, FP m21, FP m22, FP m23,FP m31, FP m32, FP m33)

        public TSMatrix(FP m11, FP m12, FP m13, FP m21, FP m22, FP m23, FP m31, FP m32, FP m33)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
        }

        #endregion

        /// <summary>
        /// 计算矩阵的行列式
        /// </summary>
        /// <returns>矩阵的行列式值</returns>
        public FP Determinant()
        {
            // 3x3矩阵行列式的计算公式
            return M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 -
                   M31 * M22 * M13 - M32 * M23 * M11 - M33 * M21 * M12;
        }

        /// <summary>
        /// 两个矩阵相乘（矩阵乘法不满足交换律）
        /// </summary>
        /// <param name="matrix1">第一个矩阵</param>
        /// <param name="matrix2">第二个矩阵</param>
        /// <returns>两个矩阵的乘积</returns>

        #region public static JMatrix Multiply(JMatrix matrix1, JMatrix matrix2)

        public static TSMatrix Multiply(TSMatrix matrix1, TSMatrix matrix2)
        {
            TSMatrix result;
            TSMatrix.Multiply(ref matrix1, ref matrix2, out result);
            return result;
        }

        /// <summary>
        /// 两个矩阵相乘，并通过输出参数返回结果（矩阵乘法不满足交换律）
        /// </summary>
        /// <param name="matrix1">第一个矩阵</param>
        /// <param name="matrix2">第二个矩阵</param>
        /// <param name="result">输出参数，存储两个矩阵的乘积</param>
        public static void Multiply(ref TSMatrix matrix1, ref TSMatrix matrix2, out TSMatrix result)
        {
            // 按照矩阵乘法规则计算每个元素的值
            FP num0 = ((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31);
            FP num1 = ((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32);
            FP num2 = ((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33);
            FP num3 = ((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31);
            FP num4 = ((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32);
            FP num5 = ((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M31);
            FP num6 = ((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31);
            FP num7 = ((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32);
            FP num8 = ((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33);

            // 为结果矩阵的各元素赋值
            result.M11 = num0;
            result.M12 = num1;
            result.M13 = num2;
            result.M21 = num3;
            result.M22 = num4;
            result.M23 = num5;
            result.M31 = num6;
            result.M32 = num7;
            result.M33 = num8;
        }

        #endregion

        /// <summary>
        /// 两个矩阵相加
        /// </summary>
        /// <param name="matrix1">第一个矩阵</param>
        /// <param name="matrix2">第二个矩阵</param>
        /// <returns>两个矩阵的和</returns>

        #region public static JMatrix Add(JMatrix matrix1, JMatrix matrix2)

        public static TSMatrix Add(TSMatrix matrix1, TSMatrix matrix2)
        {
            TSMatrix result;
            TSMatrix.Add(ref matrix1, ref matrix2, out result);
            return result;
        }

        /// <summary>
        /// 两个矩阵相加，并通过输出参数返回结果
        /// </summary>
        /// <param name="matrix1">第一个矩阵</param>
        /// <param name="matrix2">第二个矩阵</param>
        /// <param name="result">输出参数，存储两个矩阵的和</param>
        public static void Add(ref TSMatrix matrix1, ref TSMatrix matrix2, out TSMatrix result)
        {
            // 对应元素相加
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
        }

        #endregion

        /// <summary>
        /// 计算给定矩阵的逆矩阵
        /// </summary>
        /// <param name="matrix">要求逆的矩阵</param>
        /// <returns>逆矩阵</returns>

        #region public static JMatrix Inverse(JMatrix matrix)

        public static TSMatrix Inverse(TSMatrix matrix)
        {
            TSMatrix result;
            TSMatrix.Inverse(ref matrix, out result);
            return result;
        }

        /// <summary>
        /// 计算矩阵的逆矩阵，并通过输出参数返回（内部实现1）
        /// </summary>
        /// <param name="matrix">要进行逆运算的矩阵</param>
        /// <param name="result">输出参数，存储逆矩阵结果</param>
        public static void Invert(ref TSMatrix matrix, out TSMatrix result)
        {
            // 计算行列式的倒数
            FP determinantInverse = 1 / matrix.Determinant();

            // 根据伴随矩阵除以行列式的方法计算逆矩阵各元素
            FP m11 = (matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32) * determinantInverse;
            FP m12 = (matrix.M13 * matrix.M32 - matrix.M33 * matrix.M12) * determinantInverse;
            FP m13 = (matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13) * determinantInverse;

            FP m21 = (matrix.M23 * matrix.M31 - matrix.M21 * matrix.M33) * determinantInverse;
            FP m22 = (matrix.M11 * matrix.M33 - matrix.M13 * matrix.M31) * determinantInverse;
            FP m23 = (matrix.M13 * matrix.M21 - matrix.M11 * matrix.M23) * determinantInverse;

            FP m31 = (matrix.M21 * matrix.M32 - matrix.M22 * matrix.M31) * determinantInverse;
            FP m32 = (matrix.M12 * matrix.M31 - matrix.M11 * matrix.M32) * determinantInverse;
            FP m33 = (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21) * determinantInverse;

            // 为结果矩阵赋值
            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// 计算矩阵的逆矩阵，并通过输出参数返回（内部实现2，使用缩放因子减少精度损失）
        /// </summary>
        /// <param name="matrix">要进行逆运算的矩阵</param>
        /// <param name="result">输出参数，存储逆矩阵结果</param>
        public static void Inverse(ref TSMatrix matrix, out TSMatrix result)
        {
            // 计算行列式（乘以1024以减少精度损失）
            FP det = 1024 * matrix.M11 * matrix.M22 * matrix.M33 -
                     1024 * matrix.M11 * matrix.M23 * matrix.M32 -
                     1024 * matrix.M12 * matrix.M21 * matrix.M33 +
                     1024 * matrix.M12 * matrix.M23 * matrix.M31 +
                     1024 * matrix.M13 * matrix.M21 * matrix.M32 -
                     1024 * matrix.M13 * matrix.M22 * matrix.M31;

            // 计算伴随矩阵元素（乘以1024）
            FP num11 = 1024 * matrix.M22 * matrix.M33 - 1024 * matrix.M23 * matrix.M32;
            FP num12 = 1024 * matrix.M13 * matrix.M32 - 1024 * matrix.M12 * matrix.M33;
            FP num13 = 1024 * matrix.M12 * matrix.M23 - 1024 * matrix.M22 * matrix.M13;

            FP num21 = 1024 * matrix.M23 * matrix.M31 - 1024 * matrix.M33 * matrix.M21;
            FP num22 = 1024 * matrix.M11 * matrix.M33 - 1024 * matrix.M31 * matrix.M13;
            FP num23 = 1024 * matrix.M13 * matrix.M21 - 1024 * matrix.M23 * matrix.M11;

            FP num31 = 1024 * matrix.M21 * matrix.M32 - 1024 * matrix.M31 * matrix.M22;
            FP num32 = 1024 * matrix.M12 * matrix.M31 - 1024 * matrix.M32 * matrix.M11;
            FP num33 = 1024 * matrix.M11 * matrix.M22 - 1024 * matrix.M21 * matrix.M12;

            // 如果行列式为0，矩阵不可逆，结果设为无穷大
            if (det == 0)
            {
                result.M11 = FP.PositiveInfinity;
                result.M12 = FP.PositiveInfinity;
                result.M13 = FP.PositiveInfinity;
                result.M21 = FP.PositiveInfinity;
                result.M22 = FP.PositiveInfinity;
                result.M23 = FP.PositiveInfinity;
                result.M31 = FP.PositiveInfinity;
                result.M32 = FP.PositiveInfinity;
                result.M33 = FP.PositiveInfinity;
            }
            else
            {
                // 伴随矩阵除以行列式得到逆矩阵
                result.M11 = num11 / det;
                result.M12 = num12 / det;
                result.M13 = num13 / det;
                result.M21 = num21 / det;
                result.M22 = num22 / det;
                result.M23 = num23 / det;
                result.M31 = num31 / det;
                result.M32 = num32 / det;
                result.M33 = num33 / det;
            }
        }

        #endregion

        /// <summary>
        /// 矩阵与缩放因子相乘
        /// </summary>
        /// <param name="matrix1">要缩放的矩阵</param>
        /// <param name="scaleFactor">缩放因子</param>
        /// <returns>缩放后的矩阵</returns>

        #region public static JMatrix Multiply(JMatrix matrix1, FP scaleFactor)

        public static TSMatrix Multiply(TSMatrix matrix1, FP scaleFactor)
        {
            TSMatrix result;
            TSMatrix.Multiply(ref matrix1, scaleFactor, out result);
            return result;
        }

        /// <summary>
        /// 矩阵与缩放因子相乘，并通过输出参数返回结果
        /// </summary>
        /// <param name="matrix1">要缩放的矩阵</param>
        /// <param name="scaleFactor">缩放因子</param>
        /// <param name="result">输出参数，存储缩放后的矩阵</param>
        public static void Multiply(ref TSMatrix matrix1, FP scaleFactor, out TSMatrix result)
        {
            FP num = scaleFactor;
            // 每个元素都乘以缩放因子
            result.M11 = matrix1.M11 * num;
            result.M12 = matrix1.M12 * num;
            result.M13 = matrix1.M13 * num;
            result.M21 = matrix1.M21 * num;
            result.M22 = matrix1.M22 * num;
            result.M23 = matrix1.M23 * num;
            result.M31 = matrix1.M31 * num;
            result.M32 = matrix1.M32 * num;
            result.M33 = matrix1.M33 * num;
        }

        #endregion

        /// <summary>
        /// 根据位置和目标点创建观察矩阵（LookAt矩阵）
        /// </summary>
        /// <param name="position">观察者位置</param>
        /// <param name="target">目标点位置</param>
        /// <returns>创建的观察矩阵</returns>
        public static TSMatrix CreateFromLookAt(TSVector position, TSVector target)
        {
            TSMatrix result;
            // 计算前向向量（目标减位置），并使用上方向向量创建观察矩阵
            LookAt(target - position, TSVector.up, out result);
            return result;
        }

        /// <summary>
        /// 根据前向向量和上方向向量创建观察矩阵
        /// </summary>
        /// <param name="forward">前向向量</param>
        /// <param name="upwards">上方向向量</param>
        /// <returns>创建的观察矩阵</returns>
        public static TSMatrix LookAt(TSVector forward, TSVector upwards)
        {
            TSMatrix result;
            LookAt(forward, upwards, out result);
            return result;
        }

        /// <summary>
        /// 根据前向向量和上方向向量创建观察矩阵，并通过输出参数返回
        /// </summary>
        /// <param name="forward">前向向量</param>
        /// <param name="upwards">上方向向量</param>
        /// <param name="result">输出参数，存储创建的观察矩阵</param>
        public static void LookAt(TSVector forward, TSVector upwards, out TSMatrix result)
        {
            // 标准化前向向量
            TSVector zaxis = forward;
            zaxis.Normalize();
            // 计算右方向向量（上方向与前向向量的叉乘）并标准化
            TSVector xaxis = TSVector.Cross(upwards, zaxis);
            xaxis.Normalize();
            // 计算修正后的上方向向量（前向向量与右方向向量的叉乘）
            TSVector yaxis = TSVector.Cross(zaxis, xaxis);

            // 填充观察矩阵（行优先存储的旋转矩阵）
            result.M11 = xaxis.x;
            result.M21 = yaxis.x;
            result.M31 = zaxis.x;
            result.M12 = xaxis.y;
            result.M22 = yaxis.y;
            result.M32 = zaxis.y;
            result.M13 = xaxis.z;
            result.M23 = yaxis.z;
            result.M33 = zaxis.z;
        }

        /// <summary>
        /// 从四元数创建表示方向的矩阵
        /// </summary>
        /// <param name="quaternion">用于创建矩阵的四元数</param>
        /// <returns>表示方向的矩阵</returns>

        #region public static JMatrix CreateFromQuaternion(JQuaternion quaternion)

        public static TSMatrix CreateFromQuaternion(TSQuaternion quaternion)
        {
            TSMatrix result;
            TSMatrix.CreateFromQuaternion(ref quaternion, out result);
            return result;
        }

        /// <summary>
        /// 从四元数创建表示方向的矩阵，并通过输出参数返回
        /// </summary>
        /// <param name="quaternion">用于创建矩阵的四元数</param>
        /// <param name="result">输出参数，存储创建的矩阵</param>
        public static void CreateFromQuaternion(ref TSQuaternion quaternion, out TSMatrix result)
        {
            // 计算四元数各分量的平方
            FP x2 = quaternion.x * quaternion.x;
            FP y2 = quaternion.y * quaternion.y;
            FP z2 = quaternion.z * quaternion.z;
            // 计算四元数分量间的乘积
            FP xy = quaternion.x * quaternion.y;
            FP zw = quaternion.z * quaternion.w;
            FP zx = quaternion.z * quaternion.x;
            FP yw = quaternion.y * quaternion.w;
            FP yz = quaternion.y * quaternion.z;
            FP xw = quaternion.x * quaternion.w;

            // 根据四元数到矩阵的转换公式计算各元素
            result.M11 = FP.One - (2 * (y2 + z2));
            result.M12 = 2 * (xy + zw);
            result.M13 = 2 * (zx - yw);
            result.M21 = 2 * (xy - zw);
            result.M22 = FP.One - (2 * (z2 + x2));
            result.M23 = 2 * (yz + xw);
            result.M31 = 2 * (zx + yw);
            result.M32 = 2 * (yz - xw);
            result.M33 = FP.One - (2 * (y2 + x2));
        }

        #endregion

        /// <summary>
        /// 计算矩阵的转置矩阵
        /// </summary>
        /// <param name="matrix">要转置的矩阵</param>
        /// <returns>转置后的矩阵</returns>

        #region public static JMatrix Transpose(JMatrix matrix)

        public static TSMatrix Transpose(TSMatrix matrix)
        {
            TSMatrix result;
            TSMatrix.Transpose(ref matrix, out result);
            return result;
        }

        /// <summary>
        /// 计算矩阵的转置矩阵，并通过输出参数返回
        /// </summary>
        /// <param name="matrix">要转置的矩阵</param>
        /// <param name="result">输出参数，存储转置后的矩阵</param>
        public static void Transpose(ref TSMatrix matrix, out TSMatrix result)
        {
            // 转置矩阵的元素(i,j)等于原矩阵的元素(j,i)
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
        }

        #endregion

        /// <summary>
        /// 重载乘法运算符，实现两个矩阵相乘
        /// </summary>
        /// <param name="value1">第一个矩阵</param>
        /// <param name="value2">第二个矩阵</param>
        /// <returns>两个矩阵的乘积</returns>

        #region public static JMatrix operator *(JMatrix value1,JMatrix value2)

        public static TSMatrix operator *(TSMatrix value1, TSMatrix value2)
        {
            TSMatrix result;
            TSMatrix.Multiply(ref value1, ref value2, out result);
            return result;
        }

        #endregion

        /// <summary>
        /// 计算矩阵的迹（主对角线元素之和）
        /// </summary>
        /// <returns>矩阵的迹</returns>
        public FP Trace()
        {
            return this.M11 + this.M22 + this.M33;
        }

        /// <summary>
        /// 重载加法运算符，实现两个矩阵相加
        /// </summary>
        /// <param name="value1">第一个矩阵</param>
        /// <param name="value2">第二个矩阵</param>
        /// <returns>两个矩阵的和</returns>

        #region public static JMatrix operator +(JMatrix value1, JMatrix value2)

        public static TSMatrix operator +(TSMatrix value1, TSMatrix value2)
        {
            TSMatrix result;
            TSMatrix.Add(ref value1, ref value2, out result);
            return result;
        }

        #endregion

        /// <summary>
        /// 重载减法运算符，实现两个矩阵相减
        /// </summary>
        /// <param name="value1">第一个矩阵（被减数）</param>
        /// <param name="value2">第二个矩阵（减数）</param>
        /// <returns>两个矩阵的差</returns>

        #region public static JMatrix operator -(JMatrix value1, JMatrix value2)

        public static TSMatrix operator -(TSMatrix value1, TSMatrix value2)
        {
            // 先将减数矩阵乘以-1，再与被减数矩阵相加
            TSMatrix result;
            TSMatrix.Multiply(ref value2, -FP.One, out value2);
            TSMatrix.Add(ref value1, ref value2, out result);
            return result;
        }

        #endregion

        /// <summary>
        /// 重载相等运算符，判断两个矩阵是否相等
        /// </summary>
        /// <param name="value1">第一个矩阵</param>
        /// <param name="value2">第二个矩阵</param>
        /// <returns>如果两个矩阵所有对应元素都相等则返回true，否则返回false</returns>
        public static bool operator ==(TSMatrix value1, TSMatrix value2)
        {
            return value1.M11 == value2.M11 &&
                   value1.M12 == value2.M12 &&
                   value1.M13 == value2.M13 &&
                   value1.M21 == value2.M21 &&
                   value1.M22 == value2.M22 &&
                   value1.M23 == value2.M23 &&
                   value1.M31 == value2.M31 &&
                   value1.M32 == value2.M32 &&
                   value1.M33 == value2.M33;
        }

        /// <summary>
        /// 重载不等运算符，判断两个矩阵是否不相等
        /// </summary>
        /// <param name="value1">第一个矩阵</param>
        /// <param name="value2">第二个矩阵</param>
        /// <returns>如果两个矩阵有任何对应元素不相等则返回true，否则返回false</returns>
        public static bool operator !=(TSMatrix value1, TSMatrix value2)
        {
            return value1.M11 != value2.M11 ||
                   value1.M12 != value2.M12 ||
                   value1.M13 != value2.M13 ||
                   value1.M21 != value2.M21 ||
                   value1.M22 != value2.M22 ||
                   value1.M23 != value2.M23 ||
                   value1.M31 != value2.M31 ||
                   value1.M32 != value2.M32 ||
                   value1.M33 != value2.M33;
        }

        /// <summary>
        /// 判断当前对象是否与指定对象相等
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>如果相等则返回true，否则返回false</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is TSMatrix)) return false;
            TSMatrix other = (TSMatrix)obj;

            // 比较所有对应元素
            return this.M11 == other.M11 &&
                   this.M12 == other.M12 &&
                   this.M13 == other.M13 &&
                   this.M21 == other.M21 &&
                   this.M22 == other.M22 &&
                   this.M23 == other.M23 &&
                   this.M31 == other.M31 &&
                   this.M32 == other.M32 &&
                   this.M33 == other.M33;
        }

        /// <summary>
        /// 获取当前对象的哈希码
        /// </summary>
        /// <returns>哈希码值</returns>
        public override int GetHashCode()
        {
            // 结合所有元素的哈希码生成当前对象的哈希码
            return M11.GetHashCode() ^
                   M12.GetHashCode() ^
                   M13.GetHashCode() ^
                   M21.GetHashCode() ^
                   M22.GetHashCode() ^
                   M23.GetHashCode() ^
                   M31.GetHashCode() ^
                   M32.GetHashCode() ^
                   M33.GetHashCode();
        }

        /// <summary>
        /// 创建一个绕指定轴旋转指定角度的旋转矩阵，并通过输出参数返回
        /// </summary>
        /// <param name="axis">旋转轴</param>
        /// <param name="angle">旋转角度（弧度）</param>
        /// <param name="result">输出参数，存储创建的旋转矩阵</param>

        #region public static void CreateFromAxisAngle(ref JVector axis, FP angle, out JMatrix result)

        public static void CreateFromAxisAngle(ref TSVector axis, FP angle, out TSMatrix result)
        {
            // 获取旋转轴的分量
            FP x = axis.x;
            FP y = axis.y;
            FP z = axis.z;
            // 计算角度的正弦和余弦值
            FP sinAngle = FP.Sin(angle);
            FP cosAngle = FP.Cos(angle);
            // 计算旋转轴分量的平方
            FP x2 = x * x;
            FP y2 = y * y;
            FP z2 = z * z;
            // 计算旋转轴分量间的乘积
            FP xy = x * y;
            FP xz = x * z;
            FP yz = y * z;

            // 根据轴角旋转矩阵公式计算各元素
            result.M11 = x2 + (cosAngle * (FP.One - x2));
            result.M12 = (xy - (cosAngle * xy)) + (sinAngle * z);
            result.M13 = (xz - (cosAngle * xz)) - (sinAngle * y);
            result.M21 = (xy - (cosAngle * xy)) - (sinAngle * z);
            result.M22 = y2 + (cosAngle * (FP.One - y2));
            result.M23 = (yz - (cosAngle * yz)) + (sinAngle * x);
            result.M31 = (xz - (cosAngle * xz)) + (sinAngle * y);
            result.M32 = (yz - (cosAngle * yz)) - (sinAngle * x);
            result.M33 = z2 + (cosAngle * (FP.One - z2));
        }

        /// <summary>
        /// 创建一个绕指定轴旋转指定角度的旋转矩阵
        /// </summary>
        /// <param name="angle">旋转角度（弧度）</param>
        /// <param name="axis">旋转轴</param>
        /// <returns>创建的旋转矩阵</returns>
        public static TSMatrix AngleAxis(FP angle, TSVector axis)
        {
            TSMatrix result;
            CreateFromAxisAngle(ref axis, angle, out result);
            return result;
        }

        #endregion

        /// <summary>
        /// 将矩阵转换为字符串表示形式
        /// </summary>
        /// <returns>矩阵的字符串表示</returns>
        public override string ToString()
        {
            // 以特定格式输出矩阵各元素的原始值
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}",
                M11.RawValue, M12.RawValue, M13.RawValue,
                M21.RawValue, M22.RawValue, M23.RawValue,
                M31.RawValue, M32.RawValue, M33.RawValue);
        }
    }
}