using System;
using System.IO;
using Build.Editor;
using Editor.Build.Runner;
using UnityEditor;

namespace Editor.Build.Steps
{
    public class AndroidPrepareStep : BaseBuildStep<BuildArgs>
    {
        public override void Execute()
        {
            PrepareSettings();
            SetKeystore();
            CleanupBuildCacheForGradle();
            PreparePath();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void PrepareSettings()
        {
            PlayerSettings.bundleVersion = Args.Config.AppVersion;
            PlayerSettings.Android.bundleVersionCode = Args.Config.VersionCode;
            PlayerSettings.SplashScreen.showUnityLogo = false;
            
            EditorUserBuildSettings.buildAppBundle = Args.IsAppBundle;
            EditorUserBuildSettings.androidCreateSymbols =
                !Args.IsDebug ? AndroidCreateSymbols.Public : AndroidCreateSymbols.Disabled;
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
            EditorUserBuildSettings.androidETC2Fallback = AndroidETC2Fallback.Quality32BitDownscaled;
        }

        private void SetKeystore()
        {
            PlayerSettings.Android.keystoreName = "../CiCd/sandbox.keystore";
            PlayerSettings.Android.keystorePass = "123456789";
            PlayerSettings.Android.keyaliasName = "sibyl";
            PlayerSettings.Android.keyaliasPass = "123456789";
        }
        
        /** Fix the problem of incorrect cache when the native library version is changed in Unity 2021 */
        private void CleanupBuildCacheForGradle()
        {
#if UNITY_2021_3_OR_NEWER
            string cachePath = "Library/Bee/Android/Prj/IL2CPP/Gradle";
            if (Directory.Exists(cachePath))
                Directory.Delete(cachePath, true);
#endif
        }
        
        private void PreparePath()
        {
            string exportPath = "../build/new";
            string cachePath = "../build";
            
            if (Directory.Exists(exportPath))
                Directory.Delete(exportPath, true);

            Directory.CreateDirectory(exportPath);

            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);
            
            string exportName = GetExportName();
            exportPath = Path.Combine(exportPath, exportName);
            cachePath = Path.Combine(cachePath, exportName);
            
            string extension = Args.IsAppBundle ? ".aab" : ".apk";
            exportPath += extension;
            cachePath += extension;
            
            Context.Set(BuildContextKey.ExportPath, exportPath);
            Context.Set(BuildContextKey.CachePath, cachePath);
        }

        private string GetExportName()
        {
            string appName = PlayerSettings.productName.Replace(" ", "_").ToLower();
            string tag = Args.IsDebug ? "dev" : "rel";
            string time = DateTime.Now.ToString("MMdd_HHmm");
            string appStore = Args.AppStore.ToString().ToLower();
            return $"{appName}_{tag}_v{Args.Config.AppVersion}_r{Args.Config.VersionCode}_{time}_{appStore}";
        }
    }
}