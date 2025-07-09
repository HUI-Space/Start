using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    [CustomEditor(typeof(UIAudio), true)]
    public class UIAudioEditor : UnityEditor.Editor
    {
        private bool _initialize;
        private SerializedProperty _volume;
        private SerializedProperty _uiAudioPath;
        private SerializedProperty _playAudioOnToggleOn;
        private SerializedProperty _playAudioOnToggleOff;
        private void OnEnable()
        {
            _volume = serializedObject.FindProperty("Volume");
            _uiAudioPath = serializedObject.FindProperty("UIAudioPath");
            _playAudioOnToggleOn = serializedObject.FindProperty("PlayAudioOnToggleOn");
            _playAudioOnToggleOff = serializedObject.FindProperty("PlayAudioOnToggleOff");
            
            UIAudio uiAudio = (UIAudio)target;
            if (!_initialize)
            {
                uiAudio.Initialize();
                _initialize = true;
            }
        }

        public override void OnInspectorGUI()
        {
            UIAudio uiAudio = (UIAudio)target;
            serializedObject.Update();
            EditorGUILayout.PropertyField(_volume, true);
            EditorGUILayout.PropertyField(_uiAudioPath, true);
            if (uiAudio.InputType == EUIInputType.Toggle)
            {
                EditorGUILayout.PropertyField(_playAudioOnToggleOn, true);
                EditorGUILayout.PropertyField(_playAudioOnToggleOff, true);
            }
            // 应用修改
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnDisable()
        {
            _initialize = false;
        }
    }
}