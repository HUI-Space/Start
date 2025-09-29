﻿using System;

namespace TrueSync {
    /**
     *  @brief 基于确定性确定性算法生成随机数的类，采用梅森旋转算法(Mersenne Twister)实现
     *  梅森旋转算法是一种高质量的伪随机数生成器，具有长周期（2^19937-1）和良好的统计特性
     **/
    public class TSRandom {
        // 算法核心参数，定义梅森旋转算法的状态数组大小和移位参数
        private const int N = 624;          // 状态数组大小
        private const int M = 397;          // 移位参数，用于生成新状态
        private const uint MATRIX_A = 0x9908b0dfU;  // 旋转矩阵系数
        private const uint UPPER_MASK = 0x80000000U; // 高位掩码（32位中最高位为1）
        private const uint LOWER_MASK = 0x7fffffffU; // 低位掩码（32位中最高位为0，其余为1）
        private const int MAX_RAND_INT = 0x7fffffff; // 最大随机整数（2^31-1）

        // 掩码数组，用于根据最低位决定是否应用旋转矩阵
        private uint[] mag01 = { 0x0U, MATRIX_A };
        private uint[] mt = new uint[N];    // 状态数组，存储随机数生成的中间状态
        private int mti = N + 1;            // 状态索引，指示下一个要使用的状态元素

        /**
         *  @brief 静态实例，使用种子1初始化的默认随机数生成器
         **/
        public static TSRandom instance;

        /// <summary>
        /// 初始化静态实例，使用种子1创建默认随机数生成器
        /// </summary>
        internal static void Init() {
            instance = New(1);
        }

        /**
         *  @brief 根据给定种子生成新的TSRandom实例
         **/
        public static TSRandom New(int seed) {
            TSRandom r = new TSRandom(seed);

            /*StateTracker.AddTracking(r, "mt");
            StateTracker.AddTracking(r, "mti");*/

            return r;
        }

        /// <summary>
        /// 无参构造函数，使用当前时间的毫秒数作为种子初始化
        /// </summary>
        private TSRandom() {
            init_genrand((uint)DateTime.Now.Millisecond);
        }

        /// <summary>
        /// 带种子的构造函数，使用指定整数作为种子初始化
        /// </summary>
        /// <param name="seed">初始化种子</param>
        private TSRandom(int seed) {
            init_genrand((uint)seed);
        }

        /// <summary>
        /// 带整数数组的构造函数，使用整数数组作为种子初始化（提供更高自由度的种子设置）
        /// </summary>
        /// <param name="init">整数数组种子</param>
        private TSRandom(int[] init) {
            uint[] initArray = new uint[init.Length];
            // 将int数组转换为uint数组
            for (int i = 0; i < init.Length; ++i)
                initArray[i] = (uint)init[i];
            init_by_array(initArray, (uint)initArray.Length);
        }

        /// <summary>
        /// 最大随机整数的属性访问器
        /// </summary>
        public static int MaxRandomInt { get { return 0x7fffffff; } }

        /**
         *  @brief 返回一个随机整数（范围：0 到 MAX_RAND_INT）
         **/
        public int Next() {
            return genrand_int31();
        }

        /**
         *  @brief 调用静态实例的Next()方法，返回随机整数
         **/
        public static int CallNext() {
            return instance.Next();
        }

        /**
         *  @brief 返回指定范围内的随机整数（[minValue, maxValue)）
         **/
        public int Next(int minValue, int maxValue) {
            // 确保min不大于max，若否则交换两者
            if (minValue > maxValue) {
                int tmp = maxValue;
                maxValue = minValue;
                minValue = tmp;
            }

            int range = maxValue - minValue;
            // 通过取模运算将随机数映射到指定范围
            return minValue + Next() % range;
        }

        /**
         *  @brief 返回指定范围内的FP类型随机数（[minValue, maxValue]）
         *  注：FP是TrueSync库中的定点数类型，用于高精度计算
         **/
        public FP Next(float minValue, float maxValue) {
            // 将浮点数放大1000倍转为整数，实现定点数精度控制
            int minValueInt = (int)(minValue * 1000), maxValueInt = (int)(maxValue * 1000);

            // 确保min不大于max，若否则交换两者
            if (minValueInt > maxValueInt) {
                int tmp = maxValueInt;
                maxValueInt = minValueInt;
                minValueInt = tmp;
            }

            // 生成范围内的随机数后缩小1000倍还原为原始精度
            return (FP.Floor((maxValueInt - minValueInt + 1) * NextFP() +
                minValueInt)) / 1000;
        }

