using System.IO;
using Build.Editor;
using Editor.Build.Runner;
using UnityEditor;

namespace Editor.Build.Steps
{
    public class IosPrepareStep : BaseBuildStep<BuildArgs>
    {
        public override void Execute()
        {
            PrepareSettings();
            SetKeystore();
            PreparePath();

            AssetDatabase.SaveAssets();
        }

        private void PrepareSettings()
        {
            PlayerSettings.bundleVersion = Args.Config.AppVersion;
            PlayerSettings.iOS.buildNumber = Args.Config.VersionCode.ToString();
            PlayerSettings.SplashScreen.showUnityLogo = false;
        }
        
        private void SetKeystore()
        {
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
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
            
            Context.Set(BuildContextKey.ExportPath, exportPath);
            Context.Set(BuildContextKey.CachePath, cachePath);
        }
        
        private string GetExportName()
        {
            return "xcode-unity";
        }
    }
}