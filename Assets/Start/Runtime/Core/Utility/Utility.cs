using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Start.Framework;
using UnityEngine;

namespace Start.Runtime
{
    /// <summary>
    /// 继承自MonoBehaviour的单例模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject(nameof(T)).AddComponent<T>();
                    _instance.transform.SetParent(Main.Root);
                }
                return _instance;
            }
        }
        private static T _instance;
    }
    
    public static class ExtensionUtility
    {
        public static RecycleTaskCompletionSource GetAwaiter(this AsyncOperation asyncOperation)
        {
            var tcs = RecycleTaskCompletionSource.Create();
            if (asyncOperation.isDone)
                tcs.SetResult();
            else
                asyncOperation.completed += operation => { tcs.SetResult(); };
            return tcs.GetAwaiter();
        }
    }
    
    public static class ZipUtility
    {
        /// <summary>
        /// 压缩多个文件到一个ZIP文件中
        /// </summary>
        /// <param name="zipFilePath">输出的ZIP文件路径</param>
        /// <param name="filesToZip">需要被压缩的文件路径集合</param>
        /// <param name="password">密码</param>
        public static void CompressFiles(string zipFilePath, string[] filesToZip,string password)
        {
            using (var fs = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (var zipStream = new ZipOutputStream(fs))
                {
                    zipStream.Password = password;
                    zipStream.SetLevel(9); // 0 - store only to 9 - means best compression
                    foreach (string file in filesToZip)
                    {
                        if (File.Exists(file))
                        {
                            var entry = new ZipEntry(Path.GetFileName(file));
                            entry.DateTime = DateTime.Now;
                            entry.Size = new FileInfo(file).Length;
                            zipStream.PutNextEntry(entry);

                            using (var inputFile = File.OpenRead(file))
                            {
                                byte[] buffer = new byte[4096];
                                StreamUtils.Copy(inputFile, zipStream, buffer);
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 解压ZIP文件到指定目录
        /// </summary>
        /// <param name="zipFilePath">ZIP文件路径</param>
        /// <param name="destinationDirectory">解压的目标目录</param>
        /// <param name="password">密码</param>
        public static void ExtractFiles(string zipFilePath, string destinationDirectory,string password)
        {
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            using (var fs = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var zipStream = new ZipInputStream(fs))
                {
                    zipStream.Password = password;
                    ZipEntry entry;
                    while ((entry = zipStream.GetNextEntry()) != null)
                    {
                        if (!entry.IsFile)
                        {
                            continue; // Ignore directories
                        }

                        string filePath = Path.Combine(destinationDirectory, entry.Name);
                        using (var streamWriter = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            StreamUtils.Copy(zipStream, streamWriter, new byte[4096]);
                        }
                    }
                }
            }
        }
    }
    
    public static class UnityUtility
    {
        #region Unity相关类，避免重复创建

        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

        #endregion

        #region Unity接口扩展

        public static void Switch(this CanvasGroup canvasGroup, bool isShow)
        {
            canvasGroup.alpha = isShow ? 1 : 0;
            canvasGroup.interactable = isShow;
            canvasGroup.blocksRaycasts = isShow;
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            return go.GetComponent<T>() == null ? go.AddComponent<T>() : go.GetComponent<T>();
        }

        #endregion

        #region Unity序列化

        /// <summary>
        ///     序列化（注意公共属性必须是public，并且可读的）
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="pretty">格式化</param>
        /// <returns></returns>
        public static string ToJson(object value, bool pretty = true)
        {
            return JsonUtility.ToJson(value, true);
        }

        /// <summary>
        ///     反序列（注意公共属性必须是public，并且可写的）
        /// </summary>
        /// <param name="json">内容</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static T FromJson<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        #endregion
    }
}