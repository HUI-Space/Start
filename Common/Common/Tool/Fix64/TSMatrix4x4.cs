/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
 *
 *  本软件按"原样"提供，不附带任何明示或暗示的担保。
 *  作者不对因使用本软件而产生的任何损害承担责任。
 *
 *  允许任何人将本软件用于任何目的，包括商业应用，并可自由修改和再分发，
 *  但需遵守以下限制：
 *
 *  1. 不得歪曲本软件的来源；不得声称自己编写了原始软件。
 *     如果在产品中使用本软件，在产品文档中致谢是值得赞赏的，但非必需。
 *  2. 修改后的源版本必须明确标记，不得歪曲为原始软件。
 *  3. 本声明不得从任何源分发中删除或修改。
 */

namespace TrueSync
{
    /// <summary>
    /// 4x4矩阵结构，用于表示三维空间中的变换（旋转、平移、缩放等）
    /// 矩阵元素采用行优先存储，M[row][column]对应M[row][column]
    /// </summary>
    public struct TSMatrix4x4
    {
        /// <summary>
        /// 矩阵第一行第一列元素
        /// </summary>
        public FP M11; // 第一行向量的第一个分量

        /// <summary>
        /// 矩阵第一行第二列元素
        /// </summary>
        public FP M12;

        /// <summary>
        /// 矩阵第一行第三列元素
        /// </summary>
        public FP M13;

        /// <summary>
        /// 矩阵第一行第四列元素
        /// </summary>
        public FP M14;

        /// <summary>
        /// 矩阵第二行第一列元素
        /// </summary>
        public FP M21; // 第二行向量的第一个分量

        /// <summary>
        /// 矩阵第二行第二列元素
        /// </summary>
        public FP M22;

        /// <summary>
        /// 矩阵第二行第三列元素
        /// </summary>
        public FP M23;

        /// <summary>
        /// 矩阵第二行第四列元素
        /// </summary>
        public FP M24;

        /// <summary>
        /// 矩阵第三行第一列元素
        /// </summary>
        public FP M31; // 第三行向量的第一个分量

        /// <summary>
        /// 矩阵第三行第二列元素
        /// </summary>
        public FP M32;

        /// <summary>
        /// 矩阵第三行第三列元素
        /// </summary>
        public FP M33;

        /// <summary>
        /// 矩阵第三行第四列元素
        /// </summary>
        public FP M34;

        /// <summary>
        /// 矩阵第四行第一列元素
        /// </summary>
        public FP M41; // 第四行向量的第一个分量

        /// <summary>
        /// 矩阵第四行第二列元素
        /// </summary>
        public FP M42;

        /// <summary>
        /// 矩阵第四行第三列元素
        /// </summary>
        public FP M43;

        /// <summary>
        /// 矩阵第四行第四列元素
        /// </summary>
        public FP M44;

        /// <summary>
        /// 内部使用的单位矩阵实例
        /// </summary>
        internal static TSMatrix4x4 InternalIdentity;

        /// <summary>
        /// 单位矩阵（对角线元素为1，其余为0）
        /// </summary>
        public static readonly TSMatrix4x4 Identity;

        /// <summary>
        /// 零矩阵（所有元素均为0）
        /// </summary>
        public static readonly TSMatrix4x4 Zero;

        /// <summary>
        /// 静态构造函数，初始化单位矩阵和零矩阵
        /// </summary>
        static TSMatrix4x4()
        {
            Zero = new TSMatrix4x4(); // 默认初始化所有元素为0

            Identity = new TSMatrix4x4();
            Identity.M11 = FP.One; // 第一行第一列设为1
            Identity.M22 = FP.One; // 第二行第二列设为1
            Identity.M33 = FP.One; // 第三行第三列设为1
            Identity.M44 = FP.One; // 第四行第四列设为1

            InternalIdentity = Identity; // 内部引用单位矩阵
        }

        /// <summary>
        /// 初始化4x4矩阵的新实例，通过指定所有16个元素的值
        /// </summary>
        /// <param name="m11">第一行第一列元素</param>
        /// <param name="m12">第一行第二列元素</param>
        /// <param name="m13">第一行第三列元素</param>
        /// <param name="m14">第一行第四列元素</param>
        /// <param name="m21">第二行第一列元素</param>
        /// <param name="m22">第二行第二列元素</param>
        /// <param name="m23">第二行第三列元素</param>
        /// <param name="m24">第二行第四列元素</param>
        /// <param name="m31">第三行第一列元素</param>
        /// <param name="m32">第三行第二列元素</param>
        /// <param name="m33">第三行第三列元素</param>
        /// <param name="m34">第三行第四列元素</param>
        /// <param name="m41">第四行第一列元素</param>
        /// <param name="m42">第四行第二列元素</param>
        /// <param name="m43">第四行第三列元素</param>
        /// <param name="m44">第四行第四列元素</param>
        public TSMatrix4x4(FP m11, FP m12, FP m13, FP m14,
            FP m21, FP m22, FP m23, FP m24,
            FP m31, FP m32, FP m33, FP m34,
            FP m41, FP m42, FP m43, FP m44)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;
            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
        }

        /// <summary>
        /// 矩阵乘法运算（注意：矩阵乘法不满足交换律）
        /// </summary>
        /// <param name="matrix1">第一个矩阵（左操作数）</param>
        /// <param name="matrix2">第二个矩阵（右操作数）</param>
        /// <returns>两个矩阵的乘积</returns>
        public static TSMatrix4x4 Multiply(TSMatrix4x4 matrix1, TSMatrix4x4 matrix2)
        {
            TSMatrix4x4 result;
            TSMatrix4x4.Multiply(ref matrix1, ref matrix2, out result);
            return result;
        }

        /// <summary>
        /// 矩阵乘法运算（引用传递版本，减少值类型复制开销）
        /// </summary>
        /// <param name="matrix1">第一个矩阵（左操作数）</param>
        /// <param name="matrix2">第二个矩阵（右操作数）</param>
        /// <param name="result">输出参数，存储两个矩阵的乘积</param>
        public static void Multiply(ref TSMatrix4x4 matrix1, ref TSMatrix4x4 matrix2, out TSMatrix4x4 result)
        {
            // 计算结果矩阵的第一行（行=矩阵1的行，列=矩阵2的列）
            result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 +
                         matrix1.M14 * matrix2.M41;
            result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 +
                         matrix1.M14 * matrix2.M42;
            result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 +
                         matrix1.M14 * matrix2.M43;
            result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 +
                         matrix1.M14 * matrix2.M44;

            // 计算结果矩阵的第二行
            result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 +
                         matrix1.M24 * matrix2.M41;
            result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 +
                         matrix1.M24 * matrix2.M42;
            result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 +
                         matrix1.M24 * matrix2.M43;
            result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 +
                         matrix1.M24 * matrix2.M44;

            // 计算结果矩阵的第三行
            result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 +
                         matrix1.M34 * matrix2.M41;
            result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 +
                         matrix1.M34 * matrix2.M42;
            result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 +
                         matrix1.M34 * matrix2.M43;
            result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 +
                         matrix1.M34 * matrix2.M44;

            // 计算结果矩阵的第四行
            result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 +
                         matrix1.M44 * matrix2.M41;
            result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 +
                         matrix1.M44 * matrix2.M42;
            result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 +
                         matrix1.M44 * matrix2.M43;
            result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 +
                         matrix1.M44 * matrix2.M44;
        }

