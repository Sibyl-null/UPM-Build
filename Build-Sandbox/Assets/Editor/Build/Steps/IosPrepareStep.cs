using System.IO;
using System.Threading.Tasks;
using Build.Editor;
using Build.Editor.Contexts;
using Editor.Build.Runner;
using UnityEditor;

namespace Editor.Build.Steps
{
    public class IosPrepareStep : BaseBuildStep<BuildArgs>
    {
        public override Task Execute()
        {
            PrepareSettings();
            SetKeystore();
            PreparePath();

            OnCustomPrepare();
            AssetDatabase.SaveAssets();
            return Task.CompletedTask;
        }

        protected virtual void PrepareSettings()
        {
            PlayerSettings.bundleVersion = Args.Config.AppVersion;
            PlayerSettings.iOS.buildNumber = Args.Config.VersionCode.ToString();
            PlayerSettings.SplashScreen.showUnityLogo = false;
        }
        
        protected virtual void SetKeystore()
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
            
            Context.Set(BuiltinContextKey.ExportPath, exportPath);
            Context.Set(BuiltinContextKey.CachePath, cachePath);
        }
        
        protected virtual string GetExportName()
        {
            return "xcode-unity";
        }
        
        protected virtual void OnCustomPrepare()
        {
        }
    }
}