#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class FindReferences : EditorWindow
{
    public static void OpenEditorWindow()
    {
        var window = GetWindow<FindReferences>("资源引用查找");
        //window.Initialize();
        window.position = new Rect(0, 0, 1200, 715);
    }

    private class RefrenceResult
    {
        public int DuplicateID = -1;
        public string SourceName;
        public string SourcePath;
        public string[] Refrences;
        public bool Expand;
        public bool Skip;

        public RefrenceResult(string sourceName, string sourcePath, string[] refrences)
        {
            SourceName = sourceName;
            SourcePath = sourcePath;
            Refrences = refrences;
        }
    }

    private enum EAssetType
    {
        Texture,
        Material,
        Shader,
        Prefab,
    }

    int ResultCountEachPage = 20;

    string[] RefrenceFilters = new string[]
    {
        "t:GameObject",
        "t:Scene",
        "t:Material",
    };


    private EAssetType mSelectedAssetType = EAssetType.Texture;
    private string[] mSearchFolder = new string[] {"Assets/Game/GameAsset"};
    private Dictionary<string, HashSet<string>> mRefrenceDependencies = new Dictionary<string, HashSet<string>>();
    private List<RefrenceResult> mRefrenceResult = new List<RefrenceResult>();
    private string mFilterRegix;
    private SearchField mFilterSearchField;
    private Vector2 mScrollPosition;
    private int mPageIndex;
    private int mMaxPageIndex;
    private bool mHasInitialied;
    private bool mIsCheckDuplicate;

    private void OnEnable()
    {
        this.mHasInitialied = false;
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("初始化 [当资源引用发生变化时，点击此按钮重新初始化]"))
        {
            this.Initialize();
        }

        if (GUILayout.Button("清理查重缓存 [当资源引用发生变化时，点击此按钮]"))
        {
            this.MD5Cache.Clear();
        }

        EditorGUILayout.EndHorizontal();

        if (this.mHasInitialied == false)
        {
            ShowNotification(new GUIContent("当前尚未初始化，点击初始化后方可使用"));
        }

        GUI.enabled = this.mHasInitialied;

        this.DrawHeader();
        if (this.mRefrenceResult.Count == 0)
        {
            if (this.mIsCheckDuplicate)
            {
                ShowNotification(new GUIContent("没有任何重复资源！"));
            }

            return;
        }

        this.DrawFilter();
        this.DrawContentHeader();
        DrawDeleteNoReference(); //临时添加
        this.DrawContent();
        this.DrawPage();

        GUI.enabled = true;
    }

    private void Initialize()
    {
        this.mRefrenceDependencies.Clear();
        this.mRefrenceResult.Clear();
        this.mFilterRegix = "";
        this.mFilterSearchField = new SearchField();
        this.mScrollPosition = Vector2.zero;
        this.mPageIndex = 0;
        float nIndex = 0;
        foreach (string rFilter in RefrenceFilters)
        {
            string[] rAssetGUIDs = AssetDatabase.FindAssets(rFilter);
            EditorUtility.DisplayProgressBar("初始化...", "这可能需要几十秒至一分钟...", nIndex++ / RefrenceFilters.Length);
            foreach (string rAssetGUID in rAssetGUIDs)
            {
                string rAssetPath = AssetDatabase.GUIDToAssetPath(rAssetGUID);
                string[] rAssetDependencies = AssetDatabase.GetDependencies(rAssetPath);
                this.mRefrenceDependencies[rAssetPath] = new HashSet<string>(rAssetDependencies);
            }
        }

        EditorUtility.ClearProgressBar();
        this.mHasInitialied = true;
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(this.mSearchFolder[0] + " [单击此处更换检查资源路径]", GUI.skin.textField))
        {
            string rNewFolder = EditorUtility.OpenFolderPanel("Find In Folder", this.mSearchFolder[0], "");
            if (string.IsNullOrEmpty(rNewFolder) == false)
            {
                this.mSearchFolder[0] = rNewFolder.Substring(rNewFolder.IndexOf("Assets/"));
            }
        }

        this.mSelectedAssetType = (EAssetType) EditorGUILayout.EnumPopup(this.mSelectedAssetType, GUILayout.Width(150f));
        if (GUILayout.Button("引用搜索", GUILayout.Width(150f)))
        {
            this.FindRefrences();
        }

        if (GUILayout.Button("资源查重", GUILayout.Width(150f)))
        {
            this.BeginDetectionDuplicate();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawFilter()
    {
        EditorGUILayout.BeginHorizontal();
        string rFilterRegix = this.mFilterSearchField.OnGUI(string.IsNullOrEmpty(this.mFilterRegix) ? "按名称搜索资源" : this.mFilterRegix);

        if (rFilterRegix != this.mFilterRegix && rFilterRegix != "按名称搜索资源")
        {
            this.mFilterRegix = rFilterRegix;
            Apply();
        }

        if (GUILayout.Button("导出CSV", GUILayout.Width(150f)))
        {
            Export();
        }

        EditorGUILayout.EndHorizontal();

        void Apply()
        {
            int nCounter = 0;
            foreach (RefrenceResult rRefrenceResult in this.mRefrenceResult)
            {
                rRefrenceResult.Skip = !rRefrenceResult.SourcePath.Contains(rFilterRegix);
                if (rRefrenceResult.Skip == false)
                {
                    nCounter++;
                }
            }

            this.mMaxPageIndex = Mathf.FloorToInt((float) nCounter / ResultCountEachPage);
        }

        void Export()
        {
            string rSavePath = EditorUtility.SaveFilePanel("导出数据", "", $"Invalid{this.mSelectedAssetType}", "csv");
            if (string.IsNullOrEmpty(rSavePath))
            {
                return;
            }

            int nCount;
            StringBuilder rText = new StringBuilder();
            rText.AppendLine($"资源名称,资源路径");
            if (this.mIsCheckDuplicate)
            {
                nCount = this.mRefrenceResult.Count;
                int nID = -1;
                foreach (RefrenceResult rRefrenceResult in this.mRefrenceResult)
                {
                    if (nID != rRefrenceResult.DuplicateID)
                    {
                        if (nID != -1)
                        {
                            rText.AppendLine();
                        }

                        nID = rRefrenceResult.DuplicateID;
                    }

                    rText.AppendLine($"\"{rRefrenceResult.SourceName}\",\"{rRefrenceResult.SourcePath}\"");
                }
            }
            else
            {
                int nIndex = 0;
                foreach (RefrenceResult rRefrenceResult in this.mRefrenceResult)
                {
                    if (rRefrenceResult.Refrences.Length > 0)
                    {
                        continue;
                    }

                    rText.AppendLine($"\"{rRefrenceResult.SourceName}\",\"{rRefrenceResult.SourcePath}\"");

                    if (nIndex > 0 && nIndex % 10 == 0)
                    {
                        rText.AppendLine();
                    }

                    nIndex++;
                }

                nCount = nIndex;
            }

            using (StreamWriter writer = new StreamWriter(rSavePath, false, Encoding.UTF8))
            {
                writer.Write(rText.ToString());
            }

            EditorUtility.DisplayDialog("提示", $"导出成功,无效资源共计{nCount}个。", "知道了");
        }
    }

    private void DrawContentHeader()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("序号", GUILayout.Width(30f));
        EditorGUILayout.LabelField("资源名称", GUILayout.Width(300f));
        EditorGUILayout.LabelField("完整路径");
        if (GUILayout.Button("引用次数", GUILayout.Width(150f)))
        {
            bool bOrder = this.mRefrenceResult[0].Refrences.Length < this.mRefrenceResult[this.mRefrenceResult.Count - 1].Refrences.Length;
            if (bOrder)
            {
                this.mRefrenceResult.Sort((x, y) => y.Refrences.Length.CompareTo(x.Refrences.Length));
            }
            else
            {
                this.mRefrenceResult.Sort((x, y) => x.Refrences.Length.CompareTo(y.Refrences.Length));
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawDeleteNoReference()
    {
        if (GUILayout.Button("删除无引用的资源", GUILayout.Width(400f)))
        {
            List<RefrenceResult> results = new List<RefrenceResult>();
            for (int i = 0; i < this.mRefrenceResult.Count; i++)
            {
                RefrenceResult rRefrenceResult = this.mRefrenceResult[i];
                if (rRefrenceResult.Skip)
                {
                    continue;
                }
                else
                {
                    if (rRefrenceResult.Refrences.Length == 0)
                    {
                        results.Add(rRefrenceResult);
                    }
                }
            }

            for (int i = 0; i < results.Count; i++)
            {
                EditorUtility.DisplayProgressBar("删除中", $"{i + 1}/{results.Count}", (float) i / (results.Count - 1));
                AssetDatabase.DeleteAsset(results[i].SourcePath);
            }

            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }
    }

    Rect rect;

    private void DrawContent()
    {
        float nCenterWidth = this.position.width - 300f - 47.5f - 150f;
        this.mScrollPosition = EditorGUILayout.BeginScrollView(this.mScrollPosition);
        if (this.mIsCheckDuplicate)
        {
            DrawByDuplicate();
        }
        else
        {
            DrawByNormal();
        }

        EditorGUILayout.EndScrollView();

        void DrawByNormal()
        {
            int nStartIndex = mPageIndex * ResultCountEachPage;
            int nEndIndex = Mathf.Min((mPageIndex + 1) * ResultCountEachPage, this.mRefrenceResult.Count);
            int nIndex = 0;
            for (int i = 0; i < this.mRefrenceResult.Count; i++)
            {
                RefrenceResult rRefrenceResult = this.mRefrenceResult[i];
                if (rRefrenceResult.Skip)
                {
                    continue;
                }
                else
                {
                    if (nIndex >= nStartIndex)
                    {
                        GUI.color = nIndex % 2 == 0 ? Color.white : Color.white * 0.6f;
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        GUI.color = Color.white;

                        if (rRefrenceResult.Refrences.Length == 0)
                        {
                            DrawRefrenceResultNoRefrences(nIndex, rRefrenceResult);
                        }
                        else
                        {
                            DrawRefrenceResult(nIndex, rRefrenceResult);
                        }

                        EditorGUILayout.EndVertical();
                    }

                    nIndex++;
                    if (nIndex >= nEndIndex)
                    {
                        break;
                    }
                }
            }
        }

        void DrawByDuplicate()
        {
            int nIndex = 0;
            for (int i = 0; i < this.mRefrenceResult.Count; i++)
            {
                RefrenceResult rRefrenceResult = this.mRefrenceResult[i];
                if (rRefrenceResult.DuplicateID != this.mPageIndex)
                {
                    continue;
                }

                GUI.color = nIndex++ % 2 == 0 ? Color.white : Color.white * 0.6f;
                EditorGUILayout.BeginVertical(GUI.skin.box);
                GUI.color = Color.white;

                if (rRefrenceResult.Refrences.Length == 0)
                {
                    DrawRefrenceResultNoRefrences(nIndex, rRefrenceResult);
                }
                else
                {
                    DrawRefrenceResult(nIndex, rRefrenceResult);
                }

                EditorGUILayout.EndVertical();
            }
        }

        void DrawRefrenceResultNoRefrences(int index, RefrenceResult result)
        {
            rect = EditorGUILayout.BeginHorizontal();

            result.Expand = false;
            EditorGUILayout.LabelField(index.ToString(), GUILayout.Width(30f));
            EditorGUILayout.LabelField(result.SourceName, GUILayout.Width(300f));
            EditorGUILayout.LabelField(result.SourcePath, GUILayout.Width(nCenterWidth));
            GUI.color = Color.red;
            EditorGUILayout.LabelField("0", GUI.skin.button, GUILayout.Width(150f));
            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();

            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(result.SourcePath));
            }
        }

        void DrawRefrenceResult(int index, RefrenceResult result)
        {
            rect = EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(index.ToString(), GUILayout.Width(30f));
            EditorGUILayout.LabelField(result.SourceName, GUILayout.Width(300f));
            EditorGUILayout.LabelField(result.SourcePath, GUILayout.Width(nCenterWidth));
            GUI.color = Color.green;
            EditorGUILayout.LabelField(result.Refrences.Length.ToString(), GUI.skin.button, GUILayout.Width(150f));
            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();

            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(result.SourcePath));
                result.Expand = !result.Expand;
            }

            if (result.Expand)
            {
                foreach (string rRefrence in result.Refrences)
                {
                    rect = EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(index.ToString(), GUILayout.Width(30f));
                    EditorGUILayout.LabelField("-" + Path.GetFileName(rRefrence), GUILayout.Width(300f));
                    EditorGUILayout.LabelField(rRefrence, GUILayout.Width(nCenterWidth));
                    EditorGUILayout.LabelField("定位", GUI.skin.button, GUILayout.Width(150f));

                    EditorGUILayout.EndHorizontal();

                    if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
                    {
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(rRefrence));
                    }
                }
            }
        }
    }

    private void DrawPage()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("上一页"))
        {
            this.mPageIndex--;
        }

        this.mPageIndex = EditorGUILayout.IntField($"当前页[最大{this.mMaxPageIndex}]", this.mPageIndex);
        if (GUILayout.Button("下一页"))
        {
            this.mPageIndex++;
        }

        this.mPageIndex = Mathf.Clamp(this.mPageIndex, 0, this.mMaxPageIndex);
        EditorGUILayout.EndHorizontal();
    }

    private void FindRefrences()
    {
        this.mIsCheckDuplicate = false;
        this.mRefrenceResult.Clear();

        string[] rAssetGUIDs = AssetDatabase.FindAssets($"t:{this.mSelectedAssetType}", this.mSearchFolder);
        string[] rAssetPaths = new string[rAssetGUIDs.Length];
        for (int i = 0; i < rAssetGUIDs.Length; i++)
        {
            rAssetPaths[i] = AssetDatabase.GUIDToAssetPath(rAssetGUIDs[i]);
        }

        List<string> rRefrences = new List<string>();
        int nIndex = 0;
        foreach (string rAssetPath in rAssetPaths)
        {
            if (nIndex % Mathf.Max(rAssetPaths.Length / 10, 1) == 0)
            {
                EditorUtility.DisplayProgressBar("查找中...", $"{nIndex}/{rAssetPaths.Length}", (float) nIndex / rAssetPaths.Length);
            }

            nIndex++;
            rRefrences.Clear();
            foreach (KeyValuePair<string, HashSet<string>> rDependencies in this.mRefrenceDependencies)
            {
                if (rDependencies.Value.Contains(rAssetPath) && rDependencies.Key != rAssetPath)
                {
                    rRefrences.Add(rDependencies.Key);
                }
            }

            this.mRefrenceResult.Add(new RefrenceResult(Path.GetFileNameWithoutExtension(rAssetPath), rAssetPath, rRefrences.ToArray()));
        }

        EditorUtility.ClearProgressBar();
        this.mRefrenceResult.Sort((x, y) => x.Refrences.Length.CompareTo(y.Refrences.Length));
        this.mMaxPageIndex = Mathf.CeilToInt((float) this.mRefrenceResult.Count / ResultCountEachPage) - 1;
        EditorUtility.DisplayDialog("提示", $"查找完毕,资源共计{this.mRefrenceResult.Count}个。", "知道了");
    }

    private object locker = new object();

    private void BeginDetectionDuplicate()
    {
        this.mIsCheckDuplicate = true;
        this.mRefrenceResult.Clear();

        string[] rCheckAssetGUIDs = AssetDatabase.FindAssets($"t:{this.mSelectedAssetType}", this.mSearchFolder);
        string[] rAllAssetGUIDs = AssetDatabase.FindAssets($"t:{this.mSelectedAssetType}");

        HashSet<string> rCheckAssetGUIDHashSet = new HashSet<string>(rCheckAssetGUIDs);
        Queue<KeyValuePair<string, bool>> rAssetQueue = new Queue<KeyValuePair<string, bool>>(rAllAssetGUIDs.Length);

        foreach (var rAssetGUID in rAllAssetGUIDs)
        {
            string rAssetPath = AssetDatabase.GUIDToAssetPath(rAssetGUID);
            bool bIsCheckAsset = rCheckAssetGUIDHashSet.Contains(rAssetGUID);
            rAssetQueue.Enqueue(new KeyValuePair<string, bool>(rAssetPath, bIsCheckAsset));
        }

        bool bCancel = false;
        Dictionary<string, HashSet<string>> rMD5ToPathsDictionary = new Dictionary<string, HashSet<string>>();
        int nCounter = 0;
        float total = rAllAssetGUIDs.Length;

        if (total > 0)
        {
            for (int i = 0; i < Mathf.Max(1, (int) total / 50); i++)
            {
                Task.Factory.StartNew(DetectionTask);
            }

            while (!bCancel && nCounter < total)
            {
                bCancel = EditorUtility.DisplayCancelableProgressBar("资源比对中...", $"当前进度{nCounter}/{total}...", nCounter / total);
                Thread.Sleep(100);
            }
        }

        List<string> rRefrences = new List<string>();
        int nID = 0;
        foreach (KeyValuePair<string, HashSet<string>> rMD5ToPathsPair in rMD5ToPathsDictionary)
        {
            if (rMD5ToPathsPair.Value.Count < 2)
            {
                continue;
            }

            int nIndex = 0;
            string[] rAssetPaths = rMD5ToPathsPair.Value.ToArray();
            foreach (string rAssetPath in rAssetPaths)
            {
                if (nIndex % Mathf.Max(rAssetPaths.Length / 10, 1) == 0)
                {
                    EditorUtility.DisplayProgressBar("查找中...", $"{nIndex}/{rAssetPaths.Length}", (float) nIndex / rAssetPaths.Length);
                }

                nIndex++;
                rRefrences.Clear();
                foreach (KeyValuePair<string, HashSet<string>> rDependencies in this.mRefrenceDependencies)
                {
                    if (rDependencies.Value.Contains(rAssetPath) && rDependencies.Key != rAssetPath)
                    {
                        rRefrences.Add(rDependencies.Key);
                    }
                }

                var result = new RefrenceResult(Path.GetFileNameWithoutExtension(rAssetPath), rAssetPath, rRefrences.ToArray());
                result.DuplicateID = nID;
                this.mRefrenceResult.Add(result);
            }

            nID++;
        }

        EditorUtility.ClearProgressBar();
        this.mMaxPageIndex = nID - 1;
        EditorUtility.DisplayDialog("提示", $"查找完毕,资源共计{this.mRefrenceResult.Count}个。", "知道了");

        void DetectionTask()
        {
            while (!bCancel && rAssetQueue.Count > 0)
            {
                KeyValuePair<string, bool> rAsset = rAssetQueue.Dequeue();
                try
                {
                    string rAssetMD5 = GetFileMD5(rAsset.Key);
                    if (rAsset.Value)
                    {
                        HashSet<string> rAssetPaths;
                        if (rMD5ToPathsDictionary.TryGetValue(rAssetMD5, out rAssetPaths) == false)
                        {
                            rAssetPaths = new HashSet<string>();
                            rMD5ToPathsDictionary.Add(rAssetMD5, rAssetPaths);
                        }

                        rAssetPaths.Add(rAsset.Key);
                    }
                    else
                    {
                        if (rMD5ToPathsDictionary.TryGetValue(rAssetMD5, out HashSet<string> rAssetPaths))
                        {
                            rAssetPaths.Add(rAsset.Key);
                        }
                    }
                }
                finally
                {
                    lock (locker)
                    {
                        nCounter++;
                    }
                }
            }
        }
    }

    private Dictionary<string, string> MD5Cache = new Dictionary<string, string>();

    private string GetFileMD5(string filePath)
    {
        if (MD5Cache.TryGetValue(filePath, out string MD5))
        {
            return MD5;
        }

        //将文件路径转化为文件流
        FileStream fs = new FileStream(filePath, FileMode.Open);
        byte[] data = new byte[(int) fs.Length];

        //读文件
        fs.Read(data, 0, (int) fs.Length);
        fs.Close();
        fs.Dispose();

        //通过MD5接口生成MD5码（获得的是Hash一个字节数组）MD5加密
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] result = md5.ComputeHash(data);

        //将获得的Hash字节数组转换为字符串
        string fileMD5 = "";
        foreach (byte b in result)
        {
            fileMD5 += Convert.ToString(b, 16);
        }

        MD5Cache.Add(filePath, fileMD5);
        //fs.Close();
        return fileMD5;
    }
}
#endif