        /// <summary>
        /// 矩阵加法运算
        /// </summary>
        /// <param name="matrix1">第一个矩阵</param>
        /// <param name="matrix2">第二个矩阵</param>
        /// <returns>两个矩阵的和（对应元素相加）</returns>
        public static TSMatrix4x4 Add(TSMatrix4x4 matrix1, TSMatrix4x4 matrix2)
        {
            TSMatrix4x4 result;
            TSMatrix4x4.Add(ref matrix1, ref matrix2, out result);
            return result;
        }

        /// <summary>
        /// 矩阵加法运算（引用传递版本）
        /// </summary>
        /// <param name="matrix1">第一个矩阵</param>
        /// <param name="matrix2">第二个矩阵</param>
        /// <param name="result">输出参数，存储两个矩阵的和</param>
        public static void Add(ref TSMatrix4x4 matrix1, ref TSMatrix4x4 matrix2, out TSMatrix4x4 result)
        {
            // 第一行元素相加
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M14 = matrix1.M14 + matrix2.M14;

            // 第二行元素相加
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M24 = matrix1.M24 + matrix2.M24;

            // 第三行元素相加
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
            result.M34 = matrix1.M34 + matrix2.M34;

            // 第四行元素相加
            result.M41 = matrix1.M41 + matrix2.M41;
            result.M42 = matrix1.M42 + matrix2.M42;
            result.M43 = matrix1.M43 + matrix2.M43;
            result.M44 = matrix1.M44 + matrix2.M44;
        }

        /// <summary>
        /// 计算矩阵的逆矩阵
        /// </summary>
        /// <param name="matrix">要计算逆矩阵的矩阵</param>
        /// <returns>矩阵的逆矩阵，如果矩阵不可逆则返回元素为无穷大的矩阵</returns>
        public static TSMatrix4x4 Inverse(TSMatrix4x4 matrix)
        {
            TSMatrix4x4 result;
            TSMatrix4x4.Inverse(ref matrix, out result);
            return result;
        }

        /// <summary>
        /// 矩阵的行列式值（用于判断矩阵是否可逆：行列式不为0则可逆）
        /// </summary>
        public FP determinant
        {
            get
            {
                // 行列式计算基于4x4矩阵的展开式：
                // | a b c d |     | f g h |     | e g h |     | e f h |     | e f g |
                // | e f g h | = a | j k l | - b | i k l | + c | i j l | - d | i j k |
                // | i j k l |     | n o p |     | m o p |     | m n p |     | m n o |
                // | m n o p |
                //
                // 各项子行列式计算：
                //   | f g h |
                // a | j k l | = a ( f ( kp - lo ) - g ( jp - ln ) + h ( jo - kn ) )
                //   | n o p |
                //
                //   | e g h |     
                // b | i k l | = b ( e ( kp - lo ) - g ( ip - lm ) + h ( io - km ) )
                //   | m o p |     
                //
                //   | e f h |
                // c | i j l | = c ( e ( jp - ln ) - f ( ip - lm ) + h ( in - jm ) )
                //   | m n p |
                //
                //   | e f g |
                // d | i j k | = d ( e ( jo - kn ) - f ( io - km ) + g ( in - jm ) )
                //   | m n o |
                //
                // 运算复杂度：17次加法和28次乘法

                // 提取矩阵元素到局部变量，简化计算
                FP a = M11, b = M12, c = M13, d = M14;
                FP e = M21, f = M22, g = M23, h = M24;
                FP i = M31, j = M32, k = M33, l = M34;
                FP m = M41, n = M42, o = M43, p = M44;

                // 预计算子表达式，减少重复计算
                FP kp_lo = k * p - l * o;
                FP jp_ln = j * p - l * n;
                FP jo_kn = j * o - k * n;
                FP ip_lm = i * p - l * m;
                FP io_km = i * o - k * m;
                FP in_jm = i * n - j * m;

                // 组合各项计算行列式值
                return a * (f * kp_lo - g * jp_ln + h * jo_kn) -
                       b * (e * kp_lo - g * ip_lm + h * io_km) +
                       c * (e * jp_ln - f * ip_lm + h * in_jm) -
                       d * (e * jo_kn - f * io_km + g * in_jm);
            }
        }

