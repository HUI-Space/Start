using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Start
{
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
}