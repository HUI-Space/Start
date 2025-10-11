using System;
using System.Collections.Generic;
using System.Linq;
using Start;
using UnityEditor;
using UnityEngine;

public class TTypeEditorWindow : EditorWindow
{
    /*[MenuItem("Start/TTypeEditorWindow")]
    public static void ShowEditorWindow()
    {
        ShowWindow(new[] { typeof(int), typeof(FP), typeof(Array),typeof(Array[,]),typeof(List<>), typeof(Dictionary<,>) },null);
    }*/
    
    
    /// <summary>
    /// 显示
    /// </summary>
    /// <param name="containerTypes">容器类型</param>
    /// <param name="callback">回调</param>
    public static void ShowWindow(Type[] containerTypes,Action<TType> callback)
    {
        var window = GetWindow<TTypeEditorWindow>("递归泛型容器选择器");
        window.Initialize(containerTypes, callback);
        window.minSize = new Vector2(1000f, 600f);
        window.Show();
    }

    /// <summary>
    /// 滚动位置
    /// </summary>
    private Vector2 _scrollPosition;
    private Type[] _containerTypes;
    private TType _rootTType;
    private Action<TType> _callback;

    private void Initialize(Type[] containerTypes,Action<TType> callback)
    {
        _containerTypes = containerTypes;
        _callback = callback;
        if (TypeFactory.TryGetType(_containerTypes[0], out TType tType))
        {
            _rootTType = tType;
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("选择数据类型");
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        GenericType();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.LabelField("当前类型：", _rootTType.ToString());
        if (GUILayout.Button("确定"))
        {
            _callback?.Invoke(_rootTType);
            Close();
        }
    }

    private void GenericType()
    {
        int containerIndex = -1;
        int paramIndex = -1;
        if (_rootTType != null)
        {
            containerIndex = Array.IndexOf(_containerTypes, _rootTType.Type);
            paramIndex = containerIndex;
            containerIndex = Mathf.Clamp(containerIndex, 0, _containerTypes.Length - 1);
        }
        
        containerIndex = EditorGUILayout.Popup("1层 类型:", containerIndex,
            _containerTypes.Select(t => t.Name.Split('`')[0]).ToArray());


        if (containerIndex != paramIndex)
        {
            if (TypeFactory.TryGetType(_containerTypes[containerIndex], out TType type))
            {
                _rootTType = type;
            }
        }

        if (_rootTType is TGenericType tGenericType)
        {
            GenericType(_rootTType, tGenericType.GenericType);
        }

        if (_rootTType is TDictionary tDictionary)
        {
            GenericType(_rootTType, tDictionary.KeyGenericType, 2, 0);
            GenericType(_rootTType, tDictionary.ValueGenericType, 2, 1);
        }
    }

    private void GenericType(TType parentTType, TType tType, int depth = 2, int index = -1)
    {
        int containerIndex = -1;
        int paramIndex = -1;
        if (tType != null)
        {
            containerIndex = Array.IndexOf(_containerTypes, tType.Type);
            paramIndex = containerIndex;
            containerIndex = Mathf.Clamp(containerIndex, 0, _containerTypes.Length - 1);
        }

        containerIndex = EditorGUILayout.Popup($"{depth}层 类型:", containerIndex,
            _containerTypes.Select(t => t.Name.Split('`')[0]).ToArray());
        if (containerIndex != paramIndex)
        {
            if (TypeFactory.TryGetType(_containerTypes[containerIndex], out TType type))
            {
                if (parentTType is TGenericType tGenericType)
                {
                    tGenericType.SetTType(type);
                }

                if (parentTType is TDictionary tDictionary)
                {
                    if (index == 0)
                    {
                        tDictionary.SetKeyTType(type);
                    }

                    if (index == 1)
                    {
                        tDictionary.SetValueTType(type);
                    }
                }
            }
        }

        if (tType != null)
        {
            if (tType is TGenericType tGenericType)
            {
                GenericType(tGenericType, tGenericType.GenericType, depth + 1);
            }

            else if (tType is TDictionary tDictionary)
            {
                GenericType(tDictionary, tDictionary.KeyGenericType, depth + 1, 0);
                GenericType(tDictionary, tDictionary.ValueGenericType, depth + 1, 1);
            }
        }
    }
}