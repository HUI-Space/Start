using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Start
{
    /// <summary>
    /// 哈希工具类
    /// 提供 SHA1 和 MD5 哈希计算功能
    /// </summary>
    public static class HashUtility
    {
        #region SHA1

        /// <summary>
        /// 获取字符串的 SHA1 哈希值
        /// </summary>
        /// <param name="str">要计算哈希的字符串</param>
        /// <returns>SHA1 哈希值（小写十六进制字符串）</returns>
        public static string StringSHA1(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            return BytesSHA1(buffer);
        }

        /// <summary>
        /// 获取文件的 SHA1 哈希值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>SHA1 哈希值（小写十六进制字符串）</returns>
        /// <exception cref="Exception">文件读取失败时抛出</exception>
        public static string FileSHA1(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return StreamSHA1(fs);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get file SHA1: {e.Message}");
            }
        }

        /// <summary>
        /// 获取数据流的 SHA1 哈希值
        /// </summary>
        /// <param name="stream">要计算哈希的数据流</param>
        /// <returns>SHA1 哈希值（小写十六进制字符串）</returns>
        public static string StreamSHA1(Stream stream)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(stream);
                return ToHexString(hashBytes);
            }
        }

        /// <summary>
        /// 获取字节数组的 SHA1 哈希值
        /// </summary>
        /// <param name="buffer">要计算哈希的字节数组</param>
        /// <returns>SHA1 哈希值（小写十六进制字符串）</returns>
        public static string BytesSHA1(byte[] buffer)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(buffer);
                return ToHexString(hashBytes);
            }
        }

        #endregion

        #region MD5

        /// <summary>
        /// 获取字符串的 MD5 哈希值
        /// </summary>
        /// <param name="str">要计算哈希的字符串</param>
        /// <returns>MD5 哈希值（小写十六进制字符串）</returns>
        public static string StringMD5(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            return BytesMD5(buffer);
        }

        /// <summary>
        /// 获取文件的 MD5 哈希值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>MD5 哈希值（小写十六进制字符串）</returns>
        /// <exception cref="Exception">文件读取失败时抛出</exception>
        public static string FileMD5(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return StreamMD5(fs);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get file MD5: {e.Message}");
            }
        }

        /// <summary>
        /// 获取数据流的 MD5 哈希值
        /// </summary>
        /// <param name="stream">要计算哈希的数据流</param>
        /// <returns>MD5 哈希值（小写十六进制字符串）</returns>
        public static string StreamMD5(Stream stream)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(stream);
                return ToHexString(hashBytes);
            }
        }

        /// <summary>
        /// 获取字节数组的 MD5 哈希值
        /// </summary>
        /// <param name="buffer">要计算哈希的字节数组</param>
        /// <returns>MD5 哈希值（小写十六进制字符串）</returns>
        public static string BytesMD5(byte[] buffer)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(buffer);
                return ToHexString(hashBytes);
            }
        }

        #endregion

        /// <summary>
        /// 将字节数组转换为小写十六进制字符串
        /// </summary>
        /// <param name="hashBytes">哈希字节数组</param>
        /// <returns>小写十六进制字符串</returns>
        private static string ToHexString(byte[] hashBytes)
        {
            StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
