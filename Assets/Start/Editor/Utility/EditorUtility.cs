using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public static partial class EditorUtility
    {
        
        public static T LoadJsonConfig<T>(string path)
        {
            if (!File.Exists(path))
            {
                return default;
            }

            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            T config = JsonUtility.FromJson<T>(textAsset.text);
            return config;
        }
        
        /// <summary>
        /// 获取指定资源的依赖资源
        /// </summary>
        /// <param name="mainAssetPath">资源路径</param>
        /// <param name="recursive">控制此方法是否递归地检查并返回所有依赖项，包括间接依赖项(当设置为true时)，还是只返回直接依赖项(当设置为false时)。</param>
        /// <returns></returns>
        public static List<string> GetAllDependencies(string mainAssetPath,bool recursive = true)
        {
            string[] depends = AssetDatabase.GetDependencies(mainAssetPath, recursive);
            List<string> dependencies = new List<string>();
            
            foreach (string assetPath in depends)
            {
                // 移除主资源
                if (assetPath == mainAssetPath)
                    continue;
                // 忽略文件夹
                if (AssetDatabase.IsValidFolder(assetPath))
                    continue;
                
                // 忽略编辑器下的类型资源
                Type type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                if ( type == typeof(LightingDataAsset) || type == typeof(DefaultAsset))
                    continue;
                
                // 忽略特定后缀名的资源
                if (IsIgnoreFile(Path.GetExtension(assetPath)))
                    continue;
                
                dependencies.Add(assetPath);
            }

            return dependencies;
        }

        /// <summary>
        /// 获取指定资源的依赖资源
        /// </summary>
        /// <param name="mainAssetPath">资源路径</param>
        /// <param name="recursive">控制此方法是否递归地检查并返回所有依赖项，包括间接依赖项(当设置为true时)，还是只返回直接依赖项(当设置为false时)。</param>
        /// <returns></returns>
        public static HashSet<string> GetDependencies(string mainAssetPath,bool recursive = true)
        {
            string[] depends = AssetDatabase.GetDependencies(mainAssetPath, recursive);
            HashSet<string> dependencies = new HashSet<string>();
            
            foreach (string assetPath in depends)
            {
                // 移除主资源
                if (assetPath == mainAssetPath)
                    continue;
                // 忽略文件夹
                if (AssetDatabase.IsValidFolder(assetPath))
                    continue;
                
                // 忽略编辑器下的类型资源
                Type type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                if ( type == typeof(LightingDataAsset) || type == typeof(DefaultAsset))
                    continue;
                
                // 忽略特定后缀名的资源
                if (IsIgnoreFile(Path.GetExtension(assetPath)))
                    continue;
                
                dependencies.Add(assetPath);
            }

            return dependencies;
        }
        
        public static string[] FindAssets(SearchType searchType, string searchInFolder)
        {
            return FindAssets(searchType, new string[] { searchInFolder });
        }
        
        public static string[] FindAssets(SearchType searchType, string[] searchInFolders)
        {
            // 注意：AssetDatabase.FindAssets()不支持末尾带分隔符的文件夹路径
            for (int i = 0; i < searchInFolders.Length; i++)
            {
                string folderPath = searchInFolders[i];
                searchInFolders[i] = folderPath.TrimEnd('/');
            }

            // 注意：获取指定目录下的所有资源对象（包括子文件夹）
            string[] guids;
            if (searchType == SearchType.All)
                guids = AssetDatabase.FindAssets(string.Empty, searchInFolders);
            else
                guids = AssetDatabase.FindAssets($"t:{searchType}", searchInFolders);

            // 注意：AssetDatabase.FindAssets()可能会获取到重复的资源
            HashSet<string> result = new HashSet<string>();
            for (int i = 0; i < guids.Length; i++)
            {
                string guid = guids[i];
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                // 忽略文件夹
                if (AssetDatabase.IsValidFolder(assetPath))
                    continue;
                if (result.Contains(assetPath) == false)
                {
                    result.Add(assetPath);
                }
            }

            // 返回结果
            return result.ToArray();
        }
        
        /// <summary>
        /// 查询是否为忽略文件
        /// </summary>
        public static bool IsIgnoreFile(string fileExtension)
        {
            return _ignoreFileExtensions.Contains(fileExtension);
        }
        private static readonly HashSet<string> _ignoreFileExtensions = new HashSet<string>() { "", ".so", ".dll", ".cs", ".js", ".boo", ".meta", ".cginc", ".hlsl" };

        
        
    }
    

    public enum SearchType
    {
        All,
        RuntimeAnimatorController,
        AnimationClip,
        AudioClip,
        AudioMixer,
        Font,
        Material,
        Mesh,
        Model,
        PhysicMaterial,
        Prefab,
        Scene,
        Script,
        Shader,
        Sprite,
        Texture,
        VideoClip,
    }
}