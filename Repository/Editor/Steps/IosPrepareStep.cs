using System.IO;
using System.Threading.Tasks;
using Build.Editor.Contexts;
using UnityEditor;

namespace Build.Editor.Steps
{
    public class IosPrepareStep : BaseBuildStep<BaseBuildArgs>
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
            PlayerSettings.bundleVersion = Config.Version;
            PlayerSettings.iOS.buildNumber = Config.VersionCode.ToString();
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