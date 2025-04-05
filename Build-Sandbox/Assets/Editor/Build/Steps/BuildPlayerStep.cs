using System;
using System.Threading.Tasks;
using Build.Editor;
using Build.Editor.Contexts;
using Editor.Build.Runner;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Editor.Build.Steps
{
    public class BuildPlayerStep : BaseBuildStep<BuildArgs>
    {
        public override Task Execute()
        {
            string exportPath = Context.Get<string>(BuildContextKey.ExportPath);
            BuildReport buildReport = BuildPipeline.BuildPlayer(
                Args.GetBuildScenes(), 
                exportPath,
                Args.GetBuildTarget(),
                Args.GetBuildOptions());

            if (buildReport == null)
                throw new Exception("BuildPlayerStep: BuildPipeline.BuildPlayer returned null");
            
            Context.Set(BuildContextKey.BuildReport, buildReport);
            return Task.CompletedTask;
        }
    }
}