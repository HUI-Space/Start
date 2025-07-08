using System.IO;
using Common;
using UnityEditor;

namespace Start.Editor
{
    public static class ResourceBuilderHelper
    {
        private const string AssetBundleExtension = ".s";
        private const string ShareAssetBundleExtension = ".share";


        public static string GetAddressName(MainResource mainResource)
        {
            switch (mainResource.AddressType)
            {
                case  AddressType.Disable:
                    return string.Empty;
                case AddressType.FileName:
                    return Path.GetFileName(mainResource.Resource.Path);
                case AddressType.GroupAndFileName:
                    return Path.Combine(mainResource.ResourceGroupName,Path.GetFileName(mainResource.Resource.Path)).RegularPath();
                case AddressType.FolderAndFileName:
                    return mainResource.Resource.Path;
                default:
                    return mainResource.Resource.Path;
            }
        }
        
        public static string GetAssetBundleName(string path,NameType nameType)
        {
            string newPath = path.RegularPath().Replace('/', '_').ToLower();
            switch (nameType)
            {
                case NameType.HashName:
                    return HashUtility.StringSHA1(newPath) + AssetBundleExtension;
                case NameType.AssetBundleName:
                    return Path.ChangeExtension(newPath, AssetBundleExtension);
                case NameType.HashNameAndAssetBundleName:
                    return Path.ChangeExtension(HashUtility.StringSHA1(newPath) + "_" + newPath,AssetBundleExtension) ;
                default:
                    return Path.ChangeExtension(newPath, AssetBundleExtension);
            }
        }
        
        public static string GetShareAssetBundleName(string path,NameType nameType)
        {
            string newPath = path.RegularPath().Replace('/', '_').ToLower();
            switch (nameType)
            {
                case NameType.HashName:
                    return HashUtility.StringSHA1(newPath) + ShareAssetBundleExtension;
                case NameType.AssetBundleName:
                    return Path.ChangeExtension(newPath, ShareAssetBundleExtension);
                case NameType.HashNameAndAssetBundleName:
                    return Path.ChangeExtension(HashUtility.StringSHA1(newPath) + "_" + newPath,ShareAssetBundleExtension) ;
                default:
                    return Path.ChangeExtension(newPath, ShareAssetBundleExtension);
            }
        }

        public static string GetAssetBundleName(MainResource mainResource,NameType nameType)
        {
            return GetAssetBundleName(GetAssetName(mainResource),nameType);
        }
        private static string GetAssetName(MainResource mainResource)
        {
            switch (mainResource.PackType)
            {
                case PackType.PackSeparately:
                    return mainResource.Resource.Path;
                case PackType.PackGroup:
                    return mainResource.ResourceGroupName;
                case PackType.PackGroupItem:
                    return mainResource.ResourceGroupItemPath;
            }

            return string.Empty;
        }
        
        public static uint GetCRCForAssetBundle(string path)
        {
            return BuildPipeline.GetCRCForAssetBundle(path, out uint crc) ? crc : default;
        }
        
    }
}