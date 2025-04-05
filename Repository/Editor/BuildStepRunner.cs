using System.Collections.Generic;
using System.Threading.Tasks;
using Build.Editor.Configs;
using Build.Editor.Contexts;

namespace Build.Editor
{
    public static class BuildStepRunner
    {
        public static async Task Run(BaseBuildArgs args, List<IBuildStep> steps)
        {
            BuildContext context = new BuildContext();
            context.Set(BuiltinContextKey.BuildArgs, args);
            context.Set(BuiltinContextKey.BuildConfig, BuildConfig.Get(args.AppStore));
                
            foreach (IBuildStep step in steps)
            {
                step.Init(context);
                await step.Execute();
            }
        }
    }
}