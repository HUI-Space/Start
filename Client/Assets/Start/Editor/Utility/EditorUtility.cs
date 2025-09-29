using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace Start.Editor
{
    public static partial class Utility
    {
        
        public static string GetFormattedName(this Type type)
        {
            // 获取不含命名空间的类名
            string className = type.Name;
        
            // 对于嵌套类型，提取最后一部分（如S.A中的A）
            if (type.IsNested)
            {
                className = className.Split('+').Last();
            }
        
            // 如果不是泛型，直接返回类名
            if (!type.IsGenericType)
            {
                return className;
            }
        
            // 对于泛型类型，处理泛型参数
            // 移除`数字部分（如Dictionary`2）
            int backtickIndex = className.IndexOf('`');
            if (backtickIndex > 0)
            {
                className = className.Substring(0, backtickIndex);
            }
        
            // 获取泛型参数
            Type[] genericArgs = type.GetGenericArguments();
        
            // 构建泛型参数的字符串表示
            StringBuilder sb = new StringBuilder();
            sb.Append(className);
            sb.Append("<");
            sb.Append(string.Join(", ", genericArgs.Select(arg => arg.GetFormattedName())));
            sb.Append(">");
        
            return sb.ToString();
        }
        
        /// <summary>
        /// 获取别名对应的类型
        /// </summary>
        /// <param name="alias">别名</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">无效的类型别名</exception>
        public static Type GetTypeFromAlias(string alias)
        {
            // 动态生成代码：定义一个返回指定类型的方法
            string code = $@"
            using System;
            using Start;
            using System.Collections.Generic;
            public class TypeHelper {{
                public static Type GetType() {{
                    return typeof({alias});
                }}
            }}";

            // 使用 C# 编译器动态编译
            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            {
                CompilerParameters parameters = new CompilerParameters();
                parameters.GenerateInMemory = true; // 仅在内存中生成，不写入磁盘

                CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);
                if (results.Errors.HasErrors)
                {
                    throw new ArgumentException($"无效的类型别名: {alias}");
                }

                // 调用动态生成的方法获取类型
                Assembly assembly = results.CompiledAssembly;
                Type helperType = assembly.GetType("TypeHelper");
                MethodInfo method = helperType.GetMethod("GetType");
                return (Type)method.Invoke(null, null);
            }
        }
        
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
        
        public static string[] FindAssets(ESearchType eSearchType, string searchInFolder)
        {
            return FindAssets(eSearchType, new string[] { searchInFolder });
        }
        
        public static string[] FindAssets(ESearchType eSearchType, string[] searchInFolders)
        {
            // 注意：AssetDatabase.FindAssets()不支持末尾带分隔符的文件夹路径
            for (int i = 0; i < searchInFolders.Length; i++)
            {
                string folderPath = searchInFolders[i];
                searchInFolders[i] = folderPath.TrimEnd('/');
            }

            // 注意：获取指定目录下的所有资源对象（包括子文件夹）
            string[] guids;
            if (eSearchType == ESearchType.All)
                guids = AssetDatabase.FindAssets(string.Empty, searchInFolders);
            else
                guids = AssetDatabase.FindAssets($"t:{eSearchType}", searchInFolders);

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

        /// <summary>
        /// 获取Game View的分辨率
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void GetGameViewSize(out int width, out int height)
        {
            Type T = Type.GetType("UnityEditor.GameView,UnityEditor");
            MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
            object result = GetSizeOfMainGameView.Invoke(null, null);
            Vector2 v = (Vector2) result;
            width = (int) v.x;
            height = (int) v.y;
        }
        
    }
    

    public enum ESearchType
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