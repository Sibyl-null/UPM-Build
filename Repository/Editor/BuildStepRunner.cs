using System.Collections.Generic;
using Build.Editor.Contexts;

namespace Build.Editor
{
    public static class BuildStepRunner
    {
        public static void Run(IBuildArgs args, List<IBuildStep> steps)
        {
            BuildContext context = new BuildContext();
            context.Set(IBuildArgs.ContextKey, args);
            args.Init();
                
            foreach (IBuildStep step in steps)
            {
                step.Init(context);
                step.Execute();
            }
        }
    }
}