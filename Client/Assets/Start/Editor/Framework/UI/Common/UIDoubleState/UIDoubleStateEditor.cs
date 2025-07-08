using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    [CustomEditor(typeof(UIDoubleState), true)]
    public class UIDoubleStateEditor : UnityEditor.Editor
    {
        private bool _initialize;
        private SerializedProperty _ignoreParent;
        private SerializedProperty _children;

        private SerializedProperty _textureChange;
        private SerializedProperty _onTexture;
        private SerializedProperty _offTexture;

        private SerializedProperty _setNativeSize;

        private SerializedProperty _spriteChange;
        private SerializedProperty _onSprite;
        private SerializedProperty _offSprite;

        private SerializedProperty _colorChange;
        private SerializedProperty _onColor;
        private SerializedProperty _offColor;

        private SerializedProperty _textChange;
        private SerializedProperty _onText;
        private SerializedProperty _offText;

        private SerializedProperty _textSizeChange;
        private SerializedProperty _onTextSize;
        private SerializedProperty _offTextSize;

        private SerializedProperty _raycastChange;
        private SerializedProperty _onRaycast;
        private SerializedProperty _offRaycast;

        private SerializedProperty _changeCanvasGroup;
        private SerializedProperty _canvasGroup;
        private SerializedProperty _onSwitch;
        private SerializedProperty _offSwitch;

        private SerializedProperty _changeActive;
        private SerializedProperty _onActive;
        private SerializedProperty _offActive;

        private SerializedProperty _changeInteractable;
        private SerializedProperty _selectable;
        private SerializedProperty _onInteractable;
        private SerializedProperty _offInteractable;

        private void OnEnable()
        {
            _ignoreParent = serializedObject.FindProperty("IgnoreParent");
            _children = serializedObject.FindProperty("Children");

            _textureChange = serializedObject.FindProperty("TextureChange");
            _onTexture = serializedObject.FindProperty("OnTexture");
            _offTexture = serializedObject.FindProperty("OffTexture");

            _setNativeSize = serializedObject.FindProperty("SetNativeSize");

            _spriteChange = serializedObject.FindProperty("SpriteChange");
            _onSprite = serializedObject.FindProperty("OnSprite");
            _offSprite = serializedObject.FindProperty("OffSprite");

            _colorChange = serializedObject.FindProperty("ColorChange");
            _onColor = serializedObject.FindProperty("OnColor");
            _offColor = serializedObject.FindProperty("OffColor");

            _textChange = serializedObject.FindProperty("TextChange");
            _onText = serializedObject.FindProperty("OnText");
            _offText = serializedObject.FindProperty("OffText");

            _textSizeChange = serializedObject.FindProperty("TextSizeChange");
            _onTextSize = serializedObject.FindProperty("OnTextSize");
            _offTextSize = serializedObject.FindProperty("OffTestSize");

            _raycastChange = serializedObject.FindProperty("RaycastChange");
            _onRaycast = serializedObject.FindProperty("OnRaycast");
            _offRaycast = serializedObject.FindProperty("OffRaycast");

            _changeCanvasGroup = serializedObject.FindProperty("ChangeCanvasGroup");
            _canvasGroup = serializedObject.FindProperty("CanvasGroup");
            _onSwitch = serializedObject.FindProperty("OnSwitch");
            _offSwitch = serializedObject.FindProperty("OffSwitch");

            _changeActive = serializedObject.FindProperty("ChangeActive");
            _onActive = serializedObject.FindProperty("OnActive");
            _offActive = serializedObject.FindProperty("OffActive");

            _changeInteractable = serializedObject.FindProperty("ChangeInteractable");
            _selectable = serializedObject.FindProperty("Selectable");
            _onInteractable = serializedObject.FindProperty("OnInteractable");
            _offInteractable = serializedObject.FindProperty("OffInteractable");

            UIDoubleState scroll = (UIDoubleState)target;
            if (!_initialize)
            {
                scroll.Initialize();
                _initialize = true;
            }
        }

        private void OnDisable()
        {
            _initialize = false;
        }

        public override void OnInspectorGUI()
        {
            UIDoubleState uiDoubleState = (UIDoubleState)target;
            serializedObject.Update();
            EditorGUILayout.PropertyField(_ignoreParent, true);
            EditorGUILayout.PropertyField(_children, true);
            switch (uiDoubleState.GraphicType)
            {
                case EGraphicType.RawImage:
                    EditorGUILayout.PropertyField(_textureChange, true);
                    if (uiDoubleState.TextureChange)
                    {
                        EditorGUILayout.PropertyField(_onTexture, true);
                        EditorGUILayout.PropertyField(_offTexture, true);
                    }
                    EditorGUILayout.PropertyField(_setNativeSize, true);
                    EditorGUILayout.PropertyField(_colorChange, true);
                    if (uiDoubleState.ColorChange)
                    {
                        EditorGUILayout.PropertyField(_onColor, true);
                        EditorGUILayout.PropertyField(_offColor, true);
                    }
                    break;
                case EGraphicType.Image:
                    EditorGUILayout.PropertyField(_spriteChange, true);
                    if (uiDoubleState.SpriteChange)
                    {
                        EditorGUILayout.PropertyField(_onSprite, true);
                        EditorGUILayout.PropertyField(_offSprite, true);
                    }
                    EditorGUILayout.PropertyField(_setNativeSize, true);
                    EditorGUILayout.PropertyField(_colorChange, true);
                    if (uiDoubleState.ColorChange)
                    {
                        EditorGUILayout.PropertyField(_onColor, true);
                        EditorGUILayout.PropertyField(_offColor, true);
                    }
                    break;
                case EGraphicType.Text:
                    EditorGUILayout.PropertyField(_textChange, true);
                    if (uiDoubleState.TextChange)
                    {
                        EditorGUILayout.PropertyField(_onText, true);
                        EditorGUILayout.PropertyField(_offText, true);
                    }
                    EditorGUILayout.PropertyField(_textSizeChange, true);
                    if (uiDoubleState.TextSizeChange)
                    {
                        EditorGUILayout.PropertyField(_onTextSize, true);
                        EditorGUILayout.PropertyField(_offTextSize, true);
                    }
                    break;
                case EGraphicType.TextMeshProUGUI:
                    break;
            }

            if (uiDoubleState.GraphicType != EGraphicType.Unknown)
            {
                EditorGUILayout.PropertyField(_raycastChange, true);
                if (uiDoubleState.RaycastChange)
                {
                    EditorGUILayout.PropertyField(_onRaycast, true);
                    EditorGUILayout.PropertyField(_offRaycast, true);
                }
            }

            EditorGUILayout.PropertyField(_changeActive, true);
            if (uiDoubleState.ChangeActive)
            {
                EditorGUILayout.PropertyField(_onActive, true);
                EditorGUILayout.PropertyField(_offActive, true);
            }

            EditorGUILayout.PropertyField(_changeCanvasGroup, true);
            if (uiDoubleState.ChangeCanvasGroup)
            {
                EditorGUILayout.PropertyField(_canvasGroup, true);
                EditorGUILayout.PropertyField(_onSwitch, true);
                EditorGUILayout.PropertyField(_offSwitch, true);
            }

            EditorGUILayout.PropertyField(_changeInteractable, true);
            if (uiDoubleState.ChangeInteractable)
            {
                EditorGUILayout.PropertyField(_selectable, true);
                EditorGUILayout.PropertyField(_onInteractable, true);
                EditorGUILayout.PropertyField(_offInteractable, true);
            }

            // 应用修改
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}