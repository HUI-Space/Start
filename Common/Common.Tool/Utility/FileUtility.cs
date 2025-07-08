using System;
using System.IO;
using System.Text;

namespace Start
{
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
            File.WriteAllText(filePath, content); //避免写入BOM标记
        }
        
        /// <summary>
        /// 写入文本数据（会覆盖指定路径的文件）
        /// </summary>
        public static void WriteAllText(string name, string path, string content)
        {
            WriteAllBytes(Path.Combine(path, name), content);
        }
        
        /// <summary>
        /// 写入文本数据（会覆盖指定路径的文件）
        /// </summary>
        public static void WriteAllBytes(string filePath, string content)
        {
            // 创建文件夹路径
            CreateFileDirectory(filePath);

            byte[] bytes = Encoding.UTF8.GetBytes(content);
            File.WriteAllBytes(filePath, bytes); //避免写入BOM标记
        }

        public static void WriteAllBytes(string name, string path, string content)
        {
            WriteAllBytes(Path.Combine(path, name), content);
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
        
        /// <summary>
        /// 获取文件后缀不好含 "."
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetExtensionWithoutDot(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new Exception("FilePath name is invalid.");
            }
            // 获取文件的扩展名，包含 "."
            string extensionWithDot = Path.GetExtension(filePath);

            // 去掉前导的 "."
            if (extensionWithDot.StartsWith("."))
            {
                return extensionWithDot.Substring(1);
            }

            return extensionWithDot;
        }
    }
}