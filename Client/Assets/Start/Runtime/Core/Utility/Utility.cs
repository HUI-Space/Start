using System;
using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;


using UnityEngine;

namespace Start
{
    public static class ExtensionUtility
    {
        /*public static Task GetAwaiter(this AsyncOperation asyncOperation)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            if (asyncOperation.isDone)
                tcs.SetResult(true);
            else
                asyncOperation.completed += operation => { tcs.SetResult(true); };
            return tcs.Task;
        }*/

        /*public static RecycleTask<AssetBundle> ToRecycleTask(this AssetBundleCreateRequest asyncOperation)
        {
            
        }*/
        
        public static RecycleTask GetAwaiter(this AsyncOperation asyncOperation)
        {
            var tcs = RecycleTask.Create();
            if (asyncOperation.isDone)
            {
                tcs.SetResult();
            }
            else
            {
                asyncOperation.completed += operation =>
                {
                    tcs.SetResult();
                };
            }
            return tcs.GetAwaiter();
        }
        
        public static async void Await(this Task task)
        {
            await task;
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
        public static readonly Vector2 Vector2Half = new Vector2(0.5f, 0.5f);
        public static readonly Vector3 Vector3Half = new Vector3(0.5f, 0.5f, 0.5f);

        #endregion

        #region Unity接口扩展

        public static void Switch(this CanvasGroup canvasGroup, bool isShow)
        {
            canvasGroup.alpha = isShow ? 1 : 0;
            canvasGroup.interactable = isShow;
            canvasGroup.blocksRaycasts = isShow;
        }
        
        public static void SetParent(this Transform transform , GameObject parent)
        {
            transform.SetParent(parent.transform);
        }
        
        public static void SetParent(this Transform transform , MonoBehaviour parent)
        {
            transform.SetParent(parent.transform);
        }

        public static void SetParent(this GameObject gameObject , GameObject parent)
        {
            gameObject.transform.SetParent(parent.transform);
        }
        
        public static void SetParent(this GameObject gameObject , Transform parent)
        {
            gameObject.transform.SetParent(parent);
        }
        
        public static void SetParent(this GameObject gameObject , MonoBehaviour monoBehaviour)
        {
            gameObject.transform.SetParent(monoBehaviour.transform);
        }
        
        public static T AddComponent<T>(this MonoBehaviour monoBehaviour) where T : Component
        {
            return monoBehaviour.gameObject.AddComponent<T>();
        }
        
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            return go.GetComponent<T>() == null ? go.AddComponent<T>() : go.GetComponent<T>();
        }
        
        public static T GetOrAddComponent<T>(this MonoBehaviour monoBehaviour) where T : Component
        {
            return monoBehaviour.GetComponent<T>() == null ? monoBehaviour.AddComponent<T>() : monoBehaviour.GetComponent<T>();
        }

        public static T1 GetOrAddComponent<T1, T2>(this GameObject go) where T1 : Component where T2 : T1
        {
            T1 t1 = go.GetComponent<T1>();
            if (t1 == null)
            {
                t1 = go.AddComponent<T2>();
            }
            return t1;
        }
        
        public static T1 GetOrAddComponent<T1, T2>(this MonoBehaviour monoBehaviour) where T1 : Component where T2 : T1
        {
            T1 t1 = monoBehaviour.GetComponent<T1>();
            if (t1 == null)
            {
                t1 = monoBehaviour.AddComponent<T2>();
            }
            return t1;
        }
        
        public static void SetSizeWithCurrentAnchors(this RectTransform rectTransform, float horizontal, float vertical)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontal);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, vertical);
        }
        
        /// <summary>
        /// 位置信息规范化
        /// </summary>
        public static void TransformNormalize(this Transform transform, string name, Transform parent)
        {
            if (parent != null)
            {
                transform.SetParent(parent);
            }

            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.gameObject.name = name;
            transform.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// UI位置信息规范化
        /// </summary>
        public static void RectTransformNormalize(this RectTransform rectTransform, string name, Transform parent)
        {
            if (parent != null)
            {
                rectTransform.SetParent(parent);
            }
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.gameObject.name = name;
            rectTransform.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 批量改Layer
        /// </summary>
        public static void BatchChangeLayer(this GameObject go, int layer)
        {
            go.layer = layer;
            Transform[] transforms = go.GetComponentsInChildren<Transform>(true);
            foreach (Transform transform in transforms)
            {
                transform.gameObject.layer = layer;
            }
        }
        
        /// <summary>
        /// 按名称递归查找子物体
        /// </summary>
        public static Transform FindDeepChild(this Transform parent, string name)
        {
            Transform result = parent.Find(name);
            if (result != null)
                return result;
            foreach (Transform child in parent)
            {
                result = child.FindDeepChild(name);
                if (result != null)
                    return result;
            }

            return null;
        }
        
        /// <summary>
        /// 查找所有子物体,不包含自身
        /// </summary>
        /// <param name="parent">父物体</param>
        /// <returns></returns>
        public static T[] FindAllChild<T>(this GameObject parent) where T : Component
        {
            if (parent == null)
            {
                Debug.LogError($"要查找的父物体{parent.name}为空");
                return null;
            }

            T[] childTrans = parent.GetComponentsInChildren<T>();
            T[] result = new T[childTrans.Length - 1];
            for (int i = 1; i < childTrans.Length; i++)
            {
                result[i - 1] = childTrans[i];
            }

            if (result.Length > 0)
                return result;
            return null;
        }
        
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childName">子物体名字</param>
        /// <returns></returns>
        public static Transform FindChild(this GameObject parent, string childName)
        {
            Transform[] childArray = parent.FindAllChild<Transform>();
            for (int i = 0; i < childArray.Length; i++)
            {
                if (childArray[i].name == childName)
                {
                    return childArray[i];
                }
            }

            return null;
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