        /// <summary>
        /// 计算矩阵的逆矩阵（引用传递版本）
        /// </summary>
        /// <param name="matrix">要计算逆矩阵的矩阵</param>
        /// <param name="result">输出参数，存储逆矩阵结果</param>
        public static void Inverse(ref TSMatrix4x4 matrix, out TSMatrix4x4 result)
        {
            // 逆矩阵计算原理：
            //                                       -1
            // 若有矩阵M，则其逆矩阵M   可通过以下公式计算
            //
            //     -1       1      
            //    M   = --------- * A
            //            det(M)
            //
            // 其中A是M的伴随矩阵（adjugate），满足：
            //
            //      T
            // A = C
            //
            // C是M的余子式矩阵（Cofactor matrix），其中：
            //           i + j
            // C   = (-1)      * det(M  )
            //  ij                    ij
            //
            // 矩阵M的形式：
            //     [ a b c d ]
            // M = [ e f g h ]
            //     [ i j k l ]
            //     [ m n o p ]
            //
            // 第一行余子式计算：
            //           2 | f g h |
            // C   = (-1)  | j k l | = + ( f ( kp - lo ) - g ( jp - ln ) + h ( jo - kn ) )
            //  11         | n o p |
            //
            //           3 | e g h |
            // C   = (-1)  | i k l | = - ( e ( kp - lo ) - g ( ip - lm ) + h ( io - km ) )
            //  12         | m o p |
            //
            // （其余余子式计算过程略，遵循相同规则）
            //
            // 运算复杂度：53次加法，104次乘法，1次除法

            // 提取矩阵元素到局部变量
            FP a = matrix.M11, b = matrix.M12, c = matrix.M13, d = matrix.M14;
            FP e = matrix.M21, f = matrix.M22, g = matrix.M23, h = matrix.M24;
            FP i = matrix.M31, j = matrix.M32, k = matrix.M33, l = matrix.M34;
            FP m = matrix.M41, n = matrix.M42, o = matrix.M43, p = matrix.M44;

            // 预计算子表达式，减少重复计算
            FP kp_lo = k * p - l * o;
            FP jp_ln = j * p - l * n;
            FP jo_kn = j * o - k * n;
            FP ip_lm = i * p - l * m;
            FP io_km = i * o - k * m;
            FP in_jm = i * n - j * m;

            // 计算伴随矩阵第一列元素（余子式矩阵的第一行转置后的值）
            FP a11 = (f * kp_lo - g * jp_ln + h * jo_kn);
            FP a12 = -(e * kp_lo - g * ip_lm + h * io_km);
            FP a13 = (e * jp_ln - f * ip_lm + h * in_jm);
            FP a14 = -(e * jo_kn - f * io_km + g * in_jm);

            // 计算行列式值
            FP det = a * a11 + b * a12 + c * a13 + d * a14;

            // 判断矩阵是否可逆（行列式为0则不可逆）
            if (det == FP.Zero)
            {
                // 不可逆时，所有元素设为正无穷
                result.M11 = FP.PositiveInfinity;
                result.M12 = FP.PositiveInfinity;
                result.M13 = FP.PositiveInfinity;
                result.M14 = FP.PositiveInfinity;
                result.M21 = FP.PositiveInfinity;
                result.M22 = FP.PositiveInfinity;
                result.M23 = FP.PositiveInfinity;
                result.M24 = FP.PositiveInfinity;
                result.M31 = FP.PositiveInfinity;
                result.M32 = FP.PositiveInfinity;
                result.M33 = FP.PositiveInfinity;
                result.M34 = FP.PositiveInfinity;
                result.M41 = FP.PositiveInfinity;
                result.M42 = FP.PositiveInfinity;
                result.M43 = FP.PositiveInfinity;
                result.M44 = FP.PositiveInfinity;
            }
            else
            {
                // 计算行列式的倒数（用于后续缩放）
                FP invDet = FP.One / det;

                // 计算逆矩阵第一列（伴随矩阵第一列 * 1/det）
                result.M11 = a11 * invDet;
                result.M21 = a12 * invDet;
                result.M31 = a13 * invDet;
                result.M41 = a14 * invDet;

                // 计算逆矩阵第二列
                result.M12 = -(b * kp_lo - c * jp_ln + d * jo_kn) * invDet;
                result.M22 = (a * kp_lo - c * ip_lm + d * io_km) * invDet;
                result.M32 = -(a * jp_ln - b * ip_lm + d * in_jm) * invDet;
                result.M42 = (a * jo_kn - b * io_km + c * in_jm) * invDet;

                // 预计算更多子表达式
                FP gp_ho = g * p - h * o;
                FP fp_hn = f * p - h * n;
                FP fo_gn = f * o - g * n;
                FP ep_hm = e * p - h * m;
                FP eo_gm = e * o - g * m;
                FP en_fm = e * n - f * m;

                // 计算逆矩阵第三列
                result.M13 = (b * gp_ho - c * fp_hn + d * fo_gn) * invDet;
                result.M23 = -(a * gp_ho - c * ep_hm + d * eo_gm) * invDet;
                result.M33 = (a * fp_hn - b * ep_hm + d * en_fm) * invDet;
                result.M43 = -(a * fo_gn - b * eo_gm + c * en_fm) * invDet;

                // 预计算更多子表达式
                FP gl_hk = g * l - h * k;
                FP fl_hj = f * l - h * j;
                FP fk_gj = f * k - g * j;
                FP el_hi = e * l - h * i;
                FP ek_gi = e * k - g * i;
                FP ej_fi = e * j - f * i;

                // 计算逆矩阵第四列
                result.M14 = -(b * gl_hk - c * fl_hj + d * fk_gj) * invDet;
                result.M24 = (a * gl_hk - c * el_hi + d * ek_gi) * invDet;
                result.M34 = -(a * fl_hj - b * el_hi + d * ej_fi) * invDet;
                result.M44 = (a * fk_gj - b * ek_gi + c * ej_fi) * invDet;
            }
        }

        /// <summary>
        /// 矩阵与标量相乘（每个元素都乘以该标量）
        /// </summary>
        /// <param name="matrix1">要缩放的矩阵</param>
        /// <param name="scaleFactor">缩放因子</param>
        /// <returns>缩放后的矩阵</returns>
        public static TSMatrix4x4 Multiply(TSMatrix4x4 matrix1, FP scaleFactor)
        {
            TSMatrix4x4 result;
            TSMatrix4x4.Multiply(ref matrix1, scaleFactor, out result);
            return result;
        }

        /// <summary>
        /// 矩阵与标量相乘（引用传递版本）
        /// </summary>
        /// <param name="matrix1">要缩放的矩阵</param>
        /// <param name="scaleFactor">缩放因子</param>
        /// <param name="result">输出参数，存储缩放后的矩阵</param>
        public static void Multiply(ref TSMatrix4x4 matrix1, FP scaleFactor, out TSMatrix4x4 result)
        {
            FP num = scaleFactor;
            // 第一行元素乘以标量
            result.M11 = matrix1.M11 * num;
            result.M12 = matrix1.M12 * num;
            result.M13 = matrix1.M13 * num;
            result.M14 = matrix1.M14 * num;

            // 第二行元素乘以标量
            result.M21 = matrix1.M21 * num;
            result.M22 = matrix1.M22 * num;
            result.M23 = matrix1.M23 * num;
            result.M24 = matrix1.M24 * num;

            // 第三行元素乘以标量
            result.M31 = matrix1.M31 * num;
            result.M32 = matrix1.M32 * num;
            result.M33 = matrix1.M33 * num;
            result.M34 = matrix1.M34 * num;

            // 第四行元素乘以标量
            result.M41 = matrix1.M41 * num;
            result.M42 = matrix1.M42 * num;
            result.M43 = matrix1.M43 * num;
            result.M44 = matrix1.M44 * num;
        }


        /// <summary>
        /// 从四元数创建旋转矩阵（四元数表示三维空间中的旋转）
        /// </summary>
        /// <param name="quaternion">表示旋转的四元数</param>
        /// <returns>对应的旋转矩阵</returns>
        public static TSMatrix4x4 Rotate(TSQuaternion quaternion)
        {
            TSMatrix4x4 result;
            TSMatrix4x4.Rotate(ref quaternion, out result);
            return result;
        }

