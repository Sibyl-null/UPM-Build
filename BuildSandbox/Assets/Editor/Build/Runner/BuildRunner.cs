using System;
using System.Collections.Generic;
using System.Linq;
using Build.Editor;
using CommandLine;
using Editor.Build.Steps;
using JetBrains.Annotations;
using UnityEngine;

namespace Editor.Build.Runner
{
    public static class BuildRunner
    {
        public static void RunByEditor(BuildArgs args)
        {
            OnExecute(args);
        }
        
        [UsedImplicitly]
        public static void RunByCiCd()
        {
            string[] args = Environment.GetCommandLineArgs();
            int idx = Array.IndexOf(args, "--");
            if (idx < 0)
                throw new Exception("Invalid command line arguments");
                
            string[] myArgs = args.Skip(idx + 1).ToArray();
            Parser.Default.ParseArguments<BuildArgs>(myArgs)
                .WithParsed(OnExecute)
                .WithNotParsed(OnParseError);
        }

        private static void OnExecute(BuildArgs args)
        {
            List<IBuildStep> steps = GetBuildSteps(args);
            BuildStepRunner.Run(args, steps);
        }
        
        private static void OnParseError(IEnumerable<Error> errors)
        {
            foreach (Error error in errors)
                Debug.LogError(error.ToString());
            
            throw new Exception("Invalid command line arguments");
        }
        
        // ---------------------------------------------------------

        private static List<IBuildStep> GetBuildSteps(BuildArgs args)
        {
            if (args.AppStore == BuildAppStore.Google)
            {
                return new List<IBuildStep>()
                {
                    new AndroidPrepareStep(),
                    new BuildPlayerStep(),
                    new AndroidPostProcessStep()
                };
            }

            if (args.AppStore == BuildAppStore.Apple)
            {
                return new List<IBuildStep>()
                {
                    new IosPrepareStep(),
                    new BuildPlayerStep(),
                    new IosPostProcessStep()
                };
            }

            throw new Exception("Unsupported app store: " + args.AppStore);
        }
    }
}