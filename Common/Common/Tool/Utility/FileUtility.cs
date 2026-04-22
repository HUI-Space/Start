using System;
using System.IO;
using System.Text;

namespace Start
{
    /// <summary>
    /// 文件操作工具类
    /// </summary>
    public static class FileUtility
    {
        /// <summary>
        /// 读取文件的文本数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件内容，文件不存在时返回空字符串</returns>
        public static string ReadAllText(string filePath)
        {
            return !File.Exists(filePath) ? string.Empty : File.ReadAllText(filePath, Encoding.UTF8);
        }

        /// <summary>
        /// 读取文件的字节数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件字节数据，文件不存在时返回 null</returns>
        public static byte[] ReadAllBytes(string filePath)
        {
            return !File.Exists(filePath) ? null : File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// 写入文本数据到文件（会覆盖已存在的文件）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">要写入的文本内容</param>
        public static void WriteAllText(string filePath, string content)
        {
            CreateFileDirectory(filePath);
            File.WriteAllText(filePath, content);
        }

        /// <summary>
        /// 写入文本数据到指定路径的文件（会覆盖已存在的文件）
        /// </summary>
        /// <param name="name">文件名</param>
        /// <param name="path">文件所在目录路径</param>
        /// <param name="content">要写入的文本内容</param>
        public static void WriteAllText(string name, string path, string content)
        {
            WriteAllBytes(Path.Combine(path, name), content);
        }

        /// <summary>
        /// 将文本内容以 UTF-8 编码写入文件（不包含 BOM 标记）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">要写入的文本内容</param>
        public static void WriteAllBytes(string filePath, string content)
        {
            CreateFileDirectory(filePath);
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            File.WriteAllBytes(filePath, bytes);
        }

        /// <summary>
        /// 将文本内容以 UTF-8 编码写入指定路径的文件（不包含 BOM 标记）
        /// </summary>
        /// <param name="name">文件名</param>
        /// <param name="path">文件所在目录路径</param>
        /// <param name="content">要写入的文本内容</param>
        public static void WriteAllBytes(string name, string path, string content)
        {
            WriteAllBytes(Path.Combine(path, name), content);
        }

        /// <summary>
        /// 写入字节数据到文件（会覆盖已存在的文件）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="data">要写入的字节数据</param>
        public static void WriteAllBytes(string filePath, byte[] data)
        {
            CreateFileDirectory(filePath);
            File.WriteAllBytes(filePath, data);
        }

        /// <summary>
        /// 创建文件所在的目录
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void CreateFileDirectory(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            CreateDirectory(directory);
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="directory">目录路径</param>
        public static void CreateDirectory(string directory)
        {
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);
        }

        /// <summary>
        /// 获取文件大小（字节数）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件大小（字节）</returns>
        /// <exception cref="FileNotFoundException">文件不存在时抛出</exception>
        public static long GetFileSize(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <exception cref="Exception">删除失败时抛出</exception>
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
        /// 删除文件夹（包括所有子文件和子文件夹）
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <exception cref="Exception">删除失败时抛出</exception>
        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to delete directory: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 拷贝文件夹（包括所有子文件和子文件夹）
        /// </summary>
        /// <param name="sourcePath">源文件夹路径</param>
        /// <param name="destPath">目标文件夹路径</param>
        /// <exception cref="Exception">拷贝失败时抛出</exception>
        public static void CopyDirectory(string sourcePath, string destPath)
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
        /// 移除空文件夹（递归删除所有空子文件夹）
        /// </summary>
        /// <param name="directoryName">要处理的文件夹路径</param>
        /// <returns>是否成功移除空文件夹</returns>
        /// <exception cref="Exception">文件夹路径无效时抛出</exception>
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
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="destPath">目标文件路径</param>
        /// <param name="overwrite">是否覆盖已存在的文件，默认为 true</param>
        /// <exception cref="Exception">拷贝失败时抛出</exception>
        public static void CopyFile(string sourcePath, string destPath, bool overwrite = true)
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

        /// <summary>
        /// 获取文件扩展名（不包含 "."）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件扩展名（不包含 "."）</returns>
        /// <exception cref="Exception">文件路径无效时抛出</exception>
        public static string GetExtensionWithoutDot(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new Exception("FilePath name is invalid.");
            }
            string extensionWithDot = Path.GetExtension(filePath);

            if (extensionWithDot.StartsWith("."))
            {
                return extensionWithDot.Substring(1);
            }

            return extensionWithDot;
        }
    }
}