        /// <summary>
        /// 从四元数创建旋转矩阵（引用传递版本）
        /// </summary>
        /// <param name="quaternion">表示旋转的四元数</param>
        /// <param name="result">输出参数，存储对应的旋转矩阵</param>
        public static void Rotate(ref TSQuaternion quaternion, out TSMatrix4x4 result)
        {
            // 预计算四元数分量的乘积，减少重复计算
            FP x = quaternion.x * 2;
            FP y = quaternion.y * 2;
            FP z = quaternion.z * 2;
            FP xx = quaternion.x * x;
            FP yy = quaternion.y * y;
            FP zz = quaternion.z * z;
            FP xy = quaternion.x * y;
            FP xz = quaternion.x * z;
            FP yz = quaternion.y * z;
            FP wx = quaternion.w * x;
            FP wy = quaternion.w * y;
            FP wz = quaternion.w * z;

            // 根据四元数到旋转矩阵的转换公式计算3x3旋转部分
            result.M11 = FP.One - (yy + zz);
            result.M21 = xy + wz;
            result.M31 = xz - wy;
            result.M41 = FP.Zero; // 平移分量为0
            result.M12 = xy - wz;
            result.M22 = FP.One - (xx + zz);
            result.M32 = yz + wx;
            result.M42 = FP.Zero; // 平移分量为0
            result.M13 = xz + wy;
            result.M23 = yz - wx;
            result.M33 = FP.One - (xx + yy);
            result.M43 = FP.Zero; // 平移分量为0
            result.M14 = FP.Zero; // 齐次坐标分量
            result.M24 = FP.Zero;
            result.M34 = FP.Zero;
            result.M44 = FP.One; // 齐次坐标分量为1
        }

        /// <summary>
        /// 计算矩阵的转置矩阵（行列互换）
        /// </summary>
        /// <param name="matrix">要转置的矩阵</param>
        /// <returns>转置后的矩阵</returns>
        public static TSMatrix4x4 Transpose(TSMatrix4x4 matrix)
        {
            TSMatrix4x4 result;
            TSMatrix4x4.Transpose(ref matrix, out result);
            return result;
        }

        /// <summary>
        /// 计算矩阵的转置矩阵（引用传递版本）
        /// </summary>
        /// <param name="matrix">要转置的矩阵</param>
        /// <param name="result">输出参数，存储转置后的矩阵</param>
        public static void Transpose(ref TSMatrix4x4 matrix, out TSMatrix4x4 result)
        {
            // 转置矩阵的元素(i,j)等于原矩阵的元素(j,i)
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M14 = matrix.M41;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M24 = matrix.M42;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
            result.M34 = matrix.M43;
            result.M41 = matrix.M14;
            result.M42 = matrix.M24;
            result.M43 = matrix.M34;
            result.M44 = matrix.M44;
        }


        /// <summary>
        /// 重载乘法运算符，实现矩阵乘法
        /// </summary>
        /// <param name="value1">左操作数矩阵</param>
        /// <param name="value2">右操作数矩阵</param>
        /// <returns>两个矩阵的乘积</returns>
        public static TSMatrix4x4 operator *(TSMatrix4x4 value1, TSMatrix4x4 value2)
        {
            TSMatrix4x4 result;
            TSMatrix4x4.Multiply(ref value1, ref value2, out result);
            return result;
        }


        /// <summary>
        /// 计算矩阵的迹（主对角线元素之和）
        /// </summary>
        /// <returns>矩阵的迹</returns>
        public FP Trace()
        {
            return this.M11 + this.M22 + this.M33 + this.M44;
        }

