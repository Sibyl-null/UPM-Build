using System.Collections.Generic;
using System.Threading.Tasks;
using Build.Editor.Contexts;

namespace Build.Editor
{
    public static class BuildStepRunner
    {
        public static async Task Run(IBuildArgs args, List<IBuildStep> steps)
        {
            BuildContext context = new BuildContext();
            context.Set(IBuildArgs.ContextKey, args);
            args.Init();
                
            foreach (IBuildStep step in steps)
            {
                step.Init(context);
                await step.Execute();
            }
        }
    }
}