        /**
         *  @brief 调用静态实例的Next(min, max)，返回指定范围整数（[min, max)）
         **/
        public static int Range(int minValue, int maxValue) {
            return instance.Next(minValue, maxValue);
        }

        /**
         *  @brief 调用静态实例的Next(min, max)，返回指定范围FP数（[min, max]）
         **/
        public static FP Range(float minValue, float maxValue) {
            return instance.Next(minValue, maxValue);
        }

        /**
         *  @brief 返回[0.0, 1.0]范围内的FP类型随机数
         **/
        public FP NextFP() {
            // 通过除以最大值将整数随机数归一化到[0,1]范围
            return ((FP)Next()) / (MaxRandomInt);
        }

        /**
         *  @brief 静态属性，返回[0.0, 1.0]范围内的FP类型随机数
         **/
        public static FP value {
            get {
                return instance.NextFP();
            }
        }

        /**
         *  @brief 返回单位球内的随机点（各分量在[0,1]范围的TSVector）
         *  注：TSVector是TrueSync库中的三维向量类型
         **/
        public static TSVector insideUnitSphere {
            get {
                // 为x、y、z分量分别生成[0,1]范围内的随机数
                return new TSVector(value, value, value);
            }
        }

        /// <summary>
        /// 生成[0.0, 1.0)范围内的float类型随机数
        /// </summary>
        private float NextFloat() {
            return (float)genrand_real2();
        }

        /// <summary>
        /// 生成float类型随机数，可指定是否包含1.0
        /// </summary>
        /// <param name="includeOne">是否包含1.0（true则范围为[0.0,1.0]，false则为[0.0,1.0)）</param>
        private float NextFloat(bool includeOne) {
            if (includeOne) {
                return (float)genrand_real1();
            }
            return (float)genrand_real2();
        }

        /// <summary>
        /// 生成(0.0, 1.0]范围内的float类型随机数（不包含0.0）
        /// </summary>
        private float NextFloatPositive() {
            return (float)genrand_real3();
        }

        /// <summary>
        /// 生成[0.0, 1.0)范围内的double类型随机数
        /// </summary>
        private double NextDouble() {
            return genrand_real2();
        }

        /// <summary>
        /// 生成double类型随机数，可指定是否包含1.0
        /// </summary>
        /// <param name="includeOne">是否包含1.0（true则范围为[0.0,1.0]，false则为[0.0,1.0)）</param>
        private double NextDouble(bool includeOne) {
            if (includeOne) {
                return genrand_real1();
            }
            return genrand_real2();
        }

        /// <summary>
        /// 生成(0.0, 1.0]范围内的double类型随机数（不包含0.0）
        /// </summary>
        private double NextDoublePositive() {
            return genrand_real3();
        }

        /// <summary>
        /// 生成53位精度的double类型随机数（更高精度的随机数）
        /// </summary>
        private double Next53BitRes() {
            return genrand_res53();
        }

        /// <summary>
        /// 使用当前时间的毫秒数重新初始化随机数生成器
        /// </summary>
        public void Initialize() {
            init_genrand((uint)DateTime.Now.Millisecond);
        }

        /// <summary>
        /// 使用指定种子重新初始化随机数生成器
        /// </summary>
        /// <param name="seed">初始化种子</param>
        public void Initialize(int seed) {
            init_genrand((uint)seed);
        }

        /// <summary>
        /// 使用整数数组重新初始化随机数生成器
        /// </summary>
        /// <param name="init">整数数组种子</param>
        public void Initialize(int[] init) {
            uint[] initArray = new uint[init.Length];
            for (int i = 0; i < init.Length; ++i)
                initArray[i] = (uint)init[i];
            init_by_array(initArray, (uint)initArray.Length);
        }

        /// <summary>
        /// 用单个32位无符号整数种子初始化状态数组
        /// </summary>
        /// <param name="s">32位无符号整数种子</param>
        private void init_genrand(uint s) {
            mt[0] = s & 0xffffffffU;  // 确保初始值为32位
            // 按递推公式初始化状态数组剩余元素
            for (mti = 1; mti < N; mti++) {
                // 递推公式：mt[i] = (1812433253 * (mt[i-1] ^ (mt[i-1] >> 30)) + i) mod 2^32
                mt[mti] = (uint)(1812433253U * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti);
                mt[mti] &= 0xffffffffU;  // 截断为32位
            }
        }

