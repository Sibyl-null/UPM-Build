using CommandLine;
using UnityEditor;

namespace Build.Editor
{
    public enum BuildAppStore
    {
        None = 0,
        Apple,
        Google
    }
    
    public abstract class BaseBuildArgs
    {
        // Unity 命令行参数
        [Option('p')] public string ProjectPath { get; set; }
        [Option('e')] public string ExecuteMethod { get; set; }
        [Option('b')] public bool BatchMode { get; set; }
        [Option('l')] public string LogFile { get; set; }
        [Option('q')] public string Quit { get; set; }
        
        // 自定义参数
        [Option(longName: "appStore", Required = true)]
        public BuildAppStore AppStore { get; set; }
        
        
        public abstract bool IsDebug { get; }
        public abstract bool IsAppBundle { get; set; }
        
        public abstract string[] GetBuildScenes();
        public abstract BuildTarget GetBuildTarget();
        public abstract BuildOptions GetBuildOptions();
    }
}