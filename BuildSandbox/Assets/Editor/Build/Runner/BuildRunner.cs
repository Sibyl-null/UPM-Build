using System;
using System.Collections.Generic;
using System.Linq;
using Build.Editor;
using CommandLine;
using Editor.Build.Steps;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Editor.Build.Runner
{
    public static class BuildRunner
    {
        public static async void RunByEditor(BuildArgs args)
        {
            List<IBuildStep> steps = GetBuildSteps(args);
            await BuildStepRunner.Run(args, steps);
        }
        
        [UsedImplicitly]
        public static void RunByCiCd()
        {
            try
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
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorApplication.Exit(1);
            }
        }

        private static void OnParseError(IEnumerable<Error> errors)
        {
            foreach (Error error in errors)
                Debug.LogError(error.ToString());
            
            EditorApplication.Exit(1);
        }

        private static async void OnExecute(BuildArgs args)
        {
            try
            {
                List<IBuildStep> steps = GetBuildSteps(args);
                await BuildStepRunner.Run(args, steps);
                EditorApplication.Exit(0);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorApplication.Exit(1);
            }
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