        /// <summary>
        /// 重载加法运算符，实现矩阵加法
        /// </summary>
        /// <param name="value1">左操作数矩阵</param>
        /// <param name="value2">右操作数矩阵</param>
        /// <returns>两个矩阵的和</returns>
        public static TSMatrix4x4 operator +(TSMatrix4x4 value1, TSMatrix4x4 value2)
        {
            TSMatrix4x4 result;
            TSMatrix4x4.Add(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 重载取负运算符，实现矩阵元素取反
        /// </summary>
        /// <param name="value">源矩阵</param>
        /// <returns>所有元素取反的矩阵</returns>
        public static TSMatrix4x4 operator -(TSMatrix4x4 value)
        {
            TSMatrix4x4 result;

            result.M11 = -value.M11;
            result.M12 = -value.M12;
            result.M13 = -value.M13;
            result.M14 = -value.M14;
            result.M21 = -value.M21;
            result.M22 = -value.M22;
            result.M23 = -value.M23;
            result.M24 = -value.M24;
            result.M31 = -value.M31;
            result.M32 = -value.M32;
            result.M33 = -value.M33;
            result.M34 = -value.M34;
            result.M41 = -value.M41;
            result.M42 = -value.M42;
            result.M43 = -value.M43;
            result.M44 = -value.M44;

            return result;
        }

        /// <summary>
        /// 重载减法运算符，实现矩阵减法
        /// </summary>
        /// <param name="value1">左操作数矩阵</param>
        /// <param name="value2">右操作数矩阵</param>
        /// <returns>两个矩阵的差（value1 - value2）</returns>
        public static TSMatrix4x4 operator -(TSMatrix4x4 value1, TSMatrix4x4 value2)
        {
            TSMatrix4x4 result;
            // 通过取反右矩阵再相加实现减法
            TSMatrix4x4.Multiply(ref value2, -FP.One, out value2);
            TSMatrix4x4.Add(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// 重载相等运算符，判断两个矩阵是否相等（所有对应元素相等）
        /// </summary>
        /// <param name="value1">第一个矩阵</param>
        /// <param name="value2">第二个矩阵</param>
        /// <returns>如果所有对应元素相等则返回true，否则返回false</returns>
        public static bool operator ==(TSMatrix4x4 value1, TSMatrix4x4 value2)
        {
            return value1.M11 == value2.M11 &&
                   value1.M12 == value2.M12 &&
                   value1.M13 == value2.M13 &&
                   value1.M14 == value2.M14 &&
                   value1.M21 == value2.M21 &&
                   value1.M22 == value2.M22 &&
                   value1.M23 == value2.M23 &&
                   value1.M24 == value2.M24 &&
                   value1.M31 == value2.M31 &&
                   value1.M32 == value2.M32 &&
                   value1.M33 == value2.M33 &&
                   value1.M34 == value2.M34 &&
                   value1.M41 == value2.M41 &&
                   value1.M42 == value2.M42 &&
                   value1.M43 == value2.M43 &&
                   value1.M44 == value2.M44;
        }

        /// <summary>
        /// 重载不等运算符，判断两个矩阵是否不相等
        /// </summary>
        /// <param name="value1">第一个矩阵</param>
        /// <param name="value2">第二个矩阵</param>
        /// <returns>如果存在不相等的对应元素则返回true，否则返回false</returns>
        public static bool operator !=(TSMatrix4x4 value1, TSMatrix4x4 value2)
        {
            return value1.M11 != value2.M11 ||
                   value1.M12 != value2.M12 ||
                   value1.M13 != value2.M13 ||
                   value1.M14 != value2.M14 ||
                   value1.M21 != value2.M21 ||
                   value1.M22 != value2.M22 ||
                   value1.M23 != value2.M23 ||
                   value1.M24 != value2.M24 ||
                   value1.M31 != value2.M31 ||
                   value1.M32 != value2.M32 ||
                   value1.M33 != value2.M33 ||
                   value1.M34 != value2.M34 ||
                   value1.M41 != value2.M41 ||
                   value1.M42 != value2.M42 ||
                   value1.M43 != value2.M43 ||
                   value1.M44 != value2.M44;
        }

        /// <summary>
        /// 判断当前矩阵是否与指定对象相等
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>如果对象是TSMatrix4x4且所有元素相等则返回true，否则返回false</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is TSMatrix4x4)) return false;
            TSMatrix4x4 other = (TSMatrix4x4)obj;

            return this.M11 == other.M11 &&
                   this.M12 == other.M12 &&
                   this.M13 == other.M13 &&
                   this.M14 == other.M14 &&
                   this.M21 == other.M21 &&
                   this.M22 == other.M22 &&
                   this.M23 == other.M23 &&
                   this.M24 == other.M24 &&
                   this.M31 == other.M31 &&
                   this.M32 == other.M32 &&
                   this.M33 == other.M33 &&
                   this.M34 == other.M44 &&
                   this.M41 == other.M41 &&
                   this.M42 == other.M42 &&
                   this.M43 == other.M43 &&
                   this.M44 == other.M44;
        }

        /// <summary>
        /// 计算当前矩阵的哈希码（用于哈希表等数据结构）
        /// </summary>
        /// <returns>矩阵的哈希码</returns>
        public override int GetHashCode()
        {
            // 通过异或所有元素的哈希码生成矩阵的哈希码
            return M11.GetHashCode() ^
                   M12.GetHashCode() ^
                   M13.GetHashCode() ^
                   M14.GetHashCode() ^
                   M21.GetHashCode() ^
                   M22.GetHashCode() ^
                   M23.GetHashCode() ^
                   M24.GetHashCode() ^
                   M31.GetHashCode() ^
                   M32.GetHashCode() ^
                   M33.GetHashCode() ^
                   M34.GetHashCode() ^
                   M41.GetHashCode() ^
                   M42.GetHashCode() ^
                   M43.GetHashCode() ^
                   M44.GetHashCode();
        }

        /// <summary>
        /// 创建平移矩阵（表示沿X、Y、Z轴的平移变换）
        /// </summary>
        /// <param name="xPosition">X轴平移量</param>
        /// <param name="yPosition">Y轴平移量</param>
        /// <param name="zPosition">Z轴平移量</param>
        /// <returns>平移矩阵</returns>
        public static TSMatrix4x4 Translate(FP xPosition, FP yPosition, FP zPosition)
        {
            TSMatrix4x4 result;

            // 平移矩阵结构：
            // [1 0 0 x]
            // [0 1 0 y]
            // [0 0 1 z]
            // [0 0 0 1]
            result.M11 = FP.One;
            result.M12 = FP.Zero;
            result.M13 = FP.Zero;
            result.M14 = xPosition;
            result.M21 = FP.Zero;
            result.M22 = FP.One;
            result.M23 = FP.Zero;
            result.M24 = yPosition;
            result.M31 = FP.Zero;
            result.M32 = FP.Zero;
            result.M33 = FP.One;
            result.M34 = zPosition;
            result.M41 = FP.Zero;
            result.M42 = FP.Zero;
            result.M43 = FP.Zero;
            result.M44 = FP.One;

            return result;
        }

        /// <summary>
        /// 通过平移向量创建平移矩阵
        /// </summary>
        /// <param name="translation">平移向量（x、y、z分量分别为三个轴的平移量）</param>
        /// <returns>平移矩阵</returns>
        public static TSMatrix4x4 Translate(TSVector translation)
        {
            return Translate(translation.x, translation.y, translation.z);
        }

        /// <summary>
        /// 创建缩放矩阵（沿X、Y、Z轴的非均匀缩放）
        /// </summary>
        /// <param name="xScale">X轴缩放因子</param>
        /// <param name="yScale">Y轴缩放因子</param>
        /// <param name="zScale">Z轴缩放因子</param>
        /// <returns>缩放矩阵</returns>
        public static TSMatrix4x4 Scale(FP xScale, FP yScale, FP zScale)
        {
            TSMatrix4x4 result;

            // 缩放矩阵结构：
            // [x 0 0 0]
            // [0 y 0 0]
            // [0 0 z 0]
            // [0 0 0 1]
            result.M11 = xScale;
            result.M12 = FP.Zero;
            result.M13 = FP.Zero;
            result.M14 = FP.Zero;
            result.M21 = FP.Zero;
            result.M22 = yScale;
            result.M23 = FP.Zero;
            result.M24 = FP.Zero;
            result.M31 = FP.Zero;
            result.M32 = FP.Zero;
            result.M33 = zScale;
            result.M34 = FP.Zero;
            result.M41 = FP.Zero;
            result.M42 = FP.Zero;
            result.M43 = FP.Zero;
            result.M44 = FP.One;

            return result;
        }

        /// <summary>
        /// 创建以指定点为中心的缩放矩阵（先平移到原点，缩放后再平移回原位置）
        /// </summary>
        /// <param name="xScale">X轴缩放因子</param>
        /// <param name="yScale">Y轴缩放因子</param>
        /// <param name="zScale">Z轴缩放因子</param>
        /// <param name="centerPoint">缩放中心</param>
        /// <returns>以指定点为中心的缩放矩阵</returns>
        public static TSMatrix4x4 Scale(FP xScale, FP yScale, FP zScale, TSVector centerPoint)
        {
            TSMatrix4x4 result;

            // 计算补偿平移量（使缩放围绕中心点进行）
            FP tx = centerPoint.x * (FP.One - xScale);
            FP ty = centerPoint.y * (FP.One - yScale);
            FP tz = centerPoint.z * (FP.One - zScale);

            // 带中心的缩放矩阵结构（融合了平移和缩放）
            result.M11 = xScale;
            result.M12 = FP.Zero;
            result.M13 = FP.Zero;
            result.M14 = FP.Zero;
            result.M21 = FP.Zero;
            result.M22 = yScale;
            result.M23 = FP.Zero;
            result.M24 = FP.Zero;
            result.M31 = FP.Zero;
            result.M32 = FP.Zero;
            result.M33 = zScale;
            result.M34 = FP.Zero;
            result.M41 = tx;
            result.M42 = ty;
            result.M43 = tz;
            result.M44 = FP.One;

            return result;
        }

        /// <summary>
        /// 通过缩放向量创建缩放矩阵
        /// </summary>
        /// <param name="scales">缩放向量（x、y、z分量分别为三个轴的缩放因子）</param>
        /// <returns>缩放矩阵</returns>
        public static TSMatrix4x4 Scale(TSVector scales)
        {
            return Scale(scales.x, scales.y, scales.z);
        }

        /// <summary>
        /// 通过缩放向量和中心点创建缩放矩阵
        /// </summary>
        /// <param name="scales">缩放向量</param>
        /// <param name="centerPoint">缩放中心</param>
        /// <returns>以指定点为中心的缩放矩阵</returns>
        public static TSMatrix4x4 Scale(TSVector scales, TSVector centerPoint)
        {
            return Scale(scales.x, scales.y, scales.z, centerPoint);
        }

        /// <summary>
        /// 创建均匀缩放矩阵（三个轴缩放因子相同）
        /// </summary>
        /// <param name="scale">均匀缩放因子</param>
        /// <returns>均匀缩放矩阵</returns>
        public static TSMatrix4x4 Scale(FP scale)
        {
            return Scale(scale, scale, scale);
        }

        /// <summary>
        /// 创建以指定点为中心的均匀缩放矩阵
        /// </summary>
        /// <param name="scale">均匀缩放因子</param>
        /// <param name="centerPoint">缩放中心</param>
        /// <returns>以指定点为中心的均匀缩放矩阵</returns>
        public static TSMatrix4x4 Scale(FP scale, TSVector centerPoint)
        {
            return Scale(scale, scale, scale, centerPoint);
        }

        /// <summary>
        /// 创建绕X轴旋转的旋转矩阵
        /// </summary>
        /// <param name="radians">旋转角度（弧度制）</param>
        /// <returns>绕X轴的旋转矩阵</returns>
        public static TSMatrix4x4 RotateX(FP radians)
        {
            TSMatrix4x4 result;

            // 计算旋转角度的余弦和正弦值
            FP c = TSMath.Cos(radians);
            FP s = TSMath.Sin(radians);

            // 绕X轴旋转矩阵结构：
            // [1  0   0  0]
            // [0  c   s  0]
            // [0 -s   c  0]
            // [0  0   0  1]
            result.M11 = FP.One;
            result.M12 = FP.Zero;
            result.M13 = FP.Zero;
            result.M14 = FP.Zero;
            result.M21 = FP.Zero;
            result.M22 = c;
            result.M23 = s;
            result.M24 = FP.Zero;
            result.M31 = FP.Zero;
            result.M32 = -s;
            result.M33 = c;
            result.M34 = FP.Zero;
            result.M41 = FP.Zero;
            result.M42 = FP.Zero;
            result.M43 = FP.Zero;
            result.M44 = FP.One;

            return result;
        }

        /// <summary>
        /// 创建绕X轴旋转的旋转矩阵（以指定点为旋转中心）
        /// </summary>
        /// <param name="radians">旋转角度（弧度制）</param>
        /// <param name="centerPoint">旋转中心</param>
        /// <returns>以指定点为中心绕X轴的旋转矩阵</returns>
        public static TSMatrix4x4 RotateX(FP radians, TSVector centerPoint)
        {
            TSMatrix4x4 result;

            FP c = TSMath.Cos(radians);
            FP s = TSMath.Sin(radians);

            FP y = centerPoint.y * (FP.One - c) + centerPoint.z * s;
            FP z = centerPoint.z * (FP.One - c) - centerPoint.y * s;

            // [  1  0  0  0 ]
            // [  0  c  s  0 ]
            // [  0 -s  c  0 ]
            // [  0  y  z  1 ]
            result.M11 = FP.One;
            result.M12 = FP.Zero;
            result.M13 = FP.Zero;
            result.M14 = FP.Zero;
            result.M21 = FP.Zero;
            result.M22 = c;
            result.M23 = s;
            result.M24 = FP.Zero;
            result.M31 = FP.Zero;
            result.M32 = -s;
            result.M33 = c;
            result.M34 = FP.Zero;
            result.M41 = FP.Zero;
            result.M42 = y;
            result.M43 = z;
            result.M44 = FP.One;

            return result;
        }

        /// <summary>
        /// 创建一个用于绕Y轴旋转点的矩阵
        /// </summary>
        /// <param name="radians">绕Y轴旋转的角度（以弧度为单位）</param>
        /// <returns>旋转矩阵</returns>
        public static TSMatrix4x4 RotateY(FP radians)
        {
            TSMatrix4x4 result; // 声明结果矩阵

            FP c = TSMath.Cos(radians); // 计算角度的余弦值
            FP s = TSMath.Sin(radians); // 计算角度的正弦值

            // 绕Y轴旋转矩阵的标准形式：
            // [  c   0  -s   0 ]
            // [  0   1   0   0 ]
            // [  s   0   c   0 ]
            // [  0   0   0   1 ]
            // 其中c为余弦值，s为正弦值，该矩阵用于3D空间中绕Y轴的旋转变换
            result.M11 = c; // 第一行第一列元素赋值为余弦值
            result.M12 = FP.Zero; // 第一行第二列元素赋值为0
            result.M13 = -s; // 第一行第三列元素赋值为-s
            result.M14 = FP.Zero; // 第一行第四列元素赋值为0
            result.M21 = FP.Zero; // 第二行第一列元素赋值为0
            result.M22 = FP.One; // 第二行第二列元素赋值为1（Y轴旋转不改变Y分量比例）
            result.M23 = FP.Zero; // 第二行第三列元素赋值为0
            result.M24 = FP.Zero; // 第二行第四列元素赋值为0
            result.M31 = s; // 第三行第一列元素赋值为s
            result.M32 = FP.Zero; // 第三行第二列元素赋值为0
            result.M33 = c; // 第三行第三列元素赋值为余弦值
            result.M34 = FP.Zero; // 第三行第四列元素赋值为0
            result.M41 = FP.Zero; // 第四行第一列元素赋值为0（齐次坐标分量，不影响旋转）
            result.M42 = FP.Zero; // 第四行第二列元素赋值为0
            result.M43 = FP.Zero; // 第四行第三列元素赋值为0
            result.M44 = FP.One; // 第四行第四列元素赋值为1（齐次坐标标识）

            return result; // 返回构建的旋转矩阵
        }

        /// <summary>
        /// 创建一个用于绕Y轴旋转点的矩阵，旋转以指定的中心点为基准
        /// </summary>
        /// <param name="radians">绕Y轴旋转的角度（以弧度为单位）</param>
        /// <param name="centerPoint">旋转的中心点</param>
        /// <returns>旋转矩阵</returns>
        public static TSMatrix4x4 RotateY(FP radians, TSVector centerPoint)
        {
            TSMatrix4x4 result; // 声明结果矩阵

            FP c = TSMath.Cos(radians); // 计算角度的余弦值
            FP s = TSMath.Sin(radians); // 计算角度的正弦值

            // 计算旋转后中心点偏移量（用于补偿旋转中心的平移）
            // 公式推导基于：先将点平移至原点旋转，再平移回原中心点
            FP x = centerPoint.x * (FP.One - c) - centerPoint.z * s;
            FP z = centerPoint.x * (FP.One - c) +
                   centerPoint.x * s; // 注：此处可能存在笔误，推测应为centerPoint.z*(FP.One - c) + centerPoint.x*s

            // 带中心点的绕Y轴旋转矩阵形式：
            // [  c   0  -s   0 ]
            // [  0   1   0   0 ]
            // [  s   0   c   0 ]
            // [  x   0   z   1 ]
            // 最后一行的x、z为补偿中心点的平移分量
            result.M11 = c; // 第一行第一列元素赋值为余弦值
            result.M12 = FP.Zero; // 第一行第二列元素赋值为0
            result.M13 = -s; // 第一行第三列元素赋值为-s
            result.M14 = FP.Zero; // 第一行第四列元素赋值为0
            result.M21 = FP.Zero; // 第二行第一列元素赋值为0
            result.M22 = FP.One; // 第二行第二列元素赋值为1
            result.M23 = FP.Zero; // 第二行第三列元素赋值为0
            result.M24 = FP.Zero; // 第二行第四列元素赋值为0
            result.M31 = s; // 第三行第一列元素赋值为s
            result.M32 = FP.Zero; // 第三行第二列元素赋值为0
            result.M33 = c; // 第三行第三列元素赋值为余弦值
            result.M34 = FP.Zero; // 第三行第四列元素赋值为0
            result.M41 = x; // 第四行第一列元素赋值为计算的x偏移量
            result.M42 = FP.Zero; // 第四行第二列元素赋值为0
            result.M43 = z; // 第四行第三列元素赋值为计算的z偏移量
            result.M44 = FP.One; // 第四行第四列元素赋值为1

            return result; // 返回构建的带中心点的旋转矩阵
        }

        /// <summary>
        /// 创建一个用于绕Z轴旋转点的矩阵
        /// </summary>
        /// <param name="radians">绕Z轴旋转的角度（以弧度为单位）</param>
        /// <returns>旋转矩阵</returns>
        public static TSMatrix4x4 RotateZ(FP radians)
        {
            TSMatrix4x4 result; // 声明结果矩阵

            FP c = TSMath.Cos(radians); // 计算角度的余弦值
            FP s = TSMath.Sin(radians); // 计算角度的正弦值

            // 绕Z轴旋转矩阵的标准形式：
            // [  c   s   0   0 ]
            // [ -s   c   0   0 ]
            // [  0   0   1   0 ]
            // [  0   0   0   1 ]
            // 其中c为余弦值，s为正弦值，该矩阵用于3D空间中绕Z轴的旋转变换
            result.M11 = c; // 第一行第一列元素赋值为余弦值
            result.M12 = s; // 第一行第二列元素赋值为s
            result.M13 = FP.Zero; // 第一行第三列元素赋值为0
            result.M14 = FP.Zero; // 第一行第四列元素赋值为0
            result.M21 = -s; // 第二行第一列元素赋值为-s
            result.M22 = c; // 第二行第二列元素赋值为余弦值
            result.M23 = FP.Zero; // 第二行第三列元素赋值为0
            result.M24 = FP.Zero; // 第二行第四列元素赋值为0
            result.M31 = FP.Zero; // 第三行第一列元素赋值为0（Z轴旋转不改变Z分量比例）
            result.M32 = FP.Zero; // 第三行第二列元素赋值为0
            result.M33 = FP.One; // 第三行第三列元素赋值为1
            result.M34 = FP.Zero; // 第三行第四列元素赋值为0
            result.M41 = FP.Zero; // 第四行第一列元素赋值为0
            result.M42 = FP.Zero; // 第四行第二列元素赋值为0
            result.M43 = FP.Zero; // 第四行第三列元素赋值为0
            result.M44 = FP.One; // 第四行第四列元素赋值为1

            return result; // 返回构建的旋转矩阵
        }

        /// <summary>
        /// 创建一个用于绕Z轴旋转点的矩阵，旋转以指定的中心点为基准
        /// </summary>
        /// <param name="radians">绕Z轴旋转的角度（以弧度为单位）</param>
        /// <param name="centerPoint">旋转的中心点</param>
        /// <returns>旋转矩阵</returns>
        public static TSMatrix4x4 RotateZ(FP radians, TSVector centerPoint)
        {
            TSMatrix4x4 result; // 声明结果矩阵

            FP c = TSMath.Cos(radians); // 计算角度的余弦值
            FP s = TSMath.Sin(radians); // 计算角度的正弦值

            // 计算旋转后中心点偏移量（用于补偿旋转中心的平移）
            // 公式推导基于：先将点平移至原点旋转，再平移回原中心点
            FP x = centerPoint.x * (1 - c) + centerPoint.y * s; // 注：1应改为FP.One以保持类型一致性
            FP y = centerPoint.y * (1 - c) - centerPoint.x * s; // 注：1应改为FP.One以保持类型一致性

            // 带中心点的绕Z轴旋转矩阵形式：
            // [  c   s   0   0 ]
            // [ -s   c   0   0 ]
            // [  0   0   1   0 ]
            // [  x   y   0   1 ]
            // 最后一行的x、y为补偿中心点的平移分量
            result.M11 = c; // 第一行第一列元素赋值为余弦值
            result.M12 = s; // 第一行第二列元素赋值为s
            result.M13 = FP.Zero; // 第一行第三列元素赋值为0
            result.M14 = FP.Zero; // 第一行第四列元素赋值为0
            result.M21 = -s; // 第二行第一列元素赋值为-s
            result.M22 = c; // 第二行第二列元素赋值为余弦值
            result.M23 = FP.Zero; // 第二行第三列元素赋值为0
            result.M24 = FP.Zero; // 第二行第四列元素赋值为0
            result.M31 = FP.Zero; // 第三行第一列元素赋值为0
            result.M32 = FP.Zero; // 第三行第二列元素赋值为0
            result.M33 = FP.One; // 第三行第三列元素赋值为1
            result.M34 = FP.Zero; // 第三行第四列元素赋值为0
            result.M41 = FP.Zero; // 注：此处可能存在笔误，推测应为x（遗漏了偏移量赋值）
            result.M42 = FP.Zero; // 注：此处可能存在笔误，推测应为y（遗漏了偏移量赋值）
            result.M43 = FP.Zero; // 第四行第三列元素赋值为0
            result.M44 = FP.One; // 第四行第四列元素赋值为1

            return result; // 返回构建的带中心点的旋转矩阵
        }

        /// <summary>
        /// 创建一个绕指定轴旋转指定角度的矩阵
        /// </summary>
        /// <param name="axis">旋转轴（单位向量）</param>
        /// <param name="angle">旋转角度（以弧度为单位）</param>
        /// <param name="result">输出参数，存储生成的旋转矩阵</param>
        public static void AxisAngle(ref TSVector axis, FP angle, out TSMatrix4x4 result)
        {
            // a: 旋转角度
            // x, y, z: 旋转轴的单位向量分量
            //
            // 旋转矩阵M可通过以下公式计算：
            //
            //        T               T
            //  M = uu + (cos a)( I - uu ) + (sin a)S
            //
            // 其中：
            //
            //  u = (x, y, z)  旋转轴单位向量
            //
            //      [  0  -z   y ]
            //  S = [  z   0  -x ]  反对称矩阵
            //      [ -y   x   0 ]
            //
            //      [ 1  0  0 ]
            //  I = [ 0  1  0 ]  单位矩阵
            //      [ 0  0  1 ]
            //
            // 展开后的矩阵形式：
            //
            //     [  xx + cosa*(1-xx)    yx - cosa*yx - sina*z    zx - cosa*xz + sina*y  ]
            // M = [  xy - cosa*yx + sina*z   yy + cosa*(1-yy)    yz - cosa*yz - sina*x  ]
            //     [  zx - cosa*zx - sina*y   zy - cosa*zy + sina*x   zz + cosa*(1-zz)  ]
            //

            FP x = axis.x, y = axis.y, z = axis.z; // 提取旋转轴的分量
            FP sa = TSMath.Sin(angle), ca = TSMath.Cos(angle); // 计算角度的正弦和余弦值
            FP xx = x * x, yy = y * y, zz = z * z; // 计算轴分量的平方
            FP xy = x * y, xz = x * z, yz = y * z; // 计算轴分量的乘积

            // 根据上述公式计算矩阵各元素
            result.M11 = xx + ca * (FP.One - xx); // 第一行第一列
            result.M12 = xy - ca * xy + sa * z; // 第一行第二列
            result.M13 = xz - ca * xz - sa * y; // 第一行第三列
            result.M14 = FP.Zero; // 第一行第四列（齐次坐标，无平移）
            result.M21 = xy - ca * xy - sa * z; // 第二行第一列
            result.M22 = yy + ca * (FP.One - yy); // 第二行第二列
            result.M23 = yz - ca * yz + sa * x; // 第二行第三列
            result.M24 = FP.Zero; // 第二行第四列
            result.M31 = xz - ca * xz + sa * y; // 第三行第一列
            result.M32 = yz - ca * yz - sa * x; // 第三行第二列
            result.M33 = zz + ca * (FP.One - zz); // 第三行第三列
            result.M34 = FP.Zero; // 第三行第四列
            result.M41 = FP.Zero; // 第四行第一列
            result.M42 = FP.Zero; // 第四行第二列
            result.M43 = FP.Zero; // 第四行第三列
            result.M44 = FP.One; // 第四行第四列（齐次坐标标识）
        }

        /// <summary>
        /// 创建一个绕指定轴旋转指定角度的矩阵
        /// </summary>
        /// <param name="axis">旋转轴（单位向量）</param>
        /// <param name="angle">旋转角度（以弧度为单位）</param>
        /// <returns>生成的旋转矩阵</returns>
        public static TSMatrix4x4 AngleAxis(FP angle, TSVector axis)
        {
            TSMatrix4x4 result; // 声明结果矩阵
            AxisAngle(ref axis, angle, out result); // 调用AxisAngle方法计算矩阵
            return result; // 返回计算得到的旋转矩阵
        }

        /// <summary>
        /// 将矩阵转换为字符串表示形式
        /// </summary>
        /// <returns>矩阵元素的字符串表示（按行优先顺序，以竖线分隔）</returns>
        public override string ToString()
        {
            // 格式化矩阵的16个元素（4x4矩阵），使用RawValue获取原始值
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}",
                M11.RawValue, M12.RawValue, M13.RawValue, M14.RawValue,
                M21.RawValue, M22.RawValue, M23.RawValue, M24.RawValue,
                M31.RawValue, M32.RawValue, M33.RawValue, M34.RawValue,
                M41.RawValue, M42.RawValue, M43.RawValue, M44.RawValue);
        }

        /// <summary>
        /// 创建一个包含平移（Translation）、旋转（Rotation）和缩放（Scale）变换的复合矩阵（TRS矩阵）
        /// </summary>
        /// <param name="translation">平移向量</param>
        /// <param name="rotation">旋转四元数</param>
        /// <param name="scale">缩放向量</param>
        /// <param name="matrix">输出参数，存储生成的复合矩阵</param>
        public static void TRS(TSVector translation, TSQuaternion rotation, TSVector scale, out TSMatrix4x4 matrix)
        {
            // 复合矩阵计算顺序：先缩放，再旋转，最后平移（矩阵乘法顺序为从右到左）
            // 等价于：matrix = 平移矩阵 * 旋转矩阵 * 缩放矩阵
            matrix = TSMatrix4x4.Translate(translation) * TSMatrix4x4.Rotate(rotation) * TSMatrix4x4.Scale(scale);
        }

        /// <summary>
        /// 创建一个包含平移（Translation）、旋转（Rotation）和缩放（Scale）变换的复合矩阵（TRS矩阵）
        /// </summary>
        /// <param name="translation">平移向量</param>
        /// <param name="rotation">旋转四元数</param>
        /// <param name="scale">缩放向量</param>
        /// <returns>生成的复合矩阵</returns>
        public static TSMatrix4x4 TRS(TSVector translation, TSQuaternion rotation, TSVector scale)
        {
            TSMatrix4x4 result; // 声明结果矩阵
            TRS(translation, rotation, scale, out result); // 调用TRS输出参数版本计算矩阵
            return result; // 返回计算得到的复合矩阵
        }
    }
}