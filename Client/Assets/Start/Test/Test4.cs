using System;
using UnityEditor;
using UnityEngine;

namespace Start.Test
{
    public class Test4 : MonoBehaviour
    {
        private async void Start()
        {
            try
            {
                
                print(Application.platform);
                print(EditorUserBuildSettings.activeBuildTarget);
                var a = AssetBundle.LoadFromFileAsync("");
                await a;
                print("111");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}