using UnityEditor;

namespace Start.Editor
{
    [CustomEditor(typeof(UIAnimation), true)]
    public class UIAnimationEditor : UnityEditor.Editor
    {
        private SerializedProperty _openClip;
        private SerializedProperty _openSpeed;
        private SerializedProperty _openDelay;
        private SerializedProperty _openAwait;

        private SerializedProperty _closeClip;
        private SerializedProperty _closeSpeed;
        private SerializedProperty _closeDelay;
        
        private SerializedProperty _loopClip;
        private SerializedProperty _loopSpeed;
        private SerializedProperty _loopDelay;
        
        private void OnEnable()
        {
            _openClip = serializedObject.FindProperty("Open.Clip");
            _openSpeed = serializedObject.FindProperty("Open.Speed");
            _openDelay = serializedObject.FindProperty("Open.Delay");
            _openAwait = serializedObject.FindProperty("Open.Await");
            
            _closeClip = serializedObject.FindProperty("Close.Clip");
            _closeSpeed = serializedObject.FindProperty("Close.Speed");
            _closeDelay = serializedObject.FindProperty("Close.Delay");
            
            _loopClip = serializedObject.FindProperty("Loop.Clip");
            _loopSpeed = serializedObject.FindProperty("Loop.Speed");
            _loopDelay = serializedObject.FindProperty("Loop.Delay");
        }

        public override void OnInspectorGUI()
        {
            UIAnimation uiAnimation = target as UIAnimation;
            serializedObject.Update();
            EditorGUILayout.LabelField(UIActionTypes.Open, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_openClip);
            EditorGUILayout.PropertyField(_openSpeed);
            EditorGUILayout.PropertyField(_openDelay);
            EditorGUILayout.PropertyField(_openAwait);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(UIActionTypes.Close, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_closeClip);
            EditorGUILayout.PropertyField(_closeSpeed);
            EditorGUILayout.PropertyField(_closeDelay);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Loop", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_loopClip);
            EditorGUILayout.PropertyField(_loopSpeed);
            EditorGUILayout.PropertyField(_loopDelay);
            
            serializedObject.ApplyModifiedProperties();
        }
        
    }
}