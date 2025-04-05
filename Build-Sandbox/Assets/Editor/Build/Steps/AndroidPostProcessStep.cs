using System.IO;
using System.Threading.Tasks;
using Build.Editor;
using Editor.Build.Runner;

namespace Editor.Build.Steps
{
    public class AndroidPostProcessStep : BaseBuildStep<BuildArgs>
    {
        public override Task Execute()
        {
            string exportPath = Context.Get<string>(BuildContextKey.ExportPath);
            string cachePath = Context.Get<string>(BuildContextKey.CachePath);
            
            if (File.Exists(exportPath))
                File.Copy(exportPath, cachePath);

            return Task.CompletedTask;
        }
    }
}