using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    /// <summary>
    /// 后续根据需求修改吧
    /// </summary>
    public class ExportAndroidProject
    {
        
        public static void Export()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.LogError("current build target != Android");
                return;
            }
            
            /*
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
            buildPlayerOptions.locationPathName = Application.dataPath.Replace("/Client/Assets", "") + "/output";
            buildPlayerOptions.target = BuildTarget.Android;            
            buildPlayerOptions.options = BuildOptions.AcceptExternalModificationsToPlayer;
            if (Directory.Exists(buildPlayerOptions.locationPathName)) 
                Directory.Delete(buildPlayerOptions.locationPathName, true);
            Directory.CreateDirectory(buildPlayerOptions.locationPathName);
            
            //App Icon 不在Unity打包中设置Icon，由运营商提供icon，然后脚本替换
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/data/texture/specialtexture/jl2icon_GA.png");
            int[] iconSize = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Android);
            Texture2D[] textureArray = new Texture2D[iconSize.Length];
            for (int i = 0; i < textureArray.Length; i++)
            {
                textureArray[i] = texture;
            }
            textureArray[0] = texture;
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, textureArray);
            
            //Splash Logo
            PlayerSettings.SplashScreen.show = true;
            PlayerSettings.SplashScreen.showUnityLogo = false;
            PlayerSettings.SplashScreen.backgroundColor = Color.white;
            PlayerSettings.SplashScreen.logos = new[]
            {
                new PlayerSettings.SplashScreenLogo(){ duration = 2f, logo = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/data/texture/specialtexture/1024x576.png")},
            };
            
            AssetDatabase.SaveAssets();
            //IL2CPP
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.useAPKExpansionFiles = false; //不使用obb分离
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64; // 32位
            
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
            
            BuildPipeline.BuildPlayer(buildPlayerOptions);*/
        }
    }
}