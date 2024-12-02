using UnityEditor;
using UnityEngine;

public static class EditorMenu
{
    [MenuItem("Start/打开目录/PersistentDataPath", false, 500)]
    public static void OpenPersistentDataFolder()
    {
        Application.OpenURL(Application.persistentDataPath);
    }
    
    [MenuItem("Start/打开目录/StreamingAssetsPath", false, 500)]
    public static void OpenStreamingAssetsFolder()
    {
        Application.OpenURL(Application.streamingAssetsPath);
    }
}
