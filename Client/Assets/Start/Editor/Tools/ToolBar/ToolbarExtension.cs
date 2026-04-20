using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[InitializeOnLoad]
public class ToolbarExtend
{
    static ScriptableObject _toolbar;
    static int _toolIconCount; //左侧的: 抓手, 移动, 旋转等那几个工具

    static GUIStyle _commandStyle;

    static ToolbarExtend()
    {
        EditorApplication.update -= OnEditorUpdate;
        EditorApplication.update += OnEditorUpdate;
    }

    static void OnEditorUpdate()
    {
        if (null != _toolbar) return;

        Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        InitToolIconCount(toolbarType);
        RegToolbarOnGUI(toolbarType);
    }

    static void InitToolIconCount(Type toolbarType)
    {
#if UNITY_2019_1_OR_NEWER
        string fieldName = "k_ToolCount";
#else
        string fieldName = "s_ShownToolIcons";
#endif
        var fieldInfo = toolbarType.GetField(fieldName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static); //Toolbar.s_ShownToolIcons
#if UNITY_2019_1_OR_NEWER
        _toolIconCount = fieldInfo != null ? ((int)fieldInfo.GetValue(null)) : 7;
#elif UNITY_2018_1_OR_NEWER
        _toolIconCount = fieldInfo != null ? ((Array) fieldInfo.GetValue(null)).Length : 6;
#else
        _toolIconCount = fieldInfo != null ? ((Array) fieldInfo.GetValue(null)).Length : 5;
#endif
    }

    //注册Toolbar的OnGUIHandler的监听
    static void RegToolbarOnGUI(Type toolbarType)
    {
        // Find toolbar
        var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
        _toolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
        if (null == _toolbar) return;

        Type guiViewType = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");
        PropertyInfo propertyInfo = guiViewType.GetProperty("visualTree",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //toolbar上可放置ui的容器, 我们这边放在第1个容器
        var visualTree = (VisualElement)propertyInfo.GetValue(_toolbar, null);
        var container = (IMGUIContainer)visualTree[0];

        //监听OnGUI
        FieldInfo handlerFieldInfo = typeof(IMGUIContainer).GetField("m_OnGUIHandler",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var handler = (Action)handlerFieldInfo.GetValue(container);
        handler -= OnGUI;
        handler += OnGUI;
        handlerFieldInfo.SetValue(container, handler);
    }

    //Toolbar的OnGUI
    static void OnGUI()
    {
        var screenWidth = EditorGUIUtility.currentViewWidth;

        // Following calculations match code reflected from Toolbar.OldOnGUI()
        float playButtonX = (screenWidth - 100) / 2;

        OnGUIRight(screenWidth, playButtonX);
    }

    static void OnGUILeft(float screenWidth, float playButtonX)
    {
        Rect leftRect = new Rect(0, 0, screenWidth, Screen.height);
        leftRect.xMin += 10; // Spacing left
        leftRect.xMin += 32 * _toolIconCount; // Tool buttons
        leftRect.xMin += 20; // Spacing between tools and pivot
        leftRect.xMin += 64 * 2; // Pivot/Center, Local/World
        leftRect.xMax = playButtonX;

        // Add spacing around existing controls
        leftRect.xMin += 10;
        leftRect.xMax -= 10;
        // Add top and bottom margins
        leftRect.y = 7;
        leftRect.height = 24;

        if (leftRect.width > 0)
        {
            GUILayout.BeginArea(leftRect);
            GUILayout.BeginHorizontal();

            //********** 在这边绘制自定义ui
            GUILayout.FlexibleSpace();
            GUILayout.Label("Toolbar左侧");
            GUILayout.TextField("", GUILayout.Width(100));
            if (GUILayout.Button("测试按钮"))
            {
                
            }
            //**********

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }

    static bool isIPhone = false;
    static void OnGUIRight(float screenWidth, float playButtonX)
    {
        if (_commandStyle == null)
            _commandStyle = new GUIStyle("CommandLeft");

        Rect rightRect = new Rect(0, 0, screenWidth, Screen.height);
        rightRect.xMin = playButtonX;
        rightRect.xMin -= _commandStyle.fixedWidth * 5; // Play buttons
        rightRect.xMax = screenWidth;
        rightRect.xMax -= 10; // Spacing right
        rightRect.xMax -= 80; // Layout
        rightRect.xMax -= 10; // Spacing between layout and layers
        rightRect.xMax -= 80; // Layers
        rightRect.xMax -= 20; // Spacing between layers and account
        rightRect.xMax -= 80; // Account
        rightRect.xMax -= 10; // Spacing between account and cloud
        rightRect.xMax -= 32; // Cloud
        rightRect.xMax -= 10; // Spacing between cloud and collab
        rightRect.xMax -= 78; // Colab

        // Add spacing around existing controls
        rightRect.xMin -= 10;
        rightRect.xMax -= 10;
        // Add top and bottom margins
        rightRect.y = 5;
        rightRect.height = 24;

        if (rightRect.width > 0)
        {
            GUILayout.BeginArea(rightRect);
            GUILayout.BeginHorizontal();

            //********** 在这边绘制自定义ui
            // GUILayout.Label("异形屏:", GUILayout.Width(42));
            // var buttonName = isIPhone ? "iPhone14" : "None";
            // if (GUILayout.Button(buttonName, GUILayout.Width(100)))
            // {
            //     isIPhone = !isIPhone;
            //     var offset = isIPhone ? 44 : 0;
            //     ScreenAdapter.UpdateDisplayCutout(offset);
            // }
            
            
            GenUguiToggle();

            GenOpenModelScriptBtn();
            GenOpenLogicScriptBtn();
            
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }

    static void GenUguiToggle()
    {
        GUILayout.Space(30);
        var iconContent = EditorGUIUtility.IconContent(@"UnityEditor.SceneView");
        iconContent.tooltip = "启用/关闭Ugui快捷选中";
        GraphicSelector.Enabled = GUILayout.Toggle(GraphicSelector.Enabled, iconContent, ToolbarStyles.CommandButtonStyle);

    }

    static void GenOpenModelScriptBtn()
    {
        var iconContent = EditorGUIUtility.IconContent("d_Profiler.UI");
        iconContent.tooltip = "打开界面Model代码";
        if (GUILayout.Button(iconContent, ToolbarStyles.CommandButtonStyle))
        {
            //ScriptHelper.OpenUIModelScript();
        }
    }
    
    static void GenOpenLogicScriptBtn()
    {
        var iconContent = EditorGUIUtility.IconContent("d_Profiler.UIDetails");
        iconContent.tooltip = "打开界面View代码";
        if (GUILayout.Button(iconContent, ToolbarStyles.CommandButtonStyle))
        {
            //ScriptHelper.OpenUILogicScript();
        }
    }

}