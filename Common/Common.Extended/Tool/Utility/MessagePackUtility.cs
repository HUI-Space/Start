using System.IO;
using MessagePack;

namespace Start
{
    /// <summary>
    /// 序列化工具类
    /// 使用 MessagePack 进行高性能二进制序列化
    /// </summary>
    public static class MessagePackUtility
    {
        private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard;

        /// <summary>
        /// 从文件反序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <returns>反序列化的对象</returns>
        public static T DeserializeObject<T>(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"文件不存在: {path}");
            }

            byte[] data = File.ReadAllBytes(path);
            return MessagePackSerializer.Deserialize<T>(data, Options);
        }

        /// <summary>
        /// 序列化对象到文件
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="objectToSerialize">要序列化的对象</param>
        public static void SerializeObject<T>(string path, T objectToSerialize)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            byte[] data = MessagePackSerializer.Serialize(objectToSerialize, Options);
            File.WriteAllBytes(path, data);
        }

        /// <summary>
        /// 从字节数组反序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="data">字节数组</param>
        /// <returns>反序列化的对象</returns>
        public static T DeserializeObject<T>(byte[] data)
        {
            return MessagePackSerializer.Deserialize<T>(data, Options);
        }

        /// <summary>
        /// 序列化对象为字节数组
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="objectToSerialize">要序列化的对象</param>
        /// <returns>字节数组</returns>
        public static byte[] SerializeObject<T>(T objectToSerialize)
        {
            return MessagePackSerializer.Serialize(objectToSerialize, Options);
        }
    }
}