using System;

namespace Common
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
}