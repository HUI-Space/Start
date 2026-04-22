using System;

namespace Start
{
    /// <summary>
    /// 路径工具类
    /// </summary>
    public static class PathUtility
    {
        /// <summary>
        /// 路径归一化，将反斜杠转换为正斜杠
        /// </summary>
        /// <param name="path">原始路径</param>
        /// <returns>归一化后的路径，空路径返回 null</returns>
        public static string RegularPath(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// 获取远程格式的路径（带有 file:// 或 http:// 前缀）
        /// </summary>
        /// <param name="path">原始路径</param>
        /// <returns>远程格式路径</returns>
        public static string GetRemotePath(string path)
        {
            string regularPath = path.RegularPath();
            if (regularPath == null)
            {
                return null;
            }

            return regularPath.Contains("://") ? regularPath : ("file:///" + regularPath).Replace("file:////", "file:///");
        }

        /// <summary>
        /// 移除路径中的文件扩展名
        /// </summary>
        /// <param name="path">原始路径</param>
        /// <returns>移除扩展名后的路径</returns>
        public static string RemoveExtension(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            int index = path.LastIndexOf(".", StringComparison.Ordinal);
            if (index == -1)
            {
                return path;
            }

            return path.Remove(index);
        }
    }
}
