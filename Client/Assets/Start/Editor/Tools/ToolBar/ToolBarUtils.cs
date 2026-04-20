using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public static class ToolbarStyles
{
    public static readonly GUIStyle CommandButtonStyle;
    public static readonly GUIStyle SceneViewButtonStyle;

    static ToolbarStyles()
    {
        CommandButtonStyle = new GUIStyle("AppCommand")
        {
            fontSize = 13,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageAbove,
            fixedWidth = 30
        };
    }
}

/*public static class ScriptHelper
{
    private static readonly string SearchCommonFeaturePath = Path.Combine(Application.dataPath.Replace("/Assets", ""), Framework.Setting.EditorScriptRoot);

    private static string editorExePath;

    public static string EditorExePath
    {
        get
        {
            if (string.IsNullOrEmpty(editorExePath))
            {
                editorExePath = File.ReadAllText(Application.dataPath + "\\Script\\Editor\\ToolBar\\toolbarInfo.txt");
            }
            return editorExePath;
        }
    }
    public static void OpenUIModelScript()
    {
        OpenUIScript("Model");
    }
    
    public static void OpenUILogicScript()
    {
        OpenUIScript("View");
    }
    /// <summary>
    /// 打开选中GameObject的cs代码
    /// </summary>
    private static void OpenUIScript(string suffix)
    {
        
        var tar = Selection.activeGameObject;

        if (tar == null)
        {
            return;
        }

        var ui = tar.gameObject.transform;
        while (ui != null)
        {
            if (ui.GetComponent<Start.UIBase>() && ui.gameObject.name.StartsWith("UI")) break;
            ui = ui.transform.parent;
        }

        if (ui == null)
        {
            Debug.Log($"选择对象{tar.gameObject.name}不是一个View");
            return;
        }

        var uiName = "";
        try
        {
            if (suffix == "View")
            {
                uiName = ui.gameObject.name;
            }
            else
            {
                string pattern = $"\"{ui.gameObject.name.Replace("View", "")}\"[^,]*,\\s*[^,]*,\\s*[^,]*,\\s*[^,]*,\\s*\"(.*?)\"";
                var text = System.IO.File.ReadAllText(System.IO.Path.Combine(Application.dataPath, "../../Lua/MV.lua"));
                var matches = System.Text.RegularExpressions.Regex.Matches(text, pattern);
                foreach (var match in matches)
                {
                    uiName = suffix + ((System.Text.RegularExpressions.Match)match).Groups[1].Value;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }

        if (string.IsNullOrEmpty(uiName))
        {
            Debug.Log($"没有找到Name {uiName}");
            return;
        }
        
        var path = FindFile(uiName, SearchCommonFeaturePath);
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log($"不存在Lua脚本 {uiName}");
            return;
        }

        var scriptPath = $"{path}/{uiName}.lua";
        scriptPath = scriptPath.Replace('/', '\\');

        try
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = EditorExePath;
            startInfo.Arguments = scriptPath;
            process.StartInfo = startInfo;
            process.Start();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// 在指定路径下搜索文件
    /// </summary>
    /// <param name="name"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    private static string FindFile(string name, string path)
    {
        var files = Directory.GetFiles(path, "*.lua", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            var fileInfo = new FileInfo(files[i]);
            if (fileInfo.Name.Equals(name + ".lua"))
                return fileInfo.Directory?.FullName;
        }

        return string.Empty;
    }
    /// <summary>
    /// 搜索bind属性在文件的行数
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="go"></param>
    /// <returns></returns>
    private static int GetOpenFileLine(string filePath, GameObject go)
    {
        // string name = go.name;
        // var uiItem = go.GetComponent<IUIItem>();
        // if (uiItem == null)
        // {
        //     return 0;
        // }
        // if (string.IsNullOrEmpty(name))
        // {
        //     return 0;
        // }
        //
        // var btnClickEventName = string.Empty;
        // if (uiItem as UIButtonItem)
        // {
        //     btnClickEventName = $"On{name}Event";
        // }
        // var fileLines = File.ReadLines(filePath);
        // var lines = new List<int>();
        // var lineNumber = 0;
        // foreach (var fileLine in fileLines)
        // {
        //     lineNumber++;
        //     if (!string.IsNullOrEmpty(btnClickEventName) && fileLine.Contains($"{btnClickEventName}"))//优先匹配点击事件
        //     {
        //         return lineNumber;
        //     }
        //
        //     if (fileLine.Contains($"{name}"))
        //     {
        //         lines.Add(lineNumber);
        //     }
        // }
        //
        // if (lines.Count <= 0) 
        //     return 0;
        //
        //
        // return lines[lines.Count-1];
        return 0;
    }
}*/

public class GraphicSelector : Editor
{
    private static readonly Vector3[] FourCorners = new Vector3[4]; // Graphic 四点位置
    private static readonly Vector3 LabelOffset = new Vector3(-5, 10, 0); // Tips 位置偏移

    private static readonly GUIStyle LabelStyle = new GUIStyle
        { fontSize = 13, alignment = TextAnchor.MiddleLeft, normal = { textColor = Color.white } }; // Tips 文本样式

    private const float Size = 5; // 按钮大小
    private static readonly Vector3 Offset = new Vector3(Size, -Size, 0); // 左上按钮显示位置偏移

    static bool m_enabled;

    public static bool Enabled
    {
        get => m_enabled;
        set
        {
            m_enabled = value;
            EditorPrefs.SetBool("GraphicSelectorEnable", value);
        }
    }

    [InitializeOnLoadMethod]
    private static void Init()
    {
        m_enabled = EditorPrefs.GetBool("GraphicSelectorEnable");
        SceneView.duringSceneGui += OnSceneViewGUI;
    }

    private static void OnSceneViewGUI(SceneView sceneView)
    {
        if (!m_enabled)
        {
            return;
        }

        var mousePosition = Event.current.mousePosition;
        var ray = Camera.current.ScreenPointToRay(new Vector3(mousePosition.x,
            -mousePosition.y + Camera.current.pixelHeight));

        var currentPrefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        var allGraphics = currentPrefabStage != null
            ? currentPrefabStage.prefabContentsRoot.GetComponentsInChildren<Graphic>()
            : FindObjectsOfType<Graphic>();

        //开始绘制GUI 
        foreach (var g in allGraphics)
        {
            if (g.canvas == null)
            {
                continue;
            }

            var scale = g.canvas.rootCanvas.transform.localScale.x;

            if (g.enabled && g.gameObject.activeInHierarchy)
            {
                var color = Color.green;
                if (g.gameObject.name.StartsWith("@"))
                {
                    color = Color.cyan;
                }
                Handles.color = color;
                g.rectTransform.GetWorldCorners(FourCorners);

                var buttonPosition = FourCorners[1] + Offset * scale;

                if (Handles.Button(buttonPosition, Quaternion.identity, Size * scale, 0, Handles.RectangleHandleCap))
                {
                    Selection.activeObject = g.gameObject;
                    var current = Event.current;
                    if (current.control)
                    {
                        //ScriptHelper.OpenUILogicScript();
                    }
                }

                if (ray.origin.x < buttonPosition.x + Size * scale / 2
                    && ray.origin.x > buttonPosition.x - Size * scale / 2
                    && ray.origin.y < buttonPosition.y + Size * scale / 2
                    && ray.origin.y > buttonPosition.y - Size * scale / 2)
                {
                    Handles.Label(ray.origin + LabelOffset * scale, g.gameObject.name, LabelStyle);
                }
            }
        }
    }
}