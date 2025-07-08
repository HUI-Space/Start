using System.IO;
using Common;

namespace Start
{
    public static class AssetConfig
    {
        public const string Assets = nameof(Assets);
        public const string Root = "Asset";
        public const string Prefab = ".prefab";
        public const string Json = ".json";
        public const string Unity = ".unity";

        public static string GetAssetRoot(EAssetType assetType)
        {
            return Path.Combine(Root, assetType.ToString()).RegularPath();
        }
        
        public static string GetAssetPath(EAssetType assetType,string assetName)
        {
             return Path.Combine(Assets,Root, assetType.ToString(), assetName).RegularPath();
        }

        public static string GetAssetPathWithoutAssets(EAssetType assetType,string assetName)
        {
            return Path.Combine(Root, assetType.ToString(), assetName).RegularPath();
        }
    }
}