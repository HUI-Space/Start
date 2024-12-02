namespace Start.Runtime
{
    public interface ILoadResourceHelper
    {
        object LoadResource(string fullPath, uint crc, ulong offset);
        object LoadAsset(object resource,string assetName);
        T LoadScene<T>(string sceneName,bool isAdditive);
    }
}