        /// <summary>
        /// 用数组种子初始化状态数组（支持更复杂的种子设置）
        /// </summary>
        /// <param name="init_key">无符号整数数组种子</param>
        /// <param name="key_length">数组长度</param>
        private void init_by_array(uint[] init_key, uint key_length) {
            int i, j, k;
            // 先用固定种子初始化
            init_genrand(19650218U);
            i = 1;
            j = 0;
            // 取状态数组大小和种子数组长度中的较大值作为循环次数
            k = (int)(N > key_length ? N : key_length);
            for (; k > 0; k--) {
                // 结合之前的状态和种子数组元素更新状态
                mt[i] = (uint)((uint)(mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525U)) + init_key[j] + j);
                mt[i] &= 0xffffffffU;  // 截断为32位
                i++;
                j++;
                // 若状态数组填满则循环回到开头
                if (i >= N) {
                    mt[0] = mt[N - 1];
                    i = 1;
                }
                // 若种子数组用完则循环使用
                if (j >= key_length)
                    j = 0;
            }
            // 进一步混淆状态，增强随机性
            for (k = N - 1; k > 0; k--) {
                mt[i] = (uint)((uint)(mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1566083941U)) - i);
                mt[i] &= 0xffffffffU;  // 截断为32位
                i++;
                if (i >= N) {
                    mt[0] = mt[N - 1];
                    i = 1;
                }
            }
            // 设置最高位为1，完成初始化
            mt[0] = 0x80000000U;
        }

        /// <summary>
        /// 生成32位无符号随机整数（核心生成函数）
        /// </summary>
        /// <returns>32位无符号随机整数</returns>
        uint genrand_int32() {
            uint y;
            // 当状态数组用尽时，更新状态数组
            if (mti >= N) {
                int kk;
                // 若未初始化则使用默认种子
                if (mti == N + 1)
                    init_genrand(5489U);
                // 生成新的状态数组（梅森旋转核心步骤）
                for (kk = 0; kk < N - M; kk++) {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1U];
                }
                for (; kk < N - 1; kk++) {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1U];
                }
                y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
                mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1U];
                mti = 0;  // 重置状态索引
            }

            y = mt[mti++];  // 取当前状态并移动索引

            // 对生成的随机数进行 tempering（进一步混淆，提高随机性）
            y ^= (y >> 11);                     // 右移11位异或
            y ^= (y << 7) & 0x9d2c5680U;        // 左移7位并与特定掩码相与后异或
            y ^= (y << 15) & 0xefc60000U;       // 左移15位并与特定掩码相与后异或
            y ^= (y >> 18);                     // 右移18位异或

            return y;
        }

        /// <summary>
        /// 生成31位有符号随机整数（范围：0 到 2^31-1）
        /// </summary>
        /// <returns>31位有符号随机整数</returns>
        private int genrand_int31() {
            // 将32位无符号数右移1位，得到31位有符号数
            return (int)(genrand_int32() >> 1);
        }

        /// <summary>
        /// 生成FP类型的随机数（范围：[0, 1]）
        /// </summary>
        /// <returns>FP类型随机数</returns>
        FP genrand_FP() {
            // 归一化到[0,1]范围（4294967295 = 2^32 - 1）
            return (FP)genrand_int32() * (FP.One / (FP)4294967295);
        }

        /// <summary>
        /// 生成[0.0, 1.0]范围的double类型随机数
        /// </summary>
        /// <returns>double类型随机数</returns>
        double genrand_real1() {
            return genrand_int32() * (1.0 / 4294967295.0);
        }

        /// <summary>
        /// 生成[0.0, 1.0)范围的double类型随机数
        /// </summary>
        /// <returns>double类型随机数</returns>
        double genrand_real2() {
            return genrand_int32() * (1.0 / 4294967296.0);
        }

        /// <summary>
        /// 生成(0.0, 1.0]范围的double类型随机数
        /// </summary>
        /// <returns>double类型随机数</returns>
        double genrand_real3() {
            return (((double)genrand_int32()) + 0.5) * (1.0 / 4294967296.0);
        }

        /// <summary>
        /// 生成53位精度的double类型随机数（范围：[0.0, 1.0)）
        /// 注：double类型的有效精度为53位，此方法充分利用其精度
        /// </summary>
        /// <returns>53位精度的double随机数</returns>
        double genrand_res53() {
            // 取高27位和高26位拼接成53位精度
            uint a = genrand_int32() >> 5, b = genrand_int32() >> 6;
            return (a * 67108864.0 + b) * (1.0 / 9007199254740992.0);
        }
    }
}