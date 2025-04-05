using System.IO;
using System.Threading.Tasks;
using Build.Editor;
using Build.Editor.Contexts;
using Editor.Build.Runner;

namespace Editor.Build.Steps
{
    public class AndroidPostProcessStep : BaseBuildStep<BuildArgs>
    {
        public override Task Execute()
        {
            string exportPath = Context.Get<string>(BuiltinContextKey.ExportPath);
            string cachePath = Context.Get<string>(BuiltinContextKey.CachePath);
            string mappingFile = exportPath + "_mapping.txt";
            
            if (File.Exists(exportPath))
                File.Copy(exportPath, cachePath);

            if (File.Exists(mappingFile))
                File.Delete(mappingFile);

            WriteBuildInfo();
            OnCustomPostProcess();
            return Task.CompletedTask;
        }

        private void WriteBuildInfo()
        {
            string tag = Args.IsDebug ? "dev" : "rel";
            string infoStr = $"{Args.Config.AppVersion} {tag}";
            
            string exportPath = "../build/new";
            string infoPath = Path.Combine(exportPath, "BuildInfoTemp");
            File.WriteAllText(infoPath, infoStr);
        }
        
        protected virtual void OnCustomPostProcess()
        {
        }
    }
}