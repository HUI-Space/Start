using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Start.Editor
{
    /// <summary>
    /// 后续根据需求修改吧
    /// </summary>
    public class ExportIOSProject
    {
        public static string packageName;
        
        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/iOS.Xcode.PBXProject.html
        /// </summary>
        public static void Export()
        { 
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
            {
                Debug.LogError("current build target != iOS");
                return;
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            BuildPlayerOptions options = new BuildPlayerOptions();
            /*
            options.target = BuildTarget.iOS;
            options.scenes = new string[] { "Assets/Launcher.unity", "Assets/Loading.unity" };
            options.options = BuildOptions.None;

            string version = File.ReadAllText(Application.streamingAssetsPath + "/version.txt");
            string time = DateTime.Now.ToString("MMddHHmm");
            packageName = "jl2-" + version + "-" + time;
            if(Directory.Exists(Application.dataPath.Replace("/Client/Assets", "") + "/IOSBuild"))
            {
                Directory.Delete(Application.dataPath.Replace("/Client/Assets", "") + "/IOSBuild", true);
            }
            options.locationPathName = Application.dataPath.Replace("/Client/Assets", "") + "/IOSBuild";

            // string[] versiones =  version.Split('.');
            // PlayerSettings.bundleVersion = 2+"."+versiones[1]+"."+versiones[2];
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.dyyfgame.kuroko.cn");
            PlayerSettings.iOS.targetOSVersionString = "12.0"; //ios最低版本
            PlayerSettings.iOS.deferSystemGesturesMode = UnityEngine.iOS.SystemGestureDeferMode.All;
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/data/texture/specialtexture/jl2icon_GA.png");
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, new Texture2D[] { icon, icon, icon, icon, icon, icon, icon, icon, icon, icon, icon, icon, icon, icon, icon, icon, icon, icon, icon }, IconKind.Any);
            */
        
            BuildPipeline.BuildPlayer(options);
        }

        /// <summary>
        /// XCODE项目发布后的处理
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xcodeProjectPath"></param>
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string xcodeProjectPath)
        {
            if (BuildTarget.iOS != target)
            {
                return;
            }
            
            string pbxProjectPath = PBXProject.GetPBXProjectPath(xcodeProjectPath);
            PBXProject  pbxProject = new PBXProject();
            pbxProject.ReadFromString(File.ReadAllText(pbxProjectPath));

            //UnityMain
            string mainGuid = pbxProject.GetUnityMainTargetGuid();
            
            //UnityFramework
            string frameworkGuid = pbxProject.GetUnityFrameworkTargetGuid();
        }
    }
}