using System;
using System.Text;
using Newtonsoft.Json;

namespace Start
{

    public static class MessageHelper
    {
        public static byte[] Serialize<T>(T t) where T : IMessage
        {
            string json = JsonConvert.SerializeObject(t);
            return Encoding.UTF8.GetBytes(json);
        }

        public static T Deserialize<T>(byte[] data) where T : IMessage
        {
            string json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T Deserialize<T>(Type type, byte[] data) where T : IMessage
        {
            string json = Encoding.UTF8.GetString(data);
            return (T)JsonConvert.DeserializeObject(json,type);
        }
        
        
    }
}