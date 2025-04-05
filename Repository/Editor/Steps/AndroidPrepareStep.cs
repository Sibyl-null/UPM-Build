using System;
using System.IO;
using System.Threading.Tasks;
using Build.Editor.Contexts;
using UnityEditor;

namespace Build.Editor.Steps
{
    public class AndroidPrepareStep : BaseBuildStep<BaseBuildArgs>
    {
        public override Task Execute()
        {
            PrepareSettings();
            SetKeystore();
            CleanupBuildCacheForGradle();
            PreparePath();

            OnCustomPrepare();
            AssetDatabase.SaveAssets();
            return Task.CompletedTask;
        }

        protected virtual void PrepareSettings()
        {
            PlayerSettings.bundleVersion = Config.Version;
            PlayerSettings.Android.bundleVersionCode = Config.VersionCode;
            PlayerSettings.SplashScreen.showUnityLogo = false;
            
            EditorUserBuildSettings.buildAppBundle = Args.IsAppBundle;
            EditorUserBuildSettings.androidCreateSymbols =
                !Args.IsDebug ? AndroidCreateSymbols.Public : AndroidCreateSymbols.Disabled;
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
            EditorUserBuildSettings.androidETC2Fallback = AndroidETC2Fallback.Quality32BitDownscaled;
        }
        
        protected virtual void SetKeystore()
        {
            PlayerSettings.Android.keystoreName = "../jenkins/keystore/ywkj.keystore";
            PlayerSettings.Android.keystorePass = "Newstartup1";
            PlayerSettings.Android.keyaliasName = "newstartup";
            PlayerSettings.Android.keyaliasPass = "Newstartup1";
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
            
            Context.Set(BuiltinContextKey.ExportPath, exportPath);
            Context.Set(BuiltinContextKey.CachePath, cachePath);
        }
        
        protected virtual string GetExportName()
        {
            string appName = PlayerSettings.productName.Replace(" ", "_").ToLower();
            string tag = Args.IsDebug ? "dev" : "rel";
            string time = DateTime.Now.ToString("MMdd_HHmm");
            string appStore = Args.AppStore.ToString().ToLower();
            return $"{appName}_{tag}_v{Config.Version}_r{Config.VersionCode}_{time}_{appStore}";
        }

        protected virtual void OnCustomPrepare()
        {
        }
    }
}