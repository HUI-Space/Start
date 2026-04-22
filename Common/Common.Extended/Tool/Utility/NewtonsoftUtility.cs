using System.IO;
using Newtonsoft.Json;

namespace Start
{
    /// <summary>
    /// 序列化工具类
    /// 使用 Newtonsoft.Json 进行 JSON 序列化
    /// </summary>
    public static class NewtonsoftUtility
    {
        private static readonly JsonSerializerSettings? Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
        };

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

            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        /// <summary>
        /// 序列化对象到文件
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="objectToSerialize">要序列化的对象</param>
        /// <param name="indented">是否格式化输出，默认为 true</param>
        public static void SerializeObject<T>(string path, T objectToSerialize, bool indented = true)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var settings = indented
                ? new JsonSerializerSettings
                    { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore }
                : new JsonSerializerSettings
                    { Formatting = Formatting.None, NullValueHandling = NullValueHandling.Ignore };

            string json = JsonConvert.SerializeObject(objectToSerialize, settings);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// 从 JSON 字符串反序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">JSON 字符串</param>
        /// <returns>反序列化的对象</returns>
        public static T DeserializeFromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        /// <summary>
        /// 序列化对象为 JSON 字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="objectToSerialize">要序列化的对象</param>
        /// <param name="indented">是否格式化输出，默认为 false</param>
        /// <returns>JSON 字符串</returns>
        public static string SerializeToJson<T>(T objectToSerialize, bool indented = false)
        {
            var formatting = indented ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(objectToSerialize, formatting, Settings);
        }

        /// <summary>
        /// 从字节数组反序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="data">字节数组（UTF-8 编码的 JSON）</param>
        /// <returns>反序列化的对象</returns>
        public static T DeserializeObject<T>(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        /// <summary>
        /// 序列化对象为字节数组
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="objectToSerialize">要序列化的对象</param>
        /// <returns>字节数组（UTF-8 编码的 JSON）</returns>
        public static byte[] SerializeObject<T>(T objectToSerialize)
        {
            string json = JsonConvert.SerializeObject(objectToSerialize, Settings);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }
    }
}