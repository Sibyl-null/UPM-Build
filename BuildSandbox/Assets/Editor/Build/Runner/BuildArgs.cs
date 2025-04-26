using System;
using System.Linq;
using Build.Editor;
using CommandLine;
using Editor.Build.Configs;
using UnityEditor;
using UnityEngine;

namespace Editor.Build.Runner
{
    public enum BuildAppStore
    {
        Apple,
        Google
    }

    public enum BuildMode
    {
        Debug,
        Release
    }

    public class BuildArgs : IBuildArgs
    {
        [Option(longName: "appStore", Required = true)]
        public BuildAppStore AppStore { get; set; }
        
        [Option(longName: "mode", Required = true)]
        public BuildMode Mode { get; set; }

        [Option(longName: "isAppBundle", Required = false, Default = false)]
        public bool IsAppBundle { get; set; }
        
        public bool IsDebug => Mode == BuildMode.Debug;
        public BuildConfig Config { get; private set; }
        
        public void Init()
        {
            Config = BuildConfig.Get(AppStore);
            Debug.Log($"BuildArgs - AppStore: {AppStore}, Mode: {Mode}, IsAppBundle: {IsAppBundle}");
        }

        public string[] GetBuildScenes()
        {
            return EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
        }

        public BuildTarget GetBuildTarget()
        {
            return AppStore switch
            {
                BuildAppStore.Apple => BuildTarget.iOS,
                BuildAppStore.Google => BuildTarget.Android,
                _ => throw new Exception("Invalid build app store")
            };
        }

        public BuildOptions GetBuildOptions()
        {
            BuildOptions options = BuildOptions.CompressWithLz4
                                   | BuildOptions.StrictMode;

            if (IsDebug)
            {
                options |= BuildOptions.Development;
                options |= BuildOptions.ConnectWithProfiler;
            }
            
            return options;
        }
    }
}