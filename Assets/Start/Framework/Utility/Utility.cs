
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Start.Framework
{
    /// <summary>
    /// 路径工具类
    /// </summary>
    public static class PathUtility
    {
        /// <summary>
        /// 路径归一化
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string RegularPath(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return default;
            }
            return path.Replace('\\', '/').Replace("\\", "/");
        }
        
        /// <summary>
        /// 获取远程格式的路径（带有file:// 或 http:// 前缀）。
        /// </summary>
        /// <param name="path">原始路径。</param>
        /// <returns>远程格式路径。</returns>
        public static string GetRemotePath(string path)
        {
            string regularPath = path.RegularPath();
            if (regularPath == null)
            {
                return null;
            }

            return regularPath.Contains("://") ? regularPath : ("file:///" + regularPath).Replace("file:////", "file:///");
        }
        
        public static string RemoveExtension(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            int index = path.LastIndexOf(".", StringComparison.Ordinal);
            if (index == -1)
                return path;
            else
                return path.Remove(index);
        }
    }
    
    /// <summary>
    /// 文件工具类
    /// </summary>
    public static class FileUtility
    {
        /// <summary>
        /// 读取文件的文本数据
        /// </summary>
        public static string ReadAllText(string filePath)
        {
            return !File.Exists(filePath) ? string.Empty : File.ReadAllText(filePath, Encoding.UTF8);
        }
        
        /// <summary>
        /// 读取文件的字节数据
        /// </summary>
        public static byte[] ReadAllBytes(string filePath)
        {
            return !File.Exists(filePath) ? null : File.ReadAllBytes(filePath);
        }
        
        /// <summary>
        /// 写入文本数据（会覆盖指定路径的文件）
        /// </summary>
        public static void WriteAllText(string filePath, string content)
        {
            // 创建文件夹路径
            CreateFileDirectory(filePath);

            byte[] bytes = Encoding.UTF8.GetBytes(content);
            File.WriteAllBytes(filePath, bytes); //避免写入BOM标记
        }

        public static void WriteAllText(string name, string path, string content)
        {
            WriteAllText(Path.Combine(path, name), content);
        }
        
        /// <summary>
        /// 写入字节数据（会覆盖指定路径的文件）
        /// </summary>
        public static void WriteAllBytes(string filePath, byte[] data)
        {
            // 创建文件夹路径
            CreateFileDirectory(filePath);

            File.WriteAllBytes(filePath, data);
        }
        
        /// <summary>
        /// 创建文件的文件夹路径
        /// </summary>
        public static void CreateFileDirectory(string filePath)
        {
            // 获取文件的文件夹路径
            string directory = Path.GetDirectoryName(filePath);
            CreateDirectory(directory);
        }
        
        /// <summary>
        /// 创建文件夹路径
        /// </summary>
        public static void CreateDirectory(string directory)
        {
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);
        }
        
        /// <summary>
        /// 获取文件大小（字节数）
        /// </summary>
        public static long GetFileSize(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }
        
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="Exception"></exception>
        public static void DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete file: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="Exception"></exception>
        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true); // true 参数表示递归删除，包括所有子文件和子文件夹
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to delete directory: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="sourcePath">源路径</param>
        /// <param name="destPath">目标路径</param>
        public static void CopyDirectory(string sourcePath,string destPath)
        {
            try
            {
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                DirectoryInfo directory = new DirectoryInfo(sourcePath);
                FileInfo[] files = directory.GetFiles();
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(destPath, file.Name);
                    file.CopyTo(tempPath, false);
                }
                DirectoryInfo[] directoryInfos = directory.GetDirectories();
                foreach (DirectoryInfo directoryInfo in directoryInfos)
                {
                    string tempPath = Path.Combine(destPath, directoryInfo.Name);
                    CopyDirectory(directoryInfo.FullName, tempPath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to copy directory: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 移除空文件夹。
        /// </summary>
        /// <param name="directoryName">要处理的文件夹名称。</param>
        /// <returns>是否移除空文件夹成功。</returns>
        public static bool RemoveEmptyDirectory(string directoryName)
        {
            if (string.IsNullOrEmpty(directoryName))
            {
                throw new Exception("Directory name is invalid.");
            }

            try
            {
                if (!Directory.Exists(directoryName))
                {
                    return false;
                }

                // 不使用 SearchOption.AllDirectories，以便于在可能产生异常的环境下删除尽可能多的目录
                string[] subDirectoryNames = Directory.GetDirectories(directoryName, "*");
                int subDirectoryCount = subDirectoryNames.Length;
                foreach (string subDirectoryName in subDirectoryNames)
                {
                    if (RemoveEmptyDirectory(subDirectoryName))
                    {
                        subDirectoryCount--;
                    }
                }

                if (subDirectoryCount > 0)
                {
                    return false;
                }

                if (Directory.GetFiles(directoryName, "*").Length > 0)
                {
                    return false;
                }

                Directory.Delete(directoryName);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <param name="overwrite"></param>
        /// <exception cref="Exception"></exception>
        public static void CopyFile(string sourcePath,string destPath,bool overwrite = true)
        {
            try
            {
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, destPath, overwrite);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to copy file：" + ex.Message);
            }
        }
    }

    /// <summary>
    /// 哈希工具类
    /// </summary>
    public static class HashUtility
    {
        #region SHA1
        /// <summary>
        /// 获取字符串的Hash值
        /// </summary>
        public static string StringSHA1(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            return BytesSHA1(buffer);
        }

        /// <summary>
        /// 获取文件的Hash值
        /// </summary>
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
        /// 获取数据流的Hash值
        /// </summary>
        public static string StreamSHA1(Stream stream)
        {
            // 说明：创建的是SHA1类的实例，生成的是160位的散列码
            HashAlgorithm hash = HashAlgorithm.Create();
            byte[] hashBytes = hash.ComputeHash(stream);
            return ToString(hashBytes);
        }

        /// <summary>
        /// 获取字节数组的Hash值
        /// </summary>
        public static string BytesSHA1(byte[] buffer)
        {
            // 说明：创建的是SHA1类的实例，生成的是160位的散列码
            HashAlgorithm hash = HashAlgorithm.Create();
            byte[] hashBytes = hash.ComputeHash(buffer);
            return ToString(hashBytes);
        }
        #endregion
        
        #region MD5
        /// <summary>
        /// 获取字符串的MD5
        /// </summary>
        public static string StringMD5(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            return BytesMD5(buffer);
        }

        /// <summary>
        /// 获取文件的MD5
        /// </summary>
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
                throw new  Exception(e.Message);
            }
        }

        /// <summary>
        /// 获取数据流的MD5
        /// </summary>
        public static string StreamMD5(Stream stream)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] hashBytes = provider.ComputeHash(stream);
            return ToString(hashBytes);
        }

        /// <summary>
        /// 获取字节数组的MD5
        /// </summary>
        public static string BytesMD5(byte[] buffer)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] hashBytes = provider.ComputeHash(buffer);
            return ToString(hashBytes);
        }
        private static string ToString(byte[] hashBytes)
        {
            string result = BitConverter.ToString(hashBytes);
            result = result.Replace("-", "");
            return result.ToLower();
        }
        #endregion
    }

    /// <summary>
    /// 序列化与反序列化工具类
    /// </summary>
    public static class SerializerUtility
    {
        public static T DeserializeObject<T>(string path)
        {
            using (FileStream memoryStream = new FileStream(path,FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
        
        public static void SerializeObject<T>(string path,T objectToSerialize)
        {
            using (FileStream memoryStream = new FileStream(path,FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, objectToSerialize);
            }
        }
        
        public static T DeserializeObject<T>(byte[] data)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
        
        public static byte[] SerializeObject<T>(T objectToSerialize)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, objectToSerialize);
                return memoryStream.ToArray();
            }
        }
    }

    /// <summary>
    /// 程序集相关的实用函数。
    /// </summary>
    public static class AssemblyUtility
    {
        private static readonly System.Reflection.Assembly[] _assemblies = null;

        private static readonly Dictionary<string, Type> _cachedTypes =
            new Dictionary<string, Type>(StringComparer.Ordinal);

        static AssemblyUtility()
        {
            _assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// 获取已加载的程序集。
        /// </summary>
        /// <returns>已加载的程序集。</returns>
        public static System.Reflection.Assembly[] GetAssemblies()
        {
            return _assemblies;
        }

        /// <summary>
        /// 获取已加载的程序集中的所有类型。
        /// </summary>
        /// <returns>已加载的程序集中的所有类型。</returns>
        public static Type[] GetTypes()
        {
            List<Type> results = new List<Type>();
            foreach (System.Reflection.Assembly assembly in _assemblies)
            {
                results.AddRange(assembly.GetTypes());
            }

            return results.ToArray();
        }

        /// <summary>
        /// 获取已加载的程序集中的所有类型。
        /// </summary>
        /// <param name="results">已加载的程序集中的所有类型。</param>
        public static void GetTypes(List<Type> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (System.Reflection.Assembly assembly in _assemblies)
            {
                results.AddRange(assembly.GetTypes());
            }
        }

        /// <summary>
        /// 获取已加载的程序集中的指定类型。
        /// </summary>
        /// <param name="typeName">要获取的类型名。</param>
        /// <returns>已加载的程序集中的指定类型。</returns>
        public static Type GetType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new Exception("Type name is invalid.");
            }

            Type type = null;
            if (_cachedTypes.TryGetValue(typeName, out type))
            {
                return type;
            }

            type = Type.GetType(typeName);
            if (type != null)
            {
                _cachedTypes.Add(typeName, type);
                return type;
            }

            foreach (System.Reflection.Assembly assembly in _assemblies)
            {
                type = Type.GetType($"{typeName}, {assembly.FullName}");
                if (type != null)
                {
                    _cachedTypes.Add(typeName, type);
                    return type;
                }
            }

            return null;
        }

        public static List<Type> GetChildType(Type type)
        {
            List<Type> implementingTypes = new List<Type>();
            Type[] types = GetTypes();
            foreach (Type t in types)
            {
                if (type.IsAssignableFrom(t) && type != t)
                {
                    implementingTypes.Add(t);
                }
            }

            return implementingTypes;
        }
    }

    /// <summary>
    /// 本地时间
    /// <para>具备程序级独立运行的时间,不受系统时间修改而影响</para>
    /// </summary>
    public static class TimeUtility
    {
        /// <summary>
        /// 秒表器
        /// </summary>
        private static readonly Stopwatch _sw = Stopwatch.StartNew();

        /// <summary>
        /// 时间节点
        /// </summary>
        private static long _time = DateTime.Now.Ticks;

        /// <summary>
        /// 计时器运行时长
        /// </summary>
        public static long ElapsedMilliseconds => _sw.ElapsedMilliseconds;

        /// <summary>
        /// 当前UTC时间
        /// </summary>
        public static DateTime UtcNow => Now.ToUniversalTime();

        /// <summary>
        /// 当前时间
        /// </summary>
        public static DateTime Now => new DateTime(_time + _sw.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond);

        /// <summary>
        /// 当前时间戳 秒级
        /// </summary>
        public static long TimeStamp => (_time + _sw.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond - 621355968000000000) / TimeSpan.TicksPerSecond;

        /// <summary>
        /// 本地时间同步当前本地时间一次
        /// </summary>
        public static void Sync()
        {
            Sync(DateTime.Now);
        }

        /// <summary>
        /// 本地时间同步一次秒级时间戳
        /// </summary>
        /// <param name="timeStamp">时间戳，单位：秒</param>
        public static void Sync(long timeStamp)
        {
            _time = timeStamp * TimeSpan.TicksPerSecond + 621355968000000000 - _sw.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 本地时间同步一次时间对象
        /// </summary>
        /// <param name="dt"></param>
        public static void Sync(DateTime dt)
        {
            _time = dt.Ticks - _sw.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 本地时间永久增加毫秒
        /// </summary>
        /// <param name="milliseconds">毫秒</param>
        public static void AddMilliseconds(int milliseconds)
        {
            _time += milliseconds * TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 本地时间永久增加秒
        /// </summary>
        /// <param name="seconds">秒</param>
        public static void AddSeconds(int seconds)
        {
            _time += seconds * TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// 本地时间永久增加分钟
        /// </summary>
        /// <param name="minutes">分钟</param>
        public static void AddMinutes(int minutes)
        {
            _time += minutes * TimeSpan.FromMinutes(1).Ticks;
        }

        /// <summary>
        /// 本地时间永久增加小时
        /// </summary>
        /// <param name="hours">小时</param>
        public static void AddHours(int hours)
        {
            _time += hours * TimeSpan.FromHours(1).Ticks;
        }

        /// <summary>
        /// 本地时间永久增加天数
        /// </summary>
        /// <param name="days">天数</param>
        public static void AddDays(int days)
        {
            _time += days * TimeSpan.FromDays(1).Ticks;
        }

        /// <summary>
        /// 本地时间永久增加月数
        /// </summary>
        /// <param name="months">月数</param>
        public static void AddMonths(int months)
        {
            _time = Now.AddMonths(months).Ticks - _sw.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 本地时间永久增加年数
        /// </summary>
        /// <param name="years">年数</param>
        public static void AddYears(int years)
        {
            _time = Now.AddYears(years).Ticks - _sw.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 日期时间转换为秒级时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ConvertToTimeStamp(DateTime dt)
        {
            return (dt.Ticks - 621355968000000000) / TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// 秒级时间戳转换为日期时间
        /// </summary>
        /// <param name="ticks">秒时间戳</param>
        /// <return></return>
        public static DateTime ConvertFromTimeStamp(long ticks)
        {
            return new DateTime(ticks * TimeSpan.TicksPerSecond + 621355968000000000);
        }
    }
}